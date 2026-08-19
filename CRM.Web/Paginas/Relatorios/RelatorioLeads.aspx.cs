using System;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using CRM.Data.Repositories;
using CRM.Models.DTOs;
using CRM.Services;
using CRM.Web.Helpers;

namespace CRM.Web.Paginas.Relatorios
{
    public partial class RelatorioLeads : PaginaBase
    {
        private readonly RelatorioService _relatorioService = new RelatorioService();
        private readonly LeadSourceRepository _leadSourceRepository = new LeadSourceRepository();
        private readonly UserRepository _userRepository = new UserRepository();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!_relatorioService.PodeAcederGeral(Perfil))
            {
                NotificacaoService.Erro("Não tens permissão para aceder a este relatório.");
                Response.Redirect("~/Dashboard/Dashboard.aspx");
                return;
            }

            if (!IsPostBack)
            {
                ddlOrigem.Items.Clear();
                ddlOrigem.Items.Add(new ListItem("Todas", ""));
                foreach (var origem in _leadSourceRepository.ListarAtivos())
                    ddlOrigem.Items.Add(new ListItem(origem.Name, origem.LeadSourceId.ToString()));

                ddlComercial.Items.Clear();
                ddlComercial.Items.Add(new ListItem("Todos", ""));
                foreach (var user in _userRepository.ListarComerciaisAtivos())
                    ddlComercial.Items.Add(new ListItem(user.Name, user.UserId.ToString()));
            }
        }

        private System.Collections.Generic.List<RelatorioLeadsLinha> ObterResultado()
        {
            int? leadSourceId = string.IsNullOrEmpty(ddlOrigem.SelectedValue) ? (int?)null : int.Parse(ddlOrigem.SelectedValue);
            int? ownerId = string.IsNullOrEmpty(ddlComercial.SelectedValue) ? (int?)null : int.Parse(ddlComercial.SelectedValue);
            int? scoreMin = int.TryParse(txtScoreMin.Text, out int sMin) ? sMin : (int?)null;
            int? scoreMax = int.TryParse(txtScoreMax.Text, out int sMax) ? sMax : (int?)null;

            return _relatorioService.ObterRelatorioLeads(
                ucFiltroDatas.DataInicial, ucFiltroDatas.DataFinal, leadSourceId,
                ddlEstado.SelectedValue, ownerId, scoreMin, scoreMax);
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            if (!ValidarPeriodo()) return;

            var linhas = ObterResultado();

            phResultado.Visible = linhas.Count > 0;
            phVazio.Visible = linhas.Count == 0;

            rptLinhas.DataSource = linhas;
            rptLinhas.DataBind();

            int quantidadeGeral = linhas.Sum(l => l.Quantidade);
            int convertidosGeral = linhas.Sum(l => l.Convertidos);

            litQuantidadeGeral.Text = quantidadeGeral.ToString();
            litConvertidosGeral.Text = convertidosGeral.ToString();
            litTaxaGeral.Text = (quantidadeGeral == 0 ? 0 : Math.Round(convertidosGeral * 100m / quantidadeGeral, 1)) + "%";
        }

        protected void btnExportarCsv_Click(object sender, EventArgs e)
        {
            var linhas = ObterResultado();

            Response.Clear();
            Response.ContentType = "text/csv; charset=utf-8";
            Response.AppendHeader("Content-Disposition", "attachment; filename=RelatorioLeads_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".csv");

            var sb = new StringBuilder();
            sb.AppendLine("Relatório de Leads");
            sb.AppendLine($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm} por {Session["UserName"]}");
            sb.AppendLine();
            sb.AppendLine("Origem;Quantidade;Convertidos;Taxa de Conversão");

            foreach (var linha in linhas)
                sb.AppendLine($"{linha.Origem};{linha.Quantidade};{linha.Convertidos};{linha.TaxaConversao}%");

            Response.Write(sb.ToString());
            Response.End();
        }
        private bool ValidarPeriodo()
        {
            if (ucFiltroDatas.DataInicial.HasValue && ucFiltroDatas.DataFinal.HasValue
                && ucFiltroDatas.DataInicial > ucFiltroDatas.DataFinal)
            {
                NotificacaoService.Erro("A data inicial não pode ser posterior à data final.");
                return false;
            }
            return true;
        }
    }
}