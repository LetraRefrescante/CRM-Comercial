using System;

namespace CRM.Web.Paginas.Dashboard
{
    public partial class Dashboard : PaginaBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            litNomeUtilizador.Text = Server.HtmlEncode(Session["UserName"] as string ?? Perfil);
        }
    }
}