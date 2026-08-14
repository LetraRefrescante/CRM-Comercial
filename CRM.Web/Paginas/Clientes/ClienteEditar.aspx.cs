using CRM.Services;
using CRM.Data.Context;
using CRM.Data.Repositories;
using CRM.Models.Entities.Clientes;
using CRM.Web.Helpers;
using CRM.Data.Helpers;
using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.UI.WebControls;

namespace CRM.Web.Paginas.Clientes
{
    public partial class ClienteEditar : PaginaBase
    {
        private readonly ClientRepository _clientRepository = new ClientRepository();
        private readonly ClientService _clientService = new ClientService();
        private readonly UserRepository _userRepository = new UserRepository();

        private int? ClientId
        {
            get
            {
                if (int.TryParse(Request.QueryString["id"], out int id)) return id;
                return null;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!_clientService.PodeCriarOuEditar(Perfil))
            {
                NotificacaoService.Erro("Não tens permissão para aceder a esta página.");
                Response.Redirect("~/Dashboard/Dashboard.aspx");
                return;
            }

            if (!IsPostBack)
            {
                if (ClientId.HasValue)
                {
                    litTitulo.Text = "Editar Cliente";
                    litTituloBreadcrumb.Text = "Editar";
                    CarregarCliente(ClientId.Value);
                }
                else
                {
                    CarregarListas();
                    litTitulo.Text = "Novo Cliente";
                    litTituloBreadcrumb.Text = "Novo";
                }
            }
        }

        private void CarregarListas(int? countryIdAtual = null, int? sectorIdAtual = null, int? accountManagerIdAtual = null)
        {
            using (var context = new CrmDbContext())
            {
                ddlPais.DataSource = context.Countries.Where(c => c.IsActive).OrderBy(c => c.Name).ToList();
                ddlPais.DataTextField = "Name";
                ddlPais.DataValueField = "CountryId";
                ddlPais.DataBind();

                if (countryIdAtual.HasValue && ddlPais.Items.FindByValue(countryIdAtual.Value.ToString()) == null)
                {
                    var pais = context.Countries.Find(countryIdAtual.Value);
                    if (pais != null) ddlPais.Items.Add(new ListItem($"{pais.Name} (inativo)", pais.CountryId.ToString()));
                }

                ddlSetor.Items.Add(new ListItem("(Sem setor)", ""));
                foreach (var setor in context.Sectors.Where(s => s.IsActive).OrderBy(s => s.Name).ToList())
                {
                    ddlSetor.Items.Add(new ListItem(setor.Name, setor.SectorId.ToString()));
                }

                if (sectorIdAtual.HasValue && ddlSetor.Items.FindByValue(sectorIdAtual.Value.ToString()) == null)
                {
                    var setor = context.Sectors.Find(sectorIdAtual.Value);
                    if (setor != null) ddlSetor.Items.Add(new ListItem($"{setor.Name} (inativo)", setor.SectorId.ToString()));
                }
            }

            ddlComercial.Items.Add(new ListItem("-- Selecionar --", ""));
            foreach (var user in _userRepository.ListarComerciaisAtivos())
            {
                ddlComercial.Items.Add(new ListItem(user.Name, user.UserId.ToString()));
            }

            if (accountManagerIdAtual.HasValue && ddlComercial.Items.FindByValue(accountManagerIdAtual.Value.ToString()) == null)
            {
                var user = _userRepository.GetById(accountManagerIdAtual.Value);
                if (user != null) ddlComercial.Items.Add(new ListItem($"{user.Name} (inativo)", user.UserId.ToString()));
            }
        }

        private void CarregarCliente(int id)
        {
            var client = _clientRepository.GetById(id);
            if (client == null)
            {
                NotificacaoService.Erro("Cliente não encontrado.");
                Response.Redirect("~/Clientes/ClienteLista.aspx");
                return;
            }

            if (_clientService.TemAmbitoProprios(Perfil) && client.AccountManagerId != UserId)
            {
                NotificacaoService.Erro("Não tens permissão para editar este cliente.");
                Response.Redirect("~/Clientes/ClienteLista.aspx");
                return;
            }

            CarregarListas(client.CountryId, client.SectorId, client.AccountManagerId);

            ViewState["RowVersion"] = Convert.ToBase64String(client.RowVersion);

            txtNomeComercial.Text = client.TradeName;
            txtNomeLegal.Text = client.LegalName;
            txtNif.Text = client.VatNumber;
            txtEmail.Text = client.Email;
            txtTelefone.Text = client.Phone;
            txtMorada.Text = client.Address;
            txtCodigoPostal.Text = client.PostalCode;
            txtCidade.Text = client.City;
            ddlPais.SelectedValue = client.CountryId.ToString();
            ddlSetor.SelectedValue = client.SectorId?.ToString() ?? "";
            ddlComercial.SelectedValue = client.AccountManagerId.ToString();
            ddlEstado.SelectedValue = client.Status;
            txtObservacoes.Text = client.Notes;
        }

