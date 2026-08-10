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

            // Corre em TODOS os pedidos (não só no !IsPostBack), para que um POST
            // direto ao Guardar não consiga contornar o âmbito "próprios" do
            // Comercial — mesmo padrão de LeadDetalhe.aspx.cs e LeadConverter.aspx.cs.
            Lead lead = null;
            if (LeadId.HasValue)
            {
                lead = _leadService.GetById(LeadId.Value);
                if (lead == null)
                {
                    Response.Redirect("~/Leads/LeadsLista.aspx");
                    return;
                }

                if (_leadService.TemAmbitoProprios(Perfil) && lead.OwnerId != UserId)
                {
                    Response.Redirect("~/AcessoNegado.aspx");
                    return;
                }
            }

            if (!IsPostBack)
            {
                CarregarOrigens();
                CarregarComerciais();
                CarregarMotivosPerda();

                if (lead != null)
                {
                    PreencherFormulario(lead);
                }
                else if (_leadService.TemAmbitoProprios(Perfil))
                {
                    // Comercial cria sempre leads para si próprio.
                    ddlComercial.SelectedValue = UserId.ToString();
                }
            }

            // Visible em controlos HTML não é guardado em ViewState, por isso tem de
            // ser recalculado em TODOS os postbacks (não só no primeiro load), senão
            // o campo reaparece sempre que o postback não vem do próprio ddlEstado.
            AtualizarVisibilidadeMotivoPerda();
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

        // Antes chamava-se CarregarLead(int) e fazia GetById + verificação de dono +
        // preenchimento. O GetById e a verificação passaram para o Page_Load (correm
        // sempre); isto só preenche os campos a partir do lead já validado.
        private void PreencherFormulario(Lead lead)
        {
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
            // Um Comercial nunca escolhe o dono pelo dropdown — o campo fica
            // desativado no ecrã, mas isso não impede um pedido forjado de vir
            // com outro valor. O dono é sempre forçado aqui quando o utilizador
            // tem âmbito "próprios", ignorando o que veio do form.
            int ownerId = _leadService.TemAmbitoProprios(Perfil)
                ? UserId
                : int.Parse(ddlComercial.SelectedValue);

            var lead = new Lead
            {
                Name = txtNome.Text.Trim(),
                CompanyName = string.IsNullOrWhiteSpace(txtEmpresa.Text) ? null : txtEmpresa.Text.Trim(),
                Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                Phone = string.IsNullOrWhiteSpace(txtTelefone.Text) ? null : txtTelefone.Text.Trim(),
                LeadSourceId = int.Parse(ddlOrigem.SelectedValue),
                Status = ddlEstado.SelectedValue,
                Score = string.IsNullOrWhiteSpace(txtPontuacao.Text) ? (int?)null : int.Parse(txtPontuacao.Text),
                OwnerId = ownerId,
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
                litDuplicados.Text = string.Join(", ", duplicados.Select(d => Server.HtmlEncode(d.Name)));
                return;
            }

            try
            {
                if (LeadId.HasValue)
                {
                    lead.UpdatedBy = UserId;
                    _leadService.Atualizar(lead, UserId, Perfil);
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
            catch (InvalidOperationException ex)
            {
                NotificacaoService.Erro(ex.Message);
                return;
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