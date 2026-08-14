using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using CRM.Data.Context;
using CRM.Models.Entities.Leads;

namespace CRM.Data.Repositories
{
    public class LeadRepository
    {
        public Lead GetById(int leadId)
        {
            using (var context = new CrmDbContext())
            {
                return context.Leads
                    .Include(l => l.LeadSource)
                    .Include(l => l.Owner)
                    .Include(l => l.LossReason)
                    .Where(l => l.LeadId == leadId && !l.IsDeleted)
                    .SingleOrDefault();
            }
        }
        public List<Lead> ListarParaSelecao()
        {
            using (var context = new CrmDbContext())
            {
                return context.Leads
                    .Where(l => !l.IsDeleted && l.Status != "Convertido")
                    .OrderBy(l => l.Name)
                    .ToList();
            }
        }

        public List<Lead> Listar(
            string pesquisa,
            string status,
            int? leadSourceId,
            int? ownerId,
            int? scoreMin,
            int? scoreMax,
            DateTime? dataInicio,
            DateTime? dataFim,
            int pagina,
            int tamanhoPagina,
            out int totalRegistos,
            string sortColumn = "CreatedDate",
            bool sortAscending = false)
        {
            using (var context = new CrmDbContext())
            {
                var query = ConstruirQuery(context, pesquisa, status, leadSourceId, ownerId, scoreMin, scoreMax, dataInicio, dataFim);

                totalRegistos = query.Count();

                return AplicarOrdenacao(query, sortColumn, sortAscending)
                    .Skip((pagina - 1) * tamanhoPagina)
                    .Take(tamanhoPagina)
                    .ToList();
            }
        }

        private IQueryable<Lead> ConstruirQuery(
            CrmDbContext context,
            string pesquisa,
            string status,
            int? leadSourceId,
            int? ownerId,
            int? scoreMin,
            int? scoreMax,
            DateTime? dataInicio,
            DateTime? dataFim)
        {
            var query = context.Leads
                .Include(l => l.LeadSource)
                .Include(l => l.Owner)
                .Where(l => !l.IsDeleted);

            if (!string.IsNullOrWhiteSpace(pesquisa))
            {
                query = query.Where(l =>
                    l.Name.Contains(pesquisa) ||
                    (l.CompanyName != null && l.CompanyName.Contains(pesquisa)) ||
                    (l.Email != null && l.Email.Contains(pesquisa)) ||
                    (l.Phone != null && l.Phone.Contains(pesquisa)));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(l => l.Status == status);
            }

            if (leadSourceId.HasValue)
            {
                query = query.Where(l => l.LeadSourceId == leadSourceId.Value);
            }

            if (ownerId.HasValue)
            {
                query = query.Where(l => l.OwnerId == ownerId.Value);
            }

            if (scoreMin.HasValue)
            {
                query = query.Where(l => l.Score != null && l.Score >= scoreMin.Value);
            }

            if (scoreMax.HasValue)
            {
                query = query.Where(l => l.Score != null && l.Score <= scoreMax.Value);
            }

            if (dataInicio.HasValue)
            {
                query = query.Where(l => l.CreatedDate >= dataInicio.Value);
            }

            if (dataFim.HasValue)
            {
                // Inclui o dia inteiro da data final selecionada.
                var fimDoDia = dataFim.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(l => l.CreatedDate <= fimDoDia);
            }

            return query;
        }

        // Whitelist explícita de propósito: nunca aceites o nome da coluna
        // diretamente num OrderBy dinâmico por string.
        private IQueryable<Lead> AplicarOrdenacao(IQueryable<Lead> query, string sortColumn, bool sortAscending)
        {
            switch (sortColumn)
            {
                case "CompanyName":
                    return sortAscending ? query.OrderBy(l => l.CompanyName) : query.OrderByDescending(l => l.CompanyName);
                case "LeadSource":
                    return sortAscending ? query.OrderBy(l => l.LeadSource.Name) : query.OrderByDescending(l => l.LeadSource.Name);
                case "Status":
                    return sortAscending ? query.OrderBy(l => l.Status) : query.OrderByDescending(l => l.Status);
                case "Score":
                    return sortAscending ? query.OrderBy(l => l.Score) : query.OrderByDescending(l => l.Score);
                case "Owner":
                    return sortAscending ? query.OrderBy(l => l.Owner.Name) : query.OrderByDescending(l => l.Owner.Name);
                case "NextContactDate":
                    return sortAscending ? query.OrderBy(l => l.NextContactDate) : query.OrderByDescending(l => l.NextContactDate);
                case "Name":
                    return sortAscending ? query.OrderBy(l => l.Name) : query.OrderByDescending(l => l.Name);
                case "CreatedDate":
                default:
                    return sortAscending ? query.OrderBy(l => l.CreatedDate) : query.OrderByDescending(l => l.CreatedDate);
            }
        }

