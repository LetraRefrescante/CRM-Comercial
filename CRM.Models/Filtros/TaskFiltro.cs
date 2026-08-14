using System;

namespace CRM.Models.Filtros
{
    public class TaskFiltro
    {
        public string Pesquisa { get; set; }
        public string Status { get; set; }
        public int? AssignedToUserId { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public int? RelatedClientId { get; set; }
        public int? RelatedLeadId { get; set; }
        public int? RelatedOpportunityId { get; set; }
    }
}