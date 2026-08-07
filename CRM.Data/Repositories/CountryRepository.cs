using CRM.Data.Context;

namespace CRM.Data.Repositories
{
    public class CountryRepository
    {
        /// <summary>
        /// Determina se o país indicado é Portugal, pelo IsoCode "PT".
        /// </summary>
        public bool EhPortugal(int countryId)
        {
            using (var context = new CrmDbContext())
            {
                var country = context.Countries.Find(countryId);
                return country != null && country.IsoCode == "PT";
            }
        }
    }
}