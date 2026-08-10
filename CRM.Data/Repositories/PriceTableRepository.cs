using System;
using System.Collections.Generic;
using System.Linq;
using CRM.Data.Context;
using CRM.Models.Entities.Catalogo;

namespace CRM.Data.Repositories
{
    public class PriceTableRepository
    {
        public PriceTable GetById(int priceTableId)
        {
            using (var context = new CrmDbContext())
            {
                return context.PriceTables.Find(priceTableId);
            }
        }

        public List<PriceTable> ListarAtivas()
        {
            using (var context = new CrmDbContext())
            {
                return context.PriceTables
                    .Where(t => t.IsActive)
                    .OrderBy(t => t.Name)
                    .ToList();
            }
        }

        public List<PriceTable> Listar(string pesquisa)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.PriceTables.AsQueryable();

                if (!string.IsNullOrWhiteSpace(pesquisa))
                {
                    query = query.Where(t => t.Name.Contains(pesquisa));
                }

                return query.OrderByDescending(t => t.IsDefault).ThenBy(t => t.Name).ToList();
            }
        }

        public bool ExisteNome(string name, int? ignorarPriceTableId = null)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.PriceTables.Where(t => t.Name == name);

                if (ignorarPriceTableId.HasValue)
                {
                    query = query.Where(t => t.PriceTableId != ignorarPriceTableId.Value);
                }

                return query.Any();
            }
        }

        public int Criar(PriceTable priceTable)
        {
            using (var context = new CrmDbContext())
            {
                priceTable.CreatedDate = DateTime.UtcNow;
                priceTable.IsActive = true;

                if (priceTable.IsDefault)
                {
                    DesmarcarPredefinidaAtual(context, priceTable.CreatedBy);
                }

                context.PriceTables.Add(priceTable);
                context.SaveChanges();
                return priceTable.PriceTableId;
            }
        }

        public void Atualizar(PriceTable priceTableAtualizada)
        {
            using (var context = new CrmDbContext())
            {
                var priceTable = context.PriceTables.Find(priceTableAtualizada.PriceTableId);
                if (priceTable == null) return;

                if (priceTableAtualizada.IsDefault && !priceTable.IsDefault)
                {
                    DesmarcarPredefinidaAtual(context, priceTableAtualizada.UpdatedBy, priceTableAtualizada.PriceTableId);
                }

                priceTable.Name = priceTableAtualizada.Name;
                priceTable.IsDefault = priceTableAtualizada.IsDefault;
                priceTable.UpdatedDate = DateTime.UtcNow;
                priceTable.UpdatedBy = priceTableAtualizada.UpdatedBy;

                context.SaveChanges();
            }
        }

        // Regra: "Listas auxiliares usadas em registos não são eliminadas; são inativadas."
        public void AlternarEstado(int priceTableId, int alteradoPor)
        {
            using (var context = new CrmDbContext())
            {
                var priceTable = context.PriceTables.Find(priceTableId);
                if (priceTable == null) return;

                priceTable.IsActive = !priceTable.IsActive;
                priceTable.UpdatedDate = DateTime.UtcNow;
                priceTable.UpdatedBy = alteradoPor;

                context.SaveChanges();
            }
        }

        private void DesmarcarPredefinidaAtual(CrmDbContext context, int? alteradoPor, int? ignorarPriceTableId = null)
        {
            var query = context.PriceTables.Where(t => t.IsDefault);

            if (ignorarPriceTableId.HasValue)
            {
                query = query.Where(t => t.PriceTableId != ignorarPriceTableId.Value);
            }

            foreach (var atual in query)
            {
                atual.IsDefault = false;
                atual.UpdatedDate = DateTime.UtcNow;
                atual.UpdatedBy = alteradoPor;
            }
        }
    }
}