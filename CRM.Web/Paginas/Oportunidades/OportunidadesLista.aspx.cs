using System;
using System.Linq;
using CRM.Data.Repositories;
using CRM.Models.Entities.Oportunidades;
using CRM.Services;

namespace CRM.Web.Oportunidades
{
    public partial class OportunidadesLista : System.Web.UI.Page
    {
        private readonly OpportunityService _opportunityService = new OpportunityService();
        private readonly OpportunityStageRepository _stageRepository = new OpportunityStageRepository();
        private readonly UserRepository _userRepository = new UserRepository();

        private string Perfil => Session["RoleName"] as string ?? "";
        private int UserId => Session["UserId"] != null ? (int)Session["UserId"] : 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lnkNova.Visible = _opportunityService.PodeEditar(Perfil);
                phFiltroComercial.Visible = !_opportunityService.TemAmbitoProprios(Perfil);

                CarregarDropDowns();
                Pesquisar();
            }
        }

        private void CarregarDropDowns()
        {
            var fases = _stageRepository.ListarAtivas();
            ddlFase.DataSource = fases;
            ddlFase.DataTextField = "Name";
            ddlFase.DataValueField = "StageId";
            ddlFase.DataBind();
            ddlFase.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Todas", ""));

            if (phFiltroComercial.Visible)
            {
                var comerciais = _userRepository.ListarComerciaisAtivos();
                ddlComercial.DataSource = comerciais;
                ddlComercial.DataTextField = "Name";
                ddlComercial.DataValueField = "UserId";
                ddlComercial.DataBind();
                ddlComercial.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Todos", ""));
            }
        }

        protected void btnPesquisar_Click(object sender, EventArgs e)
        {
            ucPaginacao.PaginaAtual = 1;
            Pesquisar();
        }

        protected void lnkLimpar_Click(object sender, EventArgs e)
        {
            txtPesquisa.Text = "";
            ddlFase.SelectedIndex = 0;
            ddlEstado.SelectedIndex = 0;
            if (phFiltroComercial.Visible) ddlComercial.SelectedIndex = 0;
            ucPaginacao.PaginaAtual = 1;
            Pesquisar();
        }

        protected void ucPaginacao_PaginaAlterada(object sender, EventArgs e)
        {
            Pesquisar();
        }

        private void Pesquisar()
        {
            int? stageId = string.IsNullOrEmpty(ddlFase.SelectedValue) ? null : (int?)int.Parse(ddlFase.SelectedValue);
            bool? isClosed = ddlEstado.SelectedValue == "aberta" ? false
                            : ddlEstado.SelectedValue == "fechada" ? true
                            : (bool?)null;
            int? ownerId = phFiltroComercial.Visible && !string.IsNullOrEmpty(ddlComercial.SelectedValue)
                ? int.Parse(ddlComercial.SelectedValue)
                : (int?)null;

            var lista = _opportunityService.Listar(txtPesquisa.Text.Trim(), stageId, null, ownerId, isClosed,
                Perfil, UserId, ucPaginacao.PaginaAtual, ucPaginacao.TamanhoPagina, out int totalRegistos);

            ucPaginacao.TotalRegistos = totalRegistos;
            rptOportunidades.DataSource = lista;
            rptOportunidades.DataBind();
            phVazio.Visible = lista.Count == 0;
        }

        protected string FormatarMoeda(object valor)
        {
            if (valor == null) return "—";
            return string.Format("{0:N2} €", Convert.ToDecimal(valor));
        }

        protected decimal CalcularValorPonderado(Opportunity opportunity) =>
            _opportunityService.CalcularValorPonderado(opportunity);

        protected string ObterBadgeEstado(Opportunity opportunity)
        {
            if (!opportunity.IsClosed)
                return "<span class=\"badge bg-primary\">Aberta</span>";

            return opportunity.Stage != null && opportunity.Stage.IsClosedWon
                ? "<span class=\"badge bg-success\">Ganha</span>"
                : "<span class=\"badge bg-danger\">Perdida</span>";
        }

        protected bool PodeFecharLinha(Opportunity opportunity) =>
            !opportunity.IsClosed && _opportunityService.PodeFechar(Perfil);
    }
}