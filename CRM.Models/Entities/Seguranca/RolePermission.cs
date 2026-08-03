using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Models.Entities.Seguranca
{
    [Table("RolePermissions")]
    public class RolePermission
    {
        public int RoleId { get; set; }
        public int PermissionId { get; set; }

        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }

        [ForeignKey("PermissionId")]
        public virtual Permission Permission { get; set; }
    }
}