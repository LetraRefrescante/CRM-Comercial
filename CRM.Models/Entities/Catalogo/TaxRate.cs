using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Models.Entities.Catalogo
{
    [Table("TaxRates")]
    public class TaxRate
    {
        public int TaxRateId { get; set; }
        public string Name { get; set; }
        public decimal Percentage { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
    }
}