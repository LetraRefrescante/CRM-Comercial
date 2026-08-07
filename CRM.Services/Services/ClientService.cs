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

        public bool TemAmbitoProprios(string perfil) => perfil == "Comercial";

        public bool PodeCriarOuEditar(string perfil) =>
            perfil == "Administrador" || perfil == "Diretor" || perfil == "Comercial";

        public bool PodeEliminar(string perfil) =>
            perfil == "Administrador" || perfil == "Diretor";

        /// <summary>
        /// Determina se o país indicado é Portugal, pelo IsoCode "PT" da tabela Countries.
        /// </summary>
        public bool PaisEhPortugal(int countryId) => _countryRepository.EhPortugal(countryId);

        /// <summary>
        /// Valida o NIF. Fora de Portugal a blueprint não define regra de formato,
        /// por isso só se aplica o algoritmo de dígito de controlo quando país == PT.
        /// </summary>
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

        /// <summary>
        /// Telefone é opcional em ambos os pontos de entrada — vazio é sempre válido.
        /// </summary>
        public bool TelefoneValido(string telefone, bool paisEhPortugal)
        {
            if (string.IsNullOrWhiteSpace(telefone)) return true;

            telefone = telefone.Trim();

            return paisEhPortugal
                ? Regex.IsMatch(telefone, @"^(\+351\s?)?[29]\d{8}$")
                : Regex.IsMatch(telefone, @"^\+?[\d\s\-]{7,20}$");
        }

        /// <summary>
        /// Código postal é opcional; formato só é imposto para Portugal (1234-567).
        /// A blueprint pede "formato por país" mas ainda não há regras definidas para
        /// outros países, por isso qualquer valor é aceite fora de PT.
        /// </summary>
        public bool CodigoPostalValido(string codigoPostal, bool paisEhPortugal)
        {
            if (string.IsNullOrWhiteSpace(codigoPostal)) return true;

            if (!paisEhPortugal) return true;

            return Regex.IsMatch(codigoPostal.Trim(), @"^\d{4}-\d{3}$");
        }

        public ResultadoGuardarCliente Criar(Client client, string perfil)
        {
            if (!PodeCriarOuEditar(perfil))
                return ResultadoGuardarCliente.SemPermissao;

            if (_clientRepository.NifAtivoExiste(client.VatNumber))
                return ResultadoGuardarCliente.NifDuplicado;

            client.InternalCode = _clientRepository.GerarProximoCodigoInterno();
            int clientId = _clientRepository.Criar(client);

            _auditService.Registar(client.CreatedBy, "Create", "Client", clientId.ToString(),
                $"Cliente '{client.TradeName}' (NIF {client.VatNumber}) criado.");

            return ResultadoGuardarCliente.Sucesso;
        }

        public ResultadoGuardarCliente Atualizar(Client client, string perfil)
        {
            if (!PodeCriarOuEditar(perfil))
                return ResultadoGuardarCliente.SemPermissao;

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