using System;
using System.Globalization;
using System.Web.UI.WebControls;
using CRM.Data.Repositories;
using CRM.Models.Entities.Oportunidades;
using CRM.Services;
using CRM.Web.Helpers;
using CRM.Web.Paginas;

namespace CRM.Web.Oportunidades
{
    public partial class OportunidadeEditar : PaginaBase
    {
        private readonly OpportunityService _opportunityService = new OpportunityService();
        private readonly OpportunityStageRepository _stageRepository = new OpportunityStageRepository();
        private readonly UserRepository _userRepository = new UserRepository();
        private readonly ClientRepository _clientRepository = new ClientRepository();
        private readonly ContactRepository _contactRepository = new ContactRepository();

        private int? OpportunityId
        {
            get
            {
                if (int.TryParse(Request.QueryString["id"], out int id)) return id;
                return null;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!_opportunityService.PodeEditar(Perfil))
            {
                NotificacaoService.Erro("Não tens permissão para aceder a esta página.");
                Response.Redirect("~/Dashboard/Dashboard.aspx");
                return;
            }

            if (!IsPostBack)
            {
                if (OpportunityId.HasValue)
                {
                    litTitulo.Text = "Editar Oportunidade";
                    litTituloBreadcrumb.Text = "Editar";
                    CarregarOportunidade(OpportunityId.Value);
                }
                else
                {
                    litTitulo.Text = "Nova Oportunidade";
                    litTituloBreadcrumb.Text = "Nova";
                    CarregarListasEstaticas(null, null);
                }
            }
        }

        private void CarregarListasEstaticas(int? stageIdAtual, int? ownerIdAtual)
        {
            var fases = _stageRepository.ListarAtivasParaAbertura();
            ddlFase.DataSource = fases;
            ddlFase.DataTextField = "Name";
            ddlFase.DataValueField = "StageId";
            ddlFase.DataBind();

            AdicionarValorAtualSeInativo(ddlFase, stageIdAtual, () =>
            {
                var fase = _stageRepository.ObterPorId(stageIdAtual.Value);
                return fase != null ? $"{fase.Name} (inativa)" : null;
            });

            ddlComercial.Items.Add(new ListItem("-- Selecionar --", ""));
            foreach (var user in _userRepository.ListarComerciaisAtivos())
            {
                ddlComercial.Items.Add(new ListItem(user.Name, user.UserId.ToString()));
            }

            AdicionarValorAtualSeInativo(ddlComercial, ownerIdAtual, () =>
            {
                var user = _userRepository.GetById(ownerIdAtual.Value);
                return user != null ? $"{user.Name} (inativo)" : null;
            });

            if (_opportunityService.TemAmbitoProprios(Perfil))
            {
                ddlComercial.SelectedValue = UserId.ToString();
            }
        }
        private void AdicionarValorAtualSeInativo(DropDownList ddl, int? valorAtualId, Func<string> obterTexto)
        {
            if (!valorAtualId.HasValue) return;
            if (ddl.Items.FindByValue(valorAtualId.Value.ToString()) != null) return;

            string texto = obterTexto();
            if (texto == null) return;

            ddl.Items.Add(new ListItem(texto, valorAtualId.Value.ToString()));
        }

        private void CarregarOportunidade(int id)
        {
            var opportunity = _opportunityService.ObterPorId(id, Perfil, UserId);
            if (opportunity == null)
            {
                NotificacaoService.Erro("Oportunidade não encontrada ou sem permissão para a editar.");
                Response.Redirect("~/Oportunidades/OportunidadesLista.aspx");
                return;
            }

            if (opportunity.IsClosed)
            {
                NotificacaoService.Erro("Esta oportunidade está fechada e não pode ser editada.");
                Response.Redirect("~/Oportunidades/OportunidadesLista.aspx");
                return;
            }

            CarregarListasEstaticas(opportunity.StageId, opportunity.OwnerId);

            ViewState["RowVersion"] = Convert.ToBase64String(opportunity.RowVersion ?? new byte[0]);
            ViewState["StageIdOriginal"] = opportunity.StageId;

            txtTitulo.Text = opportunity.Title;
            SelecionarCliente(opportunity.ClientId, opportunity.Client?.TradeName, opportunity.Client?.VatNumber, opportunity.ContactId);

            ddlFase.SelectedValue = opportunity.StageId.ToString();
            txtValorEstimado.Text = opportunity.EstimatedValue.ToString("0.00", CultureInfo.InvariantCulture);
            txtProbabilidade.Text = opportunity.Probability.ToString();
            txtDataFechoPrevista.Text = opportunity.ExpectedCloseDate.ToString("yyyy-MM-dd");
            txtConcorrente.Text = opportunity.Competitor;

            ddlComercial.SelectedValue = opportunity.OwnerId.ToString();
        }

