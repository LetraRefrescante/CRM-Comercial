using System;
using System.Collections.Generic;
using CRM.Data.Repositories;
using CRM.Models.Entities.Oportunidades;

namespace CRM.Services
{
    public class OpportunityService
    {
        private readonly OpportunityRepository _opportunityRepository = new OpportunityRepository();
        private readonly OpportunityStageRepository _stageRepository = new OpportunityStageRepository();
        private readonly AuditService _auditService = new AuditService();

        // ---------- Âmbito e permissões (matriz da blueprint) ----------

        public bool TemAmbitoProprios(string perfil) => perfil == "Comercial";

        public bool PodeEditar(string perfil) =>
            perfil == "Administrador" || perfil == "Diretor" || perfil == "Comercial";

        public bool PodeFechar(string perfil) => PodeEditar(perfil);

        // ---------- Consulta ----------

        public List<Opportunity> Listar(string pesquisa, int? stageId, int? clientId, bool? isClosed,
            string perfil, int userId, int pagina, int tamanhoPagina, out int totalRegistos)
        {
            int? ownerId = TemAmbitoProprios(perfil) ? userId : (int?)null;
            return _opportunityRepository.Listar(pesquisa, stageId, ownerId, clientId, isClosed,
                pagina, tamanhoPagina, out totalRegistos);
        }

        public List<Opportunity> ListarParaPipeline(string perfil, int userId)
        {
            int? ownerId = TemAmbitoProprios(perfil) ? userId : (int?)null;
            return _opportunityRepository.ListarAbertasParaPipeline(ownerId);
        }
        public Opportunity ObterPorId(int opportunityId, string perfil, int userId)
        {
            var opportunity = _opportunityRepository.GetById(opportunityId);
            if (opportunity == null) return null;

            if (TemAmbitoProprios(perfil) && opportunity.OwnerId != userId)
                return null;

            return opportunity;
        }

        // ---------- Escrita ----------

        public void Criar(Opportunity opportunity, int userId)
        {
            var fase = _stageRepository.ObterPorId(opportunity.StageId);
            if (opportunity.Probability == 0 && fase != null)
                opportunity.Probability = fase.DefaultProbability;

            opportunity.CreatedDate = DateTime.UtcNow;
            opportunity.CreatedBy = userId;
            opportunity.IsClosed = false;

            _opportunityRepository.Adicionar(opportunity);

            _opportunityRepository.RegistarHistoricoFase(new OpportunityStageHistory
            {
                OpportunityId = opportunity.OpportunityId,
                PreviousStageId = null,
                NewStageId = opportunity.StageId,
                ChangedDate = DateTime.UtcNow,
                ChangedBy = userId
            });

            _auditService.Registar(userId, "Criação", "Opportunity", opportunity.OpportunityId.ToString());
        }

        public void Atualizar(Opportunity opportunity, int faseAnterior, int userId)
        {
            opportunity.UpdatedDate = DateTime.UtcNow;
            opportunity.UpdatedBy = userId;

            _opportunityRepository.Atualizar(opportunity);

            if (opportunity.StageId != faseAnterior)
            {
                _opportunityRepository.RegistarHistoricoFase(new OpportunityStageHistory
                {
                    OpportunityId = opportunity.OpportunityId,
                    PreviousStageId = faseAnterior,
                    NewStageId = opportunity.StageId,
                    ChangedDate = DateTime.UtcNow,
                    ChangedBy = userId
                });
            }

            _auditService.Registar(userId, "Alteração", "Opportunity", opportunity.OpportunityId.ToString());
        }

        public string MudarFase(int opportunityId, int novaFaseId, string perfil, int userId)
        {
            var opportunity = _opportunityRepository.GetById(opportunityId);
            if (opportunity == null) return "Oportunidade não encontrada.";

            if (TemAmbitoProprios(perfil) && opportunity.OwnerId != userId)
                return "Sem permissão para mover esta oportunidade.";

            if (opportunity.IsClosed) return "Esta oportunidade já está fechada.";

            int faseAnterior = opportunity.StageId;
            if (faseAnterior == novaFaseId) return null;

            var fase = _stageRepository.ObterPorId(novaFaseId);

            opportunity.StageId = novaFaseId;
            if (fase != null) opportunity.Probability = fase.DefaultProbability;
            opportunity.UpdatedDate = DateTime.UtcNow;
            opportunity.UpdatedBy = userId;

            _opportunityRepository.Atualizar(opportunity);

            _opportunityRepository.RegistarHistoricoFase(new OpportunityStageHistory
            {
                OpportunityId = opportunityId,
                PreviousStageId = faseAnterior,
                NewStageId = novaFaseId,
                ChangedDate = DateTime.UtcNow,
                ChangedBy = userId
            });

            _auditService.Registar(userId, "Mudança de Fase", "Opportunity", opportunityId.ToString());
            return null;
        }

        public string Fechar(int opportunityId, bool ganho, int? lossReasonId, int userId)
        {
            if (!ganho && !lossReasonId.HasValue)
                return "É obrigatório indicar o motivo de perda.";

            var opportunity = _opportunityRepository.GetById(opportunityId);
            if (opportunity == null) return "Oportunidade não encontrada.";
            if (opportunity.IsClosed) return "Esta oportunidade já está fechada.";

            var faseFecho = _stageRepository.ObterFaseFechamento(isClosedWon: ganho);
            int faseAnterior = opportunity.StageId;

            opportunity.IsClosed = true;
            opportunity.ClosedDate = DateTime.UtcNow;
            opportunity.LossReasonId = ganho ? null : lossReasonId;
            if (faseFecho != null) opportunity.StageId = faseFecho.StageId;
            opportunity.Probability = ganho ? 100 : 0;
            opportunity.UpdatedDate = DateTime.UtcNow;
            opportunity.UpdatedBy = userId;

            _opportunityRepository.Atualizar(opportunity);

            if (faseFecho != null && faseFecho.StageId != faseAnterior)
            {
                _opportunityRepository.RegistarHistoricoFase(new OpportunityStageHistory
                {
                    OpportunityId = opportunityId,
                    PreviousStageId = faseAnterior,
                    NewStageId = faseFecho.StageId,
                    ChangedDate = DateTime.UtcNow,
                    ChangedBy = userId
                });
            }

            _auditService.Registar(userId, ganho ? "Fecho Ganho" : "Fecho Perdido", "Opportunity", opportunityId.ToString());
            return null;
        }

        // ---------- Cálculos ----------

        public decimal CalcularValorPonderado(Opportunity opportunity) =>
            opportunity.EstimatedValue * opportunity.Probability / 100m;
    }
}