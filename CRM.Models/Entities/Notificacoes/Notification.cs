using System;
using System.ComponentModel.DataAnnotations.Schema;
using CRM.Models.Entities.Seguranca;

namespace CRM.Models.Entities.Notificacoes
{
    [Table("Notifications")]
    public class Notification
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public string Title { get; set; }
        public string Message { get; set; }
        public string NotificationType { get; set; }

        public int? RelatedClientId { get; set; }
        public int? RelatedLeadId { get; set; }
        public int? RelatedOpportunityId { get; set; }
        public int? RelatedProposalId { get; set; }
        public int? RelatedSaleId { get; set; }
        public int? RelatedTaskId { get; set; }

        public bool IsRead { get; set; }
        public DateTime? ReadDate { get; set; }
        public bool IsArchived { get; set; }
        public DateTime? ArchivedDate { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}