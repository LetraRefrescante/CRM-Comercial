using CRM.Models.Entities.Clientes;
using CRM.Models.Entities.Leads;
using CRM.Models.Entities.Oportunidades;
using CRM.Models.Entities.Seguranca;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Models.Entities.Atividades
{
    [Table("Tasks")]
    public class TaskItem
    {
        [Key]
        public int TaskId { get; set; }

        public string Subject { get; set; }

        [ForeignKey(nameof(RelatedClient))]
        public int? RelatedClientId { get; set; }

        public Client RelatedClient { get; set; }

        [ForeignKey(nameof(RelatedLead))]
        public int? RelatedLeadId { get; set; }

        public Lead RelatedLead { get; set; }

        [ForeignKey(nameof(RelatedOpportunity))]
        public int? RelatedOpportunityId { get; set; }
        public Opportunity RelatedOpportunity { get; set; }

        [ForeignKey(nameof(AssignedTo))]
        public int AssignedToUserId { get; set; }

        public User AssignedTo { get; set; }

        public DateTime DueDate { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public DateTime? CompletedDateTime { get; set; }

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