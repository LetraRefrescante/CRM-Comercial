using System;
using System.Web.UI.WebControls;
using CRM.Models.Entities.Notificacoes;
using CRM.Services;
using CRM.Web.Helpers;

namespace CRM.Web.Paginas.Notificacoes
{
    public partial class TemplatesEmail : PaginaBase
    {
        private readonly EmailTemplateService _emailTemplateService = new EmailTemplateService();

        private int? EmailTemplateIdEmEdicao
        {
            get => ViewState["EmailTemplateIdEmEdicao"] as int?;
            set => ViewState["EmailTemplateIdEmEdicao"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!_emailTemplateService.PodeGerir(Perfil))
            {
                NotificacaoService.Erro("Não tens permissão para gerir modelos de email.");
                Response.Redirect("~/Dashboard/Dashboard.aspx");
                return;
            }

            if (!IsPostBack)
                CarregarTemplates();
        }

        private void CarregarTemplates()
        {
            var templates = _emailTemplateService.Listar(null);
            rptTemplates.DataSource = templates;
            rptTemplates.DataBind();
            phVazio.Visible = templates.Count == 0;
        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            EmailTemplateIdEmEdicao = null;
            txtNome.Text = "";
            txtAssunto.Text = "";
            txtCorpo.Text = "";
            litTituloFormulario.Text = "Novo Modelo";
            pnlFormulario.Visible = true;
        }

        protected void btnCancelarEdicao_Click(object sender, EventArgs e)
        {
            pnlFormulario.Visible = false;
        }

        protected void rptTemplates_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int emailTemplateId = int.Parse(e.CommandArgument.ToString());

            if (e.CommandName == "Editar")
            {
                var template = _emailTemplateService.GetById(emailTemplateId);
                if (template == null) return;

                EmailTemplateIdEmEdicao = emailTemplateId;
                txtNome.Text = template.Name;
                txtAssunto.Text = template.Subject;
                txtCorpo.Text = template.Body;
                litTituloFormulario.Text = "Editar Modelo";
                pnlFormulario.Visible = true;
            }
            else if (e.CommandName == "AlternarEstado")
            {
                _emailTemplateService.AlternarEstado(emailTemplateId, UserId);
                NotificacaoService.Sucesso("Estado do modelo atualizado.");
                CarregarTemplates();
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            bool nomeJaExiste = _emailTemplateService.ExisteNome(txtNome.Text.Trim(), EmailTemplateIdEmEdicao);

            var template = new EmailTemplate
            {
                Name = txtNome.Text.Trim(),
                Subject = txtAssunto.Text.Trim(),
                Body = txtCorpo.Text
            };

            var erros = _emailTemplateService.Validar(template, nomeJaExiste);
            if (erros.Count > 0)
            {
                NotificacaoService.Erro(string.Join(" ", erros));
                return;
            }

            if (EmailTemplateIdEmEdicao.HasValue)
            {
                template.EmailTemplateId = EmailTemplateIdEmEdicao.Value;
                template.UpdatedBy = UserId;
                _emailTemplateService.Atualizar(template);
                NotificacaoService.Sucesso("Modelo atualizado.");
            }
            else
            {
                template.CreatedBy = UserId;
                template.IsActive = true;
                _emailTemplateService.Criar(template);
                NotificacaoService.Sucesso("Modelo criado.");
            }

            pnlFormulario.Visible = false;
            CarregarTemplates();
        }

        protected string GetEstadoTexto(object isActiveObj) => (bool)isActiveObj ? "Ativo" : "Inativo";

        protected string GetEstadoBadgeClasse(object isActiveObj) => (bool)isActiveObj ? "badge-ativo" : "badge-inativo";
    }
}