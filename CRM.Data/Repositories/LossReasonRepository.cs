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
    }
}