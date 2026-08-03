using System;
using System.Web;
using System.Web.Compilation;
using System.Web.Routing;
using System.Web.UI;

namespace CRM.Web
{
    public class PaginasRouteHandler : IRouteHandler
    {
        public static readonly string[] PaginasPublicas =
        {
            "account/login.aspx",
            "erro.aspx"
        };

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var httpContext = requestContext.HttpContext;
            string path = requestContext.RouteData.Values["path"] as string ?? "";

            // Bloqueia acesso direto que já inclua "Paginas/" no URL escrito no browser
            if (path.StartsWith("Paginas/", StringComparison.OrdinalIgnoreCase))
            {
                httpContext.Response.Redirect("~/Erro.aspx?codigo=404", true);
                return null;
            }

            if (string.IsNullOrEmpty(path))
            {
                httpContext.Response.Redirect("~/Account/Login.aspx", true);
                return null;
            }

            if (!path.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
                path += ".aspx";

            string virtualPath = "~/Paginas/" + path;

            if (!System.IO.File.Exists(httpContext.Server.MapPath(virtualPath)))
            {
                httpContext.Response.Redirect("~/Erro.aspx?codigo=404", true);
                return null;
            }

            // Guarda no contexto para o Global.asax usar depois, já com Session disponível
            httpContext.Items["EhPaginaPublica"] = Array.Exists(PaginasPublicas,
                p => p.Equals(path, StringComparison.OrdinalIgnoreCase));

            return BuildManager.CreateInstanceFromVirtualPath(virtualPath, typeof(Page)) as IHttpHandler;
        }
    }

    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            // Não aplicar routing a recursos estáticos e handlers do sistema
            routes.Add(new Route("{resource}.axd/{*pathInfo}", new StopRoutingHandler()));
            routes.Add(new Route("Content/{*pathInfo}", new StopRoutingHandler()));
            routes.Add(new Route("Scripts/{*pathInfo}", new StopRoutingHandler()));
            routes.Add(new Route("bundles/{*pathInfo}", new StopRoutingHandler()));

            // Wildcard: qualquer coisa que sobrar vai bater aqui e é servida a partir de /Paginas/
            routes.Add("PaginasCatchAll", new Route("{*path}", new PaginasRouteHandler()));
        }
    }
}