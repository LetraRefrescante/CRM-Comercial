using CRM.Data.Repositories;
using CRM.Web.Helpers;
using System;
using System.Web.UI.WebControls;

namespace CRM.Web.Paginas.Clientes
{
    public partial class ClienteDetalhe : System.Web.UI.Page
    {
        private readonly ClientRepository _clientRepository = new ClientRepository();

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

            string perfil = Session["RoleName"] as string;
            int userId = (int)Session["UserId"];

            if (perfil == "Comercial" && client.AccountManagerId != userId)
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
        }
    }
}