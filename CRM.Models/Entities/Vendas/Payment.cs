using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Models.Entities.Vendas
{
    [Table("Payments")]
    public class Payment
    {
        public int PaymentId { get; set; }

        public int SaleId { get; set; }
        public Sale Sale { get; set; }

        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string Reference { get; set; }
        public string Notes { get; set; }

        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public int? DeletedBy { get; set; }
    }
}