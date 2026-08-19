using System.Collections.Generic;

namespace CRM.Models.DTOs
{
    public class RelatorioVendasLinha
    {
        public string Periodo { get; set; }
        public int Quantidade { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxTotal { get; set; }
        public decimal Total { get; set; }
    }

    public class RelatorioVendasResultado
    {
        public List<RelatorioVendasLinha> Linhas { get; set; }
        public int QuantidadeGeral { get; set; }
        public decimal TotalGeral { get; set; }
    }
}