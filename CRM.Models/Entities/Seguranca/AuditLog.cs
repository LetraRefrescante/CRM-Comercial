using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Models.Entities.Seguranca
{
    [Table("AuditLogs")]
    public class AuditLog
    {
        [Key]
        public long AuditLogId { get; set; }

        public int? UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Action { get; set; } // Create, Update, Delete, Login, LoginFailed, Logout, PasswordReset...

        [MaxLength(100)]
        public string EntityName { get; set; }

        [MaxLength(50)]
        public string EntityId { get; set; }

        public string Details { get; set; } // JSON com valores antigos/novos

        [MaxLength(45)]
        public string IpAddress { get; set; }

        public DateTime CreatedDate { get; set; }

        // Navegação
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}