using System.Collections.Generic;
using CRM.Data.Repositories;
using CRM.Models.Entities.ListasAuxiliares;

namespace CRM.Services
{
    public class PaymentTermService
    {
        private readonly PaymentTermRepository _paymentTermRepository = new PaymentTermRepository();
        private readonly AuditService _auditService = new AuditService();

        public bool PodeGerir(string perfil) => perfil == "Administrador" || perfil == "Diretor";

        public List<string> Validar(PaymentTerm paymentTerm, bool nomeJaExiste)
        {
            var erros = new List<string>();
            if (string.IsNullOrWhiteSpace(paymentTerm.Name))
                erros.Add("O nome é obrigatório.");
            if (nomeJaExiste)
                erros.Add("Já existe uma condição de pagamento com este nome.");
            if (paymentTerm.DaysDue.HasValue && paymentTerm.DaysDue.Value < 0)
                erros.Add("Os dias não podem ser negativos.");
            return erros;
        }

        public PaymentTerm GetById(int id) => _paymentTermRepository.GetById(id);
        public List<PaymentTerm> Listar(string pesquisa) => _paymentTermRepository.Listar(pesquisa);
        public bool ExisteNome(string name, int? ignorarId = null) => _paymentTermRepository.ExisteNome(name, ignorarId);

        public int Criar(PaymentTerm paymentTerm)
        {
            int id = _paymentTermRepository.Criar(paymentTerm);
            _auditService.Registar(paymentTerm.CreatedBy, "Criar", "PaymentTerm", id.ToString(), $"Condição '{paymentTerm.Name}' criada.");
            return id;
        }

        public void Atualizar(PaymentTerm paymentTerm)
        {
            _paymentTermRepository.Atualizar(paymentTerm);
            _auditService.Registar(paymentTerm.UpdatedBy, "Atualizar", "PaymentTerm", paymentTerm.PaymentTermId.ToString(), $"Condição '{paymentTerm.Name}' atualizada.");
        }

        public void AlternarEstado(int id, int alteradoPor)
        {
            _paymentTermRepository.AlternarEstado(id, alteradoPor);
            _auditService.Registar(alteradoPor, "AlternarEstado", "PaymentTerm", id.ToString(), "Estado da condição alternado.");
        }
    }
}