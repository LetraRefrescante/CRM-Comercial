using System.Collections.Generic;
using CRM.Data.Repositories;
using CRM.Models.Entities.ListasAuxiliares;

namespace CRM.Services
{
    public class SettingsService
    {
        private readonly SettingsRepository _settingsRepository =
            new SettingsRepository();

        public bool PodeGerir(string perfil)
            => perfil == "Administrador";

        public Settings Obter()
            => _settingsRepository.Obter();

        public List<string> Validar(Settings settings)
        {
            var erros = new List<string>();

            if (string.IsNullOrWhiteSpace(settings.CompanyName))
                erros.Add("O nome da empresa é obrigatório.");

            if (string.IsNullOrWhiteSpace(settings.Currency))
                erros.Add("A moeda é obrigatória.");

            if (string.IsNullOrWhiteSpace(settings.TimeZone))
                erros.Add("O fuso horário é obrigatório.");

            if (settings.AlertDaysLeads < 0)
                erros.Add("Os dias de alerta de Leads não podem ser negativos.");

            if (settings.AlertDaysOpportunities < 0)
                erros.Add("Os dias de alerta de Oportunidades não podem ser negativos.");

            if (settings.AlertDaysProposals < 0)
                erros.Add("Os dias de alerta de Propostas não podem ser negativos.");

            return erros;
        }

        public void Guardar(Settings settings, int userId)
        {
            settings.UpdatedBy = userId;

            _settingsRepository.Guardar(settings);
        }
    }
}