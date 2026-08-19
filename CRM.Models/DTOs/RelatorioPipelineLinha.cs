namespace CRM.Models.DTOs
{
    public class RelatorioPipelineLinha
    {
        public string Fase { get; set; }
        public int Quantidade { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal ValorPonderado { get; set; }
    }
}