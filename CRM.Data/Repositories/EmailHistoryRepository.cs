using System;
using System.Linq;
using System.Collections.Generic;
using CRM.Data.Context;
using CRM.Models.Entities.Notificacoes;
using CRM.Models.Filtros;

namespace CRM.Data.Repositories
{
    public class EmailHistoryRepository
    {
        public List<EmailHistory> Pesquisar(
            EmailHistoryFiltro filtro,
            int pagina, int tamanhoPagina, out int totalRegistos,
            string sortColumn, bool sortAscending)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.EmailHistories.AsQueryable();

                if (!string.IsNullOrWhiteSpace(filtro.Pesquisa))
                    query = query.Where(h => h.ToAddress.Contains(filtro.Pesquisa) || h.Subject.Contains(filtro.Pesquisa));

                if (!string.IsNullOrEmpty(filtro.Status))
                    query = query.Where(h => h.Status == filtro.Status);

                if (filtro.DataInicio.HasValue)
                    query = query.Where(h => h.SentDate >= filtro.DataInicio.Value);

                if (filtro.DataFim.HasValue)
                    query = query.Where(h => h.SentDate <= filtro.DataFim.Value);

                totalRegistos = query.Count();

                query = sortColumn == "ToAddress"
                    ? (sortAscending ? query.OrderBy(h => h.ToAddress) : query.OrderByDescending(h => h.ToAddress))
                    : (sortAscending ? query.OrderBy(h => h.SentDate) : query.OrderByDescending(h => h.SentDate));

                return query.Skip((pagina - 1) * tamanhoPagina).Take(tamanhoPagina).ToList();
            }
        }

        public int Criar(EmailHistory history)
        {
            using (var context = new CrmDbContext())
            {
                history.SentDate = DateTime.UtcNow;
                context.EmailHistories.Add(history);
                context.SaveChanges();
                return history.EmailHistoryId;
            }
        }
    }
}