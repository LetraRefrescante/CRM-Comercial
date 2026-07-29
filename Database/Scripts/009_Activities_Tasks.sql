-- =============================================
-- Script: 009_Activities_Tasks.sql
-- Descrição: Atividades (chamadas, emails, reuniões, visitas, notas) e Tarefas
-- =============================================

USE CRM;
GO

-- =============================================
-- Tabela: Activities
-- Cobre: chamadas, emails, reuniões, visitas, notas (tudo o que tem Início/Fim)
-- =============================================
CREATE TABLE Activities (
    ActivityId              INT             IDENTITY(1,1) PRIMARY KEY,
    Type                       NVARCHAR(20)    NOT NULL,   -- Chamada, Email, Reunião, Visita, Nota
    Subject                     NVARCHAR(180)   NOT NULL,

    -- Relacionada Com (polimórfico: Cliente, Lead ou Oportunidade)
    RelatedClientId               INT             NULL,
    RelatedLeadId                   INT             NULL,
    RelatedOpportunityId             INT             NULL,

    AssignedToUserId                   INT             NOT NULL,   -- Responsável
    StartDateTime                        DATETIME2       NOT NULL,
    EndDateTime                            DATETIME2       NULL,
    Priority                                 NVARCHAR(20)    NULL,       -- Baixa, Normal, Alta, Urgente
    Status                                     NVARCHAR(20)    NOT NULL DEFAULT 'Planeada', -- Planeada, Em Curso, Concluída, Cancelada
    Description                                 NVARCHAR(4000)  NULL,
    ReminderDateTime                              DATETIME2       NULL,
    CompletedDateTime                               DATETIME2       NULL,   -- data/hora real de conclusão

    CreatedDate                                     DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy                                       INT             NULL,
    UpdatedDate                                      DATETIME2       NULL,
    UpdatedBy                                        INT             NULL,
    IsDeleted                                        BIT             NOT NULL DEFAULT 0,
    DeletedDate                                      DATETIME2       NULL,
    DeletedBy                                        INT             NULL,
    RowVersion                                       ROWVERSION,

    CONSTRAINT FK_Activities_Clients FOREIGN KEY (RelatedClientId) REFERENCES Clients(ClientId),
    CONSTRAINT FK_Activities_Leads FOREIGN KEY (RelatedLeadId) REFERENCES Leads(LeadId),
    CONSTRAINT FK_Activities_Opportunities FOREIGN KEY (RelatedOpportunityId) REFERENCES Opportunities(OpportunityId),
    CONSTRAINT FK_Activities_AssignedTo FOREIGN KEY (AssignedToUserId) REFERENCES Users(UserId),
    CONSTRAINT FK_Activities_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_Activities_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_Activities_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES Users(UserId),

    CONSTRAINT CK_Activities_Type CHECK (Type IN ('Chamada', 'Email', 'Reunião', 'Visita', 'Nota')),
    CONSTRAINT CK_Activities_Priority CHECK (Priority IS NULL OR Priority IN ('Baixa', 'Normal', 'Alta', 'Urgente')),
    CONSTRAINT CK_Activities_Status CHECK (Status IN ('Planeada', 'Em Curso', 'Concluída', 'Cancelada')),
    CONSTRAINT CK_Activities_EndAfterStart CHECK (EndDateTime IS NULL OR EndDateTime >= StartDateTime)
);
GO

CREATE INDEX IX_Activities_AssignedToUserId ON Activities(AssignedToUserId);
CREATE INDEX IX_Activities_StartDateTime ON Activities(StartDateTime);
CREATE INDEX IX_Activities_Status ON Activities(Status);
CREATE INDEX IX_Activities_Type ON Activities(Type);
CREATE INDEX IX_Activities_RelatedClientId ON Activities(RelatedClientId);
CREATE INDEX IX_Activities_RelatedLeadId ON Activities(RelatedLeadId);
CREATE INDEX IX_Activities_RelatedOpportunityId ON Activities(RelatedOpportunityId);
GO

-- =============================================
-- Tabela: ActivityParticipants
-- Regra: "Reuniões podem ter participantes internos e externos"
-- =============================================
CREATE TABLE ActivityParticipants (
    ActivityParticipantId    INT             IDENTITY(1,1) PRIMARY KEY,
    ActivityId                 INT             NOT NULL,
    UserId                       INT             NULL,       -- participante interno (se aplicável)
    ExternalName                  NVARCHAR(150)   NULL,       -- participante externo (nome livre)
    ExternalEmail                   NVARCHAR(150)   NULL,

    CONSTRAINT FK_ActivityParticipants_Activities FOREIGN KEY (ActivityId) REFERENCES Activities(ActivityId),
    CONSTRAINT FK_ActivityParticipants_Users FOREIGN KEY (UserId) REFERENCES Users(UserId),

    CONSTRAINT CK_ActivityParticipants_InternalOrExternal CHECK (UserId IS NOT NULL OR ExternalName IS NOT NULL)
);
GO

CREATE INDEX IX_ActivityParticipants_ActivityId ON ActivityParticipants(ActivityId);
GO

-- =============================================
-- Tabela: Tasks
-- Nota: "Tasks" é palavra reservada em alguns contextos, mas é nome de tabela válido em SQL Server
-- =============================================
CREATE TABLE Tasks (
    TaskId                    INT             IDENTITY(1,1) PRIMARY KEY,
    Subject                     NVARCHAR(180)   NOT NULL,

    RelatedClientId               INT             NULL,
    RelatedLeadId                   INT             NULL,
    RelatedOpportunityId             INT             NULL,

    AssignedToUserId                   INT             NOT NULL,
    DueDate                               DATETIME2       NOT NULL,
    Priority                               NVARCHAR(20)    NULL,
    Status                                   NVARCHAR(20)    NOT NULL DEFAULT 'Planeada', -- reaproveita mesmos estados
    Description                               NVARCHAR(4000)  NULL,
    CompletedDateTime                           DATETIME2       NULL,

    CreatedDate                                 DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy                                   INT             NULL,
    UpdatedDate                                  DATETIME2       NULL,
    UpdatedBy                                    INT             NULL,
    IsDeleted                                    BIT             NOT NULL DEFAULT 0,
    DeletedDate                                  DATETIME2       NULL,
    DeletedBy                                    INT             NULL,
    RowVersion                                   ROWVERSION,

    CONSTRAINT FK_Tasks_Clients FOREIGN KEY (RelatedClientId) REFERENCES Clients(ClientId),
    CONSTRAINT FK_Tasks_Leads FOREIGN KEY (RelatedLeadId) REFERENCES Leads(LeadId),
    CONSTRAINT FK_Tasks_Opportunities FOREIGN KEY (RelatedOpportunityId) REFERENCES Opportunities(OpportunityId),
    CONSTRAINT FK_Tasks_AssignedTo FOREIGN KEY (AssignedToUserId) REFERENCES Users(UserId),
    CONSTRAINT FK_Tasks_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_Tasks_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_Tasks_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES Users(UserId),

    CONSTRAINT CK_Tasks_Priority CHECK (Priority IS NULL OR Priority IN ('Baixa', 'Normal', 'Alta', 'Urgente')),
    CONSTRAINT CK_Tasks_Status CHECK (Status IN ('Planeada', 'Em Curso', 'Concluída', 'Cancelada'))
);
GO

CREATE INDEX IX_Tasks_AssignedToUserId ON Tasks(AssignedToUserId);
CREATE INDEX IX_Tasks_DueDate ON Tasks(DueDate);
CREATE INDEX IX_Tasks_Status ON Tasks(Status);
GO