using CRM.Data.Repositories;
using CRM.Models.Entities.Leads;
using CRM.Services;
using CRM.Web.Helpers;
using System;
using System.Linq;

namespace CRM.Web.Paginas.Leads
{
    public partial class LeadEditar : PaginaBase
    {
        private readonly LeadService _leadService = new LeadService();
        private readonly LeadSourceRepository _leadSourceRepository = new LeadSourceRepository();
        private readonly LossReasonRepository _lossReasonRepository = new LossReasonRepository();
        private readonly UserRepository _userRepository = new UserRepository();

        private int? LeadId
        {
            get
            {
                if (int.TryParse(Request.QueryString["id"], out int id)) return id;
                return null;
            }
        }

        public string TituloPagina => LeadId.HasValue ? "Editar Lead" : "Novo Lead";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!_leadService.PodeCriarOuEditar(Perfil))
            {
                Response.Redirect("~/AcessoNegado.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CarregarOrigens();
                CarregarComerciais();
                CarregarMotivosPerda();
                AtualizarVisibilidadeMotivoPerda();

                if (LeadId.HasValue)
                {
                    CarregarLead(LeadId.Value);
                }
                else if (_leadService.TemAmbitoProprios(Perfil))
                {
                    // Comercial cria sempre leads para si próprio.
                    ddlComercial.SelectedValue = UserId.ToString();
                }
            }
        }

        private void CarregarOrigens()
        {
            ddlOrigem.Items.Clear();
            ddlOrigem.Items.Add(new System.Web.UI.WebControls.ListItem("Seleciona...", ""));
            foreach (var origem in _leadSourceRepository.ListarAtivos())
            {
                ddlOrigem.Items.Add(new System.Web.UI.WebControls.ListItem(origem.Name, origem.LeadSourceId.ToString()));
            }
        }

        private void CarregarComerciais()
        {
            bool podeEscolherComercial = !_leadService.TemAmbitoProprios(Perfil);
            ddlComercial.Enabled = podeEscolherComercial;

            ddlComercial.Items.Clear();
            ddlComercial.Items.Add(new System.Web.UI.WebControls.ListItem("Seleciona...", ""));
            foreach (var user in _userRepository.ListarComerciaisAtivos())
            {
                ddlComercial.Items.Add(new System.Web.UI.WebControls.ListItem(user.Name, user.UserId.ToString()));
            }
        }

        private void CarregarMotivosPerda()
        {
            ddlMotivoPerda.Items.Clear();
            ddlMotivoPerda.Items.Add(new System.Web.UI.WebControls.ListItem("Seleciona...", ""));
            foreach (var motivo in _lossReasonRepository.ListarAtivos())
            {
                ddlMotivoPerda.Items.Add(new System.Web.UI.WebControls.ListItem(motivo.Name, motivo.LossReasonId.ToString()));
            }
        }

        private void CarregarLead(int leadId)
        {
            var lead = _leadService.GetById(leadId);
            if (lead == null)
            {
                Response.Redirect("~/Leads/LeadsLista.aspx");
                return;
            }

            // Comercial só pode editar os próprios leads.
            if (_leadService.TemAmbitoProprios(Perfil) && lead.OwnerId != UserId)
            {
                Response.Redirect("~/AcessoNegado.aspx");
                return;
            }

            txtNome.Text = lead.Name;
            txtEmpresa.Text = lead.CompanyName;
            txtEmail.Text = lead.Email;
            txtTelefone.Text = lead.Phone;
            ddlOrigem.SelectedValue = lead.LeadSourceId.ToString();
            ddlEstado.SelectedValue = lead.Status;
            txtPontuacao.Text = lead.Score?.ToString();
            ddlComercial.SelectedValue = lead.OwnerId.ToString();
            txtProximoContacto.Text = lead.NextContactDate?.ToString("yyyy-MM-ddTHH:mm");
            ddlMotivoPerda.SelectedValue = lead.LossReasonId?.ToString() ?? "";

            ViewState["RowVersion"] = Convert.ToBase64String(lead.RowVersion);

            bool bloqueado = _leadService.EstaBloqueadoParaEdicao(lead);
            phBloqueado.Visible = bloqueado;

            if (bloqueado)
            {
                DesativarFormulario();
            }

            AtualizarVisibilidadeMotivoPerda();
        }

