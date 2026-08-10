using System;
using System.Collections.Generic;
using CRM.Data.Repositories;
using CRM.Models.DTOs;

namespace CRM.Services
{
    public class LeadConversionService
    {
        private readonly ClientRepository _clientRepository = new ClientRepository();
        private readonly LeadConversionRepository _leadConversionRepository = new LeadConversionRepository();
        private readonly AuditService _auditService = new AuditService();

        public List<string> Validar(LeadConversionRequest request)
        {
            var erros = new List<string>();
            bool clienteNovo = !request.ClienteExistenteId.HasValue;

            if (clienteNovo)
            {
                if (string.IsNullOrWhiteSpace(request.NovoClienteNomeComercial) || request.NovoClienteNomeComercial.Trim().Length < 2)
                    erros.Add("O nome comercial do cliente tem de ter entre 2 e 150 caracteres.");

                if (string.IsNullOrWhiteSpace(request.NovoClienteNif))
                    erros.Add("O NIF do cliente é obrigatório.");
                else if (_clientRepository.NifAtivoExiste(request.NovoClienteNif))
                    erros.Add("Já existe um cliente ativo com este NIF. Usa a opção \"Cliente Existente\" para associar a esse cliente em vez de criar um novo.");

                if (!request.NovoClienteCountryId.HasValue)
                    erros.Add("O país do cliente é obrigatório.");

                if (request.NovoClienteAccountManagerId <= 0)
                    erros.Add("O comercial responsável pelo cliente é obrigatório.");
            }

            if (request.CriarContacto && string.IsNullOrWhiteSpace(request.ContactoNome))
                erros.Add("O nome do contacto é obrigatório.");

            if (request.CriarOportunidade)
            {
                if (string.IsNullOrWhiteSpace(request.OportunidadeTitulo))
                    erros.Add("O título da oportunidade é obrigatório.");

                if (request.OportunidadeStageId <= 0)
                    erros.Add("A fase inicial da oportunidade é obrigatória.");

                if (request.OportunidadeValorEstimado < 0)
                    erros.Add("O valor estimado da oportunidade não pode ser negativo.");

                if (request.OportunidadeDataFechoPrevista == default(DateTime))
                    erros.Add("A data prevista de fecho da oportunidade é obrigatória.");

                if (request.OportunidadeOwnerId <= 0)
                    erros.Add("O comercial responsável pela oportunidade é obrigatório.");
            }

            return erros;
        }

        public LeadConversionResult Converter(LeadConversionRequest request)
        {
            var resultado = _leadConversionRepository.Converter(request);

            var detalhe = $"Cliente {resultado.ClientId}"
                + (resultado.ContactId.HasValue ? $", Contacto {resultado.ContactId}" : "")
                + (resultado.OpportunityId.HasValue ? $", Oportunidade {resultado.OpportunityId}" : "");

            _auditService.Registar(request.UserId, "Convert", "Lead", request.LeadId.ToString(), detalhe);

            return resultado;
        }
    }
}