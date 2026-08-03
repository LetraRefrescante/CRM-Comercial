-- =============================================
-- Script: 012_Seed_Permissions.sql
-- Descrição: Seed de Permissions e RolePermissions conforme a matriz de perfis do blueprint
-- =============================================

USE CRM;
GO

-- =============================================
-- Permissions por módulo
-- =============================================
INSERT INTO Permissions (Code, Module, Description) VALUES
('Utilizadores.Total',     'Utilizadores',   'Gerir utilizadores e perfis'),
('Utilizadores.Consulta',  'Utilizadores',   'Consultar utilizadores'),

('Clientes.Total',         'Clientes',       'Acesso total a clientes'),
('Clientes.Proprios',      'Clientes',       'Acesso aos clientes próprios ou da equipa'),
('Clientes.Consulta',      'Clientes',       'Apenas consulta de clientes'),

('Leads.Total',            'Leads',          'Acesso total a leads'),
('Leads.Proprios',         'Leads',          'Acesso aos leads próprios ou da equipa'),
('Leads.Consulta',         'Leads',          'Apenas consulta de leads'),

('Oportunidades.Total',    'Oportunidades',  'Acesso total a oportunidades'),
('Oportunidades.Proprios', 'Oportunidades',  'Acesso às oportunidades próprias ou da equipa'),
('Oportunidades.Consulta', 'Oportunidades',  'Apenas consulta de oportunidades'),

('Propostas.Total',        'Propostas',      'Acesso total a propostas'),
('Propostas.Proprios',     'Propostas',      'Acesso às propostas próprias ou da equipa'),
('Propostas.Consulta',     'Propostas',      'Apenas consulta de propostas'),

('Vendas.Total',           'Vendas',         'Acesso total a vendas e pagamentos'),
('Vendas.Proprios',        'Vendas',         'Acesso às vendas próprias ou da equipa'),
('Vendas.Consulta',        'Vendas',         'Apenas consulta de vendas'),

('Relatorios.Total',       'Relatorios',     'Acesso total a relatórios'),
('Relatorios.Financeiros', 'Relatorios',     'Acesso apenas a relatórios financeiros'),
('Relatorios.Consulta',    'Relatorios',     'Apenas consulta de relatórios'),

('Configuracoes.Total',    'Configuracoes',  'Gerir parâmetros e listas auxiliares'),
('Configuracoes.Consulta', 'Configuracoes',  'Apenas consulta de configurações');
GO

-- =============================================
-- Atribuição de permissões por perfil (RolePermissions)
-- Assume-se que os RoleId seguem a ordem de inserção do script 001:
-- 1 = Administrador, 2 = Diretor, 3 = Comercial, 4 = Financeiro, 5 = Consulta
-- =============================================

-- Administrador: TOTAL em tudo
INSERT INTO RolePermissions (RoleId, PermissionId)
SELECT r.RoleId, p.PermissionId
FROM Roles r CROSS JOIN Permissions p
WHERE r.Name = 'Administrador';
GO

-- Diretor: Utilizadores CONSULTA, Clientes/Leads/Oportunidades/Propostas TOTAL, Vendas CONSULTA, Relatorios TOTAL, Configuracoes CONSULTA
INSERT INTO RolePermissions (RoleId, PermissionId)
SELECT r.RoleId, p.PermissionId
FROM Roles r CROSS JOIN Permissions p
WHERE r.Name = 'Diretor' AND p.Code IN (
    'Utilizadores.Consulta',
    'Clientes.Total', 'Leads.Total', 'Oportunidades.Total', 'Propostas.Total',
    'Vendas.Consulta',
    'Relatorios.Total',
    'Configuracoes.Consulta'
);
GO

-- Comercial: Clientes/Leads/Oportunidades/Propostas/Vendas PRÓPRIOS; sem Utilizadores, Relatorios ou Configuracoes
INSERT INTO RolePermissions (RoleId, PermissionId)
SELECT r.RoleId, p.PermissionId
FROM Roles r CROSS JOIN Permissions p
WHERE r.Name = 'Comercial' AND p.Code IN (
    'Clientes.Proprios', 'Leads.Proprios', 'Oportunidades.Proprios', 'Propostas.Proprios', 'Vendas.Proprios'
);
GO

-- Financeiro: Clientes/Leads/Oportunidades/Propostas CONSULTA, Vendas TOTAL, Relatorios FINANCEIROS
INSERT INTO RolePermissions (RoleId, PermissionId)
SELECT r.RoleId, p.PermissionId
FROM Roles r CROSS JOIN Permissions p
WHERE r.Name = 'Financeiro' AND p.Code IN (
    'Clientes.Consulta', 'Leads.Consulta', 'Oportunidades.Consulta', 'Propostas.Consulta',
    'Vendas.Total',
    'Relatorios.Financeiros'
);
GO

-- Consulta: CONSULTA em Clientes/Leads/Oportunidades/Propostas/Vendas/Relatorios; sem Utilizadores nem Configuracoes
INSERT INTO RolePermissions (RoleId, PermissionId)
SELECT r.RoleId, p.PermissionId
FROM Roles r CROSS JOIN Permissions p
WHERE r.Name = 'Consulta' AND p.Code IN (
    'Clientes.Consulta', 'Leads.Consulta', 'Oportunidades.Consulta', 'Propostas.Consulta',
    'Vendas.Consulta', 'Relatorios.Consulta'
);
GO