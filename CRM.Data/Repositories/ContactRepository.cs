using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using CRM.Data.Context;
using CRM.Models.Entities.Clientes;

namespace CRM.Data.Repositories
{
    public class ContactRepository
    {
        public Contact GetById(int contactId)
        {
            using (var context = new CrmDbContext())
            {
                return context.Contacts
                    .Include(c => c.Client)
                    .Where(c => c.ContactId == contactId && !c.IsDeleted)
                    .SingleOrDefault();
            }
        }

        public List<Contact> ListarPorCliente(int clientId)
        {
            using (var context = new CrmDbContext())
            {
                return context.Contacts
                    .Where(c => c.ClientId == clientId && !c.IsDeleted)
                    .OrderByDescending(c => c.IsPrimary)
                    .ThenBy(c => c.Name)
                    .ToList();
            }
        }

        /// <summary>
        /// Listagem global de contactos (ContactosLista.aspx), com pesquisa por nome/email/cliente
        /// e âmbito opcional por comercial responsável do cliente associado.
        /// </summary>
        public List<Contact> ListarGlobal(
            string pesquisa,
            int? accountManagerId,
            int pagina,
            int tamanhoPagina,
            out int totalRegistos,
            string sortColumn = "Name",
            bool sortAscending = true)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.Contacts
                    .Include(c => c.Client)
                    .Where(c => !c.IsDeleted && !c.Client.IsDeleted);

                if (!string.IsNullOrWhiteSpace(pesquisa))
                {
                    query = query.Where(c =>
                        c.Name.Contains(pesquisa) ||
                        (c.Email != null && c.Email.Contains(pesquisa)) ||
                        c.Client.TradeName.Contains(pesquisa));
                }

                if (accountManagerId.HasValue)
                {
                    query = query.Where(c => c.Client.AccountManagerId == accountManagerId.Value);
                }

                totalRegistos = query.Count();

                return AplicarOrdenacao(query, sortColumn, sortAscending)
                    .Skip((pagina - 1) * tamanhoPagina)
                    .Take(tamanhoPagina)
                    .ToList();
            }
        }

        // Whitelist explícita de propósito: nunca aceites o nome da coluna
        // diretamente num OrderBy dinâmico por string (isso sim seria uma
        // porta para injeção).
        private IQueryable<Contact> AplicarOrdenacao(IQueryable<Contact> query, string sortColumn, bool sortAscending)
        {
            switch (sortColumn)
            {
                case "Client":
                    return sortAscending
                        ? query.OrderBy(c => c.Client.TradeName).ThenBy(c => c.Name)
                        : query.OrderByDescending(c => c.Client.TradeName).ThenBy(c => c.Name);
                case "JobTitle":
                    return sortAscending ? query.OrderBy(c => c.JobTitle) : query.OrderByDescending(c => c.JobTitle);
                case "Email":
                    return sortAscending ? query.OrderBy(c => c.Email) : query.OrderByDescending(c => c.Email);
                case "Name":
                default:
                    return sortAscending ? query.OrderBy(c => c.Name) : query.OrderByDescending(c => c.Name);
            }
        }

        public int Criar(Contact contact)
        {
            using (var context = new CrmDbContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    contact.CreatedDate = DateTime.UtcNow;

                    if (contact.IsPrimary)
                    {
                        DesmarcarPrincipaisExistentes(context, contact.ClientId, null);
                    }

                    context.Contacts.Add(contact);
                    context.SaveChanges();

                    transaction.Commit();
                    return contact.ContactId;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void Atualizar(Contact contactoAtualizado)
        {
            using (var context = new CrmDbContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var contact = context.Contacts.Find(contactoAtualizado.ContactId);
                    if (contact == null) return;

                    if (contactoAtualizado.IsPrimary && !contact.IsPrimary)
                    {
                        DesmarcarPrincipaisExistentes(context, contact.ClientId, contact.ContactId);
                    }

                    contact.Name = contactoAtualizado.Name;
                    contact.JobTitle = contactoAtualizado.JobTitle;
                    contact.Department = contactoAtualizado.Department;
                    contact.Email = contactoAtualizado.Email;
                    contact.Phone = contactoAtualizado.Phone;
                    contact.MobilePhone = contactoAtualizado.MobilePhone;
                    contact.BirthDate = contactoAtualizado.BirthDate;
                    contact.IsPrimary = contactoAtualizado.IsPrimary;
                    contact.ContactPreference = contactoAtualizado.ContactPreference;
                    contact.ConsentGiven = contactoAtualizado.ConsentGiven;
                    contact.ContactRestrictions = contactoAtualizado.ContactRestrictions;
                    contact.UpdatedDate = DateTime.UtcNow;
                    contact.UpdatedBy = contactoAtualizado.UpdatedBy;

                    context.Entry(contact).OriginalValues["RowVersion"] = contactoAtualizado.RowVersion;

                    context.SaveChanges();
                    transaction.Commit();
                }
                catch (DbUpdateConcurrencyException)
                {
                    transaction.Rollback();
                    throw;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void EliminarLogico(int contactId, int eliminadoPor)
        {
            using (var context = new CrmDbContext())
            {
                var contact = context.Contacts.Find(contactId);
                if (contact == null) return;

                contact.IsDeleted = true;
                contact.DeletedDate = DateTime.UtcNow;
                contact.DeletedBy = eliminadoPor;

                context.SaveChanges();
            }
        }

        private void DesmarcarPrincipaisExistentes(CrmDbContext context, int clientId, int? ignorarContactId)
        {
            var query = context.Contacts.Where(c =>
                c.ClientId == clientId &&
                c.IsPrimary &&
                !c.IsDeleted);

            if (ignorarContactId.HasValue)
            {
                query = query.Where(c => c.ContactId != ignorarContactId.Value);
            }

            foreach (var existente in query.ToList())
            {
                existente.IsPrimary = false;
            }

            context.SaveChanges();
        }
    }
}