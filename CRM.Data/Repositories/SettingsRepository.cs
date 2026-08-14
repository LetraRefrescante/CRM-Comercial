using System.Linq;
using CRM.Data.Context;
using CRM.Models.Entities.ListasAuxiliares;

namespace CRM.Data.Repositories
{
    public class SettingsRepository
    {
        public Settings ObterConfiguracaoAtual()
        {
            using (var context = new CrmDbContext())
            {
                return context.Settings.FirstOrDefault();
            }
        }
    }
}