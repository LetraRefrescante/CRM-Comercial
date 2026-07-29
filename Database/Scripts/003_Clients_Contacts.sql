-- =============================================
-- Script: 003_Clients_Contacts.sql
-- Descrição: Clientes e Contactos associados
-- =============================================

USE CRM;
GO

-- =============================================
-- Tabela: Clients
-- =============================================
CREATE TABLE Clients (
    ClientId            INT             IDENTITY(1,1) PRIMARY KEY,
    InternalCode        NVARCHAR(20)    NOT NULL,   -- código interno gerado automaticamente
    CommercialName       NVARCHAR(150)   NOT NULL,   -- Nome Comercial (2-150 caracteres)
    LegalName            NVARCHAR(200)   NULL,       -- Nome Legal (máx 200)
    VatNumber            NVARCHAR(20)    NOT NULL,   -- NIF
    Email                NVARCHAR(150)   NULL,
    Phone                NVARCHAR(30)    NULL,
    Address              NVARCHAR(300)   NULL,
    PostalCode           NVARCHAR(20)    NULL,
    City                 NVARCHAR(100)   NULL,
    CountryId            INT             NOT NULL,
    SectorId             INT             NULL,
    AccountManagerId     INT             NOT NULL,   -- Comercial Responsável (FK Users)
    Status               NVARCHAR(20)    NOT NULL DEFAULT 'Potencial', -- Potencial, Ativo, Inativo, Bloqueado
    Notes                NVARCHAR(4000)  NULL,

    CreatedDate          DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy            INT             NULL,
    UpdatedDate          DATETIME2       NULL,
    UpdatedBy            INT             NULL,
    IsDeleted            BIT             NOT NULL DEFAULT 0,
    DeletedDate           DATETIME2       NULL,
    DeletedBy             INT             NULL,
    RowVersion            ROWVERSION,

    CONSTRAINT FK_Clients_Countries FOREIGN KEY (CountryId) REFERENCES Countries(CountryId),
    CONSTRAINT FK_Clients_Sectors FOREIGN KEY (SectorId) REFERENCES Sectors(SectorId),
    CONSTRAINT FK_Clients_AccountManager FOREIGN KEY (AccountManagerId) REFERENCES Users(UserId),
    CONSTRAINT FK_Clients_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_Clients_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_Clients_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES Users(UserId),

    CONSTRAINT CK_Clients_Status CHECK (Status IN ('Potencial', 'Ativo', 'Inativo', 'Bloqueado')),
    CONSTRAINT CK_Clients_CommercialName_Length CHECK (LEN(CommercialName) >= 2)
);
GO

-- Índice único filtrado: NIF único apenas entre clientes ativos e não eliminados
-- (Regra: "Não permitir clientes ativos duplicados pelo mesmo NIF")
CREATE UNIQUE INDEX UX_Clients_VatNumber 
ON Clients(VatNumber) 
WHERE IsDeleted = 0 AND Status = 'Ativo';
GO

-- Índices para pesquisa (nome, cidade, estado, comercial)
CREATE INDEX IX_Clients_CommercialName ON Clients(CommercialName);
CREATE INDEX IX_Clients_City ON Clients(City);
CREATE INDEX IX_Clients_Status ON Clients(Status);
CREATE INDEX IX_Clients_AccountManagerId ON Clients(AccountManagerId);
GO

-- =============================================
-- Tabela: Contacts
-- =============================================
CREATE TABLE Contacts (
    ContactId           INT             IDENTITY(1,1) PRIMARY KEY,
    ClientId             INT             NOT NULL,
    Name                 NVARCHAR(120)   NOT NULL,
    Position              NVARCHAR(100)   NULL,       -- Cargo
    Department            NVARCHAR(100)   NULL,
    Email                 NVARCHAR(150)   NULL,
    Phone                 NVARCHAR(30)    NULL,
    Mobile                NVARCHAR(30)    NULL,
    BirthDate             DATE            NULL,
    IsPrimary             BIT             NOT NULL DEFAULT 0,  -- Contacto principal
    ContactPreference     NVARCHAR(20)    NULL,       -- Email, Telefone, Telemóvel, Reunião
    ConsentGiven          BIT             NOT NULL DEFAULT 0,  -- Consentimento de contacto
    ContactRestrictions   NVARCHAR(500)   NULL,       -- Restrições de contacto

    CreatedDate           DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy             INT             NULL,
    UpdatedDate           DATETIME2       NULL,
    UpdatedBy             INT             NULL,
    IsDeleted             BIT             NOT NULL DEFAULT 0,
    DeletedDate           DATETIME2       NULL,
    DeletedBy             INT             NULL,
    RowVersion            ROWVERSION,

    CONSTRAINT FK_Contacts_Clients FOREIGN KEY (ClientId) REFERENCES Clients(ClientId),
    CONSTRAINT FK_Contacts_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_Contacts_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_Contacts_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES Users(UserId),

    CONSTRAINT CK_Contacts_BirthDate CHECK (BirthDate IS NULL OR BirthDate <= CAST(GETDATE() AS DATE)),
    CONSTRAINT CK_Contacts_Preference CHECK (ContactPreference IS NULL OR ContactPreference IN ('Email', 'Telefone', 'Telemóvel', 'Reunião'))
);
GO

CREATE INDEX IX_Contacts_ClientId ON Contacts(ClientId);
CREATE INDEX IX_Contacts_Name ON Contacts(Name);
GO