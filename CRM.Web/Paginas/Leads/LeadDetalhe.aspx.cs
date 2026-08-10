using CRM.Data.Repositories;
using CRM.Models.Entities.Atividades;
using CRM.Models.Entities.Leads;
using CRM.Services;
using CRM.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CRM.Web.Paginas.Leads
{
    public partial class LeadDetalhe : PaginaBase
    {
        private readonly LeadService _leadService = new LeadService();
        private readonly ActivityService _activityService = new ActivityService();
        private readonly UserRepository _userRepository = new UserRepository();

        public Lead Lead { get; private set; }
        public string NomeLead => Lead?.Name ?? "Lead";
        public int? ClienteConvertidoId => Lead?.ConvertedClientId;

        private int LeadId
        {
            get
            {
                if (int.TryParse(Request.QueryString["id"], out int id)) return id;
                Response.Redirect("~/Leads/LeadsLista.aspx");
                return 0;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Lead = _leadService.GetById(LeadId);
            if (Lead == null)
            {
                Response.Redirect("~/Leads/LeadsLista.aspx");
                return;
            }

            // Comercial só pode ver os próprios leads. Diretor/Administrador (TOTAL) e
            // Financeiro/Consulta (CONSULTA) veem todos, mesmo sem poder editar.
            if (_leadService.TemAmbitoProprios(Perfil) && Lead.OwnerId != UserId)
            {
                Response.Redirect("~/AcessoNegado.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CarregarResumo();
                CarregarAcoes();
                CarregarHistoricoEstados();
                CarregarResponsaveisAtividade();
                CarregarAtividades();

                if (!string.IsNullOrEmpty(txtDataHoraAtividade.Text) == false)
                {
                    txtDataHoraAtividade.Text = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
                }
            }
        }

        private void CarregarResumo()
        {
            litNome.Text = Server.HtmlEncode(Lead.Name);
            litEmpresa.Text = Server.HtmlEncode(Lead.CompanyName ?? "—");
            litEmail.Text = Server.HtmlEncode(Lead.Email ?? "—");
            litTelefone.Text = Server.HtmlEncode(Lead.Phone ?? "—");
            litOrigem.Text = Server.HtmlEncode(Lead.LeadSource?.Name ?? "—");
            litPontuacao.Text = Lead.Score.HasValue ? Lead.Score.Value.ToString() : "—";
            litComercial.Text = Server.HtmlEncode(Lead.Owner?.Name ?? "—");
            litProximoContacto.Text = Lead.NextContactDate.HasValue
                ? Lead.NextContactDate.Value.ToString("dd/MM/yyyy HH:mm")
                : "—";
            litCriadoEm.Text = Lead.CreatedDate.ToString("dd/MM/yyyy HH:mm");

            if (Lead.LossReasonId.HasValue)
            {
                phMotivoPerda.Visible = true;
                litMotivoPerda.Text = Server.HtmlEncode(Lead.LossReason?.Name ?? "—");
            }

            bool bloqueado = _leadService.EstaBloqueadoParaEdicao(Lead);
            phBloqueado.Visible = bloqueado;
            phLinkClienteConvertido.Visible = bloqueado && Lead.ConvertedClientId.HasValue;
        }

        private void CarregarAcoes()
        {
            bool bloqueado = _leadService.EstaBloqueadoParaEdicao(Lead);

            phEditar.Visible = _leadService.PodeCriarOuEditar(Perfil) && !bloqueado;
            lnkEditar.NavigateUrl = $"~/Leads/LeadEditar.aspx?id={Lead.LeadId}";

            phConverter.Visible = _leadService.PodeConverter(Perfil) && !bloqueado;
            lnkConverter.NavigateUrl = $"~/Leads/LeadConverter.aspx?id={Lead.LeadId}";

            phEliminar.Visible = _leadService.PodeEliminar(Perfil);

            // O formulário de registo rápido de atividade segue a mesma permissão de edição.
            phNovaAtividade.Visible = _leadService.PodeCriarOuEditar(Perfil) && !bloqueado;
        }

        private void CarregarHistoricoEstados()
        {
            var historico = _leadService.ListarHistoricoEstados(Lead.LeadId);

            var idsUtilizadores = historico
                .Where(h => h.ChangedBy.HasValue)
                .Select(h => h.ChangedBy.Value)
                .Distinct()
                .ToList();

            var nomes = idsUtilizadores.Count > 0
                ? _userRepository.ObterNomesPorIds(idsUtilizadores)
                : new Dictionary<int, string>();

            var modeloVista = historico.Select(h => new
            {
                h.PreviousStatus,
                h.NewStatus,
                h.ChangedDate,
                ChangedByName = h.ChangedBy.HasValue && nomes.ContainsKey(h.ChangedBy.Value)
                    ? nomes[h.ChangedBy.Value]
                    : "Sistema"
            }).ToList();

            rptHistoricoEstados.DataSource = modeloVista;
            rptHistoricoEstados.DataBind();

            phSemHistorico.Visible = modeloVista.Count == 0;
        }

        private void CarregarResponsaveisAtividade()
        {
            ddlResponsavelAtividade.Items.Clear();
            foreach (var user in _userRepository.ListarComerciaisAtivos())
            {
                ddlResponsavelAtividade.Items.Add(new System.Web.UI.WebControls.ListItem(user.Name, user.UserId.ToString()));
            }

            var itemAtual = ddlResponsavelAtividade.Items.FindByValue(UserId.ToString());
            if (itemAtual != null)
            {
                ddlResponsavelAtividade.ClearSelection();
                itemAtual.Selected = true;
            }
        }

        private void CarregarAtividades()
        {
            var atividades = _activityService.ListarPorLead(Lead.LeadId);
            rptAtividades.DataSource = atividades;
            rptAtividades.DataBind();

            phSemAtividades.Visible = atividades.Count == 0;
        }

        protected void btnRegistarAtividade_Click(object sender, EventArgs e)
        {
            lblErroAtividade.Visible = false;

            var atividade = new Activity
            {
                Type = ddlTipoAtividade.SelectedValue,
                Subject = txtAssunto.Text.Trim(),
                Description = string.IsNullOrWhiteSpace(txtDescricaoAtividade.Text) ? null : txtDescricaoAtividade.Text.Trim(),
                RelatedLeadId = Lead.LeadId,
                AssignedToUserId = int.Parse(ddlResponsavelAtividade.SelectedValue),
                StartDateTime = string.IsNullOrWhiteSpace(txtDataHoraAtividade.Text)
                    ? DateTime.Now
                    : DateTime.Parse(txtDataHoraAtividade.Text),
                Status = ddlEstadoAtividade.SelectedValue,
                Priority = "Normal",
                CreatedBy = UserId
            };

            if (atividade.Status == "Concluída")
            {
                atividade.CompletedDateTime = DateTime.UtcNow;
            }

            var erros = _activityService.Validar(atividade);
            if (erros.Count > 0)
            {
                lblErroAtividade.Text = string.Join(" ", erros);
                lblErroAtividade.Visible = true;
                CarregarAtividades();
                return;
            }

            _activityService.Criar(atividade);
            NotificacaoService.Sucesso("Atividade registada.");

            txtAssunto.Text = "";
            txtDescricaoAtividade.Text = "";
            CarregarAtividades();
        }

        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            if (_leadService.Eliminar(Lead.LeadId, UserId, Perfil))
            {
                NotificacaoService.Sucesso("Lead eliminado.");
                Response.Redirect("~/Leads/LeadsLista.aspx");
            }
            else
            {
                NotificacaoService.Erro("Não tens permissão para eliminar leads.");
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