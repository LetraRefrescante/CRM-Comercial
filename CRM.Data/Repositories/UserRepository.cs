using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using CRM.Data.Context;
using CRM.Models.Entities.Seguranca;

namespace CRM.Data.Repositories
{
    public class UserRepository
    {
        public User GetByEmail(string email)
        {
            using (var context = new CrmDbContext())
            {
                return context.Users
                    .Include(u => u.Role)
                    .Where(u => u.Email == email && !u.IsDeleted)
                    .SingleOrDefault();
            }
        }

        public User GetById(int userId)
        {
            using (var context = new CrmDbContext())
            {
                return context.Users
                    .Include(u => u.Role)
                    .Where(u => u.UserId == userId && !u.IsDeleted)
                    .SingleOrDefault();
            }
        }

        public List<User> Listar(string pesquisa = null, int? roleId = null, string status = null)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.Users
                    .Include(u => u.Role)
                    .Where(u => !u.IsDeleted);

                if (!string.IsNullOrWhiteSpace(pesquisa))
                {
                    query = query.Where(u => u.Name.Contains(pesquisa) || u.Email.Contains(pesquisa));
                }

                if (roleId.HasValue)
                {
                    query = query.Where(u => u.RoleId == roleId.Value);
                }

                if (!string.IsNullOrWhiteSpace(status))
                {
                    query = query.Where(u => u.Status == status);
                }

                return query.OrderBy(u => u.Name).ToList();
            }
        }

        public List<User> ListarComerciaisAtivos()
        {
            using (var context = new CrmDbContext())
            {
                return context.Users
                    .Include(u => u.Role)
                    .Where(u => !u.IsDeleted && u.Status == "Ativo" && u.Role.Name == "Comercial")
                    .OrderBy(u => u.Name)
                    .ToList();
            }
        }

        /// <summary>
        /// Devolve o nome de qualquer utilizador (ativo, inativo ou de outro perfil),
        /// ao contrário de ListarComerciaisAtivos(). Usado em ecrãs de histórico
        /// (ex: LeadStatusHistory.ChangedBy) onde quem fez a ação pode já não ser
        /// comercial ativo — um admin, ou alguém entretanto desativado.
        /// </summary>
        public Dictionary<int, string> ObterNomesPorIds(IEnumerable<int> userIds)
        {
            using (var context = new CrmDbContext())
            {
                return context.Users
                    .Where(u => userIds.Contains(u.UserId))
                    .ToDictionary(u => u.UserId, u => u.Name);
            }
        }

        public bool EmailExiste(string email, int? ignorarUserId = null)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.Users.Where(u => u.Email == email && !u.IsDeleted);

                if (ignorarUserId.HasValue)
                {
                    query = query.Where(u => u.UserId != ignorarUserId.Value);
                }

                return query.Any();
            }
        }

        public int Criar(User user)
        {
            using (var context = new CrmDbContext())
            {
                user.CreatedDate = DateTime.UtcNow;
                context.Users.Add(user);
                context.SaveChanges();
                return user.UserId;
            }
        }

        public void Atualizar(User userAtualizado)
        {
            using (var context = new CrmDbContext())
            {
                var user = context.Users.Find(userAtualizado.UserId);
                if (user == null) return;

                user.Name = userAtualizado.Name;
                user.Email = userAtualizado.Email;
                user.RoleId = userAtualizado.RoleId;
                user.Status = userAtualizado.Status;
                user.UpdatedDate = DateTime.UtcNow;
                user.UpdatedBy = userAtualizado.UpdatedBy;
                context.Entry(user).OriginalValues["RowVersion"] = userAtualizado.RowVersion;

                TrySave(context);
            }
        }

        public void AtualizarPassword(int userId, string passwordHash, string passwordSalt)
        {
            using (var context = new CrmDbContext())
            {
                var user = context.Users.Find(userId);
                if (user == null) return;

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.UpdatedDate = DateTime.UtcNow;

                TrySave(context);
            }
        }

        public void AlterarStatus(int userId, string novoStatus, int alteradoPor)
        {
            using (var context = new CrmDbContext())
            {
                var user = context.Users.Find(userId);
                if (user == null) return;

                user.Status = novoStatus;

                if (novoStatus == "Ativo")
                {
                    user.FailedLoginAttempts = 0;
                    user.LockedUntil = null;
                }

                user.UpdatedDate = DateTime.UtcNow;
                user.UpdatedBy = alteradoPor;

                TrySave(context);
            }
        }

        public void EliminarLogico(int userId, int eliminadoPor)
        {
            using (var context = new CrmDbContext())
            {
                var user = context.Users.Find(userId);
                if (user == null) return;

                user.IsDeleted = true;
                user.DeletedDate = DateTime.UtcNow;
                user.DeletedBy = eliminadoPor;

                TrySave(context);
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

                TrySave(context);
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

                TrySave(context);
            }
        }

        private void TrySave(CrmDbContext context)
        {
            context.SaveChanges();
        }
    }
}