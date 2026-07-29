-- =============================================
-- Script: 008_Sales_SaleLines_Payments.sql
-- Descrição: Vendas, linhas de venda e pagamentos
-- =============================================

USE CRM;
GO

-- =============================================
-- Tabela: Sales
-- =============================================
CREATE TABLE Sales (
    SaleId                 INT             IDENTITY(1,1) PRIMARY KEY,
    SaleNumber               NVARCHAR(30)    NOT NULL,   -- Sequencial
    ClientId                   INT             NOT NULL,
    ProposalId                   INT             NULL,       -- Origem: Proposta ou manual
    SaleDate                       DATE            NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    OwnerId                          INT             NOT NULL,   -- Comercial responsável
    Status                            NVARCHAR(20)    NOT NULL DEFAULT 'Pendente', -- Pendente, Confirmada, Parcial, Concluída, Cancelada
    Origin                              NVARCHAR(20)    NOT NULL,   -- Proposta ou Manual
    PaymentMethod                        NVARCHAR(30)    NULL,       -- Transferência, Referência, Cartão, Outro
    DueDate                                DATE            NULL,
    CommissionValue                          DECIMAL(18,2)   NULL,       -- valor ou percentagem calculada
    CancellationReason                        NVARCHAR(500)   NULL,       -- obrigatório se Cancelada

    -- Totais gravados no momento (copiados da proposta se aplicável)
    SubTotal                                    DECIMAL(18,2)   NOT NULL DEFAULT 0,
    TaxTotal                                      DECIMAL(18,2)   NOT NULL DEFAULT 0,
    Total                                          DECIMAL(18,2)   NOT NULL DEFAULT 0,

    CreatedDate                                    DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy                                      INT             NULL,
    UpdatedDate                                     DATETIME2       NULL,
    UpdatedBy                                       INT             NULL,
    IsDeleted                                       BIT             NOT NULL DEFAULT 0,
    DeletedDate                                     DATETIME2       NULL,
    DeletedBy                                       INT             NULL,
    RowVersion                                      ROWVERSION,

    CONSTRAINT FK_Sales_Clients FOREIGN KEY (ClientId) REFERENCES Clients(ClientId),
    CONSTRAINT FK_Sales_Proposals FOREIGN KEY (ProposalId) REFERENCES Proposals(ProposalId),
    CONSTRAINT FK_Sales_Owner FOREIGN KEY (OwnerId) REFERENCES Users(UserId),
    CONSTRAINT FK_Sales_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_Sales_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_Sales_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES Users(UserId),

    CONSTRAINT CK_Sales_Status CHECK (Status IN ('Pendente', 'Confirmada', 'Parcial', 'Concluída', 'Cancelada')),
    CONSTRAINT CK_Sales_Origin CHECK (Origin IN ('Proposta', 'Manual')),
    CONSTRAINT CK_Sales_PaymentMethod CHECK (PaymentMethod IS NULL OR PaymentMethod IN ('Transferência', 'Referência', 'Cartão', 'Outro'))
);
GO

CREATE UNIQUE INDEX UX_Sales_SaleNumber ON Sales(SaleNumber);
CREATE INDEX IX_Sales_ClientId ON Sales(ClientId);
CREATE INDEX IX_Sales_OwnerId ON Sales(OwnerId);
CREATE INDEX IX_Sales_Status ON Sales(Status);
CREATE INDEX IX_Sales_SaleDate ON Sales(SaleDate);
GO

-- =============================================
-- Tabela: SaleLines
-- =============================================
CREATE TABLE SaleLines (
    SaleLineId             INT             IDENTITY(1,1) PRIMARY KEY,
    SaleId                   INT             NOT NULL,
    ProductId                 INT             NOT NULL,
    LineOrder                   INT             NOT NULL DEFAULT 1,
    Description                   NVARCHAR(500)   NULL,
    Quantity                       DECIMAL(18,3)   NOT NULL DEFAULT 1,
    UnitPrice                       DECIMAL(18,2)   NOT NULL,
    DiscountPercent                   DECIMAL(5,2)    NOT NULL DEFAULT 0,
    TaxRateId                          INT             NOT NULL,
    LineTotal                            DECIMAL(18,2)   NOT NULL,

    CONSTRAINT FK_SaleLines_Sales FOREIGN KEY (SaleId) REFERENCES Sales(SaleId),
    CONSTRAINT FK_SaleLines_Products FOREIGN KEY (ProductId) REFERENCES Products(ProductId),
    CONSTRAINT FK_SaleLines_TaxRates FOREIGN KEY (TaxRateId) REFERENCES TaxRates(TaxRateId),

    CONSTRAINT CK_SaleLines_Quantity CHECK (Quantity > 0),
    CONSTRAINT CK_SaleLines_UnitPrice CHECK (UnitPrice >= 0),
    CONSTRAINT CK_SaleLines_Discount CHECK (DiscountPercent >= 0 AND DiscountPercent <= 100)
);
GO

CREATE INDEX IX_SaleLines_SaleId ON SaleLines(SaleId);
GO

-- =============================================
-- Tabela: Payments
-- =============================================
CREATE TABLE Payments (
    PaymentId               INT             IDENTITY(1,1) PRIMARY KEY,
    SaleId                    INT             NOT NULL,
    Amount                      DECIMAL(18,2)   NOT NULL,
    PaymentDate                  DATE            NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    PaymentMethod                  NVARCHAR(30)    NULL,
    Reference                        NVARCHAR(100)   NULL,   -- nº transação/referência bancária
    Notes                              NVARCHAR(500)   NULL,

    CreatedDate                        DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy                          INT             NULL,
    IsDeleted                          BIT             NOT NULL DEFAULT 0,
    DeletedDate                        DATETIME2       NULL,
    DeletedBy                          INT             NULL,

    CONSTRAINT FK_Payments_Sales FOREIGN KEY (SaleId) REFERENCES Sales(SaleId),
    CONSTRAINT FK_Payments_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_Payments_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES Users(UserId),

    CONSTRAINT CK_Payments_Amount CHECK (Amount > 0)
);
GO

CREATE INDEX IX_Payments_SaleId ON Payments(SaleId);
CREATE INDEX IX_Payments_PaymentDate ON Payments(PaymentDate);
GO