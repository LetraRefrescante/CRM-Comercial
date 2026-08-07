using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Models.Entities.Documentos
{
    [Table("DocumentAccessLog")]
    public class DocumentAccessLog
    {
        public int DocumentAccessLogId { get; set; }
        public int DocumentId { get; set; }
        public Document Document { get; set; }
        public string Action { get; set; }
        public int? UserId { get; set; }
        public DateTime AccessDate { get; set; }
        public string IpAddress { get; set; }
    }
}