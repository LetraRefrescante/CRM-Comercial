using System;
using System.Collections.Generic;
using System.Linq;
using CRM.Data.Context;
using CRM.Models.Entities.ListasAuxiliares;

namespace CRM.Data.Repositories
{
    public class PaymentTermRepository
    {
        public PaymentTerm GetById(int id)
        {
            using (var context = new CrmDbContext())
            {
                return context.PaymentTerms.Find(id);
            }
        }
        public List<PaymentTerm> ListarAtivas()
        {
            using (var context = new CrmDbContext())
            {
                return context.PaymentTerms
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.Name)
                    .ToList();
            }
        }
        public List<PaymentTerm> Listar(string pesquisa, bool incluirInativos)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.PaymentTerms.AsQueryable();

                if (!incluirInativos)
                    query = query.Where(p => p.IsActive);

                if (!string.IsNullOrWhiteSpace(pesquisa))
                    query = query.Where(p => p.Name.Contains(pesquisa));

                return query.OrderBy(p => p.Name).ToList();
            }
        }

        public bool ExisteNome(string name, int? ignorarPaymentTermId = null)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.PaymentTerms.Where(p => p.Name == name);
                if (ignorarPaymentTermId.HasValue) query = query.Where(p => p.PaymentTermId != ignorarPaymentTermId.Value);
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
                var existente = context.PaymentTerms.Find(paymentTermAtualizado.PaymentTermId);
                if (existente == null) return;

                existente.Name = paymentTermAtualizado.Name;
                existente.DaysDue = paymentTermAtualizado.DaysDue;
                existente.UpdatedDate = DateTime.UtcNow;
                existente.UpdatedBy = paymentTermAtualizado.UpdatedBy;

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