using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using CRM.Data.Context;
using CRM.Data.Helpers;
using CRM.Models.DTOs;
using CRM.Models.Entities.Clientes;
using CRM.Models.Entities.Leads;
using CRM.Models.Entities.Oportunidades;

namespace CRM.Data.Repositories
{
    public class LeadConversionRepository
    {
        private readonly ClientRepository _clientRepository = new ClientRepository();

        public LeadConversionResult Converter(LeadConversionRequest request)
        {
            using (var context = new CrmDbContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var lead = context.Leads.Find(request.LeadId);
                    if (lead == null || lead.IsDeleted)
                        throw new AplicacaoException("Lead não encontrado.");

                    if (lead.Status == "Convertido")
                        throw new AplicacaoException("Este lead já foi convertido anteriormente.");

                    string statusAnterior = lead.Status;

                    // ---------- Cliente ----------
                    Client client;

                    if (request.ClienteExistenteId.HasValue)
                    {
                        client = context.Clients
                            .SingleOrDefault(c => c.ClientId == request.ClienteExistenteId.Value && !c.IsDeleted);

                        if (client == null)
                            throw new AplicacaoException("O cliente selecionado não foi encontrado.");
                    }
                    else
                    {
                        bool nifDuplicado = context.Clients.Any(c =>
                            !c.IsDeleted && c.Status == "Ativo" && c.VatNumber == request.NovoClienteNif);

                        if (nifDuplicado)
                            throw new AplicacaoException("Já existe um cliente ativo com este NIF.");

                        client = new Client
                        {
                            InternalCode = _clientRepository.GerarProximoCodigoInterno(),
                            TradeName = request.NovoClienteNomeComercial,
                            LegalName = string.IsNullOrWhiteSpace(request.NovoClienteNomeLegal) ? null : request.NovoClienteNomeLegal,
                            VatNumber = request.NovoClienteNif,
                            Email = string.IsNullOrWhiteSpace(request.NovoClienteEmail) ? null : request.NovoClienteEmail,
                            Phone = string.IsNullOrWhiteSpace(request.NovoClienteTelefone) ? null : request.NovoClienteTelefone,
                            CountryId = request.NovoClienteCountryId.Value,
                            SectorId = request.NovoClienteSectorId,
                            AccountManagerId = request.NovoClienteAccountManagerId,
                            CreatedDate = DateTime.UtcNow,
                            CreatedBy = request.UserId
                        };

                        context.Clients.Add(client);

                        try
                        {
                            context.SaveChanges();
                        }
                        catch (DbUpdateException ex)
                        {
                            throw new AplicacaoException(DbErrorTranslator.Traduzir(ex), ex);
                        }
                    }

                    // ---------- Contacto ----------
                    Contact contact = null;

                    if (request.CriarContacto)
                    {
                        contact = new Contact
                        {
                            ClientId = client.ClientId,
                            Name = request.ContactoNome,
                            JobTitle = string.IsNullOrWhiteSpace(request.ContactoCargo) ? null : request.ContactoCargo,
                            Email = string.IsNullOrWhiteSpace(request.ContactoEmail) ? null : request.ContactoEmail,
                            Phone = string.IsNullOrWhiteSpace(request.ContactoTelefone) ? null : request.ContactoTelefone,
                            IsPrimary = !context.Contacts.Any(c => c.ClientId == client.ClientId && !c.IsDeleted),
                            CreatedDate = DateTime.UtcNow,
                            CreatedBy = request.UserId
                        };

                        context.Contacts.Add(contact);
                        context.SaveChanges();
                    }

                    // ---------- Oportunidade ----------
                    Opportunity opportunity = null;

                    if (request.CriarOportunidade)
                    {
                        var fase = context.OpportunityStages.Find(request.OportunidadeStageId);
                        if (fase == null)
                            throw new AplicacaoException("A fase inicial selecionada não foi encontrada.");

                        opportunity = new Opportunity
                        {
                            Title = request.OportunidadeTitulo,
                            ClientId = client.ClientId,
                            ContactId = contact?.ContactId,
                            StageId = fase.StageId,
                            EstimatedValue = request.OportunidadeValorEstimado,
                            Probability = fase.DefaultProbability,
                            ExpectedCloseDate = request.OportunidadeDataFechoPrevista,
                            OwnerId = request.OportunidadeOwnerId,
                            CreatedDate = DateTime.UtcNow,
                            CreatedBy = request.UserId
                        };

                        context.Opportunities.Add(opportunity);
                        context.SaveChanges();
                    }

                    // ---------- Lead ----------
                    lead.Status = "Convertido";
                    lead.ConvertedDate = DateTime.UtcNow;
                    lead.ConvertedByUserId = request.UserId;
                    lead.ConvertedClientId = client.ClientId;
                    lead.ConvertedContactId = contact?.ContactId;
                    lead.ConvertedOpportunityId = opportunity?.OpportunityId;
                    lead.UpdatedDate = DateTime.UtcNow;
                    lead.UpdatedBy = request.UserId;

                    context.SaveChanges();

                    context.LeadStatusHistories.Add(new LeadStatusHistory
                    {
                        LeadId = lead.LeadId,
                        PreviousStatus = statusAnterior,
                        NewStatus = "Convertido",
                        ChangedDate = DateTime.UtcNow,
                        ChangedBy = request.UserId
                    });
                    context.SaveChanges();

                    transaction.Commit();

                    return new LeadConversionResult
                    {
                        ClientId = client.ClientId,
                        ContactId = contact?.ContactId,
                        OpportunityId = opportunity?.OpportunityId
                    };
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}