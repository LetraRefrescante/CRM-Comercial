using System.Collections.Generic;
using System.Linq;
using CRM.Data.Context;
using CRM.Models.Entities.Seguranca;

namespace CRM.Data.Repositories
{
    public class RoleRepository
    {
        public List<Role> Listar()
        {
            using (var context = new CrmDbContext())
            {
                return context.Roles
                    .Where(r => !r.IsDeleted)
                    .OrderBy(r => r.Name)
                    .ToList();
            }
        }

        public Role GetById(int roleId)
        {
            using (var context = new CrmDbContext())
            {
                return context.Roles
                    .Where(r => r.RoleId == roleId && !r.IsDeleted)
                    .SingleOrDefault();
            }
        }
    }
}