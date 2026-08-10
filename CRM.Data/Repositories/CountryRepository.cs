using CRM.Data.Context;
using CRM.Models.Entities.ListasAuxiliares;
using System.Collections.Generic;
using System.Linq;

namespace CRM.Data.Repositories
{
    public class CountryRepository
    {
        public bool EhPortugal(int countryId)
        {
            using (var context = new CrmDbContext())
            {
                var country = context.Countries.Find(countryId);
                return country != null && country.IsoCode == "PT";
            }
        }
        public List<Country> ListarAtivos()
        {
            using (var context = new CrmDbContext())
            {
                return context.Countries.Where(c => c.IsActive).OrderBy(c => c.Name).ToList();
            }
        }
    }
}