using System.Collections.Generic;
using CRM.Data.Repositories;
using CRM.Models.Entities.Catalogo;

namespace CRM.Services
{
    public class TaxRateService
    {
        private readonly TaxRateRepository _taxRateRepository = new TaxRateRepository();
        private readonly AuditService _auditService = new AuditService();

        public bool PodeGerir(string perfil) => perfil == "Administrador" || perfil == "Diretor";

        public List<string> Validar(TaxRate taxRate, bool nomeJaExiste)
        {
            var erros = new List<string>();
            if (string.IsNullOrWhiteSpace(taxRate.Name))
                erros.Add("O nome é obrigatório.");
            if (nomeJaExiste)
                erros.Add("Já existe uma taxa com este nome.");
            if (taxRate.Percentage < 0 || taxRate.Percentage > 100)
                erros.Add("A percentagem tem de estar entre 0 e 100.");
            return erros;
        }

        public TaxRate GetById(int id) => _taxRateRepository.GetById(id);
        public List<TaxRate> Listar(string pesquisa) => _taxRateRepository.Listar(pesquisa);
        public bool ExisteNome(string name, int? ignorarId = null) => _taxRateRepository.ExisteNome(name, ignorarId);

        public int Criar(TaxRate taxRate)
        {
            int id = _taxRateRepository.Criar(taxRate);
            _auditService.Registar(taxRate.CreatedBy, "Criar", "TaxRate", id.ToString(), $"Taxa '{taxRate.Name}' criada.");
            return id;
        }

        public void Atualizar(TaxRate taxRate)
        {
            _taxRateRepository.Atualizar(taxRate);
            _auditService.Registar(taxRate.UpdatedBy, "Atualizar", "TaxRate", taxRate.TaxRateId.ToString(), $"Taxa '{taxRate.Name}' atualizada.");
        }

        public void AlternarEstado(int id, int alteradoPor)
        {
            _taxRateRepository.AlternarEstado(id, alteradoPor);
            _auditService.Registar(alteradoPor, "AlternarEstado", "TaxRate", id.ToString(), "Estado da taxa alternado.");
        }
    }
}