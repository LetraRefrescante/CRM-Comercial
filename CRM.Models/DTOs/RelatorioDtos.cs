using System.Collections.Generic;

namespace CRM.Models.DTOs
{
    public class RelatorioLeadsLinha
    {
        public string Origem { get; set; }
        public int Quantidade { get; set; }
        public int Convertidos { get; set; }
        public decimal TaxaConversao { get; set; }
    }

    public class RelatorioAtividadesLinha
    {
        public string Responsavel { get; set; }
        public int Total { get; set; }
        public int Concluidas { get; set; }
        public int Planeadas { get; set; }
        public int EmCurso { get; set; }
        public int Canceladas { get; set; }
    }

    public class RelatorioClientesLinha
    {
        public string Setor { get; set; }
        public int Total { get; set; }
        public int Ativos { get; set; }
        public int Potenciais { get; set; }
        public int Inativos { get; set; }
        public int Bloqueados { get; set; }
    }

    public class RelatorioComissoesLinha
    {
        public string Comercial { get; set; }
        public int QuantidadeVendas { get; set; }
        public decimal TotalVendas { get; set; }
        public decimal TotalComissao { get; set; }
    }
}