using CRM.Data.Context;
using CRM.Models.Entities.ListasAuxiliares;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CRM.Data.Repositories
{
    public class PaymentTermRepository
    {
        public List<PaymentTerm> ListarAtivas()
        {
            using (var context = new CrmDbContext())
            {
                return context.PaymentTerms
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.DaysDue)
                    .ToList();
            }
        }
        // PaymentTermRepository.cs — adicionar aos métodos que já tinha (só ListarAtivas)
        public PaymentTerm GetById(int paymentTermId)
        {
            using (var context = new CrmDbContext())
                return context.PaymentTerms.Find(paymentTermId);
        }

        public List<PaymentTerm> Listar(string pesquisa)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.PaymentTerms.AsQueryable();
                if (!string.IsNullOrWhiteSpace(pesquisa))
                    query = query.Where(t => t.Name.Contains(pesquisa));
                return query.OrderBy(t => t.DaysDue).ToList();
            }
        }

        public bool ExisteNome(string name, int? ignorarPaymentTermId = null)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.PaymentTerms.Where(t => t.Name == name);
                if (ignorarPaymentTermId.HasValue) query = query.Where(t => t.PaymentTermId != ignorarPaymentTermId.Value);
                return query.Any();
            }
        }

        public int Criar(PaymentTerm paymentTerm)
        {
            using (var context = new CrmDbContext())
            {
                paymentTerm.CreatedDate = DateTime.UtcNow;
                paymentTerm.IsActive = true;
                context.PaymentTerms.Add(paymentTerm);
                context.SaveChanges();
                return paymentTerm.PaymentTermId;
            }
        }

        public void Atualizar(PaymentTerm paymentTermAtualizado)
        {
            using (var context = new CrmDbContext())
            {
                var paymentTerm = context.PaymentTerms.Find(paymentTermAtualizado.PaymentTermId);
                if (paymentTerm == null) return;
                paymentTerm.Name = paymentTermAtualizado.Name;
                paymentTerm.DaysDue = paymentTermAtualizado.DaysDue;
                paymentTerm.UpdatedDate = DateTime.UtcNow;
                paymentTerm.UpdatedBy = paymentTermAtualizado.UpdatedBy;
                context.SaveChanges();
            }
        }

        public void AlternarEstado(int paymentTermId, int alteradoPor)
        {
            using (var context = new CrmDbContext())
            {
                var paymentTerm = context.PaymentTerms.Find(paymentTermId);
                if (paymentTerm == null) return;
                paymentTerm.IsActive = !paymentTerm.IsActive;
                paymentTerm.UpdatedDate = DateTime.UtcNow;
                paymentTerm.UpdatedBy = alteradoPor;
                context.SaveChanges();
            }
        }
    }
}