using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CRM.Data.Context;
using CRM.Models.Entities.Catalogo;

namespace CRM.Data.Repositories
{
    public class PriceTableItemRepository
    {
        public PriceTableItem GetById(int priceTableItemId)
        {
            using (var context = new CrmDbContext())
            {
                return context.PriceTableItems
                    .Include(i => i.Product)
                    .SingleOrDefault(i => i.PriceTableItemId == priceTableItemId);
            }
        }

        public List<PriceTableItem> ListarPorTabela(int priceTableId)
        {
            using (var context = new CrmDbContext())
            {
                return context.PriceTableItems
                    .Include(i => i.Product)
                    .Include(i => i.Product.Category)
                    .Where(i => i.PriceTableId == priceTableId)
                    .OrderBy(i => i.Product.Name)
                    .ToList();
            }
        }

        public bool ExisteProdutoNaTabela(int priceTableId, int productId, int? ignorarPriceTableItemId = null)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.PriceTableItems
                    .Where(i => i.PriceTableId == priceTableId && i.ProductId == productId);

                if (ignorarPriceTableItemId.HasValue)
                {
                    query = query.Where(i => i.PriceTableItemId != ignorarPriceTableItemId.Value);
                }

                return query.Any();
            }
        }

        public int Criar(PriceTableItem item)
        {
            using (var context = new CrmDbContext())
            {
                item.CreatedDate = DateTime.UtcNow;
                context.PriceTableItems.Add(item);
                context.SaveChanges();
                return item.PriceTableItemId;
            }
        }

        public void Atualizar(PriceTableItem itemAtualizado)
        {
            using (var context = new CrmDbContext())
            {
                var item = context.PriceTableItems.Find(itemAtualizado.PriceTableItemId);
                if (item == null) return;

                item.Price = itemAtualizado.Price;
                item.UpdatedDate = DateTime.UtcNow;
                item.UpdatedBy = itemAtualizado.UpdatedBy;

                context.SaveChanges();
            }
        }

        public void Eliminar(int priceTableItemId)
        {
            using (var context = new CrmDbContext())
            {
                var item = context.PriceTableItems.Find(priceTableItemId);
                if (item == null) return;

                context.PriceTableItems.Remove(item);
                context.SaveChanges();
            }
        }
    }
}