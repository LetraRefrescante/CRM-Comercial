using System;
using System.Collections.Generic;
using System.Linq;
using CRM.Data.Context;
using CRM.Models.Entities.Notificacoes;

namespace CRM.Data.Repositories
{
    public class NotificationRepository
    {
        public List<Notification> ListarPorUtilizador(int userId, bool incluirArquivadas)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.Notifications.Where(n => n.UserId == userId);
                if (!incluirArquivadas)
                    query = query.Where(n => !n.IsArchived);

                return query.OrderByDescending(n => n.CreatedDate).ToList();
            }
        }

        public int ContarNaoLidas(int userId)
        {
            using (var context = new CrmDbContext())
            {
                return context.Notifications.Count(n => n.UserId == userId && !n.IsRead && !n.IsArchived);
            }
        }

        public Notification GetById(int notificationId)
        {
            using (var context = new CrmDbContext())
            {
                return context.Notifications.Find(notificationId);
            }
        }

        public int Criar(Notification notification)
        {
            using (var context = new CrmDbContext())
            {
                notification.CreatedDate = DateTime.UtcNow;
                context.Notifications.Add(notification);
                context.SaveChanges();
                return notification.NotificationId;
            }
        }

        public void MarcarComoLida(int notificationId)
        {
            using (var context = new CrmDbContext())
            {
                var notification = context.Notifications.Find(notificationId);
                if (notification == null || notification.IsRead) return;

                notification.IsRead = true;
                notification.ReadDate = DateTime.UtcNow;
                context.SaveChanges();
            }
        }

        public void Arquivar(int notificationId)
        {
            using (var context = new CrmDbContext())
            {
                var notification = context.Notifications.Find(notificationId);
                if (notification == null) return;

                notification.IsArchived = true;
                notification.ArchivedDate = DateTime.UtcNow;
                context.SaveChanges();
            }
        }
    }
}