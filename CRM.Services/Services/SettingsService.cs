using System.Collections.Generic;
using CRM.Data.Repositories;
using CRM.Models.Entities.ListasAuxiliares;

namespace CRM.Services
{
    public class SettingsService
    {
        private readonly SettingsRepository _settingsRepository = new SettingsRepository();
        private readonly AuditService _auditService = new AuditService();
        private readonly PermissionService _permissionService = new PermissionService();

        private const string Modulo = "Configuracoes";

        public bool PodeGerir(string perfil) => _permissionService.ObterNivel(perfil, Modulo) == NivelAcesso.Total;
        public bool PodeConsultar(string perfil) => _permissionService.ObterNivel(perfil, Modulo) >= NivelAcesso.Consulta;

        public Settings ObterConfiguracaoAtual() => _settingsRepository.ObterConfiguracaoAtual();

        public List<string> Validar(Settings settings)
        {
            var erros = new List<string>();

            if (string.IsNullOrWhiteSpace(settings.CompanyName))
                erros.Add("O nome da empresa é obrigatório.");

            if (string.IsNullOrWhiteSpace(settings.Currency) || settings.Currency.Trim().Length != 3)
                erros.Add("A moeda tem de ser um código de 3 letras (ex.: EUR).");

            if (string.IsNullOrWhiteSpace(settings.TimeZone))
                erros.Add("O fuso horário é obrigatório.");

            if (settings.AlertDaysLeads < 0 || settings.AlertDaysOpportunities < 0 || settings.AlertDaysProposals < 0)
                erros.Add("Os dias de alerta não podem ser negativos.");

            if (settings.MaxFailedLoginAttempts < 1)
                erros.Add("O número máximo de tentativas falhadas tem de ser pelo menos 1.");

            if (settings.AccountLockoutMinutes < 1)
                erros.Add("O tempo de bloqueio de conta tem de ser pelo menos 1 minuto.");

            if (settings.SessionTimeoutMinutes < 1)
                erros.Add("O tempo de expiração de sessão tem de ser pelo menos 1 minuto.");

            return erros;
        }

        public void Atualizar(Settings settings)
        {
            _settingsRepository.Atualizar(settings);
            _auditService.Registar(settings.UpdatedBy, "Update", "Settings", "1", "Configurações gerais atualizadas.");
        }
    }
}