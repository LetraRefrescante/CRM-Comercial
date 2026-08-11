using System;
using CRM.Data.Repositories;
using CRM.Models.Entities.Catalogo;

namespace CRM.Web.Controls
{
    public partial class SeletorProduto : System.Web.UI.UserControl
    {
        private readonly ProductRepository _productRepository = new ProductRepository();

        public int? ProdutoId
        {
            get => int.TryParse(hdnProdutoId.Value, out int id) ? id : (int?)null;
            set
            {
                hdnProdutoId.Value = value?.ToString() ?? "";
                txtProdutoNome.Text = value.HasValue
                    ? _productRepository.GetById(value.Value)?.Name ?? ""
                    : "";
            }
        }

        public bool Enabled
        {
            get => !btnAbrirSeletor.Disabled;
            set
            {
                txtProdutoNome.Enabled = value;
                txtPesquisa.Enabled = value;
                ddlTipo.Enabled = value;
                btnPesquisar.Enabled = value;
                btnAbrirSeletor.Disabled = !value;
            }
        }

        public bool Obrigatorio
        {
            get => cvProdutoObrigatorio.Enabled;
            set => cvProdutoObrigatorio.Enabled = value;
        }

        public bool OcultarCampoTexto { get; set; }
        public string TextoBotao { get; set; } = "Escolher";
        public string IconeBotao { get; set; } = "fa-box";
        public string CssClassBotao { get; set; } = "btn btn-outline-secondary";

        public event EventHandler ProdutoSelecionado;

        protected void Page_Load(object sender, EventArgs e)
        {
            txtProdutoNome.Visible = !OcultarCampoTexto;

            if (OcultarCampoTexto)
            {
                divSeletor.Attributes.Remove("class");
                divSeletor.Attributes.Remove("style");
            }

            btnAbrirSeletor.InnerHtml = $"<i class=\"fas {IconeBotao}\"></i> {System.Web.HttpUtility.HtmlEncode(TextoBotao)}";
            btnAbrirSeletor.Attributes["class"] = CssClassBotao;
            btnAbrirSeletor.Attributes["data-bs-target"] = "#" + mdlSeletor.ClientID;

            if (!IsPostBack)
            {
                Pesquisar();
            }
        }

        protected void btnPesquisar_Click(object sender, EventArgs e)
        {
            Pesquisar();
        }

        private void Pesquisar()
        {
            // Blueprint: "Inativos não podem ser adicionados a novos documentos" — por
            // isso isActive fica sempre fixo a true aqui, não é um filtro exposto.
            var resultados = _productRepository.Listar(
                pesquisa: txtPesquisa.Text.Trim(),
                type: ddlTipo.SelectedValue,
                categoryId: null,
                isActive: true,
                pagina: 1,
                tamanhoPagina: 50,
                totalRegistos: out int _);

            rptResultados.DataSource = resultados;
            rptResultados.DataBind();
            phSemResultados.Visible = resultados.Count == 0;
        }

        protected void rptResultados_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Escolher") return;

            int productId = int.Parse(e.CommandArgument.ToString());
            var produto = _productRepository.GetById(productId);
            if (produto == null) return;

            hdnProdutoId.Value = produto.ProductId.ToString();
            txtProdutoNome.Text = produto.Name;

            ProdutoSelecionado?.Invoke(this, EventArgs.Empty);
        }
        public Product ObterProdutoSelecionado() => ProdutoId.HasValue ? _productRepository.GetById(ProdutoId.Value) : null;

        protected void cvProdutoObrigatorio_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            args.IsValid = ProdutoId.HasValue;
        }
    }
}