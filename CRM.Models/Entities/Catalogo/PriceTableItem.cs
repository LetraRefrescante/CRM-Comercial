using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Models.Entities.Catalogo
{
    [Table("PriceTableItems")]
    public class PriceTableItem
    {
        public int PriceTableItemId { get; set; }
        public int PriceTableId { get; set; }
        public PriceTable PriceTable { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public decimal Price { get; set; }

        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
    }
}