using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Models.Entities.Clientes
{
    [Table("Contacts")]
    public class Contact
    {
        [Key]
        public int ContactId { get; set; }

        [Required]
        public int ClientId { get; set; }

        [Required]
        [MaxLength(120)]
        public string Name { get; set; }

        [MaxLength(100)]
        [Column("Position")]
        public string JobTitle { get; set; }

        [MaxLength(100)]
        public string Department { get; set; }

        [MaxLength(150)]
        public string Email { get; set; }

        [MaxLength(30)]
        public string Phone { get; set; }

        [MaxLength(30)]
        [Column("Mobile")]
        public string MobilePhone { get; set; }

        public DateTime? BirthDate { get; set; }

        public bool IsPrimary { get; set; }

        [MaxLength(20)]
        public string ContactPreference { get; set; }

        public bool ConsentGiven { get; set; }

        [MaxLength(500)]
        public string ContactRestrictions { get; set; }

        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public int? DeletedBy { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }
    }
}