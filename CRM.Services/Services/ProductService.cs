using System;
using System.Collections.Generic;
using CRM.Data.Repositories;
using CRM.Models.Entities.Catalogo;

namespace CRM.Services
{
    public class ProductService
    {
        private readonly ProductRepository _productRepository = new ProductRepository();
        private readonly AuditService _auditService = new AuditService();

        public const string TipoProduto = "Produto";
        public const string TipoServico = "Serviço";

        public static readonly string[] Unidades = { "Unidade", "Hora", "Dia", "Mês", "Pacote" };
        public bool PodeGerir(string perfil) => perfil == "Administrador" || perfil == "Diretor";

        public List<string> Validar(Product product, bool codigoJaExiste)
        {
            var erros = new List<string>();

            if (string.IsNullOrWhiteSpace(product.Code))
                erros.Add("O código é obrigatório.");
            else if (codigoJaExiste)
                erros.Add("Já existe um produto/serviço com este código.");

            if (string.IsNullOrWhiteSpace(product.Name))
                erros.Add("O nome é obrigatório.");

            if (product.Type != TipoProduto && product.Type != TipoServico)
                erros.Add("O tipo tem de ser Produto ou Serviço.");

            if (product.CategoryId <= 0)
                erros.Add("A categoria é obrigatória.");

            if (product.TaxRateId <= 0)
                erros.Add("A taxa de IVA é obrigatória.");

            if (product.BasePrice < 0)
                erros.Add("O preço base não pode ser negativo.");

            if (Array.IndexOf(Unidades, product.Unit) < 0)
                erros.Add("A unidade selecionada não é válida.");

            return erros;
        }

        public Product GetById(int productId) => _productRepository.GetById(productId);

        public List<Product> Listar(
            string pesquisa, string type, int? categoryId, bool? isActive,
            int pagina, int tamanhoPagina, out int totalRegistos,
            string sortColumn, bool sortAscending)
            => _productRepository.Listar(pesquisa, type, categoryId, isActive, pagina, tamanhoPagina, out totalRegistos, sortColumn, sortAscending);

        public bool ExisteCodigo(string code, int? ignorarProductId = null) => _productRepository.ExisteCodigo(code, ignorarProductId);

        public int Criar(Product product)
        {
            int productId = _productRepository.Criar(product);

            _auditService.Registar(product.CreatedBy, "Criar", "Product", productId.ToString(),
                $"Produto '{product.Name}' (código {product.Code}) criado.");

            return productId;
        }

        public void Atualizar(Product product)
        {
            _productRepository.Atualizar(product);

            _auditService.Registar(product.UpdatedBy, "Atualizar", "Product", product.ProductId.ToString(),
                $"Produto '{product.Name}' atualizado.");
        }

        public bool Eliminar(int productId, int userId, string perfil)
        {
            if (!PodeGerir(perfil)) return false;

            _productRepository.EliminarLogico(productId, userId);

            _auditService.Registar(userId, "Eliminar", "Product", productId.ToString(),
                "Produto eliminado (soft delete + inativado).");

            return true;
        }
    }
}