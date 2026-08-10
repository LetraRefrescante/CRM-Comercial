using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CRM.Data.Context;
using CRM.Models.Entities.Atividades;

namespace CRM.Data.Repositories
{
    public class ActivityRepository
    {
        public List<Activity> ListarPorLead(int leadId)
        {
            using (var context = new CrmDbContext())
            {
                return context.Activities
                    .Include(a => a.AssignedTo)
                    .Where(a => a.RelatedLeadId == leadId && !a.IsDeleted)
                    .OrderByDescending(a => a.StartDateTime)
                    .ToList();
            }
        }

        public int Criar(Activity activity)
        {
            using (var context = new CrmDbContext())
            {
                activity.CreatedDate = DateTime.UtcNow;
                context.Activities.Add(activity);
                context.SaveChanges();
                return activity.ActivityId;
            }
        }
    }
}