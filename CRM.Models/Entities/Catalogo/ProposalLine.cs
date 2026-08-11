using System.ComponentModel.DataAnnotations.Schema;
using CRM.Models.Entities.ListasAuxiliares;

namespace CRM.Models.Entities.Catalogo
{
    [Table("ProposalLines")]
    public class ProposalLine
    {
        public int ProposalLineId { get; set; }

        public int ProposalId { get; set; }
        public Proposal Proposal { get; set; }

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