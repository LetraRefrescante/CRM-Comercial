using System;
using CRM.Services;
using CRM.Web.Helpers;

namespace CRM.Web.Paginas.Conta
{
    public partial class AlterarPassword : PaginaBase
    {
        private readonly AuthenticationService _authService = new AuthenticationService();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnAlterar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            bool sucesso = _authService.AlterarPassword(UserId, txtPasswordAtual.Text, txtNovaPassword.Text);

            if (!sucesso)
            {
                lblErro.Text = "Password atual incorreta.";
                lblErro.Visible = true;
                return;
            }

            NotificacaoService.Sucesso("Password alterada com sucesso.");
            Response.Redirect("~/Dashboard/Dashboard.aspx");
        }
    }
}