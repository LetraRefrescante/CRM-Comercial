using CRM.Models.Entities.Catalogo;
using CRM.Services;
using CRM.Web.Helpers;
using System;
using System.Web.UI.WebControls;

namespace CRM.Web.Paginas.Catalogo
{
    public partial class TabelasPreco : PaginaBase
    {
        private readonly PriceTableService _priceTableService = new PriceTableService();
        private readonly PriceTableItemService _priceTableItemService = new PriceTableItemService();

        private int? PriceTableIdEmEdicao
        {
            get => ViewState["PriceTableIdEmEdicao"] as int?;
            set => ViewState["PriceTableIdEmEdicao"] = value;
        }

        private int? PriceTableIdSelecionado
        {
            get => ViewState["PriceTableIdSelecionado"] as int?;
            set => ViewState["PriceTableIdSelecionado"] = value;
        }

        private int? PriceTableItemIdEmEdicao
        {
            get => ViewState["PriceTableItemIdEmEdicao"] as int?;
            set => ViewState["PriceTableItemIdEmEdicao"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!_priceTableService.PodeGerir(Perfil))
            {
                Response.Redirect("~/AcessoNegado.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CarregarTabelas();
            }
        }

        // ===================== Tabelas =====================

        private void CarregarTabelas()
        {
            var tabelas = _priceTableService.Listar(null);

            rptTabelas.DataSource = tabelas;
            rptTabelas.DataBind();

            phVazioTabelas.Visible = tabelas.Count == 0;
        }

        protected bool IsTabelaSelecionada(object priceTableId) =>
            PriceTableIdSelecionado.HasValue && (int)priceTableId == PriceTableIdSelecionado.Value;

        private void LimparFormularioTabela()
        {
            PriceTableIdEmEdicao = null;
            txtNomeTabela.Text = string.Empty;
            chkPredefinida.Checked = false;
            litModoEdicaoTabela.Visible = false;
            btnCancelarTabela.Visible = false;
        }

        protected void cvRegrasTabela_ServerValidate(object source, ServerValidateEventArgs args)
        {
            bool nomeJaExiste = _priceTableService.ExisteNome(txtNomeTabela.Text.Trim(), PriceTableIdEmEdicao);

            var priceTable = new PriceTable { Name = txtNomeTabela.Text.Trim() };
            var erros = _priceTableService.Validar(priceTable, nomeJaExiste);

            args.IsValid = erros.Count == 0;
            cvRegrasTabela.ErrorMessage = string.Join(" ", erros);
        }

        protected void btnGuardarTabela_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            if (PriceTableIdEmEdicao.HasValue)
            {
                var priceTable = new PriceTable
                {
                    PriceTableId = PriceTableIdEmEdicao.Value,
                    Name = txtNomeTabela.Text.Trim(),
                    IsDefault = chkPredefinida.Checked,
                    UpdatedBy = UserId
                };
                _priceTableService.Atualizar(priceTable);
                NotificacaoService.Sucesso("Tabela de preços atualizada.");
            }
            else
            {
                var priceTable = new PriceTable
                {
                    Name = txtNomeTabela.Text.Trim(),
                    IsDefault = chkPredefinida.Checked,
                    CreatedBy = UserId
                };
                _priceTableService.Criar(priceTable);
                NotificacaoService.Sucesso("Tabela de preços criada.");
            }

            LimparFormularioTabela();
            CarregarTabelas();
        }

        protected void btnCancelarTabela_Click(object sender, EventArgs e)
        {
            LimparFormularioTabela();
        }

        protected void rptTabelas_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int priceTableId = int.Parse(e.CommandArgument.ToString());

            switch (e.CommandName)
            {
                case "Editar":
                    var priceTable = _priceTableService.GetById(priceTableId);
                    if (priceTable == null) return;

                    PriceTableIdEmEdicao = priceTable.PriceTableId;
                    txtNomeTabela.Text = priceTable.Name;
                    chkPredefinida.Checked = priceTable.IsDefault;
                    litModoEdicaoTabela.Text = $"A editar: {priceTable.Name}";
                    litModoEdicaoTabela.Visible = true;
                    btnCancelarTabela.Visible = true;
                    break;

                case "AlternarEstado":
                    if (_priceTableService.AlternarEstado(priceTableId, UserId))
                    {
                        NotificacaoService.Sucesso("Estado da tabela atualizado.");
                    }
                    else
                    {
                        NotificacaoService.Erro("Não é possível desativar a tabela predefinida — define outra como predefinida primeiro.");
                    }
                    CarregarTabelas();
                    break;

                case "GerirItens":
                    PriceTableIdSelecionado = priceTableId;
                    LimparFormularioItem();
                    CarregarTabelas();
                    CarregarItens();
                    break;
            }
        }

