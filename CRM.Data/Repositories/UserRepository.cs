using System;
using System.Linq;
using CRM.Data.Context;
using CRM.Models.Entities;

namespace CRM.Data.Repositories
{
    public class UserRepository
    {
        public User GetByEmail(string email)
        {
            using (var context = new CrmDbContext())
            {
                return context.Users
                    .Where(u => u.Email == email && !u.IsDeleted)
                    .SingleOrDefault();
            }
        }

        public User GetById(int userId)
        {
            using (var context = new CrmDbContext())
            {
                return context.Users
                    .Where(u => u.UserId == userId && !u.IsDeleted)
                    .SingleOrDefault();
            }
        }

        public void RegisterFailedLogin(int userId, int maxAttempts, int lockoutMinutes)
        {
            using (var context = new CrmDbContext())
            {
                var user = context.Users.Find(userId);
                if (user == null) return;

                user.FailedLoginAttempts++;

                if (user.FailedLoginAttempts >= maxAttempts)
                {
                    user.LockedUntil = DateTime.UtcNow.AddMinutes(lockoutMinutes);
                }

                context.SaveChanges();
            }
        }

        public void RegisterSuccessfulLogin(int userId)
        {
            using (var context = new CrmDbContext())
            {
                var user = context.Users.Find(userId);
                if (user == null) return;

                user.FailedLoginAttempts = 0;
                user.LockedUntil = null;
                user.LastLoginDate = DateTime.UtcNow;

                context.SaveChanges();
            }
        }
    }
}