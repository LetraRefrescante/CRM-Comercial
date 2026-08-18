using System;
using System.Globalization;
using System.Linq;
using System.Text;
using CRM.Models.Entities.Atividades;
using CRM.Services;

namespace CRM.Web.Paginas.Dashboard
{
    public partial class Dashboard : PaginaBase
    {
        private readonly DashboardService _dashboardService = new DashboardService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarIndicadores();
                CarregarListas();
                CarregarGraficos();
            }
        }

        private void CarregarIndicadores()
        {
            var indicadores = _dashboardService.ObterIndicadores(UserId, Perfil);

            litClientesAtivos.Text = indicadores.TotalClientesAtivos.ToString();
            litNovosClientes.Text = indicadores.NovosClientesMes.ToString();

            litLeadsNovos.Text = indicadores.LeadsNovos.ToString();
            litLeadsEmContacto.Text = indicadores.LeadsEmContacto.ToString();
            litLeadsQualificados.Text = indicadores.LeadsQualificados.ToString();

            litOportunidadesAbertas.Text = indicadores.OportunidadesAbertas.ToString();
            litValorPonderado.Text = indicadores.ValorPonderadoAberto.ToString("C");

            litVendasMes.Text = indicadores.VendasMes.ToString("C");
            litVendasAno.Text = indicadores.VendasAno.ToString("C");

            litTarefasVencidas.Text = indicadores.TarefasVencidas.ToString();
            litTarefasHoje.Text = indicadores.TarefasHoje.ToString();
            litPropostasAExpirar.Text = indicadores.PropostasAExpirar.ToString();
        }

        private void CarregarListas()
        {
            var atividades = _dashboardService.ObterUltimasAtividades(UserId, Perfil);
            rptUltimasAtividades.DataSource = atividades;
            rptUltimasAtividades.DataBind();
            phSemAtividades.Visible = atividades.Count == 0;

            var reunioes = _dashboardService.ObterProximasReunioes(UserId, Perfil);
            rptProximasReunioes.DataSource = reunioes;
            rptProximasReunioes.DataBind();
            phSemReunioes.Visible = reunioes.Count == 0;

            var oportunidadesPendentes = _dashboardService.ObterOportunidadesSemAtividade(UserId, Perfil);
            rptOportunidadesSemAtividade.DataSource = oportunidadesPendentes;
            rptOportunidadesSemAtividade.DataBind();
            phSemOportunidadesPendentes.Visible = oportunidadesPendentes.Count == 0;

            bool podeVerTopComerciais = _dashboardService.PodeVerTopComerciais(Perfil);
            phTopComerciais.Visible = podeVerTopComerciais;
            if (podeVerTopComerciais)
            {
                var topComerciais = _dashboardService.ObterTopComerciais();
                rptTopComerciais.DataSource = topComerciais;
                rptTopComerciais.DataBind();
                phSemTopComerciais.Visible = topComerciais.Count == 0;
            }
        }

        private void CarregarGraficos()
        {
            var vendasPorMes = _dashboardService.ObterVendasPorMes(UserId, Perfil);
            var pipelinePorFase = _dashboardService.ObterPipelinePorFase(UserId, Perfil);
            var origemLeads = _dashboardService.ObterOrigemLeads(UserId, Perfil);

            string labelsVendas = string.Join(",", vendasPorMes.Select(v => "\"" + v.Mes + "\""));
            string valoresVendas = string.Join(",", vendasPorMes.Select(v => v.Total.ToString(CultureInfo.InvariantCulture)));

            string labelsPipeline = string.Join(",", pipelinePorFase.Select(f => "\"" + f.Fase.Replace("\"", "") + "\""));
            string valoresPipeline = string.Join(",", pipelinePorFase.Select(f => f.Valor.ToString(CultureInfo.InvariantCulture)));

            string labelsOrigem = string.Join(",", origemLeads.Select(o => "\"" + o.Origem.Replace("\"", "") + "\""));
            string valoresOrigem = string.Join(",", origemLeads.Select(o => o.Quantidade.ToString(CultureInfo.InvariantCulture)));

            var sb = new StringBuilder();
            sb.AppendLine("<script>");
            sb.AppendLine("document.addEventListener('DOMContentLoaded', function () {");
            sb.AppendLine("new Chart(document.getElementById('chartVendas'), {");
            sb.AppendLine("  type: 'bar',");
            sb.AppendLine($"  data: {{ labels: [{labelsVendas}], datasets: [{{ label: 'Vendas', data: [{valoresVendas}], backgroundColor: '#1F7A5C' }}] }},");
            sb.AppendLine("  options: { responsive: true, plugins: { legend: { display: false } } }");
            sb.AppendLine("});");
            sb.AppendLine("new Chart(document.getElementById('chartPipeline'), {");
            sb.AppendLine("  type: 'doughnut',");
            sb.AppendLine($"  data: {{ labels: [{labelsPipeline}], datasets: [{{ data: [{valoresPipeline}], backgroundColor: ['#1F7A5C','#2E9E76','#6FBF9B','#A8D8C4','#12213B','#5C6F8C'] }}] }},");
            sb.AppendLine("  options: { responsive: true } ");
            sb.AppendLine("});");
            sb.AppendLine("new Chart(document.getElementById('chartOrigemLeads'), {");
            sb.AppendLine("  type: 'pie',");
            sb.AppendLine($"  data: {{ labels: [{labelsOrigem}], datasets: [{{ data: [{valoresOrigem}], backgroundColor: ['#1F7A5C','#2E9E76','#6FBF9B','#A8D8C4','#12213B','#5C6F8C','#E0A458'] }}] }},");
            sb.AppendLine("  options: { responsive: true } ");
            sb.AppendLine("});");
            sb.AppendLine("});");
            sb.AppendLine("</script>");

            litScriptGraficos.Text = sb.ToString();
        }

        protected string GetRelacionado(object dataItem)
        {
            var activity = (Activity)dataItem;

            if (activity.RelatedClientId.HasValue)
                return "Cliente: " + activity.RelatedClient?.TradeName;

            if (activity.RelatedLeadId.HasValue)
                return "Lead: " + activity.RelatedLead?.Name;

            if (activity.RelatedOpportunityId.HasValue)
                return "Oportunidade #" + activity.RelatedOpportunityId;

            return "";
        }
    }
}