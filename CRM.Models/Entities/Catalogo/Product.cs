using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Models.Entities.Catalogo
{
    [Table("Products")]
    public class Product
    {
        public int ProductId { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public int TaxRateId { get; set; }
        public TaxRate TaxRate { get; set; }
        public string Unit { get; set; }
        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public int? DeletedBy { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}