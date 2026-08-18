using System.Collections.Generic;
using System.Linq;
using CRM.Data.Context;
using CRM.Models.Entities.ListasAuxiliares;

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
        // LossReasonRepository.cs — LossReason.cs (esse já vi) NÃO tem CreatedDate/By/UpdatedDate/By,
        // por isso Criar/Atualizar aqui são mais simples que os outros três.
        public LossReason GetById(int lossReasonId)
        {
            using (var context = new CrmDbContext())
                return context.LossReasons.Find(lossReasonId);
        }

        public List<LossReason> Listar(string pesquisa)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.LossReasons.AsQueryable();
                if (!string.IsNullOrWhiteSpace(pesquisa))
                    query = query.Where(r => r.Name.Contains(pesquisa));
                return query.OrderBy(r => r.Name).ToList();
            }
        }

        public bool ExisteNome(string name, int? ignorarLossReasonId = null)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.LossReasons.Where(r => r.Name == name);
                if (ignorarLossReasonId.HasValue) query = query.Where(r => r.LossReasonId != ignorarLossReasonId.Value);
                return query.Any();
            }
        }

        public int Criar(LossReason lossReason)
        {
            using (var context = new CrmDbContext())
            {
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
                context.SaveChanges();
            }
        }

        public void AlternarEstado(int lossReasonId)
        {
            using (var context = new CrmDbContext())
            {
                var lossReason = context.LossReasons.Find(lossReasonId);
                if (lossReason == null) return;
                lossReason.IsActive = !lossReason.IsActive;
                context.SaveChanges();
            }
        }
    }
}