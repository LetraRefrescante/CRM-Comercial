using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using CRM.Services;

namespace CRM.Web.MasterPages
{
    public partial class SiteMaster : MasterPage
    {
        private readonly AuditService _auditService = new AuditService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/Account/Login.aspx?returnUrl=" + Server.UrlEncode(Request.Url.PathAndQuery));
                return;
            }

            if (!IsPostBack)
            {
                AplicarPermissoesMenu();
                MarcarItemAtivo();
            }
        }
        private void AplicarPermissoesMenu()
        {
            string perfil = Session["RoleName"] as string ?? string.Empty;

            phClientes.Visible = true;
            phLeads.Visible = true;
            phOportunidades.Visible = true;
            phPropostas.Visible = true;
            phVendas.Visible = true;
            phRelatorios.Visible = perfil == "Administrador" || perfil == "Diretor" ||
                                    perfil == "Financeiro" || perfil == "Consulta" ||
                                    perfil == "Comercial";
            phImportarClientes.Visible = perfil == "Administrador" || perfil == "Diretor" || perfil == "Comercial";
            phAdministracao.Visible = perfil == "Administrador" || perfil == "Diretor";
        }

        private void MarcarItemAtivo()
        {
            string paginaAtual = Request.Url.AbsolutePath.ToLower();
            MarcarSeCorresponder(menuLateral, paginaAtual);
        }

        private void MarcarSeCorresponder(Control control, string paginaAtual)
        {
            if (control is HtmlAnchor anchor && !string.IsNullOrEmpty(anchor.HRef))
            {
                string href = ResolveUrl(anchor.HRef).ToLower();

                if (href.EndsWith(".aspx"))
                {
                    href = href.Substring(0, href.Length - ".aspx".Length);
                }

                if (!string.IsNullOrEmpty(href) && paginaAtual.Contains(href))
                {
                    string classeAtual = anchor.Attributes["class"] ?? "";
                    anchor.Attributes["class"] = (classeAtual + " active").Trim();
                }
            }

            foreach (Control child in control.Controls)
            {
                MarcarSeCorresponder(child, paginaAtual);
            }
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            int? userId = Session["UserId"] as int?;
            _auditService.Registar(userId, "Logout", "User", userId?.ToString());

            Session.Clear();
            Session.Abandon();
            System.Web.Security.FormsAuthentication.SignOut();
            Response.Redirect("~/Account/Login.aspx");
        }
    }
}