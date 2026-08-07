using CRM.Business.Services;
using CRM.Data.Repositories;
using CRM.Web.Helpers;
using System;

namespace CRM.Web.Paginas.Clientes
{
    public partial class ClienteDetalhe : PaginaBase
    {
        private readonly ClientRepository _clientRepository = new ClientRepository();
        private readonly ContactRepository _contactRepository = new ContactRepository();
        private readonly ContactService _contactService = new ContactService();
        private readonly ClientService _clientService = new ClientService();

        public int ClientIdAtual { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!int.TryParse(Request.QueryString["id"], out int id))
            {
                Response.Redirect("~/Clientes/ClienteLista.aspx");
                return;
            }

            ClientIdAtual = id;
            var client = _clientRepository.GetById(id);

            if (client == null)
            {
                NotificacaoService.Erro("Cliente não encontrado.");
                Response.Redirect("~/Clientes/ClienteLista.aspx");
                return;
            }

            if (_clientService.TemAmbitoProprios(Perfil) && client.AccountManagerId != UserId)
            {
                NotificacaoService.Erro("Não tens permissão para consultar este cliente.");
                Response.Redirect("~/Clientes/ClienteLista.aspx");
                return;
            }

            litNome.Text = client.TradeName;
            litNomeBreadcrumb.Text = client.TradeName;
            litCodigo.Text = client.InternalCode;
            litNif.Text = client.VatNumber;
            litEstado.Text = client.Status;
            litComercial.Text = client.AccountManager?.Name ?? "—";
            litEmail.Text = string.IsNullOrEmpty(client.Email) ? "—" : client.Email;
            litTelefone.Text = string.IsNullOrEmpty(client.Phone) ? "—" : client.Phone;
            litMorada.Text = string.IsNullOrEmpty(client.Address) ? "—" : client.Address;
            lnkNovoContacto.NavigateUrl = $"~/Clientes/ContactoEditar.aspx?clienteId={ClientIdAtual}";

            ucAnexos.Inicializar("Client", ClientIdAtual, UserId);
            ucHistorico.Inicializar("Client", ClientIdAtual.ToString());

            if (!IsPostBack)
            {
                CarregarContactos();
            }
        }

        private void CarregarContactos()
        {
            var contactos = _contactRepository.ListarPorCliente(ClientIdAtual);
            rptContactos.DataSource = contactos;
            rptContactos.DataBind();
            phContactosVazio.Visible = contactos.Count == 0;

            bool podeGerir = _contactService.PodeCriarOuEditar(Perfil);
            bool podeEliminar = _contactService.PodeEliminar(Perfil);
            lnkNovoContacto.Visible = podeGerir;

            foreach (System.Web.UI.WebControls.RepeaterItem item in rptContactos.Items)
            {
                var phEditar = item.FindControl("phEditarContacto") as System.Web.UI.WebControls.PlaceHolder;
                var phEliminar = item.FindControl("phEliminarContacto") as System.Web.UI.WebControls.PlaceHolder;
                if (phEditar != null) phEditar.Visible = podeGerir;
                if (phEliminar != null) phEliminar.Visible = podeEliminar;
            }
        }

        protected void rptContactos_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Eliminar")
            {
                int contactId = int.Parse(e.CommandArgument.ToString());
                if (_contactService.Eliminar(contactId, ClientIdAtual, UserId, Perfil, UserId))
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