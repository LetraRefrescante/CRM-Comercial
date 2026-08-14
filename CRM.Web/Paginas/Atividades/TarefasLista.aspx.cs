using System;
using System.Web.UI.WebControls;
using CRM.Data.Repositories;
using CRM.Models.Entities.Atividades;
using CRM.Models.Filtros;
using CRM.Services;
using CRM.Web.Helpers;

namespace CRM.Web.Paginas.Atividades
{
    public partial class TarefasLista : PaginaBase
    {
        private readonly TaskService _taskService = new TaskService();
        private readonly UserRepository _userRepository = new UserRepository();

        private string SortColumn
        {
            get => ViewState["SortColumn"] as string ?? "DueDate";
            set => ViewState["SortColumn"] = value;
        }

        private bool SortAscending
        {
            get => ViewState["SortAscending"] as bool? ?? true;
            set => ViewState["SortAscending"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarResponsaveis();
                CarregarTarefas();
            }
        }

        private void CarregarResponsaveis()
        {
            bool podeFiltrarPorResponsavel = !_taskService.TemAmbitoProprios(Perfil);

            ddlResponsavel.Visible = podeFiltrarPorResponsavel;
            lblResponsavel.Visible = podeFiltrarPorResponsavel;

            if (!podeFiltrarPorResponsavel) return;

            ddlResponsavel.Items.Add(new ListItem("Todos", ""));
            foreach (var user in _userRepository.ListarAtivos())
                ddlResponsavel.Items.Add(new ListItem(user.Name, user.UserId.ToString()));
        }

        private int? ObterFiltroResponsavel()
        {
            if (_taskService.TemAmbitoProprios(Perfil)) return UserId;

            if (ddlResponsavel.Visible && !string.IsNullOrEmpty(ddlResponsavel.SelectedValue))
                return int.Parse(ddlResponsavel.SelectedValue);

            return null;
        }

        private void CarregarTarefas()
        {
            var filtro = new TaskFiltro
            {
                Pesquisa = txtPesquisa.Text.Trim(),
                Status = ddlEstado.SelectedValue,
                AssignedToUserId = ObterFiltroResponsavel(),
                DataInicio = ucFiltroDatas.DataInicial,
                DataFim = ucFiltroDatas.DataFinal
            };

            var tarefas = _taskService.Pesquisar(
                filtro,
                ucPaginacao.PaginaAtual,
                ucPaginacao.TamanhoPagina,
                out int total,
                SortColumn,
                SortAscending,
                UserId,
                Perfil);

            ucPaginacao.TotalRegistos = total;

            rptTarefas.DataSource = tarefas;
            rptTarefas.DataBind();

            phVazio.Visible = tarefas.Count == 0;

            lnkNova.Visible = _taskService.PodeCriar(Perfil);
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            ucPaginacao.PaginaAtual = 1;
            CarregarTarefas();
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
            CarregarTarefas();
        }

        protected void ucPaginacao_PaginaAlterada(object sender, EventArgs e)
        {
            CarregarTarefas();
        }

        protected void rptTarefas_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var task = (TaskItem)e.Item.DataItem;

            var phEditar = e.Item.FindControl("phEditar") as PlaceHolder;
            var phEliminar = e.Item.FindControl("phEliminar") as PlaceHolder;

            if (phEditar != null)
                phEditar.Visible = _taskService.PodeGerir(task, UserId, Perfil);

            if (phEliminar != null)
                phEliminar.Visible = _taskService.PodeEliminar(task, UserId, Perfil);
        }

        protected void rptTarefas_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Eliminar") return;

            int taskId = int.Parse(e.CommandArgument.ToString());

            try
            {
                _taskService.Eliminar(taskId, UserId, Perfil);
                NotificacaoService.Sucesso("Tarefa eliminada.");
            }
            catch (UnauthorizedAccessException ex)
            {
                NotificacaoService.Erro(ex.Message);
            }

            CarregarTarefas();
        }

        protected string GetRelacionado(object dataItem)
        {
            var task = (TaskItem)dataItem;

            if (task.RelatedClientId.HasValue)
                return "Cliente: " + task.RelatedClient?.TradeName;

            if (task.RelatedLeadId.HasValue)
                return "Lead: " + task.RelatedLead?.Name;

            if (task.RelatedOpportunityId.HasValue)
                return "Oportunidade #" + task.RelatedOpportunityId;

            return "-";
        }

        protected string GetVencidaClasse(object dataItem)
        {
            var task = (TaskItem)dataItem;
            bool vencida = task.Status != "Concluída" && task.Status != "Cancelada" && task.DueDate < DateTime.UtcNow;
            return vencida ? "text-danger fw-bold" : "";
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