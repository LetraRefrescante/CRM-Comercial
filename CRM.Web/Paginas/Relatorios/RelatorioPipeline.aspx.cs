using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using CRM.Data.Repositories;
using CRM.Models.DTOs;
using CRM.Services;
using CRM.Web.Helpers;

namespace CRM.Web.Paginas.Relatorios
{
    public partial class RelatorioPipeline : PaginaBase
    {
        private readonly OpportunityService _opportunityService = new OpportunityService();
        private readonly RelatorioService _relatorioService = new RelatorioService();
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
                ddlComercial.Items.Clear();
                ddlComercial.Items.Add(new ListItem("Todos", ""));
                foreach (var user in _userRepository.ListarComerciaisAtivos())
                    ddlComercial.Items.Add(new ListItem(user.Name, user.UserId.ToString()));
            }
        }

        private List<RelatorioPipelineLinha> ObterResultado()
        {
            int? clientId = ucCliente.ClienteId;
            int? ownerId = string.IsNullOrEmpty(ddlComercial.SelectedValue) ? (int?)null : int.Parse(ddlComercial.SelectedValue);
            bool? isClosed = ddlEstado.SelectedValue == "aberta" ? false : ddlEstado.SelectedValue == "fechada" ? true : (bool?)null;

            return _opportunityService.ObterRelatorioPipeline(ucFiltroDatas.DataInicial, ucFiltroDatas.DataFinal, clientId, ownerId, isClosed);
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            if (!ValidarPeriodo()) return;

            var linhas = ObterResultado();

            phResultado.Visible = linhas.Count > 0;
            phVazio.Visible = linhas.Count == 0;

            rptLinhas.DataSource = linhas;
            rptLinhas.DataBind();

            litQuantidadeGeral.Text = linhas.Sum(l => l.Quantidade).ToString();
            litValorGeral.Text = linhas.Sum(l => l.ValorTotal).ToString("C");
            litValorPonderadoGeral.Text = linhas.Sum(l => l.ValorPonderado).ToString("C");
        }

        protected void btnExportarCsv_Click(object sender, EventArgs e)
        {
            var linhas = ObterResultado();

            Response.Clear();
            Response.ContentType = "text/csv; charset=utf-8";
            Response.AppendHeader("Content-Disposition", "attachment; filename=RelatorioPipeline_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".csv");

            var sb = new StringBuilder();
            sb.AppendLine("Relatório de Pipeline");
            sb.AppendLine($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm} por {Session["UserName"]}");
            sb.AppendLine();
            sb.AppendLine("Fase;Quantidade;Valor Estimado;Valor Ponderado");

            foreach (var linha in linhas)
                sb.AppendLine($"{linha.Fase};{linha.Quantidade};{linha.ValorTotal:0.00};{linha.ValorPonderado:0.00}");

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