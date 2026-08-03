using System;
using CRM.Business.Services;
using CRM.Data.Repositories;
using CRM.Web.Helpers;

namespace CRM.Web.Paginas.Clientes
{
    public partial class ClienteLista : System.Web.UI.Page
    {
        private readonly ClientRepository _clientRepository = new ClientRepository();
        private readonly ClientService _clientService = new ClientService();

        private string Perfil => Session["RoleName"] as string ?? string.Empty;
        private int UserId => (int)Session["UserId"];

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarClientes();
            }
        }

        private void CarregarClientes()
        {
            int? accountManagerId = _clientService.TemAmbitoProprios(Perfil) ? UserId : (int?)null;

            var clientes = _clientRepository.Listar(
                pesquisa: txtPesquisa.Text.Trim(),
                status: ddlEstado.SelectedValue,
                accountManagerId: accountManagerId,
                pagina: ucPaginacao.PaginaAtual,
                tamanhoPagina: ucPaginacao.TamanhoPagina,
                totalRegistos: out int total);

            ucPaginacao.TotalRegistos = total;

            rptClientes.DataSource = clientes;
            rptClientes.DataBind();

            phVazio.Visible = clientes.Count == 0;
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            ucPaginacao.PaginaAtual = 1;
            CarregarClientes();
        }

        protected void ucPaginacao_PaginaAlterada(object sender, EventArgs e)
        {
            CarregarClientes();
        }

        protected void rptClientes_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Eliminar")
            {
                int clientId = int.Parse(e.CommandArgument.ToString());

                if (_clientService.Eliminar(clientId, UserId, Perfil))
                {
                    NotificacaoService.Sucesso("Cliente eliminado.");
                }
                else
                {
                    NotificacaoService.Erro("Não tens permissão para eliminar clientes.");
                }

                CarregarClientes();
            }
        }

        protected string GetBadgeClasse(string status)
        {
            switch (status)
            {
                case "Ativo": return "badge-ativo";
                case "Potencial": return "badge-potencial";
                case "Bloqueado": return "badge-bloqueado";
                case "Inativo": return "badge-inativo";
                default: return "bg-secondary";
            }
        }
    }
}