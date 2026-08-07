using CRM.Data.Repositories;
using CRM.Models.Entities.Seguranca;
using System;
using System.Collections.Generic;

namespace CRM.Business.Services
{
    /// <summary>
    /// Regista ações relevantes (criação, alteração, eliminação) na tabela AuditLogs.
    /// </summary>
    public class AuditService
    {
        private readonly AuditLogRepository _auditLogRepository = new AuditLogRepository();

        public void Registar(int? userId, string action, string entityName, string entityId, string details = null)
        {
            var log = new AuditLog
            {
                UserId = userId,
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                Details = details,
                IpAddress = ObterIpAtual(),
                CreatedDate = DateTime.UtcNow
            };

            _auditLogRepository.Criar(log);
        }

        private string ObterIpAtual()
        {
            try
            {
                return System.Web.HttpContext.Current?.Request?.UserHostAddress;
            }
            catch
            {
                return null;
            }
        }

        public List<AuditLog> Listar(string entityName, string entityId)
            => _auditLogRepository.ListarPorEntidade(entityName, entityId);
    }
}