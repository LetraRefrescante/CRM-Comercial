using System;
using System.Web.UI.WebControls;
using CRM.Data.Repositories;
using CRM.Models.Entities.Atividades;
using CRM.Models.Filtros;
using CRM.Services;
using CRM.Web.Helpers;

namespace CRM.Web.Paginas.Atividades
{
    public partial class AtividadesLista : PaginaBase
    {
        private readonly ActivityService _activityService = new ActivityService();
        private readonly UserRepository _userRepository = new UserRepository();

        private string SortColumn
        {
            get => ViewState["SortColumn"] as string ?? "StartDateTime";
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
                CarregarResponsaveis();
                CarregarAtividades();
            }
        }

        private void CarregarResponsaveis()
        {
            bool podeFiltrarPorResponsavel = !_activityService.TemAmbitoProprios(Perfil);

            ddlResponsavel.Visible = podeFiltrarPorResponsavel;
            lblResponsavel.Visible = podeFiltrarPorResponsavel;

            if (!podeFiltrarPorResponsavel) return;

            ddlResponsavel.Items.Add(new ListItem("Todos", ""));
            foreach (var user in _userRepository.ListarAtivos())
                ddlResponsavel.Items.Add(new ListItem(user.Name, user.UserId.ToString()));
        }

        private int? ObterFiltroResponsavel()
        {
            if (_activityService.TemAmbitoProprios(Perfil)) return UserId;

            if (ddlResponsavel.Visible && !string.IsNullOrEmpty(ddlResponsavel.SelectedValue))
                return int.Parse(ddlResponsavel.SelectedValue);

            return null;
        }

        private void CarregarAtividades()
        {
            var filtro = new ActivityFiltro
            {
                Pesquisa = txtPesquisa.Text.Trim(),
                Tipo = ddlTipo.SelectedValue,
                Status = ddlEstado.SelectedValue,
                AssignedToUserId = ObterFiltroResponsavel(),
                DataInicio = ucFiltroDatas.DataInicial,
                DataFim = ucFiltroDatas.DataFinal
            };

            var atividades = _activityService.Pesquisar(
                filtro,
                ucPaginacao.PaginaAtual,
                ucPaginacao.TamanhoPagina,
                out int total,
                SortColumn,
                SortAscending,
                UserId,
                Perfil);

            ucPaginacao.TotalRegistos = total;

            rptAtividades.DataSource = atividades;
            rptAtividades.DataBind();

            phVazio.Visible = atividades.Count == 0;

            lnkNova.Visible = _activityService.PodeCriar(Perfil);
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            ucPaginacao.PaginaAtual = 1;
            CarregarAtividades();
        }

        protected void lnkOrdenar_Command(object sender, CommandEventArgs e)
        {
            string coluna = e.CommandArgument.ToString();

            if (SortColumn == coluna)
                SortAscending = !SortAscending;
            else
            {
                SortColumn = coluna;
                SortAscending = true;
            }

            ucPaginacao.PaginaAtual = 1;
            CarregarAtividades();
        }

        protected void ucPaginacao_PaginaAlterada(object sender, EventArgs e)
        {
            CarregarAtividades();
        }

        protected void rptAtividades_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var activity = (Activity)e.Item.DataItem;

            var phEditar = e.Item.FindControl("phEditar") as PlaceHolder;
            var phEliminar = e.Item.FindControl("phEliminar") as PlaceHolder;

            if (phEditar != null)
                phEditar.Visible = _activityService.PodeGerir(activity, UserId, Perfil);

            if (phEliminar != null)
                phEliminar.Visible = _activityService.PodeEliminar(activity, UserId, Perfil);
        }

        protected void rptAtividades_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Eliminar") return;

            int activityId = int.Parse(e.CommandArgument.ToString());

            try
            {
                _activityService.Eliminar(activityId, UserId, Perfil);
                NotificacaoService.Sucesso("Atividade eliminada.");
            }
            catch (UnauthorizedAccessException ex)
            {
                NotificacaoService.Erro(ex.Message);
            }

            CarregarAtividades();
        }

        protected string GetRelacionado(object dataItem)
        {
            var activity = (Activity)dataItem;

            if (activity.RelatedClientId.HasValue)
                return "Cliente: " + activity.RelatedClient?.TradeName;

            if (activity.RelatedLeadId.HasValue)
                return "Lead: " + activity.RelatedLead?.Name;

            if (activity.RelatedOpportunityId.HasValue)
                return "Oportunidade #" + activity.RelatedOpportunityId;

            return "-";
        }

        protected string GetBadgeClasse(string status)
        {
            switch (status)
            {
                case "Planeada": return "bg-secondary";
                case "Em Curso": return "badge-em-contacto";
                case "Concluída": return "badge-ativo";
                case "Cancelada": return "badge-bloqueado";
                default: return "bg-secondary";
            }
        }
    }
}