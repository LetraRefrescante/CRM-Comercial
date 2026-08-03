using System;

namespace CRM.Web.Paginas.Dashboard
{
    public partial class Dashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            litNomeUtilizador.Text = Server.HtmlEncode(Session["UserName"] as string ?? "");
        }
    }
}