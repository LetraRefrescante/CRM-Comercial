using CRM.Business.Services;
using CRM.Data.Context;
using CRM.Data.Repositories;
using CRM.Models.Entities.Clientes;
using CRM.Web.Helpers;
using System;
using System.Linq;

namespace CRM.Web.Paginas.Clientes
{
    public partial class ClienteEditar : System.Web.UI.Page
    {
        private readonly ClientRepository _clientRepository = new ClientRepository();
        private readonly ClientService _clientService = new ClientService();
        private readonly UserRepository _userRepository = new UserRepository();

        private string Perfil => Session["RoleName"] as string ?? string.Empty;
        private int UserId => (int)Session["UserId"];

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
                CarregarListas();

                if (ClientId.HasValue)
                {
                    litTitulo.Text = "Editar Cliente";
                    litTituloBreadcrumb.Text = "Editar";
                    CarregarCliente(ClientId.Value);
                }
                else
                {
                    litTitulo.Text = "Novo Cliente";
                    litTituloBreadcrumb.Text = "Novo";
                }
            }
        }

        private void CarregarListas()
        {
            using (var context = new CrmDbContext())
            {
                ddlPais.DataSource = context.Countries.Where(c => c.IsActive).OrderBy(c => c.Name).ToList();
                ddlPais.DataTextField = "Name";
                ddlPais.DataValueField = "CountryId";
                ddlPais.DataBind();

                ddlSetor.Items.Add(new System.Web.UI.WebControls.ListItem("(Sem setor)", ""));
                var setores = context.Sectors.Where(s => s.IsActive).OrderBy(s => s.Name).ToList();
                foreach (var setor in setores)
                {
                    ddlSetor.Items.Add(new System.Web.UI.WebControls.ListItem(setor.Name, setor.SectorId.ToString()));
                }
            }

            ddlComercial.Items.Add(new System.Web.UI.WebControls.ListItem("-- Selecionar --", ""));
            var comerciais = _userRepository.Listar(status: "Ativo");
            foreach (var user in comerciais)
            {
                ddlComercial.Items.Add(new System.Web.UI.WebControls.ListItem(user.Name, user.UserId.ToString()));
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

            if (ClientId.HasValue)
            {
                client.ClientId = ClientId.Value;
                client.RowVersion = Convert.FromBase64String(ViewState["RowVersion"] as string ?? "");
                resultado = _clientService.Atualizar(client, Perfil);
            }
            else
            {
                client.CreatedBy = UserId;
                resultado = _clientService.Criar(client, Perfil);
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