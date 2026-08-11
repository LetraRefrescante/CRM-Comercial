using CRM.Data.Repositories;
using CRM.Models.Entities.Vendas;
using CRM.Services;
using CRM.Web.Helpers;
using System;

namespace CRM.Web.Paginas.Vendas
{
    public partial class VendasLista : PaginaBase
    {
        private readonly SaleService _saleService = new SaleService();
        private readonly UserRepository _userRepository = new UserRepository();

        private string SortColumn
        {
            get => ViewState["SortColumn"] as string ?? "SaleDate";
            set => ViewState["SortColumn"] = value;
        }

        private bool SortAscending
        {
            get => ViewState["SortAscending"] as bool? ?? false;
            set => ViewState["SortAscending"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarComerciais();
                CarregarVendas();
            }
        }

        private void CarregarComerciais()
        {
            bool podeFiltrarPorComercial = !_saleService.TemAmbitoProprios(Perfil);

            ddlComercial.Visible = podeFiltrarPorComercial;
            lblComercial.Visible = podeFiltrarPorComercial;

            if (!podeFiltrarPorComercial) return;

            ddlComercial.Items.Add(new System.Web.UI.WebControls.ListItem("Todos", ""));
            foreach (var user in _userRepository.ListarComerciaisAtivos())
            {
                ddlComercial.Items.Add(new System.Web.UI.WebControls.ListItem(user.Name, user.UserId.ToString()));
            }
        }

        private int? ObterFiltroComercial()
        {
            if (_saleService.TemAmbitoProprios(Perfil)) return UserId;

            if (ddlComercial.Visible && !string.IsNullOrEmpty(ddlComercial.SelectedValue))
                return int.Parse(ddlComercial.SelectedValue);

            return null;
        }

        private void CarregarVendas()
        {
            var vendas = _saleService.Listar(
                pesquisa: txtPesquisa.Text.Trim(),
                status: ddlEstado.SelectedValue,
                clientId: null,
                ownerId: ObterFiltroComercial(),
                dataInicio: ucFiltroDatas.DataInicial,
                dataFim: ucFiltroDatas.DataFinal,
                pagina: ucPaginacao.PaginaAtual,
                tamanhoPagina: ucPaginacao.TamanhoPagina,
                totalRegistos: out int total,
                sortColumn: SortColumn,
                sortAscending: SortAscending);

            ucPaginacao.TotalRegistos = total;

            rptVendas.DataSource = vendas;
            rptVendas.DataBind();

            phVazio.Visible = vendas.Count == 0;

            lnkNova.Visible = _saleService.PodeCriarOuEditar(Perfil);
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            ucPaginacao.PaginaAtual = 1;
            CarregarVendas();
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
            CarregarVendas();
        }

        protected void ucPaginacao_PaginaAlterada(object sender, EventArgs e)
        {
            CarregarVendas();
        }

        protected void rptVendas_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != System.Web.UI.WebControls.ListItemType.Item &&
                e.Item.ItemType != System.Web.UI.WebControls.ListItemType.AlternatingItem)
                return;

            var sale = (Sale)e.Item.DataItem;

            var phEliminar = e.Item.FindControl("phEliminar") as System.Web.UI.WebControls.PlaceHolder;

            if (phEliminar != null)
                phEliminar.Visible = _saleService.PodeEliminar(Perfil);
        }

        protected void rptVendas_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Eliminar")
            {
                int saleId = int.Parse(e.CommandArgument.ToString());

                if (_saleService.Eliminar(saleId, UserId, Perfil))
                {
                    NotificacaoService.Sucesso("Venda eliminada.");
                }
                else
                {
                    NotificacaoService.Erro("Não tens permissão para eliminar vendas.");
                }

                CarregarVendas();
            }
        }

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