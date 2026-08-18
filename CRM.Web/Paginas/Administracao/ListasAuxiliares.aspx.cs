using System;
using System.Web.UI.WebControls;
using CRM.Models.Entities.Catalogo;
using CRM.Models.Entities.ListasAuxiliares;
using CRM.Services;
using CRM.Web.Helpers;

namespace CRM.Web.Paginas.Administracao
{
    public partial class ListasAuxiliares : PaginaBase
    {
        private readonly LeadSourceService _leadSourceService = new LeadSourceService();
        private readonly LossReasonService _lossReasonService = new LossReasonService();
        private readonly PaymentTermService _paymentTermService = new PaymentTermService();
        private readonly TaxRateService _taxRateService = new TaxRateService();

        private int? LeadSourceIdEmEdicao { get => ViewState["LSId"] as int?; set => ViewState["LSId"] = value; }
        private int? LossReasonIdEmEdicao { get => ViewState["LRId"] as int?; set => ViewState["LRId"] = value; }
        private int? PaymentTermIdEmEdicao { get => ViewState["PTId"] as int?; set => ViewState["PTId"] = value; }
        private int? TaxRateIdEmEdicao { get => ViewState["TRId"] as int?; set => ViewState["TRId"] = value; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!_leadSourceService.PodeGerir(Perfil))
            {
                NotificacaoService.Erro("Não tens permissão para gerir listas auxiliares.");
                Response.Redirect("~/Dashboard/Dashboard.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CarregarLeadSources();
                CarregarLossReasons();
                CarregarPaymentTerms();
                CarregarTaxRates();
            }
        }

        // ---------- Origens de Lead ----------
        private void CarregarLeadSources()
        {
            var lista = _leadSourceService.Listar(null);
            rptLeadSources.DataSource = lista;
            rptLeadSources.DataBind();
            phLeadSourcesVazio.Visible = lista.Count == 0;
        }

        protected void cvLeadSource_ServerValidate(object source, ServerValidateEventArgs args)
        {
            bool existe = _leadSourceService.ExisteNome(txtLeadSourceNome.Text.Trim(), LeadSourceIdEmEdicao);
            var erros = _leadSourceService.Validar(new LeadSource { Name = txtLeadSourceNome.Text.Trim() }, existe);
            args.IsValid = erros.Count == 0;
            cvLeadSource.ErrorMessage = string.Join(" ", erros);
        }

        protected void btnLeadSourceGuardar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            if (LeadSourceIdEmEdicao.HasValue)
            {
                _leadSourceService.Atualizar(new LeadSource { LeadSourceId = LeadSourceIdEmEdicao.Value, Name = txtLeadSourceNome.Text.Trim(), UpdatedBy = UserId });
                NotificacaoService.Sucesso("Origem atualizada.");
            }
            else
            {
                _leadSourceService.Criar(new LeadSource { Name = txtLeadSourceNome.Text.Trim(), CreatedBy = UserId });
                NotificacaoService.Sucesso("Origem criada.");
            }

            LimparFormLeadSource();
            CarregarLeadSources();
        }

        private void LimparFormLeadSource()
        {
            LeadSourceIdEmEdicao = null;
            txtLeadSourceNome.Text = "";
            btnLeadSourceGuardar.Text = "Adicionar";
            btnLeadSourceCancelar.Visible = false;
        }

        protected void btnLeadSourceCancelar_Click(object sender, EventArgs e) => LimparFormLeadSource();

        protected void rptLeadSources_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id = int.Parse(e.CommandArgument.ToString());

