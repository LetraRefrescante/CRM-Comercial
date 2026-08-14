using CRM.Data.Repositories;
using CRM.Models.Entities.Catalogo;
using CRM.Services;
using CRM.Web.Helpers;
using System;
using System.Globalization;

namespace CRM.Web.Paginas.Catalogo
{
    public partial class TabelaPrecoDetalhe : PaginaBase
    {
        private readonly PriceTableService _priceTableService = new PriceTableService();
        private readonly PriceTableItemService _priceTableItemService = new PriceTableItemService();
        private readonly ProductRepository _productRepository = new ProductRepository();

        private int? PriceTableId
        {
            get
            {
                if (int.TryParse(Request.QueryString["id"], out int id)) return id;
                return null;
            }
        }

        protected string NomeTabela
        {
            get { return ViewState["NomeTabela"] as string ?? "Preços da Tabela"; }
            set { ViewState["NomeTabela"] = value; }
        }

        private int? PriceTableItemIdEmEdicao
        {
            get => ViewState["PriceTableItemIdEmEdicao"] as int?;
            set => ViewState["PriceTableItemIdEmEdicao"] = value;
        }

        private int? ProductIdEmEdicao
        {
            get => ViewState["ProductIdEmEdicao"] as int?;
            set => ViewState["ProductIdEmEdicao"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!_priceTableService.PodeGerir(Perfil))
            {
                Response.Redirect("~/AcessoNegado.aspx");
                return;
            }

            var priceTable = PriceTableId.HasValue ? _priceTableService.GetById(PriceTableId.Value) : null;
            if (priceTable == null)
            {
                Response.Redirect("~/Catalogo/TabelasPreco.aspx");
                return;
            }

            NomeTabela = priceTable.Name;

            if (!IsPostBack)
            {
                CarregarProdutos();
                CarregarItens();
            }
        }

        private void CarregarProdutos()
        {
            ddlProduto.Items.Clear();
            ddlProduto.Items.Add(new System.Web.UI.WebControls.ListItem("Seleciona...", ""));

            foreach (var produto in _productRepository.ListarAtivos())
            {
                ddlProduto.Items.Add(new System.Web.UI.WebControls.ListItem($"{produto.Code} — {produto.Name}", produto.ProductId.ToString()));
            }
        }

        private void CarregarItens()
        {
            var itens = _priceTableItemService.ListarPorTabela(PriceTableId.Value);

            rptItens.DataSource = itens;
            rptItens.DataBind();

            phVazio.Visible = itens.Count == 0;
        }

        private void LimparFormulario()
        {
            PriceTableItemIdEmEdicao = null;
            ProductIdEmEdicao = null;
            ddlProduto.Visible = true;
            ddlProduto.SelectedIndex = 0;
            litProdutoSelecionado.Visible = false;
            txtPreco.Text = string.Empty;
            litModoEdicao.Visible = false;
            btnCancelar.Visible = false;
        }

        private decimal ObterPrecoDigitado()
        {
            string texto = txtPreco.Text?.Replace(',', '.') ?? "0";
            return decimal.TryParse(texto, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal preco) ? preco : -1;
        }

        protected void cvRegrasNegocio_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            int productId = PriceTableItemIdEmEdicao.HasValue
                ? ProductIdEmEdicao ?? 0
                : (string.IsNullOrEmpty(ddlProduto.SelectedValue) ? 0 : int.Parse(ddlProduto.SelectedValue));

            bool produtoJaTemPreco = productId > 0 &&
                _priceTableItemService.ExisteProdutoNaTabela(PriceTableId.Value, productId, PriceTableItemIdEmEdicao);

            var item = new PriceTableItem
            {
                ProductId = productId,
                Price = ObterPrecoDigitado()
            };
            var erros = _priceTableItemService.Validar(item, produtoJaTemPreco);

            args.IsValid = erros.Count == 0;
            cvRegrasNegocio.ErrorMessage = string.Join(" ", erros);
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            decimal preco = ObterPrecoDigitado();

            if (PriceTableItemIdEmEdicao.HasValue)
            {
                var item = new PriceTableItem
                {
                    PriceTableItemId = PriceTableItemIdEmEdicao.Value,
                    Price = preco,
                    UpdatedBy = UserId
                };
                _priceTableItemService.Atualizar(item);
                NotificacaoService.Sucesso("Preço atualizado.");
            }
            else
            {
                var item = new PriceTableItem
                {
                    PriceTableId = PriceTableId.Value,
                    ProductId = int.Parse(ddlProduto.SelectedValue),
                    Price = preco,
                    CreatedBy = UserId
                };
                _priceTableItemService.Criar(item);
                NotificacaoService.Sucesso("Preço adicionado.");
            }

            LimparFormulario();
            CarregarItens();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            LimparFormulario();
        }

        protected void rptItens_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            int priceTableItemId = int.Parse(e.CommandArgument.ToString());

            if (e.CommandName == "Editar")
            {
                var item = _priceTableItemService.GetById(priceTableItemId);
                if (item == null) return;

                PriceTableItemIdEmEdicao = item.PriceTableItemId;
                ProductIdEmEdicao = item.ProductId;
                ddlProduto.Visible = false;
                litProdutoSelecionado.Visible = true;
                litProdutoSelecionado.Text = item.Product?.Name;
                txtPreco.Text = item.Price.ToString("0.00", CultureInfo.InvariantCulture);
                litModoEdicao.Text = $"A editar preço de: {item.Product?.Name}";
                litModoEdicao.Visible = true;
                btnCancelar.Visible = true;
                return;
            }

            if (e.CommandName == "Remover")
            {
                _priceTableItemService.Eliminar(priceTableItemId, UserId);
                NotificacaoService.Sucesso("Preço removido.");
                CarregarItens();
            }
        }
    }
}