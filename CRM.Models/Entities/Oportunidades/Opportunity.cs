using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CRM.Models.Entities.Clientes;
using CRM.Models.Entities.ListasAuxiliares;
using CRM.Models.Entities.Seguranca;

namespace CRM.Models.Entities.Oportunidades
{
    [Table("Opportunities")]
    public class Opportunity
    {
        public int OpportunityId { get; set; }
        public string Title { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; }

        public int? ContactId { get; set; }
        public Contact Contact { get; set; }

        public int StageId { get; set; }
        public OpportunityStage Stage { get; set; }

        public decimal EstimatedValue { get; set; }
        public int Probability { get; set; }
        public DateTime ExpectedCloseDate { get; set; }

        public int OwnerId { get; set; }
        public User Owner { get; set; }

        public string Competitor { get; set; }

        public int? LossReasonId { get; set; }
        public LossReason LossReason { get; set; }

        public bool IsClosed { get; set; }
        public DateTime? ClosedDate { get; set; }

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