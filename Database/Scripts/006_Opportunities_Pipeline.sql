-- =============================================
-- Script: 006_Opportunities_Pipeline.sql
-- Descrição: Oportunidades, histórico de fase, e fecho da FK pendente de Leads
-- =============================================

USE CRM;
GO

-- =============================================
-- Tabela: Opportunities
-- =============================================
CREATE TABLE Opportunities (
    OpportunityId        INT             IDENTITY(1,1) PRIMARY KEY,
    Title                 NVARCHAR(180)   NOT NULL,
    ClientId               INT             NOT NULL,
    ContactId               INT             NULL,
    StageId                 INT             NOT NULL,
    EstimatedValue          DECIMAL(18,2)   NOT NULL,
    Probability              INT             NOT NULL,   -- 0-100, sugerida pela fase
    ExpectedCloseDate        DATE            NOT NULL,
    OwnerId                  INT             NOT NULL,   -- Comercial responsável
    Competitor                NVARCHAR(150)   NULL,
    LossReasonId              INT             NULL,       -- obrigatório no fecho perdido
    IsClosed                  BIT             NOT NULL DEFAULT 0,
    ClosedDate                 DATETIME2       NULL,

    CreatedDate                DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy                  INT             NULL,
    UpdatedDate                 DATETIME2       NULL,
    UpdatedBy                   INT             NULL,
    IsDeleted                   BIT             NOT NULL DEFAULT 0,
    DeletedDate                 DATETIME2       NULL,
    DeletedBy                   INT             NULL,
    RowVersion                  ROWVERSION,

    CONSTRAINT FK_Opportunities_Clients FOREIGN KEY (ClientId) REFERENCES Clients(ClientId),
    CONSTRAINT FK_Opportunities_Contacts FOREIGN KEY (ContactId) REFERENCES Contacts(ContactId),
    CONSTRAINT FK_Opportunities_Stages FOREIGN KEY (StageId) REFERENCES OpportunityStages(StageId),
    CONSTRAINT FK_Opportunities_Owner FOREIGN KEY (OwnerId) REFERENCES Users(UserId),
    CONSTRAINT FK_Opportunities_LossReasons FOREIGN KEY (LossReasonId) REFERENCES LossReasons(LossReasonId),
    CONSTRAINT FK_Opportunities_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_Opportunities_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_Opportunities_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES Users(UserId),

    CONSTRAINT CK_Opportunities_EstimatedValue CHECK (EstimatedValue >= 0),
    CONSTRAINT CK_Opportunities_Probability CHECK (Probability >= 0 AND Probability <= 100)
);
GO

-- Índices para pipeline e filtros
CREATE INDEX IX_Opportunities_ClientId ON Opportunities(ClientId);
CREATE INDEX IX_Opportunities_StageId ON Opportunities(StageId);
CREATE INDEX IX_Opportunities_OwnerId ON Opportunities(OwnerId);
CREATE INDEX IX_Opportunities_ExpectedCloseDate ON Opportunities(ExpectedCloseDate);
CREATE INDEX IX_Opportunities_IsClosed ON Opportunities(IsClosed);
GO

-- =============================================
-- Tabela: OpportunityStageHistory
-- Regra: "Histórico identifica fase anterior, nova fase, utilizador e data"
-- =============================================
CREATE TABLE OpportunityStageHistory (
    OpportunityStageHistoryId  INT             IDENTITY(1,1) PRIMARY KEY,
    OpportunityId               INT             NOT NULL,
    PreviousStageId               INT             NULL,
    NewStageId                     INT             NOT NULL,
    ChangedDate                     DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),
    ChangedBy                       INT             NULL,

    CONSTRAINT FK_OpportunityStageHistory_Opportunities FOREIGN KEY (OpportunityId) REFERENCES Opportunities(OpportunityId),
    CONSTRAINT FK_OpportunityStageHistory_PreviousStage FOREIGN KEY (PreviousStageId) REFERENCES OpportunityStages(StageId),
    CONSTRAINT FK_OpportunityStageHistory_NewStage FOREIGN KEY (NewStageId) REFERENCES OpportunityStages(StageId),
    CONSTRAINT FK_OpportunityStageHistory_ChangedBy FOREIGN KEY (ChangedBy) REFERENCES Users(UserId)
);
GO

CREATE INDEX IX_OpportunityStageHistory_OpportunityId ON OpportunityStageHistory(OpportunityId);
GO

-- =============================================
-- Fecha a FK pendente do script 004 (Leads.ConvertedOpportunityId)
-- =============================================
ALTER TABLE Leads 
ADD CONSTRAINT FK_Leads_ConvertedOpportunity 
FOREIGN KEY (ConvertedOpportunityId) REFERENCES Opportunities(OpportunityId);
GO