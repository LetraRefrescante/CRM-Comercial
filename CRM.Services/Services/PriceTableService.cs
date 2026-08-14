using System.Collections.Generic;
using CRM.Data.Repositories;
using CRM.Models.Entities.Catalogo;

namespace CRM.Services
{
    public class PriceTableService
    {
        private readonly PriceTableRepository _priceTableRepository = new PriceTableRepository();
        private readonly AuditService _auditService = new AuditService();

        public bool PodeGerir(string perfil) => perfil == "Administrador" || perfil == "Diretor";

        public List<string> Validar(PriceTable priceTable, bool nomeJaExiste)
        {
            var erros = new List<string>();

            if (string.IsNullOrWhiteSpace(priceTable.Name))
                erros.Add("O nome é obrigatório.");
            else if (priceTable.Name.Trim().Length > 100)
                erros.Add("O nome não pode exceder 100 caracteres.");

            if (nomeJaExiste)
                erros.Add("Já existe uma tabela de preços com este nome.");

            return erros;
        }

        public PriceTable GetById(int priceTableId) => _priceTableRepository.GetById(priceTableId);

        public List<PriceTable> ListarAtivas() => _priceTableRepository.ListarAtivas();

        public List<PriceTable> Listar(string pesquisa) => _priceTableRepository.Listar(pesquisa);

        public bool ExisteNome(string name, int? ignorarPriceTableId = null) => _priceTableRepository.ExisteNome(name, ignorarPriceTableId);

        public int Criar(PriceTable priceTable)
        {
            var id = _priceTableRepository.Criar(priceTable);

            _auditService.Registar(priceTable.CreatedBy, "Create", "PriceTable", id.ToString(),
                $"Tabela de preços '{priceTable.Name}' criada.");

            return id;
        }

        public void Atualizar(PriceTable priceTable)
        {
            _priceTableRepository.Atualizar(priceTable);

            _auditService.Registar(priceTable.UpdatedBy, "Update", "PriceTable", priceTable.PriceTableId.ToString(),
                $"Tabela de preços '{priceTable.Name}' atualizada.");
        }

        public bool AlternarEstado(int priceTableId, int alteradoPor)
        {
            var priceTable = _priceTableRepository.GetById(priceTableId);
            if (priceTable == null) return false;

            if (priceTable.IsActive && priceTable.IsDefault) return false;

            _priceTableRepository.AlternarEstado(priceTableId, alteradoPor);

            _auditService.Registar(alteradoPor, priceTable.IsActive ? "Deactivate" : "Activate", "PriceTable", priceTableId.ToString(),
                $"Tabela de preços '{priceTable.Name}' {(priceTable.IsActive ? "desativada" : "ativada")}.");

            return true;
        }
    }
}