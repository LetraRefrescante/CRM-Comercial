namespace CRM.Models.DTOs
{
    public class EmailComporRequest
    {
        public string ToAddress { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public int? EmailTemplateId { get; set; }
        public string RelatedEntityType { get; set; }
        public int? RelatedEntityId { get; set; }
    }
}