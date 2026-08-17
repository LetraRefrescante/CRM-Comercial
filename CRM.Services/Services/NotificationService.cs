using System.Collections.Generic;
using CRM.Data.Repositories;
using CRM.Models.Entities.Notificacoes;

namespace CRM.Services
{
    public class NotificationService
    {
        private readonly NotificationRepository _notificationRepository = new NotificationRepository();

        public List<Notification> ListarPorUtilizador(int userId, bool incluirArquivadas = false) =>
            _notificationRepository.ListarPorUtilizador(userId, incluirArquivadas);

        public int ContarNaoLidas(int userId) => _notificationRepository.ContarNaoLidas(userId);
        public int Criar(Notification notification) => _notificationRepository.Criar(notification);

        public bool MarcarComoLida(int notificationId, int userId)
        {
            var notification = _notificationRepository.GetById(notificationId);
            if (notification == null || notification.UserId != userId) return false;

            _notificationRepository.MarcarComoLida(notificationId);
            return true;
        }

        public bool Arquivar(int notificationId, int userId)
        {
            var notification = _notificationRepository.GetById(notificationId);
            if (notification == null || notification.UserId != userId) return false;

            _notificationRepository.Arquivar(notificationId);
            return true;
        }

        public string ResolverUrl(Notification notification)
        {
            if (notification.RelatedTaskId.HasValue)
                return $"~/Atividades/TarefaEditar.aspx?id={notification.RelatedTaskId}";
            if (notification.RelatedProposalId.HasValue)
                return $"~/Catalogo/PropostaDetalhe.aspx?id={notification.RelatedProposalId}";
            if (notification.RelatedSaleId.HasValue)
                return $"~/Vendas/VendaDetalhe.aspx?id={notification.RelatedSaleId}";
            if (notification.RelatedOpportunityId.HasValue)
                return $"~/Oportunidades/OportunidadeDetalhe.aspx?id={notification.RelatedOpportunityId}";
            if (notification.RelatedClientId.HasValue)
                return $"~/Clientes/ClienteDetalhe.aspx?id={notification.RelatedClientId}";
            return null;
        }
    }
}