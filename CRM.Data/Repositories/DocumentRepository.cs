using System;
using System.Collections.Generic;
using System.Linq;
using CRM.Data.Context;
using CRM.Models.Entities.Documentos;

namespace CRM.Data.Repositories
{
    public class DocumentRepository
    {
        public List<Document> ListarPorCliente(int clientId)
        {
            using (var context = new CrmDbContext())
            {
                return context.Documents
                    .Where(d => d.RelatedClientId == clientId && !d.IsDeleted)
                    .OrderByDescending(d => d.CreatedDate)
                    .ToList();
            }
        }

        public List<Document> ListarPorVenda(int saleId)
        {
            using (var context = new CrmDbContext())
            {
                return context.Documents
                    .Where(d => d.RelatedSaleId == saleId && !d.IsDeleted)
                    .OrderByDescending(d => d.CreatedDate)
                    .ToList();
            }
        }

        public List<Document> ListarPorProposta(int proposalId)
        {
            using (var context = new CrmDbContext())
            {
                return context.Documents
                    .Where(d => d.RelatedProposalId == proposalId && !d.IsDeleted)
                    .OrderByDescending(d => d.CreatedDate)
                    .ToList();
            }
        }

        public Document GetById(int documentId)
        {
            using (var context = new CrmDbContext())
            {
                return context.Documents.SingleOrDefault(d => d.DocumentId == documentId && !d.IsDeleted);
            }
        }

        public int Criar(Document document)
        {
            using (var context = new CrmDbContext())
            {
                document.CreatedDate = DateTime.UtcNow;
                context.Documents.Add(document);
                context.SaveChanges();
                return document.DocumentId;
            }
        }

        public void EliminarLogico(int documentId, int eliminadoPor)
        {
            using (var context = new CrmDbContext())
            {
                var document = context.Documents.Find(documentId);
                if (document == null) return;
                document.IsDeleted = true;
                document.DeletedDate = DateTime.UtcNow;
                document.DeletedBy = eliminadoPor;
                context.SaveChanges();
            }
        }

        public void RegistarAcesso(DocumentAccessLog log)
        {
            using (var context = new CrmDbContext())
            {
                log.AccessDate = DateTime.UtcNow;
                context.DocumentAccessLogs.Add(log);
                context.SaveChanges();
            }
        }
    }
}