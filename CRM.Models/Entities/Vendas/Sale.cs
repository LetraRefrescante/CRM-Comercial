using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CRM.Models.Entities.Catalogo;   // Proposal
using CRM.Models.Entities.Clientes;   // Client
using CRM.Models.Entities.Seguranca;  // User

namespace CRM.Models.Entities.Vendas
{
    [Table("Sales")]
    public class Sale
    {
        public int SaleId { get; set; }
        public string SaleNumber { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; }

        // Origem: preenchido só quando Origin == "Proposta" (008_Sales_SaleLines_Payments.sql)
        public int? ProposalId { get; set; }
        public Proposal Proposal { get; set; }
        public DateTime SaleDate { get; set; }
        public int OwnerId { get; set; }
        public User Owner { get; set; }
        public string Status { get; set; }
        public string Origin { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal? CommissionValue { get; set; }
        public string CancellationReason { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxTotal { get; set; }
        public decimal Total { get; set; }

        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public int? DeletedBy { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public List<SaleLine> Lines { get; set; } = new List<SaleLine>();
        public List<Payment> Payments { get; set; } = new List<Payment>();
    }
}