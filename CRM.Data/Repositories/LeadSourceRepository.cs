using CRM.Data.Context;
using CRM.Models.Entities.ListasAuxiliares;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CRM.Data.Repositories
{
    public class LeadSourceRepository
    {
        public List<LeadSource> ListarAtivos()
        {
            using (var context = new CrmDbContext())
            {
                return context.LeadSources
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.Name)
                    .ToList();
            }
        }
        public LeadSource GetById(int leadSourceId)
        {
            using (var context = new CrmDbContext())
                return context.LeadSources.Find(leadSourceId);
        }

        public List<LeadSource> Listar(string pesquisa)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.LeadSources.AsQueryable();
                if (!string.IsNullOrWhiteSpace(pesquisa))
                    query = query.Where(s => s.Name.Contains(pesquisa));
                return query.OrderBy(s => s.Name).ToList();
            }
        }

        public bool ExisteNome(string name, int? ignorarLeadSourceId = null)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.LeadSources.Where(s => s.Name == name);
                if (ignorarLeadSourceId.HasValue) query = query.Where(s => s.LeadSourceId != ignorarLeadSourceId.Value);
                return query.Any();
            }
        }

        public int Criar(LeadSource leadSource)
        {
            using (var context = new CrmDbContext())
            {
                leadSource.CreatedDate = DateTime.UtcNow;
                leadSource.IsActive = true;
                context.LeadSources.Add(leadSource);
                context.SaveChanges();
                return leadSource.LeadSourceId;
            }
        }

        public void Atualizar(LeadSource leadSourceAtualizada)
        {
            using (var context = new CrmDbContext())
            {
                var leadSource = context.LeadSources.Find(leadSourceAtualizada.LeadSourceId);
                if (leadSource == null) return;
                leadSource.Name = leadSourceAtualizada.Name;
                leadSource.UpdatedDate = DateTime.UtcNow;
                leadSource.UpdatedBy = leadSourceAtualizada.UpdatedBy;
                context.SaveChanges();
            }
        }

        public void AlternarEstado(int leadSourceId, int alteradoPor)
        {
            using (var context = new CrmDbContext())
            {
                var leadSource = context.LeadSources.Find(leadSourceId);
                if (leadSource == null) return;
                leadSource.IsActive = !leadSource.IsActive;
                leadSource.UpdatedDate = DateTime.UtcNow;
                leadSource.UpdatedBy = alteradoPor;
                context.SaveChanges();
            }
        }
    }
}