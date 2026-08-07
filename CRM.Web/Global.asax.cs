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
                httpContext.Response.Redirect("~/Account/Login.aspx?returnUrl=" + returnUrl, false);
                httpContext.ApplicationInstance.CompleteRequest();
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();

            if (ex is System.Threading.ThreadAbortException)
            {
                Server.ClearError();
                return;
            }

            string pasta = Server.MapPath("~/App_Data");
            if (!System.IO.Directory.Exists(pasta))
                System.IO.Directory.CreateDirectory(pasta);

            System.IO.File.AppendAllText(
                System.IO.Path.Combine(pasta, "erros.log"),
                DateTime.Now + " | " + Request.Url + Environment.NewLine + ex + Environment.NewLine + "----" + Environment.NewLine);

            System.Diagnostics.Trace.TraceError(ex?.ToString() ?? "Erro desconhecido em Application_Error.");
        }
        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            try
            {
                HttpContext context = HttpContext.Current;
                if (context?.Session == null) return;
                if (context.Session["UserId"] != null) return; // já está populada
                if (!context.Request.IsAuthenticated) return;   // sem cookie válido
                if (!int.TryParse(context.User.Identity.Name, out int userId)) return;

                var userRepository = new CRM.Data.Repositories.UserRepository();
                var user = userRepository.GetById(userId);

                if (user == null || user.Status != "Ativo") return;

                context.Session["UserId"] = user.UserId;
                context.Session["UserName"] = user.Name;
                context.Session["RoleId"] = user.RoleId;
                context.Session["RoleName"] = user.Role?.Name;
            }
            catch
            {
                // Se a restauração falhar (ex: BD momentaneamente indisponível), não deixar
                // isso rebentar o pedido inteiro - a página trata a Session vazia normalmente.
            }
        }
    }
}