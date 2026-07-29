-- =============================================
-- Script: 010_Documents.sql
-- Descrição: Documentos e anexos ligados a Clientes, Leads, Oportunidades, Propostas ou Vendas
-- =============================================

USE CRM;
GO

-- =============================================
-- Tabela: Documents
-- =============================================
CREATE TABLE Documents (
    DocumentId              INT             IDENTITY(1,1) PRIMARY KEY,
    Title                     NVARCHAR(180)   NOT NULL,
    Category                    NVARCHAR(30)    NOT NULL,   -- Contrato, Proposta, Identificação, Outro

    -- Relacionado Com (polimórfico: Cliente, Lead, Oportunidade, Proposta ou Venda)
    RelatedClientId               INT             NULL,
    RelatedLeadId                   INT             NULL,
    RelatedOpportunityId             INT             NULL,
    RelatedProposalId                 INT             NULL,
    RelatedSaleId                       INT             NULL,

    -- Ficheiro: guardado fora de pastas públicas; caminho/nome interno seguro, não o nome original direto
    StoredFileName                       NVARCHAR(260)   NOT NULL,   -- nome seguro gerado no disco/storage
    OriginalFileName                       NVARCHAR(260)   NOT NULL,   -- nome original do upload (só para exibição)
    MimeType                                 NVARCHAR(100)   NOT NULL,
    FileSizeBytes                             BIGINT          NOT NULL,

    VersionLabel                               NVARCHAR(20)    NULL,       -- gerada automaticamente quando aplicável
    ParentDocumentId                             INT             NULL,       -- para versionamento, tal como Proposals
    IsConfidential                                 BIT             NOT NULL DEFAULT 0,  -- restringe acesso por perfil

    CreatedDate                                     DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy                                       INT             NULL,
    UpdatedDate                                      DATETIME2       NULL,
    UpdatedBy                                        INT             NULL,
    IsDeleted                                        BIT             NOT NULL DEFAULT 0,
    DeletedDate                                      DATETIME2       NULL,
    DeletedBy                                        INT             NULL,
    RowVersion                                       ROWVERSION,

    CONSTRAINT FK_Documents_Clients FOREIGN KEY (RelatedClientId) REFERENCES Clients(ClientId),
    CONSTRAINT FK_Documents_Leads FOREIGN KEY (RelatedLeadId) REFERENCES Leads(LeadId),
    CONSTRAINT FK_Documents_Opportunities FOREIGN KEY (RelatedOpportunityId) REFERENCES Opportunities(OpportunityId),
    CONSTRAINT FK_Documents_Proposals FOREIGN KEY (RelatedProposalId) REFERENCES Proposals(ProposalId),
    CONSTRAINT FK_Documents_Sales FOREIGN KEY (RelatedSaleId) REFERENCES Sales(SaleId),
    CONSTRAINT FK_Documents_ParentDocument FOREIGN KEY (ParentDocumentId) REFERENCES Documents(DocumentId),
    CONSTRAINT FK_Documents_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_Documents_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_Documents_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES Users(UserId),

    CONSTRAINT CK_Documents_Category CHECK (Category IN ('Contrato', 'Proposta', 'Identificação', 'Outro')),

    -- Garante que está ligado a pelo menos UMA entidade
    CONSTRAINT CK_Documents_HasRelation CHECK (
        RelatedClientId IS NOT NULL OR RelatedLeadId IS NOT NULL OR 
        RelatedOpportunityId IS NOT NULL OR RelatedProposalId IS NOT NULL OR 
        RelatedSaleId IS NOT NULL
    )
);
GO

CREATE INDEX IX_Documents_RelatedClientId ON Documents(RelatedClientId);
CREATE INDEX IX_Documents_RelatedLeadId ON Documents(RelatedLeadId);
CREATE INDEX IX_Documents_RelatedOpportunityId ON Documents(RelatedOpportunityId);
CREATE INDEX IX_Documents_RelatedProposalId ON Documents(RelatedProposalId);
CREATE INDEX IX_Documents_RelatedSaleId ON Documents(RelatedSaleId);
CREATE INDEX IX_Documents_Category ON Documents(Category);
GO

-- =============================================
-- Tabela: DocumentAccessLog
-- Regra: "Registar upload, download e eliminação lógica"
-- =============================================
CREATE TABLE DocumentAccessLog (
    DocumentAccessLogId    INT             IDENTITY(1,1) PRIMARY KEY,
    DocumentId               INT             NOT NULL,
    Action                     NVARCHAR(20)    NOT NULL,   -- Upload, Download, Delete
    UserId                       INT             NULL,
    AccessDate                    DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),
    IpAddress                       NVARCHAR(45)    NULL,

    CONSTRAINT FK_DocumentAccessLog_Documents FOREIGN KEY (DocumentId) REFERENCES Documents(DocumentId),
    CONSTRAINT FK_DocumentAccessLog_Users FOREIGN KEY (UserId) REFERENCES Users(UserId),

    CONSTRAINT CK_DocumentAccessLog_Action CHECK (Action IN ('Upload', 'Download', 'Delete'))
);
GO

CREATE INDEX IX_DocumentAccessLog_DocumentId ON DocumentAccessLog(DocumentId);
GO