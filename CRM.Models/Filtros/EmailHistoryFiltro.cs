using System;

namespace CRM.Models.Filtros
{
    public class EmailHistoryFiltro
    {
        public string Pesquisa { get; set; }
        public string Status { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
    }
}