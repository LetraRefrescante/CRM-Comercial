using System;
using System.Web.UI.WebControls;
using CRM.Models.Filtros;
using CRM.Services;
using CRM.Web.Helpers;

namespace CRM.Web.Paginas.Notificacoes
{
    public partial class EmailHistorico : PaginaBase
    {
        private readonly EmailHistoryService _emailHistoryService = new EmailHistoryService();

        private string SortColumn
        {
            get => ViewState["SortColumn"] as string ?? "SentDate";
            set => ViewState["SortColumn"] = value;
        }

        private bool SortAscending
        {
            get => ViewState["SortAscending"] as bool? ?? false;
            set => ViewState["SortAscending"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!_emailHistoryService.PodeAcederListaGlobal(Perfil))
            {
                NotificacaoService.Erro("Não tens permissão para consultar o histórico de emails.");
                Response.Redirect("~/Dashboard/Dashboard.aspx");
                return;
            }

            if (!IsPostBack)
                CarregarHistorico();
        }

        private void CarregarHistorico()
        {
            var filtro = new EmailHistoryFiltro
            {
                Pesquisa = txtPesquisa.Text.Trim(),
                Status = ddlEstado.SelectedValue,
                DataInicio = ucFiltroDatas.DataInicial,
                DataFim = ucFiltroDatas.DataFinal
            };

            var historico = _emailHistoryService.Pesquisar(
                filtro, ucPaginacao.PaginaAtual, ucPaginacao.TamanhoPagina, out int total,
                SortColumn, SortAscending);

            ucPaginacao.TotalRegistos = total;

            rptHistorico.DataSource = historico;
            rptHistorico.DataBind();

            phVazio.Visible = historico.Count == 0;
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            ucPaginacao.PaginaAtual = 1;
            CarregarHistorico();
        }

        protected void lnkOrdenar_Command(object sender, CommandEventArgs e)
        {
            string coluna = e.CommandArgument.ToString();
            if (SortColumn == coluna) SortAscending = !SortAscending;
            else { SortColumn = coluna; SortAscending = true; }

            ucPaginacao.PaginaAtual = 1;
            CarregarHistorico();
        }

        protected void ucPaginacao_PaginaAlterada(object sender, EventArgs e) => CarregarHistorico();

        protected string GetBadgeClasse(string status) =>
            status == EmailHistoryService.StatusEnviado ? "badge-ativo" : "badge-bloqueado";
    }
}