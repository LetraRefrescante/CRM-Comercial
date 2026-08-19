using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using CRM.Data.Repositories;
using CRM.Models.DTOs;
using CRM.Services;
using CRM.Web.Helpers;

namespace CRM.Web.Paginas.Relatorios
{
    public partial class RelatorioVendas : PaginaBase
    {
        private readonly SaleService _saleService = new SaleService();
        private readonly RelatorioService _relatorioService = new RelatorioService();
        private readonly UserRepository _userRepository = new UserRepository();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!_relatorioService.PodeAcederFinanceiro(Perfil))
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

                ucFiltroDatas.DataInicial = new DateTime(DateTime.Today.Year, 1, 1);
                ucFiltroDatas.DataFinal = DateTime.Today;
            }
        }

        private bool ValidarFiltros(out DateTime dataInicio, out DateTime dataFim)
        {
            dataInicio = ucFiltroDatas.DataInicial ?? DateTime.Today.AddMonths(-1);
            dataFim = ucFiltroDatas.DataFinal ?? DateTime.Today;

            if (dataInicio > dataFim)
            {
                NotificacaoService.Erro("A data inicial não pode ser posterior à data final.");
                return false;
            }
            return true;
        }

        private RelatorioVendasResultado ObterResultado(DateTime dataInicio, DateTime dataFim)
        {
            int? clientId = ucCliente.ClienteId;
            int? ownerId = string.IsNullOrEmpty(ddlComercial.SelectedValue) ? (int?)null : int.Parse(ddlComercial.SelectedValue);

            var estados = new List<string>();
            foreach (ListItem item in cblEstados.Items)
                if (item.Selected) estados.Add(item.Value);

            return _saleService.ObterRelatorio(dataInicio, dataFim, clientId, ownerId, estados, ddlAgrupamento.SelectedValue);
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            if (!ValidarFiltros(out DateTime dataInicio, out DateTime dataFim)) return;

            var resultado = ObterResultado(dataInicio, dataFim);

            phResultado.Visible = resultado.Linhas.Count > 0;
            phVazio.Visible = resultado.Linhas.Count == 0;

            rptLinhas.DataSource = resultado.Linhas;
            rptLinhas.DataBind();

            litQuantidadeGeral.Text = resultado.QuantidadeGeral.ToString();
            litTotalGeral.Text = resultado.TotalGeral.ToString("C");
        }

        protected void btnExportarCsv_Click(object sender, EventArgs e)
        {
            if (!ValidarFiltros(out DateTime dataInicio, out DateTime dataFim)) return;

            var resultado = ObterResultado(dataInicio, dataFim);

            Response.Clear();
            Response.ContentType = "text/csv; charset=utf-8";
            Response.AppendHeader("Content-Disposition", "attachment; filename=RelatorioVendas_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".csv");

            var sb = new StringBuilder();
            sb.AppendLine("Relatório de Vendas");
            sb.AppendLine($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm} por {Session["UserName"]}");
            sb.AppendLine($"Período: {dataInicio:dd/MM/yyyy} a {dataFim:dd/MM/yyyy}");
            sb.AppendLine($"Agrupamento: {ddlAgrupamento.SelectedItem.Text}");
            sb.AppendLine();
            sb.AppendLine("Período;Quantidade;Subtotal;IVA;Total");

            foreach (var linha in resultado.Linhas)
                sb.AppendLine($"{linha.Periodo};{linha.Quantidade};{linha.SubTotal:0.00};{linha.TaxTotal:0.00};{linha.Total:0.00}");

            sb.AppendLine();
            sb.AppendLine($"Total Geral;{resultado.QuantidadeGeral};;;{resultado.TotalGeral:0.00}");

            Response.Write(sb.ToString());
            Response.End();
        }
    }
}