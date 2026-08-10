using System.Collections.Generic;
using CRM.Data.Repositories;
using CRM.Models.Entities.Atividades;

namespace CRM.Services
{
    public class ActivityService
    {
        private readonly ActivityRepository _activityRepository = new ActivityRepository();

        public List<Activity> ListarPorLead(int leadId) => _activityRepository.ListarPorLead(leadId);

        public List<string> Validar(Activity activity)
        {
            var erros = new List<string>();

            if (string.IsNullOrWhiteSpace(activity.Subject))
                erros.Add("O assunto é obrigatório.");

            if (activity.EndDateTime.HasValue && activity.EndDateTime.Value < activity.StartDateTime)
                erros.Add("A data de fim tem de ser posterior à data de início.");

            return erros;
        }

        public int Criar(Activity activity) => _activityRepository.Criar(activity);
    }
}