using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CRM.Models.Entities.Seguranca;
using CRM.Models.Entities.ListasAuxiliares;

namespace CRM.Models.Entities.Clientes
{
    [Table("Clients")]
    public class Client
    {
        [Key]
        public int ClientId { get; set; }

        [Required]
        [MaxLength(20)]
        public string InternalCode { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("CommercialName")]
        public string TradeName { get; set; }

        [MaxLength(200)]
        public string LegalName { get; set; }

        [Required]
        [MaxLength(30)]
        public string VatNumber { get; set; }

        [MaxLength(150)]
        public string Email { get; set; }

        [MaxLength(30)]
        public string Phone { get; set; }

        [MaxLength(300)]
        public string Address { get; set; }

        [MaxLength(20)]
        public string PostalCode { get; set; }

        [MaxLength(100)]
        public string City { get; set; }

        public int CountryId { get; set; }

        public int? SectorId { get; set; }

        [Required]
        public int AccountManagerId { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Potencial";

        [MaxLength(4000)]
        public string Notes { get; set; }

        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public int? DeletedBy { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        [ForeignKey("CountryId")]
        public virtual Country Country { get; set; }

        [ForeignKey("SectorId")]
        public virtual Sector Sector { get; set; }

        [ForeignKey("AccountManagerId")]
        public virtual User AccountManager { get; set; }

        public virtual ICollection<Contact> Contacts { get; set; }

        public Client()
        {
            Contacts = new HashSet<Contact>();
        }
    }
}