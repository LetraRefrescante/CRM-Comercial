using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CRM.Models.Entities.Clientes;
using CRM.Models.Entities.ListasAuxiliares;
using CRM.Models.Entities.Seguranca;

namespace CRM.Models.Entities.Leads
{
    [Table("Leads")]
    public class Lead
    {
        public int LeadId { get; set; }
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int LeadSourceId { get; set; }
        public LeadSource LeadSource { get; set; }
        public string Status { get; set; }
        public int? Score { get; set; }
        public int OwnerId { get; set; }
        public User Owner { get; set; }
        public DateTime? NextContactDate { get; set; }
        public int? LossReasonId { get; set; }
        public LossReason LossReason { get; set; }
        public DateTime? ConvertedDate { get; set; }
        public int? ConvertedByUserId { get; set; }
        public int? ConvertedClientId { get; set; }
        public Client ConvertedClient { get; set; }
        public int? ConvertedContactId { get; set; }
        public Contact ConvertedContact { get; set; }
        public int? ConvertedOpportunityId { get; set; }
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