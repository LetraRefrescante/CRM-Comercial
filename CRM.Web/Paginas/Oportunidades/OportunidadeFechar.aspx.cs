using System;
using System.Globalization;
using System.Web.UI.WebControls;
using CRM.Data.Repositories;
using CRM.Services;
using CRM.Web.Helpers;

namespace CRM.Web.Oportunidades
{
    public partial class OportunidadeFechar : System.Web.UI.Page
    {
        private readonly OpportunityService _opportunityService = new OpportunityService();
        private readonly LossReasonRepository _lossReasonRepository = new LossReasonRepository();

        private string Perfil => Session["RoleName"] as string ?? "";
        private int UserId => Session["UserId"] != null ? (int)Session["UserId"] : 0;

        private int? OpportunityId
        {
            get
            {
                if (int.TryParse(Request.QueryString["id"], out int id)) return id;
                return null;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!_opportunityService.PodeFechar(Perfil))
            {
                NotificacaoService.Erro("Não tens permissão para fechar oportunidades.");
                Response.Redirect("~/Oportunidades/OportunidadesLista.aspx");
                return;
            }

            if (!OpportunityId.HasValue)
            {
                Response.Redirect("~/Oportunidades/OportunidadesLista.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CarregarMotivosPerda();
                CarregarOportunidade();
            }
        }

        private void CarregarMotivosPerda()
        {
            ddlMotivoPerda.Items.Clear();
            ddlMotivoPerda.Items.Add(new ListItem("-- Selecionar --", ""));
            foreach (var motivo in _lossReasonRepository.ListarAtivos())
            {
                ddlMotivoPerda.Items.Add(new ListItem(motivo.Name, motivo.LossReasonId.ToString()));
            }
        }

        private void CarregarOportunidade()
        {
            var opportunity = _opportunityService.ObterPorId(OpportunityId.Value, Perfil, UserId);
            if (opportunity == null)
            {
                NotificacaoService.Erro("Oportunidade não encontrada ou sem permissão para a fechar.");
                Response.Redirect("~/Oportunidades/OportunidadesLista.aspx");
                return;
            }

            if (opportunity.IsClosed)
            {
                NotificacaoService.Erro("Esta oportunidade já está fechada.");
                Response.Redirect("~/Oportunidades/OportunidadesLista.aspx");
                return;
            }

            litTitulo.Text = Server.HtmlEncode(opportunity.Title);
            litCliente.Text = Server.HtmlEncode(opportunity.Client?.TradeName ?? "—");
            litComercial.Text = Server.HtmlEncode(opportunity.Owner?.Name ?? "—");
            litFase.Text = Server.HtmlEncode(opportunity.Stage?.Name ?? "—");
            litValor.Text = opportunity.EstimatedValue.ToString("N2", CultureInfo.InvariantCulture) + " €";
            litDataFecho.Text = opportunity.ExpectedCloseDate.ToString("dd/MM/yyyy");
        }

        protected void rblResultado_SelectedIndexChanged(object sender, EventArgs e)
        {
            phMotivoPerda.Visible = rblResultado.SelectedValue == "perdido";
        }

        protected void cvMotivoPerda_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (rblResultado.SelectedValue != "perdido")
            {
                args.IsValid = true;
                return;
            }

            args.IsValid = int.TryParse(args.Value, out int id) && id > 0;
        }

        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            bool ganho = rblResultado.SelectedValue == "ganho";
            int? lossReasonId = !ganho && !string.IsNullOrEmpty(ddlMotivoPerda.SelectedValue)
                ? int.Parse(ddlMotivoPerda.SelectedValue)
                : (int?)null;

            string erro = _opportunityService.Fechar(OpportunityId.Value, ganho, lossReasonId, Perfil, UserId);

            if (erro != null)
            {
                NotificacaoService.Erro(erro);
                return;
            }

            NotificacaoService.Sucesso(ganho
                ? "Oportunidade fechada como Ganha."
                : "Oportunidade fechada como Perdida.");

            Response.Redirect("~/Oportunidades/OportunidadesLista.aspx");
        }
    }
}