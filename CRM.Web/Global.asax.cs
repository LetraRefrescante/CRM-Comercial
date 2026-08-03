using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace CRM.Web
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_PostAcquireRequestState(object sender, EventArgs e)
        {
            var httpContext = HttpContext.Current;
            if (httpContext == null || httpContext.Handler == null)
                return;

            if (!(httpContext.Items["EhPaginaPublica"] is bool ehPaginaPublica))
                return;

            if (!ehPaginaPublica && httpContext.Session["UserId"] == null)
            {
                string returnUrl = HttpUtility.UrlEncode(httpContext.Request.RawUrl);
                httpContext.Response.Redirect("~/Account/Login.aspx?returnUrl=" + returnUrl, true);
            }
        }
    }
}