using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Models.Entities.Catalogo
{
    [Table("PriceTables")]
    public class PriceTable
    {
        public int PriceTableId { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
    }
}