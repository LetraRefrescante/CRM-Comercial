using System.Collections.Generic;
using CRM.Data.Repositories;
using CRM.Models.Entities.Catalogo;

namespace CRM.Services
{
    public class PriceTableItemService
    {
        private readonly PriceTableItemRepository _priceTableItemRepository = new PriceTableItemRepository();

        public List<string> Validar(PriceTableItem item, bool produtoJaTemPreco)
        {
            var erros = new List<string>();

            if (item.ProductId <= 0)
                erros.Add("O produto é obrigatório.");
            else if (produtoJaTemPreco)
                erros.Add("Este produto já tem um preço definido nesta tabela.");

            if (item.Price < 0)
                erros.Add("O preço não pode ser negativo.");

            return erros;
        }

        public List<PriceTableItem> ListarPorTabela(int priceTableId) => _priceTableItemRepository.ListarPorTabela(priceTableId);

        public PriceTableItem GetById(int priceTableItemId) => _priceTableItemRepository.GetById(priceTableItemId);

        public bool ExisteProdutoNaTabela(int priceTableId, int productId, int? ignorarPriceTableItemId = null)
            => _priceTableItemRepository.ExisteProdutoNaTabela(priceTableId, productId, ignorarPriceTableItemId);

        public int Criar(PriceTableItem item) => _priceTableItemRepository.Criar(item);

        public void Atualizar(PriceTableItem item) => _priceTableItemRepository.Atualizar(item);

        public void Eliminar(int priceTableItemId) => _priceTableItemRepository.Eliminar(priceTableItemId);
    }
}