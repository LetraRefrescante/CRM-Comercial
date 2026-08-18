using System.Collections.Generic;
using CRM.Data.Repositories;
using CRM.Models.Entities.ListasAuxiliares;

namespace CRM.Services
{
    public class LeadSourceService
    {
        private readonly LeadSourceRepository _leadSourceRepository = new LeadSourceRepository();
        private readonly AuditService _auditService = new AuditService();

        public bool PodeGerir(string perfil) => perfil == "Administrador" || perfil == "Diretor";

        public List<string> Validar(LeadSource leadSource, bool nomeJaExiste)
        {
            var erros = new List<string>();
            if (string.IsNullOrWhiteSpace(leadSource.Name))
                erros.Add("O nome é obrigatório.");
            else if (leadSource.Name.Trim().Length > 100)
                erros.Add("O nome não pode exceder 100 caracteres.");
            if (nomeJaExiste)
                erros.Add("Já existe uma origem com este nome.");
            return erros;
        }

        public LeadSource GetById(int id) => _leadSourceRepository.GetById(id);
        public List<LeadSource> Listar(string pesquisa) => _leadSourceRepository.Listar(pesquisa);
        public bool ExisteNome(string name, int? ignorarId = null) => _leadSourceRepository.ExisteNome(name, ignorarId);

        public int Criar(LeadSource leadSource)
        {
            int id = _leadSourceRepository.Criar(leadSource);
            _auditService.Registar(leadSource.CreatedBy, "Criar", "LeadSource", id.ToString(), $"Origem '{leadSource.Name}' criada.");
            return id;
        }

        public void Atualizar(LeadSource leadSource)
        {
            _leadSourceRepository.Atualizar(leadSource);
            _auditService.Registar(leadSource.UpdatedBy, "Atualizar", "LeadSource", leadSource.LeadSourceId.ToString(), $"Origem '{leadSource.Name}' atualizada.");
        }

        public void AlternarEstado(int id, int alteradoPor)
        {
            _leadSourceRepository.AlternarEstado(id, alteradoPor);
            _auditService.Registar(alteradoPor, "AlternarEstado", "LeadSource", id.ToString(), "Estado da origem alternado.");
        }
    }
}