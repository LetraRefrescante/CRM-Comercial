using CRM.Data.Repositories;
using CRM.Models.Entities.Catalogo;
using CRM.Services;
using CRM.Web.Helpers;
using System;

namespace CRM.Web.Paginas.Catalogo
{
    public partial class ProdutoEditar : PaginaBase
    {
        private readonly ProductService _productService = new ProductService();
        private readonly CategoryRepository _categoryRepository = new CategoryRepository();
        private readonly TaxRateRepository _taxRateRepository = new TaxRateRepository();

        private int? ProductId
        {
            get
            {
                if (int.TryParse(Request.QueryString["id"], out int id)) return id;
                return null;
            }
        }

        public string TituloPagina => ProductId.HasValue ? "Editar Produto" : "Novo Produto";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!_productService.PodeGerir(Perfil))
            {
                Response.Redirect("~/AcessoNegado.aspx");
                return;
            }

            if (!IsPostBack)
            {
                Product product = null;

                if (ProductId.HasValue)
                {
                    product = _productService.GetById(ProductId.Value);
                    if (product == null)
                    {
                        Response.Redirect("~/Catalogo/ProdutosLista.aspx");
                        return;
                    }
                }

                CarregarCategorias(product?.CategoryId);
                CarregarTaxasIva(product?.TaxRateId);

                if (product != null)
                {
                    PreencherFormulario(product);
                }
            }
        }

        private void CarregarCategorias(int? categoriaAtualId = null)
        {
            ddlCategoria.Items.Clear();
            ddlCategoria.Items.Add(new System.Web.UI.WebControls.ListItem("Seleciona...", ""));

            foreach (var categoria in _categoryRepository.ListarAtivas())
            {
                ddlCategoria.Items.Add(new System.Web.UI.WebControls.ListItem(categoria.Name, categoria.CategoryId.ToString()));
            }

            AdicionarValorAtualSeInativo(ddlCategoria, categoriaAtualId,
                () => _categoryRepository.GetById(categoriaAtualId.Value)?.Name);
        }

        private void CarregarTaxasIva(int? taxaAtualId = null)
        {
            ddlTaxaIva.Items.Clear();
            ddlTaxaIva.Items.Add(new System.Web.UI.WebControls.ListItem("Seleciona...", ""));

            foreach (var taxa in _taxRateRepository.ListarAtivas())
            {
                ddlTaxaIva.Items.Add(new System.Web.UI.WebControls.ListItem($"{taxa.Name} ({taxa.Percentage}%)", taxa.TaxRateId.ToString()));
            }

            AdicionarValorAtualSeInativo(ddlTaxaIva, taxaAtualId, () =>
            {
                var taxa = _taxRateRepository.GetById(taxaAtualId.Value);
                return taxa != null ? $"{taxa.Name} ({taxa.Percentage}%)" : null;
            });
        }
        private void AdicionarValorAtualSeInativo(System.Web.UI.WebControls.DropDownList ddl, int? valorAtualId, Func<string> obterTexto)
        {
            if (!valorAtualId.HasValue) return;
            if (ddl.Items.FindByValue(valorAtualId.Value.ToString()) != null) return;

            string texto = obterTexto();
            if (texto == null) return;

            ddl.Items.Add(new System.Web.UI.WebControls.ListItem($"{texto} (inativa)", valorAtualId.Value.ToString()));
        }

        private void PreencherFormulario(Product product)
        {
            txtCodigo.Text = product.Code;
            ddlTipo.SelectedValue = product.Type;
            txtNome.Text = product.Name;
            ddlCategoria.SelectedValue = product.CategoryId.ToString();
            ddlTaxaIva.SelectedValue = product.TaxRateId.ToString();
            ddlUnidade.SelectedValue = product.Unit;
            txtPrecoBase.Text = product.BasePrice.ToString("0.00");
            chkAtivo.Checked = product.IsActive;
            txtDescricao.Text = product.Description;

            ViewState["RowVersion"] = Convert.ToBase64String(product.RowVersion);
        }

        private void CarregarProduto(int productId)
        {
            var product = _productService.GetById(productId);
            if (product == null)
            {
                Response.Redirect("~/Catalogo/ProdutosLista.aspx");
                return;
            }

            txtCodigo.Text = product.Code;
            ddlTipo.SelectedValue = product.Type;
            txtNome.Text = product.Name;
            ddlCategoria.SelectedValue = product.CategoryId.ToString();
            ddlTaxaIva.SelectedValue = product.TaxRateId.ToString();
            ddlUnidade.SelectedValue = product.Unit;
            txtPrecoBase.Text = product.BasePrice.ToString("0.00");
            chkAtivo.Checked = product.IsActive;
            txtDescricao.Text = product.Description;

            ViewState["RowVersion"] = Convert.ToBase64String(product.RowVersion);
        }

        private Product MontarProdutoDoFormulario()
        {
            var product = new Product
            {
                Code = txtCodigo.Text.Trim(),
                Type = ddlTipo.SelectedValue,
                Name = txtNome.Text.Trim(),
                CategoryId = string.IsNullOrEmpty(ddlCategoria.SelectedValue) ? 0 : int.Parse(ddlCategoria.SelectedValue),
                TaxRateId = string.IsNullOrEmpty(ddlTaxaIva.SelectedValue) ? 0 : int.Parse(ddlTaxaIva.SelectedValue),
                Unit = ddlUnidade.SelectedValue,
                BasePrice = string.IsNullOrWhiteSpace(txtPrecoBase.Text) ? 0 : decimal.Parse(txtPrecoBase.Text),
                IsActive = chkAtivo.Checked,
                Description = string.IsNullOrWhiteSpace(txtDescricao.Text) ? null : txtDescricao.Text.Trim()
            };

            if (ProductId.HasValue)
            {
                product.ProductId = ProductId.Value;
                product.RowVersion = Convert.FromBase64String(ViewState["RowVersion"] as string);
            }

            return product;
        }

        protected void cvRegrasNegocio_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            var product = MontarProdutoDoFormulario();
            bool codigoJaExiste = _productService.ExisteCodigo(product.Code, ProductId);

            var erros = _productService.Validar(product, codigoJaExiste);

            args.IsValid = erros.Count == 0;
            cvRegrasNegocio.ErrorMessage = string.Join(" ", erros);
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            var product = MontarProdutoDoFormulario();

            try
            {
                if (ProductId.HasValue)
                {
                    product.UpdatedBy = UserId;
                    _productService.Atualizar(product);
                    NotificacaoService.Sucesso("Produto atualizado.");
                }
                else
                {
                    product.CreatedBy = UserId;
                    int novoId = _productService.Criar(product);
                    NotificacaoService.Sucesso("Produto criado.");
                    Response.Redirect($"~/Catalogo/ProdutoEditar.aspx?id={novoId}");
                    return;
                }
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException)
            {
                NotificacaoService.Erro("Este produto foi alterado por outro utilizador entretanto. Recarrega a página e tenta novamente.");
                return;
            }

            Response.Redirect("~/Catalogo/ProdutosLista.aspx");
        }
    }
}