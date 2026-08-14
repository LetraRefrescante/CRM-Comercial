using System;
using System.Linq;
using CRM.Services;
using CRM.Web.Helpers;

namespace CRM.Web.Paginas.Catalogo
{
    public partial class PropostaEnviar : PaginaBase
    {
        private readonly ProposalService _proposalService = new ProposalService();
        private readonly EmailService _emailService = new EmailService();

        private int? ProposalId => int.TryParse(Request.QueryString["id"], out int id) ? id : (int?)null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!ProposalId.HasValue)
            {
                Response.Redirect("~/Catalogo/PropostasLista.aspx");
                return;
            }

            if (!IsPostBack)
                CarregarProposta();
        }

        private void CarregarProposta()
        {
            var proposal = _proposalService.GetById(ProposalId.Value);
            if (proposal == null)
            {
                NotificacaoService.Erro("Proposta não encontrada.");
                Response.Redirect("~/Catalogo/PropostasLista.aspx");
                return;
            }

            if (!_proposalService.PodeEnviar(proposal, UserId, Perfil))
            {
                NotificacaoService.Erro("Esta proposta não pode ser enviada neste estado ou não tens permissão.");
                Response.Redirect($"PropostaDetalhe.aspx?id={ProposalId}");
                return;
            }

            lblNumero.Text = $"{proposal.ProposalNumber} · v{proposal.VersionNumber}";

            phJaEnviada.Visible = proposal.SentDate.HasValue;
            lblDataEnvioAnterior.Text = proposal.SentDate?.ToString("dd/MM/yyyy HH:mm");
            lblEmailAnterior.Text = proposal.SentToEmail;

            litNumeroPreview.Text = Server.HtmlEncode(proposal.ProposalNumber);
            litClientePreview.Text = Server.HtmlEncode(proposal.Client?.TradeName ?? "—");
            litTotalPreview.Text = proposal.Total.ToString("C");

            lnkVerPdfPreview.HRef = $"PropostaPDF.aspx?id={proposal.ProposalId}";

            txtEmail.Text = proposal.Client?.Email;
        }

        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            var erros = _proposalService.ValidarEnvio(txtEmail.Text.Trim());
            if (erros.Any())
            {
                NotificacaoService.Erro(string.Join(" ", erros));
                return;
            }

            if (!_proposalService.Enviar(ProposalId.Value, txtEmail.Text.Trim(), UserId, Perfil))
            {
                NotificacaoService.Erro("Não foi possível enviar esta proposta.");
                return;
            }

            // Envio real fica pendente do módulo de Notificações e Email (Fase 5) — mesma
            // estratégia do RecuperarPassword: a falha aqui não bloqueia o fluxo, porque
            // ProposalService.Enviar já registou o envio e a atividade.
            try
            {
                var proposal = _proposalService.GetById(ProposalId.Value);
                _emailService.Enviar(
                    txtEmail.Text.Trim(),
                    $"Proposta {proposal.ProposalNumber}",
                    $"Segue a proposta {proposal.ProposalNumber}, no valor de {proposal.Total:C}.");
            }
            catch (NotImplementedException)
            {
                System.Diagnostics.Trace.TraceWarning(
                    $"Email da proposta {ProposalId} não enviado de facto (serviço de email por implementar).");
            }

            NotificacaoService.Sucesso("Proposta marcada como enviada.");
            Response.Redirect($"PropostaDetalhe.aspx?id={ProposalId}");
        }
    }
}