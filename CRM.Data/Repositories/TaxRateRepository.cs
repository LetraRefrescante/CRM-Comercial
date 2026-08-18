using CRM.Data.Context;
using CRM.Models.Entities.Catalogo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CRM.Data.Repositories
{
    public class TaxRateRepository
    {
        public List<TaxRate> ListarAtivas()
        {
            using (var context = new CrmDbContext())
            {
                return context.TaxRates
                    .Where(t => t.IsActive)
                    .OrderBy(t => t.Percentage)
                    .ToList();
            }
        }
        public List<TaxRate> ListarTodas()
        {
            using (var context = new CrmDbContext())
            {
                return context.TaxRates
                    .OrderBy(t => t.Percentage)
                    .ToList();
            }
        }

        public TaxRate GetById(int taxRateId)
        {
            using (var context = new CrmDbContext())
            {
                return context.TaxRates.Find(taxRateId);
            }
        }
        public List<TaxRate> Listar(string pesquisa)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.TaxRates.AsQueryable();
                if (!string.IsNullOrWhiteSpace(pesquisa))
                    query = query.Where(t => t.Name.Contains(pesquisa));
                return query.OrderBy(t => t.Percentage).ToList();
            }
        }

        public bool ExisteNome(string name, int? ignorarTaxRateId = null)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.TaxRates.Where(t => t.Name == name);
                if (ignorarTaxRateId.HasValue) query = query.Where(t => t.TaxRateId != ignorarTaxRateId.Value);
                return query.Any();
            }
        }

        public int Criar(TaxRate taxRate)
        {
            using (var context = new CrmDbContext())
            {
                taxRate.CreatedDate = DateTime.UtcNow;
                taxRate.IsActive = true;
                context.TaxRates.Add(taxRate);
                context.SaveChanges();
                return taxRate.TaxRateId;
            }
        }

        public void Atualizar(TaxRate taxRateAtualizada)
        {
            using (var context = new CrmDbContext())
            {
                var taxRate = context.TaxRates.Find(taxRateAtualizada.TaxRateId);
                if (taxRate == null) return;
                taxRate.Name = taxRateAtualizada.Name;
                taxRate.Percentage = taxRateAtualizada.Percentage;
                taxRate.UpdatedDate = DateTime.UtcNow;
                taxRate.UpdatedBy = taxRateAtualizada.UpdatedBy;
                context.SaveChanges();
            }
        }

        public void AlternarEstado(int taxRateId, int alteradoPor)
        {
            using (var context = new CrmDbContext())
            {
                var taxRate = context.TaxRates.Find(taxRateId);
                if (taxRate == null) return;
                taxRate.IsActive = !taxRate.IsActive;
                taxRate.UpdatedDate = DateTime.UtcNow;
                taxRate.UpdatedBy = alteradoPor;
                context.SaveChanges();
            }
        }
    }
}