using System;
using System.Globalization;
using System.Web.UI.WebControls;
using CRM.Data.Repositories;
using CRM.Models.Entities.Atividades;
using CRM.Services;
using CRM.Web.Helpers;

namespace CRM.Web.Paginas.Atividades
{
    public partial class AtividadeEditar : PaginaBase
    {
        private readonly ActivityService _activityService = new ActivityService();
        private readonly UserRepository _userRepository = new UserRepository();
        private readonly LeadRepository _leadRepository = new LeadRepository();

        protected int? ActivityId
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

                if (ActivityId.HasValue)
                {
                    CarregarAtividade(ActivityId.Value);
                }
                else
                {
                    ddlEstado.SelectedValue = "Planeada";

                    // Vindo da Agenda com um dia clicado (?data=yyyy-MM-dd) — pré-preenche o Início.
                    if (DateTime.TryParse(Request.QueryString["data"], CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dataPreenchida))
                        txtInicio.Text = dataPreenchida.ToString("yyyy-MM-ddTHH:mm");
                }
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

        private void CarregarAtividade(int id)
        {
            var activity = _activityService.ObterPorId(id);
            if (activity == null)
            {
                NotificacaoService.Erro("Atividade não encontrada.");
                Response.Redirect("AtividadesLista.aspx");
                return;
            }

            if (!_activityService.PodeGerir(activity, UserId, Perfil))
            {
                NotificacaoService.Erro("Não tens permissão para editar esta atividade.");
                Response.Redirect("AtividadesLista.aspx");
                return;
            }

            ddlTipo.SelectedValue = activity.Type;
            txtAssunto.Text = activity.Subject;
            txtInicio.Text = activity.StartDateTime.ToString("yyyy-MM-ddTHH:mm");
            txtFim.Text = activity.EndDateTime?.ToString("yyyy-MM-ddTHH:mm");
            ddlPrioridade.SelectedValue = activity.Priority ?? "";
            ddlEstado.SelectedValue = activity.Status;
            ddlResponsavel.SelectedValue = activity.AssignedToUserId.ToString();
            txtLembrete.Text = activity.ReminderDateTime?.ToString("yyyy-MM-ddTHH:mm");
            txtDescricao.Text = activity.Description;

            if (activity.RelatedClientId.HasValue)
            {
                ddlTipoRelacao.SelectedValue = "Cliente";
                ucCliente.ClienteId = activity.RelatedClientId;
            }
            else if (activity.RelatedLeadId.HasValue)
            {
                ddlTipoRelacao.SelectedValue = "Lead";
                ddlLead.SelectedValue = activity.RelatedLeadId.Value.ToString();
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
            var activity = MontarAPartirDoFormulario();

            try
            {
                if (ActivityId.HasValue)
                {
                    activity.ActivityId = ActivityId.Value;
                    _activityService.Atualizar(activity, UserId, Perfil);
                    NotificacaoService.Sucesso("Atividade atualizada.");
                    CarregarAtividade(ActivityId.Value);
                }
                else
                {
                    var id = _activityService.Criar(activity, UserId, Perfil);
                    NotificacaoService.Sucesso("Atividade criada.");
                    Response.Redirect($"AtividadeEditar.aspx?id={id}");
                }
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is UnauthorizedAccessException)
            {
                NotificacaoService.Erro(ex.Message);
            }
        }

        private Activity MontarAPartirDoFormulario()
        {
            DateTime.TryParse(txtInicio.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime inicio);
            DateTime? fim = DateTime.TryParse(txtFim.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime f) ? f : (DateTime?)null;
            DateTime? lembrete = DateTime.TryParse(txtLembrete.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime l) ? l : (DateTime?)null;
            int.TryParse(ddlResponsavel.SelectedValue, out int responsavelId);

            var activity = new Activity
            {
                Type = ddlTipo.SelectedValue,
                Subject = txtAssunto.Text.Trim(),
                StartDateTime = inicio,
                EndDateTime = fim,
                Priority = string.IsNullOrEmpty(ddlPrioridade.SelectedValue) ? null : ddlPrioridade.SelectedValue,
                Status = ddlEstado.SelectedValue,
                AssignedToUserId = responsavelId,
                ReminderDateTime = lembrete,
                Description = string.IsNullOrWhiteSpace(txtDescricao.Text) ? null : txtDescricao.Text.Trim()
            };

            if (ddlTipoRelacao.SelectedValue == "Cliente")
                activity.RelatedClientId = ucCliente.ClienteId;
            else if (ddlTipoRelacao.SelectedValue == "Lead" && !string.IsNullOrEmpty(ddlLead.SelectedValue))
                activity.RelatedLeadId = int.Parse(ddlLead.SelectedValue);

            return activity;
        }
    }
}