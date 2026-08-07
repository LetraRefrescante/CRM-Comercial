using System.Collections.Generic;
using CRM.Data.Repositories;
using CRM.Models.Entities.Catalogo;

namespace CRM.Services
{
    public class CategoryService
    {
        private readonly CategoryRepository _categoryRepository = new CategoryRepository();
        public bool PodeGerir(string perfil) => perfil == "Administrador" || perfil == "Diretor";

        public List<string> Validar(Category category, bool nomeJaExiste)
        {
            var erros = new List<string>();

            if (string.IsNullOrWhiteSpace(category.Name))
                erros.Add("O nome da categoria é obrigatório.");
            else if (category.Name.Trim().Length > 100)
                erros.Add("O nome não pode exceder 100 caracteres.");

            if (nomeJaExiste)
                erros.Add("Já existe uma categoria com este nome.");

            return erros;
        }

        public Category GetById(int categoryId) => _categoryRepository.GetById(categoryId);

        public List<Category> ListarAtivas() => _categoryRepository.ListarAtivas();

        public List<Category> Listar(string pesquisa) => _categoryRepository.Listar(pesquisa);

        public bool ExisteNome(string name, int? ignorarCategoryId = null) => _categoryRepository.ExisteNome(name, ignorarCategoryId);

        public int Criar(Category category) => _categoryRepository.Criar(category);

        public void Atualizar(Category category) => _categoryRepository.Atualizar(category);

        public void AlternarEstado(int categoryId, int alteradoPor) => _categoryRepository.AlternarEstado(categoryId, alteradoPor);
    }
}