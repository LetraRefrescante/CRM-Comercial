using CRM.Models.Entities.Catalogo;
using CRM.Services;
using CRM.Web.Helpers;
using System;

namespace CRM.Web.Paginas.Catalogo
{
    public partial class TabelasPreco : PaginaBase
    {
        private readonly PriceTableService _priceTableService = new PriceTableService();

        private int? PriceTableIdEmEdicao
        {
            get => ViewState["PriceTableIdEmEdicao"] as int?;
            set => ViewState["PriceTableIdEmEdicao"] = value;
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

        private void CarregarTabelas()
        {
            var tabelas = _priceTableService.Listar(null);

            rptTabelas.DataSource = tabelas;
            rptTabelas.DataBind();

            phVazio.Visible = tabelas.Count == 0;
        }

        private void LimparFormulario()
        {
            PriceTableIdEmEdicao = null;
            txtNome.Text = string.Empty;
            chkPredefinida.Checked = false;
            litModoEdicao.Visible = false;
            btnCancelar.Visible = false;
        }

        protected void cvRegrasNegocio_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            bool nomeJaExiste = _priceTableService.ExisteNome(txtNome.Text.Trim(), PriceTableIdEmEdicao);

            var priceTable = new PriceTable { Name = txtNome.Text.Trim() };
            var erros = _priceTableService.Validar(priceTable, nomeJaExiste);

            args.IsValid = erros.Count == 0;
            cvRegrasNegocio.ErrorMessage = string.Join(" ", erros);
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            if (PriceTableIdEmEdicao.HasValue)
            {
                var priceTable = new PriceTable
                {
                    PriceTableId = PriceTableIdEmEdicao.Value,
                    Name = txtNome.Text.Trim(),
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
                    Name = txtNome.Text.Trim(),
                    IsDefault = chkPredefinida.Checked,
                    CreatedBy = UserId
                };
                _priceTableService.Criar(priceTable);
                NotificacaoService.Sucesso("Tabela de preços criada.");
            }

            LimparFormulario();
            CarregarTabelas();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            LimparFormulario();
        }

        protected void rptTabelas_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            int priceTableId = int.Parse(e.CommandArgument.ToString());

            if (e.CommandName == "Editar")
            {
                var priceTable = _priceTableService.GetById(priceTableId);
                if (priceTable == null) return;

                PriceTableIdEmEdicao = priceTable.PriceTableId;
                txtNome.Text = priceTable.Name;
                chkPredefinida.Checked = priceTable.IsDefault;
                litModoEdicao.Text = $"A editar: {priceTable.Name}";
                litModoEdicao.Visible = true;
                btnCancelar.Visible = true;
                return;
            }

            if (e.CommandName == "AlternarEstado")
            {
                if (_priceTableService.AlternarEstado(priceTableId, UserId))
                {
                    NotificacaoService.Sucesso("Estado da tabela de preços atualizado.");
                }
                else
                {
                    NotificacaoService.Erro("Não é possível desativar a tabela de preços predefinida. Define outra tabela como predefinida primeiro.");
                }

                CarregarTabelas();
            }
        }
    }
}