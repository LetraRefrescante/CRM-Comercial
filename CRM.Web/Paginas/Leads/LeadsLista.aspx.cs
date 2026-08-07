using CRM.Data.Repositories;
using CRM.Models.Entities.Leads;
using CRM.Services;
using CRM.Web.Helpers;
using System;

namespace CRM.Web.Paginas.Leads
{
    public partial class LeadsLista : PaginaBase
    {
        private readonly LeadService _leadService = new LeadService();
        private readonly LeadSourceRepository _leadSourceRepository = new LeadSourceRepository();
        private readonly UserRepository _userRepository = new UserRepository();

        private string SortColumn
        {
            get => ViewState["SortColumn"] as string ?? "CreatedDate";
            set => ViewState["SortColumn"] = value;
        }

        private bool SortAscending
        {
            get => ViewState["SortAscending"] as bool? ?? false;
            set => ViewState["SortAscending"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarOrigens();
                CarregarComerciais();
                CarregarLeads();
            }
        }

        private void CarregarOrigens()
        {
            ddlOrigem.Items.Clear();
            ddlOrigem.Items.Add(new System.Web.UI.WebControls.ListItem("Todas", ""));
            foreach (var origem in _leadSourceRepository.ListarAtivos())
            {
                ddlOrigem.Items.Add(new System.Web.UI.WebControls.ListItem(origem.Name, origem.LeadSourceId.ToString()));
            }
        }

        private void CarregarComerciais()
        {
            bool podeFiltrarPorComercial = !_leadService.TemAmbitoProprios(Perfil);

            ddlComercial.Visible = podeFiltrarPorComercial;
            lblComercial.Visible = podeFiltrarPorComercial;

            if (!podeFiltrarPorComercial) return;

            ddlComercial.Items.Add(new System.Web.UI.WebControls.ListItem("Todos", ""));
            foreach (var user in _userRepository.ListarComerciaisAtivos())
            {
                ddlComercial.Items.Add(new System.Web.UI.WebControls.ListItem(user.Name, user.UserId.ToString()));
            }
        }

        private int? ObterFiltroComercial()
        {
            if (_leadService.TemAmbitoProprios(Perfil)) return UserId;

            if (ddlComercial.Visible && !string.IsNullOrEmpty(ddlComercial.SelectedValue))
                return int.Parse(ddlComercial.SelectedValue);

            return null;
        }

        private void CarregarLeads()
        {
            int? leadSourceId = string.IsNullOrEmpty(ddlOrigem.SelectedValue) ? (int?)null : int.Parse(ddlOrigem.SelectedValue);
            int? scoreMin = string.IsNullOrWhiteSpace(txtPontuacaoMin.Text) ? (int?)null : int.Parse(txtPontuacaoMin.Text);
            int? scoreMax = string.IsNullOrWhiteSpace(txtPontuacaoMax.Text) ? (int?)null : int.Parse(txtPontuacaoMax.Text);

            var leads = _leadService.Listar(
                pesquisa: txtPesquisa.Text.Trim(),
                status: ddlEstado.SelectedValue,
                leadSourceId: leadSourceId,
                ownerId: ObterFiltroComercial(),
                scoreMin: scoreMin,
                scoreMax: scoreMax,
                dataInicio: ucFiltroDatas.DataInicial,
                dataFim: ucFiltroDatas.DataFinal,
                pagina: ucPaginacao.PaginaAtual,
                tamanhoPagina: ucPaginacao.TamanhoPagina,
                totalRegistos: out int total,
                sortColumn: SortColumn,
                sortAscending: SortAscending);

            ucPaginacao.TotalRegistos = total;

            rptLeads.DataSource = leads;
            rptLeads.DataBind();

            phVazio.Visible = leads.Count == 0;

            lnkNovo.Visible = _leadService.PodeCriarOuEditar(Perfil);
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            ucPaginacao.PaginaAtual = 1;
            CarregarLeads();
        }

        protected void lnkOrdenar_Command(object sender, System.Web.UI.WebControls.CommandEventArgs e)
        {
            string coluna = e.CommandArgument.ToString();

            if (SortColumn == coluna)
            {
                SortAscending = !SortAscending;
            }
            else
            {
                SortColumn = coluna;
                SortAscending = true;
            }

            ucPaginacao.PaginaAtual = 1;
            CarregarLeads();
        }

        protected void ucPaginacao_PaginaAlterada(object sender, EventArgs e)
        {
            CarregarLeads();
        }

        protected void rptLeads_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != System.Web.UI.WebControls.ListItemType.Item &&
                e.Item.ItemType != System.Web.UI.WebControls.ListItemType.AlternatingItem)
                return;

            var lead = (Lead)e.Item.DataItem;

            var phEditar = e.Item.FindControl("phEditar") as System.Web.UI.WebControls.PlaceHolder;
            var phEliminar = e.Item.FindControl("phEliminar") as System.Web.UI.WebControls.PlaceHolder;
            var phConverter = e.Item.FindControl("phConverter") as System.Web.UI.WebControls.PlaceHolder;

            bool bloqueado = _leadService.EstaBloqueadoParaEdicao(lead);

            if (phEditar != null) phEditar.Visible = _leadService.PodeCriarOuEditar(Perfil) && !bloqueado;
            if (phEliminar != null) phEliminar.Visible = _leadService.PodeEliminar(Perfil);
            if (phConverter != null) phConverter.Visible = _leadService.PodeConverter(Perfil) && !bloqueado;
        }

        protected void rptLeads_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Eliminar")
            {
                int leadId = int.Parse(e.CommandArgument.ToString());

                if (_leadService.Eliminar(leadId, UserId, Perfil))
                {
                    NotificacaoService.Sucesso("Lead eliminado.");
                }
                else
                {
                    NotificacaoService.Erro("Não tens permissão para eliminar leads.");
                }

                CarregarLeads();
            }
        }

        protected string GetBadgeClasse(string status)
        {
            switch (status)
            {
                case "Novo": return "badge-novo";
                case "Em Contacto": return "badge-em-contacto";
                case "Qualificado": return "badge-qualificado";
                case "Não Qualificado": return "badge-nao-qualificado";
                case "Convertido": return "badge-convertido";
                default: return "bg-secondary";
            }
        }
    }
}