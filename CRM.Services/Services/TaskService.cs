using System;
using System.Collections.Generic;
using System.Linq;
using CRM.Data.Repositories;
using CRM.Models.Entities.Atividades;
using CRM.Models.Filtros;

namespace CRM.Services
{
    public class TaskService
    {
        private readonly TaskRepository _taskRepository = new TaskRepository();
        private readonly UserRepository _userRepository = new UserRepository();
        private readonly AuditService _auditService = new AuditService();

        private static readonly string[] PrioridadesValidas = { "Baixa", "Normal", "Alta", "Urgente" };
        private static readonly string[] EstadosValidos = { "Planeada", "Em Curso", "Concluída", "Cancelada" };

        public TaskItem ObterPorId(int taskId) => _taskRepository.ObterPorId(taskId);

        public List<TaskItem> ListarPorLead(int leadId) => _taskRepository.ListarPorLead(leadId);

        public List<TaskItem> ListarVencidas() => _taskRepository.ListarVencidas();

        // Atualizado: agora suporta paginação e ordenação, para a TarefasLista.aspx
        public List<TaskItem> Pesquisar(
            TaskFiltro filtro,
            int pagina, int tamanhoPagina, out int totalRegistos,
            string sortColumn, bool sortAscending,
            int currentUserId, string currentUserRole)
        {
            AplicarAmbitoPermissao(filtro, currentUserId, currentUserRole);
            return _taskRepository.Pesquisar(filtro, pagina, tamanhoPagina, out totalRegistos, sortColumn, sortAscending);
        }

        public List<string> Validar(TaskItem task)
        {
            var erros = new List<string>();

            if (string.IsNullOrWhiteSpace(task.Subject))
                erros.Add("O assunto é obrigatório.");
            else if (task.Subject.Length > 180)
                erros.Add("O assunto não pode ultrapassar 180 caracteres.");

            if (task.AssignedToUserId <= 0)
            {
                erros.Add("O responsável é obrigatório.");
            }
            else
            {
                var responsavel = _userRepository.GetById(task.AssignedToUserId);
                if (responsavel == null || responsavel.Status != "Ativo")
                    erros.Add("O responsável tem de ser um utilizador ativo.");
            }

            if (task.DueDate == default(DateTime))
                erros.Add("A data limite é obrigatória.");

            if (!string.IsNullOrEmpty(task.Priority) && !PrioridadesValidas.Contains(task.Priority))
                erros.Add("Prioridade inválida.");

            if (string.IsNullOrWhiteSpace(task.Status) || !EstadosValidos.Contains(task.Status))
                erros.Add("Estado inválido.");

            var relacionadas = new[] { task.RelatedClientId, task.RelatedLeadId, task.RelatedOpportunityId }
                .Count(id => id.HasValue);
            if (relacionadas > 1)
                erros.Add("A tarefa só pode estar relacionada com um registo (cliente, lead ou oportunidade).");

            return erros;
        }

        public int Criar(TaskItem task, int currentUserId, string currentUserRole)
        {
            var erros = Validar(task);
            if (erros.Any())
                throw new InvalidOperationException(string.Join(" ", erros));

            if (!PodeGerir(task, currentUserId, currentUserRole))
                throw new UnauthorizedAccessException("Sem permissão para criar esta tarefa.");

            task.CreatedBy = currentUserId;
            var id = _taskRepository.Criar(task);

            _auditService.Registar(currentUserId, "Create", "Task", id.ToString(),
                $"Tarefa '{task.Subject}' criada.");

            return id;
        }

        public void Atualizar(TaskItem task, int currentUserId, string currentUserRole)
        {
            var erros = Validar(task);
            if (erros.Any())
                throw new InvalidOperationException(string.Join(" ", erros));

            if (!PodeGerir(task, currentUserId, currentUserRole))
                throw new UnauthorizedAccessException("Sem permissão para editar esta tarefa.");

            if (task.Status == "Concluída" && !task.CompletedDateTime.HasValue)
                task.CompletedDateTime = DateTime.UtcNow;

            task.UpdatedBy = currentUserId;
            _taskRepository.Atualizar(task);

            _auditService.Registar(currentUserId, "Update", "Task", task.TaskId.ToString(),
                $"Tarefa '{task.Subject}' atualizada.");
        }

        public void Eliminar(int taskId, int currentUserId, string currentUserRole)
        {
            var task = _taskRepository.ObterPorId(taskId);
            if (task == null) return;

            if (task.Status == "Concluída" && !EhPerfilPrivilegiado(currentUserRole))
                throw new UnauthorizedAccessException("Tarefas concluídas só podem ser eliminadas por Administrador ou Diretor.");

            if (!PodeGerir(task, currentUserId, currentUserRole))
                throw new UnauthorizedAccessException("Sem permissão para eliminar esta tarefa.");

            _taskRepository.Eliminar(taskId, currentUserId);

            _auditService.Registar(currentUserId, "Delete", "Task", taskId.ToString(),
                $"Tarefa '{task.Subject}' eliminada (lógico).");
        }

        public bool PodeCriar(string currentUserRole) => !EhPerfilConsulta(currentUserRole);

        public bool PodeGerir(TaskItem task, int currentUserId, string currentUserRole)
        {
            if (EhPerfilPrivilegiado(currentUserRole)) return true;
            if (EhPerfilConsulta(currentUserRole)) return false;
            return task.AssignedToUserId == currentUserId;
        }

        public bool PodeEliminar(TaskItem task, int currentUserId, string currentUserRole)
        {
            if (task.Status == "Concluída" && !EhPerfilPrivilegiado(currentUserRole)) return false;
            return PodeGerir(task, currentUserId, currentUserRole);
        }

        public bool TemAmbitoProprios(string currentUserRole) => currentUserRole == "Comercial";

        private void AplicarAmbitoPermissao(TaskFiltro filtro, int currentUserId, string currentUserRole)
        {
            if (EhPerfilPrivilegiado(currentUserRole)) return;
            if (EhPerfilConsulta(currentUserRole)) return;
            filtro.AssignedToUserId = currentUserId;
        }

        private static bool EhPerfilPrivilegiado(string role) =>
            role == "Administrador" || role == "Diretor";

        private static bool EhPerfilConsulta(string role) =>
            role == "Financeiro" || role == "Consulta";
    }
}