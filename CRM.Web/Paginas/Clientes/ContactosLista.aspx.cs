using System;
using CRM.Business.Services;
using CRM.Data.Repositories;
using CRM.Web.Helpers;

namespace CRM.Web.Paginas.Clientes
{
    public partial class ContactosLista : PaginaBase
    {
        private readonly ContactRepository _contactRepository = new ContactRepository();
        private readonly ContactService _contactService = new ContactService();
        private readonly ClientService _clientService = new ClientService();
        private readonly UserRepository _userRepository = new UserRepository();

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
            phNovoContacto.Visible = _contactService.PodeCriarOuEditar(Perfil);

            if (!IsPostBack)
            {
                CarregarComerciais();
                CarregarContactos();
            }
        }

        private void CarregarComerciais()
        {
            bool podeFiltrarPorComercial = !_clientService.TemAmbitoProprios(Perfil);

            colComercial.Visible = podeFiltrarPorComercial;

            if (!podeFiltrarPorComercial) return;

            ddlComercial.Items.Add(new System.Web.UI.WebControls.ListItem("Todos", ""));
            foreach (var user in _userRepository.ListarComerciaisAtivos())
            {
                ddlComercial.Items.Add(new System.Web.UI.WebControls.ListItem(user.Name, user.UserId.ToString()));
            }
        }

        private int? ObterFiltroComercial()
        {
            if (_clientService.TemAmbitoProprios(Perfil)) return UserId;

            if (colComercial.Visible && !string.IsNullOrEmpty(ddlComercial.SelectedValue))
                return int.Parse(ddlComercial.SelectedValue);

            return null;
        }

        private void CarregarContactos()
        {
            var contactos = _contactRepository.ListarGlobal(
                pesquisa: txtPesquisa.Text.Trim(),
                accountManagerId: ObterFiltroComercial(),
                pagina: ucPaginacao.PaginaAtual,
                tamanhoPagina: ucPaginacao.TamanhoPagina,
                totalRegistos: out int total,
                sortColumn: SortColumn,
                sortAscending: SortAscending);

            ucPaginacao.TotalRegistos = total;

            rptContactos.DataSource = contactos;
            rptContactos.DataBind();

            phVazio.Visible = contactos.Count == 0;
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            ucPaginacao.PaginaAtual = 1;
            CarregarContactos();
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
            CarregarContactos();
        }

        protected void ucSeletorCliente_ClienteSelecionado(object sender, EventArgs e)
        {
            Response.Redirect($"~/Clientes/ContactoEditar.aspx?clienteId={ucSeletorCliente.ClienteId}");
        }

        protected void ucPaginacao_PaginaAlterada(object sender, EventArgs e)
        {
            CarregarContactos();
        }

        protected void rptContactos_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != System.Web.UI.WebControls.ListItemType.Item &&
                e.Item.ItemType != System.Web.UI.WebControls.ListItemType.AlternatingItem)
                return;

            var phEditar = e.Item.FindControl("phEditar") as System.Web.UI.WebControls.PlaceHolder;
            var phEliminar = e.Item.FindControl("phEliminar") as System.Web.UI.WebControls.PlaceHolder;

            if (phEditar != null) phEditar.Visible = _contactService.PodeCriarOuEditar(Perfil);
            if (phEliminar != null) phEliminar.Visible = _contactService.PodeEliminar(Perfil);
        }

        protected void rptContactos_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Eliminar")
            {
                var partes = e.CommandArgument.ToString().Split('|');
                int contactId = int.Parse(partes[0]);
                int clientId = int.Parse(partes[1]);

                if (_contactService.Eliminar(contactId, clientId, UserId, Perfil, UserId))
                {
                    NotificacaoService.Sucesso("Contacto eliminado.");
                }
                else
                {
                    NotificacaoService.Erro("Não tens permissão para eliminar este contacto.");
                }

                CarregarContactos();
            }
        }
    }
}