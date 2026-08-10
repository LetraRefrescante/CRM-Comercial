using CRM.Data.Helpers;
using CRM.Data.Repositories;
using CRM.Models.DTOs;
using CRM.Services;
using CRM.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CRM.Web.Paginas.Leads
{
    public partial class LeadConverter : PaginaBase
    {
        private readonly LeadService _leadService = new LeadService();
        private readonly LeadConversionService _leadConversionService = new LeadConversionService();
        private readonly CountryRepository _countryRepository = new CountryRepository();
        private readonly SectorRepository _sectorRepository = new SectorRepository();
        private readonly OpportunityStageRepository _opportunityStageRepository = new OpportunityStageRepository();
        private readonly UserRepository _userRepository = new UserRepository();

        private int LeadId
        {
            get
            {
                if (int.TryParse(Request.QueryString["id"], out int id)) return id;
                Response.Redirect("~/Leads/LeadsLista.aspx");
                return 0;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var lead = _leadService.GetById(LeadId);
            if (lead == null)
            {
                Response.Redirect("~/Leads/LeadsLista.aspx");
                return;
            }

            if (!_leadService.PodeConverter(Perfil) ||
                (_leadService.TemAmbitoProprios(Perfil) && lead.OwnerId != UserId))
            {
                Response.Redirect("~/AcessoNegado.aspx");
                return;
            }

            if (_leadService.EstaBloqueadoParaEdicao(lead))
            {
                Response.Redirect($"~/Leads/LeadDetalhe.aspx?id={lead.LeadId}");
                return;
            }

            litNomeLead.Text = Server.HtmlEncode(lead.Name);
            lnkBreadcrumbLead.Text = Server.HtmlEncode(lead.Name);
            lnkBreadcrumbLead.NavigateUrl = $"~/Leads/LeadDetalhe.aspx?id={lead.LeadId}";
            lnkCancelar.NavigateUrl = $"~/Leads/LeadDetalhe.aspx?id={lead.LeadId}";

            if (!IsPostBack)
            {
                CarregarPaises();
                CarregarSetores();
                CarregarComerciais();
                CarregarFasesIniciais();

                // Pré-preenchimento a partir do Lead — poupa trabalho repetido ao utilizador.
                txtNomeComercial.Text = lead.CompanyName ?? lead.Name;
                txtClienteEmail.Text = lead.Email;
                txtClienteTelefone.Text = lead.Phone;
                if (ddlComercialCliente.Items.FindByValue(lead.OwnerId.ToString()) != null)
                    ddlComercialCliente.SelectedValue = lead.OwnerId.ToString();

                txtContactoNome.Text = lead.Name;
                txtContactoEmail.Text = lead.Email;
                txtContactoTelefone.Text = lead.Phone;

                txtOportunidadeTitulo.Text = $"Oportunidade - {lead.CompanyName ?? lead.Name}";
                if (ddlComercialOportunidade.Items.FindByValue(lead.OwnerId.ToString()) != null)
                    ddlComercialOportunidade.SelectedValue = lead.OwnerId.ToString();
                txtDataFecho.Text = DateTime.Now.AddMonths(1).ToString("yyyy-MM-dd");
            }
        }

        private void CarregarPaises()
        {
            var paises = _countryRepository.ListarAtivos();

            ddlPais.Items.Clear();
            ddlPais.Items.Add(new System.Web.UI.WebControls.ListItem("Seleciona...", ""));
            foreach (var pais in paises)
            {
                ddlPais.Items.Add(new System.Web.UI.WebControls.ListItem(pais.Name, pais.CountryId.ToString()));
            }

            // Portugal pré-selecionado quando existir — é o país mais comum na carteira.
            var portugal = paises.FirstOrDefault(p => p.IsoCode == "PT");
            if (portugal != null)
            {
                ddlPais.ClearSelection();
                ddlPais.Items.FindByValue(portugal.CountryId.ToString()).Selected = true;
            }
        }

        private void CarregarSetores()
        {
            ddlSetor.Items.Clear();
            ddlSetor.Items.Add(new System.Web.UI.WebControls.ListItem("Nenhum", ""));
            foreach (var setor in _sectorRepository.ListarAtivos())
            {
                ddlSetor.Items.Add(new System.Web.UI.WebControls.ListItem(setor.Name, setor.SectorId.ToString()));
            }
        }

        private void CarregarComerciais()
        {
            ddlComercialCliente.Items.Clear();
            ddlComercialOportunidade.Items.Clear();

            foreach (var user in _userRepository.ListarComerciaisAtivos())
            {
                ddlComercialCliente.Items.Add(new System.Web.UI.WebControls.ListItem(user.Name, user.UserId.ToString()));
                ddlComercialOportunidade.Items.Add(new System.Web.UI.WebControls.ListItem(user.Name, user.UserId.ToString()));
            }

            // Um Comercial converte sempre para si próprio — mesma lógica de âmbito
            // "PRÓPRIOS" usada em LeadEditar.aspx.cs.
            bool podeEscolherComercial = !_leadService.TemAmbitoProprios(Perfil);
            ddlComercialCliente.Enabled = podeEscolherComercial;
            ddlComercialOportunidade.Enabled = podeEscolherComercial;
        }

        private void CarregarFasesIniciais()
        {
            ddlFaseInicial.Items.Clear();
            foreach (var fase in _opportunityStageRepository.ListarAtivasParaAbertura())
            {
                ddlFaseInicial.Items.Add(new System.Web.UI.WebControls.ListItem(fase.Name, fase.StageId.ToString()));
            }
        }

        protected void rblTipoCliente_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool clienteNovo = rblTipoCliente.SelectedValue == "Novo";
            phClienteNovo.Visible = clienteNovo;
            phClienteExistente.Visible = !clienteNovo;
        }

        protected void chkCriarContacto_CheckedChanged(object sender, EventArgs e)
        {
            phContacto.Visible = chkCriarContacto.Checked;
        }

        protected void chkCriarOportunidade_CheckedChanged(object sender, EventArgs e)
        {
            phOportunidade.Visible = chkCriarOportunidade.Checked;
        }

        private LeadConversionRequest MontarPedido()
        {
            bool podeEscolherComercial = !_leadService.TemAmbitoProprios(Perfil);

            var request = new LeadConversionRequest
            {
                LeadId = LeadId,
                UserId = UserId,
                CriarContacto = chkCriarContacto.Checked,
                CriarOportunidade = chkCriarOportunidade.Checked
            };

            if (rblTipoCliente.SelectedValue == "Existente")
            {
                request.ClienteExistenteId = ucSeletorCliente.ClienteId > 0 ? ucSeletorCliente.ClienteId : (int?)null;
            }
            else
            {
                request.NovoClienteNif = txtNif.Text.Trim();
                request.NovoClienteNomeComercial = txtNomeComercial.Text.Trim();
                request.NovoClienteNomeLegal = txtNomeLegal.Text.Trim();
                request.NovoClienteEmail = txtClienteEmail.Text.Trim();
                request.NovoClienteTelefone = txtClienteTelefone.Text.Trim();
                request.NovoClienteCountryId = string.IsNullOrEmpty(ddlPais.SelectedValue) ? (int?)null : int.Parse(ddlPais.SelectedValue);
                request.NovoClienteSectorId = string.IsNullOrEmpty(ddlSetor.SelectedValue) ? (int?)null : int.Parse(ddlSetor.SelectedValue);
                request.NovoClienteAccountManagerId = podeEscolherComercial
                    ? (string.IsNullOrEmpty(ddlComercialCliente.SelectedValue) ? 0 : int.Parse(ddlComercialCliente.SelectedValue))
                    : UserId;
            }

            if (request.CriarContacto)
            {
                request.ContactoNome = txtContactoNome.Text.Trim();
                request.ContactoCargo = txtContactoCargo.Text.Trim();
                request.ContactoEmail = txtContactoEmail.Text.Trim();
                request.ContactoTelefone = txtContactoTelefone.Text.Trim();
            }

            if (request.CriarOportunidade)
            {
                request.OportunidadeTitulo = txtOportunidadeTitulo.Text.Trim();
                request.OportunidadeStageId = string.IsNullOrEmpty(ddlFaseInicial.SelectedValue) ? 0 : int.Parse(ddlFaseInicial.SelectedValue);
                request.OportunidadeValorEstimado = string.IsNullOrWhiteSpace(txtValorEstimado.Text)
                    ? 0
                    : decimal.Parse(txtValorEstimado.Text, CultureInfo.InvariantCulture);
                request.OportunidadeDataFechoPrevista = string.IsNullOrWhiteSpace(txtDataFecho.Text)
                    ? default(DateTime)
                    : DateTime.Parse(txtDataFecho.Text);
                request.OportunidadeOwnerId = podeEscolherComercial
                    ? (string.IsNullOrEmpty(ddlComercialOportunidade.SelectedValue) ? 0 : int.Parse(ddlComercialOportunidade.SelectedValue))
                    : UserId;
            }

            return request;
        }

        protected void btnConverter_Click(object sender, EventArgs e)
        {
            if (rblTipoCliente.SelectedValue == "Existente" && !ucSeletorCliente.ClienteId.HasValue)
            {
                MostrarErros(new List<string> { "Seleciona um cliente existente." });
                return;
            }

            var request = MontarPedido();
            var erros = _leadConversionService.Validar(request);

            if (erros.Count > 0)
            {
                MostrarErros(erros);
                return;
            }

            try
            {
                var resultado = _leadConversionService.Converter(request);
                NotificacaoService.Sucesso("Lead convertido com sucesso.");
                Response.Redirect($"~/Clientes/ClienteDetalhe.aspx?id={resultado.ClientId}");
            }
            catch (AplicacaoException ex)
            {
                MostrarErros(new List<string> { ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                MostrarErros(new List<string> { ex.Message });
            }
        }

        private void MostrarErros(List<string> erros)
        {
            lblErros.Text = string.Join("<br />", erros.Select(Server.HtmlEncode));
            lblErros.Visible = true;
        }
    }
}