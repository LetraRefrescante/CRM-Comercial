using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using CRM.Business.Security;
using CRM.Data.Repositories;
using CRM.Web.Helpers;
using CRM.Models.Entities.Seguranca;

namespace CRM.Web.Paginas.Administracao
{
    public partial class UtilizadorEditar : Page
    {
        private readonly UserRepository _userRepository = new UserRepository();
        private readonly RoleRepository _roleRepository = new RoleRepository();

        private int? UserIdEdicao
        {
            get
            {
                int id;
                return int.TryParse(Request.QueryString["id"], out id) ? (int?)id : null;
            }
        }

        private bool EhOProprioUtilizador => UserIdEdicao.HasValue && UserIdEdicao.Value == (int)Session["UserId"];

        protected void Page_Load(object sender, EventArgs e)
        {
            // Só o Administrador cria/edita utilizadores - Diretor tem apenas CONSULTA na listagem
            if (Session["RoleName"] as string != "Administrador")
            {
                NotificacaoService.Erro("Não tens permissão para aceder a esta página.");
                Response.Redirect("~/Dashboard/Dashboard.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CarregarPerfis();

                if (UserIdEdicao.HasValue)
                {
                    litTitulo.Text = "Editar Utilizador";
                    litTituloBreadcrumb.Text = "Editar";
                    phPasswordInicial.Visible = false;
                    phResetPassword.Visible = true;
                    CarregarUtilizador(UserIdEdicao.Value);

                    if (EhOProprioUtilizador)
                    {
                        // Impede auto-bloqueio ou auto-alteração de perfil por engano
                        ddlPerfil.Enabled = false;
                        ddlEstado.Enabled = false;
                        avisoAutoEdicao.Visible = true;
                    }
                }
                else
                {
                    litTitulo.Text = "Novo Utilizador";
                    litTituloBreadcrumb.Text = "Novo";
                    phPasswordInicial.Visible = true;
                    phResetPassword.Visible = false;
                }
            }
        }

        private void CarregarPerfis()
        {
            ddlPerfil.DataSource = _roleRepository.Listar();
            ddlPerfil.DataTextField = "Name";
            ddlPerfil.DataValueField = "RoleId";
            ddlPerfil.DataBind();
        }

        private void CarregarUtilizador(int userId)
        {
            var user = _userRepository.GetById(userId);
            if (user == null)
            {
                NotificacaoService.Erro("Utilizador não encontrado.");
                Response.Redirect("~/Administracao/UtilizadoresLista.aspx");
                return;
            }

            txtNome.Text = user.Name;
            txtEmail.Text = user.Email;
            ddlPerfil.SelectedValue = user.RoleId.ToString();
            ddlEstado.SelectedValue = user.Status;

            ViewState["RowVersion"] = Convert.ToBase64String(user.RowVersion);
        }

        protected void chkResetPassword_CheckedChanged(object sender, EventArgs e)
        {
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            if (_userRepository.EmailExiste(txtEmail.Text.Trim(), UserIdEdicao))
            {
                NotificacaoService.Erro("Já existe um utilizador com este email.");
                return;
            }

            int utilizadorAtualId = (int)Session["UserId"];

            if (UserIdEdicao.HasValue)
            {
                var user = new User
                {
                    UserId = UserIdEdicao.Value,
                    Name = txtNome.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    // Se for o próprio utilizador, os dropdowns estão desativados - mantém os valores originais
                    RoleId = EhOProprioUtilizador ? int.Parse(Request.Form[ddlPerfil.UniqueID] ?? ddlPerfil.SelectedValue) : int.Parse(ddlPerfil.SelectedValue),
                    Status = ddlEstado.SelectedValue,
                    UpdatedBy = utilizadorAtualId,
                    RowVersion = Convert.FromBase64String(ViewState["RowVersion"].ToString())
                };

                _userRepository.Atualizar(user);

                if (chkResetPassword.Checked)
                {
                    string novaPassword = GerarPasswordTemporaria();
                    string salt = PasswordHasher.GenerateSalt();
                    string hash = PasswordHasher.HashPassword(novaPassword, salt);

                    _userRepository.AtualizarPassword(UserIdEdicao.Value, hash, salt);

                    phPasswordGerada.Visible = true;
                    litPasswordGerada.Text = novaPassword;
                }

                NotificacaoService.Sucesso("Utilizador atualizado com sucesso.");
            }
            else
            {
                string passwordTemporaria = GerarPasswordTemporaria();
                string salt = PasswordHasher.GenerateSalt();
                string hash = PasswordHasher.HashPassword(passwordTemporaria, salt);

                var user = new User
                {
                    Name = txtNome.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    RoleId = int.Parse(ddlPerfil.SelectedValue),
                    Status = ddlEstado.SelectedValue,
                    PasswordHash = hash,
                    PasswordSalt = salt,
                    CreatedBy = utilizadorAtualId
                };

                _userRepository.Criar(user);

                phPasswordGerada.Visible = true;
                litPasswordGerada.Text = passwordTemporaria;

                phFormulario.Visible = false;
                phAcoesPosCriacao.Visible = true;

                NotificacaoService.Sucesso("Utilizador criado com sucesso.");
            }
        }

        private string GerarPasswordTemporaria()
        {
            const string caracteres = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789";
            var random = new Random();
            var chars = new char[10];

            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = caracteres[random.Next(caracteres.Length)];
            }

            return new string(chars) + "!";
        }
    }
}