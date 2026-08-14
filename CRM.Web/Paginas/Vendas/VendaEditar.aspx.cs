using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using CRM.Data.Repositories;
using CRM.Models.Entities.Catalogo;
using CRM.Models.Entities.Vendas;
using CRM.Services;
using CRM.Web.Controls;
using CRM.Web.Helpers;

namespace CRM.Web.Paginas.Vendas
{
    public partial class VendaEditar : PaginaBase
    {
        private readonly SaleService _saleService = new SaleService();
        private readonly PaymentService _paymentService = new PaymentService();
        private readonly ProposalRepository _proposalRepository = new ProposalRepository();
        private readonly UserRepository _userRepository = new UserRepository();
        private readonly TaxRateRepository _taxRateRepository = new TaxRateRepository();

        private List<TaxRate> _taxasIva;
        private List<TaxRate> GetTaxasIva() => _taxasIva ?? (_taxasIva = _taxRateRepository.ListarAtivas());

        private int? SaleId => ViewState["SaleId"] as int?;
        private string StatusAtual => ViewState["Status"] as string ?? SaleService.StatusPendente;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarListasAuxiliares();

                if (int.TryParse(Request.QueryString["id"], out int id))
                {
                    CarregarVenda(id);
                }
                else if (int.TryParse(Request.QueryString["proposalId"], out int proposalId))
                {
                    CarregarNovaAPartirDeProposta(proposalId);
                }
                else
                {
                    CarregarNovaManual();
                }
            }
        }

        // ===================== Carregamento inicial =====================

        private void CarregarListasAuxiliares()
        {
            ddlComercial.Items.Add(new ListItem("(Selecionar)", ""));
            foreach (var user in _userRepository.ListarComerciaisAtivos())
                ddlComercial.Items.Add(new ListItem(user.Name, user.UserId.ToString()));
        }

        private void CarregarNovaManual()
        {
            lblNumero.Text = "(gerado ao gravar)";
            AtualizarBadgeEstado(SaleService.StatusPendente);
            ddlOrigem.SelectedValue = SaleService.OrigemManual;
            phSeletorProposta.Visible = false;
            txtDataVenda.Text = DateTime.Today.ToString("dd/MM/yyyy");

            AplicarComercialPorDefeito();

            RebindLinhas(new List<SaleLine> { NovaLinhaVazia() });

            AtualizarVisibilidadeBotoes(null);
            phPagamentos.Visible = false;
            phCancelamento.Visible = false;
            phAnexosHistorico.Visible = false;
        }

        // Entrada a partir de "Converter em Venda" numa proposta (~/Vendas/VendaEditar.aspx?proposalId=X).
        // ASSUNÇÃO: só propostas Aceites podem originar uma venda — se a proposta não
        // existir ou não estiver Aceite, cai-se para uma venda manual em vez de rebentar.
        private void CarregarNovaAPartirDeProposta(int proposalId)
        {
            var proposal = _proposalRepository.GetById(proposalId);

            if (proposal == null || proposal.Status != "Aceite")
            {
                NotificacaoService.Erro("A proposta indicada não existe ou não está Aceite. Cria a venda manualmente ou escolhe outra proposta.");
                CarregarNovaManual();
                return;
            }

            var saleTemp = _saleService.MontarAPartirDeProposta(proposal);

            lblNumero.Text = "(gerado ao gravar)";
            AtualizarBadgeEstado(SaleService.StatusPendente);

            ucCliente.ClienteId = saleTemp.ClientId;
            ddlOrigem.SelectedValue = SaleService.OrigemProposta;
            phSeletorProposta.Visible = true;
            CarregarPropostasDoCliente(saleTemp.ClientId);
            ddlProposta.SelectedValue = proposalId.ToString();

            txtDataVenda.Text = DateTime.Today.ToString("dd/MM/yyyy");

            AplicarComercialPorDefeito();

            RebindLinhas(saleTemp.Lines);

            AtualizarVisibilidadeBotoes(null);
            phPagamentos.Visible = false;
            phCancelamento.Visible = false;
            phAnexosHistorico.Visible = false;
        }

        private void CarregarVenda(int saleId)
        {
            var sale = _saleService.GetById(saleId);
            if (sale == null)
            {
                Response.Redirect("~/AcessoNegado.aspx");
                return;
            }

            if (!_saleService.PodeAceder(sale, UserId, Perfil))
            {
                Response.Redirect("~/AcessoNegado.aspx");
                return;
            }

            ViewState["SaleId"] = sale.SaleId;
            ViewState["Status"] = sale.Status;

            ucCliente.ClienteId = sale.ClientId;

            lblNumero.Text = sale.SaleNumber;
            AtualizarBadgeEstado(sale.Status);

            ddlComercial.SelectedValue = sale.OwnerId.ToString();
            ddlComercial.Enabled = !_saleService.TemAmbitoProprios(Perfil);

            ddlOrigem.SelectedValue = sale.Origin;
            phSeletorProposta.Visible = sale.Origin == SaleService.OrigemProposta;
            if (sale.Origin == SaleService.OrigemProposta)
            {
                CarregarPropostasDoCliente(sale.ClientId);
                if (sale.ProposalId.HasValue)
                    ddlProposta.SelectedValue = sale.ProposalId.Value.ToString();
            }

            txtDataVenda.Text = sale.SaleDate.ToString("dd/MM/yyyy");
            txtDataVencimento.Text = sale.DueDate?.ToString("dd/MM/yyyy") ?? "";
            ddlMetodoPagamento.SelectedValue = sale.PaymentMethod ?? "";
            txtComissao.Text = sale.CommissionValue?.ToString("0.##", CultureInfo.InvariantCulture) ?? "";

            var linhas = sale.Lines.Any() ? sale.Lines.ToList() : new List<SaleLine> { NovaLinhaVazia() };
            RebindLinhas(linhas);

            AtualizarVisibilidadeBotoes(sale);

            // Pagamentos
            phPagamentos.Visible = sale.Status != SaleService.StatusCancelada;
            if (phPagamentos.Visible)
                CarregarPagamentos(sale);

            // Cancelamento
            phCancelamento.Visible = _saleService.PodeCancelar(sale, UserId, Perfil);

            // Confirmar (só Pendente)
            btnConfirmar.Visible = sale.Status == SaleService.StatusPendente
                && _saleService.PodeCriarOuEditar(Perfil)
                && _saleService.PodeAceder(sale, UserId, Perfil);

            // Anexos e histórico
            phAnexosHistorico.Visible = true;
            ucAnexos.Inicializar("Sale", sale.SaleId, UserId);
            ucHistorico.Inicializar("Sale", sale.SaleId.ToString());
        }

        private void AplicarComercialPorDefeito()
        {
            if (_saleService.TemAmbitoProprios(Perfil))
            {
                ddlComercial.SelectedValue = UserId.ToString();
                ddlComercial.Enabled = false;
            }
            else
            {
                ddlComercial.Enabled = true;
            }
        }

        private void CarregarPropostasDoCliente(int clientId)
        {
            ddlProposta.Items.Clear();
            ddlProposta.Items.Add(new ListItem("(Selecionar)", ""));

            foreach (var proposal in _proposalRepository.ListarAceitesPorCliente(clientId))
                ddlProposta.Items.Add(new ListItem($"{proposal.ProposalNumber} · {proposal.Total:C}", proposal.ProposalId.ToString()));
        }

        private void AtualizarBadgeEstado(string status)
        {
            spanStatus.InnerText = status;
            spanStatus.Attributes["class"] = "badge " + GetBadgeClasse(status);
        }

        // ===================== Cliente / Origem / Proposta =====================

        protected void ucCliente_ClienteSelecionado(object sender, EventArgs e)
        {
            if (!ucCliente.ClienteId.HasValue) return;

            if (ddlOrigem.SelectedValue == SaleService.OrigemProposta)
            {
                CarregarPropostasDoCliente(ucCliente.ClienteId.Value);
                RebindLinhas(new List<SaleLine> { NovaLinhaVazia() });
            }
        }

        protected void ddlOrigem_SelectedIndexChanged(object sender, EventArgs e)
        {
            phSeletorProposta.Visible = ddlOrigem.SelectedValue == SaleService.OrigemProposta;

            if (phSeletorProposta.Visible && ucCliente.ClienteId.HasValue)
            {
                CarregarPropostasDoCliente(ucCliente.ClienteId.Value);
            }
            else
            {
                ddlProposta.Items.Clear();
            }

            RebindLinhas(new List<SaleLine> { NovaLinhaVazia() });
        }

        protected void ddlProposta_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlProposta.SelectedValue)) return;

            var proposal = _proposalRepository.GetById(int.Parse(ddlProposta.SelectedValue));
            if (proposal == null) return;

            var saleTemp = _saleService.MontarAPartirDeProposta(proposal);
            RebindLinhas(saleTemp.Lines);
        }

        // ===================== Repeater: colher / recompor (sem ViewModel) =====================

        private SaleLine NovaLinhaVazia() => new SaleLine { Quantity = 1 };

        private List<SaleLine> ColherLinhasDosControles()
        {
            var linhas = new List<SaleLine>();

            foreach (RepeaterItem item in rptLinhas.Items)
            {
                if (item.ItemType != ListItemType.Item && item.ItemType != ListItemType.AlternatingItem)
                    continue;

                var ucProduto = (SeletorProduto)item.FindControl("ucProduto");
                var hdnSaleLineId = (HiddenField)item.FindControl("hdnSaleLineId");
                var hdnUnitPrice = (HiddenField)item.FindControl("hdnUnitPrice");
                var hdnTaxRateId = (HiddenField)item.FindControl("hdnTaxRateId");
                var txtDescricao = (TextBox)item.FindControl("txtDescricao");
                var txtQuantidade = (TextBox)item.FindControl("txtQuantidade");
                var txtDesconto = (TextBox)item.FindControl("txtDesconto");

                int taxRateId = int.TryParse(hdnTaxRateId.Value, out int tid) ? tid : 0;

                linhas.Add(new SaleLine
                {
                    SaleLineId = int.TryParse(hdnSaleLineId.Value, out int lid) ? lid : 0,
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

        private void RebindLinhas(List<SaleLine> linhas)
        {
            var linhasValidas = linhas.Where(l => l.ProductId > 0).ToList();

            var saleTemp = new Sale { Lines = linhasValidas };

            if (linhasValidas.Any())
                _saleService.CalcularTotais(saleTemp);

            rptLinhas.DataSource = linhas;
            rptLinhas.DataBind();

            phSemLinhas.Visible = !linhas.Any();

            lblSubTotal.Text = saleTemp.SubTotal.ToString("C");
            lblIvaTotal.Text = saleTemp.TaxTotal.ToString("C");
            lblTotalGeral.Text = saleTemp.Total.ToString("C");
        }

        protected void rptLinhas_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var linha = (SaleLine)e.Item.DataItem;

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

        private void AtualizarVisibilidadeBotoes(Sale sale)
        {
            bool ehNova = sale == null;

            bool temPermissaoPerfil = _saleService.PodeCriarOuEditar(Perfil);
            bool ehDono = ehNova || _saleService.PodeAceder(sale, UserId, Perfil);
            bool estadoPermiteEdicaoDireta = ehNova || _saleService.PodeEditarDiretamente(sale);

            bool podeEditar = temPermissaoPerfil && ehDono && estadoPermiteEdicaoDireta;

            pnlCamposEditaveis.Enabled = podeEditar;
            btnGuardar.Visible = podeEditar;
            btnAdicionarLinha.Visible = podeEditar;
            phAvisoSoLeitura.Visible = !podeEditar && !ehNova;
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

            var saleExistente = SaleId.HasValue ? _saleService.GetById(SaleId.Value) : null;

            bool podeGravar = _saleService.PodeCriarOuEditar(Perfil)
                && (saleExistente == null || _saleService.PodeAceder(saleExistente, UserId, Perfil))
                && (saleExistente == null || _saleService.PodeEditarDiretamente(saleExistente));

            if (!podeGravar)
            {
                NotificacaoService.Erro("Não tens permissão para gravar esta venda.");
                return;
            }

            var linhas = ColherLinhasDosControles().Where(l => l.ProductId > 0).ToList();

            var sale = new Sale
            {
                SaleId = SaleId ?? 0,
                ClientId = ucCliente.ClienteId ?? 0,
                OwnerId = string.IsNullOrEmpty(ddlComercial.SelectedValue) ? 0 : int.Parse(ddlComercial.SelectedValue),
                Origin = ddlOrigem.SelectedValue,
                ProposalId = string.IsNullOrEmpty(ddlProposta.SelectedValue) ? (int?)null : int.Parse(ddlProposta.SelectedValue),
                SaleDate = DateTime.TryParse(txtDataVenda.Text, out DateTime dataVenda) ? dataVenda : DateTime.Today,
                DueDate = DateTime.TryParse(txtDataVencimento.Text, out DateTime dataVenc) ? dataVenc : (DateTime?)null,
                PaymentMethod = string.IsNullOrEmpty(ddlMetodoPagamento.SelectedValue) ? null : ddlMetodoPagamento.SelectedValue,
                CommissionValue = decimal.TryParse(txtComissao.Text, out decimal comissao) ? comissao : (decimal?)null,
                Lines = linhas
            };

            var erros = _saleService.Validar(sale);
            if (erros.Any())
            {
                NotificacaoService.Erro(string.Join(" ", erros));
                return;
            }

            if (SaleId.HasValue)
            {
                sale.Status = StatusAtual;
                _saleService.Atualizar(sale, UserId);
                NotificacaoService.Sucesso("Venda atualizada.");
                Response.Redirect($"VendaEditar.aspx?id={SaleId}");
            }
            else
            {
                var criada = _saleService.Criar(sale, UserId);
                NotificacaoService.Sucesso("Venda criada.");
                Response.Redirect($"VendaEditar.aspx?id={criada.SaleId}");
            }
        }

        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            if (!SaleId.HasValue) return;

            if (_saleService.ConfirmarManualmente(SaleId.Value, UserId, Perfil))
                NotificacaoService.Sucesso("Venda confirmada.");
            else
                NotificacaoService.Erro("Não foi possível confirmar a venda.");

            Response.Redirect($"VendaEditar.aspx?id={SaleId}");
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

            Response.Redirect($"VendaEditar.aspx?id={SaleId}");
        }

        // ===================== Pagamentos =====================

        private void CarregarPagamentos(Sale sale)
        {
            var pagamentos = _paymentService.ListarPorVenda(sale.SaleId);

            rptPagamentos.DataSource = pagamentos;
            rptPagamentos.DataBind();
            phSemPagamentos.Visible = pagamentos.Count == 0;

            decimal totalPago = _paymentService.TotalPago(sale.SaleId);
            lblTotalPago.Text = totalPago.ToString("C");
            lblSaldoEmAberto.Text = (sale.Total - totalPago).ToString("C");

            pnlNovoPagamento.Visible = _saleService.PodeRegistarPagamento(sale, UserId, Perfil);

            txtDataPagamento.Text = DateTime.Today.ToString("dd/MM/yyyy");
        }

        protected void btnRegistarPagamento_Click(object sender, EventArgs e)
        {
            if (!SaleId.HasValue) return;

            var sale = _saleService.GetById(SaleId.Value);
            if (sale == null || !_saleService.PodeRegistarPagamento(sale, UserId, Perfil))
            {
                NotificacaoService.Erro("Não tens permissão para registar pagamentos nesta venda.");
                return;
            }

            var payment = new Payment
            {
                SaleId = SaleId.Value,
                Amount = decimal.TryParse(txtValorPagamento.Text, out decimal valor) ? valor : 0,
                PaymentDate = DateTime.TryParse(txtDataPagamento.Text, out DateTime data) ? data : DateTime.Today,
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
            Response.Redirect($"VendaEditar.aspx?id={SaleId}");
        }

        protected void rptPagamentos_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Eliminar" || !SaleId.HasValue) return;

            var sale = _saleService.GetById(SaleId.Value);
            if (sale == null || !_saleService.PodeRegistarPagamento(sale, UserId, Perfil))
            {
                NotificacaoService.Erro("Não tens permissão para eliminar este pagamento.");
                Response.Redirect($"VendaEditar.aspx?id={SaleId}");
                return;
            }

            int paymentId = int.Parse(e.CommandArgument.ToString());
            _paymentService.Eliminar(paymentId, SaleId.Value, UserId);

            NotificacaoService.Sucesso("Pagamento eliminado.");
            Response.Redirect($"VendaEditar.aspx?id={SaleId}");
        }

        // ===================== Auxiliares =====================

        protected string GetBadgeClasse(string status)
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
    }
}