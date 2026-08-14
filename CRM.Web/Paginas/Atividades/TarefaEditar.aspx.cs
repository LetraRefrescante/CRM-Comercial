using System;
using System.Globalization;
using System.Web.UI.WebControls;
using CRM.Data.Repositories;
using CRM.Models.Entities.Atividades;
using CRM.Services;
using CRM.Web.Helpers;

namespace CRM.Web.Paginas.Atividades
{
    public partial class TarefaEditar : PaginaBase
    {
        private readonly TaskService _taskService = new TaskService();
        private readonly UserRepository _userRepository = new UserRepository();
        private readonly LeadRepository _leadRepository = new LeadRepository();

        protected int? TaskId
        {
            get
            {
                if (int.TryParse(Request.QueryString["id"], out int id)) return id;
                return null;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarResponsaveis();
                CarregarLeads();

                if (TaskId.HasValue)
                    CarregarTarefa(TaskId.Value);
                else
                    ddlEstado.SelectedValue = "Planeada";
            }
        }

        private void CarregarResponsaveis()
        {
            ddlResponsavel.Items.Clear();
            foreach (var user in _userRepository.ListarAtivos())
                ddlResponsavel.Items.Add(new ListItem(user.Name, user.UserId.ToString()));
        }

        private void CarregarLeads()
        {
            ddlLead.Items.Clear();
            ddlLead.Items.Add(new ListItem("Selecione...", ""));

            foreach (var lead in _leadRepository.ListarParaSelecao())
                ddlLead.Items.Add(new ListItem(lead.Name, lead.LeadId.ToString()));
        }

        private void CarregarTarefa(int id)
        {
            var task = _taskService.ObterPorId(id);
            if (task == null)
            {
                NotificacaoService.Erro("Tarefa não encontrada.");
                Response.Redirect("TarefasLista.aspx");
                return;
            }

            if (!_taskService.PodeGerir(task, UserId, Perfil))
            {
                NotificacaoService.Erro("Não tens permissão para editar esta tarefa.");
                Response.Redirect("TarefasLista.aspx");
                return;
            }

            txtAssunto.Text = task.Subject;
            txtDataLimite.Text = task.DueDate.ToString("yyyy-MM-ddTHH:mm");
            ddlPrioridade.SelectedValue = task.Priority ?? "";
            ddlEstado.SelectedValue = task.Status;
            ddlResponsavel.SelectedValue = task.AssignedToUserId.ToString();
            txtDescricao.Text = task.Description;

            if (task.RelatedClientId.HasValue)
            {
                ddlTipoRelacao.SelectedValue = "Cliente";
                ucCliente.ClienteId = task.RelatedClientId;
            }
            else if (task.RelatedLeadId.HasValue)
            {
                ddlTipoRelacao.SelectedValue = "Lead";
                ddlLead.SelectedValue = task.RelatedLeadId.Value.ToString();
            }

            AtualizarVisibilidadeRelacao();
        }

        protected void ddlTipoRelacao_SelectedIndexChanged(object sender, EventArgs e)
        {
            AtualizarVisibilidadeRelacao();
        }

        private void AtualizarVisibilidadeRelacao()
        {
            pnlCliente.Visible = ddlTipoRelacao.SelectedValue == "Cliente";
            pnlLead.Visible = ddlTipoRelacao.SelectedValue == "Lead";
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            var task = MontarAPartirDoFormulario();

            try
            {
                if (TaskId.HasValue)
                {
                    task.TaskId = TaskId.Value;
                    _taskService.Atualizar(task, UserId, Perfil);
                    NotificacaoService.Sucesso("Tarefa atualizada.");
                    CarregarTarefa(TaskId.Value);
                }
                else
                {
                    var id = _taskService.Criar(task, UserId, Perfil);
                    NotificacaoService.Sucesso("Tarefa criada.");
                    Response.Redirect($"TarefaEditar.aspx?id={id}");
                }
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is UnauthorizedAccessException)
            {
                NotificacaoService.Erro(ex.Message);
            }
        }

        private TaskItem MontarAPartirDoFormulario()
        {
            DateTime.TryParse(txtDataLimite.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dataLimite);
            int.TryParse(ddlResponsavel.SelectedValue, out int responsavelId);

            var task = new TaskItem
            {
                Subject = txtAssunto.Text.Trim(),
                DueDate = dataLimite,
                Priority = string.IsNullOrEmpty(ddlPrioridade.SelectedValue) ? null : ddlPrioridade.SelectedValue,
                Status = ddlEstado.SelectedValue,
                AssignedToUserId = responsavelId,
                Description = string.IsNullOrWhiteSpace(txtDescricao.Text) ? null : txtDescricao.Text.Trim()
            };

            if (ddlTipoRelacao.SelectedValue == "Cliente")
                task.RelatedClientId = ucCliente.ClienteId;
            else if (ddlTipoRelacao.SelectedValue == "Lead" && !string.IsNullOrEmpty(ddlLead.SelectedValue))
                task.RelatedLeadId = int.Parse(ddlLead.SelectedValue);

            return task;
        }
    }
}