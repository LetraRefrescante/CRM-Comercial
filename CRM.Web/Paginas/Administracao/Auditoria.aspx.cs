using CRM.Data.Repositories;
using CRM.Web.Controls;
using CRM.Web.Helpers;
using System;
using System.Web.UI.WebControls;

namespace CRM.Web.Paginas.Administracao
{
    public partial class Auditoria : PaginaBase
    {
        private readonly AuditLogRepository _auditLogRepository = new AuditLogRepository();
        private readonly UserRepository _userRepository = new UserRepository();

        private bool PodeConsultar => Perfil == "Administrador" || Perfil == "Diretor";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!PodeConsultar)
            {
                NotificacaoService.Erro("Não tens permissão para aceder a esta página.");
                Response.Redirect("~/Dashboard/Dashboard.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CarregarUtilizadores();
                CarregarAcoes();
                CarregarLogs();
            }
        }

        private void CarregarUtilizadores()
        {
            ddlUtilizador.Items.Add(new ListItem("Todos", ""));
            foreach (var user in _userRepository.Listar())
            {
                ddlUtilizador.Items.Add(new ListItem(user.Name, user.UserId.ToString()));
            }
        }

        private void CarregarAcoes()
        {
            ddlAcao.Items.Add(new ListItem("Todas", ""));
            foreach (var acao in _auditLogRepository.ListarAcoesDistintas())
            {
                ddlAcao.Items.Add(new ListItem(acao, acao));
            }
        }

        private void CarregarLogs()
        {
            int? userId = string.IsNullOrEmpty(ddlUtilizador.SelectedValue) ? (int?)null : int.Parse(ddlUtilizador.SelectedValue);
            string acao = string.IsNullOrEmpty(ddlAcao.SelectedValue) ? null : ddlAcao.SelectedValue;
            string entidade = txtEntidade.Text.Trim();

            DateTime? dataInicial = DateTime.TryParse(txtDataInicial.Text, out DateTime di) ? di : (DateTime?)null;
            DateTime? dataFinal = DateTime.TryParse(txtDataFinal.Text, out DateTime df) ? df : (DateTime?)null;

            var logs = _auditLogRepository.Listar(
                userId, acao, string.IsNullOrEmpty(entidade) ? null : entidade,
                dataInicial, dataFinal,
                ucPaginacao.PaginaAtual, ucPaginacao.TamanhoPagina, out int total);

            ucPaginacao.TotalRegistos = total;

            rptLogs.DataSource = logs;
            rptLogs.DataBind();

            phVazio.Visible = logs.Count == 0;
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            ucPaginacao.PaginaAtual = 1;
            CarregarLogs();
        }

        protected void ucPaginacao_PaginaAlterada(object sender, EventArgs e)
        {
            CarregarLogs();
        }
    }
}