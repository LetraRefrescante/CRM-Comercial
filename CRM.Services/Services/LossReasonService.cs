using System.Collections.Generic;
using CRM.Data.Repositories;
using CRM.Models.Entities.ListasAuxiliares;

namespace CRM.Services
{
    public class LossReasonService
    {
        private readonly LossReasonRepository _lossReasonRepository = new LossReasonRepository();
        private readonly AuditService _auditService = new AuditService();

        public bool PodeGerir(string perfil) => perfil == "Administrador" || perfil == "Diretor";

        public List<string> Validar(LossReason lossReason, bool nomeJaExiste)
        {
            var erros = new List<string>();
            if (string.IsNullOrWhiteSpace(lossReason.Name))
                erros.Add("O nome é obrigatório.");
            if (nomeJaExiste)
                erros.Add("Já existe um motivo com este nome.");
            return erros;
        }

        public LossReason GetById(int id) => _lossReasonRepository.GetById(id);

        public List<LossReason> Listar(string pesquisa, bool incluirInativos = false)
            => _lossReasonRepository.Listar(pesquisa, incluirInativos);

        public bool ExisteNome(string name, int? ignorarId = null) => _lossReasonRepository.ExisteNome(name, ignorarId);

        public int Criar(LossReason lossReason, int userId)
        {
            int id = _lossReasonRepository.Criar(lossReason);
            _auditService.Registar(userId, "Criar", "LossReason", id.ToString(), $"Motivo '{lossReason.Name}' criado.");
            return id;
        }

        public void Atualizar(LossReason lossReason, int userId)
        {
            lossReason.UpdatedBy = userId;
            _lossReasonRepository.Atualizar(lossReason);
            _auditService.Registar(userId, "Atualizar", "LossReason", lossReason.LossReasonId.ToString(), $"Motivo '{lossReason.Name}' atualizado.");
        }

        public void AlternarEstado(int id, int userId)
        {
            _lossReasonRepository.AlternarEstado(id, userId);
            _auditService.Registar(userId, "AlternarEstado", "LossReason", id.ToString(), "Estado do motivo alternado.");
        }
    }
}