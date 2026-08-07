using System;
using CRM.Services;

namespace CRM.Web.Account
{
    public partial class RedefinirPassword : System.Web.UI.Page
    {
        private readonly AuthenticationService _authService = new AuthenticationService();

        private string Token => Request.QueryString["token"];

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            if (string.IsNullOrEmpty(Token) || !_authService.TokenValido(Token))
            {
                pnlFormulario.Visible = false;
                pnlTokenInvalido.Visible = true;
            }
        }

        protected void btnRedefinir_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            bool sucesso = _authService.RedefinirPasswordComToken(Token, txtNovaPassword.Text);

            if (!sucesso)
            {
                pnlFormulario.Visible = false;
                pnlTokenInvalido.Visible = true;
                return;
            }

            Response.Redirect("~/Account/Login.aspx");
        }
    }
}