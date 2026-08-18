using System;
using System.Linq;
using CRM.Data.Context;
using CRM.Models.Entities.ListasAuxiliares;

namespace CRM.Data.Repositories
{
    public class SettingsRepository
    {
        public Settings Obter()
        {
            using (var context = new CrmDbContext())
            {
                var settings = context.Settings
                    .OrderBy(s => s.SettingId)
                    .FirstOrDefault();

                if (settings != null)
                    return settings;

                return new Settings
                {
                    Currency = "EUR",
                    TimeZone = "Europe/Lisbon",

                    AlertDaysLeads = 7,
                    AlertDaysOpportunities = 7,
                    AlertDaysProposals = 7,

                    MaxFailedLoginAttempts = 5,
                    AccountLockoutMinutes = 15,
                    SessionTimeoutMinutes = 30
                };
            }
        }

        public Settings ObterConfiguracaoAtual()
        {
            return Obter();
        }

        public void Guardar(Settings settingsAtualizados)
        {
            using (var context = new CrmDbContext())
            {
                var existente = context.Settings
                    .OrderBy(s => s.SettingId)
                    .FirstOrDefault();

                if (existente == null)
                {
                    settingsAtualizados.UpdatedDate = DateTime.UtcNow;
                    context.Settings.Add(settingsAtualizados);
                }
                else
                {
                    existente.CompanyName = settingsAtualizados.CompanyName;
                    existente.Currency = settingsAtualizados.Currency;
                    existente.TimeZone = settingsAtualizados.TimeZone;

                    existente.AlertDaysLeads = settingsAtualizados.AlertDaysLeads;
                    existente.AlertDaysOpportunities = settingsAtualizados.AlertDaysOpportunities;
                    existente.AlertDaysProposals = settingsAtualizados.AlertDaysProposals;

                    existente.MaxFailedLoginAttempts = settingsAtualizados.MaxFailedLoginAttempts;
                    existente.AccountLockoutMinutes = settingsAtualizados.AccountLockoutMinutes;
                    existente.SessionTimeoutMinutes = settingsAtualizados.SessionTimeoutMinutes;

                    existente.UpdatedDate = DateTime.UtcNow;
                    existente.UpdatedBy = settingsAtualizados.UpdatedBy;
                }

                context.SaveChanges();
            }
        }
    }
}