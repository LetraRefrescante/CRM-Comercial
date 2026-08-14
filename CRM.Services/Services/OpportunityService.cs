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
        private readonly ClientRepository _clientRepository = new ClientRepository();
        private readonly AuditService _auditService = new AuditService();
        private readonly PermissionService _permissionService = new PermissionService();

        private const string Modulo = "Oportunidades";

        // ---------- Âmbito e permissões (tabela Permissions/RolePermissions) ----------

        public bool TemAmbitoProprios(string perfil) =>
            _permissionService.ObterNivel(perfil, Modulo) == NivelAcesso.Proprios;

        public bool PodeEditar(string perfil) =>
            _permissionService.ObterNivel(perfil, Modulo) >= NivelAcesso.Proprios;

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

            if (TemAmbitoProprios(perfil))
            {
                if (opportunity.OwnerId != userId) return null;
            }
            else if (_permissionService.ObterNivel(perfil, Modulo) < NivelAcesso.Consulta)
            {
                return null;
            }

            return opportunity;
        }
        public List<OpportunityStageHistory> ListarHistoricoFases(int opportunityId) =>
    _opportunityRepository.ListarHistoricoFases(opportunityId);

        // ---------- Escrita ----------

        /// <summary>
        /// Antes só recebia (opportunity, userId) e não validava permissão nem âmbito —
        /// isso ficava por conta de quem chamava (ex.: código da página). Alinhado agora com
        /// o padrão do ClientService: um Comercial (âmbito "próprios") só pode criar
        /// oportunidades atribuídas a si mesmo, e apenas para clientes que já são seus.
        /// </summary>
        public string Criar(Opportunity opportunity, string perfil, int userId)
        {
            if (!PodeEditar(perfil))
                return "Sem permissão para criar oportunidades.";

            if (TemAmbitoProprios(perfil))
            {
                if (opportunity.OwnerId != userId)
                    return "Só podes atribuir oportunidades a ti próprio.";

                var cliente = _clientRepository.GetById(opportunity.ClientId);
                if (cliente == null || cliente.AccountManagerId != userId)
                    return "Só podes criar oportunidades para clientes atribuídos a ti.";
            }

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

            return null;
        }

        /// <summary>
        /// Igual ao Criar: passa a validar permissão e âmbito. Reconsulta o registo
        /// existente (não o objeto vindo do formulário) para confirmar a posse — o
        /// mesmo cuidado que o ClientService.Atualizar já tinha, para não confiar
        /// apenas no que veio no postback.
        /// </summary>
        public string Atualizar(Opportunity opportunity, int faseAnterior, string perfil, int userId)
        {
            if (!PodeEditar(perfil))
                return "Sem permissão para editar oportunidades.";

            if (TemAmbitoProprios(perfil))
            {
                var existente = _opportunityRepository.GetById(opportunity.OpportunityId);
                if (existente == null || existente.OwnerId != userId)
                    return "Sem permissão para editar esta oportunidade.";

                if (opportunity.OwnerId != userId)
                    return "Só podes atribuir oportunidades a ti próprio.";

                var cliente = _clientRepository.GetById(opportunity.ClientId);
                if (cliente == null || cliente.AccountManagerId != userId)
                    return "Só podes atribuir oportunidades a clientes atribuídos a ti.";
            }

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

            return null;
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

        /// <summary>
        /// Passa a receber "perfil" e a validar PodeFechar + âmbito próprio (antes não
        /// validava nada disto — qualquer utilizador autenticado conseguia fechar qualquer
        /// oportunidade, desde que soubesse o Id).
        /// </summary>
        public string Fechar(int opportunityId, bool ganho, int? lossReasonId, string perfil, int userId)
        {
            if (!PodeFechar(perfil))
                return "Sem permissão para fechar oportunidades.";

            if (!ganho && !lossReasonId.HasValue)
                return "É obrigatório indicar o motivo de perda.";

            var opportunity = _opportunityRepository.GetById(opportunityId);
            if (opportunity == null) return "Oportunidade não encontrada.";

            if (TemAmbitoProprios(perfil) && opportunity.OwnerId != userId)
                return "Sem permissão para fechar esta oportunidade.";

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