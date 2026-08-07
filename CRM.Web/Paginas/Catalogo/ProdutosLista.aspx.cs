using CRM.Data.Repositories;
using CRM.Services;
using CRM.Web.Helpers;
using System;

namespace CRM.Web.Paginas.Catalogo
{
    public partial class ProdutosLista : PaginaBase
    {
        private readonly ProductService _productService = new ProductService();
        private readonly CategoryRepository _categoryRepository = new CategoryRepository();

        private string SortColumn
        {
            get => ViewState["SortColumn"] as string ?? "Name";
            set => ViewState["SortColumn"] = value;
        }

        private bool SortAscending
        {
            get => ViewState["SortAscending"] as bool? ?? true;
            set => ViewState["SortAscending"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarCategorias();
                CarregarProdutos();
            }
        }

        private void CarregarCategorias()
        {
            ddlCategoria.Items.Clear();
            ddlCategoria.Items.Add(new System.Web.UI.WebControls.ListItem("Todas", ""));
            foreach (var categoria in _categoryRepository.ListarAtivas())
            {
                ddlCategoria.Items.Add(new System.Web.UI.WebControls.ListItem(categoria.Name, categoria.CategoryId.ToString()));
            }
        }

        private void CarregarProdutos()
        {
            int? categoryId = string.IsNullOrEmpty(ddlCategoria.SelectedValue) ? (int?)null : int.Parse(ddlCategoria.SelectedValue);
            bool? isActive = ddlEstado.SelectedValue == "Ativo" ? true : ddlEstado.SelectedValue == "Inativo" ? (bool?)false : null;

            var produtos = _productService.Listar(
                pesquisa: txtPesquisa.Text.Trim(),
                type: ddlTipo.SelectedValue,
                categoryId: categoryId,
                isActive: isActive,
                pagina: ucPaginacao.PaginaAtual,
                tamanhoPagina: ucPaginacao.TamanhoPagina,
                totalRegistos: out int total,
                sortColumn: SortColumn,
                sortAscending: SortAscending);

            ucPaginacao.TotalRegistos = total;

            rptProdutos.DataSource = produtos;
            rptProdutos.DataBind();

            phVazio.Visible = produtos.Count == 0;

            lnkNovo.Visible = _productService.PodeGerir(Perfil);
            lnkCategorias.Visible = _productService.PodeGerir(Perfil);
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            ucPaginacao.PaginaAtual = 1;
            CarregarProdutos();
        }

        protected void lnkOrdenar_Command(object sender, System.Web.UI.WebControls.CommandEventArgs e)
        {
            string coluna = e.CommandArgument.ToString();

            if (SortColumn == coluna)
            {
                SortAscending = !SortAscending;
            }
            else
            {
                SortColumn = coluna;
                SortAscending = true;
            }

            ucPaginacao.PaginaAtual = 1;
            CarregarProdutos();
        }

        protected void ucPaginacao_PaginaAlterada(object sender, EventArgs e)
        {
            CarregarProdutos();
        }

        protected void rptProdutos_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != System.Web.UI.WebControls.ListItemType.Item &&
                e.Item.ItemType != System.Web.UI.WebControls.ListItemType.AlternatingItem)
                return;

            var phEditar = e.Item.FindControl("phEditar") as System.Web.UI.WebControls.PlaceHolder;
            var phEliminar = e.Item.FindControl("phEliminar") as System.Web.UI.WebControls.PlaceHolder;

            if (phEditar != null) phEditar.Visible = _productService.PodeGerir(Perfil);
            if (phEliminar != null) phEliminar.Visible = _productService.PodeGerir(Perfil);
        }

        protected void rptProdutos_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Eliminar")
            {
                int productId = int.Parse(e.CommandArgument.ToString());

                if (_productService.Eliminar(productId, UserId, Perfil))
                {
                    NotificacaoService.Sucesso("Produto eliminado.");
                }
                else
                {
                    NotificacaoService.Erro("Não tens permissão para eliminar produtos.");
                }

                CarregarProdutos();
            }
        }
    }
}