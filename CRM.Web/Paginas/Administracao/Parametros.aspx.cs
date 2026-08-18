using System;
using System.Web.UI.WebControls;
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
            if (!_settingsService.PodeGerir(Perfil))
            {
                NotificacaoService.Erro("Só o Administrador pode alterar os parâmetros do sistema.");
                Response.Redirect("~/Dashboard/Dashboard.aspx");
                return;
            }

            if (!IsPostBack)
                CarregarParametros();
        }

        private void CarregarParametros()
        {
            var settings = _settingsService.Obter();

            txtEmpresa.Text = settings.CompanyName;

            ddlMoeda.SelectedValue =
                string.IsNullOrEmpty(settings.Currency)
                    ? "EUR"
                    : settings.Currency;

            ddlFusoHorario.SelectedValue =
                string.IsNullOrEmpty(settings.TimeZone)
                    ? "Europe/Lisbon"
                    : settings.TimeZone;

            txtDiasAlerta.Text = settings.AlertDaysProposals.ToString();
        }

        protected void cvRegrasNegocio_ServerValidate(
            object source,
            ServerValidateEventArgs args)
        {
            var settings = MontarDoFormulario();

            var erros = _settingsService.Validar(settings);

            args.IsValid = erros.Count == 0;

            cvRegrasNegocio.ErrorMessage =
                string.Join(" ", erros);
        }

        private Settings MontarDoFormulario()
        {
            int.TryParse(txtDiasAlerta.Text, out int dias);

            return new Settings
            {
                CompanyName = txtEmpresa.Text.Trim(),
                Currency = ddlMoeda.SelectedValue,
                TimeZone = ddlFusoHorario.SelectedValue,

                AlertDaysProposals = dias
            };
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            _settingsService.Guardar(
                MontarDoFormulario(),
                UserId);

            NotificacaoService.Sucesso(
                "Parâmetros atualizados.");
        }
    }
}