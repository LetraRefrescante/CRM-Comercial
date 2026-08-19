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
    public partial class RelatorioClientes : PaginaBase
    {
        private readonly ClientService _clientService = new ClientService();
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

        private List<RelatorioClientesLinha> ObterResultado()
        {
            int? accountManagerId = string.IsNullOrEmpty(ddlComercial.SelectedValue) ? (int?)null : int.Parse(ddlComercial.SelectedValue);
            return _clientService.ObterRelatorioCarteira(ddlEstado.SelectedValue, accountManagerId);
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            var linhas = ObterResultado();

            phResultado.Visible = linhas.Count > 0;
            phVazio.Visible = linhas.Count == 0;

            rptLinhas.DataSource = linhas;
            rptLinhas.DataBind();

            litTotalGeral.Text = linhas.Sum(l => l.Total).ToString();
        }

        protected void btnExportarCsv_Click(object sender, EventArgs e)
        {
            var linhas = ObterResultado();

            Response.Clear();
            Response.ContentType = "text/csv; charset=utf-8";
            Response.AppendHeader("Content-Disposition", "attachment; filename=RelatorioClientes_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".csv");

            var sb = new StringBuilder();
            sb.AppendLine("Relatório de Clientes");
            sb.AppendLine($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm} por {Session["UserName"]}");
            sb.AppendLine();
            sb.AppendLine("Setor;Total;Potenciais;Ativos;Inativos;Bloqueados");

            foreach (var linha in linhas)
                sb.AppendLine($"{linha.Setor};{linha.Total};{linha.Potenciais};{linha.Ativos};{linha.Inativos};{linha.Bloqueados}");

            Response.Write(sb.ToString());
            Response.End();
        }
    }
}