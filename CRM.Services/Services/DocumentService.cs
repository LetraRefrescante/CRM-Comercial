using System;
using System.Collections.Generic;
using System.IO;
using CRM.Data.Repositories;
using CRM.Models.Entities.Documentos;

namespace CRM.Services
{
    public class DocumentService
    {
        private readonly DocumentRepository _documentRepository = new DocumentRepository();
        private static readonly string[] ExtensoesPermitidas =
            { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".png", ".jpg", ".jpeg" };
        private const long TamanhoMaximoBytes = 10 * 1024 * 1024; // 10 MB

        public bool ExtensaoPermitida(string fileName)
        {
            var extensao = Path.GetExtension(fileName)?.ToLowerInvariant();
            return !string.IsNullOrEmpty(extensao) && Array.IndexOf(ExtensoesPermitidas, extensao) >= 0;
        }

        public bool TamanhoPermitido(long fileSizeBytes) => fileSizeBytes <= TamanhoMaximoBytes;

        // Só "Client" está ligado a uma entidade real por agora.
        public List<Document> Listar(string entityType, int entityId)
        {
            switch (entityType)
            {
                case "Client":
                    return _documentRepository.ListarPorCliente(entityId);
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
    }
}