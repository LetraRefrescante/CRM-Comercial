using System;
using System.Collections.Generic;
using System.Linq;
using CRM.Data.Repositories;
using CRM.Models.Entities.Atividades;
using CRM.Models.Entities.Catalogo;

namespace CRM.Services
{
    public class ProposalService
    {
        private readonly ProposalRepository _proposalRepository = new ProposalRepository();
        private readonly TaxRateRepository _taxRateRepository = new TaxRateRepository();
        private readonly AuditService _auditService = new AuditService();
        private readonly ActivityService _activityService = new ActivityService();
        private readonly PermissionService _permissionService = new PermissionService();

        private const string Modulo = "Propostas";

        public const string StatusRascunho = "Rascunho";
        public const string StatusEnviada = "Enviada";
        public const string StatusAceite = "Aceite";
        public const string StatusRecusada = "Recusada";
        public const string StatusExpirada = "Expirada";
        public const string StatusCancelada = "Cancelada";

        // ===================== Permissões (tabela Permissions/RolePermissions) =====================

        public bool TemAmbitoProprios(string perfil) =>
            _permissionService.ObterNivel(perfil, Modulo) == NivelAcesso.Proprios;

        public bool PodeCriarOuEditar(string perfil) =>
            _permissionService.ObterNivel(perfil, Modulo) >= NivelAcesso.Proprios;

        public bool PodeEliminar(Proposal proposal, int userId, string perfil)
        {
            var nivel = _permissionService.ObterNivel(perfil, Modulo);
            if (nivel == NivelAcesso.Total) return true;
            if (nivel == NivelAcesso.Proprios) return EhDono(proposal, userId);
            return false;
        }

        // Valida âmbito "próprios" ao aceder a uma proposta específica por Id.
        // ASSUNÇÃO: "próprios" segue o comercial responsável do Cliente (Client.AccountManagerId),
        // o mesmo critério já usado no filtro de listagem (PropostasLista.aspx.cs → ObterFiltroComercial).
        //
        // Alterado de "return true" para "nível >= Consulta" no ramo sem âmbito próprios — mesma
        // correção aplicada no SaleService: um perfil sem qualquer permissão de Propostas (nível
        // Nenhum) fica corretamente bloqueado, em vez de ver tudo por omissão. Não muda o
        // comportamento de nenhum dos 5 perfis atuais (todos têm pelo menos Consulta).
        public bool PodeAceder(Proposal proposal, int userId, string perfil)
        {
            if (proposal == null) return false;
            if (!TemAmbitoProprios(perfil)) return _permissionService.ObterNivel(perfil, Modulo) >= NivelAcesso.Consulta;
            return EhDono(proposal, userId);
        }

        private bool EhDono(Proposal proposal, int userId) =>
            proposal?.Client != null && proposal.Client.AccountManagerId == userId;

        // ===================== Edição direta vs. Nova Versão =====================
        // ASSUNÇÃO: só uma proposta em Rascunho pode ser editada diretamente.
        // Qualquer alteração a uma proposta já Enviada/Aceite/Recusada/etc. obriga a criar nova versão.

        public bool PodeEditarDiretamente(Proposal proposal) => proposal.Status == StatusRascunho;

        public bool PodeCriarNovaVersao(Proposal proposal, string perfil) =>
            PodeCriarOuEditar(perfil) && proposal.Status != StatusRascunho;

        // ===================== Validação (cabeçalho) =====================

        public List<string> Validar(Proposal proposal)
        {
            var erros = new List<string>();

            if (proposal.ClientId <= 0)
                erros.Add("O cliente é obrigatório.");

            if (proposal.ValidUntil < proposal.IssueDate)
                erros.Add("A validade não pode ser anterior à data de emissão.");

            if (proposal.GlobalDiscountPercent < 0 || proposal.GlobalDiscountPercent > 100)
                erros.Add("O desconto global tem de estar entre 0 e 100.");

            erros.AddRange(ValidarLinhas(proposal));

            return erros;
        }

