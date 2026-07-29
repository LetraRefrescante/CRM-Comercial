-- =============================================
-- Script: 002_Countries_Settings_ListasAuxiliares.sql
-- Descrição: Tabelas de apoio/configuração usadas por Clientes, Leads, 
--            Oportunidades, Catálogo e Propostas
-- =============================================

USE CRM;
GO

-- =============================================
-- Tabela: Countries
-- Usada em: Clientes.País
-- =============================================
CREATE TABLE Countries (
    CountryId       INT             IDENTITY(1,1) PRIMARY KEY,
    Code            NVARCHAR(3)     NOT NULL,   -- ISO ex: "PT", "ES", "BR"
    Name            NVARCHAR(100)   NOT NULL,
    IsActive        BIT             NOT NULL DEFAULT 1,

    CreatedDate     DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy       INT             NULL,
    UpdatedDate     DATETIME2       NULL,
    UpdatedBy       INT             NULL,

    CONSTRAINT FK_Countries_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_Countries_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES Users(UserId)
);
GO

CREATE UNIQUE INDEX UX_Countries_Code ON Countries(Code);
GO

-- =============================================
-- Tabela: Sectors
-- Usada em: Clientes.Setor
-- =============================================
CREATE TABLE Sectors (
    SectorId        INT             IDENTITY(1,1) PRIMARY KEY,
    Name            NVARCHAR(100)   NOT NULL,
    IsActive        BIT             NOT NULL DEFAULT 1,

    CreatedDate     DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy       INT             NULL,
    UpdatedDate     DATETIME2       NULL,
    UpdatedBy       INT             NULL,

    CONSTRAINT FK_Sectors_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_Sectors_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES Users(UserId)
);
GO

-- =============================================
-- Tabela: LeadSources
-- Usada em: Leads.Origem
-- =============================================
CREATE TABLE LeadSources (
    LeadSourceId    INT             IDENTITY(1,1) PRIMARY KEY,
    Name            NVARCHAR(100)   NOT NULL,   -- Website, Referência, Evento, Campanha, Telefone, Outro
    IsActive        BIT             NOT NULL DEFAULT 1,

    CreatedDate     DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy       INT             NULL,
    UpdatedDate     DATETIME2       NULL,
    UpdatedBy       INT             NULL,

    CONSTRAINT FK_LeadSources_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_LeadSources_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES Users(UserId)
);
GO

-- =============================================
-- Tabela: LossReasons
-- Usada em: Leads.MotivoDePerda e Oportunidades.MotivoPerda
-- =============================================
CREATE TABLE LossReasons (
    LossReasonId    INT             IDENTITY(1,1) PRIMARY KEY,
    Name            NVARCHAR(150)   NOT NULL,
    IsActive        BIT             NOT NULL DEFAULT 1,

    CreatedDate     DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy       INT             NULL,
    UpdatedDate     DATETIME2       NULL,
    UpdatedBy       INT             NULL,

    CONSTRAINT FK_LossReasons_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_LossReasons_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES Users(UserId)
);
GO

-- =============================================
-- Tabela: TaxRates
-- Usada em: Produtos.TaxaIVA
-- =============================================
CREATE TABLE TaxRates (
    TaxRateId       INT             IDENTITY(1,1) PRIMARY KEY,
    Name            NVARCHAR(50)    NOT NULL,   -- ex: "Normal", "Intermédia", "Reduzida", "Isenta"
    Percentage      DECIMAL(5,2)    NOT NULL,   -- ex: 23.00
    IsActive        BIT             NOT NULL DEFAULT 1,

    CreatedDate     DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy       INT             NULL,
    UpdatedDate     DATETIME2       NULL,
    UpdatedBy       INT             NULL,

    CONSTRAINT FK_TaxRates_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_TaxRates_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES Users(UserId)
);
GO

-- =============================================
-- Tabela: PaymentTerms
-- Usada em: Propostas.CondiçõesPagamento
-- =============================================
CREATE TABLE PaymentTerms (
    PaymentTermId   INT             IDENTITY(1,1) PRIMARY KEY,
    Name            NVARCHAR(100)   NOT NULL,   -- ex: "Pronto pagamento", "30 dias", "60 dias"
    DaysDue         INT             NULL,       -- número de dias até vencimento
    IsActive        BIT             NOT NULL DEFAULT 1,

    CreatedDate     DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy       INT             NULL,
    UpdatedDate     DATETIME2       NULL,
    UpdatedBy       INT             NULL,

    CONSTRAINT FK_PaymentTerms_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_PaymentTerms_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES Users(UserId)
);
GO

