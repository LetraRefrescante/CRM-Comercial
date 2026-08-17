using System;

namespace CRM.Models.Filtros
{
    public class DocumentFiltro
    {
        public string Pesquisa { get; set; }
        public string Category { get; set; }
        public string EntityType { get; set; }
        public bool? IsConfidential { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
    }
}