using CRM.Data.Repositories;
using CRM.Models.Entities.Oportunidades;
using CRM.Services;
using CRM.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using CRM.Web.Paginas;

namespace CRM.Web.Oportunidades
{
    public partial class OportunidadeDetalhe : PaginaBase
    {
        private readonly OpportunityService _opportunityService = new OpportunityService();
        private readonly ActivityService _activityService = new ActivityService();
        private readonly ProposalService _proposalService = new ProposalService();
        private readonly UserRepository _userRepository = new UserRepository();

        private Dictionary<int, string> _nomesUtilizadores;

        private int? OpportunityId
        {
            get
            {
                if (int.TryParse(Request.QueryString["id"], out int id)) return id;
                return null;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!OpportunityId.HasValue)
            {
                Response.Redirect("~/Oportunidades/Pipeline.aspx");
                return;
            }

            if (!IsPostBack)
                CarregarOportunidade();
        }

        private void CarregarOportunidade()
        {
            var opportunity = _opportunityService.ObterPorId(OpportunityId.Value, Perfil, UserId);
            if (opportunity == null)
            {
                NotificacaoService.Erro("Oportunidade não encontrada ou sem permissão para a consultar.");
                Response.Redirect("~/Oportunidades/OportunidadesLista.aspx");
                return;
            }

            litTitulo.Text = Server.HtmlEncode(opportunity.Title);

            spanEstado.InnerText = ObterTextoEstado(opportunity);
            spanEstado.Attributes["class"] = "badge " + ObterClasseEstado(opportunity);

            phMotivoPerda.Visible = opportunity.IsClosed && opportunity.LossReason != null;
            litMotivoPerda.Text = Server.HtmlEncode(opportunity.LossReason?.Name ?? "");

            litCliente.Text = Server.HtmlEncode(opportunity.Client?.TradeName ?? "—");
            litContacto.Text = Server.HtmlEncode(opportunity.Contact?.Name ?? "—");
            litFase.Text = Server.HtmlEncode(opportunity.Stage?.Name ?? "—");
            litComercial.Text = Server.HtmlEncode(opportunity.Owner?.Name ?? "—");

            litValor.Text = opportunity.EstimatedValue.ToString("N2") + " €";
            litValorPonderado.Text = _opportunityService.CalcularValorPonderado(opportunity).ToString("N2") + " €";
            litProbabilidade.Text = opportunity.Probability.ToString();
            litDataFecho.Text = opportunity.ExpectedCloseDate.ToString("dd/MM/yyyy");

            phConcorrente.Visible = !string.IsNullOrWhiteSpace(opportunity.Competitor);
            litConcorrente.Text = Server.HtmlEncode(opportunity.Competitor ?? "");

            // ---------- Ações ----------
            bool podeEditar = !opportunity.IsClosed && _opportunityService.PodeEditar(Perfil);
            lnkEditar.Visible = podeEditar;
            lnkEditar.NavigateUrl = $"~/Oportunidades/OportunidadeEditar.aspx?id={opportunity.OpportunityId}";

            bool podeFechar = !opportunity.IsClosed && _opportunityService.PodeFechar(Perfil);
            lnkFechar.Visible = podeFechar;
            lnkFechar.NavigateUrl = $"~/Oportunidades/OportunidadeFechar.aspx?id={opportunity.OpportunityId}";

            // "opportunityId" é o contrato que vou seguir quando construir PropostaEditar.aspx —
            // ainda não existe, por isso este link fica pendente até essa página estar feita.
            lnkNovaProposta.Visible = podeEditar;
            lnkNovaProposta.NavigateUrl =
                $"~/Catalogo/PropostaEditar.aspx?opportunityId={opportunity.OpportunityId}&clientId={opportunity.ClientId}";

            // ---------- Propostas relacionadas ----------
            var propostas = _proposalService.ListarPorOportunidade(opportunity.OpportunityId);
            rptPropostas.DataSource = propostas;
            rptPropostas.DataBind();
            phPropostasVazio.Visible = propostas.Count == 0;

            // ---------- Atividades relacionadas ----------
            var atividades = _activityService.ListarPorOportunidade(opportunity.OpportunityId);
            rptAtividades.DataSource = atividades;
            rptAtividades.DataBind();
            phAtividadesVazio.Visible = atividades.Count == 0;

            // ---------- Histórico de fases ----------
            var historico = _opportunityService.ListarHistoricoFases(opportunity.OpportunityId);

            _nomesUtilizadores = _userRepository.ObterNomesPorIds(
                historico.Where(h => h.ChangedBy.HasValue).Select(h => h.ChangedBy.Value).Distinct());

            rptHistorico.DataSource = historico;
            rptHistorico.DataBind();
            phHistoricoVazio.Visible = historico.Count == 0;
        }

        protected void rptHistorico_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var registo = (OpportunityStageHistory)e.Item.DataItem;
            var litUtilizador = (System.Web.UI.WebControls.Literal)e.Item.FindControl("litUtilizadorHistorico");

            litUtilizador.Text = registo.ChangedBy.HasValue && _nomesUtilizadores.TryGetValue(registo.ChangedBy.Value, out string nome)
                ? Server.HtmlEncode(nome)
                : "—";
        }

        // Mesma lógica do ObterBadgeEstado em OportunidadesLista.aspx.cs — repetida aqui
        // porque é local à página, não ao Service (é apresentação, não regra de negócio).
        private string ObterTextoEstado(Opportunity opportunity)
        {
            if (!opportunity.IsClosed) return "Aberta";
            return opportunity.Stage != null && opportunity.Stage.IsClosedWon ? "Ganha" : "Perdida";
        }

        private string ObterClasseEstado(Opportunity opportunity)
        {
            if (!opportunity.IsClosed) return "bg-primary";
            return opportunity.Stage != null && opportunity.Stage.IsClosedWon ? "bg-success" : "bg-danger";
        }

        protected string GetBadgeClassePropostaEstado(string status)
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

        protected string GetBadgeClasseAtividadeEstado(string status)
        {
            switch (status)
            {
                case "Planeada": return "bg-secondary";
                case "Em Curso": return "badge-em-contacto";
                case "Concluída": return "badge-ativo";
                case "Cancelada": return "badge-bloqueado";
                default: return "bg-secondary";
            }
        }
    }
}