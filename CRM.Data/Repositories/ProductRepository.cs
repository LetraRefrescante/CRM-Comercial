using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using CRM.Data.Context;
using CRM.Models.Entities.Catalogo;

namespace CRM.Data.Repositories
{
    public class ProductRepository
    {
        public Product GetById(int productId)
        {
            using (var context = new CrmDbContext())
            {
                return context.Products
                    .Include(p => p.Category)
                    .Include(p => p.TaxRate)
                    .Where(p => p.ProductId == productId && !p.IsDeleted)
                    .SingleOrDefault();
            }
        }

        public List<Product> Listar(
            string pesquisa,
            string type,
            int? categoryId,
            bool? isActive,
            int pagina,
            int tamanhoPagina,
            out int totalRegistos,
            string sortColumn = "Name",
            bool sortAscending = true)
        {
            using (var context = new CrmDbContext())
            {
                var query = ConstruirQuery(context, pesquisa, type, categoryId, isActive);

                totalRegistos = query.Count();

                return AplicarOrdenacao(query, sortColumn, sortAscending)
                    .Skip((pagina - 1) * tamanhoPagina)
                    .Take(tamanhoPagina)
                    .ToList();
            }
        }

        private IQueryable<Product> ConstruirQuery(
            CrmDbContext context,
            string pesquisa,
            string type,
            int? categoryId,
            bool? isActive)
        {
            var query = context.Products
                .Include(p => p.Category)
                .Include(p => p.TaxRate)
                .Where(p => !p.IsDeleted);

            if (!string.IsNullOrWhiteSpace(pesquisa))
            {
                query = query.Where(p =>
                    p.Code.Contains(pesquisa) ||
                    p.Name.Contains(pesquisa));
            }

            if (!string.IsNullOrWhiteSpace(type))
            {
                query = query.Where(p => p.Type == type);
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (isActive.HasValue)
            {
                query = query.Where(p => p.IsActive == isActive.Value);
            }

            return query;
        }
        private IQueryable<Product> AplicarOrdenacao(IQueryable<Product> query, string sortColumn, bool sortAscending)
        {
            switch (sortColumn)
            {
                case "Code":
                    return sortAscending ? query.OrderBy(p => p.Code) : query.OrderByDescending(p => p.Code);
                case "Type":
                    return sortAscending ? query.OrderBy(p => p.Type) : query.OrderByDescending(p => p.Type);
                case "Category":
                    return sortAscending ? query.OrderBy(p => p.Category.Name) : query.OrderByDescending(p => p.Category.Name);
                case "BasePrice":
                    return sortAscending ? query.OrderBy(p => p.BasePrice) : query.OrderByDescending(p => p.BasePrice);
                case "IsActive":
                    return sortAscending ? query.OrderBy(p => p.IsActive) : query.OrderByDescending(p => p.IsActive);
                case "Name":
                default:
                    return sortAscending ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name);
            }
        }

        public bool ExisteCodigo(string code, int? ignorarProductId = null)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.Products.Where(p => !p.IsDeleted && p.Code == code);

                if (ignorarProductId.HasValue)
                {
                    query = query.Where(p => p.ProductId != ignorarProductId.Value);
                }

                return query.Any();
            }
        }

        public int Criar(Product product)
        {
            using (var context = new CrmDbContext())
            {
                product.CreatedDate = DateTime.UtcNow;
                context.Products.Add(product);
                context.SaveChanges();
                return product.ProductId;
            }
        }

        public void Atualizar(Product productAtualizado)
        {
            using (var context = new CrmDbContext())
            {
                try
                {
                    var product = context.Products.Find(productAtualizado.ProductId);
                    if (product == null) return;

                    product.Code = productAtualizado.Code;
                    product.Type = productAtualizado.Type;
                    product.Name = productAtualizado.Name;
                    product.CategoryId = productAtualizado.CategoryId;
                    product.Description = productAtualizado.Description;
                    product.BasePrice = productAtualizado.BasePrice;
                    product.TaxRateId = productAtualizado.TaxRateId;
                    product.Unit = productAtualizado.Unit;
                    product.IsActive = productAtualizado.IsActive;
                    product.UpdatedDate = DateTime.UtcNow;
                    product.UpdatedBy = productAtualizado.UpdatedBy;

                    context.Entry(product).OriginalValues["RowVersion"] = productAtualizado.RowVersion;

                    context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }
        }
        public void EliminarLogico(int productId, int eliminadoPor)
        {
            using (var context = new CrmDbContext())
            {
                var product = context.Products.Find(productId);
                if (product == null) return;

                product.IsDeleted = true;
                product.DeletedDate = DateTime.UtcNow;
                product.DeletedBy = eliminadoPor;
                product.IsActive = false;

                context.SaveChanges();
            }
        }
        public List<Product> ListarAtivos()
        {
            using (var context = new CrmDbContext())
            {
                return context.Products
                    .Where(p => p.IsActive && !p.IsDeleted)
                    .OrderBy(p => p.Name)
                    .ToList();
            }
        }
    }
}