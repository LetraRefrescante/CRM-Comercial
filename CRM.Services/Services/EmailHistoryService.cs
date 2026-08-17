using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CRM.Data.Repositories;
using CRM.Models.DTOs;
using CRM.Models.Entities.Atividades;
using CRM.Models.Entities.Notificacoes;
using CRM.Models.Filtros;

namespace CRM.Services
{
    public class EmailHistoryService
    {
        private readonly EmailHistoryRepository _emailHistoryRepository = new EmailHistoryRepository();
        private readonly ContactRepository _contactRepository = new ContactRepository();
        private readonly ProposalRepository _proposalRepository = new ProposalRepository();
        private readonly EmailService _emailService = new EmailService();
        private readonly ActivityService _activityService = new ActivityService();
        private readonly AuditService _auditService = new AuditService();

        public const string StatusEnviado = "Enviado";
        public const string StatusFalhou = "Falhou";

        public bool PodeAcederListaGlobal(string perfil) => perfil == "Administrador" || perfil == "Diretor";

        public bool PodeComporEmail(string perfil) => perfil != "Consulta";

        public List<string> Validar(EmailComporRequest request)
        {
            var erros = new List<string>();

            if (string.IsNullOrWhiteSpace(request.ToAddress))
                erros.Add("O destinatário é obrigatório.");
            else if (!Regex.IsMatch(request.ToAddress.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                erros.Add("O email do destinatário não tem um formato válido.");

            if (string.IsNullOrWhiteSpace(request.Subject))
                erros.Add("O assunto é obrigatório.");

            if (string.IsNullOrWhiteSpace(request.Body))
                erros.Add("O corpo do email é obrigatório.");

            return erros;
        }

        public List<EmailHistory> Pesquisar(
            EmailHistoryFiltro filtro,
            int pagina, int tamanhoPagina, out int totalRegistos,
            string sortColumn, bool sortAscending)
            => _emailHistoryRepository.Pesquisar(filtro, pagina, tamanhoPagina, out totalRegistos, sortColumn, sortAscending);

        public bool Enviar(EmailComporRequest request, int userId, string perfil)
        {
            string status = StatusEnviado;
            string failureReason = null;

            try
            {
                _emailService.Enviar(request.ToAddress.Trim(), request.Subject.Trim(), request.Body);
            }
            catch (NotImplementedException)
            {
                status = StatusFalhou;
                failureReason = "Serviço de email por implementar (SMTP não configurado).";
            }
            catch (Exception)
            {
                status = StatusFalhou;
                failureReason = "Falha no envio do email.";
            }

            var history = new EmailHistory
            {
                ToAddress = request.ToAddress.Trim(),
                Subject = request.Subject.Trim(),
                Body = request.Body,
                EmailTemplateId = request.EmailTemplateId,
                RelatedClientId = request.RelatedEntityType == "Client" ? request.RelatedEntityId : null,
                RelatedContactId = request.RelatedEntityType == "Contact" ? request.RelatedEntityId : null,
                RelatedLeadId = request.RelatedEntityType == "Lead" ? request.RelatedEntityId : null,
                RelatedOpportunityId = request.RelatedEntityType == "Opportunity" ? request.RelatedEntityId : null,
                RelatedProposalId = request.RelatedEntityType == "Proposal" ? request.RelatedEntityId : null,
                SentByUserId = userId,
                Status = status,
                FailureReason = failureReason
            };

            int historyId = _emailHistoryRepository.Criar(history);

            CriarAtividadeRelacionada(request, userId, perfil);

            _auditService.Registar(userId, status == StatusEnviado ? "EnviarEmail" : "FalhaEnvioEmail",
                "EmailHistory", historyId.ToString(), request.Subject);

            return status == StatusEnviado;
        }

        private void CriarAtividadeRelacionada(EmailComporRequest request, int userId, string perfil)
        {
            int? relatedClientId = null;
            int? relatedLeadId = null;
            int? relatedOpportunityId = null;

            switch (request.RelatedEntityType)
            {
                case "Client":
                    relatedClientId = request.RelatedEntityId;
                    break;
                case "Lead":
                    relatedLeadId = request.RelatedEntityId;
                    break;
                case "Opportunity":
                    relatedOpportunityId = request.RelatedEntityId;
                    break;
                case "Contact":
                    relatedClientId = request.RelatedEntityId.HasValue
                        ? _contactRepository.GetById(request.RelatedEntityId.Value)?.ClientId
                        : null;
                    break;
                case "Proposal":
                    relatedClientId = request.RelatedEntityId.HasValue
                        ? _proposalRepository.GetById(request.RelatedEntityId.Value)?.ClientId
                        : null;
                    break;
            }

            try
            {
                _activityService.Criar(new Activity
                {
                    Type = "Email",
                    Subject = $"Email: {request.Subject.Trim()}",
                    RelatedClientId = relatedClientId,
                    RelatedLeadId = relatedLeadId,
                    RelatedOpportunityId = relatedOpportunityId,
                    AssignedToUserId = userId,
                    StartDateTime = DateTime.Now,
                    Status = "Concluída",
                    CompletedDateTime = DateTime.UtcNow,
                    Description = $"Email enviado para {request.ToAddress.Trim()}."
                }, userId, perfil);
            }
            catch
            {
                // Não bloqueia o envio se a atividade falhar a criar.
            }
        }
    }
}