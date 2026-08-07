using System;
using CRM.Services;

namespace CRM.Web.Account
{
    public partial class RecuperarPassword : System.Web.UI.Page
    {
        private readonly AuthenticationService _authService = new AuthenticationService();

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnSolicitar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            _authService.SolicitarRecuperacaoPassword(txtEmail.Text.Trim());

            pnlPedido.Visible = false;
            pnlConfirmacao.Visible = true;
        }
    }
}