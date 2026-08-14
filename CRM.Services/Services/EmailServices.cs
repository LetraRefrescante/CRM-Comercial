using System;

namespace CRM.Services
{
    /// <summary>
    /// Envio de email. Ainda não ligado a um servidor SMTP real — o módulo de Notificações e
    /// Email (011_Notifications_Email.sql: EmailTemplates/EmailHistory) fica para a Fase 5.
    /// Por agora, tal como AuthenticationService.EnviarEmailRecuperacao, lança
    /// NotImplementedException; quem chama tem de apanhar isto e continuar sem bloquear o
    /// fluxo principal.
    /// </summary>
    public class EmailService
    {
        public void Enviar(string destinatario, string assunto, string corpo)
        {
            throw new NotImplementedException("Ligar ao serviço de email do projeto (SMTP via Web.config).");
        }
    }
}