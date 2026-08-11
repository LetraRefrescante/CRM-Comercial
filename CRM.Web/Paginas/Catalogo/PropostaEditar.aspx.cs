using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using CRM.Data.Repositories;
using CRM.Models.Entities.Catalogo;
using CRM.Services;
using CRM.Web.Controls;
using CRM.Web.Helpers;

namespace CRM.Web.Paginas.Catalogo
{
    public partial class PropostaEditar : PaginaBase
    {
        private readonly ProposalService _proposalService = new ProposalService();
        private readonly OpportunityRepository _opportunityRepository = new OpportunityRepository();
        private readonly PaymentTermRepository _paymentTermRepository = new PaymentTermRepository();
        private readonly TaxRateRepository _taxRateRepository = new TaxRateRepository();

        private List<TaxRate> _taxasIva;
        private List<TaxRate> GetTaxasIva() => _taxasIva ?? (_taxasIva = _taxRateRepository.ListarAtivas());

        private int? ProposalId => ViewState["ProposalId"] as int?;
        private string StatusAtual => ViewState["Status"] as string ?? ProposalService.StatusRascunho;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarListasAuxiliares();

                if (int.TryParse(Request.QueryString["id"], out int id))
                    CarregarProposta(id);
                else
                    CarregarNova();
            }
        }

        private void CarregarListasAuxiliares()
        {
            ddlCondicaoPagamento.Items.Add(new ListItem("(Nenhuma)", ""));
            foreach (var pt in _paymentTermRepository.ListarAtivas())
                ddlCondicaoPagamento.Items.Add(new ListItem(pt.Name, pt.PaymentTermId.ToString()));
        }

        private void CarregarOportunidades(int clientId)
        {
            ddlOportunidade.Items.Clear();
            ddlOportunidade.Items.Add(new ListItem("(Nenhuma)", ""));

            // ASSUNÇÃO: OpportunityRepository.ListarPorCliente(clientId) — cria este método se ainda não existir.
            foreach (var op in _opportunityRepository.ListarPorCliente(clientId))
                ddlOportunidade.Items.Add(new ListItem(op.Title, op.OpportunityId.ToString()));
        }

        private void CarregarNova()
        {
            lblNumero.Text = "(gerado ao gravar)";
            spanStatus.InnerText = ProposalService.StatusRascunho;
            spanStatus.Attributes["class"] = "badge bg-secondary";
            txtEmissao.Text = DateTime.Today.ToString("dd/MM/yyyy");
            txtValidade.Text = DateTime.Today.AddDays(30).ToString("dd/MM/yyyy");

            RebindLinhas(new List<ProposalLine> { NovaLinhaVazia() });

            AtualizarVisibilidadeBotoes(null);
        }

        private void CarregarProposta(int proposalId)
        {
            var proposal = _proposalService.GetById(proposalId);
            if (proposal == null)
            {
                Response.Redirect("~/AcessoNegado.aspx");
                return;
            }

            if (!_proposalService.PodeAceder(proposal, UserId, Perfil))
            {
                Response.Redirect("~/AcessoNegado.aspx");
                return;
            }

            ViewState["ProposalId"] = proposal.ProposalId;
            ViewState["Status"] = proposal.Status;

            ucCliente.ClienteId = proposal.ClientId;

            lblNumero.Text = $"{proposal.ProposalNumber} · v{proposal.VersionNumber}";
            spanStatus.InnerText = proposal.Status;
            spanStatus.Attributes["class"] = "badge " + GetBadgeClasse(proposal.Status);

            txtEmissao.Text = proposal.IssueDate.ToString("dd/MM/yyyy");
            txtValidade.Text = proposal.ValidUntil.ToString("dd/MM/yyyy");
            txtDescontoGlobal.Text = proposal.GlobalDiscountPercent.ToString("0.##");
            txtNotas.Text = proposal.Notes;

            CarregarOportunidades(proposal.ClientId);
            if (proposal.OpportunityId.HasValue)
                ddlOportunidade.SelectedValue = proposal.OpportunityId.Value.ToString();

            if (proposal.PaymentTermId.HasValue)
                ddlCondicaoPagamento.SelectedValue = proposal.PaymentTermId.Value.ToString();

            var linhas = proposal.Lines.Any() ? proposal.Lines.ToList() : new List<ProposalLine> { NovaLinhaVazia() };
            RebindLinhas(linhas);

            AtualizarVisibilidadeBotoes(proposal);
        }

        private ProposalLine NovaLinhaVazia() => new ProposalLine { Quantity = 1 };

        // ===================== Repeater: colher / recompor (sem ViewModel) =====================

        private List<ProposalLine> ColherLinhasDosControles()
        {
            var linhas = new List<ProposalLine>();

            foreach (RepeaterItem item in rptLinhas.Items)
            {
                if (item.ItemType != ListItemType.Item && item.ItemType != ListItemType.AlternatingItem)
                    continue;

                var ucProduto = (SeletorProduto)item.FindControl("ucProduto");
                var hdnProposalLineId = (HiddenField)item.FindControl("hdnProposalLineId");
                var hdnUnitPrice = (HiddenField)item.FindControl("hdnUnitPrice");
                var hdnTaxRateId = (HiddenField)item.FindControl("hdnTaxRateId");
                var txtDescricao = (TextBox)item.FindControl("txtDescricao");
                var txtQuantidade = (TextBox)item.FindControl("txtQuantidade");
                var txtDesconto = (TextBox)item.FindControl("txtDesconto");

                int taxRateId = int.TryParse(hdnTaxRateId.Value, out int tid) ? tid : 0;

                linhas.Add(new ProposalLine
                {
                    ProposalLineId = int.TryParse(hdnProposalLineId.Value, out int lid) ? lid : 0,
                    ProductId = ucProduto.ProdutoId ?? 0,
                    Description = txtDescricao.Text.Trim(),
                    Quantity = decimal.TryParse(txtQuantidade.Text, out decimal q) ? q : 0,
                    UnitPrice = decimal.TryParse(hdnUnitPrice.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal up) ? up : 0,
                    DiscountPercent = decimal.TryParse(txtDesconto.Text, out decimal d) ? d : 0,
                    TaxRateId = taxRateId,
                    TaxRate = GetTaxasIva().SingleOrDefault(t => t.TaxRateId == taxRateId)
                });
            }

            return linhas;
        }

        private void RebindLinhas(List<ProposalLine> linhas)
        {
            var linhasValidas = linhas.Where(l => l.ProductId > 0).ToList();

            var proposalTemp = new Proposal
            {
                GlobalDiscountPercent = ObterDescontoGlobal(),
                Lines = linhasValidas
            };

            if (linhasValidas.Any())
                _proposalService.CalcularTotais(proposalTemp);

            rptLinhas.DataSource = linhas;
            rptLinhas.DataBind();

            phSemLinhas.Visible = !linhas.Any();

            lblSubTotal.Text = proposalTemp.SubTotal.ToString("C");
            lblIvaTotal.Text = proposalTemp.TaxTotal.ToString("C");
            lblTotalGeral.Text = proposalTemp.Total.ToString("C");
        }

        private decimal ObterDescontoGlobal() =>
            decimal.TryParse(txtDescontoGlobal.Text, out decimal d) ? d : 0;

        protected void rptLinhas_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var linha = (ProposalLine)e.Item.DataItem;

            var ucProduto = (SeletorProduto)e.Item.FindControl("ucProduto");
            ucProduto.ProdutoId = linha.ProductId > 0 ? linha.ProductId : (int?)null;
            ucProduto.ProdutoSelecionado += (s, args) => ucProduto_ProdutoSelecionado(ucProduto, e.Item);

            ((HiddenField)e.Item.FindControl("hdnUnitPrice")).Value =
                linha.UnitPrice.ToString(CultureInfo.InvariantCulture);
            ((HiddenField)e.Item.FindControl("hdnTaxRateId")).Value =
                linha.TaxRateId.ToString();

            ((Label)e.Item.FindControl("lblPrecoUnit")).Text = linha.UnitPrice.ToString("C");
            ((Label)e.Item.FindControl("lblIva")).Text =
                linha.TaxRate != null ? $"{linha.TaxRate.Percentage:0.##}%" : "—";
            ((Label)e.Item.FindControl("lblTotalLinha")).Text = linha.LineTotal.ToString("C");
        }

        private void ucProduto_ProdutoSelecionado(SeletorProduto ucProduto, RepeaterItem item)
        {
            var produto = ucProduto.ObterProdutoSelecionado();
            if (produto == null) return;

            ((HiddenField)item.FindControl("hdnUnitPrice")).Value =
                produto.BasePrice.ToString(CultureInfo.InvariantCulture);
            ((HiddenField)item.FindControl("hdnTaxRateId")).Value =
                produto.TaxRateId.ToString();

            var txtDescricao = (TextBox)item.FindControl("txtDescricao");
            if (string.IsNullOrWhiteSpace(txtDescricao.Text))
                txtDescricao.Text = produto.Name;

            RebindLinhas(ColherLinhasDosControles());
        }

        protected void txtLinha_TextChanged(object sender, EventArgs e) =>
            RebindLinhas(ColherLinhasDosControles());

        protected void txtDescontoGlobal_TextChanged(object sender, EventArgs e) =>
            RebindLinhas(ColherLinhasDosControles());

        protected void ucCliente_ClienteSelecionado(object sender, EventArgs e)
        {
            // ASSUNÇÃO de contrato do SeletorCliente — confirma se expõe ClienteId (int?) assim.
            if (ucCliente.ClienteId.HasValue)
                CarregarOportunidades(ucCliente.ClienteId.Value);
        }

        protected void btnAdicionarLinha_Click(object sender, EventArgs e)
        {
            var linhas = ColherLinhasDosControles();
            linhas.Add(NovaLinhaVazia());
            RebindLinhas(linhas);
        }

        protected void rptLinhas_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Remover") return;

            var linhas = ColherLinhasDosControles();
            linhas.RemoveAt(e.Item.ItemIndex);

            if (!linhas.Any())
                linhas.Add(NovaLinhaVazia());

            RebindLinhas(linhas);
        }

        // ===================== Permissões / visibilidade =====================

        private void AtualizarVisibilidadeBotoes(Proposal proposal)
        {
            bool ehNova = proposal == null;

            bool temPermissaoPerfil = _proposalService.PodeCriarOuEditar(Perfil);
            bool ehDono = ehNova || _proposalService.PodeAceder(proposal, UserId, Perfil);
            bool estadoPermiteEdicaoDireta = ehNova || _proposalService.PodeEditarDiretamente(proposal);

            bool podeEditar = temPermissaoPerfil && ehDono && estadoPermiteEdicaoDireta;
            bool podeNovaVersao = !ehNova && temPermissaoPerfil && ehDono &&
                _proposalService.PodeCriarNovaVersao(proposal, Perfil);

            pnlCamposEditaveis.Enabled = podeEditar;
            btnGuardar.Visible = podeEditar;
            btnAdicionarLinha.Visible = podeEditar;
            btnCriarNovaVersao.Visible = podeNovaVersao;
            phAvisoSoLeitura.Visible = !podeEditar;
        }

        // ===================== Validação =====================

        protected void cvLinhas_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ColherLinhasDosControles().Any(l => l.ProductId > 0);
        }

        // ===================== Gravação =====================

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            // NOVO: revalida no servidor, não confiar só no Visible/Enabled dos controlos —
            // um POST direto ao evento contornava isso antes.
            var proposalExistente = ProposalId.HasValue ? _proposalService.GetById(ProposalId.Value) : null;

            bool podeGravar = _proposalService.PodeCriarOuEditar(Perfil)
                && (proposalExistente == null || _proposalService.PodeAceder(proposalExistente, UserId, Perfil))
                && (proposalExistente == null || _proposalService.PodeEditarDiretamente(proposalExistente));

            if (!podeGravar)
            {
                NotificacaoService.Erro("Não tens permissão para gravar esta proposta.");
                return;
            }

            var linhas = ColherLinhasDosControles().Where(l => l.ProductId > 0).ToList();

            var proposal = new Proposal
            {
                ProposalId = ProposalId ?? 0,
                ClientId = ucCliente.ClienteId ?? 0,
                OpportunityId = string.IsNullOrEmpty(ddlOportunidade.SelectedValue) ? (int?)null : int.Parse(ddlOportunidade.SelectedValue),
                PaymentTermId = string.IsNullOrEmpty(ddlCondicaoPagamento.SelectedValue) ? (int?)null : int.Parse(ddlCondicaoPagamento.SelectedValue),
                IssueDate = DateTime.Parse(txtEmissao.Text),
                ValidUntil = DateTime.Parse(txtValidade.Text),
                GlobalDiscountPercent = ObterDescontoGlobal(),
                Notes = txtNotas.Text.Trim(),
                Lines = linhas
            };

            var erros = _proposalService.Validar(proposal);
            if (erros.Any())
            {
                NotificacaoService.Erro(string.Join(" ", erros));
                return;
            }

            if (ProposalId.HasValue)
            {
                proposal.Status = StatusAtual;
                _proposalService.Atualizar(proposal, UserId);
                NotificacaoService.Sucesso("Proposta atualizada.");
                Response.Redirect($"PropostaEditar.aspx?id={ProposalId}");
            }
            else
            {
                var criada = _proposalService.Criar(proposal, UserId);
                NotificacaoService.Sucesso("Proposta criada.");
                Response.Redirect($"PropostaEditar.aspx?id={criada.ProposalId}");
            }
        }

        protected void btnCriarNovaVersao_Click(object sender, EventArgs e)
        {
            if (!ProposalId.HasValue) return;

            // NOVO: mesma revalidação no servidor.
            var proposalExistente = _proposalService.GetById(ProposalId.Value);
            if (proposalExistente == null
                || !_proposalService.PodeCriarNovaVersao(proposalExistente, Perfil)
                || !_proposalService.PodeAceder(proposalExistente, UserId, Perfil))
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
    }
}