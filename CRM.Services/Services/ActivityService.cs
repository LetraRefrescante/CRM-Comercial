using CRM.Data.Repositories;
using CRM.Models.DTOs;
using CRM.Models.Entities.Atividades;
using CRM.Models.Filtros;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CRM.Services
{
    public class ActivityService
    {
        private readonly ActivityParticipantRepository _participantRepository = new ActivityParticipantRepository();
        private readonly ActivityRepository _activityRepository = new ActivityRepository();
        private readonly UserRepository _userRepository = new UserRepository();
        private readonly AuditService _auditService = new AuditService();

        private static readonly string[] TiposValidos = { "Chamada", "Email", "Reunião", "Visita", "Nota" };
        private static readonly string[] PrioridadesValidas = { "Baixa", "Normal", "Alta", "Urgente" };
        private static readonly string[] EstadosValidos = { "Planeada", "Em Curso", "Concluída", "Cancelada" };

        public Activity ObterPorId(int activityId) => _activityRepository.ObterPorId(activityId);

        public List<Activity> ListarPorLead(int leadId) => _activityRepository.ListarPorLead(leadId);

        public List<Activity> Pesquisar(
            ActivityFiltro filtro,
            int pagina, int tamanhoPagina, out int totalRegistos,
            string sortColumn, bool sortAscending,
            int currentUserId, string currentUserRole)
        {
            AplicarAmbitoPermissao(filtro, currentUserId, currentUserRole);
            return _activityRepository.Pesquisar(filtro, pagina, tamanhoPagina, out totalRegistos, sortColumn, sortAscending);
        }

        public List<string> Validar(Activity activity)
        {
            var erros = new List<string>();

            if (string.IsNullOrWhiteSpace(activity.Subject))
                erros.Add("O assunto é obrigatório.");
            else if (activity.Subject.Length > 180)
                erros.Add("O assunto não pode ultrapassar 180 caracteres.");

            if (string.IsNullOrWhiteSpace(activity.Type) || !TiposValidos.Contains(activity.Type))
                erros.Add("O tipo é obrigatório e deve ser um dos valores permitidos (Chamada, Email, Reunião, Visita, Nota).");

            if (activity.AssignedToUserId <= 0)
            {
                erros.Add("O responsável é obrigatório.");
            }
            else
            {
                var responsavel = _userRepository.GetById(activity.AssignedToUserId);
                if (responsavel == null || responsavel.Status != "Ativo")
                    erros.Add("O responsável tem de ser um utilizador ativo.");
            }

            if (activity.StartDateTime == default(DateTime))
                erros.Add("A data/hora de início é obrigatória.");

            if (activity.EndDateTime.HasValue && activity.EndDateTime.Value < activity.StartDateTime)
                erros.Add("A data de fim tem de ser posterior ou igual à data de início.");

            if (!string.IsNullOrEmpty(activity.Priority) && !PrioridadesValidas.Contains(activity.Priority))
                erros.Add("Prioridade inválida.");

            if (string.IsNullOrWhiteSpace(activity.Status) || !EstadosValidos.Contains(activity.Status))
                erros.Add("Estado inválido.");

            if (activity.ReminderDateTime.HasValue && activity.ReminderDateTime.Value > activity.StartDateTime)
                erros.Add("O lembrete deve ser anterior (ou igual) à data de início.");

            var relacionadas = new[] { activity.RelatedClientId, activity.RelatedLeadId, activity.RelatedOpportunityId }
                .Count(id => id.HasValue);
            if (relacionadas > 1)
                erros.Add("A atividade só pode estar relacionada com um registo (cliente, lead ou oportunidade).");

            return erros;
        }

        public int Criar(Activity activity, int currentUserId, string currentUserRole)
        {
            var erros = Validar(activity);
            if (erros.Any())
                throw new InvalidOperationException(string.Join(" ", erros));

            if (!PodeGerir(activity, currentUserId, currentUserRole))
                throw new UnauthorizedAccessException("Sem permissão para criar esta atividade.");

            activity.CreatedBy = currentUserId;
            var id = _activityRepository.Criar(activity);

            _auditService.Registar(currentUserId, "Create", "Activity", id.ToString(),
                $"Atividade '{activity.Subject}' criada.");

            return id;
        }

        public void Atualizar(Activity activity, int currentUserId, string currentUserRole)
        {
            var erros = Validar(activity);
            if (erros.Any())
                throw new InvalidOperationException(string.Join(" ", erros));

            if (!PodeGerir(activity, currentUserId, currentUserRole))
                throw new UnauthorizedAccessException("Sem permissão para editar esta atividade.");

            if (activity.Status == "Concluída" && !activity.CompletedDateTime.HasValue)
                activity.CompletedDateTime = DateTime.UtcNow;

            activity.UpdatedBy = currentUserId;
            _activityRepository.Atualizar(activity);

            _auditService.Registar(currentUserId, "Update", "Activity", activity.ActivityId.ToString(),
                $"Atividade '{activity.Subject}' atualizada.");
        }

        public void Eliminar(int activityId, int currentUserId, string currentUserRole)
        {
            var activity = _activityRepository.ObterPorId(activityId);
            if (activity == null) return;

            if (activity.Status == "Concluída" && !EhPerfilPrivilegiado(currentUserRole))
                throw new UnauthorizedAccessException("Atividades concluídas só podem ser eliminadas por Administrador ou Diretor.");

            if (!PodeGerir(activity, currentUserId, currentUserRole))
                throw new UnauthorizedAccessException("Sem permissão para eliminar esta atividade.");

            _activityRepository.Eliminar(activityId, currentUserId);

            _auditService.Registar(currentUserId, "Delete", "Activity", activityId.ToString(),
                $"Atividade '{activity.Subject}' eliminada (lógico).");
        }
        public List<RelatorioAtividadesLinha> ObterRelatorioProdutividade(
            DateTime? dataInicio, DateTime? dataFim, int? assignedToUserId, string tipo, string status)
        {
            var filtro = new ActivityFiltro
            {
                DataInicio = dataInicio,
                DataFim = dataFim,
                AssignedToUserId = assignedToUserId,
                Tipo = tipo,
                Status = status
            };

            var atividades = _activityRepository.Pesquisar(filtro, 1, int.MaxValue, out int _, "StartDateTime", true);

            return atividades
                .GroupBy(a => a.AssignedTo?.Name ?? "(sem responsável)")
                .OrderByDescending(g => g.Count())
                .Select(g => new RelatorioAtividadesLinha
                {
                    Responsavel = g.Key,
                    Total = g.Count(),
                    Concluidas = g.Count(a => a.Status == "Concluída"),
                    Planeadas = g.Count(a => a.Status == "Planeada"),
                    EmCurso = g.Count(a => a.Status == "Em Curso"),
                    Canceladas = g.Count(a => a.Status == "Cancelada")
                })
                .ToList();
        }

        public bool PodeCriar(string currentUserRole) => !EhPerfilConsulta(currentUserRole);

        public bool PodeGerir(Activity activity, int currentUserId, string currentUserRole)
        {
            if (EhPerfilPrivilegiado(currentUserRole)) return true;
            if (EhPerfilConsulta(currentUserRole)) return false;
            return activity.AssignedToUserId == currentUserId;
        }

        public bool PodeEliminar(Activity activity, int currentUserId, string currentUserRole)
        {
            if (activity.Status == "Concluída" && !EhPerfilPrivilegiado(currentUserRole)) return false;
            return PodeGerir(activity, currentUserId, currentUserRole);
        }

        public bool TemAmbitoProprios(string currentUserRole) => currentUserRole == "Comercial";

        private void AplicarAmbitoPermissao(ActivityFiltro filtro, int currentUserId, string currentUserRole)
        {
            if (EhPerfilPrivilegiado(currentUserRole)) return;
            if (EhPerfilConsulta(currentUserRole)) return;
            filtro.AssignedToUserId = currentUserId;
        }
        public List<Activity> ListarPorPeriodo(DateTime inicio, DateTime fim, ActivityFiltro filtro, int currentUserId, string currentUserRole)
        {
            filtro = filtro ?? new ActivityFiltro();
            AplicarAmbitoPermissao(filtro, currentUserId, currentUserRole);
            return _activityRepository.ListarPorPeriodo(inicio, fim, filtro);
        }
        private static bool EhPerfilPrivilegiado(string role) =>
            role == "Administrador" || role == "Diretor";

        private static bool EhPerfilConsulta(string role) =>
            role == "Financeiro" || role == "Consulta";
        public List<Activity> ListarPorOportunidade(int opportunityId) => 
            _activityRepository.ListarPorOportunidade(opportunityId);

        public List<ActivityParticipant> ListarParticipantes(int activityId) =>
            _participantRepository.ListarPorAtividade(activityId);

        public void SincronizarParticipantes(int activityId, List<ActivityParticipant> participantes) =>
            _participantRepository.Sincronizar(activityId, participantes ?? new List<ActivityParticipant>());
    }
}