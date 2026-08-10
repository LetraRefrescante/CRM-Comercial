using System;
using System.ComponentModel.DataAnnotations.Schema;
using CRM.Models.Entities.ListasAuxiliares;

namespace CRM.Models.Entities.Oportunidades
{
    [Table("OpportunityStageHistory")]
    public class OpportunityStageHistory
    {
        public int OpportunityStageHistoryId { get; set; }

        public int OpportunityId { get; set; }
        public Opportunity Opportunity { get; set; }

        public int? PreviousStageId { get; set; }
        public OpportunityStage PreviousStage { get; set; }

        public int NewStageId { get; set; }
        public OpportunityStage NewStage { get; set; }

        public DateTime ChangedDate { get; set; }
        public int? ChangedBy { get; set; }
    }
}