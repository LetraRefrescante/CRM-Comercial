using System.ComponentModel.DataAnnotations.Schema;
using CRM.Models.Entities.Seguranca;

namespace CRM.Models.Entities.Atividades
{
    [Table("ActivityParticipants")]
    public class ActivityParticipant
    {
        public int ActivityParticipantId { get; set; }
        public int ActivityId { get; set; }
        public Activity Activity { get; set; }

        public int? UserId { get; set; }
        public User User { get; set; }

        public string ExternalName { get; set; }
        public string ExternalEmail { get; set; }
    }
}