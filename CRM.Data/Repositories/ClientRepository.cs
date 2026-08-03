using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using CRM.Data.Context;
using CRM.Models.Entities.Clientes;

namespace CRM.Data.Repositories
{
    public class ClientRepository
    {
        public Client GetById(int clientId)
        {
            using (var context = new CrmDbContext())
            {
                return context.Clients
                    .Include(c => c.Country)
                    .Include(c => c.Sector)
                    .Include(c => c.AccountManager)
                    .Where(c => c.ClientId == clientId && !c.IsDeleted)
                    .SingleOrDefault();
            }
        }

        public List<Client> Listar(
            string pesquisa,
            string status,
            int? accountManagerId,
            int pagina,
            int tamanhoPagina,
            out int totalRegistos)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.Clients
                    .Include(c => c.Country)
                    .Include(c => c.AccountManager)
                    .Where(c => !c.IsDeleted);

                if (!string.IsNullOrWhiteSpace(pesquisa))
                {
                    query = query.Where(c =>
                        c.TradeName.Contains(pesquisa) ||
                        c.VatNumber.Contains(pesquisa) ||
                        (c.Email != null && c.Email.Contains(pesquisa)) ||
                        (c.City != null && c.City.Contains(pesquisa)));
                }

                if (!string.IsNullOrWhiteSpace(status))
                {
                    query = query.Where(c => c.Status == status);
                }

                if (accountManagerId.HasValue)
                {
                    query = query.Where(c => c.AccountManagerId == accountManagerId.Value);
                }

                totalRegistos = query.Count();

                return query
                    .OrderBy(c => c.TradeName)
                    .Skip((pagina - 1) * tamanhoPagina)
                    .Take(tamanhoPagina)
                    .ToList();
            }
        }

        public bool NifAtivoExiste(string vatNumber, int? ignorarClientId = null)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.Clients.Where(c =>
                    c.VatNumber == vatNumber &&
                    !c.IsDeleted &&
                    c.Status != "Inativo");

                if (ignorarClientId.HasValue)
                {
                    query = query.Where(c => c.ClientId != ignorarClientId.Value);
                }

                return query.Any();
            }
        }

        public string GerarProximoCodigoInterno()
        {
            using (var context = new CrmDbContext())
            {
                int totalClientes = context.Clients.Count(); // inclui eliminados, para nunca repetir código
                return $"CLI{(totalClientes + 1):D5}";
            }
        }

        public int Criar(Client client)
        {
            using (var context = new CrmDbContext())
            {
                client.CreatedDate = DateTime.UtcNow;
                context.Clients.Add(client);
                context.SaveChanges();
                return client.ClientId;
            }
        }

        public void Atualizar(Client clienteAtualizado)
        {
            using (var context = new CrmDbContext())
            {
                var client = context.Clients.Find(clienteAtualizado.ClientId);
                if (client == null) return;

                client.TradeName = clienteAtualizado.TradeName;
                client.LegalName = clienteAtualizado.LegalName;
                client.VatNumber = clienteAtualizado.VatNumber;
                client.Email = clienteAtualizado.Email;
                client.Phone = clienteAtualizado.Phone;
                client.Address = clienteAtualizado.Address;
                client.PostalCode = clienteAtualizado.PostalCode;
                client.City = clienteAtualizado.City;
                client.CountryId = clienteAtualizado.CountryId;
                client.SectorId = clienteAtualizado.SectorId;
                client.AccountManagerId = clienteAtualizado.AccountManagerId;
                client.Status = clienteAtualizado.Status;
                client.Notes = clienteAtualizado.Notes;
                client.UpdatedDate = DateTime.UtcNow;
                client.UpdatedBy = clienteAtualizado.UpdatedBy;

                context.Entry(client).OriginalValues["RowVersion"] = clienteAtualizado.RowVersion;

                TrySave(context);
            }
        }

        public void EliminarLogico(int clientId, int eliminadoPor)
        {
            using (var context = new CrmDbContext())
            {
                var client = context.Clients.Find(clientId);
                if (client == null) return;

                client.IsDeleted = true;
                client.DeletedDate = DateTime.UtcNow;
                client.DeletedBy = eliminadoPor;

                TrySave(context);
            }
        }

        private void TrySave(CrmDbContext context)
        {
            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Outro utilizador alterou o registo entretanto; a página trata o aviso ao utilizador.
                throw;
            }
        }
    }
}