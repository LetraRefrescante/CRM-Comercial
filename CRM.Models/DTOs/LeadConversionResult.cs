namespace CRM.Models.DTOs
{
    public class LeadConversionResult
    {
        public int ClientId { get; set; }
        public int? ContactId { get; set; }
        public int? OpportunityId { get; set; }
    }
}