namespace CRM.Models.DTOs
{
    public class DashboardIndicadores
    {
        public int TotalClientesAtivos { get; set; }
        public int NovosClientesMes { get; set; }

        public int LeadsNovos { get; set; }
        public int LeadsEmContacto { get; set; }
        public int LeadsQualificados { get; set; }

        public int OportunidadesAbertas { get; set; }
        public decimal ValorPonderadoAberto { get; set; }

        public decimal VendasMes { get; set; }
        public decimal VendasAno { get; set; }

        public int TarefasVencidas { get; set; }
        public int TarefasHoje { get; set; }

        public int PropostasAExpirar { get; set; }
    }
}