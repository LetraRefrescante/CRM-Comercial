using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Models.Entities.ListasAuxiliares
{
    [Table("LossReasons")]
    public class LossReason
    {
        public int LossReasonId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
    }
}