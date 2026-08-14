using CRM.Data.Repositories;
using CRM.Models.Entities.Clientes;

namespace CRM.Services
{
    public enum ResultadoGuardarContacto
    {
        Sucesso,
        SemPermissao
    }

    public class ContactService
    {
        private readonly ContactRepository _contactRepository = new ContactRepository();
        private readonly ClientRepository _clientRepository = new ClientRepository();
        private readonly AuditService _auditService = new AuditService();
        private readonly PermissionService _permissionService = new PermissionService();

        // Contactos não têm coluna própria na matriz — partilham o módulo "Clientes"
        // (o blueprint trata Contactos como sub-secção de Clientes, e não há Permissions
        // com prefixo "Contactos." no seed).
        private const string Modulo = "Clientes";

        public bool PodeCriarOuEditar(string perfil) =>
            _permissionService.ObterNivel(perfil, Modulo) >= NivelAcesso.Proprios;

        public bool PodeEliminar(string perfil) =>
            _permissionService.ObterNivel(perfil, Modulo) == NivelAcesso.Total;

        public bool TemAcessoAoCliente(int clientId, string perfil, int userId)
        {
            if (_permissionService.ObterNivel(perfil, Modulo) != NivelAcesso.Proprios) return true;

            var client = _clientRepository.GetById(clientId);
            return client != null && client.AccountManagerId == userId;
        }

        public ResultadoGuardarContacto Criar(Contact contact, string perfil, int userId)
        {
            if (!PodeCriarOuEditar(perfil) || !TemAcessoAoCliente(contact.ClientId, perfil, userId))
                return ResultadoGuardarContacto.SemPermissao;

            int contactId = _contactRepository.Criar(contact);

            _auditService.Registar(contact.CreatedBy, "Create", "Contact", contactId.ToString(),
                $"Contacto '{contact.Name}' criado (ClientId {contact.ClientId}).");

            return ResultadoGuardarContacto.Sucesso;
        }

        public ResultadoGuardarContacto Atualizar(Contact contact, string perfil, int userId)
        {
            if (!PodeCriarOuEditar(perfil) || !TemAcessoAoCliente(contact.ClientId, perfil, userId))
                return ResultadoGuardarContacto.SemPermissao;

            _contactRepository.Atualizar(contact);

            _auditService.Registar(contact.UpdatedBy, "Update", "Contact", contact.ContactId.ToString(),
                $"Contacto '{contact.Name}' atualizado.");

            return ResultadoGuardarContacto.Sucesso;
        }

        public bool Eliminar(int contactId, int clientId, int eliminadoPor, string perfil, int userId)
        {
            if (!PodeEliminar(perfil) || !TemAcessoAoCliente(clientId, perfil, userId))
                return false;

            _contactRepository.EliminarLogico(contactId, eliminadoPor);

            _auditService.Registar(eliminadoPor, "Delete", "Contact", contactId.ToString(),
                "Contacto eliminado (soft delete).");

            return true;
        }
    }
}