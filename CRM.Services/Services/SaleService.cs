using CRM.Data.Repositories;
using CRM.Models.DTOs;
using CRM.Models.Entities.Catalogo;
using CRM.Models.Entities.Vendas;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CRM.Services
{
    public class SaleService
    {
        private readonly SaleRepository _saleRepository = new SaleRepository();
        private readonly PaymentRepository _paymentRepository = new PaymentRepository();
        private readonly TaxRateRepository _taxRateRepository = new TaxRateRepository();
        private readonly AuditService _auditService = new AuditService();
        private readonly PermissionService _permissionService = new PermissionService();

        private const string Modulo = "Vendas";

        public const string StatusPendente = "Pendente";
        public const string StatusConfirmada = "Confirmada";
        public const string StatusParcial = "Parcial";
        public const string StatusConcluida = "Concluída";
        public const string StatusCancelada = "Cancelada";

        public const string OrigemProposta = "Proposta";
        public const string OrigemManual = "Manual";

        // ===================== Permissões (agora tabela Permissions/RolePermissions) =====================

        public bool TemAmbitoProprios(string perfil) =>
            _permissionService.ObterNivel(perfil, Modulo) == NivelAcesso.Proprios;

        public bool PodeCriarOuEditar(string perfil) =>
            _permissionService.ObterNivel(perfil, Modulo) >= NivelAcesso.Proprios;

        public bool PodeAceder(Sale sale, int userId, string perfil)
        {
            if (sale == null) return false;
            if (!TemAmbitoProprios(perfil)) return _permissionService.ObterNivel(perfil, Modulo) >= NivelAcesso.Consulta;
            return EhDono(sale, userId);
        }

        public bool PodeEliminar(string perfil) =>
            _permissionService.ObterNivel(perfil, Modulo) == NivelAcesso.Total;

        public bool PodeCancelar(Sale sale, int userId, string perfil)
        {
            if (sale.Status == StatusCancelada) return false;

            var nivel = _permissionService.ObterNivel(perfil, Modulo);
            if (nivel == NivelAcesso.Total) return true;
            if (nivel == NivelAcesso.Proprios) return EhDono(sale, userId);
            return false;
        }

        public bool PodeRegistarPagamento(Sale sale, int userId, string perfil) =>
            sale != null
            && sale.Status != StatusCancelada
            && sale.Status != StatusPendente
            && PodeAceder(sale, userId, perfil)
            && PodeCriarOuEditar(perfil);

        private bool EhDono(Sale sale, int userId) => sale.OwnerId == userId;

        // ===================== Edição direta =====================
        public bool PodeEditarDiretamente(Sale sale) =>
            sale.Status == StatusPendente || sale.Status == StatusConfirmada;

        // ===================== Validação =====================

        public List<string> Validar(Sale sale)
        {
            var erros = new List<string>();

            if (sale.ClientId <= 0)
                erros.Add("O cliente é obrigatório.");

            if (sale.OwnerId <= 0)
                erros.Add("O comercial responsável é obrigatório.");

            if (string.IsNullOrWhiteSpace(sale.Origin))
                erros.Add("A origem (Proposta ou Manual) é obrigatória.");

            if (sale.Origin == OrigemProposta && !sale.ProposalId.HasValue)
                erros.Add("Uma venda de origem \"Proposta\" tem de referenciar a proposta de origem.");

            erros.AddRange(ValidarLinhas(sale));

            return erros;
        }

        public List<string> ValidarLinhas(Sale sale)
        {
            var erros = new List<string>();

            if (sale.Lines == null || !sale.Lines.Any())
            {
                erros.Add("A venda tem de ter pelo menos uma linha.");
                return erros;
            }

            foreach (var linha in sale.Lines)
            {
                if (linha.Quantity <= 0)
                    erros.Add($"A quantidade da linha \"{linha.Description}\" tem de ser superior a zero.");

                if (linha.DiscountPercent < 0 || linha.DiscountPercent > 100)
                    erros.Add($"O desconto da linha \"{linha.Description}\" tem de estar entre 0 e 100.");
            }

            return erros;
        }

        public List<string> ValidarCancelamento(string motivo)
        {
            var erros = new List<string>();

            if (string.IsNullOrWhiteSpace(motivo))
                erros.Add("O motivo de cancelamento é obrigatório.");

            return erros;
        }

        // ===================== Cálculo de totais =====================

        public void CalcularTotais(Sale sale)
        {
            var taxasIva = _taxRateRepository.ListarTodas();

            decimal subTotal = 0;
            decimal taxTotal = 0;

            foreach (var linha in sale.Lines)
            {
                linha.LineTotal = Math.Round(linha.Quantity * linha.UnitPrice * (1 - linha.DiscountPercent / 100m), 2);
                subTotal += linha.LineTotal;

                var taxa = taxasIva.SingleOrDefault(t => t.TaxRateId == linha.TaxRateId);
                decimal percentagem = taxa?.Percentage ?? 0;
                taxTotal += Math.Round(linha.LineTotal * (percentagem / 100m), 2);
            }

            sale.SubTotal = subTotal;
            sale.TaxTotal = taxTotal;
            sale.Total = subTotal + taxTotal;
        }

        // ===================== Criar a partir de Proposta =====================

        public Sale MontarAPartirDeProposta(Proposal proposal)
        {
            return new Sale
            {
                ClientId = proposal.ClientId,
                ProposalId = proposal.ProposalId,
                Origin = OrigemProposta,
                SaleDate = DateTime.Today,
                SubTotal = proposal.SubTotal,
                TaxTotal = proposal.TaxTotal,
                Total = proposal.Total,
                Lines = proposal.Lines.Select(l => new SaleLine
                {
                    ProductId = l.ProductId,
                    LineOrder = l.LineOrder,
                    Description = l.Description,
                    Quantity = l.Quantity,
                    UnitPrice = l.UnitPrice,
                    DiscountPercent = l.DiscountPercent,
                    TaxRateId = l.TaxRateId,
                    LineTotal = l.LineTotal
                }).ToList()
            };
        }

        // ===================== Consulta / Listagem =====================

        public Sale GetById(int saleId) => _saleRepository.GetById(saleId);

        public bool ExisteVendaParaProposta(int proposalId) => _saleRepository.ExisteVendaParaProposta(proposalId);

        public List<Sale> Listar(
            string pesquisa, string status, int? clientId, int? ownerId,
            DateTime? dataInicio, DateTime? dataFim,
            int pagina, int tamanhoPagina, out int totalRegistos,
            string sortColumn, bool sortAscending)
            => _saleRepository.Listar(pesquisa, status, clientId, ownerId, dataInicio, dataFim,
                pagina, tamanhoPagina, out totalRegistos, sortColumn, sortAscending);

        public List<Sale> ListarParaSelecao(string perfil, int userId)
        {
            int? ownerId = TemAmbitoProprios(perfil) ? userId : (int?)null;
            return _saleRepository.ListarParaSelecao(ownerId);
        }

        public List<RelatorioComissoesLinha> ObterRelatorioComissoes(
            DateTime dataInicio, DateTime dataFim, int? ownerId)
        {
            var vendas = _saleRepository.ListarParaRelatorio(dataInicio, dataFim, null, ownerId, new List<string> { "Concluída" });

            return vendas
                .GroupBy(v => v.Owner?.Name ?? "(sem comercial)")
                .OrderByDescending(g => g.Sum(v => v.CommissionValue ?? 0))
                .Select(g => new RelatorioComissoesLinha
                {
                    Comercial = g.Key,
                    QuantidadeVendas = g.Count(),
                    TotalVendas = g.Sum(v => v.Total),
                    TotalComissao = g.Sum(v => v.CommissionValue ?? 0)
                })
                .ToList();
        }

        // ===================== Gravação =====================

        public Sale Criar(Sale sale, int userId)
        {
            sale.CreatedBy = userId;
            sale.CreatedDate = DateTime.UtcNow;
            sale.Status = StatusPendente;
            CalcularTotais(sale);
            var criada = _saleRepository.Criar(sale);

            _auditService.Registar(userId, "Criar", "Sale", criada.SaleId.ToString());

            return criada;
        }

        public void Atualizar(Sale sale, int userId)
        {
            sale.UpdatedBy = userId;
            sale.UpdatedDate = DateTime.UtcNow;
            CalcularTotais(sale);
            _saleRepository.Atualizar(sale);

            _auditService.Registar(userId, "Editar", "Sale", sale.SaleId.ToString());
        }

        public bool Cancelar(int saleId, string motivo, int userId, string perfil)
        {
            var sale = _saleRepository.GetById(saleId);
            if (sale == null) return false;
            if (!PodeCancelar(sale, userId, perfil)) return false;

            _saleRepository.AtualizarEstado(saleId, StatusCancelada, motivo, userId);
            _auditService.Registar(userId, "Cancelar", "Sale", saleId.ToString(), motivo);

            return true;
        }

        public bool Eliminar(int saleId, int userId, string perfil)
        {
            if (!PodeEliminar(perfil)) return false;

            var sale = _saleRepository.GetById(saleId);
            if (sale == null) return false;

            _saleRepository.EliminarLogico(saleId, userId);
            _auditService.Registar(userId, "Eliminar", "Sale", saleId.ToString());

            return true;
        }

        public bool ConfirmarManualmente(int saleId, int userId, string perfil)
        {
            var sale = _saleRepository.GetById(saleId);
            if (sale == null || sale.Status != StatusPendente) return false;
            if (!PodeAceder(sale, userId, perfil) || !PodeCriarOuEditar(perfil)) return false;

            _saleRepository.AtualizarEstado(saleId, StatusConfirmada, null, userId);
            _auditService.Registar(userId, "Confirmar", "Sale", saleId.ToString());

            return true;
        }

        // ===================== Estado financeiro =====================
        public void RecalcularEstadoFinanceiro(int saleId, int userId)
        {
            var sale = _saleRepository.GetById(saleId);
            if (sale == null || sale.Status == StatusCancelada) return;

            if (sale.Status == StatusPendente) return;

            decimal totalPago = _paymentRepository.TotalPagoPorVenda(saleId);

            string novoStatus =
                totalPago <= 0 ? StatusConfirmada :
                totalPago >= sale.Total ? StatusConcluida :
                StatusParcial;

            if (novoStatus != sale.Status)
                _saleRepository.AtualizarEstado(saleId, novoStatus, sale.CancellationReason, userId);
        }

        // ===================== Relatorio =====================
        public RelatorioVendasResultado ObterRelatorio(
        DateTime dataInicio, DateTime dataFim, int? clientId, int? ownerId, List<string> estados, string agrupamento)
        {
            var vendas = _saleRepository.ListarParaRelatorio(dataInicio, dataFim, clientId, ownerId, estados);

            var linhas = vendas
                .GroupBy(v => ChavePeriodo(v.SaleDate, agrupamento))
                .OrderBy(g => g.Key)
                .Select(g => new RelatorioVendasLinha
                {
                    Periodo = g.Key,
                    Quantidade = g.Count(),
                    SubTotal = g.Sum(v => v.SubTotal),
                    TaxTotal = g.Sum(v => v.TaxTotal),
                    Total = g.Sum(v => v.Total)
                })
                .ToList();

            return new RelatorioVendasResultado
            {
                Linhas = linhas,
                QuantidadeGeral = vendas.Count,
                TotalGeral = vendas.Sum(v => v.Total)
            };
        }

        private string ChavePeriodo(DateTime data, string agrupamento)
        {
            switch (agrupamento)
            {
                case "Dia": return data.ToString("yyyy-MM-dd");
                case "Trimestre": return $"{data.Year}-T{(data.Month - 1) / 3 + 1}";
                case "Ano": return data.Year.ToString();
                default: return data.ToString("yyyy-MM"); // "Mes"
            }
        }
    }
}