using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using CRM.Data.Repositories;
using CRM.Models.Entities.ListasAuxiliares;
using CRM.Models.Entities.Oportunidades;
using CRM.Services;

namespace CRM.Web.Oportunidades
{
    public partial class Pipeline : Page
    {
        private readonly OpportunityService _opportunityService = new OpportunityService();
        private readonly OpportunityStageRepository _stageRepository = new OpportunityStageRepository();

        private string Perfil => Session["RoleName"] as string ?? "";
        private int UserId => Session["UserId"] != null ? (int)Session["UserId"] : 0;

        public bool PodeMover => _opportunityService.PodeEditar(Perfil);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lnkNova.Visible = PodeMover;
                phModoConsulta.Visible = !PodeMover;
                CarregarPipeline();
            }
        }

        private void CarregarPipeline()
        {
            var fases = _stageRepository.ListarAtivasParaAbertura();
            var oportunidades = _opportunityService.ListarParaPipeline(Perfil, UserId);

            var colunas = fases.Select(fase => new PipelineColuna
            {
                Stage = fase,
                Oportunidades = oportunidades.Where(o => o.StageId == fase.StageId).ToList()
            }).ToList();

            rptColunas.DataSource = colunas;
            rptColunas.DataBind();
        }

        protected void rptColunas_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var coluna = (PipelineColuna)e.Item.DataItem;
            var rptCartoes = (Repeater)e.Item.FindControl("rptCartoes");
            rptCartoes.DataSource = coluna.Oportunidades;
            rptCartoes.DataBind();
        }

        protected int ContarCartoes(object coluna) => ((PipelineColuna)coluna).Oportunidades.Count;

        protected string FormatarValorColuna(object coluna) =>
            string.Format("{0:N2} €", ((PipelineColuna)coluna).ValorTotal);
    }

    public class PipelineColuna
    {
        public OpportunityStage Stage { get; set; }
        public List<Opportunity> Oportunidades { get; set; }
        public decimal ValorTotal => Oportunidades.Sum(o => o.EstimatedValue);
    }
}