        public List<string> ValidarLinhas(Proposal proposal)
        {
            var erros = new List<string>();

            if (proposal.Lines == null || !proposal.Lines.Any())
            {
                erros.Add("A proposta tem de ter pelo menos uma linha.");
                return erros;
            }

            foreach (var linha in proposal.Lines)
            {
                if (linha.Quantity <= 0)
                    erros.Add($"A quantidade da linha \"{linha.Description}\" tem de ser superior a zero.");

                if (linha.DiscountPercent < 0 || linha.DiscountPercent > 100)
                    erros.Add($"O desconto da linha \"{linha.Description}\" tem de estar entre 0 e 100.");
            }

            return erros;
        }


        // ===================== Cálculo de totais =====================

        public void CalcularTotais(Proposal proposal)
        {
            var taxasIva = _taxRateRepository.ListarTodas();

            decimal subTotalBruto = 0;

            foreach (var linha in proposal.Lines)
            {
                linha.LineTotal = Math.Round(linha.Quantity * linha.UnitPrice * (1 - linha.DiscountPercent / 100m), 2);
                subTotalBruto += linha.LineTotal;
            }

            decimal fatorDescontoGlobal = 1 - (proposal.GlobalDiscountPercent / 100m);
            decimal subTotalComDescontoGlobal = Math.Round(subTotalBruto * fatorDescontoGlobal, 2);

            decimal taxTotal = 0;
            foreach (var linha in proposal.Lines)
            {
                decimal proporcao = subTotalBruto == 0 ? 0 : linha.LineTotal / subTotalBruto;
                decimal baseComDesconto = subTotalComDescontoGlobal * proporcao;
                var taxa = taxasIva.SingleOrDefault(t => t.TaxRateId == linha.TaxRateId);
                decimal percentagem = taxa?.Percentage ?? 0;
                taxTotal += Math.Round(baseComDesconto * (percentagem / 100m), 2);
            }

            proposal.SubTotal = subTotalComDescontoGlobal;
            proposal.TaxTotal = taxTotal;
            proposal.Total = subTotalComDescontoGlobal + taxTotal;
        }

        // ===================== Consulta / Listagem =====================

        public Proposal GetById(int proposalId) => _proposalRepository.GetById(proposalId);

        public List<Proposal> Listar(
            string pesquisa, string status, int? clientId, int? accountManagerId,
            DateTime? dataInicio, DateTime? dataFim,
            int pagina, int tamanhoPagina, out int totalRegistos,
            string sortColumn, bool sortAscending)
            => _proposalRepository.Listar(pesquisa, status, clientId, accountManagerId, dataInicio, dataFim,
                pagina, tamanhoPagina, out totalRegistos, sortColumn, sortAscending);

        public List<Proposal> ListarVersoes(int proposalId) => _proposalRepository.ListarVersoes(proposalId);
        public List<Proposal> ListarPorOportunidade(int opportunityId) =>
            _proposalRepository.ListarPorOportunidade(opportunityId);

        // ===================== Gravação =====================

        public Proposal Criar(Proposal proposal, int userId)
        {
            proposal.CreatedBy = userId;
            proposal.Status = StatusRascunho;
            proposal.VersionNumber = 1;
            CalcularTotais(proposal);
            var criada = _proposalRepository.Criar(proposal);

            _auditService.Registar(userId, "Criar", "Proposal", criada.ProposalId.ToString());

            return criada;
        }

        public void Atualizar(Proposal proposal, int userId)
        {
            proposal.UpdatedBy = userId;
            proposal.UpdatedDate = DateTime.UtcNow;
            CalcularTotais(proposal);
            _proposalRepository.Atualizar(proposal);

            _auditService.Registar(userId, "Editar", "Proposal", proposal.ProposalId.ToString());
        }

        public Proposal CriarNovaVersao(int proposalId, int userId)
        {
            var novaVersao = _proposalRepository.CriarNovaVersao(proposalId, userId);

            if (novaVersao != null)
                _auditService.Registar(userId, "CriarNovaVersao", "Proposal", novaVersao.ProposalId.ToString(),
                    $"Nova versão a partir da proposta {proposalId}");

            return novaVersao;
        }

