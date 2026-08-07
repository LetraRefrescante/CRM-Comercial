using CRM.Business.Services;
using CRM.Data.Repositories;
using CRM.Models.Entities.Clientes;
using CRM.Web.Helpers;
using System;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;

namespace CRM.Web.Paginas.Clientes
{
    public partial class ContactoEditar : PaginaBase
    {
        private readonly ContactRepository _contactRepository = new ContactRepository();
        private readonly ContactService _contactService = new ContactService();
        private readonly ClientRepository _clientRepository = new ClientRepository();

        private int? ContactId
        {
            get
            {
                if (int.TryParse(Request.QueryString["id"], out int id)) return id;
                return null;
            }
        }

        /// <summary>
        /// Pré-preenchimento opcional vindo de um link "Novo Contacto" com cliente já conhecido
        /// (ex.: a partir de ClienteDetalhe.aspx). Se não vier, o utilizador escolhe no próprio
        /// campo Cliente da página, conforme exigido pela blueprint.
        /// </summary>
        private int ClienteIdQueryString
        {
            get
            {
                int.TryParse(Request.QueryString["clienteId"], out int id);
                return id;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!_contactService.PodeCriarOuEditar(Perfil))
            {
                NotificacaoService.Erro("Não tens permissão para aceder a esta página.");
                Response.Redirect("~/Dashboard/Dashboard.aspx");
                return;
            }

            if (!IsPostBack)
            {
                if (ContactId.HasValue)
                {
                    CarregarContacto(ContactId.Value);
                }
                else
                {
                    litTitulo.Text = "Novo Contacto";
                    litTituloBreadcrumb.Text = "Novo";
                    lnkCancelar.NavigateUrl = "~/Clientes/ClienteLista.aspx";

                    int clienteId = ClienteIdQueryString;

                    if (clienteId != 0)
                    {
                        if (!_contactService.TemAcessoAoCliente(clienteId, Perfil, UserId))
                        {
                            NotificacaoService.Erro("Cliente inválido ou sem permissão.");
                            Response.Redirect("~/Clientes/ClienteLista.aspx");
                            return;
                        }

                        ucSeletorCliente.ClienteId = clienteId;
                        AtualizarContextoCliente(clienteId);
                    }
                }
            }
        }

        private void CarregarContacto(int id)
        {
            var contact = _contactRepository.GetById(id);
            if (contact == null)
            {
                NotificacaoService.Erro("Contacto não encontrado.");
                Response.Redirect("~/Clientes/ClienteLista.aspx");
                return;
            }

            if (!_contactService.TemAcessoAoCliente(contact.ClientId, Perfil, UserId))
            {
                NotificacaoService.Erro("Não tens permissão para editar este contacto.");
                Response.Redirect("~/Clientes/ClienteLista.aspx");
                return;
            }

            ViewState["RowVersion"] = Convert.ToBase64String(contact.RowVersion);

            litTitulo.Text = "Editar Contacto";
            litTituloBreadcrumb.Text = "Editar";

            ucSeletorCliente.ClienteId = contact.ClientId;
            ucSeletorCliente.Enabled = false;
            AtualizarContextoCliente(contact.ClientId);

            txtNome.Text = contact.Name;
            txtCargo.Text = contact.JobTitle;
            txtDepartamento.Text = contact.Department;
            txtEmail.Text = contact.Email;
            txtTelefone.Text = contact.Phone;
            txtTelemovel.Text = contact.MobilePhone;
            txtDataNascimento.Text = contact.BirthDate?.ToString("yyyy-MM-dd");
            ddlPreferencia.SelectedValue = contact.ContactPreference ?? "";
            chkPrincipal.Checked = contact.IsPrimary;
            chkConsentimento.Checked = contact.ConsentGiven;
            txtRestricoes.Text = contact.ContactRestrictions;
        }

        /// <summary>
        /// Atualiza o breadcrumb, o link de cancelar e o país em cache (ViewState) sempre que
        /// o cliente do contacto é conhecido no carregamento ou trocado via SeletorCliente.
        /// </summary>
        private void AtualizarContextoCliente(int clienteId)
        {
            var client = _clientRepository.GetById(clienteId);

            ViewState["ClientePaisIso"] = client?.Country?.IsoCode ?? "";

            lnkClientePai.InnerText = client?.TradeName ?? "Cliente";
            lnkClientePai.Attributes["href"] = $"~/Clientes/ClienteDetalhe.aspx?id={clienteId}";
            lnkCancelar.NavigateUrl = $"~/Clientes/ClienteDetalhe.aspx?id={clienteId}";
        }