            if (e.CommandName == "Editar")
            {
                var item = _leadSourceService.GetById(id);
                if (item == null) return;
                LeadSourceIdEmEdicao = id;
                txtLeadSourceNome.Text = item.Name;
                btnLeadSourceGuardar.Text = "Guardar";
                btnLeadSourceCancelar.Visible = true;
            }
            else if (e.CommandName == "AlternarEstado")
            {
                _leadSourceService.AlternarEstado(id, UserId);
                NotificacaoService.Sucesso("Estado atualizado.");
                CarregarLeadSources();
            }
        }

        // ---------- Motivos de Perda ----------
        private void CarregarLossReasons()
        {
            var lista = _lossReasonService.Listar(null);
            rptLossReasons.DataSource = lista;
            rptLossReasons.DataBind();
            phLossReasonsVazio.Visible = lista.Count == 0;
        }

        protected void cvLossReason_ServerValidate(object source, ServerValidateEventArgs args)
        {
            bool existe = _lossReasonService.ExisteNome(txtLossReasonNome.Text.Trim(), LossReasonIdEmEdicao);
            var erros = _lossReasonService.Validar(new LossReason { Name = txtLossReasonNome.Text.Trim() }, existe);
            args.IsValid = erros.Count == 0;
            cvLossReason.ErrorMessage = string.Join(" ", erros);
        }

        protected void btnLossReasonGuardar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            if (LossReasonIdEmEdicao.HasValue)
            {
                _lossReasonService.Atualizar(new LossReason { LossReasonId = LossReasonIdEmEdicao.Value, Name = txtLossReasonNome.Text.Trim() }, UserId);
                NotificacaoService.Sucesso("Motivo atualizado.");
            }
            else
            {
                _lossReasonService.Criar(new LossReason { Name = txtLossReasonNome.Text.Trim() }, UserId);
                NotificacaoService.Sucesso("Motivo criado.");
            }

            LimparFormLossReason();
            CarregarLossReasons();
        }

        private void LimparFormLossReason()
        {
            LossReasonIdEmEdicao = null;
            txtLossReasonNome.Text = "";
            btnLossReasonGuardar.Text = "Adicionar";
            btnLossReasonCancelar.Visible = false;
        }

        protected void btnLossReasonCancelar_Click(object sender, EventArgs e) => LimparFormLossReason();

        protected void rptLossReasons_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id = int.Parse(e.CommandArgument.ToString());

            if (e.CommandName == "Editar")
            {
                var item = _lossReasonService.GetById(id);
                if (item == null) return;
                LossReasonIdEmEdicao = id;
                txtLossReasonNome.Text = item.Name;
                btnLossReasonGuardar.Text = "Guardar";
                btnLossReasonCancelar.Visible = true;
            }
            else if (e.CommandName == "AlternarEstado")
            {
                _lossReasonService.AlternarEstado(id, UserId);
                NotificacaoService.Sucesso("Estado atualizado.");
                CarregarLossReasons();
            }
        }

        // ---------- Condições de Pagamento ----------
        private void CarregarPaymentTerms()
        {
            var lista = _paymentTermService.Listar(null);
            rptPaymentTerms.DataSource = lista;
            rptPaymentTerms.DataBind();
            phPaymentTermsVazio.Visible = lista.Count == 0;
        }

        protected void cvPaymentTerm_ServerValidate(object source, ServerValidateEventArgs args)
        {
            bool existe = _paymentTermService.ExisteNome(txtPaymentTermNome.Text.Trim(), PaymentTermIdEmEdicao);
            int? dias = int.TryParse(txtPaymentTermDias.Text, out int d) ? d : (int?)null;
            var erros = _paymentTermService.Validar(new PaymentTerm { Name = txtPaymentTermNome.Text.Trim(), DaysDue = dias }, existe);
            args.IsValid = erros.Count == 0;
            cvPaymentTerm.ErrorMessage = string.Join(" ", erros);
        }

        protected void btnPaymentTermGuardar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            int? dias = int.TryParse(txtPaymentTermDias.Text, out int d) ? d : (int?)null;

            if (PaymentTermIdEmEdicao.HasValue)
            {
                _paymentTermService.Atualizar(new PaymentTerm { PaymentTermId = PaymentTermIdEmEdicao.Value, Name = txtPaymentTermNome.Text.Trim(), DaysDue = dias, UpdatedBy = UserId });
                NotificacaoService.Sucesso("Condição atualizada.");
            }
            else
            {
                _paymentTermService.Criar(new PaymentTerm { Name = txtPaymentTermNome.Text.Trim(), DaysDue = dias, CreatedBy = UserId });
                NotificacaoService.Sucesso("Condição criada.");
            }

            LimparFormPaymentTerm();
            CarregarPaymentTerms();
        }

        private void LimparFormPaymentTerm()
        {
            PaymentTermIdEmEdicao = null;
            txtPaymentTermNome.Text = "";
            txtPaymentTermDias.Text = "";
            btnPaymentTermGuardar.Text = "Adicionar";
            btnPaymentTermCancelar.Visible = false;
        }

        protected void btnPaymentTermCancelar_Click(object sender, EventArgs e) => LimparFormPaymentTerm();

        protected void rptPaymentTerms_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id = int.Parse(e.CommandArgument.ToString());

            if (e.CommandName == "Editar")
            {
                var item = _paymentTermService.GetById(id);
                if (item == null) return;
                PaymentTermIdEmEdicao = id;
                txtPaymentTermNome.Text = item.Name;
                txtPaymentTermDias.Text = item.DaysDue?.ToString() ?? "";
                btnPaymentTermGuardar.Text = "Guardar";
                btnPaymentTermCancelar.Visible = true;
            }
            else if (e.CommandName == "AlternarEstado")
            {
                _paymentTermService.AlternarEstado(id, UserId);
                NotificacaoService.Sucesso("Estado atualizado.");
                CarregarPaymentTerms();
            }
        }

        // ---------- Taxas de IVA ----------
        private void CarregarTaxRates()
        {
            var lista = _taxRateService.Listar(null);
            rptTaxRates.DataSource = lista;
            rptTaxRates.DataBind();
            phTaxRatesVazio.Visible = lista.Count == 0;
        }

        protected void cvTaxRate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            bool existe = _taxRateService.ExisteNome(txtTaxRateNome.Text.Trim(), TaxRateIdEmEdicao);
            decimal percentagem = decimal.TryParse(txtTaxRatePercentagem.Text, out decimal p) ? p : -1;
            var erros = _taxRateService.Validar(new TaxRate { Name = txtTaxRateNome.Text.Trim(), Percentage = percentagem }, existe);
            args.IsValid = erros.Count == 0;
            cvTaxRate.ErrorMessage = string.Join(" ", erros);
        }

        protected void btnTaxRateGuardar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            decimal percentagem = decimal.Parse(txtTaxRatePercentagem.Text);

            if (TaxRateIdEmEdicao.HasValue)
            {
                _taxRateService.Atualizar(new TaxRate { TaxRateId = TaxRateIdEmEdicao.Value, Name = txtTaxRateNome.Text.Trim(), Percentage = percentagem, UpdatedBy = UserId });
                NotificacaoService.Sucesso("Taxa atualizada.");
            }
            else
            {
                _taxRateService.Criar(new TaxRate { Name = txtTaxRateNome.Text.Trim(), Percentage = percentagem, CreatedBy = UserId });
                NotificacaoService.Sucesso("Taxa criada.");
            }

            LimparFormTaxRate();
            CarregarTaxRates();
        }

        private void LimparFormTaxRate()
        {
            TaxRateIdEmEdicao = null;
            txtTaxRateNome.Text = "";
            txtTaxRatePercentagem.Text = "";
            btnTaxRateGuardar.Text = "Adicionar";
            btnTaxRateCancelar.Visible = false;
        }

        protected void btnTaxRateCancelar_Click(object sender, EventArgs e) => LimparFormTaxRate();

        protected void rptTaxRates_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id = int.Parse(e.CommandArgument.ToString());

            if (e.CommandName == "Editar")
            {
                var item = _taxRateService.GetById(id);
                if (item == null) return;
                TaxRateIdEmEdicao = id;
                txtTaxRateNome.Text = item.Name;
                txtTaxRatePercentagem.Text = item.Percentage.ToString("0.##");
                btnTaxRateGuardar.Text = "Guardar";
                btnTaxRateCancelar.Visible = true;
            }
            else if (e.CommandName == "AlternarEstado")
            {
                _taxRateService.AlternarEstado(id, UserId);
                NotificacaoService.Sucesso("Estado atualizado.");
                CarregarTaxRates();
            }
        }
    }
}