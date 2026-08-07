using System;
using CRM.Services;
using CRM.Data.Repositories;

namespace CRM.Web.Controls
{
    public partial class SeletorCliente : System.Web.UI.UserControl
    {
        private readonly ClientRepository _clientRepository = new ClientRepository();
        private readonly ClientService _clientService = new ClientService();

        private string Perfil => Session["RoleName"] as string ?? "";
        private int UserId => Session["UserId"] != null ? (int)Session["UserId"] : 0;

        public int? ClienteId
        {
            get => int.TryParse(hdnClienteId.Value, out int id) ? id : (int?)null;
            set
            {
                hdnClienteId.Value = value?.ToString() ?? "";
                txtClienteNome.Text = value.HasValue
                    ? _clientRepository.GetById(value.Value)?.TradeName ?? ""
                    : "";
            }
        }

        public bool Enabled
        {
            get => !btnAbrirSeletor.Disabled;
            set
            {
                txtClienteNome.Enabled = value;
                txtPesquisa.Enabled = value;
                btnPesquisar.Enabled = value;
                btnAbrirSeletor.Disabled = !value;
            }
        }

        public bool Obrigatorio
        {
            get => cvClienteObrigatorio.Enabled;
            set => cvClienteObrigatorio.Enabled = value;
        }
        public bool OcultarCampoTexto { get; set; }
        public string TextoBotao { get; set; } = "Escolher";
        public string IconeBotao { get; set; } = "fa-search";
        public string CssClassBotao { get; set; } = "btn btn-outline-secondary";

        public event EventHandler ClienteSelecionado;

        protected void Page_Load(object sender, EventArgs e)
        {
            txtClienteNome.Visible = !OcultarCampoTexto;

            // Quando o campo de texto está oculto, o wrapper deixa de precisar
            // do comportamento "input-group" (width:100% + max-width:340px do Bootstrap),
            // senão o botão fica esticado e sobra espaço vazio ao lado.
            if (OcultarCampoTexto)
            {
                divSeletor.Attributes.Remove("class");
                divSeletor.Attributes.Remove("style");
            }

            btnAbrirSeletor.InnerHtml = $"<i class=\"fas {IconeBotao}\"></i> {System.Web.HttpUtility.HtmlEncode(TextoBotao)}";
            btnAbrirSeletor.Attributes["class"] = CssClassBotao;
            btnAbrirSeletor.Attributes["data-bs-target"] = "#" + mdlSeletor.ClientID;

            if (!IsPostBack)
            {
                Pesquisar();
            }
        }

        protected void btnPesquisar_Click(object sender, EventArgs e)
        {
            Pesquisar();
        }

        private void Pesquisar()
        {
            int? filtroComercial = _clientService.TemAmbitoProprios(Perfil) ? UserId : (int?)null;

            var resultados = _clientRepository.Listar(
                pesquisa: txtPesquisa.Text.Trim(),
                status: "Ativo",
                accountManagerId: filtroComercial,
                pagina: 1,
                tamanhoPagina: 50,
                totalRegistos: out int _);

            rptResultados.DataSource = resultados;
            rptResultados.DataBind();
            phSemResultados.Visible = resultados.Count == 0;
        }

        protected void rptResultados_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Escolher") return;

            var partes = e.CommandArgument.ToString().Split('|');
            hdnClienteId.Value = partes[0];
            txtClienteNome.Text = partes[1];

            ClienteSelecionado?.Invoke(this, EventArgs.Empty);
        }

        protected void cvClienteObrigatorio_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            args.IsValid = ClienteId.HasValue;
        }
    }
}