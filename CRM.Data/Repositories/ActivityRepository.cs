using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CRM.Data.Context;
using CRM.Models.Entities.Atividades;
using CRM.Models.Filtros;

namespace CRM.Data.Repositories
{
    public class ActivityRepository
    {
        public Activity ObterPorId(int activityId)
        {
            using (var context = new CrmDbContext())
            {
                return context.Activities
                    .Include(a => a.AssignedTo)
                    .Include(a => a.RelatedClient)
                    .Include(a => a.RelatedLead)
                    .FirstOrDefault(a => a.ActivityId == activityId && !a.IsDeleted);
            }
        }

        public List<Activity> ListarPorLead(int leadId)
        {
            using (var context = new CrmDbContext())
            {
                return context.Activities
                    .Include(a => a.AssignedTo)
                    .Where(a => a.RelatedLeadId == leadId && !a.IsDeleted)
                    .OrderByDescending(a => a.StartDateTime)
                    .ToList();
            }
        }

        // Usado por AtividadesLista.aspx: filtros + paginação + ordenação
        public List<Activity> Pesquisar(
            ActivityFiltro filtro,
            int pagina, int tamanhoPagina, out int totalRegistos,
            string sortColumn, bool sortAscending)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.Activities
                    .Include(a => a.AssignedTo)
                    .Include(a => a.RelatedClient)
                    .Include(a => a.RelatedLead)
                    .Where(a => !a.IsDeleted);

                if (!string.IsNullOrWhiteSpace(filtro.Pesquisa))
                    query = query.Where(a => a.Subject.Contains(filtro.Pesquisa));

                if (!string.IsNullOrEmpty(filtro.Tipo))
                    query = query.Where(a => a.Type == filtro.Tipo);

                if (!string.IsNullOrEmpty(filtro.Status))
                    query = query.Where(a => a.Status == filtro.Status);

                if (filtro.AssignedToUserId.HasValue)
                    query = query.Where(a => a.AssignedToUserId == filtro.AssignedToUserId.Value);

                if (filtro.DataInicio.HasValue)
                    query = query.Where(a => a.StartDateTime >= filtro.DataInicio.Value);

                if (filtro.DataFim.HasValue)
                    query = query.Where(a => a.StartDateTime <= filtro.DataFim.Value);

                if (filtro.RelatedClientId.HasValue)
                    query = query.Where(a => a.RelatedClientId == filtro.RelatedClientId.Value);

                if (filtro.RelatedLeadId.HasValue)
                    query = query.Where(a => a.RelatedLeadId == filtro.RelatedLeadId.Value);

                if (filtro.RelatedOpportunityId.HasValue)
                    query = query.Where(a => a.RelatedOpportunityId == filtro.RelatedOpportunityId.Value);

                totalRegistos = query.Count();

                query = AplicarOrdenacao(query, sortColumn, sortAscending);

                return query
                    .Skip((pagina - 1) * tamanhoPagina)
                    .Take(tamanhoPagina)
                    .ToList();
            }
        }

        public List<Activity> ListarPorPeriodo(DateTime inicio, DateTime fim, ActivityFiltro filtro)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.Activities
                    .Include(a => a.AssignedTo)
                    .Include(a => a.RelatedClient)
                    .Include(a => a.RelatedLead)
                    .Where(a => !a.IsDeleted && a.StartDateTime >= inicio && a.StartDateTime < fim);

                if (!string.IsNullOrEmpty(filtro?.Tipo))
                    query = query.Where(a => a.Type == filtro.Tipo);

                if (!string.IsNullOrEmpty(filtro?.Status))
                    query = query.Where(a => a.Status == filtro.Status);

                if (filtro?.AssignedToUserId != null)
                    query = query.Where(a => a.AssignedToUserId == filtro.AssignedToUserId.Value);

                return query.OrderBy(a => a.StartDateTime).ToList();
            }
        }

        public List<Activity> ListarPorOportunidade(int opportunityId)
        {
            using (var context = new CrmDbContext())
            {
                return context.Activities
                    .Include(a => a.AssignedTo)
                    .Where(a => a.RelatedOpportunityId == opportunityId && !a.IsDeleted)
                    .OrderByDescending(a => a.StartDateTime)
                    .ToList();
            }
        }

        private IQueryable<Activity> AplicarOrdenacao(IQueryable<Activity> query, string sortColumn, bool sortAscending)
        {
            switch (sortColumn)
            {
                case "Subject":
                    return sortAscending ? query.OrderBy(a => a.Subject) : query.OrderByDescending(a => a.Subject);
                case "Type":
                    return sortAscending ? query.OrderBy(a => a.Type) : query.OrderByDescending(a => a.Type);
                case "Status":
                    return sortAscending ? query.OrderBy(a => a.Status) : query.OrderByDescending(a => a.Status);
                case "AssignedTo":
                    return sortAscending ? query.OrderBy(a => a.AssignedTo.Name) : query.OrderByDescending(a => a.AssignedTo.Name);
                case "StartDateTime":
                default:
                    return sortAscending ? query.OrderBy(a => a.StartDateTime) : query.OrderByDescending(a => a.StartDateTime);
            }
        }

        public int Criar(Activity activity)
        {
            using (var context = new CrmDbContext())
            {
                activity.CreatedDate = DateTime.UtcNow;
                context.Activities.Add(activity);
                context.SaveChanges();
                return activity.ActivityId;
            }
        }

        public void Atualizar(Activity activity)
        {
            using (var context = new CrmDbContext())
            {
                activity.UpdatedDate = DateTime.UtcNow;
                context.Entry(activity).State = EntityState.Modified;

                context.Entry(activity).Property(a => a.CreatedDate).IsModified = false;
                context.Entry(activity).Property(a => a.CreatedBy).IsModified = false;
                context.Entry(activity).Property(a => a.IsDeleted).IsModified = false;
                context.Entry(activity).Property(a => a.DeletedDate).IsModified = false;
                context.Entry(activity).Property(a => a.DeletedBy).IsModified = false;

                context.SaveChanges();
            }
        }

        public void Eliminar(int activityId, int userId)
        {
            using (var context = new CrmDbContext())
            {
                var activity = context.Activities.Find(activityId);
                if (activity == null) return;

                activity.IsDeleted = true;
                activity.DeletedDate = DateTime.UtcNow;
                activity.DeletedBy = userId;
                context.SaveChanges();
            }
        }
    }
}