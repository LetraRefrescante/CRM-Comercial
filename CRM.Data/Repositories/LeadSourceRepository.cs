using System;
using System.Collections.Generic;
using System.Linq;
using CRM.Data.Context;
using CRM.Models.Entities.ListasAuxiliares;

namespace CRM.Data.Repositories
{
    public class LeadSourceRepository
    {
        public LeadSource GetById(int id)
        {
            using (var context = new CrmDbContext())
            {
                return context.LeadSources.Find(id);
            }
        }
        public List<LeadSource> ListarAtivos()
        {
            using (var context = new CrmDbContext())
            {
                return context.LeadSources
                    .Where(l => l.IsActive)
                    .OrderBy(l => l.Name)
                    .ToList();
            }
        }

        public List<LeadSource> Listar(string pesquisa, bool incluirInativos)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.LeadSources.AsQueryable();

                if (!incluirInativos)
                    query = query.Where(l => l.IsActive);

                if (!string.IsNullOrWhiteSpace(pesquisa))
                    query = query.Where(l => l.Name.Contains(pesquisa));

                return query.OrderBy(l => l.Name).ToList();
            }
        }

        public List<LeadSource> Listar(bool incluirInativos) => Listar(null, incluirInativos);

        public bool ExisteNome(string name, int? ignorarLeadSourceId = null)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.LeadSources.Where(l => l.Name == name);
                if (ignorarLeadSourceId.HasValue) query = query.Where(l => l.LeadSourceId != ignorarLeadSourceId.Value);
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
        public void Atualizar(LeadSource leadSourceAtualizado)
        {
            using (var context = new CrmDbContext())
            {
                var existente = context.LeadSources.Find(leadSourceAtualizado.LeadSourceId);
                if (existente == null) return;

                existente.Name = leadSourceAtualizado.Name;
                existente.UpdatedDate = DateTime.UtcNow;
                existente.UpdatedBy = leadSourceAtualizado.UpdatedBy;

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