        protected void ddlPais_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Só para reavaliar os validators visualmente no próximo submit; não precisa de lógica.
        }

        /// <summary>
        /// Wrapper fino sobre ClientService.PaisEhPortugal — a regra de negócio (o que
        /// conta como "Portugal") vive na camada de serviço, não aqui.
        /// </summary>
        private bool PaisEhPortugal()
        {
            if (string.IsNullOrEmpty(ddlPais.SelectedValue)) return false;
            return _clientService.PaisEhPortugal(int.Parse(ddlPais.SelectedValue));
        }

        protected void cvNomeComercial_ServerValidate(object source, ServerValidateEventArgs args)
        {
            string valor = args.Value?.Trim() ?? "";
            args.IsValid = valor.Length >= 2 && valor.Length <= 150;
        }

        protected void cvNomeLegal_ServerValidate(object source, ServerValidateEventArgs args)
        {
            // Opcional (blueprint: "Não" obrigatório), mas quando preenchido não pode
            // exceder 200 caracteres — o MaxLength do TextBox só protege no browser.
            args.IsValid = string.IsNullOrEmpty(args.Value) || args.Value.Trim().Length <= 200;
        }

        protected void cvNif_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = _clientService.NifValido(args.Value, PaisEhPortugal());
        }

        protected void cvTelefone_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = _clientService.TelefoneValido(args.Value, PaisEhPortugal());
        }

        protected void cvCodigoPostal_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = _clientService.CodigoPostalValido(args.Value, PaisEhPortugal());
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            var client = new Client
            {
                TradeName = txtNomeComercial.Text.Trim(),
                LegalName = string.IsNullOrWhiteSpace(txtNomeLegal.Text) ? null : txtNomeLegal.Text.Trim(),
                VatNumber = txtNif.Text.Trim(),
                Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                Phone = string.IsNullOrWhiteSpace(txtTelefone.Text) ? null : txtTelefone.Text.Trim(),
                Address = string.IsNullOrWhiteSpace(txtMorada.Text) ? null : txtMorada.Text.Trim(),
                PostalCode = string.IsNullOrWhiteSpace(txtCodigoPostal.Text) ? null : txtCodigoPostal.Text.Trim(),
                City = string.IsNullOrWhiteSpace(txtCidade.Text) ? null : txtCidade.Text.Trim(),
                CountryId = int.Parse(ddlPais.SelectedValue),
                SectorId = string.IsNullOrEmpty(ddlSetor.SelectedValue) ? (int?)null : int.Parse(ddlSetor.SelectedValue),
                AccountManagerId = int.Parse(ddlComercial.SelectedValue),
                Status = ddlEstado.SelectedValue,
                Notes = string.IsNullOrWhiteSpace(txtObservacoes.Text) ? null : txtObservacoes.Text.Trim(),
                UpdatedBy = UserId
            };

            ResultadoGuardarCliente resultado;

            try
            {
                if (ClientId.HasValue)
                {
                    client.ClientId = ClientId.Value;
                    client.RowVersion = Convert.FromBase64String(ViewState["RowVersion"] as string ?? "");
                    resultado = _clientService.Atualizar(client, Perfil, UserId);
                }
                else
                {
                    client.CreatedBy = UserId;
                    resultado = _clientService.Criar(client, Perfil, UserId);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                NotificacaoService.Erro("Este cliente foi alterado por outro utilizador. Recarrega a página e tenta novamente.");
                return;
            }
            catch (AplicacaoException ex)
            {
                NotificacaoService.Erro(ex.Message);
                return;
            }

            switch (resultado)
            {
                case ResultadoGuardarCliente.Sucesso:
                    NotificacaoService.Sucesso("Cliente guardado com sucesso.");
                    Response.Redirect("~/Clientes/ClienteLista.aspx");
                    break;

                case ResultadoGuardarCliente.NifDuplicado:
                    NotificacaoService.Erro("Já existe um cliente ativo com este NIF.");
                    break;

                case ResultadoGuardarCliente.SemPermissao:
                    NotificacaoService.Erro("Não tens permissão para executar esta ação.");
                    break;
            }
        }
    }
}