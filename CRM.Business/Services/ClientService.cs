using System;
using CRM.Data.Repositories;
using CRM.Models.Entities.Clientes;

namespace CRM.Business.Services
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

        /// <summary>
        /// Determina se o perfil vê apenas os seus próprios clientes (regra "PRÓPRIOS" da blueprint).
        /// </summary>
        public bool TemAmbitoProprios(string perfil) => perfil == "Comercial";

        public bool PodeCriarOuEditar(string perfil) =>
            perfil == "Administrador" || perfil == "Diretor" || perfil == "Comercial";

        public bool PodeEliminar(string perfil) =>
            perfil == "Administrador" || perfil == "Diretor";

        public ResultadoGuardarCliente Criar(Client client, string perfil)
        {
            if (!PodeCriarOuEditar(perfil))
                return ResultadoGuardarCliente.SemPermissao;

            if (_clientRepository.NifAtivoExiste(client.VatNumber))
                return ResultadoGuardarCliente.NifDuplicado;

            client.InternalCode = _clientRepository.GerarProximoCodigoInterno();
            _clientRepository.Criar(client);
            return ResultadoGuardarCliente.Sucesso;
        }

        public ResultadoGuardarCliente Atualizar(Client client, string perfil)
        {
            if (!PodeCriarOuEditar(perfil))
                return ResultadoGuardarCliente.SemPermissao;

            if (_clientRepository.NifAtivoExiste(client.VatNumber, client.ClientId))
                return ResultadoGuardarCliente.NifDuplicado;

            _clientRepository.Atualizar(client);
            return ResultadoGuardarCliente.Sucesso;
        }

        public bool Eliminar(int clientId, int eliminadoPor, string perfil)
        {
            if (!PodeEliminar(perfil)) return false;

            _clientRepository.EliminarLogico(clientId, eliminadoPor);
            return true;
        }
    }
}