using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Models.Entities.ListasAuxiliares
{
    [Table("Settings")]
    public class Settings
    {
        public int SettingId { get; set; }

        public string CompanyName { get; set; }
        public string Currency { get; set; }
        public string TimeZone { get; set; }

        public int AlertDaysLeads { get; set; }
        public int AlertDaysOpportunities { get; set; }
        public int AlertDaysProposals { get; set; }

        public int MaxFailedLoginAttempts { get; set; }
        public int AccountLockoutMinutes { get; set; }
        public int SessionTimeoutMinutes { get; set; }

        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
    }
}