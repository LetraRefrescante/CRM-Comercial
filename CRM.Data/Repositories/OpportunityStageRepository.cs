using System.Collections.Generic;
using System.Linq;
using CRM.Data.Context;
using CRM.Models.Entities.ListasAuxiliares;

namespace CRM.Data.Repositories
{
    public class OpportunityStageRepository
    {
        public List<OpportunityStage> ListarAtivasParaAbertura()
        {
            using (var context = new CrmDbContext())
            {
                return context.OpportunityStages
                    .Where(s => s.IsActive && !s.IsClosedWon && !s.IsClosedLost)
                    .OrderBy(s => s.OrderIndex)
                    .ToList();
            }
        }
        public List<OpportunityStage> ListarAtivas()
        {
            using (var context = new CrmDbContext())
            {
                return context.OpportunityStages
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.OrderIndex)
                    .ToList();
            }
        }
        public OpportunityStage ObterPorId(int stageId)
        {
            using (var context = new CrmDbContext())
            {
                return context.OpportunityStages.FirstOrDefault(s => s.StageId == stageId);
            }
        }

        public OpportunityStage ObterFaseFechamento(bool isClosedWon)
        {
            using (var context = new CrmDbContext())
            {
                return isClosedWon
                    ? context.OpportunityStages.FirstOrDefault(s => s.IsClosedWon)
                    : context.OpportunityStages.FirstOrDefault(s => s.IsClosedLost);
            }
        }
    }
}