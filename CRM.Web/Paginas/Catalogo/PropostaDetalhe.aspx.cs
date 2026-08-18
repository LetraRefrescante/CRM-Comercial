using System;
using System.Linq;
using CRM.Services;
using CRM.Web.Helpers;

namespace CRM.Web.Paginas.Catalogo
{
    public partial class PropostaDetalhe : PaginaBase
    {
        private readonly ProposalService _proposalService = new ProposalService();
        private readonly SaleService _saleService = new SaleService();

        private int? ProposalId => int.TryParse(Request.QueryString["id"], out int id) ? id : (int?)null;

        public int VersaoAtualId => ProposalId ?? 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!ProposalId.HasValue)
            {
                Response.Redirect("PropostasLista.aspx");
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
                Response.Redirect("PropostasLista.aspx");
                return;
            }

            if (!_proposalService.PodeAceder(proposal, UserId, Perfil))
            {
                NotificacaoService.Erro("Não tens permissão para aceder a esta proposta.");
                Response.Redirect("PropostasLista.aspx");
                return;
            }

            lblNumero.Text = $"{proposal.ProposalNumber} · v{proposal.VersionNumber}";
            spanStatus.InnerText = proposal.Status;
            spanStatus.Attributes["class"] = "badge " + GetBadgeClasse(proposal.Status);

            lblCliente.Text = proposal.Client?.TradeName;
            lblEmissao.Text = proposal.IssueDate.ToString("dd/MM/yyyy");
            lblValidade.Text = proposal.ValidUntil.ToString("dd/MM/yyyy");
            lblComercial.Text = proposal.Client?.AccountManager?.Name ?? "—";

            phNotas.Visible = !string.IsNullOrWhiteSpace(proposal.Notes);
            lblNotas.Text = proposal.Notes;

            rptLinhas.DataSource = proposal.Lines;
            rptLinhas.DataBind();

            lblSubTotal.Text = proposal.SubTotal.ToString("C");
            lblIvaTotal.Text = proposal.TaxTotal.ToString("C");
            lblTotalGeral.Text = proposal.Total.ToString("C");

            var versoes = _proposalService.ListarVersoes(proposal.ProposalId);
            phVersoes.Visible = versoes.Count > 1;
            rptVersoes.DataSource = versoes;
            rptVersoes.DataBind();

            lnkEditar.NavigateUrl = $"PropostaEditar.aspx?id={proposal.ProposalId}";
            lnkEditar.Visible = _proposalService.PodeEditarDiretamente(proposal)
                && _proposalService.PodeCriarOuEditar(Perfil)
                && _proposalService.PodeAceder(proposal, UserId, Perfil);

            btnNovaVersao.Visible = _proposalService.PodeCriarNovaVersao(proposal, Perfil)
                && _proposalService.PodeAceder(proposal, UserId, Perfil);

            lnkEnviar.Visible = _proposalService.PodeEnviar(proposal, UserId, Perfil);
            lnkEnviar.NavigateUrl = $"PropostaEnviar.aspx?id={proposal.ProposalId}";

            lnkVerPdf.NavigateUrl = $"PropostaPDF.aspx?id={proposal.ProposalId}";

            phAceitarRecusar.Visible = _proposalService.PodeAceitarOuRecusar(proposal, UserId, Perfil);

            phInfoAceitacao.Visible = proposal.Status == ProposalService.StatusAceite;
            if (phInfoAceitacao.Visible)
            {
                lblDataAceitacao.Text = proposal.AcceptedDate?.ToString("dd/MM/yyyy HH:mm");
                lblQuemAceitou.Text = proposal.AcceptedByUser?.Name ?? "—";
                lblObservacaoAceitacao.Text = string.IsNullOrWhiteSpace(proposal.AcceptanceNotes)
                    ? ""
                    : $"— \"{proposal.AcceptanceNotes}\"";
            }

            bool jaTemVenda = proposal.Status == ProposalService.StatusAceite
                && _saleService.ExisteVendaParaProposta(proposal.ProposalId);

            lnkCriarVenda.Visible = proposal.Status == ProposalService.StatusAceite && !jaTemVenda
                && _saleService.PodeCriarOuEditar(Perfil);
            lnkCriarVenda.NavigateUrl = $"~/Vendas/VendaEditar.aspx?proposalId={proposal.ProposalId}";

            ucAnexos.Inicializar("Proposal", proposal.ProposalId, UserId);
            ucHistorico.Inicializar("Proposal", proposal.ProposalId.ToString());
        }

        protected void btnAceitar_Click(object sender, EventArgs e)
        {
            if (_proposalService.Aceitar(ProposalId.Value, txtObservacaoAceitacao.Text.Trim(), UserId, Perfil))
                NotificacaoService.Sucesso("Proposta aceite.");
            else
                NotificacaoService.Erro("Não foi possível aceitar a proposta.");

            CarregarProposta();
        }

        protected void btnRecusar_Click(object sender, EventArgs e)
        {
            if (_proposalService.Recusar(ProposalId.Value, txtMotivoRecusa.Text.Trim(), UserId, Perfil))
                NotificacaoService.Sucesso("Proposta recusada.");
            else
                NotificacaoService.Erro("Não foi possível recusar a proposta.");

            CarregarProposta();
        }

        protected void btnNovaVersao_Click(object sender, EventArgs e)
        {
            var proposal = _proposalService.GetById(ProposalId.Value);
            if (proposal == null || !_proposalService.PodeCriarNovaVersao(proposal, Perfil)
                || !_proposalService.PodeAceder(proposal, UserId, Perfil))
            {
                NotificacaoService.Erro("Não tens permissão para criar uma nova versão desta proposta.");
                return;
            }

            var novaVersao = _proposalService.CriarNovaVersao(ProposalId.Value, UserId);
            if (novaVersao == null)
            {
                NotificacaoService.Erro("Não foi possível criar uma nova versão.");
                return;
            }

            NotificacaoService.Sucesso($"Nova versão criada: {novaVersao.ProposalNumber} (v{novaVersao.VersionNumber}).");
            Response.Redirect($"PropostaEditar.aspx?id={novaVersao.ProposalId}");
        }

        protected string GetBadgeClasse(string status)
        {
            switch (status)
            {
                case "Rascunho": return "bg-secondary";
                case "Enviada": return "badge-em-contacto";
                case "Aceite": return "badge-ativo";
                case "Recusada": return "badge-bloqueado";
                case "Expirada": return "badge-inativo";
                case "Cancelada": return "badge-bloqueado";
                default: return "bg-secondary";
            }
        }

        protected string GetVersaoRowClass(object proposalId) =>
            (int)proposalId == VersaoAtualId ? "table-active" : "";
    }
}