using System.Linq;
using CRM.Data.Context;
using CRM.Models.Entities.Seguranca;

namespace CRM.Data.Repositories
{
    public class PasswordResetTokenRepository
    {
        public PasswordResetToken ObterPorToken(string token)
        {
            using (var context = new CrmDbContext())
            {
                return context.PasswordResetTokens.SingleOrDefault(t => t.Token == token);
            }
        }

        public void InvalidarTokensAtivos(int userId)
        {
            using (var context = new CrmDbContext())
            {
                var tokensAtivos = context.PasswordResetTokens
                    .Where(t => t.UserId == userId && !t.Utilizado)
                    .ToList();

                foreach (var t in tokensAtivos)
                {
                    t.Utilizado = true;
                }

                context.SaveChanges();
            }
        }

        public void Criar(PasswordResetToken token)
        {
            using (var context = new CrmDbContext())
            {
                context.PasswordResetTokens.Add(token);
                context.SaveChanges();
            }
        }

        public void MarcarComoUtilizado(int id)
        {
            using (var context = new CrmDbContext())
            {
                var token = context.PasswordResetTokens.Find(id);
                if (token == null) return;

                token.Utilizado = true;
                context.SaveChanges();
            }
        }
    }
}