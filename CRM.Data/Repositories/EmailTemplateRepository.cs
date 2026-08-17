using System;
using System.Collections.Generic;
using System.Linq;
using CRM.Data.Context;
using CRM.Models.Entities.Notificacoes;

namespace CRM.Data.Repositories
{
    public class EmailTemplateRepository
    {
        public List<EmailTemplate> Listar(string pesquisa)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.EmailTemplates.AsQueryable();
                if (!string.IsNullOrWhiteSpace(pesquisa))
                    query = query.Where(t => t.Name.Contains(pesquisa));

                return query.OrderBy(t => t.Name).ToList();
            }
        }

        public List<EmailTemplate> ListarAtivos()
        {
            using (var context = new CrmDbContext())
            {
                return context.EmailTemplates.Where(t => t.IsActive).OrderBy(t => t.Name).ToList();
            }
        }

        public EmailTemplate GetById(int emailTemplateId)
        {
            using (var context = new CrmDbContext())
            {
                return context.EmailTemplates.Find(emailTemplateId);
            }
        }

        public bool ExisteNome(string name, int? ignorarEmailTemplateId = null)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.EmailTemplates.Where(t => t.Name == name);
                if (ignorarEmailTemplateId.HasValue)
                    query = query.Where(t => t.EmailTemplateId != ignorarEmailTemplateId.Value);
                return query.Any();
            }
        }

        public int Criar(EmailTemplate template)
        {
            using (var context = new CrmDbContext())
            {
                template.CreatedDate = DateTime.UtcNow;
                context.EmailTemplates.Add(template);
                context.SaveChanges();
                return template.EmailTemplateId;
            }
        }

        public void Atualizar(EmailTemplate template)
        {
            using (var context = new CrmDbContext())
            {
                var existente = context.EmailTemplates.Find(template.EmailTemplateId);
                if (existente == null) return;

                existente.Name = template.Name;
                existente.Subject = template.Subject;
                existente.Body = template.Body;
                existente.UpdatedDate = DateTime.UtcNow;
                existente.UpdatedBy = template.UpdatedBy;

                context.SaveChanges();
            }
        }

        public void AlternarEstado(int emailTemplateId, int alteradoPor)
        {
            using (var context = new CrmDbContext())
            {
                var template = context.EmailTemplates.Find(emailTemplateId);
                if (template == null) return;

                template.IsActive = !template.IsActive;
                template.UpdatedDate = DateTime.UtcNow;
                template.UpdatedBy = alteradoPor;
                context.SaveChanges();
            }
        }
    }
}