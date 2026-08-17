using CRM.Data.Context;
using CRM.Models.Entities.Documentos;
using CRM.Models.Filtros;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public List<Document> ListarPorLead(int leadId)
        {
            using (var context = new CrmDbContext())
            {
                return context.Documents
                    .Where(d => d.RelatedLeadId == leadId && !d.IsDeleted)
                    .OrderByDescending(d => d.CreatedDate)
                    .ToList();
            }
        }

        public List<Document> ListarPorOportunidade(int opportunityId)
        {
            using (var context = new CrmDbContext())
            {
                return context.Documents
                    .Where(d => d.RelatedOpportunityId == opportunityId && !d.IsDeleted)
                    .OrderByDescending(d => d.CreatedDate)
                    .ToList();
            }
        }

        public List<Document> Pesquisar(
            DocumentFiltro filtro,
            int pagina, int tamanhoPagina, out int totalRegistos,
            string sortColumn, bool sortAscending)
        {
            using (var context = new CrmDbContext())
            {
                var query = context.Documents.Where(d => !d.IsDeleted);

                if (!string.IsNullOrWhiteSpace(filtro.Pesquisa))
                    query = query.Where(d => d.Title.Contains(filtro.Pesquisa) || d.OriginalFileName.Contains(filtro.Pesquisa));

                if (!string.IsNullOrEmpty(filtro.Category))
                    query = query.Where(d => d.Category == filtro.Category);

                if (filtro.IsConfidential.HasValue)
                    query = query.Where(d => d.IsConfidential == filtro.IsConfidential.Value);

                if (filtro.DataInicio.HasValue)
                    query = query.Where(d => d.CreatedDate >= filtro.DataInicio.Value);

                if (filtro.DataFim.HasValue)
                    query = query.Where(d => d.CreatedDate <= filtro.DataFim.Value);

                switch (filtro.EntityType)
                {
                    case "Client": query = query.Where(d => d.RelatedClientId != null); break;
                    case "Lead": query = query.Where(d => d.RelatedLeadId != null); break;
                    case "Opportunity": query = query.Where(d => d.RelatedOpportunityId != null); break;
                    case "Proposal": query = query.Where(d => d.RelatedProposalId != null); break;
                    case "Sale": query = query.Where(d => d.RelatedSaleId != null); break;
                }

                totalRegistos = query.Count();

                query = sortColumn == "Title"
                    ? (sortAscending ? query.OrderBy(d => d.Title) : query.OrderByDescending(d => d.Title))
                    : (sortAscending ? query.OrderBy(d => d.CreatedDate) : query.OrderByDescending(d => d.CreatedDate));

                return query.Skip((pagina - 1) * tamanhoPagina).Take(tamanhoPagina).ToList();
            }
        }
    }
}