using System;
using System.Linq;
using CRM.Services;
using CRM.Web.Helpers;

namespace CRM.Web.Paginas.Vendas
{
    public partial class VendaDetalhe : PaginaBase
    {
        private readonly SaleService _saleService = new SaleService();
        private readonly PaymentService _paymentService = new PaymentService();

        private int? SaleId => int.TryParse(Request.QueryString["id"], out int id) ? id : (int?)null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SaleId.HasValue)
            {
                Response.Redirect("VendasLista.aspx");
                return;
            }

            if (!IsPostBack)
                CarregarVenda();
        }

        private void CarregarVenda()
        {
            var sale = _saleService.GetById(SaleId.Value);
            if (sale == null)
            {
                NotificacaoService.Erro("Venda não encontrada.");
                Response.Redirect("VendasLista.aspx");
                return;
            }

            if (!_saleService.PodeAceder(sale, UserId, Perfil))
            {
                NotificacaoService.Erro("Não tens permissão para aceder a esta venda.");
                Response.Redirect("VendasLista.aspx");
                return;
            }

            lblNumero.Text = sale.SaleNumber;
            spanStatus.InnerText = sale.Status;
            spanStatus.Attributes["class"] = "badge " + GetBadgeClasse(sale.Status);

            lblCliente.Text = sale.Client?.TradeName;
            lblData.Text = sale.SaleDate.ToString("dd/MM/yyyy");
            lblComercial.Text = sale.Owner?.Name ?? "—";
            lblOrigem.Text = sale.Origin;

            phProposta.Visible = sale.ProposalId.HasValue;
            if (sale.ProposalId.HasValue)
            {
                lnkProposta.Text = sale.Proposal?.ProposalNumber ?? ("#" + sale.ProposalId);
                lnkProposta.NavigateUrl = $"~/Catalogo/PropostaDetalhe.aspx?id={sale.ProposalId}";
            }

            lblMetodoPagamento.Text = sale.PaymentMethod ?? "—";
            lblDataVencimento.Text = sale.DueDate?.ToString("dd/MM/yyyy") ?? "—";
            lblComissao.Text = sale.CommissionValue.HasValue ? sale.CommissionValue.Value.ToString("C") : "—";

            phMotivoCancelamento.Visible = sale.Status == SaleService.StatusCancelada;
            lblMotivoCancelamento.Text = sale.CancellationReason;

            rptLinhas.DataSource = sale.Lines;
            rptLinhas.DataBind();

            lblSubTotal.Text = sale.SubTotal.ToString("C");
            lblIvaTotal.Text = sale.TaxTotal.ToString("C");
            lblTotalGeral.Text = sale.Total.ToString("C");

            // ===================== Pagamentos =====================
            var pagamentos = _paymentService.ListarPorVenda(sale.SaleId);
            rptPagamentos.DataSource = pagamentos;
            rptPagamentos.DataBind();
            phSemPagamentos.Visible = pagamentos.Count == 0;

            decimal totalPago = _paymentService.TotalPago(sale.SaleId);
            lblTotalPago.Text = totalPago.ToString("C");
            lblSaldo.Text = (sale.Total - totalPago).ToString("C");

            bool podeRegistarPagamento = _saleService.PodeRegistarPagamento(sale, UserId, Perfil);
            lnkGerirPagamentos.Visible = podeRegistarPagamento;
            lnkGerirPagamentos.NavigateUrl = $"Pagamentos.aspx?saleId={sale.SaleId}";

            // ===================== Ações de ciclo de vida =====================
            lnkEditar.Visible = _saleService.PodeEditarDiretamente(sale)
                && _saleService.PodeCriarOuEditar(Perfil)
                && _saleService.PodeAceder(sale, UserId, Perfil);
            lnkEditar.NavigateUrl = $"VendaEditar.aspx?id={sale.SaleId}";

            btnConfirmar.Visible = sale.Status == SaleService.StatusPendente
                && _saleService.PodeAceder(sale, UserId, Perfil)
                && _saleService.PodeCriarOuEditar(Perfil);

            phCancelar.Visible = _saleService.PodeCancelar(sale, UserId, Perfil);

            // ===================== Anexos e histórico =====================
            ucAnexos.Inicializar("Sale", sale.SaleId, UserId);
            ucHistorico.Inicializar("Sale", sale.SaleId.ToString());
        }

        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            if (_saleService.ConfirmarManualmente(SaleId.Value, UserId, Perfil))
                NotificacaoService.Sucesso("Venda confirmada.");
            else
                NotificacaoService.Erro("Não foi possível confirmar esta venda.");

            CarregarVenda();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            var erros = _saleService.ValidarCancelamento(txtMotivoCancelamento.Text.Trim());
            if (erros.Any())
            {
                NotificacaoService.Erro(string.Join(" ", erros));
                return;
            }

            if (_saleService.Cancelar(SaleId.Value, txtMotivoCancelamento.Text.Trim(), UserId, Perfil))
                NotificacaoService.Sucesso("Venda cancelada.");
            else
                NotificacaoService.Erro("Não foi possível cancelar esta venda.");

            CarregarVenda();
        }

        protected string GetBadgeClasse(string status)
        {
            switch (status)
            {
                case SaleService.StatusPendente: return "bg-secondary";
                case SaleService.StatusConfirmada: return "badge-em-contacto";
                case SaleService.StatusParcial: return "bg-warning text-dark";
                case SaleService.StatusConcluida: return "badge-ativo";
                case SaleService.StatusCancelada: return "badge-bloqueado";
                default: return "bg-secondary";
            }
        }
    }
}