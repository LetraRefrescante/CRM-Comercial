using System;
using System.Collections.Generic;
using System.Linq;
using CRM.Data.Repositories;
using CRM.Models.Entities.Leads;

namespace CRM.Services
{
    public class LeadService
    {
        private readonly LeadRepository _leadRepository = new LeadRepository();

        public const string StatusNovo = "Novo";
        public const string StatusEmContacto = "Em Contacto";
        public const string StatusQualificado = "Qualificado";
        public const string StatusNaoQualificado = "Não Qualificado";
        public const string StatusConvertido = "Convertido";

        private static readonly string[] EstadosAtivos = { StatusNovo, StatusEmContacto, StatusQualificado };

        // ===================== Permissões (matriz do blueprint) =====================
        // Administrador/Diretor: TOTAL · Comercial: PRÓPRIOS · Financeiro/Consulta: CONSULTA

        public bool TemAmbitoProprios(string perfil) => perfil == "Comercial";

        public bool PodeCriarOuEditar(string perfil) =>
            perfil == "Administrador" || perfil == "Diretor" || perfil == "Comercial";

        public bool PodeEliminar(string perfil) =>
            perfil == "Administrador" || perfil == "Diretor";

        public bool PodeConverter(string perfil) =>
            perfil == "Administrador" || perfil == "Diretor" || perfil == "Comercial";

        // ===================== Validação =====================

        public List<string> Validar(Lead lead)
        {
            var erros = new List<string>();

            if (string.IsNullOrWhiteSpace(lead.Name))
                erros.Add("O nome é obrigatório.");

            if (string.IsNullOrWhiteSpace(lead.Email) && string.IsNullOrWhiteSpace(lead.Phone))
                erros.Add("Tens de preencher pelo menos o email ou o telefone.");

            if (lead.Score.HasValue && (lead.Score < 0 || lead.Score > 100))
                erros.Add("A pontuação tem de estar entre 0 e 100.");

            if (EstadosAtivos.Contains(lead.Status) && lead.NextContactDate.HasValue && lead.NextContactDate.Value <= DateTime.Now)
                erros.Add("A data do próximo contacto tem de ser futura enquanto o lead estiver ativo.");

            if (lead.Status == StatusNaoQualificado && !lead.LossReasonId.HasValue)
                erros.Add("O motivo de perda é obrigatório quando o lead não é qualificado.");

            return erros;
        }

        /// <summary>
        /// Blueprint: "Evitar duplicados por email, telefone ou NIF, apresentando avisos."
        /// É um AVISO — a gravação pode prosseguir mesmo com duplicados encontrados.
        /// </summary>
        public List<Lead> ProcurarPossiveisDuplicados(string email, string phone, int? ignorarLeadId = null)
            => _leadRepository.ProcurarPossiveisDuplicados(email, phone, ignorarLeadId);

        // ===================== Operações =====================

        public Lead GetById(int leadId) => _leadRepository.GetById(leadId);

        public List<Lead> Listar(
            string pesquisa, string status, int? leadSourceId, int? ownerId,
            int? scoreMin, int? scoreMax, DateTime? dataInicio, DateTime? dataFim,
            int pagina, int tamanhoPagina, out int totalRegistos,
            string sortColumn, bool sortAscending)
            => _leadRepository.Listar(pesquisa, status, leadSourceId, ownerId, scoreMin, scoreMax,
                dataInicio, dataFim, pagina, tamanhoPagina, out totalRegistos, sortColumn, sortAscending);

        public int Criar(Lead lead) => _leadRepository.Criar(lead);

        public void Atualizar(Lead lead, int alteradoPor) => _leadRepository.Atualizar(lead, alteradoPor);

        public bool Eliminar(int leadId, int userId, string perfil)
        {
            if (!PodeEliminar(perfil)) return false;

            _leadRepository.EliminarLogico(leadId, userId);
            return true;
        }

        /// <summary>
        /// Regra: "Após a conversão, o lead fica bloqueado para edição comercial."
        /// </summary>
        public bool EstaBloqueadoParaEdicao(Lead lead) => lead.Status == StatusConvertido;

        public List<LeadStatusHistory> ListarHistoricoEstados(int leadId) => _leadRepository.ListarHistoricoEstados(leadId);
    }
}