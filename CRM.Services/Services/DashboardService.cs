using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CRM.Data.Repositories;
using CRM.Models.DTOs;
using CRM.Models.Entities.Atividades;
using CRM.Models.Entities.Oportunidades;
using CRM.Models.Filtros;

namespace CRM.Services
{
    public class DashboardService
    {
        private readonly ClientRepository _clientRepository = new ClientRepository();
        private readonly ClientService _clientService = new ClientService();
        private readonly LeadService _leadService = new LeadService();
        private readonly OpportunityService _opportunityService = new OpportunityService();
        private readonly ProposalService _proposalService = new ProposalService();
        private readonly SaleService _saleService = new SaleService();
        private readonly TaskService _taskService = new TaskService();
        private readonly ActivityService _activityService = new ActivityService();
        private readonly SettingsRepository _settingsRepository = new SettingsRepository();

        private static readonly CultureInfo PtPt = new CultureInfo("pt-PT");

        // ===================== Indicadores =====================
        public DashboardIndicadores ObterIndicadores(int userId, string perfil)
        {
            var settings = _settingsRepository.ObterConfiguracaoAtual();
            int diasAlertaPropostas = settings?.AlertDaysProposals ?? 7;

            int? clientAccountManagerId = _clientService.TemAmbitoProprios(perfil) ? userId : (int?)null;
            int? leadOwnerId = _leadService.TemAmbitoProprios(perfil) ? userId : (int?)null;
            int? proposalAccountManagerId = _proposalService.TemAmbitoProprios(perfil) ? userId : (int?)null;
            int? saleOwnerId = _saleService.TemAmbitoProprios(perfil) ? userId : (int?)null;

            var hoje = DateTime.Today;
            var inicioMes = new DateTime(hoje.Year, hoje.Month, 1);
            var inicioAno = new DateTime(hoje.Year, 1, 1);

            var indicadores = new DashboardIndicadores
            {
                TotalClientesAtivos = _clientRepository.ContarAtivos(clientAccountManagerId),
                NovosClientesMes = _clientRepository.ContarNovosNoMes(clientAccountManagerId),

                LeadsNovos = ContarLeads(LeadService.StatusNovo, leadOwnerId),
                LeadsEmContacto = ContarLeads(LeadService.StatusEmContacto, leadOwnerId),
                LeadsQualificados = ContarLeads(LeadService.StatusQualificado, leadOwnerId)
            };

            var oportunidadesAbertas = _opportunityService.Listar(
                null, null, null, null, false, perfil, userId, 1, 5000, out int totalOportunidades);
            indicadores.OportunidadesAbertas = totalOportunidades;
            indicadores.ValorPonderadoAberto = oportunidadesAbertas.Sum(o => _opportunityService.CalcularValorPonderado(o));

            var vendasMes = _saleService.Listar(null, null, null, saleOwnerId, inicioMes, hoje.AddDays(1),
                1, 5000, out _, "SaleDate", false)
                .Where(s => s.Status != SaleService.StatusCancelada);
            indicadores.VendasMes = vendasMes.Sum(s => s.Total);

            var vendasAno = _saleService.Listar(null, null, null, saleOwnerId, inicioAno, hoje.AddDays(1),
                1, 5000, out _, "SaleDate", false)
                .Where(s => s.Status != SaleService.StatusCancelada);
            indicadores.VendasAno = vendasAno.Sum(s => s.Total);

            var vencidas = _taskService.ListarVencidas();
            if (_taskService.TemAmbitoProprios(perfil))
                vencidas = vencidas.Where(t => t.AssignedToUserId == userId).ToList();
            indicadores.TarefasVencidas = vencidas.Count;

            var filtroHoje = new TaskFiltro { DataInicio = hoje, DataFim = hoje.AddDays(1) };
            var tarefasHoje = _taskService.Pesquisar(filtroHoje, 1, 5000, out _, "DueDate", true, userId, perfil);
            indicadores.TarefasHoje = tarefasHoje.Count(t => t.Status != "Concluída" && t.Status != "Cancelada");

            var propostasEnviadas = _proposalService.Listar(null, ProposalService.StatusEnviada, null,
                proposalAccountManagerId, null, null, 1, 5000, out _, "ValidUntil", true);
            indicadores.PropostasAExpirar = propostasEnviadas
                .Count(p => p.ValidUntil >= hoje && p.ValidUntil <= hoje.AddDays(diasAlertaPropostas));

            return indicadores;
        }

        private int ContarLeads(string status, int? ownerId)
        {
            _leadService.Listar(null, status, null, ownerId, null, null, null, null, 1, 1, out int total, "CreatedDate", false);
            return total;
        }

