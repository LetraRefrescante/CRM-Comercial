using CRM.Data.Repositories;
using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using UserEntity = CRM.Models.Entities.Seguranca.User;

namespace CRM.Web.MasterPages
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                if (!TentarRestaurarSessaoDoCookie())
                {
                    Response.Redirect("~/Account/Login.aspx?returnUrl=" + Server.UrlEncode(Request.Url.PathAndQuery));
                    return;
                }
            }

            if (!IsPostBack)
            {
                AplicarPermissoesMenu();
                MarcarItemAtivo();
            }
        }

        private bool TentarRestaurarSessaoDoCookie()
        {
            if (!Request.IsAuthenticated) return false;

            if (!int.TryParse(Context.User.Identity.Name, out int userId)) return false;

            var userRepository = new UserRepository();
            UserEntity user = userRepository.GetById(userId);

            if (user == null || user.Status != "Ativo") return false;

            Session["UserId"] = user.UserId;
            Session["UserName"] = user.Name;
            Session["RoleId"] = user.RoleId;
            Session["RoleName"] = user.Role?.Name;

            return true;
        }


        private void AplicarPermissoesMenu()
        {
            string perfil = Session["RoleName"] as string ?? string.Empty;

            switch (perfil)
            {
                case "Administrador":
                    phClientes.Visible = true;
                    phLeads.Visible = true;
                    phOportunidades.Visible = true;
                    phPropostas.Visible = true;
                    phVendas.Visible = true;
                    phRelatorios.Visible = true;
                    phAdministracao.Visible = true;
                    break;

                case "Diretor":
                case "Financeiro":
                case "Consulta":
                    phClientes.Visible = true;
                    phLeads.Visible = true;
                    phOportunidades.Visible = true;
                    phPropostas.Visible = true;
                    phVendas.Visible = true;
                    phRelatorios.Visible = true;
                    phAdministracao.Visible = false;
                    break;

                case "Comercial":
                default:
                    phClientes.Visible = true;
                    phLeads.Visible = true;
                    phOportunidades.Visible = true;
                    phPropostas.Visible = true;
                    phVendas.Visible = true;
                    phRelatorios.Visible = false;
                    phAdministracao.Visible = false;
                    break;
            }
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
            Session.Clear();
            Session.Abandon();
            System.Web.Security.FormsAuthentication.SignOut();
            Response.Redirect("~/Account/Login.aspx");
        }
    }
}