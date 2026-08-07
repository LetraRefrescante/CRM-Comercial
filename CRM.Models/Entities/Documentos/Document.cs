using System;
using System.ComponentModel.DataAnnotations.Schema;
using CRM.Models.Entities.Clientes;

namespace CRM.Models.Entities.Documentos
{
    [Table("Documents")]
    public class Document
    {
        public int DocumentId { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }

        public int? RelatedClientId { get; set; }
        public Client RelatedClient { get; set; }

        public int? RelatedLeadId { get; set; }
        public int? RelatedOpportunityId { get; set; }
        public int? RelatedProposalId { get; set; }
        public int? RelatedSaleId { get; set; }

        public string StoredFileName { get; set; }
        public string OriginalFileName { get; set; }
        public string MimeType { get; set; }
        public long FileSizeBytes { get; set; }

        public string VersionLabel { get; set; }
        public int? ParentDocumentId { get; set; }
        public bool IsConfidential { get; set; }

        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public int? DeletedBy { get; set; }
        public byte[] RowVersion { get; set; }
    }
}