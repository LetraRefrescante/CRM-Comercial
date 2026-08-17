using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using CRM.Data.Repositories;
using CRM.Models.DTOs;
using CRM.Services;
using CRM.Web.Helpers;

namespace CRM.Web.Paginas.Notificacoes
{
    public partial class EmailCompor : PaginaBase
    {
        private readonly EmailHistoryService _emailHistoryService = new EmailHistoryService();
        private readonly EmailTemplateService _emailTemplateService = new EmailTemplateService();
        private readonly ClientRepository _clientRepository = new ClientRepository();
        private readonly ContactRepository _contactRepository = new ContactRepository();
        private readonly LeadRepository _leadRepository = new LeadRepository();
        private readonly OpportunityRepository _opportunityRepository = new OpportunityRepository();
        private readonly ProposalRepository _proposalRepository = new ProposalRepository();

        private string Tipo => Request.QueryString["tipo"];
        private int? EntidadeId => int.TryParse(Request.QueryString["id"], out int id) ? id : (int?)null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!_emailHistoryService.PodeComporEmail(Perfil))
            {
                NotificacaoService.Erro("Não tens permissão para enviar emails.");
                Response.Redirect("~/Dashboard/Dashboard.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CarregarTemplates();
                PreencherContexto();
            }
        }

        private void CarregarTemplates()
        {
            foreach (var template in _emailTemplateService.ListarAtivos())
                ddlTemplate.Items.Add(new ListItem(template.Name, template.EmailTemplateId.ToString()));
        }

        private Dictionary<string, string> ObterVariaveis()
        {
            var variaveis = new Dictionary<string, string>();
            if (!EntidadeId.HasValue) return variaveis;

            switch (Tipo)
            {
                case "Client":
                    var client = _clientRepository.GetById(EntidadeId.Value);
                    if (client != null)
                    {
                        variaveis["ClientName"] = client.TradeName;
                        txtDestinatario.Text = client.Email;
                    }
                    break;
                case "Contact":
                    var contact = _contactRepository.GetById(EntidadeId.Value);
                    if (contact != null)
                    {
                        variaveis["ContactName"] = contact.Name;
                        var clienteDoContacto = _clientRepository.GetById(contact.ClientId);
                        if (clienteDoContacto != null) variaveis["ClientName"] = clienteDoContacto.TradeName;
                        txtDestinatario.Text = contact.Email;
                    }
                    break;
                case "Lead":
                    var lead = _leadRepository.GetById(EntidadeId.Value);
                    if (lead != null)
                    {
                        variaveis["LeadName"] = lead.Name;
                        txtDestinatario.Text = lead.Email;
                    }
                    break;
                case "Opportunity":
                    var opportunity = _opportunityRepository.GetById(EntidadeId.Value);
                    if (opportunity != null)
                    {
                        variaveis["OpportunityTitle"] = opportunity.Title;
                        if (opportunity.Client != null) variaveis["ClientName"] = opportunity.Client.TradeName;
                    }
                    break;
                case "Proposal":
                    var proposal = _proposalRepository.GetById(EntidadeId.Value);
                    if (proposal != null)
                    {
                        variaveis["ProposalNumber"] = proposal.ProposalNumber;
                        variaveis["ProposalTotal"] = proposal.Total.ToString("C");
                        if (proposal.Client != null)
                        {
                            variaveis["ClientName"] = proposal.Client.TradeName;
                            txtDestinatario.Text = proposal.Client.Email;
                        }
                    }
                    break;
            }

            return variaveis;
        }

        private void PreencherContexto()
        {
            var variaveis = ObterVariaveis();
            if (variaveis.Count == 0) return;

            phContexto.Visible = true;
            litContexto.Text = string.Join(", ", variaveis.Values);
        }

        protected void ddlTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlTemplate.SelectedValue))
            {
                txtAssunto.Text = "";
                txtCorpo.Text = "";
                return;
            }

            var template = _emailTemplateService.GetById(int.Parse(ddlTemplate.SelectedValue));
            if (template == null) return;

            var variaveis = ObterVariaveis();
            txtAssunto.Text = _emailTemplateService.SubstituirVariaveis(template.Subject, variaveis);
            txtCorpo.Text = _emailTemplateService.SubstituirVariaveis(template.Body, variaveis);
        }

        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            var request = new EmailComporRequest
            {
                ToAddress = txtDestinatario.Text.Trim(),
                Subject = txtAssunto.Text.Trim(),
                Body = txtCorpo.Text,
                EmailTemplateId = string.IsNullOrEmpty(ddlTemplate.SelectedValue) ? (int?)null : int.Parse(ddlTemplate.SelectedValue),
                RelatedEntityType = Tipo,
                RelatedEntityId = EntidadeId
            };

            var erros = _emailHistoryService.Validar(request);
            if (erros.Count > 0)
            {
                NotificacaoService.Erro(string.Join(" ", erros));
                return;
            }

            bool enviadoDeFacto = _emailHistoryService.Enviar(request, UserId, Perfil);

            NotificacaoService.Sucesso(enviadoDeFacto
                ? "Email enviado."
                : "Email registado, mas o envio SMTP real ainda não está ligado.");

            Response.Redirect("~/Notificacoes/EmailHistorico.aspx");
        }
    }
}