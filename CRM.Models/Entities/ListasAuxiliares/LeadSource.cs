using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Models.Entities.ListasAuxiliares
{
    [Table("LeadSources")]
    public class LeadSource
    {
        public int LeadSourceId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}