-- =============================================
-- Tabela: OpportunityStages
-- Usada em: Oportunidades.Fase (Pipeline)
-- =============================================
CREATE TABLE OpportunityStages (
    StageId                 INT             IDENTITY(1,1) PRIMARY KEY,
    Name                    NVARCHAR(100)   NOT NULL,   -- ex: "Qualificação", "Proposta", "Negociação", "Fechado Ganho", "Fechado Perdido"
    OrderIndex              INT             NOT NULL,   -- ordem no Kanban
    DefaultProbability      INT             NOT NULL DEFAULT 0,  -- 0 a 100, sugerida ao entrar nesta fase
    IsClosedWon             BIT             NOT NULL DEFAULT 0,
    IsClosedLost            BIT             NOT NULL DEFAULT 0,
    IsActive                BIT             NOT NULL DEFAULT 1,

    CreatedDate             DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy               INT             NULL,
    UpdatedDate             DATETIME2       NULL,
    UpdatedBy               INT             NULL,

    CONSTRAINT FK_OpportunityStages_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_OpportunityStages_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES Users(UserId)
);
GO

-- =============================================
-- Tabela: Settings
-- Parâmetros gerais da aplicação (linha única de configuração)
-- =============================================
CREATE TABLE Settings (
    SettingId               INT             IDENTITY(1,1) PRIMARY KEY,
    CompanyName             NVARCHAR(150)   NOT NULL,
    Currency                NVARCHAR(3)     NOT NULL DEFAULT 'EUR',
    TimeZone                NVARCHAR(50)    NOT NULL DEFAULT 'Europe/Lisbon',
    AlertDaysLeads          INT             NOT NULL DEFAULT 7,
    AlertDaysOpportunities  INT             NOT NULL DEFAULT 7,
    AlertDaysProposals      INT             NOT NULL DEFAULT 7,
    MaxFailedLoginAttempts  INT             NOT NULL DEFAULT 5,
    AccountLockoutMinutes   INT             NOT NULL DEFAULT 15,
    SessionTimeoutMinutes   INT             NOT NULL DEFAULT 30,

    UpdatedDate             DATETIME2       NULL,
    UpdatedBy               INT             NULL,

    CONSTRAINT FK_Settings_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES Users(UserId)
);
GO

-- =============================================
-- Seed: dados iniciais
-- =============================================

-- Países (principais + PT primeiro)
INSERT INTO Countries (Code, Name) VALUES
('PT', 'Portugal'),
('ES', 'Espanha'),
('FR', 'França'),
('BR', 'Brasil'),
('AO', 'Angola'),
('MZ', 'Moçambique');
GO

-- Origens de Lead
INSERT INTO LeadSources (Name) VALUES
('Website'),
('Referência'),
('Evento'),
('Campanha'),
('Telefone'),
('Outro');
GO

-- Motivos de Perda
INSERT INTO LossReasons (Name) VALUES
('Preço'),
('Concorrência'),
('Sem orçamento'),
('Sem resposta do cliente'),
('Projeto cancelado'),
('Outro');
GO

-- Taxas de IVA (Portugal)
INSERT INTO TaxRates (Name, Percentage) VALUES
('Normal', 23.00),
('Intermédia', 13.00),
('Reduzida', 6.00),
('Isenta', 0.00);
GO

-- Condições de Pagamento
INSERT INTO PaymentTerms (Name, DaysDue) VALUES
('Pronto pagamento', 0),
('30 dias', 30),
('60 dias', 60),
('90 dias', 90);
GO

-- Fases do Pipeline
INSERT INTO OpportunityStages (Name, OrderIndex, DefaultProbability, IsClosedWon, IsClosedLost) VALUES
('Qualificação', 1, 10, 0, 0),
('Análise de Necessidades', 2, 25, 0, 0),
('Proposta Enviada', 3, 50, 0, 0),
('Negociação', 4, 75, 0, 0),
('Fechado Ganho', 5, 100, 1, 0),
('Fechado Perdido', 6, 0, 0, 1);
GO

-- Configuração inicial (linha única)
INSERT INTO Settings (CompanyName, Currency, TimeZone) VALUES
('Nome da Empresa, Lda.', 'EUR', 'Europe/Lisbon');
GO