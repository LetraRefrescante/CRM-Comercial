using System;
using System.Collections.Generic;
using System.Linq;
using CRM.Data.Context;
using CRM.Models.Entities.Seguranca;

namespace CRM.Data.Repositories
{
    public class RolePermissionRepository
    {
        public HashSet<int> ObterPermissoesDoRole(int roleId)
        {
            using (var context = new CrmDbContext())
            {
                return new HashSet<int>(
                    context.RolePermissions
                        .Where(rp => rp.RoleId == roleId)
                        .Select(rp => rp.PermissionId)
                );
            }
        }

        public HashSet<string> ObterCodigosDoRole(int roleId)
        {
            using (var context = new CrmDbContext())
            {
                return new HashSet<string>(
                    context.RolePermissions
                        .Where(rp => rp.RoleId == roleId)
                        .Select(rp => rp.Permission.Code)
                );
            }
        }

        public void AtualizarPermissoesDoRole(int roleId, List<int> permissionIds, int alteradoPor)
        {
            using (var context = new CrmDbContext())
            {
                var atuais = context.RolePermissions.Where(rp => rp.RoleId == roleId).ToList();
                context.RolePermissions.RemoveRange(atuais);

                foreach (var permissionId in permissionIds)
                {
                    context.RolePermissions.Add(new RolePermission
                    {
                        RoleId = roleId,
                        PermissionId = permissionId,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = alteradoPor
                    });
                }

                context.SaveChanges();
            }
        }
    }
}