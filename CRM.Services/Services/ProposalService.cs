using System;
using System.Collections.Generic;
using System.Linq;
using CRM.Data.Repositories;
using CRM.Models.Entities.Catalogo;

namespace CRM.Services
{
    public class ProposalService
    {
        private readonly ProposalRepository _proposalRepository = new ProposalRepository();
        private readonly TaxRateRepository _taxRateRepository = new TaxRateRepository();
        private readonly AuditService _auditService = new AuditService();

        public const string StatusRascunho = "Rascunho";
        public const string StatusEnviada = "Enviada";
        public const string StatusAceite = "Aceite";
        public const string StatusRecusada = "Recusada";
        public const string StatusExpirada = "Expirada";
        public const string StatusCancelada = "Cancelada";

        // ===================== Permissões (matriz do blueprint) =====================

        public bool TemAmbitoProprios(string perfil) => perfil == "Comercial";

        public bool PodeCriarOuEditar(string perfil) =>
            perfil == "Administrador" || perfil == "Diretor" || perfil == "Comercial";

        // ASSUNÇÃO: a blueprint diz "Comercial = PRÓPRIOS" em Propostas — inclui eliminar as suas
        // próprias, não só Administrador/Diretor (que têm TOTAL). Se eliminar deve ficar reservado
        // só a quem tem TOTAL, troca a linha do Comercial para "return false;". Ainda por confirmar.
        public bool PodeEliminar(Proposal proposal, int userId, string perfil)
        {
            if (perfil == "Administrador" || perfil == "Diretor") return true;
            if (perfil == "Comercial") return EhDono(proposal, userId);
            return false;
        }

        // Valida âmbito "próprios" ao aceder a uma proposta específica por Id.
        // ASSUNÇÃO: "próprios" segue o comercial responsável do Cliente (Client.AccountManagerId),
        // o mesmo critério já usado no filtro de listagem (PropostasLista.aspx.cs → ObterFiltroComercial).
        public bool PodeAceder(Proposal proposal, int userId, string perfil)
        {
            if (proposal == null) return false;
            if (!TemAmbitoProprios(perfil)) return true;
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
    }
}