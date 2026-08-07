using System;
using System.Collections.Generic;
using System.Linq;
using CRM.Data.Context;
using CRM.Models.Entities.Catalogo;

namespace CRM.Data.Repositories
{
    public class CategoryRepository
    {
        public Category GetById(int categoryId)
        {
            using (var context = new CrmDbContext())
            {
                return context.Categories.Find(categoryId);
            }
        }

        public List<Category> ListarAtivas()
        {
            using (var context = new CrmDbContext())
            {
                return context.Categories
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.Name)
                    .ToList();
            }
        }

        public List<Category> Listar(string pesquisa)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.Categories.AsQueryable();

                if (!string.IsNullOrWhiteSpace(pesquisa))
                {
                    query = query.Where(c => c.Name.Contains(pesquisa));
                }

                return query.OrderBy(c => c.Name).ToList();
            }
        }

        public bool ExisteNome(string name, int? ignorarCategoryId = null)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.Categories.Where(c => c.Name == name);

                if (ignorarCategoryId.HasValue)
                {
                    query = query.Where(c => c.CategoryId != ignorarCategoryId.Value);
                }

                return query.Any();
            }
        }

        public int Criar(Category category)
        {
            using (var context = new CrmDbContext())
            {
                category.CreatedDate = DateTime.UtcNow;
                category.IsActive = true;
                context.Categories.Add(category);
                context.SaveChanges();
                return category.CategoryId;
            }
        }

        public void Atualizar(Category categoryAtualizada)
        {
            using (var context = new CrmDbContext())
            {
                var category = context.Categories.Find(categoryAtualizada.CategoryId);
                if (category == null) return;

                category.Name = categoryAtualizada.Name;
                category.UpdatedDate = DateTime.UtcNow;
                category.UpdatedBy = categoryAtualizada.UpdatedBy;

                context.SaveChanges();
            }
        }

        // Regra: "Listas auxiliares usadas em registos não são eliminadas; são inativadas."
        public void AlternarEstado(int categoryId, int alteradoPor)
        {
            using (var context = new CrmDbContext())
            {
                var category = context.Categories.Find(categoryId);
                if (category == null) return;

                category.IsActive = !category.IsActive;
                category.UpdatedDate = DateTime.UtcNow;
                category.UpdatedBy = alteradoPor;

                context.SaveChanges();
            }
        }
    }
}