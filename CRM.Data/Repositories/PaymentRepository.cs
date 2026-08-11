using System;
using System.Collections.Generic;
using System.Linq;
using CRM.Data.Context;
using CRM.Models.Entities.Vendas;

namespace CRM.Data.Repositories
{
    public class PaymentRepository
    {
        public List<Payment> ListarPorVenda(int saleId)
        {
            using (var context = new CrmDbContext())
            {
                return context.Payments
                    .Where(p => p.SaleId == saleId && !p.IsDeleted)
                    .OrderByDescending(p => p.PaymentDate)
                    .ThenByDescending(p => p.PaymentId)
                    .ToList();
            }
        }

        public decimal TotalPagoPorVenda(int saleId)
        {
            using (var context = new CrmDbContext())
            {
                return context.Payments
                    .Where(p => p.SaleId == saleId && !p.IsDeleted)
                    .Select(p => (decimal?)p.Amount)
                    .Sum() ?? 0;
            }
        }

        public Payment Criar(Payment payment)
        {
            using (var context = new CrmDbContext())
            {
                payment.CreatedDate = DateTime.UtcNow;
                context.Payments.Add(payment);
                context.SaveChanges();
                return payment;
            }
        }

        public void EliminarLogico(int paymentId, int eliminadoPor)
        {
            using (var context = new CrmDbContext())
            {
                var payment = context.Payments.Find(paymentId);
                if (payment == null) return;

                payment.IsDeleted = true;
                payment.DeletedDate = DateTime.UtcNow;
                payment.DeletedBy = eliminadoPor;

                context.SaveChanges();
            }
        }
    }
}