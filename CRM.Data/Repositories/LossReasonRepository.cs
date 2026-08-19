using CRM.Data.Context;
using CRM.Models.Entities.ListasAuxiliares;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CRM.Data.Repositories
{
    public class LossReasonRepository
    {
        public List<LossReason> ListarAtivos()
        {
            using (var context = new CrmDbContext())
            {
                return context.LossReasons
                    .Where(r => r.IsActive)
                    .OrderBy(r => r.Name)
                    .ToList();
            }
        }

        public LossReason GetById(int lossReasonId)
        {
            using (var context = new CrmDbContext())
                return context.LossReasons.Find(lossReasonId);
        }
        public List<LossReason> Listar(string pesquisa, bool incluirInativos)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.LossReasons.AsQueryable();

                if (!incluirInativos)
                    query = query.Where(l => l.IsActive);

                if (!string.IsNullOrWhiteSpace(pesquisa))
                    query = query.Where(l => l.Name.Contains(pesquisa));

                return query.OrderBy(l => l.Name).ToList();
            }
        }

        public bool ExisteNome(string name, int? ignorarLossReasonId = null)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.LossReasons.Where(l => l.Name == name);
                if (ignorarLossReasonId.HasValue) query = query.Where(l => l.LossReasonId != ignorarLossReasonId.Value);
                return query.Any();
            }
        }

        public int Criar(LossReason lossReason)
        {
            using (var context = new CrmDbContext())
            {
                lossReason.CreatedDate = DateTime.UtcNow;
                lossReason.IsActive = true;
                context.LossReasons.Add(lossReason);
                context.SaveChanges();
                return lossReason.LossReasonId;
            }
        }
        public void Atualizar(LossReason lossReasonAtualizado)
        {
            using (var context = new CrmDbContext())
            {
                var lossReason = context.LossReasons.Find(lossReasonAtualizado.LossReasonId);
                if (lossReason == null) return;

                lossReason.Name = lossReasonAtualizado.Name;
                lossReason.UpdatedDate = DateTime.UtcNow;
                lossReason.UpdatedBy = lossReasonAtualizado.UpdatedBy;

                context.SaveChanges();
            }
        }

        public void AlternarEstado(int lossReasonId, int alteradoPor)
        {
            using (var context = new CrmDbContext())
            {
                var lossReason = context.LossReasons.Find(lossReasonId);
                if (lossReason == null) return;

                lossReason.IsActive = !lossReason.IsActive;
                lossReason.UpdatedDate = DateTime.UtcNow;
                lossReason.UpdatedBy = alteradoPor;
                context.SaveChanges();
            }
        }
    }
}