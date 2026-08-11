using CRM.Data.Repositories;
using CRM.Models.Entities.Catalogo;
using CRM.Services;
using CRM.Web.Helpers;
using System;

namespace CRM.Web.Paginas.Catalogo
{
    public partial class PropostasLista : PaginaBase
    {
        private readonly ProposalService _proposalService = new ProposalService();
        private readonly UserRepository _userRepository = new UserRepository();

        private string SortColumn
        {
            get => ViewState["SortColumn"] as string ?? "IssueDate";
            set => ViewState["SortColumn"] = value;
        }

        private bool SortAscending
        {
            get => ViewState["SortAscending"] as bool? ?? false;
            set => ViewState["SortAscending"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarComerciais();
                CarregarPropostas();
            }
        }

        private void CarregarComerciais()
        {
            bool podeFiltrarPorComercial = !_proposalService.TemAmbitoProprios(Perfil);

            ddlComercial.Visible = podeFiltrarPorComercial;
            lblComercial.Visible = podeFiltrarPorComercial;

            if (!podeFiltrarPorComercial) return;

            ddlComercial.Items.Add(new System.Web.UI.WebControls.ListItem("Todos", ""));
            foreach (var user in _userRepository.ListarComerciaisAtivos())
            {
                ddlComercial.Items.Add(new System.Web.UI.WebControls.ListItem(user.Name, user.UserId.ToString()));
            }
        }

        private int? ObterFiltroComercial()
        {
            if (_proposalService.TemAmbitoProprios(Perfil)) return UserId;

            if (ddlComercial.Visible && !string.IsNullOrEmpty(ddlComercial.SelectedValue))
                return int.Parse(ddlComercial.SelectedValue);

            return null;
        }

        private void CarregarPropostas()
        {
            var propostas = _proposalService.Listar(
                pesquisa: txtPesquisa.Text.Trim(),
                status: ddlEstado.SelectedValue,
                clientId: null,
                accountManagerId: ObterFiltroComercial(),
                dataInicio: ucFiltroDatas.DataInicial,
                dataFim: ucFiltroDatas.DataFinal,
                pagina: ucPaginacao.PaginaAtual,
                tamanhoPagina: ucPaginacao.TamanhoPagina,
                totalRegistos: out int total,
                sortColumn: SortColumn,
                sortAscending: SortAscending);

            ucPaginacao.TotalRegistos = total;

            rptPropostas.DataSource = propostas;
            rptPropostas.DataBind();

            phVazio.Visible = propostas.Count == 0;

            lnkNova.Visible = _proposalService.PodeCriarOuEditar(Perfil);
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            ucPaginacao.PaginaAtual = 1;
            CarregarPropostas();
        }

        protected void lnkOrdenar_Command(object sender, System.Web.UI.WebControls.CommandEventArgs e)
        {
            string coluna = e.CommandArgument.ToString();

            if (SortColumn == coluna)
            {
                SortAscending = !SortAscending;
            }
            else
            {
                SortColumn = coluna;
                SortAscending = true;
            }

            ucPaginacao.PaginaAtual = 1;
            CarregarPropostas();
        }

        protected void ucPaginacao_PaginaAlterada(object sender, EventArgs e)
        {
            CarregarPropostas();
        }

        protected void rptPropostas_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != System.Web.UI.WebControls.ListItemType.Item &&
                e.Item.ItemType != System.Web.UI.WebControls.ListItemType.AlternatingItem)
                return;

            var proposal = (Proposal)e.Item.DataItem;

            var phEditar = e.Item.FindControl("phEditar") as System.Web.UI.WebControls.PlaceHolder;
            var phEliminar = e.Item.FindControl("phEliminar") as System.Web.UI.WebControls.PlaceHolder;

            if (phEditar != null)
                phEditar.Visible = _proposalService.PodeCriarOuEditar(Perfil) && _proposalService.PodeAceder(proposal, UserId, Perfil);

            if (phEliminar != null)
                phEliminar.Visible = _proposalService.PodeEliminar(proposal, UserId, Perfil);
        }

        protected void rptPropostas_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Eliminar")
            {
                int proposalId = int.Parse(e.CommandArgument.ToString());

                if (_proposalService.Eliminar(proposalId, UserId, Perfil))
                {
                    NotificacaoService.Sucesso("Proposta eliminada.");
                }
                else
                {
                    NotificacaoService.Erro("Não tens permissão para eliminar propostas.");
                }

                CarregarPropostas();
            }
        }

        protected string GetBadgeClasse(string status)
        {
            switch (status)
            {
                case "Rascunho": return "bg-secondary";
                case "Enviada": return "badge-em-contacto";
                case "Aceite": return "badge-ativo";
                case "Recusada": return "badge-bloqueado";
                case "Expirada": return "badge-inativo";
                case "Cancelada": return "badge-bloqueado";
                default: return "bg-secondary";
            }
        }
    }
}