using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using CRM.Data.Repositories;
using CRM.Models.DTOs;
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

        private List<ParticipanteLinha> Participantes
        {
            get => ViewState["Participantes"] as List<ParticipanteLinha> ?? new List<ParticipanteLinha>();
            set => ViewState["Participantes"] = value;
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

                    if (DateTime.TryParse(Request.QueryString["data"], CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dataPreenchida))
                        txtInicio.Text = dataPreenchida.ToString("yyyy-MM-ddTHH:mm");
                }

                AtualizarVisibilidadeParticipantes();
                BindParticipantes();
            }
        }

        private void CarregarResponsaveis()
        {
            ddlResponsavel.Items.Clear();
            ddlParticipanteInterno.Items.Clear();
            ddlParticipanteInterno.Items.Add(new ListItem("Selecione...", ""));

            foreach (var user in _userRepository.ListarAtivos())
            {
                ddlResponsavel.Items.Add(new ListItem(user.Name, user.UserId.ToString()));
                ddlParticipanteInterno.Items.Add(new ListItem(user.Name, user.UserId.ToString()));
            }
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

            if (activity.Type == "Reunião")
            {
                Participantes = _activityService.ListarParticipantes(id)
                    .Select(p => new ParticipanteLinha
                    {
                        UserId = p.UserId,
                        NomeExibicao = p.User?.Name,
                        ExternalName = p.ExternalName,
                        ExternalEmail = p.ExternalEmail
                    })
                    .ToList();
            }
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

        protected void ddlTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Só Reuniões têm participantes — muda para outro tipo limpa a lista,
            // para não gravar participantes "escondidos" que ficaram em ViewState.
            if (ddlTipo.SelectedValue != "Reunião")
                Participantes = new List<ParticipanteLinha>();

            AtualizarVisibilidadeParticipantes();
            BindParticipantes();
        }

        private void AtualizarVisibilidadeParticipantes()
        {
            pnlParticipantes.Visible = ddlTipo.SelectedValue == "Reunião";
        }

        private void BindParticipantes()
        {
            var lista = Participantes;
            rptParticipantes.DataSource = lista;
            rptParticipantes.DataBind();
            phSemParticipantes.Visible = lista.Count == 0;
        }

        protected void btnAdicionarParticipante_Click(object sender, EventArgs e)
        {
            var lista = Participantes;

            if (!string.IsNullOrEmpty(ddlParticipanteInterno.SelectedValue))
            {
                int userId = int.Parse(ddlParticipanteInterno.SelectedValue);

                if (lista.Any(p => p.UserId == userId))
                {
                    NotificacaoService.Erro("Este utilizador já está na lista de participantes.");
                    BindParticipantes();
                    return;
                }

                lista.Add(new ParticipanteLinha
                {
                    UserId = userId,
                    NomeExibicao = ddlParticipanteInterno.SelectedItem.Text
                });
            }
            else if (!string.IsNullOrWhiteSpace(txtParticipanteExternoNome.Text))
            {
                string email = txtParticipanteExternoEmail.Text.Trim();
                if (!string.IsNullOrEmpty(email) && !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    NotificacaoService.Erro("O email do participante externo não tem um formato válido.");
                    BindParticipantes();
                    return;
                }

                lista.Add(new ParticipanteLinha
                {
                    ExternalName = txtParticipanteExternoNome.Text.Trim(),
                    ExternalEmail = string.IsNullOrEmpty(email) ? null : email
                });
            }
            else
            {
                NotificacaoService.Erro("Seleciona um utilizador interno ou preenche o nome do participante externo.");
                BindParticipantes();
                return;
            }

            Participantes = lista;

            ddlParticipanteInterno.SelectedIndex = 0;
            txtParticipanteExternoNome.Text = "";
            txtParticipanteExternoEmail.Text = "";

            BindParticipantes();
        }

        protected void rptParticipantes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Remover") return;

            int indice = int.Parse(e.CommandArgument.ToString());
            var lista = Participantes;
            if (indice >= 0 && indice < lista.Count)
                lista.RemoveAt(indice);

            Participantes = lista;
            BindParticipantes();
        }

        protected string GetNomeParticipante(object dataItem)
        {
            var p = (ParticipanteLinha)dataItem;
            return p.EhInterno ? p.NomeExibicao : $"{p.ExternalName} ({(string.IsNullOrEmpty(p.ExternalEmail) ? "sem email" : p.ExternalEmail)})";
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            var activity = MontarAPartirDoFormulario();

            try
            {
                int activityId;

                if (ActivityId.HasValue)
                {
                    activity.ActivityId = ActivityId.Value;
                    _activityService.Atualizar(activity, UserId, Perfil);
                    activityId = ActivityId.Value;
                    NotificacaoService.Sucesso("Atividade atualizada.");
                }
                else
                {
                    activityId = _activityService.Criar(activity, UserId, Perfil);
                    NotificacaoService.Sucesso("Atividade criada.");
                }

                _activityService.SincronizarParticipantes(activityId,
                    Participantes.Select(p => new ActivityParticipant
                    {
                        UserId = p.UserId,
                        ExternalName = p.ExternalName,
                        ExternalEmail = p.ExternalEmail
                    }).ToList());

                Response.Redirect($"AtividadeEditar.aspx?id={activityId}");
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