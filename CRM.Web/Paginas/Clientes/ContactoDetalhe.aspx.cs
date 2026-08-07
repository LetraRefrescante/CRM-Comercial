using CRM.Services;
using CRM.Data.Repositories;
using CRM.Web.Helpers;
using System;

namespace CRM.Web.Paginas.Clientes
{
    public partial class ContactoDetalhe : PaginaBase
    {
        private readonly ContactRepository _contactRepository = new ContactRepository();
        private readonly ContactService _contactService = new ContactService();
        private readonly ClientRepository _clientRepository = new ClientRepository();

        public int ContactIdAtual { get; private set; }
        public int ClientIdAtual { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!int.TryParse(Request.QueryString["id"], out int id))
            {
                Response.Redirect("~/Clientes/ContactosLista.aspx");
                return;
            }

            ContactIdAtual = id;
            var contact = _contactRepository.GetById(id);

            if (contact == null)
            {
                NotificacaoService.Erro("Contacto não encontrado.");
                Response.Redirect("~/Clientes/ContactosLista.aspx");
                return;
            }

            if (!_contactService.TemAcessoAoCliente(contact.ClientId, Perfil, UserId))
            {
                NotificacaoService.Erro("Não tens permissão para consultar este contacto.");
                Response.Redirect("~/Clientes/ContactosLista.aspx");
                return;
            }

            ClientIdAtual = contact.ClientId;
            var client = _clientRepository.GetById(contact.ClientId);

            litNome.Text = contact.Name;
            litNomeBreadcrumb.Text = contact.Name;
            litClienteNome.Text = client?.TradeName ?? "—";
            lnkClientePai.InnerText = client?.TradeName ?? "Cliente";
            lnkClientePai.Attributes["href"] = $"~/Clientes/ClienteDetalhe.aspx?id={contact.ClientId}";
            litCargo.Text = string.IsNullOrEmpty(contact.JobTitle) ? "—" : contact.JobTitle;
            litDepartamento.Text = string.IsNullOrEmpty(contact.Department) ? "—" : contact.Department;
            litEmail.Text = string.IsNullOrEmpty(contact.Email) ? "—" : contact.Email;
            litTelefone.Text = string.IsNullOrEmpty(contact.Phone) ? "—" : contact.Phone;
            litTelemovel.Text = string.IsNullOrEmpty(contact.MobilePhone) ? "—" : contact.MobilePhone;
            litDataNascimento.Text = contact.BirthDate.HasValue ? contact.BirthDate.Value.ToString("dd/MM/yyyy") : "—";
            litPrincipal.Text = contact.IsPrimary ? "Sim" : "Não";
            litPreferencia.Text = string.IsNullOrEmpty(contact.ContactPreference) ? "—" : contact.ContactPreference;
            litConsentimento.Text = contact.ConsentGiven ? "Sim" : "Não";
            litRestricoes.Text = string.IsNullOrEmpty(contact.ContactRestrictions) ? "—" : contact.ContactRestrictions;

            phEditar.Visible = _contactService.PodeCriarOuEditar(Perfil);
            phEliminar.Visible = _contactService.PodeEliminar(Perfil);

            ucHistorico.Inicializar("Contact", ContactIdAtual.ToString());

            // Fora de escopo por agora: quando o módulo de Atividades existir (Fase 5 da
            // blueprint), esta secção deve listar as atividades relacionadas com o contacto.
        }

        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            if (_contactService.Eliminar(ContactIdAtual, ClientIdAtual, UserId, Perfil, UserId))
            {
                NotificacaoService.Sucesso("Contacto eliminado.");
                Response.Redirect($"~/Clientes/ClienteDetalhe.aspx?id={ClientIdAtual}");
            }
            else
            {
                NotificacaoService.Erro("Não tens permissão para eliminar este contacto.");
            }
        }
    }
}