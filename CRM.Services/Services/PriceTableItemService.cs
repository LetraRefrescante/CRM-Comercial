using System.Collections.Generic;
using CRM.Data.Repositories;
using CRM.Models.Entities.Catalogo;

namespace CRM.Services
{
    public class PriceTableItemService
    {
        private readonly PriceTableItemRepository _priceTableItemRepository = new PriceTableItemRepository();
        private readonly AuditService _auditService = new AuditService();

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

        public int Criar(PriceTableItem item)
        {
            var id = _priceTableItemRepository.Criar(item);

            _auditService.Registar(item.CreatedBy, "Create", "PriceTableItem", id.ToString(),
                $"Preço do produto #{item.ProductId} adicionado à tabela #{item.PriceTableId}.");

            return id;
        }

        public void Atualizar(PriceTableItem item)
        {
            _priceTableItemRepository.Atualizar(item);

            _auditService.Registar(item.UpdatedBy, "Update", "PriceTableItem", item.PriceTableItemId.ToString(),
                $"Preço do item #{item.PriceTableItemId} atualizado.");
        }

        // Passa a receber currentUserId para poder auditar quem eliminou.
        public void Eliminar(int priceTableItemId, int currentUserId)
        {
            _priceTableItemRepository.Eliminar(priceTableItemId);

            _auditService.Registar(currentUserId, "Delete", "PriceTableItem", priceTableItemId.ToString(),
                $"Preço #{priceTableItemId} removido.");
        }
    }
}