using System;
using System.Collections.Generic;
using CRM.Services;
using CRM.Web.Helpers;

namespace CRM.Web.Paginas.Relatorios
{
    public partial class Relatorios : PaginaBase
    {
        private readonly RelatorioService _relatorioService = new RelatorioService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            var itens = new List<object>();

            if (_relatorioService.PodeAcederFinanceiro(Perfil))
            {
                itens.Add(new { Nome = "Vendas", Descricao = "Análise de vendas por período.", Url = "RelatorioVendas.aspx" });
                itens.Add(new { Nome = "Comissões", Descricao = "Comissões por comercial.", Url = "RelatorioComissoes.aspx" });
            }

            if (_relatorioService.PodeAcederGeral(Perfil))
            {
                itens.Add(new { Nome = "Pipeline", Descricao = "Oportunidades por fase e valor.", Url = "RelatorioPipeline.aspx" });
                itens.Add(new { Nome = "Leads", Descricao = "Conversão e origem de leads.", Url = "RelatorioLeads.aspx" });
                itens.Add(new { Nome = "Atividades", Descricao = "Produtividade comercial.", Url = "RelatorioAtividades.aspx" });
                itens.Add(new { Nome = "Clientes", Descricao = "Carteira e segmentação.", Url = "RelatorioClientes.aspx" });
            }

            rptRelatorios.DataSource = itens;
            rptRelatorios.DataBind();
        }
    }
}