        // ===================== Vendas por Mês (últimos 12 meses) =====================
        public List<VendaMensalDto> ObterVendasPorMes(int userId, string perfil, int meses = 12)
        {
            int? saleOwnerId = _saleService.TemAmbitoProprios(perfil) ? userId : (int?)null;

            var inicio = DateTime.Today.AddMonths(-(meses - 1));
            var inicioMesInicio = new DateTime(inicio.Year, inicio.Month, 1);

            var vendas = _saleService.Listar(null, null, null, saleOwnerId, inicioMesInicio, DateTime.Today.AddDays(1),
                1, 10000, out _, "SaleDate", true)
                .Where(s => s.Status != SaleService.StatusCancelada)
                .ToList();

            var resultado = new List<VendaMensalDto>();
            for (int i = meses - 1; i >= 0; i--)
            {
                var mesRef = DateTime.Today.AddMonths(-i);
                decimal total = vendas
                    .Where(s => s.SaleDate.Year == mesRef.Year && s.SaleDate.Month == mesRef.Month)
                    .Sum(s => s.Total);

                resultado.Add(new VendaMensalDto
                {
                    Mes = mesRef.ToString("MMM/yy", PtPt),
                    Total = total
                });
            }

            return resultado;
        }

        // ===================== Pipeline por Fase =====================
        public List<FasePipelineDto> ObterPipelinePorFase(int userId, string perfil)
        {
            var abertas = _opportunityService.Listar(null, null, null, null, false, perfil, userId, 1, 5000, out _);

            return abertas
                .GroupBy(o => new { Nome = o.Stage?.Name ?? "—", Ordem = o.Stage?.OrderIndex ?? 0 })
                .OrderBy(g => g.Key.Ordem)
                .Select(g => new FasePipelineDto
                {
                    Fase = g.Key.Nome,
                    Quantidade = g.Count(),
                    Valor = g.Sum(o => o.EstimatedValue)
                })
                .ToList();
        }

        // ===================== Origem dos Leads =====================
        public List<OrigemLeadDto> ObterOrigemLeads(int userId, string perfil)
        {
            int? leadOwnerId = _leadService.TemAmbitoProprios(perfil) ? userId : (int?)null;

            var leads = _leadService.Listar(null, null, null, leadOwnerId, null, null, null, null,
                1, 10000, out _, "CreatedDate", false);

            return leads
                .GroupBy(l => l.LeadSource?.Name ?? "—")
                .Select(g => new OrigemLeadDto { Origem = g.Key, Quantidade = g.Count() })
                .OrderByDescending(g => g.Quantidade)
                .ToList();
        }

        // ===================== Top Comerciais =====================
        // Só faz sentido para quem não está limitado a "próprios" — um Comercial só se veria a
        // si mesmo. A página consulta isto antes de mostrar o widget.
        public bool PodeVerTopComerciais(string perfil) => !_saleService.TemAmbitoProprios(perfil);

        public List<TopComercialDto> ObterTopComerciais(int top = 5)
        {
            var hoje = DateTime.Today;
            var inicioMes = new DateTime(hoje.Year, hoje.Month, 1);

            var vendasMes = _saleService.Listar(null, null, null, null, inicioMes, hoje.AddDays(1),
                1, 10000, out _, "SaleDate", false)
                .Where(s => s.Status != SaleService.StatusCancelada);

            return vendasMes
                .GroupBy(s => new { s.OwnerId, Nome = s.Owner?.Name ?? "—" })
                .Select(g => new TopComercialDto { Comercial = g.Key.Nome, TotalVendido = g.Sum(s => s.Total) })
                .OrderByDescending(g => g.TotalVendido)
                .Take(top)
                .ToList();
        }

        // ===================== Próximas Reuniões =====================
        public List<Activity> ObterProximasReunioes(int userId, string perfil, int top = 5)
        {
            var filtro = new ActivityFiltro { Tipo = "Reunião", DataInicio = DateTime.Now };
            return _activityService.Pesquisar(filtro, 1, top, out _, "StartDateTime", true, userId, perfil);
        }

        // ===================== Oportunidades sem Atividade Recente =====================
        public List<Opportunity> ObterOportunidadesSemAtividade(int userId, string perfil, int top = 5)
        {
            var settings = _settingsRepository.ObterConfiguracaoAtual();
            int diasAlerta = settings?.AlertDaysOpportunities ?? 14;

            return _opportunityService.ListarSemAtividadeRecente(diasAlerta, perfil, userId).Take(top).ToList();
        }

        // ===================== Últimas Atividades =====================
        public List<Activity> ObterUltimasAtividades(int userId, string perfil, int top = 6)
        {
            var filtro = new ActivityFiltro();
            return _activityService.Pesquisar(filtro, 1, top, out _, "StartDateTime", false, userId, perfil);
        }
    }
}