        private void SelecionarCliente(int clientId, string nomeCliente, string nif, int? contactIdSelecionar = null)
        {
            hdnClientId.Value = clientId.ToString();

            phClienteBusca.Visible = false;
            phClienteSelecionado.Visible = true;
            litClienteNome.Text = Server.HtmlEncode(nomeCliente ?? "");
            litClienteNif.Text = "NIF " + Server.HtmlEncode(nif ?? "");

            CarregarContactos(clientId, contactIdSelecionar);
        }

        private void CarregarContactos(int clientId, int? contactIdSelecionar = null)
        {
            ddlContacto.Items.Clear();
            ddlContacto.Items.Add(new ListItem("(Sem contacto)", ""));

            foreach (var contacto in _contactRepository.ListarPorCliente(clientId))
            {
                ddlContacto.Items.Add(new ListItem(contacto.Name, contacto.ContactId.ToString()));
            }

            if (contactIdSelecionar.HasValue)
            {
                var item = ddlContacto.Items.FindByValue(contactIdSelecionar.Value.ToString());
                if (item != null) ddlContacto.SelectedValue = contactIdSelecionar.Value.ToString();
            }
        }

        protected void ddlFase_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlFase.SelectedValue)) return;

            var fase = _stageRepository.ObterPorId(int.Parse(ddlFase.SelectedValue));
            if (fase != null)
            {
                txtProbabilidade.Text = fase.DefaultProbability.ToString();
            }
        }

        protected void btnClienteSelecionado_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(hdnClientId.Value, out int clientId)) return;

            var cliente = _clientRepository.GetById(clientId);
            if (cliente == null) return;

            if (_opportunityService.TemAmbitoProprios(Perfil) && cliente.AccountManagerId != UserId)
            {
                NotificacaoService.Erro("Só podes criar oportunidades para clientes atribuídos a ti.");
                hdnClientId.Value = "";
                return;
            }

            SelecionarCliente(cliente.ClientId, cliente.TradeName, cliente.VatNumber);
        }

        protected void lnkAlterarCliente_Click(object sender, EventArgs e)
        {
            hdnClientId.Value = "";
            phClienteBusca.Visible = true;
            phClienteSelecionado.Visible = false;
            txtClientePesquisa.Text = "";

            ddlContacto.Items.Clear();
            ddlContacto.Items.Add(new ListItem("(Sem contacto)", ""));
        }

        protected void cvTitulo_ServerValidate(object source, ServerValidateEventArgs args)
        {
            string valor = args.Value?.Trim() ?? "";
            args.IsValid = valor.Length >= 2 && valor.Length <= 200;
        }

        protected void cvCliente_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = int.TryParse(args.Value, out int id) && id > 0;
        }

        protected void cvValorEstimado_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = decimal.TryParse(args.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal valor) && valor >= 0;
        }

        protected void cvProbabilidade_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = int.TryParse(args.Value, out int valor) && valor >= 0 && valor <= 100;
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            int clientId = int.Parse(hdnClientId.Value);

            var opportunity = new Opportunity
            {
                Title = txtTitulo.Text.Trim(),
                ClientId = clientId,
                ContactId = string.IsNullOrEmpty(ddlContacto.SelectedValue) ? (int?)null : int.Parse(ddlContacto.SelectedValue),
                StageId = int.Parse(ddlFase.SelectedValue),
                EstimatedValue = decimal.Parse(txtValorEstimado.Text.Trim(), CultureInfo.InvariantCulture),
                Probability = int.Parse(txtProbabilidade.Text.Trim()),
                ExpectedCloseDate = DateTime.Parse(txtDataFechoPrevista.Text.Trim()),
                OwnerId = int.Parse(ddlComercial.SelectedValue),
                Competitor = string.IsNullOrWhiteSpace(txtConcorrente.Text) ? null : txtConcorrente.Text.Trim()
            };

            string erro;

            try
            {
                if (OpportunityId.HasValue)
                {
                    opportunity.OpportunityId = OpportunityId.Value;
                    opportunity.RowVersion = Convert.FromBase64String(ViewState["RowVersion"] as string ?? "");
                    int faseAnterior = (int)ViewState["StageIdOriginal"];
                    erro = _opportunityService.Atualizar(opportunity, faseAnterior, Perfil, UserId);
                }
                else
                {
                    erro = _opportunityService.Criar(opportunity, Perfil, UserId);
                }
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException)
            {
                NotificacaoService.Erro("Esta oportunidade foi alterada por outro utilizador. Recarrega a página e tenta novamente.");
                return;
            }

            if (erro != null)
            {
                NotificacaoService.Erro(erro);
                return;
            }

            NotificacaoService.Sucesso("Oportunidade guardada com sucesso.");
            Response.Redirect("~/Oportunidades/OportunidadesLista.aspx");
        }
    }
}