using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CRM.Models.Entities.Clientes;

namespace CRM.Models.Entities.ListasAuxiliares
{
    [Table("Sectors")]
    public class Sector
    {
        [Key]
        public int SectorId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }

        public virtual ICollection<Client> Clients { get; set; }

        public Sector()
        {
            Clients = new HashSet<Client>();
        }
    }
}