        /// <summary>
        /// Blueprint: "Evitar duplicados por email, telefone ou NIF, apresentando avisos."
        /// A tabela Leads não tem NIF (só existe em Clients), por isso este método só
        /// cobre email/telefone. Devolve leads não eliminados e não convertidos —
        /// é um AVISO, não bloqueia a gravação.
        /// </summary>
        public List<Lead> ProcurarPossiveisDuplicados(string email, string phone, int? ignorarLeadId = null)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.Leads.Where(l => !l.IsDeleted && l.Status != "Convertido");

                query = query.Where(l =>
                    (!string.IsNullOrEmpty(email) && l.Email == email) ||
                    (!string.IsNullOrEmpty(phone) && l.Phone == phone));

                if (ignorarLeadId.HasValue)
                {
                    query = query.Where(l => l.LeadId != ignorarLeadId.Value);
                }

                return query.ToList();
            }
        }

        public int Criar(Lead lead)
        {
            using (var context = new CrmDbContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    lead.CreatedDate = DateTime.UtcNow;
                    context.Leads.Add(lead);
                    context.SaveChanges();

                    context.LeadStatusHistories.Add(new LeadStatusHistory
                    {
                        LeadId = lead.LeadId,
                        PreviousStatus = null,
                        NewStatus = lead.Status,
                        ChangedDate = DateTime.UtcNow,
                        ChangedBy = lead.CreatedBy
                    });
                    context.SaveChanges();

                    transaction.Commit();
                    return lead.LeadId;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Regra: "Registar todas as alterações de estado" — grava em LeadStatusHistory
        /// sempre que o Status muda, na mesma transação da atualização dos restantes campos.
        /// </summary>
        public void Atualizar(Lead leadAtualizado, int alteradoPor)
        {
            using (var context = new CrmDbContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var lead = context.Leads.Find(leadAtualizado.LeadId);
                    if (lead == null) return;

                    string statusAnterior = lead.Status;

                    lead.Name = leadAtualizado.Name;
                    lead.CompanyName = leadAtualizado.CompanyName;
                    lead.Email = leadAtualizado.Email;
                    lead.Phone = leadAtualizado.Phone;
                    lead.LeadSourceId = leadAtualizado.LeadSourceId;
                    lead.Status = leadAtualizado.Status;
                    lead.Score = leadAtualizado.Score;
                    lead.OwnerId = leadAtualizado.OwnerId;
                    lead.NextContactDate = leadAtualizado.NextContactDate;
                    lead.LossReasonId = leadAtualizado.LossReasonId;
                    lead.UpdatedDate = DateTime.UtcNow;
                    lead.UpdatedBy = leadAtualizado.UpdatedBy;

                    context.Entry(lead).OriginalValues["RowVersion"] = leadAtualizado.RowVersion;

                    context.SaveChanges();

                    if (statusAnterior != lead.Status)
                    {
                        context.LeadStatusHistories.Add(new LeadStatusHistory
                        {
                            LeadId = lead.LeadId,
                            PreviousStatus = statusAnterior,
                            NewStatus = lead.Status,
                            ChangedDate = DateTime.UtcNow,
                            ChangedBy = alteradoPor
                        });
                        context.SaveChanges();
                    }

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

        public void EliminarLogico(int leadId, int eliminadoPor)
        {
            using (var context = new CrmDbContext())
            {
                var lead = context.Leads.Find(leadId);
                if (lead == null) return;

                lead.IsDeleted = true;
                lead.DeletedDate = DateTime.UtcNow;
                lead.DeletedBy = eliminadoPor;

                context.SaveChanges();
            }
        }

        public List<LeadStatusHistory> ListarHistoricoEstados(int leadId)
        {
            using (var context = new CrmDbContext())
            {
                return context.LeadStatusHistories
                    .Where(h => h.LeadId == leadId)
                    .OrderByDescending(h => h.ChangedDate)
                    .ToList();
            }
        }
    }
}