using System;
using CRM.Models.Entities.ListasAuxiliares;
using CRM.Services;
using CRM.Web.Helpers;

namespace CRM.Web.Paginas.Administracao
{
    public partial class Parametros : PaginaBase
    {
        private readonly SettingsService _settingsService = new SettingsService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!_settingsService.PodeConsultar(Perfil))
            {
                NotificacaoService.Erro("Não tens permissão para aceder a esta página.");
                Response.Redirect("~/Dashboard/Dashboard.aspx");
                return;
            }

            if (!IsPostBack)
                CarregarConfiguracao();
        }

        private void CarregarConfiguracao()
        {
            var settings = _settingsService.ObterConfiguracaoAtual();
            if (settings == null)
            {
                NotificacaoService.Erro("Ainda não existe nenhuma linha de configuração na tabela Settings.");
                return;
            }

            txtNomeEmpresa.Text = settings.CompanyName;
            txtMoeda.Text = settings.Currency;
            txtFusoHorario.Text = settings.TimeZone;
            txtAlertaLeads.Text = settings.AlertDaysLeads.ToString();
            txtAlertaOportunidades.Text = settings.AlertDaysOpportunities.ToString();
            txtAlertaPropostas.Text = settings.AlertDaysProposals.ToString();
            txtMaxTentativas.Text = settings.MaxFailedLoginAttempts.ToString();
            txtBloqueioMinutos.Text = settings.AccountLockoutMinutes.ToString();
            txtSessaoMinutos.Text = settings.SessionTimeoutMinutes.ToString();

            bool podeGerir = _settingsService.PodeGerir(Perfil);
            btnGuardar.Visible = podeGerir;

            txtNomeEmpresa.Enabled = podeGerir;
            txtMoeda.Enabled = podeGerir;
            txtFusoHorario.Enabled = podeGerir;
            txtAlertaLeads.Enabled = podeGerir;
            txtAlertaOportunidades.Enabled = podeGerir;
            txtAlertaPropostas.Enabled = podeGerir;
            txtMaxTentativas.Enabled = podeGerir;
            txtBloqueioMinutos.Enabled = podeGerir;
            txtSessaoMinutos.Enabled = podeGerir;
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!_settingsService.PodeGerir(Perfil))
            {
                NotificacaoService.Erro("Não tens permissão para executar esta ação.");
                return;
            }

            int.TryParse(txtAlertaLeads.Text, out int alertaLeads);
            int.TryParse(txtAlertaOportunidades.Text, out int alertaOportunidades);
            int.TryParse(txtAlertaPropostas.Text, out int alertaPropostas);
            int.TryParse(txtMaxTentativas.Text, out int maxTentativas);
            int.TryParse(txtBloqueioMinutos.Text, out int bloqueioMinutos);
            int.TryParse(txtSessaoMinutos.Text, out int sessaoMinutos);

            var settings = new Settings
            {
                CompanyName = txtNomeEmpresa.Text.Trim(),
                Currency = txtMoeda.Text.Trim().ToUpperInvariant(),
                TimeZone = txtFusoHorario.Text.Trim(),
                AlertDaysLeads = alertaLeads,
                AlertDaysOpportunities = alertaOportunidades,
                AlertDaysProposals = alertaPropostas,
                MaxFailedLoginAttempts = maxTentativas,
                AccountLockoutMinutes = bloqueioMinutos,
                SessionTimeoutMinutes = sessaoMinutos,
                UpdatedBy = UserId
            };

            var erros = _settingsService.Validar(settings);
            if (erros.Count > 0)
            {
                NotificacaoService.Erro(string.Join(" ", erros));
                return;
            }

            _settingsService.Atualizar(settings);
            NotificacaoService.Sucesso("Parâmetros atualizados.");
            CarregarConfiguracao();
        }
    }
}