using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Models.Entities
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(120)]
        public string Name { get; set; }

        [Required]
        [MaxLength(150)]
        public string Email { get; set; }

        [Required]
        [MaxLength(256)]
        public string PasswordHash { get; set; }

        [Required]
        [MaxLength(256)]
        public string PasswordSalt { get; set; }

        public int RoleId { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Ativo"; // Ativo, Bloqueado, Inativo

        public int FailedLoginAttempts { get; set; }
        public DateTime? LockedUntil { get; set; }
        public DateTime? LastLoginDate { get; set; }

        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public int? DeletedBy { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        // Navegação
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }

        public virtual ICollection<AuditLog> AuditLogs { get; set; }

        public User()
        {
            AuditLogs = new HashSet<AuditLog>();
        }
    }
}