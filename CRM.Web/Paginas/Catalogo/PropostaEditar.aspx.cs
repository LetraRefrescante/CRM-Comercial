using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using CRM.Data.Repositories;
using CRM.Models.Entities.Catalogo;
using CRM.Services;
using CRM.Web.Helpers;

namespace CRM.Web.Paginas.Catalogo
{
    public partial class PropostaEditar : PaginaBase
    {
        private readonly ProposalService _proposalService = new ProposalService();
        private readonly OpportunityService _opportunityService = new OpportunityService();
        private readonly ClientRepository _clientRepository = new ClientRepository();
        private readonly PaymentTermRepository _paymentTermRepository = new PaymentTermRepository();
        private readonly TaxRateRepository _taxRateRepository = new TaxRateRepository();

        [Serializable]
        private class LinhaEdicao
        {
            public int ProposalLineId { get; set; } 
            public int ProductId { get; set; }
            public string ProductCode { get; set; }
            public string ProductName { get; set; }
            public string Description { get; set; }
            public decimal Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal DiscountPercent { get; set; }
            public int TaxRateId { get; set; }
            public string TaxRateName { get; set; }
            public decimal LineTotal { get; set; }
        }

        private int? ProposalId => int.TryParse(Request.QueryString["id"], out int id) ? id : (int?)null;
        private int? ClientIdInicial => int.TryParse(Request.QueryString["clientId"], out int id) ? id : (int?)null;
        private int? OpportunityIdInicial => int.TryParse(Request.QueryString["opportunityId"], out int id) ? id : (int?)null;

        public string TituloPagina => ProposalId.HasValue ? "Editar Proposta" : "Nova Proposta";

        private List<LinhaEdicao> Linhas
        {
            get => ViewState["Linhas"] as List<LinhaEdicao> ?? new List<LinhaEdicao>();
            set => ViewState["Linhas"] = value;
        }

        private int? LinhaIndexEmEdicao
        {
            get => ViewState["LinhaIndexEmEdicao"] as int?;
            set => ViewState["LinhaIndexEmEdicao"] = value;
        }

        private bool SomenteLeitura
        {
            get => ViewState["SomenteLeitura"] as bool? ?? false;
            set => ViewState["SomenteLeitura"] = value;
        }

        protected bool PodeEditarLinhas => !SomenteLeitura;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!_proposalService.PodeCriarOuEditar(Perfil))
            {
                Response.Redirect("~/AcessoNegado.aspx");
                return;
            }

            CarregarTaxasIva();

