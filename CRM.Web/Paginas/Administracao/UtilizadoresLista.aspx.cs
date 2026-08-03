using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using CRM.Data.Repositories;
using CRM.Web.Helpers;

namespace CRM.Web.Paginas.Administracao
{
    public partial class UtilizadoresLista : Page
    {
        private readonly UserRepository _userRepository = new UserRepository();
        private readonly RoleRepository _roleRepository = new RoleRepository();

        private bool PodeGerir => Session["RoleName"] as string == "Administrador";
        private bool PodeConsultar => PodeGerir || Session["RoleName"] as string == "Diretor";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!PodeConsultar)
            {
                NotificacaoService.Erro("Não tens permissão para aceder a esta página.");
                Response.Redirect("~/Dashboard/Dashboard.aspx");
                return;
            }

            lnkNovo.Visible = PodeGerir;

            if (!IsPostBack)
            {
                CarregarPerfis();
                CarregarUtilizadores();
            }
        }

        private void CarregarPerfis()
        {
            ddlPerfil.DataSource = _roleRepository.Listar();
            ddlPerfil.DataTextField = "Name";
            ddlPerfil.DataValueField = "RoleId";
            ddlPerfil.DataBind();
            ddlPerfil.Items.Insert(0, new ListItem("Todos", ""));
        }

        private void CarregarUtilizadores()
        {
            int? roleId = string.IsNullOrEmpty(ddlPerfil.SelectedValue) ? (int?)null : int.Parse(ddlPerfil.SelectedValue);
            string estado = string.IsNullOrEmpty(ddlEstado.SelectedValue) ? null : ddlEstado.SelectedValue;

            var utilizadores = _userRepository.Listar(txtPesquisa.Text.Trim(), roleId, estado);

            rptUtilizadores.DataSource = utilizadores;
            rptUtilizadores.DataBind();

            phVazio.Visible = utilizadores.Count == 0;
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            CarregarUtilizadores();
        }

        protected void rptUtilizadores_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (!PodeGerir)
            {
                NotificacaoService.Erro("Não tens permissão para executar esta ação.");
                CarregarUtilizadores();
                return;
            }

            int userId = int.Parse(e.CommandArgument.ToString());
            int utilizadorAtualId = (int)Session["UserId"];

            if (userId == utilizadorAtualId)
            {
                NotificacaoService.Erro("Não podes alterar o teu próprio estado ou eliminar-te.");
                CarregarUtilizadores();
                return;
            }

            switch (e.CommandName)
            {
                case "Bloquear":
                    _userRepository.AlterarStatus(userId, "Bloqueado", utilizadorAtualId);
                    NotificacaoService.Sucesso("Utilizador bloqueado.");
                    break;

                case "Ativar":
                    _userRepository.AlterarStatus(userId, "Ativo", utilizadorAtualId);
                    NotificacaoService.Sucesso("Utilizador ativado.");
                    break;

                case "Eliminar":
                    _userRepository.EliminarLogico(userId, utilizadorAtualId);
                    NotificacaoService.Sucesso("Utilizador eliminado.");
                    break;
            }

            CarregarUtilizadores();
        }

        protected void rptUtilizadores_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var phAcoes = e.Item.FindControl("phAcoesGestao") as PlaceHolder;
            if (phAcoes != null)
            {
                phAcoes.Visible = PodeGerir;
            }
        }

        protected string GetBadgeClasse(string status)
        {
            switch (status)
            {
                case "Ativo": return "text-bg-success";
                case "Bloqueado": return "text-bg-danger";
                default: return "text-bg-secondary";
            }
        }
    }
}