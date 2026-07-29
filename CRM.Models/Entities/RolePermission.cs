using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Models.Entities
{
    [Table("RolePermissions")]
    public class RolePermission
    {
        // Chave composta (RoleId, PermissionId) - configurada no OnModelCreating do DbContext
        public int RoleId { get; set; }
        public int PermissionId { get; set; }

        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }

        // Navegação
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }

        [ForeignKey("PermissionId")]
        public virtual Permission Permission { get; set; }
    }
}