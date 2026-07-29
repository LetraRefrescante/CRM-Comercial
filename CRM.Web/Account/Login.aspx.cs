using System;
using CRM.Business.Services;

namespace CRM.Web.Account
{
    public partial class Login : System.Web.UI.Page
    {
        private readonly AuthenticationService _authService = new AuthenticationService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] != null)
            {
                Response.Redirect("~/Dashboard/Dashboard.aspx");
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            var result = _authService.Login(email, password);

            if (result.Result == LoginResult.Success)
            {
                Session["UserId"] = result.User.UserId;
                Session["UserName"] = result.User.Name;
                Session["RoleId"] = result.User.RoleId;

                Response.Redirect("~/Dashboard/Dashboard.aspx");
            }
            else
            {
                pnlErro.Visible = true;
                litErro.Text = result.ErrorMessage;
            }
        }
    }
}