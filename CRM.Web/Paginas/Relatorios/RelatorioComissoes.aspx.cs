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
    public partial class RelatorioComissoes : PaginaBase
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

                ucFiltroDatas.DataInicial = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
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

        private List<RelatorioComissoesLinha> ObterResultado(DateTime dataInicio, DateTime dataFim)
        {
            int? ownerId = string.IsNullOrEmpty(ddlComercial.SelectedValue) ? (int?)null : int.Parse(ddlComercial.SelectedValue);
            return _saleService.ObterRelatorioComissoes(dataInicio, dataFim, ownerId);
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            if (!ValidarFiltros(out DateTime dataInicio, out DateTime dataFim)) return;

            var linhas = ObterResultado(dataInicio, dataFim);

            phResultado.Visible = linhas.Count > 0;
            phVazio.Visible = linhas.Count == 0;

            rptLinhas.DataSource = linhas;
            rptLinhas.DataBind();

            litQuantidadeGeral.Text = linhas.Sum(l => l.QuantidadeVendas).ToString();
            litVendasGeral.Text = linhas.Sum(l => l.TotalVendas).ToString("C");
            litComissaoGeral.Text = linhas.Sum(l => l.TotalComissao).ToString("C");
        }

        protected void btnExportarCsv_Click(object sender, EventArgs e)
        {
            if (!ValidarFiltros(out DateTime dataInicio, out DateTime dataFim)) return;

            var linhas = ObterResultado(dataInicio, dataFim);

            Response.Clear();
            Response.ContentType = "text/csv; charset=utf-8";
            Response.AppendHeader("Content-Disposition", "attachment; filename=RelatorioComissoes_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".csv");

            var sb = new StringBuilder();
            sb.AppendLine("Relatório de Comissões");
            sb.AppendLine($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm} por {Session["UserName"]}");
            sb.AppendLine($"Período: {dataInicio:dd/MM/yyyy} a {dataFim:dd/MM/yyyy}");
            sb.AppendLine();
            sb.AppendLine("Comercial;Nº Vendas;Total Vendas;Total Comissão");

            foreach (var linha in linhas)
                sb.AppendLine($"{linha.Comercial};{linha.QuantidadeVendas};{linha.TotalVendas:0.00};{linha.TotalComissao:0.00}");

            Response.Write(sb.ToString());
            Response.End();
        }
    }
}