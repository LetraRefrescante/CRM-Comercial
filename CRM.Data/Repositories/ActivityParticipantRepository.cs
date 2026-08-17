using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CRM.Data.Context;
using CRM.Models.Entities.Atividades;

namespace CRM.Data.Repositories
{
    public class ActivityParticipantRepository
    {
        public List<ActivityParticipant> ListarPorAtividade(int activityId)
        {
            using (var context = new CrmDbContext())
            {
                return context.ActivityParticipants
                    .Include(p => p.User)
                    .Where(p => p.ActivityId == activityId)
                    .ToList();
            }
        }

        public void Sincronizar(int activityId, List<ActivityParticipant> participantes)
        {
            using (var context = new CrmDbContext())
            {
                var atuais = context.ActivityParticipants.Where(p => p.ActivityId == activityId).ToList();
                context.ActivityParticipants.RemoveRange(atuais);

                foreach (var participante in participantes)
                {
                    context.ActivityParticipants.Add(new ActivityParticipant
                    {
                        ActivityId = activityId,
                        UserId = participante.UserId,
                        ExternalName = participante.ExternalName,
                        ExternalEmail = participante.ExternalEmail
                    });
                }

                context.SaveChanges();
            }
        }
    }
}