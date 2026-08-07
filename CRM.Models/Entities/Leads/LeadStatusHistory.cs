using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Models.Entities.Leads
{
    [Table("LeadStatusHistory")]
    public class LeadStatusHistory
    {
        public int LeadStatusHistoryId { get; set; }

        public int LeadId { get; set; }
        public Lead Lead { get; set; }

        public string PreviousStatus { get; set; }
        public string NewStatus { get; set; }
        public DateTime ChangedDate { get; set; }
        public int? ChangedBy { get; set; }
    }
}