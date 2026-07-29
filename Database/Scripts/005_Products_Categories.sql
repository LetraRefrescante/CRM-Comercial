-- =============================================
-- Script: 005_Products_Categories.sql
-- Descrição: Catálogo de Produtos/Serviços e Categorias
-- =============================================

USE CRM;
GO

-- =============================================
-- Tabela: Categories
-- =============================================
CREATE TABLE Categories (
    CategoryId          INT             IDENTITY(1,1) PRIMARY KEY,
    Name                NVARCHAR(100)   NOT NULL,
    IsActive            BIT             NOT NULL DEFAULT 1,

    CreatedDate         DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy           INT             NULL,
    UpdatedDate          DATETIME2       NULL,
    UpdatedBy            INT             NULL,

    CONSTRAINT FK_Categories_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_Categories_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES Users(UserId)
);
GO

CREATE UNIQUE INDEX UX_Categories_Name ON Categories(Name);
GO

-- =============================================
-- Tabela: Products
-- =============================================
CREATE TABLE Products (
    ProductId           INT             IDENTITY(1,1) PRIMARY KEY,
    Code                 NVARCHAR(30)    NOT NULL,   -- Código único
    Type                 NVARCHAR(20)    NOT NULL,   -- Produto ou Serviço
    Name                 NVARCHAR(180)   NOT NULL,
    CategoryId           INT             NOT NULL,
    Description           NVARCHAR(4000)  NULL,
    BasePrice             DECIMAL(18,2)   NOT NULL,
    TaxRateId             INT             NOT NULL,
    Unit                  NVARCHAR(20)    NOT NULL,   -- Unidade, Hora, Dia, Mês, Pacote
    IsActive               BIT             NOT NULL DEFAULT 1,  -- Inativos não podem ser adicionados a novos documentos

    CreatedDate            DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy              INT             NULL,
    UpdatedDate             DATETIME2       NULL,
    UpdatedBy               INT             NULL,
    IsDeleted               BIT             NOT NULL DEFAULT 0,  -- itens usados não podem ser eliminados fisicamente
    DeletedDate             DATETIME2       NULL,
    DeletedBy               INT             NULL,
    RowVersion               ROWVERSION,

    CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId),
    CONSTRAINT FK_Products_TaxRates FOREIGN KEY (TaxRateId) REFERENCES TaxRates(TaxRateId),
    CONSTRAINT FK_Products_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_Products_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_Products_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES Users(UserId),

    CONSTRAINT CK_Products_Type CHECK (Type IN ('Produto', 'Serviço')),
    CONSTRAINT CK_Products_Unit CHECK (Unit IN ('Unidade', 'Hora', 'Dia', 'Mês', 'Pacote')),
    CONSTRAINT CK_Products_BasePrice CHECK (BasePrice >= 0)
);
GO

-- Código não pode repetir (apenas entre não eliminados)
CREATE UNIQUE INDEX UX_Products_Code ON Products(Code) WHERE IsDeleted = 0;
GO

-- Índices de pesquisa (código, nome, tipo, categoria, estado)
CREATE INDEX IX_Products_Name ON Products(Name);
CREATE INDEX IX_Products_Type ON Products(Type);
CREATE INDEX IX_Products_CategoryId ON Products(CategoryId);
CREATE INDEX IX_Products_IsActive ON Products(IsActive);
GO

-- =============================================
-- Tabela: PriceTables
-- Usada em: TabelasPreco.aspx — preços por tabela comercial
-- =============================================
CREATE TABLE PriceTables (
    PriceTableId         INT             IDENTITY(1,1) PRIMARY KEY,
    Name                  NVARCHAR(100)   NOT NULL,   -- ex: "Tabela Padrão", "Tabela Revenda", "Tabela Promocional"
    IsDefault             BIT             NOT NULL DEFAULT 0,
    IsActive              BIT             NOT NULL DEFAULT 1,

    CreatedDate            DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy              INT             NULL,
    UpdatedDate             DATETIME2       NULL,
    UpdatedBy               INT             NULL,

    CONSTRAINT FK_PriceTables_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_PriceTables_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES Users(UserId)
);
GO

-- =============================================
-- Tabela: PriceTableItems
-- Preço de cada produto dentro de cada tabela de preço
-- =============================================
CREATE TABLE PriceTableItems (
    PriceTableItemId      INT             IDENTITY(1,1) PRIMARY KEY,
    PriceTableId           INT             NOT NULL,
    ProductId               INT             NOT NULL,
    Price                    DECIMAL(18,2)   NOT NULL,

    CreatedDate              DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy                INT             NULL,
    UpdatedDate               DATETIME2       NULL,
    UpdatedBy                 INT             NULL,

    CONSTRAINT FK_PriceTableItems_PriceTables FOREIGN KEY (PriceTableId) REFERENCES PriceTables(PriceTableId),
    CONSTRAINT FK_PriceTableItems_Products FOREIGN KEY (ProductId) REFERENCES Products(ProductId),
    CONSTRAINT FK_PriceTableItems_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_PriceTableItems_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES Users(UserId),

    CONSTRAINT CK_PriceTableItems_Price CHECK (Price >= 0)
);
GO

CREATE UNIQUE INDEX UX_PriceTableItems_Table_Product ON PriceTableItems(PriceTableId, ProductId);
GO

-- =============================================
-- Seed: categoria e tabela de preço por defeito
-- =============================================
INSERT INTO Categories (Name) VALUES
('Materiais de Construção'),
('Mobiliário'),
('Serviços de Instalação'),
('Consultoria');
GO

INSERT INTO PriceTables (Name, IsDefault) VALUES
('Tabela Padrão', 1);
GO