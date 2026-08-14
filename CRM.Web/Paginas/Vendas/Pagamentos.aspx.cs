using System;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using CRM.Models.Entities.Vendas;
using CRM.Services;
using CRM.Web.Helpers;

namespace CRM.Web.Paginas.Vendas
{
    public partial class Pagamentos : PaginaBase
    {
        private readonly SaleService _saleService = new SaleService();
        private readonly PaymentService _paymentService = new PaymentService();

        private int? SaleId => int.TryParse(Request.QueryString["saleId"], out int id) ? id : (int?)null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SaleId.HasValue)
            {
                Response.Redirect("VendasLista.aspx");
                return;
            }

            if (!IsPostBack)
                CarregarPagina();
        }

        private void CarregarPagina()
        {
            var sale = _saleService.GetById(SaleId.Value);
            if (sale == null)
            {
                NotificacaoService.Erro("Venda não encontrada.");
                Response.Redirect("VendasLista.aspx");
                return;
            }

            if (!_saleService.PodeRegistarPagamento(sale, UserId, Perfil))
            {
                NotificacaoService.Erro("Não tens permissão para gerir pagamentos desta venda.");
                Response.Redirect($"VendaDetalhe.aspx?id={SaleId}");
                return;
            }

            lblNumero.Text = sale.SaleNumber;
            spanStatus.InnerText = sale.Status;

            // Estados encerrados (Concluída/Cancelada) continuam a mostrar o histórico,
            // mas escondem o formulário — não faz sentido adicionar pagamentos depois disso.
            phFormulario.Visible = sale.Status != SaleService.StatusCancelada && sale.Status != SaleService.StatusConcluida;

            AtualizarResumo(sale);
            CarregarPagamentos();
        }

        private void AtualizarResumo(Sale sale)
        {
            decimal totalPago = _paymentService.TotalPago(sale.SaleId);
            lblTotalVenda.Text = sale.Total.ToString("C");
            lblTotalPago.Text = totalPago.ToString("C");
            lblSaldo.Text = (sale.Total - totalPago).ToString("C");
        }

        private void CarregarPagamentos()
        {
            var pagamentos = _paymentService.ListarPorVenda(SaleId.Value);
            rptPagamentos.DataSource = pagamentos;
            rptPagamentos.DataBind();
            phVazio.Visible = pagamentos.Count == 0;
        }

        protected void btnRegistar_Click(object sender, EventArgs e)
        {
            decimal.TryParse(txtValor.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out decimal valor);
            DateTime.TryParse(txtData.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime data);

            var payment = new Payment
            {
                SaleId = SaleId.Value,
                Amount = valor,
                PaymentDate = data,
                PaymentMethod = string.IsNullOrEmpty(ddlMetodo.SelectedValue) ? null : ddlMetodo.SelectedValue,
                Reference = string.IsNullOrWhiteSpace(txtReferencia.Text) ? null : txtReferencia.Text.Trim(),
                Notes = string.IsNullOrWhiteSpace(txtNotas.Text) ? null : txtNotas.Text.Trim()
            };

            var erros = _paymentService.Validar(payment);
            if (erros.Any())
            {
                NotificacaoService.Erro(string.Join(" ", erros));
                return;
            }

            _paymentService.Registar(payment, UserId);
            NotificacaoService.Sucesso("Pagamento registado.");

            txtValor.Text = "";
            txtData.Text = "";
            ddlMetodo.SelectedIndex = 0;
            txtReferencia.Text = "";
            txtNotas.Text = "";

            CarregarPagina();
        }

        protected void rptPagamentos_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Eliminar") return;

            int paymentId = int.Parse(e.CommandArgument.ToString());
            _paymentService.Eliminar(paymentId, SaleId.Value, UserId);

            NotificacaoService.Sucesso("Pagamento eliminado.");
            CarregarPagina();
        }
    }
}