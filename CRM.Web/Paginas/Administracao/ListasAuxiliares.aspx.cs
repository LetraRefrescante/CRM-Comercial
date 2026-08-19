using System;
using System.Web.UI.WebControls;
using CRM.Services;
using CRM.Web.Helpers;

namespace CRM.Web.Paginas.Administracao
{
    public partial class ListasAuxiliares : PaginaBase
    {
        private readonly AuxiliaryListService _auxiliaryListService = new AuxiliaryListService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!_auxiliaryListService.PodeConsultar(Perfil))
            {
                NotificacaoService.Erro("Não tens permissão para aceder a esta página.");
                Response.Redirect("~/Dashboard/Dashboard.aspx");
                return;
            }

            bool podeGerir = _auxiliaryListService.PodeGerir(Perfil);
            phFormSetor.Visible = podeGerir;
            phFormOrigem.Visible = podeGerir;
            phFormMotivo.Visible = podeGerir;
            phFormTaxa.Visible = podeGerir;
            phFormCondicao.Visible = podeGerir;
            phFormPais.Visible = podeGerir;

            if (!IsPostBack)
                CarregarTudo();
        }

        private void CarregarTudo()
        {
            rptSetores.DataSource = _auxiliaryListService.ListarSetores(true);
            rptSetores.DataBind();

            rptOrigens.DataSource = _auxiliaryListService.ListarOrigensLead(true);
            rptOrigens.DataBind();

            rptMotivos.DataSource = _auxiliaryListService.ListarMotivosPerda(true);
            rptMotivos.DataBind();

            rptTaxas.DataSource = _auxiliaryListService.ListarTaxasIva(true);
            rptTaxas.DataBind();

            rptCondicoes.DataSource = _auxiliaryListService.ListarCondicoesPagamento(true);
            rptCondicoes.DataBind();

            rptPaises.DataSource = _auxiliaryListService.ListarPaises(true);
            rptPaises.DataBind();
        }

        protected void btnAddSetor_Click(object sender, EventArgs e)
        {
            var erro = _auxiliaryListService.CriarSetor(txtNovoSetor.Text, UserId);
            if (erro != null) { NotificacaoService.Erro(erro); return; }

            txtNovoSetor.Text = "";
            NotificacaoService.Sucesso("Setor criado.");
            CarregarTudo();
        }

        protected void rptSetores_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Alternar") return;
            _auxiliaryListService.AlternarEstadoSetor(int.Parse(e.CommandArgument.ToString()), UserId);
            CarregarTudo();
        }

        protected void btnAddOrigem_Click(object sender, EventArgs e)
        {
            var erro = _auxiliaryListService.CriarOrigemLead(txtNovaOrigem.Text, UserId);
            if (erro != null) { NotificacaoService.Erro(erro); return; }

            txtNovaOrigem.Text = "";
            NotificacaoService.Sucesso("Origem criada.");
            CarregarTudo();
        }

        protected void rptOrigens_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Alternar") return;
            _auxiliaryListService.AlternarEstadoOrigemLead(int.Parse(e.CommandArgument.ToString()), UserId);
            CarregarTudo();
        }

        protected void btnAddMotivo_Click(object sender, EventArgs e)
        {
            var erro = _auxiliaryListService.CriarMotivoPerda(txtNovoMotivo.Text, UserId);
            if (erro != null) { NotificacaoService.Erro(erro); return; }

            txtNovoMotivo.Text = "";
            NotificacaoService.Sucesso("Motivo criado.");
            CarregarTudo();
        }

        protected void rptMotivos_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Alternar") return;
            _auxiliaryListService.AlternarEstadoMotivoPerda(int.Parse(e.CommandArgument.ToString()), UserId);
            CarregarTudo();
        }

        protected void btnAddTaxa_Click(object sender, EventArgs e)
        {
            decimal.TryParse(txtNovaTaxaPercentagem.Text, out decimal percentagem);
            var erro = _auxiliaryListService.CriarTaxaIva(txtNovaTaxaNome.Text, percentagem, UserId);
            if (erro != null) { NotificacaoService.Erro(erro); return; }

            txtNovaTaxaNome.Text = "";
            txtNovaTaxaPercentagem.Text = "";
            NotificacaoService.Sucesso("Taxa criada.");
            CarregarTudo();
        }

        protected void rptTaxas_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Alternar") return;
            _auxiliaryListService.AlternarEstadoTaxaIva(int.Parse(e.CommandArgument.ToString()), UserId);
            CarregarTudo();
        }

        protected void btnAddCondicao_Click(object sender, EventArgs e)
        {
            int? dias = int.TryParse(txtNovaCondicaoDias.Text, out int d) ? d : (int?)null;
            var erro = _auxiliaryListService.CriarCondicaoPagamento(txtNovaCondicaoNome.Text, dias, UserId);
            if (erro != null) { NotificacaoService.Erro(erro); return; }

            txtNovaCondicaoNome.Text = "";
            txtNovaCondicaoDias.Text = "";
            NotificacaoService.Sucesso("Condição criada.");
            CarregarTudo();
        }

        protected void rptCondicoes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Alternar") return;
            _auxiliaryListService.AlternarEstadoCondicaoPagamento(int.Parse(e.CommandArgument.ToString()), UserId);
            CarregarTudo();
        }

        protected void btnAddPais_Click(object sender, EventArgs e)
        {
            var erro = _auxiliaryListService.CriarPais(txtNovoPaisCodigo.Text, txtNovoPaisNome.Text, UserId);
            if (erro != null) { NotificacaoService.Erro(erro); return; }

            txtNovoPaisCodigo.Text = "";
            txtNovoPaisNome.Text = "";
            NotificacaoService.Sucesso("País criado.");
            CarregarTudo();
        }

        protected void rptPaises_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Alternar") return;
            _auxiliaryListService.AlternarEstadoPais(int.Parse(e.CommandArgument.ToString()), UserId);
            CarregarTudo();
        }

        protected string GetTextoClasse(object isActiveObj) => (bool)isActiveObj ? "" : "text-muted text-decoration-line-through";
        protected string GetTextoBotaoEstado(object isActiveObj) => (bool)isActiveObj ? "Inativar" : "Ativar";
        protected string GetDiasVencimentoTexto(object daysDueObj) => daysDueObj == null ? "" : $" ({daysDueObj} dias)";
    }
}