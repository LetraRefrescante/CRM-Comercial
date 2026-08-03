using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using CRM.Data.Repositories;
using CRM.Web.Helpers;

namespace CRM.Web.Paginas.Administracao
{
    public partial class PerfisPermissoes : Page
    {
        private readonly RoleRepository _roleRepository = new RoleRepository();
        private readonly PermissionRepository _permissionRepository = new PermissionRepository();
        private readonly RolePermissionRepository _rolePermissionRepository = new RolePermissionRepository();

        private bool PodeGerir => Session["RoleName"] as string == "Administrador";
        private bool PodeConsultar => PodeGerir || Session["RoleName"] as string == "Diretor";
        public bool PodeGerirPublico => PodeGerir;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!PodeConsultar)
            {
                NotificacaoService.Erro("Não tens permissão para aceder a esta página.");
                Response.Redirect("~/Dashboard/Dashboard.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CarregarPerfis();
                CarregarPermissoes();
            }
        }

        private void CarregarPerfis()
        {
            ddlPerfil.DataSource = _roleRepository.Listar();
            ddlPerfil.DataTextField = "Name";
            ddlPerfil.DataValueField = "RoleId";
            ddlPerfil.DataBind();
        }

        private void CarregarPermissoes()
        {
            if (ddlPerfil.Items.Count == 0) return;

            var todasPermissoes = _permissionRepository.Listar();

            if (todasPermissoes.Count == 0)
            {
                phVazio.Visible = true;
                btnGuardar.Visible = false;
                rptModulos.DataSource = null;
                rptModulos.DataBind();
                return;
            }

            int roleId = int.Parse(ddlPerfil.SelectedValue);
            var permissoesDoRole = _rolePermissionRepository.ObterPermissoesDoRole(roleId);

            var modulos = todasPermissoes
                .GroupBy(p => p.Module)
                .Select(g => new ModuloPermissoesViewModel
                {
                    Modulo = g.Key,
                    Permissoes = g.Select(p => new PermissaoItemViewModel
                    {
                        PermissionId = p.PermissionId,
                        Code = p.Code,
                        Description = p.Description,
                        Selecionado = permissoesDoRole.Contains(p.PermissionId)
                    }).ToList()
                })
                .OrderBy(m => m.Modulo)
                .ToList();

            rptModulos.DataSource = modulos;
            rptModulos.DataBind();

            phVazio.Visible = false;
            // Diretor só consulta: mostra as checkboxes mas sem botão de gravar
            btnGuardar.Visible = PodeGerir;
        }

        protected void ddlPerfil_SelectedIndexChanged(object sender, EventArgs e)
        {
            CarregarPermissoes();
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!PodeGerir)
            {
                NotificacaoService.Erro("Não tens permissão para executar esta ação.");
                return;
            }

            int roleId = int.Parse(ddlPerfil.SelectedValue);
            int utilizadorAtualId = (int)Session["UserId"];

            var selecionadas = Request.Form.GetValues("permissao");
            var permissionIds = selecionadas == null
                ? new List<int>()
                : selecionadas.Select(int.Parse).ToList();

            _rolePermissionRepository.AtualizarPermissoesDoRole(roleId, permissionIds, utilizadorAtualId);

            NotificacaoService.Sucesso("Permissões atualizadas para o perfil " + ddlPerfil.SelectedItem.Text + ".");

            CarregarPermissoes();
        }
    }

    public class ModuloPermissoesViewModel
    {
        public string Modulo { get; set; }
        public List<PermissaoItemViewModel> Permissoes { get; set; }
    }

    public class PermissaoItemViewModel
    {
        public int PermissionId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool Selecionado { get; set; }
    }
}