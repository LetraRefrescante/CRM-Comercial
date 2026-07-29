USE CRM;
GO

INSERT INTO Users (Name, Email, PasswordHash, PasswordSalt, RoleId, Status)
VALUES (
    'Administrador',
    'admin@empresa.pt',
    '5beipf0eBACX/hNcq+DTasTrw6TPEbTQsjHAwBv3XWc=',
    'M7SxLcfOju2ekUK1xJWhfA==',
    (SELECT RoleId FROM Roles WHERE Name = 'Administrador'),
    'Ativo'
);
GO