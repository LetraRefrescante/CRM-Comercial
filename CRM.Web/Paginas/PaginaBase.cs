using System;
using System.Web.UI;

namespace CRM.Web.Paginas
{
    public abstract class PaginaBase : Page
    {
        protected string Perfil { get; private set; } = string.Empty;
        protected int UserId { get; private set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            int? userId = Session["UserId"] as int?;
            string perfil = Session["RoleName"] as string;

            bool sessaoValida = userId.HasValue && !string.IsNullOrEmpty(perfil);

            if (!sessaoValida)
            {
                Response.Redirect("~/Account/Login.aspx?returnUrl=" + Server.UrlEncode(Request.Url.PathAndQuery), endResponse: true);
                return;
            }

            UserId = userId.GetValueOrDefault();
            Perfil = perfil;
        }
    }
}