        protected void ucSeletorCliente_ClienteSelecionado(object sender, EventArgs e)
        {
            if (ucSeletorCliente.ClienteId.HasValue)
            {
                AtualizarContextoCliente(ucSeletorCliente.ClienteId.Value);
            }
        }

        /// <summary>
        /// País do cliente associado, lido do ViewState (atualizado sempre que o cliente é
        /// carregado ou trocado) em vez de ir à base de dados a cada postback de validação.
        /// </summary>
        private bool PaisDoClienteEhPortugal()
        {
            return ViewState["ClientePaisIso"] as string == "PT";
        }

        protected void cvDataNascimento_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(args.Value))
            {
                args.IsValid = true; // Campo opcional.
                return;
            }

            args.IsValid = DateTime.TryParse(args.Value, out DateTime data) && data.Date <= DateTime.Today;
        }

        protected void cvTelefone_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = TelefonePtValido(args.Value, exigirMovel: false);
        }

        protected void cvTelemovel_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = TelefonePtValido(args.Value, exigirMovel: true);
        }

        private bool TelefonePtValido(string valor, bool exigirMovel)
        {
            if (string.IsNullOrWhiteSpace(valor)) return true;

            if (!PaisDoClienteEhPortugal())
            {
                return Regex.IsMatch(valor.Trim(), @"^\+?[\d\s\-]{7,20}$");
            }

            valor = valor.Trim();

            return exigirMovel
                ? Regex.IsMatch(valor, @"^(\+351\s?)?9\d{8}$")
                : Regex.IsMatch(valor, @"^(\+351\s?)?[29]\d{8}$");
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            int? clienteId = ucSeletorCliente.ClienteId;

            if (!clienteId.HasValue || !_contactService.TemAcessoAoCliente(clienteId.Value, Perfil, UserId))
            {
                NotificacaoService.Erro("Cliente inválido ou sem permissão.");
                return;
            }

            var clienteSelecionado = _clientRepository.GetById(clienteId.Value);
            if (clienteSelecionado == null || clienteSelecionado.Status != "Ativo")
            {
                NotificacaoService.Erro("O contacto só pode ser associado a um cliente ativo.");
                return;
            }

            var contact = new Contact
            {
                ClientId = clienteId.Value,
                Name = txtNome.Text.Trim(),
                JobTitle = string.IsNullOrWhiteSpace(txtCargo.Text) ? null : txtCargo.Text.Trim(),
                Department = string.IsNullOrWhiteSpace(txtDepartamento.Text) ? null : txtDepartamento.Text.Trim(),
                Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                Phone = string.IsNullOrWhiteSpace(txtTelefone.Text) ? null : txtTelefone.Text.Trim(),
                MobilePhone = string.IsNullOrWhiteSpace(txtTelemovel.Text) ? null : txtTelemovel.Text.Trim(),
                BirthDate = DateTime.TryParse(txtDataNascimento.Text, out DateTime dataNasc) ? dataNasc : (DateTime?)null,
                IsPrimary = chkPrincipal.Checked,
                ContactPreference = string.IsNullOrEmpty(ddlPreferencia.SelectedValue) ? null : ddlPreferencia.SelectedValue,
                ConsentGiven = chkConsentimento.Checked,
                ContactRestrictions = string.IsNullOrWhiteSpace(txtRestricoes.Text) ? null : txtRestricoes.Text.Trim(),
                UpdatedBy = UserId
            };

            ResultadoGuardarContacto resultado;

            if (ContactId.HasValue)
            {
                contact.ContactId = ContactId.Value;
                contact.RowVersion = Convert.FromBase64String(ViewState["RowVersion"] as string ?? "");

                try
                {
                    resultado = _contactService.Atualizar(contact, Perfil, UserId);
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException)
                {
                    NotificacaoService.Erro("Este contacto foi alterado por outro utilizador. Recarrega a página e tenta novamente.");
                    return;
                }
            }
            else
            {
                contact.CreatedBy = UserId;
                resultado = _contactService.Criar(contact, Perfil, UserId);
            }

            switch (resultado)
            {
                case ResultadoGuardarContacto.Sucesso:
                    NotificacaoService.Sucesso("Contacto guardado com sucesso.");
                    Response.Redirect($"~/Clientes/ClienteDetalhe.aspx?id={clienteId.Value}");
                    break;

                case ResultadoGuardarContacto.SemPermissao:
                    NotificacaoService.Erro("Não tens permissão para executar esta ação.");
                    break;
            }
        }
    }
}