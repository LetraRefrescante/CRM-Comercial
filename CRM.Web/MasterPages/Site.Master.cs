using System;
using System.Web.UI;

namespace CRM.Web.MasterPages
{
    public partial class SiteMaster : MasterPage
    {
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
            }
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

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Account/Login.aspx");
        }

        public void MostrarMensagem(string texto, bool sucesso)
        {
            phMensagem.Visible = true;
            divMensagem.InnerText = texto;
            divMensagem.Attributes["class"] = "alert " + (sucesso ? "alert-success" : "alert-danger");
        }
    }
}