using System.ComponentModel.DataAnnotations.Schema;
using CRM.Models.Entities.Catalogo;

namespace CRM.Models.Entities.Vendas
{
    [Table("SaleLines")]
    public class SaleLine
    {
        public int SaleLineId { get; set; }

        public int SaleId { get; set; }
        public Sale Sale { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int LineOrder { get; set; }
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountPercent { get; set; }

        public int TaxRateId { get; set; }
        public TaxRate TaxRate { get; set; }

        public decimal LineTotal { get; set; }
    }
}