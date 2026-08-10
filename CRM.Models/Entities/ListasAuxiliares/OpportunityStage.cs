using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Models.Entities.ListasAuxiliares
{
    [Table("OpportunityStages")]
    public class OpportunityStage
    {
        [Key]
        public int StageId { get; set; }
        public string Name { get; set; }
        public int OrderIndex { get; set; }
        public int DefaultProbability { get; set; }
        public bool IsClosedWon { get; set; }
        public bool IsClosedLost { get; set; }
        public bool IsActive { get; set; }
    }
}