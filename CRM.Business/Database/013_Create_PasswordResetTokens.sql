-- ============================================================
-- Cria a tabela de tokens para RecuperarPassword/RedefinirPassword.
-- ============================================================

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'PasswordResetTokens')
BEGIN
    CREATE TABLE dbo.PasswordResetTokens
    (
        Id            INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        UserId        INT               NOT NULL,
        Token         NVARCHAR(200)     NOT NULL,
        DataCriacao   DATETIME          NOT NULL CONSTRAINT DF_PasswordResetTokens_DataCriacao DEFAULT (GETUTCDATE()),
        DataExpiracao DATETIME          NOT NULL,
        Utilizado     BIT               NOT NULL CONSTRAINT DF_PasswordResetTokens_Utilizado DEFAULT (0),

        CONSTRAINT FK_PasswordResetTokens_Users FOREIGN KEY (UserId)
            REFERENCES dbo.Users (UserId)
    );

    CREATE UNIQUE INDEX UX_PasswordResetTokens_Token ON dbo.PasswordResetTokens (Token);
    CREATE INDEX IX_PasswordResetTokens_UserId ON dbo.PasswordResetTokens (UserId);
END
GO