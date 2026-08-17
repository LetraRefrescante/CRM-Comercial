-- =============================================
-- Script: 014_Sales_TechnicalFields_Fix.sql
-- Descrição: Acrescenta os campos técnicos obrigatórios que faltavam em
--            SaleLines e Payments (ver checklist "Campos Técnicos Obrigatórios",
--            transversal a todas as tabelas).
--
--            SaleLines: não tinha NENHUM destes campos.
--            Payments: tinha CreatedDate/CreatedBy/IsDeleted/DeletedDate/DeletedBy,
--                      mas faltava UpdatedDate/UpdatedBy/RowVersion.
--
-- =============================================

USE CRM;
GO

-- =============================================
-- SaleLines
-- =============================================
ALTER TABLE SaleLines ADD
    CreatedDate     DATETIME2   NOT NULL CONSTRAINT DF_SaleLines_CreatedDate DEFAULT SYSUTCDATETIME(),
    CreatedBy       INT         NULL,
    UpdatedDate     DATETIME2   NULL,
    UpdatedBy       INT         NULL,
    IsDeleted       BIT         NOT NULL CONSTRAINT DF_SaleLines_IsDeleted DEFAULT 0,
    DeletedDate     DATETIME2   NULL,
    DeletedBy       INT         NULL,
    RowVersion      ROWVERSION;
GO

ALTER TABLE SaleLines ADD CONSTRAINT FK_SaleLines_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId);
ALTER TABLE SaleLines ADD CONSTRAINT FK_SaleLines_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES Users(UserId);
ALTER TABLE SaleLines ADD CONSTRAINT FK_SaleLines_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES Users(UserId);
GO

-- =============================================
-- Payments
-- =============================================
ALTER TABLE Payments ADD
    UpdatedDate     DATETIME2   NULL,
    UpdatedBy       INT         NULL,
    RowVersion      ROWVERSION;
GO

ALTER TABLE Payments ADD CONSTRAINT FK_Payments_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES Users(UserId);
GO