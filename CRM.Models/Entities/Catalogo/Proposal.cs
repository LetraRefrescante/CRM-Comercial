using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CRM.Models.Entities.Clientes;
using CRM.Models.Entities.ListasAuxiliares;
using CRM.Models.Entities.Oportunidades;
using CRM.Models.Entities.Seguranca;

namespace CRM.Models.Entities.Catalogo
{
    [Table("Proposals")]
    public class Proposal
    {
        public int ProposalId { get; set; }
        public string ProposalNumber { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; }

        public int? OpportunityId { get; set; }
        public Opportunity Opportunity { get; set; }

        public DateTime IssueDate { get; set; }
        public DateTime ValidUntil { get; set; }
        public string Status { get; set; }
        public decimal GlobalDiscountPercent { get; set; }

        public int? PaymentTermId { get; set; }
        public PaymentTerm PaymentTerm { get; set; }

        public string Notes { get; set; }

        public int? ParentProposalId { get; set; }
        public Proposal ParentProposal { get; set; }
        public int VersionNumber { get; set; }

        public decimal SubTotal { get; set; }
        public decimal TaxTotal { get; set; }
        public decimal Total { get; set; }

        public DateTime? AcceptedDate { get; set; }
        public int? AcceptedByUserId { get; set; }
        [ForeignKey("AcceptedByUserId")]
        public User AcceptedByUser { get; set; }
        public string AcceptanceNotes { get; set; }

        public DateTime? SentDate { get; set; }
        public string SentToEmail { get; set; }

        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public int? DeletedBy { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public List<ProposalLine> Lines { get; set; } = new List<ProposalLine>();
    }
}