        // ===================== Itens (preço por produto) =====================

        private void CarregarItens()
        {
            if (!PriceTableIdSelecionado.HasValue)
            {
                phItens.Visible = false;
                return;
            }

            var tabela = _priceTableService.GetById(PriceTableIdSelecionado.Value);
            if (tabela == null)
            {
                PriceTableIdSelecionado = null;
                phItens.Visible = false;
                return;
            }

            phItens.Visible = true;
            litNomeTabelaItens.Text = Server.HtmlEncode(tabela.Name);

            var itens = _priceTableItemService.ListarPorTabela(PriceTableIdSelecionado.Value);
            rptItens.DataSource = itens;
            rptItens.DataBind();
            phVazioItens.Visible = itens.Count == 0;
        }

        protected void lnkFecharItens_Click(object sender, EventArgs e)
        {
            PriceTableIdSelecionado = null;
            LimparFormularioItem();
            CarregarTabelas();
        }

        private void LimparFormularioItem()
        {
            PriceTableItemIdEmEdicao = null;
            ucSeletorProduto.ProdutoId = null;
            ucSeletorProduto.Enabled = true;
            txtPrecoItem.Text = string.Empty;
            litModoEdicaoItem.Visible = false;
            btnCancelarItem.Visible = false;
            btnGuardarItem.Text = "Adicionar";
        }

        protected void cvRegrasItem_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (!PriceTableIdSelecionado.HasValue)
            {
                args.IsValid = false;
                cvRegrasItem.ErrorMessage = "Nenhuma tabela selecionada.";
                return;
            }

            int? produtoId = ucSeletorProduto.ProdutoId;
            if (!produtoId.HasValue)
            {
                args.IsValid = false;
                cvRegrasItem.ErrorMessage = "Tens de selecionar um produto.";
                return;
            }

            bool produtoJaTemPreco = _priceTableItemService.ExisteProdutoNaTabela(
                PriceTableIdSelecionado.Value, produtoId.Value, PriceTableItemIdEmEdicao);

            decimal preco = decimal.TryParse(txtPrecoItem.Text, out decimal p) ? p : -1;

            var item = new PriceTableItem { ProductId = produtoId.Value, Price = preco };
            var erros = _priceTableItemService.Validar(item, produtoJaTemPreco);

            args.IsValid = erros.Count == 0;
            cvRegrasItem.ErrorMessage = string.Join(" ", erros);
        }

        protected void btnGuardarItem_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            if (!PriceTableIdSelecionado.HasValue) return;

            decimal preco = decimal.Parse(txtPrecoItem.Text);

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
                    PriceTableId = PriceTableIdSelecionado.Value,
                    ProductId = ucSeletorProduto.ProdutoId.Value,
                    Price = preco,
                    CreatedBy = UserId
                };
                _priceTableItemService.Criar(item);
                NotificacaoService.Sucesso("Preço adicionado à tabela.");
            }

            LimparFormularioItem();
            CarregarItens();
        }

        protected void btnCancelarItem_Click(object sender, EventArgs e)
        {
            LimparFormularioItem();
        }

        protected void rptItens_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int itemId = int.Parse(e.CommandArgument.ToString());

            if (e.CommandName == "Editar")
            {
                var item = _priceTableItemService.GetById(itemId);
                if (item == null) return;

                PriceTableItemIdEmEdicao = item.PriceTableItemId;
                ucSeletorProduto.ProdutoId = item.ProductId;
                ucSeletorProduto.Enabled = false; // trocar de produto = eliminar e criar outro item, não editar
                txtPrecoItem.Text = item.Price.ToString("0.00");
                litModoEdicaoItem.Text = $"A editar preço de: {item.Product?.Name}";
                litModoEdicaoItem.Visible = true;
                btnCancelarItem.Visible = true;
                btnGuardarItem.Text = "Guardar";
                return;
            }

            if (e.CommandName == "Eliminar")
            {
                _priceTableItemService.Eliminar(itemId, UserId);
                NotificacaoService.Sucesso("Preço removido da tabela.");
                CarregarItens();
            }
        }
    }
}