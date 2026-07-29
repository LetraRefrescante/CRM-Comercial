-- =============================================
-- Script: 007_Proposals_ProposalLines.sql
-- Descrição: Propostas Comerciais, linhas e versionamento
-- =============================================

USE CRM;
GO

-- =============================================
-- Tabela: Proposals
-- =============================================
CREATE TABLE Proposals (
    ProposalId            INT             IDENTITY(1,1) PRIMARY KEY,
    ProposalNumber         NVARCHAR(30)    NOT NULL,   -- Sequencial configurável
    ClientId                INT             NOT NULL,
    OpportunityId            INT             NULL,
    IssueDate                 DATE            NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    ValidUntil                 DATE            NOT NULL,
    Status                      NVARCHAR(20)    NOT NULL DEFAULT 'Rascunho', -- Rascunho, Enviada, Aceite, Recusada, Expirada, Cancelada
    GlobalDiscountPercent         DECIMAL(5,2)    NOT NULL DEFAULT 0,
    PaymentTermId                  INT             NULL,
    Notes                            NVARCHAR(4000)  NULL,

    -- Versionamento: proposta original ou versão de outra
    ParentProposalId                 INT             NULL,
    VersionNumber                     INT             NOT NULL DEFAULT 1,

    -- Totais calculados e gravados no momento (não recalculados dinamicamente depois)
    SubTotal                          DECIMAL(18,2)   NOT NULL DEFAULT 0,
    TaxTotal                            DECIMAL(18,2)   NOT NULL DEFAULT 0,
    Total                                DECIMAL(18,2)   NOT NULL DEFAULT 0,

    -- Aceitação
    AcceptedDate                         DATETIME2       NULL,
    AcceptedByUserId                       INT             NULL,
    AcceptanceNotes                         NVARCHAR(1000)  NULL,

    -- Envio
    SentDate                                DATETIME2       NULL,
    SentToEmail                              NVARCHAR(150)   NULL,

    CreatedDate                              DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy                                INT             NULL,
    UpdatedDate                               DATETIME2       NULL,
    UpdatedBy                                 INT             NULL,
    IsDeleted                                 BIT             NOT NULL DEFAULT 0,
    DeletedDate                               DATETIME2       NULL,
    DeletedBy                                 INT             NULL,
    RowVersion                                ROWVERSION,

    CONSTRAINT FK_Proposals_Clients FOREIGN KEY (ClientId) REFERENCES Clients(ClientId),
    CONSTRAINT FK_Proposals_Opportunities FOREIGN KEY (OpportunityId) REFERENCES Opportunities(OpportunityId),
    CONSTRAINT FK_Proposals_PaymentTerms FOREIGN KEY (PaymentTermId) REFERENCES PaymentTerms(PaymentTermId),
    CONSTRAINT FK_Proposals_ParentProposal FOREIGN KEY (ParentProposalId) REFERENCES Proposals(ProposalId),
    CONSTRAINT FK_Proposals_AcceptedByUser FOREIGN KEY (AcceptedByUserId) REFERENCES Users(UserId),
    CONSTRAINT FK_Proposals_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_Proposals_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_Proposals_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES Users(UserId),

    CONSTRAINT CK_Proposals_Status CHECK (Status IN ('Rascunho', 'Enviada', 'Aceite', 'Recusada', 'Expirada', 'Cancelada')),
    CONSTRAINT CK_Proposals_ValidUntil CHECK (ValidUntil >= IssueDate),
    CONSTRAINT CK_Proposals_Discount CHECK (GlobalDiscountPercent >= 0 AND GlobalDiscountPercent <= 100)
);
GO

CREATE UNIQUE INDEX UX_Proposals_ProposalNumber ON Proposals(ProposalNumber);
CREATE INDEX IX_Proposals_ClientId ON Proposals(ClientId);
CREATE INDEX IX_Proposals_Status ON Proposals(Status);
CREATE INDEX IX_Proposals_ValidUntil ON Proposals(ValidUntil);
GO

-- =============================================
-- Tabela: ProposalLines
-- =============================================
CREATE TABLE ProposalLines (
    ProposalLineId         INT             IDENTITY(1,1) PRIMARY KEY,
    ProposalId               INT             NOT NULL,
    ProductId                 INT             NOT NULL,
    LineOrder                   INT             NOT NULL DEFAULT 1,
    Description                   NVARCHAR(500)   NULL,   -- descrição livre da linha (pode diferir do produto)
    Quantity                       DECIMAL(18,3)   NOT NULL DEFAULT 1,
    UnitPrice                       DECIMAL(18,2)   NOT NULL,   -- preço copiado do produto NO MOMENTO da gravação
    DiscountPercent                   DECIMAL(5,2)    NOT NULL DEFAULT 0,
    TaxRateId                          INT             NOT NULL,   -- taxa copiada do produto no momento
    LineTotal                            DECIMAL(18,2)   NOT NULL,   -- (Quantity * UnitPrice) - desconto, sem IVA

    CONSTRAINT FK_ProposalLines_Proposals FOREIGN KEY (ProposalId) REFERENCES Proposals(ProposalId),
    CONSTRAINT FK_ProposalLines_Products FOREIGN KEY (ProductId) REFERENCES Products(ProductId),
    CONSTRAINT FK_ProposalLines_TaxRates FOREIGN KEY (TaxRateId) REFERENCES TaxRates(TaxRateId),

    CONSTRAINT CK_ProposalLines_Quantity CHECK (Quantity > 0),
    CONSTRAINT CK_ProposalLines_UnitPrice CHECK (UnitPrice >= 0),
    CONSTRAINT CK_ProposalLines_Discount CHECK (DiscountPercent >= 0 AND DiscountPercent <= 100)
);
GO

CREATE INDEX IX_ProposalLines_ProposalId ON ProposalLines(ProposalId);
GO