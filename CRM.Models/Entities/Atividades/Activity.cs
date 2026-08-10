using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CRM.Models.Entities.Clientes;
using CRM.Models.Entities.Leads;
using CRM.Models.Entities.Seguranca;

namespace CRM.Models.Entities.Atividades
{
    [Table("Activities")]
    public class Activity
    {
        public int ActivityId { get; set; }
        public string Type { get; set; }
        public string Subject { get; set; }

        public int? RelatedClientId { get; set; }
        public Client RelatedClient { get; set; }

        public int? RelatedLeadId { get; set; }
        public Lead RelatedLead { get; set; }
        public int? RelatedOpportunityId { get; set; }

        public int AssignedToUserId { get; set; }
        public User AssignedTo { get; set; }

        public DateTime StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public DateTime? ReminderDateTime { get; set; }
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