using System.Collections.Generic;
using System.Linq;
using CRM.Data.Context;
using CRM.Models.Entities.ListasAuxiliares;

namespace CRM.Data.Repositories
{
    public class PaymentTermRepository
    {
        public List<PaymentTerm> ListarAtivas()
        {
            using (var context = new CrmDbContext())
            {
                return context.PaymentTerms
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.DaysDue)
                    .ToList();
            }
        }
    }
}