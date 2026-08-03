using System;
using System.Web.Security;
using CRM.Business.Services;
using CRM.Data.Repositories;
using UserEntity = CRM.Models.Entities.Seguranca.User;

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
                return;
            }

            if (Request.IsAuthenticated && TentarRestaurarSessaoDoCookie())
            {
                Response.Redirect("~/Dashboard/Dashboard.aspx");
                return;
            }

            if (!IsPostBack)
            {
                chkLembrar.InputAttributes["class"] = "form-check-input";
            }
        }

        private bool TentarRestaurarSessaoDoCookie()
        {
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
                Session["RoleName"] = result.User.Role?.Name;

                FormsAuthentication.SetAuthCookie(result.User.UserId.ToString(), chkLembrar.Checked);

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