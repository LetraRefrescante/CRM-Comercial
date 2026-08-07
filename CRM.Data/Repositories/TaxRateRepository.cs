using System.Collections.Generic;
using System.Linq;
using CRM.Data.Context;
using CRM.Models.Entities.Catalogo;

namespace CRM.Data.Repositories
{
    public class TaxRateRepository
    {
        public List<TaxRate> ListarAtivas()
        {
            using (var context = new CrmDbContext())
            {
                return context.TaxRates
                    .Where(t => t.IsActive)
                    .OrderBy(t => t.Percentage)
                    .ToList();
            }
        }
    }
}