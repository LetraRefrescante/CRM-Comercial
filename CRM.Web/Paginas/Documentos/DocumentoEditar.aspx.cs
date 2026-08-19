using System;
using System.IO;
using System.Web.UI.WebControls;
using CRM.Services;
using CRM.Web.Helpers;

namespace CRM.Web.Paginas.Documentos
{
    public partial class DocumentoEditar : PaginaBase
    {
        private readonly DocumentService _documentService = new DocumentService();
        private readonly LeadService _leadService = new LeadService();
        private readonly OpportunityService _opportunityService = new OpportunityService();
        private readonly ProposalService _proposalService = new ProposalService();
        private readonly SaleService _saleService = new SaleService();

        private const string PastaArmazenamento = "~/App_Data/DocumentUploads/";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!_documentService.PodeCarregar(Perfil))
            {
                NotificacaoService.Erro("Não tens permissão para carregar documentos.");
                Response.Redirect("~/Dashboard/Dashboard.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CarregarLeads();
                CarregarOportunidades();
                CarregarPropostas();
                CarregarVendas();
            }
        }

        private void CarregarLeads()
        {
            ddlLead.Items.Clear();
            ddlLead.Items.Add(new ListItem("Selecione...", ""));
            foreach (var lead in _leadService.ListarParaSelecao(Perfil, UserId))
                ddlLead.Items.Add(new ListItem(lead.Name, lead.LeadId.ToString()));
        }

        private void CarregarOportunidades()
        {
            ddlOportunidade.Items.Clear();
            ddlOportunidade.Items.Add(new ListItem("Selecione...", ""));
            foreach (var opportunity in _opportunityService.ListarParaSelecao(Perfil, UserId))
                ddlOportunidade.Items.Add(new ListItem(opportunity.Title, opportunity.OpportunityId.ToString()));
        }

        private void CarregarPropostas()
        {
            ddlProposta.Items.Clear();
            ddlProposta.Items.Add(new ListItem("Selecione...", ""));
            foreach (var proposal in _proposalService.ListarParaSelecao(Perfil, UserId))
                ddlProposta.Items.Add(new ListItem(proposal.ProposalNumber, proposal.ProposalId.ToString()));
        }

        private void CarregarVendas()
        {
            ddlVenda.Items.Clear();
            ddlVenda.Items.Add(new ListItem("Selecione...", ""));
            foreach (var sale in _saleService.ListarParaSelecao(Perfil, UserId))
                ddlVenda.Items.Add(new ListItem(sale.SaleNumber, sale.SaleId.ToString()));
        }

        protected void ddlTipoRelacao_SelectedIndexChanged(object sender, EventArgs e) => AtualizarVisibilidadeRelacao();

        private void AtualizarVisibilidadeRelacao()
        {
            pnlCliente.Visible = ddlTipoRelacao.SelectedValue == "Client";
            pnlLead.Visible = ddlTipoRelacao.SelectedValue == "Lead";
            pnlOportunidade.Visible = ddlTipoRelacao.SelectedValue == "Opportunity";
            pnlProposta.Visible = ddlTipoRelacao.SelectedValue == "Proposal";
            pnlVenda.Visible = ddlTipoRelacao.SelectedValue == "Sale";
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!fuFicheiro.HasFile)
            {
                NotificacaoService.Erro("Tens de escolher um ficheiro.");
                return;
            }

            if (!_documentService.ExtensaoPermitida(fuFicheiro.FileName))
            {
                NotificacaoService.Erro("Extensão de ficheiro não permitida.");
                return;
            }

            if (!_documentService.MimeCorrespondeExtensao(fuFicheiro.FileName, fuFicheiro.PostedFile.ContentType))
            {
                NotificacaoService.Erro("O tipo de ficheiro não corresponde à extensão indicada.");
                return;
            }

            if (!_documentService.TamanhoPermitido(fuFicheiro.PostedFile.ContentLength))
            {
                NotificacaoService.Erro("O ficheiro excede o tamanho máximo permitido (10 MB).");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTitulo.Text))
            {
                NotificacaoService.Erro("O título é obrigatório.");
                return;
            }

            if (!ObterEntidadeSelecionada(out string entityType, out int entityId))
            {
                NotificacaoService.Erro("Seleciona a que registo este documento está relacionado.");
                return;
            }

            string extensao = Path.GetExtension(fuFicheiro.FileName);
            string storedFileName = Guid.NewGuid().ToString("N") + extensao;

            string pastaFisica = Server.MapPath(PastaArmazenamento);
            if (!Directory.Exists(pastaFisica))
                Directory.CreateDirectory(pastaFisica);

            fuFicheiro.PostedFile.SaveAs(Path.Combine(pastaFisica, storedFileName));

            _documentService.Guardar(
                entityType, entityId, ddlCategoria.SelectedValue,
                storedFileName, fuFicheiro.FileName, fuFicheiro.PostedFile.ContentType,
                fuFicheiro.PostedFile.ContentLength, chkConfidencial.Checked,
                UserId, Request.UserHostAddress);

            NotificacaoService.Sucesso("Documento carregado.");

            Response.Redirect(ResolveUrl(ResolverUrlEntidade(entityType, entityId)));
        }

        private bool ObterEntidadeSelecionada(out string entityType, out int entityId)
        {
            entityType = ddlTipoRelacao.SelectedValue;
            entityId = 0;

            switch (entityType)
            {
                case "Client":
                    if (!ucCliente.ClienteId.HasValue) return false;
                    entityId = ucCliente.ClienteId.Value;
                    return true;
                case "Lead":
                    if (string.IsNullOrEmpty(ddlLead.SelectedValue)) return false;
                    entityId = int.Parse(ddlLead.SelectedValue);
                    return true;
                case "Opportunity":
                    if (string.IsNullOrEmpty(ddlOportunidade.SelectedValue)) return false;
                    entityId = int.Parse(ddlOportunidade.SelectedValue);
                    return true;
                case "Proposal":
                    if (string.IsNullOrEmpty(ddlProposta.SelectedValue)) return false;
                    entityId = int.Parse(ddlProposta.SelectedValue);
                    return true;
                case "Sale":
                    if (string.IsNullOrEmpty(ddlVenda.SelectedValue)) return false;
                    entityId = int.Parse(ddlVenda.SelectedValue);
                    return true;
                default:
                    return false;
            }
        }

        private string ResolverUrlEntidade(string entityType, int entityId)
        {
            switch (entityType)
            {
                case "Client": return $"~/Clientes/ClienteDetalhe.aspx?id={entityId}";
                case "Lead": return $"~/Leads/LeadDetalhe.aspx?id={entityId}";
                case "Opportunity": return $"~/Oportunidades/OportunidadeDetalhe.aspx?id={entityId}";
                case "Proposal": return $"~/Catalogo/PropostaDetalhe.aspx?id={entityId}";
                case "Sale": return $"~/Vendas/VendaDetalhe.aspx?id={entityId}";
                default: return "~/Dashboard/Dashboard.aspx";
            }
        }
    }
}