        private void DesativarFormulario()
        {
            txtNome.Enabled = false;
            txtEmpresa.Enabled = false;
            txtEmail.Enabled = false;
            txtTelefone.Enabled = false;
            ddlOrigem.Enabled = false;
            ddlEstado.Enabled = false;
            txtPontuacao.Enabled = false;
            ddlComercial.Enabled = false;
            txtProximoContacto.Enabled = false;
            ddlMotivoPerda.Enabled = false;
            btnGuardar.Visible = false;
        }

        protected void ddlEstado_SelectedIndexChanged(object sender, EventArgs e)
        {
            AtualizarVisibilidadeMotivoPerda();
        }

        private void AtualizarVisibilidadeMotivoPerda()
        {
            divMotivoPerda.Visible = ddlEstado.SelectedValue == LeadService.StatusNaoQualificado;
        }

        private Lead MontarLeadDoFormulario()
        {
            var lead = new Lead
            {
                Name = txtNome.Text.Trim(),
                CompanyName = string.IsNullOrWhiteSpace(txtEmpresa.Text) ? null : txtEmpresa.Text.Trim(),
                Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                Phone = string.IsNullOrWhiteSpace(txtTelefone.Text) ? null : txtTelefone.Text.Trim(),
                LeadSourceId = int.Parse(ddlOrigem.SelectedValue),
                Status = ddlEstado.SelectedValue,
                Score = string.IsNullOrWhiteSpace(txtPontuacao.Text) ? (int?)null : int.Parse(txtPontuacao.Text),
                OwnerId = int.Parse(ddlComercial.SelectedValue),
                NextContactDate = string.IsNullOrWhiteSpace(txtProximoContacto.Text)
                    ? (DateTime?)null
                    : DateTime.Parse(txtProximoContacto.Text),
                LossReasonId = string.IsNullOrWhiteSpace(ddlMotivoPerda.SelectedValue)
                    ? (int?)null
                    : int.Parse(ddlMotivoPerda.SelectedValue)
            };

            if (LeadId.HasValue)
            {
                lead.LeadId = LeadId.Value;
                lead.RowVersion = Convert.FromBase64String(ViewState["RowVersion"] as string);
            }

            return lead;
        }

        protected void cvRegrasNegocio_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            var lead = MontarLeadDoFormulario();
            var erros = _leadService.Validar(lead);

            args.IsValid = erros.Count == 0;
            cvRegrasNegocio.ErrorMessage = string.Join(" ", erros);
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            var lead = MontarLeadDoFormulario();

            // Aviso de possíveis duplicados — não bloqueia, o blueprint pede aviso.
            var duplicados = _leadService.ProcurarPossiveisDuplicados(lead.Email, lead.Phone, LeadId);
            if (duplicados.Count > 0 && !chkConfirmarDuplicado.Checked)
            {
                phDuplicados.Visible = true;
                litDuplicados.Text = string.Join(", ", duplicados.Select(d => d.Name));
                return;
            }

            try
            {
                if (LeadId.HasValue)
                {
                    lead.UpdatedBy = UserId;
                    _leadService.Atualizar(lead, UserId);
                    NotificacaoService.Sucesso("Lead atualizado.");
                }
                else
                {
                    lead.CreatedBy = UserId;
                    int novoId = _leadService.Criar(lead);
                    NotificacaoService.Sucesso("Lead criado.");
                    Response.Redirect($"~/Leads/LeadEditar.aspx?id={novoId}");
                    return;
                }
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException)
            {
                NotificacaoService.Erro("Este lead foi alterado por outro utilizador entretanto. Recarrega a página e tenta novamente.");
                return;
            }

            Response.Redirect("~/Leads/LeadsLista.aspx");
        }
    }
}