using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CRM.Data.Context;
using CRM.Models.Entities.Seguranca;

namespace CRM.Data.Repositories
{
    public class AuditLogRepository
    {
        public void Criar(AuditLog log)
        {
            using (var context = new CrmDbContext())
            {
                context.AuditLogs.Add(log);
                context.SaveChanges();
            }
        }

        public List<AuditLog> Listar(
            int? userId,
            string action,
            string entityName,
            DateTime? dataInicial,
            DateTime? dataFinal,
            int pagina,
            int tamanhoPagina,
            out int totalRegistos)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.AuditLogs.Include(a => a.User).AsQueryable();

                if (userId.HasValue)
                    query = query.Where(a => a.UserId == userId.Value);

                if (!string.IsNullOrWhiteSpace(action))
                    query = query.Where(a => a.Action == action);

                if (!string.IsNullOrWhiteSpace(entityName))
                    query = query.Where(a => a.EntityName == entityName);

                if (dataInicial.HasValue)
                    query = query.Where(a => a.CreatedDate >= dataInicial.Value);

                if (dataFinal.HasValue)
                    query = query.Where(a => a.CreatedDate < dataFinal.Value.AddDays(1));

                totalRegistos = query.Count();

                return query
                    .OrderByDescending(a => a.CreatedDate)
                    .Skip((pagina - 1) * tamanhoPagina)
                    .Take(tamanhoPagina)
                    .ToList();
            }
        }

        public List<string> ListarAcoesDistintas()
        {
            using (var context = new CrmDbContext())
            {
                return context.AuditLogs
                    .Select(a => a.Action)
                    .Distinct()
                    .OrderBy(a => a)
                    .ToList();
            }
        }
        public List<AuditLog> ListarPorEntidade(string entityName, string entityId)
        {
            using (var context = new CrmDbContext())
            {
                return context.AuditLogs
                    .Where(a => a.EntityName == entityName && a.EntityId == entityId)
                    .OrderByDescending(a => a.CreatedDate)
                    .ToList();
            }
        }
    }
}