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
    public partial class RelatorioAtividades : PaginaBase
    {
        private readonly ActivityService _activityService = new ActivityService();
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
                ddlResponsavel.Items.Clear();
                ddlResponsavel.Items.Add(new ListItem("Todos", ""));
                foreach (var user in _userRepository.ListarAtivos())
                    ddlResponsavel.Items.Add(new ListItem(user.Name, user.UserId.ToString()));
            }
        }

        private List<RelatorioAtividadesLinha> ObterResultado()
        {
            int? assignedToUserId = string.IsNullOrEmpty(ddlResponsavel.SelectedValue) ? (int?)null : int.Parse(ddlResponsavel.SelectedValue);

            return _activityService.ObterRelatorioProdutividade(
                ucFiltroDatas.DataInicial, ucFiltroDatas.DataFinal, assignedToUserId,
                ddlTipo.SelectedValue, ddlEstado.SelectedValue);
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            if (!ValidarPeriodo()) return;

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
            Response.AppendHeader("Content-Disposition", "attachment; filename=RelatorioAtividades_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".csv");

            var sb = new StringBuilder();
            sb.AppendLine("Relatório de Atividades");
            sb.AppendLine($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm} por {Session["UserName"]}");
            sb.AppendLine();
            sb.AppendLine("Responsável;Total;Planeadas;Em Curso;Concluídas;Canceladas");

            foreach (var linha in linhas)
                sb.AppendLine($"{linha.Responsavel};{linha.Total};{linha.Planeadas};{linha.EmCurso};{linha.Concluidas};{linha.Canceladas}");

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