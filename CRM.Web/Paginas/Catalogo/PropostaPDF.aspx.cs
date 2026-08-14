using System;
using System.Globalization;
using CRM.Data.Repositories;
using CRM.Services;

namespace CRM.Web.Paginas.Catalogo
{
    public partial class PropostaPDF : PaginaBase
    {
        private readonly ProposalService _proposalService = new ProposalService();
        private readonly SettingsRepository _settingsRepository = new SettingsRepository();

        private int? ProposalId => int.TryParse(Request.QueryString["id"], out int id) ? id : (int?)null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!ProposalId.HasValue)
            {
                Response.Redirect("~/Catalogo/PropostasLista.aspx");
                return;
            }

            var proposal = _proposalService.GetById(ProposalId.Value);
            if (proposal == null)
            {
                Response.Redirect("~/Catalogo/PropostasLista.aspx");
                return;
            }

            if (!_proposalService.PodeAceder(proposal, UserId, Perfil))
            {
                Response.Redirect("~/AcessoNegado.aspx");
                return;
            }

            var settings = _settingsRepository.ObterConfiguracaoAtual();
            string nomeEmpresa = settings?.CompanyName ?? "Empresa";

            Page.Title = "Proposta " + proposal.ProposalNumber;

            litEmpresa.Text = Server.HtmlEncode(nomeEmpresa);
            litEmpresaRodape.Text = Server.HtmlEncode(nomeEmpresa);
            litNumero.Text = Server.HtmlEncode(proposal.ProposalNumber);
            litVersao.Text = proposal.VersionNumber.ToString();

            litCliente.Text = Server.HtmlEncode(proposal.Client?.TradeName ?? "—");
            litMoradaCliente.Text = Server.HtmlEncode(proposal.Client?.Address ?? "");
            litNifCliente.Text = string.IsNullOrEmpty(proposal.Client?.VatNumber) ? "" : "NIF " + proposal.Client.VatNumber;

            litEmissao.Text = proposal.IssueDate.ToString("dd/MM/yyyy");
            litValidade.Text = proposal.ValidUntil.ToString("dd/MM/yyyy");

            rptLinhas.DataSource = proposal.Lines;
            rptLinhas.DataBind();

            litSubTotal.Text = proposal.SubTotal.ToString("C", CultureInfo.CurrentCulture);
            litIvaTotal.Text = proposal.TaxTotal.ToString("C", CultureInfo.CurrentCulture);
            litTotalGeral.Text = proposal.Total.ToString("C", CultureInfo.CurrentCulture);

            phCondicoes.Visible = proposal.PaymentTerm != null;
            litCondicoesPagamento.Text = Server.HtmlEncode(proposal.PaymentTerm?.Name ?? "");

            phNotas.Visible = !string.IsNullOrWhiteSpace(proposal.Notes);
            litNotas.Text = Server.HtmlEncode(proposal.Notes);

            litDataGeracao.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        }
    }
}