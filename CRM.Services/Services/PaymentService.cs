using System;
using System.Collections.Generic;
using CRM.Data.Repositories;
using CRM.Models.Entities.Vendas;

namespace CRM.Services
{
    public class PaymentService
    {
        private readonly PaymentRepository _paymentRepository = new PaymentRepository();
        private readonly SaleService _saleService = new SaleService();
        private readonly AuditService _auditService = new AuditService();

        public List<Payment> ListarPorVenda(int saleId) => _paymentRepository.ListarPorVenda(saleId);

        public decimal TotalPago(int saleId) => _paymentRepository.TotalPagoPorVenda(saleId);

        public List<string> Validar(Payment payment)
        {
            var erros = new List<string>();

            if (payment.Amount <= 0)
                erros.Add("O valor do pagamento tem de ser superior a zero.");

            if (payment.PaymentDate > DateTime.Today)
                erros.Add("A data de pagamento não pode ser futura.");

            return erros;
        }
        public Payment Registar(Payment payment, int userId)
        {
            payment.CreatedBy = userId;
            var criado = _paymentRepository.Criar(payment);

            _saleService.RecalcularEstadoFinanceiro(payment.SaleId, userId);
            _auditService.Registar(userId, "Criar", "Payment", criado.PaymentId.ToString());

            return criado;
        }

        public void Eliminar(int paymentId, int saleId, int userId)
        {
            _paymentRepository.EliminarLogico(paymentId, userId);
            _saleService.RecalcularEstadoFinanceiro(saleId, userId);
            _auditService.Registar(userId, "Eliminar", "Payment", paymentId.ToString());
        }
    }
}