-- =============================================
-- Script: 001_Users_Roles_Permissions_AuditLogs.sql
-- Descrição: Tabelas base de autenticação, perfis, permissões e auditoria
-- =============================================

USE CRM;
GO

-- =============================================
-- Tabela: Roles
-- =============================================
CREATE TABLE Roles (
    RoleId          INT             IDENTITY(1,1) PRIMARY KEY,
    Name            NVARCHAR(50)    NOT NULL,
    Description     NVARCHAR(200)   NULL,

    CreatedDate     DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy       INT             NULL,
    UpdatedDate     DATETIME2       NULL,
    UpdatedBy       INT             NULL,
    IsDeleted       BIT             NOT NULL DEFAULT 0,
    DeletedDate     DATETIME2       NULL,
    DeletedBy       INT             NULL,
    RowVersion      ROWVERSION
);
GO

CREATE UNIQUE INDEX UX_Roles_Name ON Roles(Name) WHERE IsDeleted = 0;
GO

-- =============================================
-- Tabela: Permissions
-- =============================================
CREATE TABLE Permissions (
    PermissionId    INT             IDENTITY(1,1) PRIMARY KEY,
    Code            NVARCHAR(100)   NOT NULL,   -- ex: "Clientes.Total", "Vendas.Proprios"
    Module          NVARCHAR(50)    NOT NULL,   -- ex: "Clientes", "Vendas"
    Description     NVARCHAR(200)   NULL,

    CreatedDate     DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy       INT             NULL,
    UpdatedDate     DATETIME2       NULL,
    UpdatedBy       INT             NULL,
    IsDeleted       BIT             NOT NULL DEFAULT 0,
    DeletedDate     DATETIME2       NULL,
    DeletedBy       INT             NULL,
    RowVersion      ROWVERSION
);
GO

CREATE UNIQUE INDEX UX_Permissions_Code ON Permissions(Code) WHERE IsDeleted = 0;
GO

-- =============================================
-- Tabela: RolePermissions (junção Roles <-> Permissions)
-- =============================================
CREATE TABLE RolePermissions (
    RoleId          INT             NOT NULL,
    PermissionId    INT             NOT NULL,

    CreatedDate     DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy       INT             NULL,

    CONSTRAINT PK_RolePermissions PRIMARY KEY (RoleId, PermissionId),
    CONSTRAINT FK_RolePermissions_Roles FOREIGN KEY (RoleId) REFERENCES Roles(RoleId),
    CONSTRAINT FK_RolePermissions_Permissions FOREIGN KEY (PermissionId) REFERENCES Permissions(PermissionId)
);
GO

-- =============================================
-- Tabela: Users
-- =============================================
CREATE TABLE Users (
    UserId              INT             IDENTITY(1,1) PRIMARY KEY,
    Name                NVARCHAR(120)   NOT NULL,
    Email               NVARCHAR(150)   NOT NULL,
    PasswordHash        NVARCHAR(256)   NOT NULL,
    PasswordSalt        NVARCHAR(256)   NOT NULL,
    RoleId              INT             NOT NULL,
    Status              NVARCHAR(20)    NOT NULL DEFAULT 'Ativo', -- Ativo, Bloqueado, Inativo
    FailedLoginAttempts INT             NOT NULL DEFAULT 0,
    LockedUntil         DATETIME2       NULL,
    LastLoginDate       DATETIME2       NULL,

    CreatedDate         DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy           INT             NULL,
    UpdatedDate         DATETIME2       NULL,
    UpdatedBy           INT             NULL,
    IsDeleted           BIT             NOT NULL DEFAULT 0,
    DeletedDate         DATETIME2       NULL,
    DeletedBy           INT             NULL,
    RowVersion          ROWVERSION,

    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES Roles(RoleId)
);
GO

-- Índice único obrigatório em Users.Email
CREATE UNIQUE INDEX UX_Users_Email ON Users(Email) WHERE IsDeleted = 0;
GO

-- Agora que Users existe, adicionar as FKs de CreatedBy/UpdatedBy/DeletedBy em cascata
ALTER TABLE Roles ADD CONSTRAINT FK_Roles_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId);
ALTER TABLE Roles ADD CONSTRAINT FK_Roles_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES Users(UserId);
ALTER TABLE Roles ADD CONSTRAINT FK_Roles_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES Users(UserId);
GO

ALTER TABLE Permissions ADD CONSTRAINT FK_Permissions_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId);
ALTER TABLE Permissions ADD CONSTRAINT FK_Permissions_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES Users(UserId);
ALTER TABLE Permissions ADD CONSTRAINT FK_Permissions_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES Users(UserId);
GO

ALTER TABLE Users ADD CONSTRAINT FK_Users_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId);
ALTER TABLE Users ADD CONSTRAINT FK_Users_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES Users(UserId);
ALTER TABLE Users ADD CONSTRAINT FK_Users_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES Users(UserId);
GO

-- =============================================
-- Tabela: AuditLogs
-- =============================================
CREATE TABLE AuditLogs (
    AuditLogId      BIGINT          IDENTITY(1,1) PRIMARY KEY,
    UserId          INT             NULL,           -- NULL permitido p/ ações de sistema
    Action          NVARCHAR(50)    NOT NULL,        -- Create, Update, Delete, Login, LoginFailed, Logout, PasswordReset...
    EntityName      NVARCHAR(100)   NULL,            -- ex: "Client", "Opportunity"
    EntityId        NVARCHAR(50)    NULL,
    Details         NVARCHAR(MAX)   NULL,            -- JSON com valores antigos/novos, se aplicável
    IpAddress       NVARCHAR(45)    NULL,
    CreatedDate     DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_AuditLogs_Users FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
GO

CREATE INDEX IX_AuditLogs_UserId ON AuditLogs(UserId);
CREATE INDEX IX_AuditLogs_CreatedDate ON AuditLogs(CreatedDate);
CREATE INDEX IX_AuditLogs_Action ON AuditLogs(Action);
GO

-- =============================================
-- Seed: Roles base
-- =============================================
INSERT INTO Roles (Name, Description) VALUES
('Administrador', 'Acesso total a todos os módulos'),
('Diretor', 'Consulta total e gestão comercial'),
('Comercial', 'Acesso aos próprios registos'),
('Financeiro', 'Consulta comercial e gestão de vendas/pagamentos'),
('Consulta', 'Apenas consulta em todos os módulos');
GO