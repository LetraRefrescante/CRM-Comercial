using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Models.Entities.Notificacoes
{
    [Table("EmailTemplates")]
    public class EmailTemplate
    {
        public int EmailTemplateId { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
    }
}