        public bool Eliminar(int proposalId, int userId, string perfil)
        {
            var proposal = _proposalRepository.GetById(proposalId);
            if (proposal == null) return false;

            if (!PodeEliminar(proposal, userId, perfil)) return false;

            _proposalRepository.EliminarLogico(proposalId, userId);
            _auditService.Registar(userId, "Eliminar", "Proposal", proposalId.ToString());

            return true;
        }

        // ===================== Envio / Aceitação / Recusa / Expiração =====================

        public bool PodeEnviar(Proposal proposal, int userId, string perfil) =>
            proposal.Status == StatusRascunho && PodeAceder(proposal, userId, perfil) && PodeCriarOuEditar(perfil);

        public bool PodeAceitarOuRecusar(Proposal proposal, int userId, string perfil) =>
            proposal.Status == StatusEnviada && PodeAceder(proposal, userId, perfil) && PodeCriarOuEditar(perfil);

        public List<string> ValidarEnvio(string email)
        {
            var erros = new List<string>();

            if (string.IsNullOrWhiteSpace(email))
                erros.Add("O email do destinatário é obrigatório.");
            else if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                erros.Add("O email do destinatário não tem um formato válido.");

            return erros;
        }

        public bool Enviar(int proposalId, string email, int userId, string perfil)
        {
            var proposal = _proposalRepository.GetById(proposalId);
            if (proposal == null) return false;
            if (!PodeEnviar(proposal, userId, perfil)) return false;

            _proposalRepository.RegistarEnvio(proposalId, email, userId);
            _auditService.Registar(userId, "Enviar", "Proposal", proposalId.ToString(), $"Enviada para {email}");

            // Regra: "Envio por email gera atividade e guarda destinatários." O registo da
            // atividade é suplementar — uma falha aqui não deve impedir o envio da proposta.
            try
            {
                _activityService.Criar(new Activity
                {
                    Type = "Email",
                    Subject = $"Proposta {proposal.ProposalNumber} enviada para {email}",
                    RelatedClientId = proposal.ClientId,
                    AssignedToUserId = userId,
                    StartDateTime = DateTime.Now,
                    Status = "Concluída",
                    CompletedDateTime = DateTime.UtcNow,
                    Description = $"Envio da proposta {proposal.ProposalNumber} (v{proposal.VersionNumber})."
                }, userId, perfil);
            }
            catch
            {
                // Não bloqueia o envio da proposta se o registo da atividade falhar.
            }

            return true;
        }

        public bool Aceitar(int proposalId, string observacao, int userId, string perfil)
        {
            var proposal = _proposalRepository.GetById(proposalId);
            if (proposal == null) return false;
            if (!PodeAceitarOuRecusar(proposal, userId, perfil)) return false;

            _proposalRepository.RegistarAceitacao(proposalId, userId, string.IsNullOrWhiteSpace(observacao) ? null : observacao.Trim());
            _auditService.Registar(userId, "Aceitar", "Proposal", proposalId.ToString(), observacao);

            return true;
        }

        public bool Recusar(int proposalId, string motivo, int userId, string perfil)
        {
            var proposal = _proposalRepository.GetById(proposalId);
            if (proposal == null) return false;
            if (!PodeAceitarOuRecusar(proposal, userId, perfil)) return false;

            _proposalRepository.AtualizarStatus(proposalId, StatusRecusada, userId);

            // Nota: não existe coluna própria para motivo de recusa em Proposals (só existe
            // em Oportunidades). O motivo fica registado apenas no AuditLog por agora.
            _auditService.Registar(userId, "Recusar", "Proposal", proposalId.ToString(), motivo);

            return true;
        }

        public int MarcarExpiradas(int userId)
        {
            int total = _proposalRepository.MarcarExpiradas();

            if (total > 0)
                _auditService.Registar(userId, "MarcarExpiradas", "Proposal", "lote", $"{total} proposta(s) marcada(s) como expirada(s).");

            return total;
        }
    }
}