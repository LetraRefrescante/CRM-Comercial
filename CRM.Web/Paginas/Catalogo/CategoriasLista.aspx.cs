using CRM.Models.Entities.Catalogo;
using CRM.Services;
using CRM.Web.Helpers;
using System;

namespace CRM.Web.Paginas.Catalogo
{
    public partial class CategoriasLista : PaginaBase
    {
        private readonly CategoryService _categoryService = new CategoryService();

        private int? CategoryIdEmEdicao
        {
            get => ViewState["CategoryIdEmEdicao"] as int?;
            set => ViewState["CategoryIdEmEdicao"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!_categoryService.PodeGerir(Perfil))
            {
                Response.Redirect("~/AcessoNegado.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CarregarCategorias();
            }
        }

        private void CarregarCategorias()
        {
            var categorias = _categoryService.Listar(null);

            rptCategorias.DataSource = categorias;
            rptCategorias.DataBind();

            phVazio.Visible = categorias.Count == 0;
        }

        private void LimparFormulario()
        {
            CategoryIdEmEdicao = null;
            txtNome.Text = string.Empty;
            litModoEdicao.Visible = false;
            btnCancelar.Visible = false;
        }

        protected void cvRegrasNegocio_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            bool nomeJaExiste = _categoryService.ExisteNome(txtNome.Text.Trim(), CategoryIdEmEdicao);

            var category = new Category { Name = txtNome.Text.Trim() };
            var erros = _categoryService.Validar(category, nomeJaExiste);

            args.IsValid = erros.Count == 0;
            cvRegrasNegocio.ErrorMessage = string.Join(" ", erros);
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            if (CategoryIdEmEdicao.HasValue)
            {
                var category = new Category
                {
                    CategoryId = CategoryIdEmEdicao.Value,
                    Name = txtNome.Text.Trim(),
                    UpdatedBy = UserId
                };
                _categoryService.Atualizar(category);
                NotificacaoService.Sucesso("Categoria atualizada.");
            }
            else
            {
                var category = new Category
                {
                    Name = txtNome.Text.Trim(),
                    CreatedBy = UserId
                };
                _categoryService.Criar(category);
                NotificacaoService.Sucesso("Categoria criada.");
            }

            LimparFormulario();
            CarregarCategorias();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            LimparFormulario();
        }

        protected void rptCategorias_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            int categoryId = int.Parse(e.CommandArgument.ToString());

            if (e.CommandName == "Editar")
            {
                var category = _categoryService.GetById(categoryId);
                if (category == null) return;

                CategoryIdEmEdicao = category.CategoryId;
                txtNome.Text = category.Name;
                litModoEdicao.Text = $"A editar: {category.Name}";
                litModoEdicao.Visible = true;
                btnCancelar.Visible = true;
                return;
            }

            if (e.CommandName == "AlternarEstado")
            {
                _categoryService.AlternarEstado(categoryId, UserId);
                NotificacaoService.Sucesso("Estado da categoria atualizado.");
                CarregarCategorias();
            }
        }
    }
}