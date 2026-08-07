using System;
using System.Text;
using CRM.Business.Services;
using CRM.Data.Repositories;
using CRM.Web.Helpers;

namespace CRM.Web.Paginas.Clientes
{
    public partial class ClienteLista : PaginaBase
    {
        private readonly ClientRepository _clientRepository = new ClientRepository();
        private readonly ClientService _clientService = new ClientService();
        private readonly UserRepository _userRepository = new UserRepository();

        private string SortColumn
        {
            get => ViewState["SortColumn"] as string ?? "TradeName";
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
                CarregarComerciais();
                CarregarClientes();
            }
        }

        private void CarregarComerciais()
        {
            bool podeFiltrarPorComercial = !_clientService.TemAmbitoProprios(Perfil);

            ddlComercial.Visible = podeFiltrarPorComercial;
            lblComercial.Visible = podeFiltrarPorComercial;

            if (!podeFiltrarPorComercial) return;

            ddlComercial.Items.Add(new System.Web.UI.WebControls.ListItem("Todos", ""));
            var comerciais = _userRepository.ListarComerciaisAtivos();
            foreach (var user in comerciais)
            {
                ddlComercial.Items.Add(new System.Web.UI.WebControls.ListItem(user.Name, user.UserId.ToString()));
            }
        }

        private int? ObterFiltroComercial()
        {
            if (_clientService.TemAmbitoProprios(Perfil)) return UserId;

            if (ddlComercial.Visible && !string.IsNullOrEmpty(ddlComercial.SelectedValue))
                return int.Parse(ddlComercial.SelectedValue);

            return null;
        }

        private void CarregarClientes()
        {
            var clientes = _clientRepository.Listar(
                pesquisa: txtPesquisa.Text.Trim(),
                status: ddlEstado.SelectedValue,
                accountManagerId: ObterFiltroComercial(),
                pagina: ucPaginacao.PaginaAtual,
                tamanhoPagina: ucPaginacao.TamanhoPagina,
                totalRegistos: out int total,
                sortColumn: SortColumn,
                sortAscending: SortAscending);

            ucPaginacao.TotalRegistos = total;

            rptClientes.DataSource = clientes;
            rptClientes.DataBind();

            phVazio.Visible = clientes.Count == 0;

            lnkNovo.Visible = _clientService.PodeCriarOuEditar(Perfil);
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            ucPaginacao.PaginaAtual = 1;
            CarregarClientes();
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
            CarregarClientes();
        }

        protected void btnExportar_Click(object sender, EventArgs e)
        {
            var clientes = _clientRepository.ListarParaExportacao(
                pesquisa: txtPesquisa.Text.Trim(),
                status: ddlEstado.SelectedValue,
                accountManagerId: ObterFiltroComercial());

            var csv = new StringBuilder();
            csv.AppendLine("Código;Nome Comercial;NIF;Cidade;Comercial;Estado");

            foreach (var cliente in clientes)
            {
                csv.AppendLine(string.Join(";",
                    EscaparCsv(cliente.InternalCode),
                    EscaparCsv(cliente.TradeName),
                    EscaparCsv(cliente.VatNumber),
                    EscaparCsv(cliente.City),
                    EscaparCsv(cliente.AccountManager?.Name),
                    EscaparCsv(cliente.Status)));
            }

            Response.Clear();
            Response.ContentType = "text/csv; charset=utf-8";
            Response.AddHeader("Content-Disposition", $"attachment; filename=Clientes_{DateTime.Now:yyyyMMddHHmm}.csv");
            Response.BinaryWrite(Encoding.UTF8.GetPreamble());
            Response.Write(csv.ToString());
            Response.End();
        }

        private string EscaparCsv(string valor)
        {
            if (string.IsNullOrEmpty(valor)) return "";
            return valor.Replace(";", ",").Replace("\r", " ").Replace("\n", " ");
        }

        protected void ucPaginacao_PaginaAlterada(object sender, EventArgs e)
        {
            CarregarClientes();
        }

        protected void rptClientes_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != System.Web.UI.WebControls.ListItemType.Item &&
                e.Item.ItemType != System.Web.UI.WebControls.ListItemType.AlternatingItem)
                return;

            var phEditar = e.Item.FindControl("phEditar") as System.Web.UI.WebControls.PlaceHolder;
            var phEliminar = e.Item.FindControl("phEliminar") as System.Web.UI.WebControls.PlaceHolder;

            if (phEditar != null) phEditar.Visible = _clientService.PodeCriarOuEditar(Perfil);
            if (phEliminar != null) phEliminar.Visible = _clientService.PodeEliminar(Perfil);
        }

        protected void rptClientes_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Eliminar")
            {
                if (!_clientService.PodeEliminar(Perfil))
                {
                    NotificacaoService.Erro("Não tens permissão para eliminar clientes.");
                    return;
                }

                int clientId = int.Parse(e.CommandArgument.ToString());

                if (_clientService.Eliminar(clientId, UserId, Perfil))
                {
                    NotificacaoService.Sucesso("Cliente eliminado.");
                }
                else
                {
                    NotificacaoService.Erro("Não tens permissão para eliminar clientes.");
                }

                CarregarClientes();
            }
        }

        protected string GetBadgeClasse(string status)
        {
            switch (status)
            {
                case "Ativo": return "badge-ativo";
                case "Potencial": return "badge-potencial";
                case "Bloqueado": return "badge-bloqueado";
                case "Inativo": return "badge-inativo";
                default: return "bg-secondary";
            }
        }
    }
}