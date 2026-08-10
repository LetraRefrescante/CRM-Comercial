using System.Collections.Generic;
using System.Linq;
using CRM.Data.Context;
using CRM.Models.Entities.ListasAuxiliares;

namespace CRM.Data.Repositories
{
    public class SectorRepository
    {
        public List<Sector> ListarAtivos()
        {
            using (var context = new CrmDbContext())
            {
                return context.Sectors.Where(s => s.IsActive).OrderBy(s => s.Name).ToList();
            }
        }
    }
}