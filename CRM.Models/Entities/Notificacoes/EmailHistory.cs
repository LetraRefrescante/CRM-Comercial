using System;
using System.ComponentModel.DataAnnotations.Schema;
using CRM.Models.Entities.Seguranca;

namespace CRM.Models.Entities.Notificacoes
{
    [Table("EmailHistory")]
    public class EmailHistory
    {
        public int EmailHistoryId { get; set; }
        public string ToAddress { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public int? EmailTemplateId { get; set; }
        public EmailTemplate EmailTemplate { get; set; }

        public int? RelatedClientId { get; set; }
        public int? RelatedContactId { get; set; }
        public int? RelatedLeadId { get; set; }
        public int? RelatedOpportunityId { get; set; }
        public int? RelatedProposalId { get; set; }

        public DateTime SentDate { get; set; }
        public int? SentByUserId { get; set; }
        public User SentByUser { get; set; }
        public string Status { get; set; }
        public string FailureReason { get; set; }
    }
}