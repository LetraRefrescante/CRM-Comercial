using System;
using System.Web.UI;
using CRM.Web.Helpers;

namespace CRM.Web.Controls
{
    public partial class Mensagens : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            rptMensagens.DataSource = NotificacaoService.ObterELimpar();
            rptMensagens.DataBind();
        }
    }
}