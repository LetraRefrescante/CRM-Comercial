using System.Collections.Generic;
using System.Linq;
using CRM.Data.Context;
using CRM.Models.Entities.Seguranca;

namespace CRM.Data.Repositories
{
    public class PermissionRepository
    {
        public List<Permission> Listar()
        {
            using (var context = new CrmDbContext())
            {
                return context.Permissions
                    .Where(p => !p.IsDeleted)
                    .OrderBy(p => p.Module)
                    .ThenBy(p => p.Code)
                    .ToList();
            }
        }
    }
}