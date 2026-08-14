using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CRM.Data.Context;
using CRM.Models.Entities.Atividades;
using CRM.Models.Filtros;

namespace CRM.Data.Repositories
{
    public class TaskRepository
    {
        public TaskItem ObterPorId(int taskId)
        {
            using (var context = new CrmDbContext())
            {
                return context.Tasks
                    .Include(t => t.AssignedTo)
                    .Include(t => t.RelatedClient)
                    .Include(t => t.RelatedLead)
                    .FirstOrDefault(t => t.TaskId == taskId && !t.IsDeleted);
            }
        }

        public List<TaskItem> ListarPorLead(int leadId)
        {
            using (var context = new CrmDbContext())
            {
                return context.Tasks
                    .Include(t => t.AssignedTo)
                    .Where(t => t.RelatedLeadId == leadId && !t.IsDeleted)
                    .OrderBy(t => t.DueDate)
                    .ToList();
            }
        }

        public List<TaskItem> ListarVencidas()
        {
            using (var context = new CrmDbContext())
            {
                var agora = DateTime.UtcNow;
                return context.Tasks
                    .Where(t => !t.IsDeleted
                        && t.Status != "Concluída"
                        && t.Status != "Cancelada"
                        && t.DueDate < agora)
                    .OrderBy(t => t.DueDate)
                    .ToList();
            }
        }

        // Usado por TarefasLista.aspx: filtros + paginação + ordenação
        public List<TaskItem> Pesquisar(
            TaskFiltro filtro,
            int pagina, int tamanhoPagina, out int totalRegistos,
            string sortColumn, bool sortAscending)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.Tasks
                    .Include(t => t.AssignedTo)
                    .Include(t => t.RelatedClient)
                    .Include(t => t.RelatedLead)
                    .Where(t => !t.IsDeleted);

                if (!string.IsNullOrWhiteSpace(filtro.Pesquisa))
                    query = query.Where(t => t.Subject.Contains(filtro.Pesquisa));

                if (!string.IsNullOrEmpty(filtro.Status))
                    query = query.Where(t => t.Status == filtro.Status);

                if (filtro.AssignedToUserId.HasValue)
                    query = query.Where(t => t.AssignedToUserId == filtro.AssignedToUserId.Value);

                if (filtro.DataInicio.HasValue)
                    query = query.Where(t => t.DueDate >= filtro.DataInicio.Value);

                if (filtro.DataFim.HasValue)
                    query = query.Where(t => t.DueDate <= filtro.DataFim.Value);

                if (filtro.RelatedClientId.HasValue)
                    query = query.Where(t => t.RelatedClientId == filtro.RelatedClientId.Value);

                if (filtro.RelatedLeadId.HasValue)
                    query = query.Where(t => t.RelatedLeadId == filtro.RelatedLeadId.Value);

                if (filtro.RelatedOpportunityId.HasValue)
                    query = query.Where(t => t.RelatedOpportunityId == filtro.RelatedOpportunityId.Value);

                totalRegistos = query.Count();

                query = AplicarOrdenacao(query, sortColumn, sortAscending);

                return query
                    .Skip((pagina - 1) * tamanhoPagina)
                    .Take(tamanhoPagina)
                    .ToList();
            }
        }

        private IQueryable<TaskItem> AplicarOrdenacao(IQueryable<TaskItem> query, string sortColumn, bool sortAscending)
        {
            switch (sortColumn)
            {
                case "Subject":
                    return sortAscending ? query.OrderBy(t => t.Subject) : query.OrderByDescending(t => t.Subject);
                case "Status":
                    return sortAscending ? query.OrderBy(t => t.Status) : query.OrderByDescending(t => t.Status);
                case "AssignedTo":
                    return sortAscending ? query.OrderBy(t => t.AssignedTo.Name) : query.OrderByDescending(t => t.AssignedTo.Name);
                case "DueDate":
                default:
                    return sortAscending ? query.OrderBy(t => t.DueDate) : query.OrderByDescending(t => t.DueDate);
            }
        }

        public int Criar(TaskItem task)
        {
            using (var context = new CrmDbContext())
            {
                task.CreatedDate = DateTime.UtcNow;
                context.Tasks.Add(task);
                context.SaveChanges();
                return task.TaskId;
            }
        }

        public void Atualizar(TaskItem task)
        {
            using (var context = new CrmDbContext())
            {
                task.UpdatedDate = DateTime.UtcNow;
                context.Entry(task).State = EntityState.Modified;

                context.Entry(task).Property(t => t.CreatedDate).IsModified = false;
                context.Entry(task).Property(t => t.CreatedBy).IsModified = false;
                context.Entry(task).Property(t => t.IsDeleted).IsModified = false;
                context.Entry(task).Property(t => t.DeletedDate).IsModified = false;
                context.Entry(task).Property(t => t.DeletedBy).IsModified = false;

                context.SaveChanges();
            }
        }

        public void Eliminar(int taskId, int userId)
        {
            using (var context = new CrmDbContext())
            {
                var task = context.Tasks.Find(taskId);
                if (task == null) return;

                task.IsDeleted = true;
                task.DeletedDate = DateTime.UtcNow;
                task.DeletedBy = userId;
                context.SaveChanges();
            }
        }
    }
}