            if (!IsPostBack)
            {
                CarregarCondicoesPagamento();

                if (ProposalId.HasValue)
                {
                    CarregarProposta(ProposalId.Value);
                }
                else
                {
                    txtDataEmissao.Text = DateTime.Today.ToString("yyyy-MM-dd");
                    txtValidade.Text = DateTime.Today.AddDays(30).ToString("yyyy-MM-dd");

                    if (ClientIdInicial.HasValue)
                    {
                        var cliente = _clientRepository.GetById(ClientIdInicial.Value);
                        if (cliente != null)
                        {
                            ucCliente.ClienteId = cliente.ClientId;
                            CarregarOportunidades(cliente.ClientId);
                            if (OpportunityIdInicial.HasValue)
                                ddlOportunidade.SelectedValue = OpportunityIdInicial.Value.ToString();
                        }
                    }

                    AtualizarTotaisNaTela();
                }
            }
        }

        // ===================== Carregamento =====================

        private void CarregarCondicoesPagamento()
        {
            ddlCondicoesPagamento.Items.Clear();
            ddlCondicoesPagamento.Items.Add(new ListItem("(Nenhuma)", ""));
            foreach (var termo in _paymentTermRepository.ListarAtivas())
            {
                ddlCondicoesPagamento.Items.Add(new ListItem(termo.Name, termo.PaymentTermId.ToString()));
            }
        }

        private void CarregarTaxasIva()
        {
            ddlTaxaIvaLinha.Items.Clear();
            foreach (var taxa in _taxRateRepository.ListarAtivas())
            {
                ddlTaxaIvaLinha.Items.Add(new ListItem($"{taxa.Name} ({taxa.Percentage}%)", taxa.TaxRateId.ToString()));
            }
        }

        private void CarregarOportunidades(int clientId)
        {
            ddlOportunidade.Items.Clear();
            ddlOportunidade.Items.Add(new ListItem("(Sem oportunidade)", ""));
            foreach (var oportunidade in _opportunityService.ListarPorCliente(clientId))
            {
                ddlOportunidade.Items.Add(new ListItem(oportunidade.Title, oportunidade.OpportunityId.ToString()));
            }
        }

        private void CarregarProposta(int id)
        {
            var proposal = _proposalService.GetById(id);
            if (proposal == null)
            {
                NotificacaoService.Erro("Proposta não encontrada.");
                Response.Redirect("~/Catalogo/PropostasLista.aspx");
                return;
            }

            if (!_proposalService.PodeAceder(proposal, UserId, Perfil))
            {
                NotificacaoService.Erro("Não tens permissão para aceder a esta proposta.");
                Response.Redirect("~/Catalogo/PropostasLista.aspx");
                return;
            }

            phNumero.Visible = true;
            txtNumero.Text = proposal.ProposalNumber;

            ucCliente.ClienteId = proposal.ClientId;
            CarregarOportunidades(proposal.ClientId);
            ddlOportunidade.SelectedValue = proposal.OpportunityId?.ToString() ?? "";

            txtDataEmissao.Text = proposal.IssueDate.ToString("yyyy-MM-dd");
            txtValidade.Text = proposal.ValidUntil.ToString("yyyy-MM-dd");
            txtDescontoGlobal.Text = proposal.GlobalDiscountPercent.ToString("0.##", CultureInfo.InvariantCulture);
            ddlCondicoesPagamento.SelectedValue = proposal.PaymentTermId?.ToString() ?? "";
            txtNotas.Text = proposal.Notes;

            Linhas = proposal.Lines.Select(l => new LinhaEdicao
            {
                ProposalLineId = l.ProposalLineId,
                ProductId = l.ProductId,
                ProductCode = l.Product?.Code,
                ProductName = l.Product?.Name,
                Description = l.Description,
                Quantity = l.Quantity,
                UnitPrice = l.UnitPrice,
                DiscountPercent = l.DiscountPercent,
                TaxRateId = l.TaxRateId,
                TaxRateName = l.TaxRate?.Name,
                LineTotal = l.LineTotal
            }).ToList();

            ViewState["RowVersion"] = Convert.ToBase64String(proposal.RowVersion ?? new byte[0]);

            bool podeEditarDiretamente = _proposalService.PodeEditarDiretamente(proposal);
            SomenteLeitura = !podeEditarDiretamente;

            phBadgeEstado.Visible = true;
            spanEstado.InnerText = proposal.Status;
            spanEstado.Attributes["class"] = "badge fs-6 " + ObterClasseBadgeEstado(proposal.Status);

            phSoLeituraAviso.Visible = SomenteLeitura;
            btnGuardar.Visible = podeEditarDiretamente;
            btnCriarNovaVersao.Visible = SomenteLeitura && _proposalService.PodeCriarNovaVersao(proposal, Perfil);
            phFormularioLinha.Visible = podeEditarDiretamente;

            ucCliente.Enabled = podeEditarDiretamente;
            ddlOportunidade.Enabled = podeEditarDiretamente;
            txtDataEmissao.Enabled = podeEditarDiretamente;
            txtValidade.Enabled = podeEditarDiretamente;
            txtDescontoGlobal.Enabled = podeEditarDiretamente;
            ddlCondicoesPagamento.Enabled = podeEditarDiretamente;
            txtNotas.Enabled = podeEditarDiretamente;

            RenderizarLinhas();
            AtualizarTotaisNaTela();
        }

        private string ObterClasseBadgeEstado(string status)
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

        protected void ucCliente_ClienteSelecionado(object sender, EventArgs e)
        {
            if (ucCliente.ClienteId.HasValue)
                CarregarOportunidades(ucCliente.ClienteId.Value);
        }

        protected void CamposCabecalho_TextChanged(object sender, EventArgs e)
        {
            AtualizarTotaisNaTela();
        }

        // ===================== Linhas =====================

        private void RenderizarLinhas()
        {
            var linhas = Linhas;
            rptLinhas.DataSource = linhas;
            rptLinhas.DataBind();
            phVazioLinhas.Visible = linhas.Count == 0;
        }

        private void AtualizarTotaisNaTela()
        {
            decimal desconto = decimal.TryParse(txtDescontoGlobal.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal d) ? d : 0;

            var proposalTemp = new Proposal
            {
                GlobalDiscountPercent = desconto,
                Lines = Linhas.Select(l => new ProposalLine
                {
                    Quantity = l.Quantity,
                    UnitPrice = l.UnitPrice,
                    DiscountPercent = l.DiscountPercent,
                    TaxRateId = l.TaxRateId
                }).ToList()
            };

            _proposalService.CalcularTotais(proposalTemp);

            var cultura = new CultureInfo("pt-PT");
            litSubTotal.Text = proposalTemp.SubTotal.ToString("N2", cultura) + " €";
            litTaxTotal.Text = proposalTemp.TaxTotal.ToString("N2", cultura) + " €";
            litTotal.Text = proposalTemp.Total.ToString("N2", cultura) + " €";
        }

        protected void ucSeletorProduto_ProdutoSelecionado(object sender, EventArgs e)
        {
            var produto = ucSeletorProduto.ObterProdutoSelecionado();
            if (produto == null) return;

            txtDescricaoLinha.Text = produto.Name;
            txtPrecoLinha.Text = produto.BasePrice.ToString("0.00", CultureInfo.InvariantCulture);
            ddlTaxaIvaLinha.SelectedValue = produto.TaxRateId.ToString();
        }

        protected void cvRegrasLinha_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (!ucSeletorProduto.ProdutoId.HasValue)
            {
                args.IsValid = false;
                cvRegrasLinha.ErrorMessage = "Tens de selecionar um produto.";
                return;
            }

            if (!decimal.TryParse(txtQuantidadeLinha.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal quantidade) || quantidade <= 0)
            {
                args.IsValid = false;
                cvRegrasLinha.ErrorMessage = "A quantidade tem de ser superior a zero.";
                return;
            }

            if (!decimal.TryParse(txtDescontoLinha.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal desconto) || desconto < 0 || desconto > 100)
            {
                args.IsValid = false;
                cvRegrasLinha.ErrorMessage = "O desconto da linha tem de estar entre 0 e 100.";
                return;
            }

            if (string.IsNullOrEmpty(ddlTaxaIvaLinha.SelectedValue))
            {
                args.IsValid = false;
                cvRegrasLinha.ErrorMessage = "Tens de selecionar a taxa de IVA.";
                return;
            }

            args.IsValid = true;
        }

        protected void btnGuardarLinha_Click(object sender, EventArgs e)
        {
            if (SomenteLeitura) return;
            if (!Page.IsValid) return;

            var produto = ucSeletorProduto.ObterProdutoSelecionado();
            if (produto == null) return;

            var taxa = _taxRateRepository.GetById(int.Parse(ddlTaxaIvaLinha.SelectedValue));

            var linha = new LinhaEdicao
            {
                ProductId = produto.ProductId,
                ProductCode = produto.Code,
                ProductName = produto.Name,
                Description = string.IsNullOrWhiteSpace(txtDescricaoLinha.Text) ? produto.Name : txtDescricaoLinha.Text.Trim(),
                Quantity = decimal.Parse(txtQuantidadeLinha.Text, CultureInfo.InvariantCulture),
                UnitPrice = decimal.Parse(txtPrecoLinha.Text, CultureInfo.InvariantCulture),
                DiscountPercent = decimal.Parse(txtDescontoLinha.Text, CultureInfo.InvariantCulture),
                TaxRateId = taxa.TaxRateId,
                TaxRateName = taxa.Name
            };

            var linhas = Linhas;

            if (LinhaIndexEmEdicao.HasValue)
            {
                linha.ProposalLineId = linhas[LinhaIndexEmEdicao.Value].ProposalLineId;
                linhas[LinhaIndexEmEdicao.Value] = linha;
            }
            else
            {
                linha.ProposalLineId = 0;
                linhas.Add(linha);
            }

            Linhas = linhas;
            LimparFormularioLinha();
            RenderizarLinhas();
            AtualizarTotaisNaTela();
        }

        private void LimparFormularioLinha()
        {
            LinhaIndexEmEdicao = null;
            ucSeletorProduto.ProdutoId = null;
            txtDescricaoLinha.Text = "";
            txtQuantidadeLinha.Text = "1";
            txtPrecoLinha.Text = "";
            txtDescontoLinha.Text = "0";
            ddlTaxaIvaLinha.SelectedIndex = -1;
            litModoEdicaoLinha.Visible = false;
            btnCancelarLinha.Visible = false;
            btnGuardarLinha.Text = "Adicionar";
        }

        protected void btnCancelarLinha_Click(object sender, EventArgs e)
        {
            LimparFormularioLinha();
        }

        protected void rptLinhas_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (SomenteLeitura) return;

            int index = int.Parse(e.CommandArgument.ToString());
            var linhas = Linhas;

            if (e.CommandName == "Editar")
            {
                var linha = linhas[index];
                LinhaIndexEmEdicao = index;

                ucSeletorProduto.ProdutoId = linha.ProductId;
                txtDescricaoLinha.Text = linha.Description;
                txtQuantidadeLinha.Text = linha.Quantity.ToString(CultureInfo.InvariantCulture);
                txtPrecoLinha.Text = linha.UnitPrice.ToString("0.00", CultureInfo.InvariantCulture);
                txtDescontoLinha.Text = linha.DiscountPercent.ToString(CultureInfo.InvariantCulture);
                ddlTaxaIvaLinha.SelectedValue = linha.TaxRateId.ToString();

                litModoEdicaoLinha.Text = $"A editar linha: {linha.ProductName}";
                litModoEdicaoLinha.Visible = true;
                btnCancelarLinha.Visible = true;
                btnGuardarLinha.Text = "Guardar Linha";
                return;
            }

            if (e.CommandName == "Eliminar")
            {
                linhas.RemoveAt(index);
                Linhas = linhas;
                LimparFormularioLinha();
                RenderizarLinhas();
                AtualizarTotaisNaTela();
            }
        }

        // ===================== Guardar =====================

        protected void cvRegrasNegocio_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var proposal = MontarPropostaDoFormulario();
            var erros = _proposalService.Validar(proposal);

            args.IsValid = erros.Count == 0;
            cvRegrasNegocio.ErrorMessage = string.Join(" ", erros);
        }

        private Proposal MontarPropostaDoFormulario()
        {
            var proposal = new Proposal
            {
                ClientId = ucCliente.ClienteId ?? 0,
                OpportunityId = string.IsNullOrEmpty(ddlOportunidade.SelectedValue) ? (int?)null : int.Parse(ddlOportunidade.SelectedValue),
                IssueDate = DateTime.TryParse(txtDataEmissao.Text, out DateTime emissao) ? emissao : DateTime.Today,
                ValidUntil = DateTime.TryParse(txtValidade.Text, out DateTime validade) ? validade : DateTime.Today,
                GlobalDiscountPercent = decimal.TryParse(txtDescontoGlobal.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal desc) ? desc : 0,
                PaymentTermId = string.IsNullOrEmpty(ddlCondicoesPagamento.SelectedValue) ? (int?)null : int.Parse(ddlCondicoesPagamento.SelectedValue),
                Notes = string.IsNullOrWhiteSpace(txtNotas.Text) ? null : txtNotas.Text.Trim(),
                Lines = Linhas.Select(l => new ProposalLine
                {
                    ProposalLineId = l.ProposalLineId,
                    ProductId = l.ProductId,
                    Description = l.Description,
                    Quantity = l.Quantity,
                    UnitPrice = l.UnitPrice,
                    DiscountPercent = l.DiscountPercent,
                    TaxRateId = l.TaxRateId
                }).ToList()
            };

            if (ProposalId.HasValue)
            {
                proposal.ProposalId = ProposalId.Value;
                proposal.RowVersion = Convert.FromBase64String(ViewState["RowVersion"] as string ?? "");
            }

            return proposal;
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (SomenteLeitura)
            {
                NotificacaoService.Erro("Esta proposta já não pode ser editada diretamente — cria uma nova versão.");
                return;
            }

            if (!Page.IsValid) return;

            var proposal = MontarPropostaDoFormulario();

            try
            {
                if (ProposalId.HasValue)
                {
                    _proposalService.Atualizar(proposal, Perfil, UserId);
                    NotificacaoService.Sucesso("Proposta atualizada.");
                    Response.Redirect($"~/Catalogo/PropostaDetalhe.aspx?id={ProposalId.Value}");
                }
                else
                {
                    var criada = _proposalService.Criar(proposal, Perfil, UserId);
                    NotificacaoService.Sucesso("Proposta criada.");
                    Response.Redirect($"~/Catalogo/PropostaDetalhe.aspx?id={criada.ProposalId}");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                NotificacaoService.Erro(ex.Message);
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException)
            {
                NotificacaoService.Erro("Esta proposta foi alterada por outro utilizador entretanto. Recarrega a página e tenta novamente.");
            }
        }

        protected void btnCriarNovaVersao_Click(object sender, EventArgs e)
        {
            if (!ProposalId.HasValue) return;

            var novaVersao = _proposalService.CriarNovaVersao(ProposalId.Value, UserId);
            if (novaVersao == null)
            {
                NotificacaoService.Erro("Não foi possível criar uma nova versão.");
                return;
            }

            NotificacaoService.Sucesso($"Nova versão criada (v{novaVersao.VersionNumber}).");
            Response.Redirect($"~/Catalogo/PropostaEditar.aspx?id={novaVersao.ProposalId}");
        }
    }
}