using System;
using System.Collections.Generic;
using System.IO;
using CRM.Data.Repositories;
using CRM.Models.Entities.Documentos;
using CRM.Models.Filtros;

namespace CRM.Services
{
    public class DocumentService
    {
        private readonly DocumentRepository _documentRepository = new DocumentRepository();
        private readonly ClientRepository _clientRepository = new ClientRepository();
        private readonly LeadRepository _leadRepository = new LeadRepository();
        private readonly OpportunityRepository _opportunityRepository = new OpportunityRepository();
        private readonly ProposalRepository _proposalRepository = new ProposalRepository();
        private readonly SaleRepository _saleRepository = new SaleRepository();
        private readonly PermissionService _permissionService = new PermissionService();

        private static readonly string[] ExtensoesPermitidas =
            { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".png", ".jpg", ".jpeg" };
        private const long TamanhoMaximoBytes = 10 * 1024 * 1024; // 10 MB

        public bool ExtensaoPermitida(string fileName)
        {
            var extensao = Path.GetExtension(fileName)?.ToLowerInvariant();
            return !string.IsNullOrEmpty(extensao) && Array.IndexOf(ExtensoesPermitidas, extensao) >= 0;
        }
        private static readonly Dictionary<string, string[]> MimeEsperados = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
            {
                { ".pdf", new[] { "application/pdf" } },
                { ".doc", new[] { "application/msword" } },
                { ".docx", new[] { "application/vnd.openxmlformats-officedocument.wordprocessingml.document" } },
                { ".xls", new[] { "application/vnd.ms-excel" } },
                { ".xlsx", new[] { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" } },
                { ".png", new[] { "image/png" } },
                { ".jpg", new[] { "image/jpeg" } },
                { ".jpeg", new[] { "image/jpeg" } },
            };

        public bool MimeCorrespondeExtensao(string fileName, string mimeType)
        {
            var extensao = Path.GetExtension(fileName)?.ToLowerInvariant();
            if (string.IsNullOrEmpty(extensao) || !MimeEsperados.TryGetValue(extensao, out var mimesValidos))
                return false;
            return Array.IndexOf(mimesValidos, mimeType?.ToLowerInvariant()) >= 0;
        }

        public bool TamanhoPermitido(long fileSizeBytes) => fileSizeBytes <= TamanhoMaximoBytes;
        public bool PodeAcederListaGlobal(string perfil) => perfil == "Administrador" || perfil == "Diretor";
        public bool PodeCarregar(string perfil) => perfil != "Consulta";

        public List<Document> Pesquisar(
            DocumentFiltro filtro,
            int pagina, int tamanhoPagina, out int totalRegistos,
            string sortColumn, bool sortAscending)
            => _documentRepository.Pesquisar(filtro, pagina, tamanhoPagina, out totalRegistos, sortColumn, sortAscending);

        public List<Document> Listar(string entityType, int entityId)
        {
            switch (entityType)
            {
                case "Client":
                    return _documentRepository.ListarPorCliente(entityId);
                case "Lead":
                    return _documentRepository.ListarPorLead(entityId);
                case "Opportunity":
                    return _documentRepository.ListarPorOportunidade(entityId);
                case "Sale":
                    return _documentRepository.ListarPorVenda(entityId);
                case "Proposal":
                    return _documentRepository.ListarPorProposta(entityId);
                default:
                    throw new NotSupportedException($"Tipo de entidade '{entityType}' ainda não suportado em Documentos.");
            }
        }

        public int Guardar(string entityType, int entityId, string category, string storedFileName,
            string originalFileName, string mimeType, long fileSizeBytes, bool isConfidential, int userId, string ip)
        {
            var document = new Document
            {
                Title = originalFileName,
                Category = category,
                StoredFileName = storedFileName,
                OriginalFileName = originalFileName,
                MimeType = mimeType,
                FileSizeBytes = fileSizeBytes,
                IsConfidential = isConfidential,
                CreatedBy = userId
            };

            switch (entityType)
            {
                case "Client":
                    document.RelatedClientId = entityId;
                    break;
                case "Lead":
                    document.RelatedLeadId = entityId;
                    break;
                case "Opportunity":
                    document.RelatedOpportunityId = entityId;
                    break;
                case "Sale":
                    document.RelatedSaleId = entityId;
                    break;
                case "Proposal":
                    document.RelatedProposalId = entityId;
                    break;
                default:
                    throw new NotSupportedException($"Tipo de entidade '{entityType}' ainda não suportado em Documentos.");
            }

            int documentId = _documentRepository.Criar(document);
            _documentRepository.RegistarAcesso(new DocumentAccessLog
            {
                DocumentId = documentId,
                Action = "Upload",
                UserId = userId,
                IpAddress = ip
            });
            return documentId;
        }

        public Document GetById(int documentId) => _documentRepository.GetById(documentId);

        public void RegistarDownload(int documentId, int userId, string ip)
        {
            _documentRepository.RegistarAcesso(new DocumentAccessLog
            {
                DocumentId = documentId,
                Action = "Download",
                UserId = userId,
                IpAddress = ip
            });
        }

        public void Eliminar(int documentId, int userId, string ip)
        {
            _documentRepository.EliminarLogico(documentId, userId);
            _documentRepository.RegistarAcesso(new DocumentAccessLog
            {
                DocumentId = documentId,
                Action = "Delete",
                UserId = userId,
                IpAddress = ip
            });
        }

        // ===================== Novo: autorização por documento (usado por DocumentoDownload.aspx) =====================
        public bool PodeAceder(Document document, int userId, string perfil)
        {
            if (document == null) return false;

            bool ehPrivilegiado = perfil == "Administrador" || perfil == "Diretor";

            if (document.IsConfidential && !ehPrivilegiado && document.CreatedBy != userId)
                return false;

            if (ehPrivilegiado) return true;

            ResolverEntidadeRelacionada(document, out string modulo, out int? donoId);
            if (modulo == null) return false;

            var nivel = _permissionService.ObterNivel(perfil, modulo);
            if (nivel == NivelAcesso.Nenhum) return false;
            if (nivel == NivelAcesso.Proprios) return donoId == userId;
            return true; // Consulta ou Total
        }

        // Novo: texto "Cliente: Nome" / "Proposta: PRP-001" para a listagem global.
        public string ObterDescricaoRelacionado(Document document)
        {
            if (document.RelatedClientId.HasValue)
                return "Cliente: " + (_clientRepository.GetById(document.RelatedClientId.Value)?.TradeName ?? ("#" + document.RelatedClientId));

            if (document.RelatedLeadId.HasValue)
                return "Lead: " + (_leadRepository.GetById(document.RelatedLeadId.Value)?.Name ?? ("#" + document.RelatedLeadId));

            if (document.RelatedOpportunityId.HasValue)
                return "Oportunidade: " + (_opportunityRepository.GetById(document.RelatedOpportunityId.Value)?.Title ?? ("#" + document.RelatedOpportunityId));

            if (document.RelatedProposalId.HasValue)
                return "Proposta: " + (_proposalRepository.GetById(document.RelatedProposalId.Value)?.ProposalNumber ?? ("#" + document.RelatedProposalId));

            if (document.RelatedSaleId.HasValue)
                return "Venda: " + (_saleRepository.GetById(document.RelatedSaleId.Value)?.SaleNumber ?? ("#" + document.RelatedSaleId));

            return "-";
        }

        private void ResolverEntidadeRelacionada(Document document, out string modulo, out int? donoId)
        {
            if (document.RelatedClientId.HasValue)
            {
                modulo = "Clientes";
                donoId = _clientRepository.GetById(document.RelatedClientId.Value)?.AccountManagerId;
                return;
            }
            if (document.RelatedLeadId.HasValue)
            {
                modulo = "Leads";
                donoId = _leadRepository.GetById(document.RelatedLeadId.Value)?.OwnerId;
                return;
            }
            if (document.RelatedOpportunityId.HasValue)
            {
                modulo = "Oportunidades";
                donoId = _opportunityRepository.GetById(document.RelatedOpportunityId.Value)?.OwnerId;
                return;
            }
            if (document.RelatedProposalId.HasValue)
            {
                var proposal = _proposalRepository.GetById(document.RelatedProposalId.Value);
                modulo = "Propostas";
                donoId = proposal?.Client?.AccountManagerId;
                return;
            }
            if (document.RelatedSaleId.HasValue)
            {
                modulo = "Vendas";
                donoId = _saleRepository.GetById(document.RelatedSaleId.Value)?.OwnerId;
                return;
            }
            modulo = null;
            donoId = null;
        }
    }
}