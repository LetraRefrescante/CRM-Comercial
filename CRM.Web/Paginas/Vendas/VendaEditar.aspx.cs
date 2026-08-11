using CRM.Data.Repositories;
using CRM.Models.Entities.Catalogo;
using CRM.Models.Entities.Vendas;
using CRM.Services;
using CRM.Web.Controls;
using CRM.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace CRM.Web.Paginas.Vendas
{
    public partial class VendaEditar : PaginaBase
    {
        private readonly SaleService _saleService = new SaleService();
        private readonly PaymentService _paymentService = new PaymentService();
        private readonly UserRepository _userRepository = new UserRepository();
        private readonly TaxRateRepository _taxRateRepository = new TaxRateRepository();
        private readonly ProposalRepository _proposalRepository = new ProposalRepository();

        // ===================== Estado auxiliar (ViewState) =====================

        private int? SaleId
        {
            get => ViewState["SaleId"] as int?;
            set => ViewState["SaleId"] = value;
        }

        private bool PodeEditar
        {
            get => ViewState["PodeEditar"] as bool? ?? true;
            set => ViewState["PodeEditar"] = value;
        }
        private List<LinhaEdicao> Linhas
        {
            get => ViewState["Linhas"] as List<LinhaEdicao> ?? new List<LinhaEdicao>();
            set => ViewState["Linhas"] = value;
        }

        [Serializable]
        private class LinhaEdicao
        {
            public int SaleLineId { get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public int LineOrder { get; set; }
            public string Description { get; set; }
            public decimal Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal DiscountPercent { get; set; }
            public int TaxRateId { get; set; }
            public decimal TaxPercentage { get; set; }
            public decimal LineTotal { get; set; }
        }

        // ===================== Ciclo de vida =====================

        protected void Page_Load(object sender, EventArgs e)
        {
            ucCliente.Obrigatorio = true;

            if (!IsPostBack)
            {
                CarregarComerciais();

                int saleId = ObterIdDaQueryString();
                if (saleId > 0)
                    CarregarVenda(saleId);
                else
                    NovaVenda();
            }
            RenderizarLinhas();
            RecalcularTotais();
        }

        private int ObterIdDaQueryString()
            => int.TryParse(Request.QueryString["id"], out int id) ? id : 0;

        // ===================== Carregamento =====================

        private void CarregarComerciais()
        {
            ddlComercial.Items.Clear();

            if (_saleService.TemAmbitoProprios(Perfil))
            {
                var utilizadorAtual = _userRepository.ListarComerciaisAtivos()
                    .SingleOrDefault(u => u.UserId == UserId);

                ddlComercial.Items.Add(new ListItem(utilizadorAtual?.Name ?? "Eu", UserId.ToString()));
                ddlComercial.Enabled = false;
            }
            else
            {
                foreach (var user in _userRepository.ListarComerciaisAtivos())
                    ddlComercial.Items.Add(new ListItem(user.Name, user.UserId.ToString()));
            }
        }

        private void NovaVenda()
        {
            SaleId = null;
            PodeEditar = _saleService.PodeCriarOuEditar(Perfil);

            Linhas = new List<LinhaEdicao>();

            lblNumero.Text = "(nova)";
            spanStatus.InnerText = SaleService.StatusPendente;
            spanStatus.Attributes["class"] = "badge " + EstadoParaClasseBadge(SaleService.StatusPendente);

            ucCliente.Enabled = PodeEditar;

            txtDataVenda.Text = DateTime.Today.ToString("dd/MM/yyyy");
            ddlOrigem.SelectedValue = SaleService.OrigemManual;
            phSeletorProposta.Visible = false;

            phAvisoSoLeitura.Visible = false;
            pnlCamposEditaveis.Enabled = PodeEditar;
            btnGuardar.Visible = PodeEditar;
            btnConfirmar.Visible = false;

            phPagamentos.Visible = false;
            phCancelamento.Visible = false;
            phAnexosHistorico.Visible = false;
        }

        private void CarregarVenda(int saleId)
        {
            var sale = _saleService.GetById(saleId);

            if (sale == null)
            {
                NotificacaoService.Erro("Venda não encontrada.");
                Response.Redirect("VendasLista.aspx");
                return;
            }

            if (!_saleService.PodeAceder(sale, UserId, Perfil))
            {
                NotificacaoService.Erro("Não tens permissão para aceder a esta venda.");
                Response.Redirect("VendasLista.aspx");
                return;
            }

            SaleId = sale.SaleId;
            PodeEditar = _saleService.PodeEditarDiretamente(sale) && _saleService.PodeCriarOuEditar(Perfil);

            lblNumero.Text = "#" + sale.SaleNumber;
            spanStatus.InnerText = sale.Status;
            spanStatus.Attributes["class"] = "badge " + EstadoParaClasseBadge(sale.Status);

            ucCliente.ClienteId = sale.ClientId;
            ucCliente.Enabled = PodeEditar;

            if (ddlComercial.Items.FindByValue(sale.OwnerId.ToString()) == null)
                ddlComercial.Items.Insert(0, new ListItem(sale.Owner?.Name ?? $"Utilizador #{sale.OwnerId}", sale.OwnerId.ToString()));
            ddlComercial.SelectedValue = sale.OwnerId.ToString();

            ddlOrigem.SelectedValue = sale.Origin;
            phSeletorProposta.Visible = sale.Origin == SaleService.OrigemProposta;

            if (phSeletorProposta.Visible)
            {
                CarregarPropostas(sale.ClientId);

                if (sale.ProposalId.HasValue)
                {
                    if (ddlProposta.Items.FindByValue(sale.ProposalId.Value.ToString()) == null)
                        ddlProposta.Items.Add(new ListItem(
                            sale.Proposal?.ProposalNumber ?? $"Proposta #{sale.ProposalId}",
                            sale.ProposalId.Value.ToString()));

                    ddlProposta.SelectedValue = sale.ProposalId.Value.ToString();
                }
            }

            txtDataVenda.Text = sale.SaleDate.ToString("dd/MM/yyyy");
            txtDataVencimento.Text = sale.DueDate?.ToString("dd/MM/yyyy") ?? "";
            ddlMetodoPagamento.SelectedValue = sale.PaymentMethod ?? "";
            txtComissao.Text = sale.CommissionValue?.ToString("0.00") ?? "";

            Linhas = MapearLinhas(sale.Lines);

            phAvisoSoLeitura.Visible = !PodeEditar;
            pnlCamposEditaveis.Enabled = PodeEditar;
            btnGuardar.Visible = PodeEditar;
            btnConfirmar.Visible = PodeEditar && sale.Status == SaleService.StatusPendente;

            // ===================== Pagamentos =====================
            phPagamentos.Visible = sale.Status != SaleService.StatusCancelada;

            if (phPagamentos.Visible)
            {
                var pagamentos = _paymentService.ListarPorVenda(saleId);
                decimal totalPago = _paymentService.TotalPago(saleId);

                rptPagamentos.DataSource = pagamentos;
                rptPagamentos.DataBind();
                phSemPagamentos.Visible = pagamentos.Count == 0;

                lblTotalPago.Text = totalPago.ToString("C");
                lblSaldoEmAberto.Text = (sale.Total - totalPago).ToString("C");

                pnlNovoPagamento.Visible = _saleService.PodeRegistarPagamento(sale, UserId, Perfil);
            }

            // ===================== Cancelamento =====================
            phCancelamento.Visible = _saleService.PodeCancelar(sale, UserId, Perfil);

            // ===================== Anexos e histórico =====================
            phAnexosHistorico.Visible = true;
            ucAnexos.Inicializar("Sale", sale.SaleId, UserId);
            ucHistorico.Inicializar("Sale", sale.SaleId.ToString());
        }

        private void CarregarPropostas(int clientId)
        {
            ddlProposta.Items.Clear();
            ddlProposta.Items.Add(new ListItem("Selecionar...", ""));

            var propostas = _proposalRepository.Listar(
                pesquisa: null,
                status: ProposalService.StatusAceite,
                clientId: clientId,
                accountManagerId: null,
                dataInicio: null,
                dataFim: null,
                pagina: 1,
                tamanhoPagina: 100,
                totalRegistos: out int _,
                sortColumn: "IssueDate",
                sortAscending: false);

            foreach (var proposal in propostas)
            {
                ddlProposta.Items.Add(new ListItem(
                    $"{proposal.ProposalNumber} — {proposal.Total:C}",
                    proposal.ProposalId.ToString()));
            }
        }

        private List<LinhaEdicao> MapearLinhas(IEnumerable<SaleLine> linhas)
        {
            return linhas
                .OrderBy(l => l.LineOrder)
                .Select(l => new LinhaEdicao
                {
                    SaleLineId = l.SaleLineId,
                    ProductId = l.ProductId,
                    ProductName = l.Product?.Name,
                    LineOrder = l.LineOrder,
                    Description = l.Description,
                    Quantity = l.Quantity,
                    UnitPrice = l.UnitPrice,
                    DiscountPercent = l.DiscountPercent,
                    TaxRateId = l.TaxRateId,
                    TaxPercentage = l.TaxRate?.Percentage ?? 0,
                    LineTotal = l.LineTotal
                })
                .ToList();
        }

        // ===================== Renderização / cálculo =====================

        private void RenderizarLinhas()
        {
            var linhas = Linhas;

            rptLinhas.DataSource = linhas;
            rptLinhas.DataBind();

            phSemLinhas.Visible = linhas.Count == 0;
            btnAdicionarLinha.Visible = PodeEditar;
        }

        private void RecalcularTotais()
        {
            var linhas = Linhas;

            decimal subTotal = linhas.Sum(l => l.LineTotal);
            decimal ivaTotal = linhas.Sum(l => Math.Round(l.LineTotal * (l.TaxPercentage / 100m), 2));

            lblSubTotal.Text = subTotal.ToString("C");
            lblIvaTotal.Text = ivaTotal.ToString("C");
            lblTotalGeral.Text = (subTotal + ivaTotal).ToString("C");
        }

        private void RecalcularLinha(LinhaEdicao linha)
        {
            linha.LineTotal = Math.Round(linha.Quantity * linha.UnitPrice * (1 - linha.DiscountPercent / 100m), 2);
        }

        private static string EstadoParaClasseBadge(string status)
        {
            switch (status)
            {
                case "Pendente": return "bg-secondary";
                case "Confirmada": return "badge-em-contacto";
                case "Parcial": return "bg-warning text-dark";
                case "Concluída": return "badge-ativo";
                case "Cancelada": return "badge-bloqueado";
                default: return "bg-secondary";
            }
        }

        // ===================== Cliente / Origem / Proposta =====================

        protected void ucCliente_ClienteSelecionado(object sender, EventArgs e)
        {
            if (!ucCliente.ClienteId.HasValue) return;

            if (ddlOrigem.SelectedValue == SaleService.OrigemProposta)
                CarregarPropostas(ucCliente.ClienteId.Value);
        }

        protected void ddlOrigem_SelectedIndexChanged(object sender, EventArgs e)
        {
            phSeletorProposta.Visible = ddlOrigem.SelectedValue == SaleService.OrigemProposta;

            if (phSeletorProposta.Visible && ucCliente.ClienteId.HasValue)
                CarregarPropostas(ucCliente.ClienteId.Value);
        }

        protected void ddlProposta_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlProposta.SelectedValue))
                return;

            int proposalId = int.Parse(ddlProposta.SelectedValue);

            var proposal = _proposalRepository.GetById(proposalId);
            if (proposal == null) return;

            var vendaTemp = _saleService.MontarAPartirDeProposta(proposal);
            Linhas = MapearLinhas(vendaTemp.Lines);
        }

        // ===================== Linhas =====================

        protected void btnAdicionarLinha_Click(object sender, EventArgs e)
        {
            var linhas = Linhas;
            linhas.Add(new LinhaEdicao { LineOrder = linhas.Count + 1, Quantity = 1, DiscountPercent = 0 });
            Linhas = linhas;
        }

        protected void rptLinhas_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var linha = (LinhaEdicao)e.Item.DataItem;
            int index = e.Item.ItemIndex;

            var ucProduto = (SeletorProduto)e.Item.FindControl("ucProduto");
            ucProduto.ProdutoSelecionado += (s, args) => AtualizarProdutoLinha(index, ucProduto);
            ucProduto.Enabled = PodeEditar;

            if (linha.ProductId > 0)
                ucProduto.ProdutoId = linha.ProductId;

            var hdnUnitPrice = (HiddenField)e.Item.FindControl("hdnUnitPrice");
            var hdnTaxRateId = (HiddenField)e.Item.FindControl("hdnTaxRateId");
            hdnUnitPrice.Value = linha.UnitPrice.ToString();
            hdnTaxRateId.Value = linha.TaxRateId.ToString();

            ((Label)e.Item.FindControl("lblPrecoUnit")).Text = linha.UnitPrice.ToString("C");
            ((Label)e.Item.FindControl("lblIva")).Text = linha.TaxPercentage.ToString("0") + "%";
            ((Label)e.Item.FindControl("lblTotalLinha")).Text = linha.LineTotal.ToString("C");

            var txtQuantidade = (TextBox)e.Item.FindControl("txtQuantidade");
            var txtDesconto = (TextBox)e.Item.FindControl("txtDesconto");
            var txtDescricao = (TextBox)e.Item.FindControl("txtDescricao");
            var lnkRemover = (LinkButton)e.Item.FindControl("lnkRemover");

            txtQuantidade.Enabled = PodeEditar;
            txtDesconto.Enabled = PodeEditar;
            txtDescricao.Enabled = PodeEditar;
            lnkRemover.Visible = PodeEditar;
        }

        private void AtualizarProdutoLinha(int index, SeletorProduto seletor)
        {
            var linhas = Linhas;
            if (index < 0 || index >= linhas.Count) return;

            var produto = seletor.ObterProdutoSelecionado();
            if (produto == null) return;

            var taxa = _taxRateRepository.ListarTodas().SingleOrDefault(t => t.TaxRateId == produto.TaxRateId);

            var linha = linhas[index];
            linha.ProductId = produto.ProductId;
            linha.ProductName = produto.Name;
            if (string.IsNullOrWhiteSpace(linha.Description))
                linha.Description = produto.Name;
            linha.UnitPrice = produto.BasePrice;
            linha.TaxRateId = taxa?.TaxRateId ?? produto.TaxRateId;
            linha.TaxPercentage = taxa?.Percentage ?? 0;
            if (linha.Quantity <= 0) linha.Quantity = 1;

            RecalcularLinha(linha);
            Linhas = linhas;
        }

        protected void txtLinha_TextChanged(object sender, EventArgs e)
        {
            var item = (RepeaterItem)((TextBox)sender).NamingContainer;
            int index = item.ItemIndex;

            var linhas = Linhas;
            if (index < 0 || index >= linhas.Count) return;

            var linha = linhas[index];

            decimal.TryParse(((TextBox)item.FindControl("txtQuantidade")).Text, out decimal quantidade);
            decimal.TryParse(((TextBox)item.FindControl("txtDesconto")).Text, out decimal desconto);

            linha.Quantity = quantidade;
            linha.DiscountPercent = desconto;
            linha.Description = ((TextBox)item.FindControl("txtDescricao")).Text;

            RecalcularLinha(linha);
            Linhas = linhas;
        }

        protected void rptLinhas_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Remover") return;

            var linhas = Linhas;
            int index = e.Item.ItemIndex;

            if (index >= 0 && index < linhas.Count)
            {
                linhas.RemoveAt(index);
                for (int i = 0; i < linhas.Count; i++)
                    linhas[i].LineOrder = i + 1;

                Linhas = linhas;
            }
        }

        protected void cvLinhas_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = Linhas.Any(l => l.ProductId > 0 && l.Quantity > 0);
        }

        // ===================== Guardar / Confirmar / Cancelar =====================

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            var sale = MontarSaleAPartirDoFormulario();
            var erros = _saleService.Validar(sale);

            if (erros.Any())
            {
                NotificacaoService.Erro(string.Join(" ", erros));
                return;
            }

            if (SaleId.HasValue)
            {
                sale.SaleId = SaleId.Value;
                _saleService.Atualizar(sale, UserId);
                NotificacaoService.Sucesso("Venda atualizada.");
                CarregarVenda(SaleId.Value);
            }
            else
            {
                var criada = _saleService.Criar(sale, UserId);
                NotificacaoService.Sucesso("Venda criada.");
                Response.Redirect($"VendaEditar.aspx?id={criada.SaleId}");
            }
        }

        private Sale MontarSaleAPartirDoFormulario()
        {
            DateTime.TryParse(txtDataVenda.Text, out DateTime dataVenda);
            DateTime? dataVencimento = DateTime.TryParse(txtDataVencimento.Text, out DateTime dv) ? dv : (DateTime?)null;
            decimal? comissao = decimal.TryParse(txtComissao.Text, out decimal com) ? com : (decimal?)null;
            int.TryParse(ddlComercial.SelectedValue, out int ownerId);

            return new Sale
            {
                ClientId = ucCliente.ClienteId ?? 0,
                OwnerId = ownerId,
                Origin = ddlOrigem.SelectedValue,
                ProposalId = ddlOrigem.SelectedValue == SaleService.OrigemProposta && !string.IsNullOrEmpty(ddlProposta.SelectedValue)
                    ? int.Parse(ddlProposta.SelectedValue)
                    : (int?)null,
                SaleDate = dataVenda == default ? DateTime.Today : dataVenda,
                DueDate = dataVencimento,
                PaymentMethod = string.IsNullOrWhiteSpace(ddlMetodoPagamento.SelectedValue) ? null : ddlMetodoPagamento.SelectedValue,
                CommissionValue = comissao,
                Lines = Linhas.Select(l => new SaleLine
                {
                    SaleLineId = l.SaleLineId,
                    ProductId = l.ProductId,
                    LineOrder = l.LineOrder,
                    Description = l.Description,
                    Quantity = l.Quantity,
                    UnitPrice = l.UnitPrice,
                    DiscountPercent = l.DiscountPercent,
                    TaxRateId = l.TaxRateId
                }).ToList()
            };
        }

        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            if (!SaleId.HasValue) return;

            if (_saleService.ConfirmarManualmente(SaleId.Value, UserId, Perfil))
                NotificacaoService.Sucesso("Venda confirmada.");
            else
                NotificacaoService.Erro("Não foi possível confirmar a venda.");

            CarregarVenda(SaleId.Value);
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            if (!SaleId.HasValue) return;

            var erros = _saleService.ValidarCancelamento(txtMotivoCancelamento.Text);
            if (erros.Any())
            {
                NotificacaoService.Erro(string.Join(" ", erros));
                return;
            }

            if (_saleService.Cancelar(SaleId.Value, txtMotivoCancelamento.Text.Trim(), UserId, Perfil))
                NotificacaoService.Sucesso("Venda cancelada.");
            else
                NotificacaoService.Erro("Não tens permissão para cancelar esta venda.");

            CarregarVenda(SaleId.Value);
        }

        // ===================== Pagamentos =====================

        protected void btnRegistarPagamento_Click(object sender, EventArgs e)
        {
            if (!SaleId.HasValue) return;

            decimal.TryParse(txtValorPagamento.Text, out decimal valor);
            DateTime.TryParse(txtDataPagamento.Text, out DateTime data);

            var payment = new Payment
            {
                SaleId = SaleId.Value,
                Amount = valor,
                PaymentDate = data == default ? DateTime.Today : data,
                PaymentMethod = ddlMetodoPagamentoPagamento.SelectedValue,
                Reference = txtReferenciaPagamento.Text.Trim(),
                Notes = txtNotasPagamento.Text.Trim()
            };

            var erros = _paymentService.Validar(payment);
            if (erros.Any())
            {
                NotificacaoService.Erro(string.Join(" ", erros));
                return;
            }

            _paymentService.Registar(payment, UserId);
            NotificacaoService.Sucesso("Pagamento registado.");

            LimparFormularioPagamento();
            CarregarVenda(SaleId.Value);
        }

        private void LimparFormularioPagamento()
        {
            txtValorPagamento.Text = "";
            txtDataPagamento.Text = "";
            txtReferenciaPagamento.Text = "";
            txtNotasPagamento.Text = "";
        }

        protected void rptPagamentos_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Eliminar" || !SaleId.HasValue) return;

            int paymentId = int.Parse(e.CommandArgument.ToString());

            _paymentService.Eliminar(paymentId, SaleId.Value, UserId);
            NotificacaoService.Sucesso("Pagamento eliminado.");

            CarregarVenda(SaleId.Value);
        }
    }
}