using System;
using System.Text.RegularExpressions;
using CRM.Data.Repositories;
using CRM.Models.Entities.Clientes;

namespace CRM.Services
{
    public enum ResultadoGuardarCliente
    {
        Sucesso,
        NifDuplicado,
        SemPermissao
    }

    public class ClientService
    {
        private readonly ClientRepository _clientRepository = new ClientRepository();
        private readonly CountryRepository _countryRepository = new CountryRepository();
        private readonly AuditService _auditService = new AuditService();
        private readonly PermissionService _permissionService = new PermissionService();

        private const string Modulo = "Clientes";

        public bool TemAmbitoProprios(string perfil) =>
            _permissionService.ObterNivel(perfil, Modulo) == NivelAcesso.Proprios;

        public bool PodeCriarOuEditar(string perfil) =>
            _permissionService.ObterNivel(perfil, Modulo) >= NivelAcesso.Proprios;

        public bool PodeEliminar(string perfil) =>
            _permissionService.ObterNivel(perfil, Modulo) == NivelAcesso.Total;

        public bool PaisEhPortugal(int countryId) => _countryRepository.EhPortugal(countryId);

        public bool NifValido(string nif, bool paisEhPortugal)
        {
            if (!paisEhPortugal) return true;

            if (string.IsNullOrWhiteSpace(nif)) return false;

            nif = nif.Trim();

            if (!Regex.IsMatch(nif, @"^\d{9}$")) return false;

            int soma = 0;
            for (int i = 0; i < 8; i++)
            {
                soma += (nif[i] - '0') * (9 - i);
            }

            int resto = soma % 11;
            int digitoControlo = resto < 2 ? 0 : 11 - resto;

            return digitoControlo == (nif[8] - '0');
        }

        public bool TelefoneValido(string telefone, bool paisEhPortugal)
        {
            if (string.IsNullOrWhiteSpace(telefone)) return true;

            telefone = telefone.Trim();

            return paisEhPortugal
                ? Regex.IsMatch(telefone, @"^(\+351\s?)?[29]\d{8}$")
                : Regex.IsMatch(telefone, @"^\+?[\d\s\-]{7,20}$");
        }

        public bool CodigoPostalValido(string codigoPostal, bool paisEhPortugal)
        {
            if (string.IsNullOrWhiteSpace(codigoPostal)) return true;

            if (!paisEhPortugal) return true;

            return Regex.IsMatch(codigoPostal.Trim(), @"^\d{4}-\d{3}$");
        }
        public ResultadoGuardarCliente Criar(Client client, string perfil, int userId)
        {
            if (!PodeCriarOuEditar(perfil))
                return ResultadoGuardarCliente.SemPermissao;

            if (TemAmbitoProprios(perfil) && client.AccountManagerId != userId)
                return ResultadoGuardarCliente.SemPermissao;

            if (_clientRepository.NifAtivoExiste(client.VatNumber))
                return ResultadoGuardarCliente.NifDuplicado;

            client.InternalCode = _clientRepository.GerarProximoCodigoInterno();
            int clientId = _clientRepository.Criar(client);

            _auditService.Registar(client.CreatedBy, "Create", "Client", clientId.ToString(),
                $"Cliente '{client.TradeName}' (NIF {client.VatNumber}) criado.");

            return ResultadoGuardarCliente.Sucesso;
        }
        public ResultadoGuardarCliente Atualizar(Client client, string perfil, int userId)
        {
            if (!PodeCriarOuEditar(perfil))
                return ResultadoGuardarCliente.SemPermissao;

            if (TemAmbitoProprios(perfil))
            {
                var existente = _clientRepository.GetById(client.ClientId);
                if (existente == null || existente.AccountManagerId != userId)
                    return ResultadoGuardarCliente.SemPermissao;

                if (client.AccountManagerId != userId)
                    return ResultadoGuardarCliente.SemPermissao;
            }

            if (_clientRepository.NifAtivoExiste(client.VatNumber, client.ClientId))
                return ResultadoGuardarCliente.NifDuplicado;

            _clientRepository.Atualizar(client);

            _auditService.Registar(client.UpdatedBy, "Update", "Client", client.ClientId.ToString(),
                $"Cliente '{client.TradeName}' atualizado.");

            return ResultadoGuardarCliente.Sucesso;
        }

        public bool Eliminar(int clientId, int eliminadoPor, string perfil)
        {
            if (!PodeEliminar(perfil)) return false;

            _clientRepository.EliminarLogico(clientId, eliminadoPor);

            _auditService.Registar(eliminadoPor, "Delete", "Client", clientId.ToString(),
                "Cliente eliminado (soft delete).");

            return true;
        }
    }
}