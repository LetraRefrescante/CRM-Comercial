using CRM.Data.Repositories;
using CRM.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CRM.Services
{
    public class RelatorioService
    {
        private readonly PermissionService _permissionService = new PermissionService();
        private readonly LeadRepository _leadRepository = new LeadRepository();

        public List<RelatorioLeadsLinha> ObterRelatorioLeads(
            DateTime? dataInicio, DateTime? dataFim, int? leadSourceId, string status,
            int? ownerId, int? scoreMin, int? scoreMax)
        {
            var leads = _leadRepository.Listar(
                pesquisa: null, status: status, leadSourceId: leadSourceId, ownerId: ownerId,
                scoreMin: scoreMin, scoreMax: scoreMax, dataInicio: dataInicio, dataFim: dataFim,
                pagina: 1, tamanhoPagina: int.MaxValue, totalRegistos: out int _);

            return leads
                .GroupBy(l => l.LeadSource?.Name ?? "(sem origem)")
                .OrderByDescending(g => g.Count())
                .Select(g => new RelatorioLeadsLinha
                {
                    Origem = g.Key,
                    Quantidade = g.Count(),
                    Convertidos = g.Count(l => l.Status == "Convertido"),
                    TaxaConversao = g.Count() == 0 ? 0 : Math.Round(g.Count(l => l.Status == "Convertido") * 100m / g.Count(), 1)
                })
                .ToList();
        }

        public bool PodeAcederFinanceiro(string perfil) =>
            _permissionService.ObterNivel(perfil, "Relatorios") >= NivelAcesso.Consulta ||
            _permissionService.TemCodigo(perfil, "Relatorios.Financeiros");

        public bool PodeAcederGeral(string perfil) =>
            _permissionService.ObterNivel(perfil, "Relatorios") >= NivelAcesso.Consulta;
    }
}