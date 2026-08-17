using System;
using System.Web.UI.WebControls;
using CRM.Models.Entities.Notificacoes;
using CRM.Services;
using CRM.Web.Helpers;

namespace CRM.Web.Paginas.Notificacoes
{
    public partial class Notificacoes : PaginaBase
    {
        private readonly NotificationService _notificationService = new NotificationService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                CarregarNotificacoes();
        }

        private void CarregarNotificacoes()
        {
            bool incluirArquivadas = ddlFiltro.SelectedValue == "ComArquivadas";
            var notificacoes = _notificationService.ListarPorUtilizador(UserId, incluirArquivadas);

            if (ddlFiltro.SelectedValue == "NaoLidas")
                notificacoes = notificacoes.FindAll(n => !n.IsRead);

            rptNotificacoes.DataSource = notificacoes;
            rptNotificacoes.DataBind();

            phVazio.Visible = notificacoes.Count == 0;
        }

        protected void ddlFiltro_SelectedIndexChanged(object sender, EventArgs e) => CarregarNotificacoes();

        protected void rptNotificacoes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int notificationId = int.Parse(e.CommandArgument.ToString());

            if (e.CommandName == "MarcarLida")
                _notificationService.MarcarComoLida(notificationId, UserId);
            else if (e.CommandName == "Arquivar")
                _notificationService.Arquivar(notificationId, UserId);

            CarregarNotificacoes();
        }

        protected string GetLidaClasse(object dataItem) =>
            !((Notification)dataItem).IsRead ? "crm-notification-nao-lida" : "";

        protected bool GetNaoLida(object dataItem) => !((Notification)dataItem).IsRead;

        protected bool GetNaoArquivada(object dataItem) => !((Notification)dataItem).IsArchived;

        protected bool GetTemUrlRelacionada(object dataItem) =>
            _notificationService.ResolverUrl((Notification)dataItem) != null;

        protected string GetUrlRelacionada(object dataItem) =>
            _notificationService.ResolverUrl((Notification)dataItem) ?? "#";
    }
}