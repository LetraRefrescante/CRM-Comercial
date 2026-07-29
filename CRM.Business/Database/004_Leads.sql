-- =============================================
-- Script: 004_Leads.sql
-- Descrição: Leads (potenciais clientes) e conversão
-- =============================================

USE CRM;
GO

-- =============================================
-- Tabela: Leads
-- =============================================
CREATE TABLE Leads (
    LeadId              INT             IDENTITY(1,1) PRIMARY KEY,
    Name                NVARCHAR(150)   NOT NULL,   -- Nome da pessoa ou empresa
    CompanyName         NVARCHAR(150)   NULL,
    Email               NVARCHAR(150)   NULL,
    Phone               NVARCHAR(30)    NULL,
    LeadSourceId        INT             NOT NULL,
    Status              NVARCHAR(20)    NOT NULL DEFAULT 'Novo', -- Novo, Em Contacto, Qualificado, Não Qualificado, Convertido
    Score               INT             NULL,       -- Pontuação 0-100
    OwnerId             INT             NOT NULL,   -- Comercial responsável (FK Users)
    NextContactDate     DATETIME2       NULL,       -- Próximo Contacto (deve ser futura quando estado ativo)
    LossReasonId        INT             NULL,       -- Motivo de Perda (obrigatório se Não Qualificado)

    -- Preenchidos apenas quando convertido
    ConvertedDate        DATETIME2       NULL,
    ConvertedByUserId    INT             NULL,
    ConvertedClientId    INT             NULL,       -- FK para Clients, preenchido na conversão
    ConvertedContactId   INT             NULL,       -- FK para Contacts, preenchido na conversão
    ConvertedOpportunityId INT           NULL,       -- FK para Opportunities, preenchido na conversão (tabela ainda não existe -> sem FK física por agora)

    CreatedDate          DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy            INT             NULL,
    UpdatedDate           DATETIME2       NULL,
    UpdatedBy             INT             NULL,
    IsDeleted             BIT             NOT NULL DEFAULT 0,
    DeletedDate           DATETIME2       NULL,
    DeletedBy             INT             NULL,
    RowVersion            ROWVERSION,

    CONSTRAINT FK_Leads_LeadSources FOREIGN KEY (LeadSourceId) REFERENCES LeadSources(LeadSourceId),
    CONSTRAINT FK_Leads_LossReasons FOREIGN KEY (LossReasonId) REFERENCES LossReasons(LossReasonId),
    CONSTRAINT FK_Leads_Owner FOREIGN KEY (OwnerId) REFERENCES Users(UserId),
    CONSTRAINT FK_Leads_ConvertedByUser FOREIGN KEY (ConvertedByUserId) REFERENCES Users(UserId),
    CONSTRAINT FK_Leads_ConvertedClient FOREIGN KEY (ConvertedClientId) REFERENCES Clients(ClientId),
    CONSTRAINT FK_Leads_ConvertedContact FOREIGN KEY (ConvertedContactId) REFERENCES Contacts(ContactId),
    CONSTRAINT FK_Leads_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_Leads_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_Leads_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES Users(UserId),

    CONSTRAINT CK_Leads_Status CHECK (Status IN ('Novo', 'Em Contacto', 'Qualificado', 'Não Qualificado', 'Convertido')),
    CONSTRAINT CK_Leads_Score CHECK (Score IS NULL OR (Score >= 0 AND Score <= 100)),
    CONSTRAINT CK_Leads_EmailOrPhone CHECK (Email IS NOT NULL OR Phone IS NOT NULL)
);
GO

-- Índices para filtros (origem, estado, comercial, pontuação, datas)
CREATE INDEX IX_Leads_Status ON Leads(Status);
CREATE INDEX IX_Leads_LeadSourceId ON Leads(LeadSourceId);
CREATE INDEX IX_Leads_OwnerId ON Leads(OwnerId);
CREATE INDEX IX_Leads_Score ON Leads(Score);
CREATE INDEX IX_Leads_NextContactDate ON Leads(NextContactDate);
CREATE INDEX IX_Leads_CreatedDate ON Leads(CreatedDate);
GO

-- Índice para deteção de duplicados por email/telefone
CREATE INDEX IX_Leads_Email ON Leads(Email) WHERE Email IS NOT NULL;
CREATE INDEX IX_Leads_Phone ON Leads(Phone) WHERE Phone IS NOT NULL;
GO

-- =============================================
-- Tabela: LeadStatusHistory
-- Regra: "Registar todas as alterações de estado"
-- =============================================
CREATE TABLE LeadStatusHistory (
    LeadStatusHistoryId INT             IDENTITY(1,1) PRIMARY KEY,
    LeadId              INT             NOT NULL,
    PreviousStatus      NVARCHAR(20)    NULL,
    NewStatus           NVARCHAR(20)    NOT NULL,
    ChangedDate         DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),
    ChangedBy           INT             NULL,

    CONSTRAINT FK_LeadStatusHistory_Leads FOREIGN KEY (LeadId) REFERENCES Leads(LeadId),
    CONSTRAINT FK_LeadStatusHistory_ChangedBy FOREIGN KEY (ChangedBy) REFERENCES Users(UserId)
);
GO

CREATE INDEX IX_LeadStatusHistory_LeadId ON LeadStatusHistory(LeadId);
GO