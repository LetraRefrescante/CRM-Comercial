-- =============================================
-- Script: 011_Notifications_Email.sql
-- Descrição: Notificações internas, templates de email e histórico de envios
-- =============================================

USE CRM;
GO

-- =============================================
-- Tabela: Notifications
-- Centro de notificações internas (Notificacoes.aspx)
-- =============================================
CREATE TABLE Notifications (
    NotificationId          INT             IDENTITY(1,1) PRIMARY KEY,
    UserId                    INT             NOT NULL,   -- destinatário interno
    Title                       NVARCHAR(180)   NOT NULL,
    Message                       NVARCHAR(1000)  NOT NULL,
    NotificationType                NVARCHAR(30)    NULL,       -- ex: "TarefaVencida", "PropostaExpirando", "LeadSemAtividade"

    -- Ligação ao registo de origem (polimórfico, tal como Documents/Activities)
    RelatedClientId                   INT             NULL,
    RelatedLeadId                       INT             NULL,
    RelatedOpportunityId                 INT             NULL,
    RelatedProposalId                     INT             NULL,
    RelatedSaleId                           INT             NULL,
    RelatedTaskId                             INT             NULL,

    IsRead                                     BIT             NOT NULL DEFAULT 0,
    ReadDate                                     DATETIME2       NULL,
    IsArchived                                     BIT             NOT NULL DEFAULT 0,
    ArchivedDate                                     DATETIME2       NULL,

    CreatedDate                                       DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_Notifications_Users FOREIGN KEY (UserId) REFERENCES Users(UserId),
    CONSTRAINT FK_Notifications_Clients FOREIGN KEY (RelatedClientId) REFERENCES Clients(ClientId),
    CONSTRAINT FK_Notifications_Leads FOREIGN KEY (RelatedLeadId) REFERENCES Leads(LeadId),
    CONSTRAINT FK_Notifications_Opportunities FOREIGN KEY (RelatedOpportunityId) REFERENCES Opportunities(OpportunityId),
    CONSTRAINT FK_Notifications_Proposals FOREIGN KEY (RelatedProposalId) REFERENCES Proposals(ProposalId),
    CONSTRAINT FK_Notifications_Sales FOREIGN KEY (RelatedSaleId) REFERENCES Sales(SaleId),
    CONSTRAINT FK_Notifications_Tasks FOREIGN KEY (RelatedTaskId) REFERENCES Tasks(TaskId)
);
GO

CREATE INDEX IX_Notifications_UserId ON Notifications(UserId);
CREATE INDEX IX_Notifications_IsRead ON Notifications(IsRead);
CREATE INDEX IX_Notifications_CreatedDate ON Notifications(CreatedDate);
GO

-- =============================================
-- Tabela: EmailTemplates
-- =============================================
CREATE TABLE EmailTemplates (
    EmailTemplateId          INT             IDENTITY(1,1) PRIMARY KEY,
    Name                       NVARCHAR(150)   NOT NULL,
    Subject                      NVARCHAR(200)   NOT NULL,
    Body                          NVARCHAR(MAX)   NOT NULL,   -- HTML sanitizado; variáveis tipo {{ClientName}}, {{ProposalNumber}}
    IsActive                       BIT             NOT NULL DEFAULT 1,

    CreatedDate                     DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy                       INT             NULL,
    UpdatedDate                      DATETIME2       NULL,
    UpdatedBy                        INT             NULL,

    CONSTRAINT FK_EmailTemplates_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_EmailTemplates_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES Users(UserId)
);
GO

CREATE UNIQUE INDEX UX_EmailTemplates_Name ON EmailTemplates(Name);
GO

-- =============================================
-- Tabela: EmailHistory
-- =============================================
CREATE TABLE EmailHistory (
    EmailHistoryId            INT             IDENTITY(1,1) PRIMARY KEY,
    ToAddress                   NVARCHAR(150)   NOT NULL,
    Subject                       NVARCHAR(200)   NOT NULL,
    Body                            NVARCHAR(MAX)   NOT NULL,
    EmailTemplateId                   INT             NULL,

    -- Relacionada Com (Cliente, Contacto, Lead, Oportunidade, Proposta)
    RelatedClientId                     INT             NULL,
    RelatedContactId                      INT             NULL,
    RelatedLeadId                           INT             NULL,
    RelatedOpportunityId                     INT             NULL,
    RelatedProposalId                         INT             NULL,

    SentDate                                   DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),
    SentByUserId                                 INT             NULL,
    Status                                         NVARCHAR(20)    NOT NULL DEFAULT 'Enviado', -- Enviado, Falhou
    FailureReason                                    NVARCHAR(500)   NULL,   -- sem expor credenciais

    CONSTRAINT FK_EmailHistory_Templates FOREIGN KEY (EmailTemplateId) REFERENCES EmailTemplates(EmailTemplateId),
    CONSTRAINT FK_EmailHistory_Clients FOREIGN KEY (RelatedClientId) REFERENCES Clients(ClientId),
    CONSTRAINT FK_EmailHistory_Contacts FOREIGN KEY (RelatedContactId) REFERENCES Contacts(ContactId),
    CONSTRAINT FK_EmailHistory_Leads FOREIGN KEY (RelatedLeadId) REFERENCES Leads(LeadId),
    CONSTRAINT FK_EmailHistory_Opportunities FOREIGN KEY (RelatedOpportunityId) REFERENCES Opportunities(OpportunityId),
    CONSTRAINT FK_EmailHistory_Proposals FOREIGN KEY (RelatedProposalId) REFERENCES Proposals(ProposalId),
    CONSTRAINT FK_EmailHistory_SentByUser FOREIGN KEY (SentByUserId) REFERENCES Users(UserId),

    CONSTRAINT CK_EmailHistory_Status CHECK (Status IN ('Enviado', 'Falhou'))
);
GO

CREATE INDEX IX_EmailHistory_SentDate ON EmailHistory(SentDate);
CREATE INDEX IX_EmailHistory_RelatedClientId ON EmailHistory(RelatedClientId);
GO

-- =============================================
-- Fecha a FK pendente do script 010 (Documents relacionado com Proposta/Venda já existiam,
-- mas Notifications.RelatedTaskId dependia de Tasks, que já existe desde o script 009 — sem pendências)
-- =============================================