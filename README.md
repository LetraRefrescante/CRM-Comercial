# CRM Comercial — Estágio 2026

*Sistema de CRM interno · ASP.NET WebForms · .NET Framework 4.8 · SQL Server*

## 📌 Objetivo

Centralizar a gestão comercial da empresa — clientes, contactos, leads, oportunidades, propostas, vendas, agenda, tarefas, documentos e relatórios — num único sistema, cobrindo o fluxo comercial completo:

```
Lead → Qualificação → Conversão → Cliente/Contacto → Oportunidade → Proposta → Aceitação → Venda → Pagamento
```

## 🚦 Estado Atual do Projeto

| Fase | Módulos | Estado |
|---|---|---|
| 0 | Setup inicial (solução, projetos, BD vazia) | ✅ Concluída |
| 1 | Fundação — autenticação, sessão, perfis/permissões, MasterPage, utilizadores | ✅ Concluída |
| 2 | CRM Base — clientes, contactos, leads | ✅ Concluída |
| 3 | Comercial — oportunidades, pipeline, catálogo de produtos, propostas | ✅ Concluída |
| 4 | Venda — vendas, pagamentos, comissões | ✅ Concluída |
| 5 | Produtividade — agenda, tarefas, atividades, documentos, notificações | ✅ Concluída |
| 6 | Gestão — dashboard, 7 relatórios, parâmetros, listas auxiliares, auditoria | ✅ Concluída* |
| 7 | Fecho — testes, segurança, instalação, documentação | 🔄 Em curso |

\* Falta ajustar `Settings` / `SettingsRepository` / `SettingsService` / `Parametros.aspx.cs` ao schema real da tabela (`SettingId`, `AlertDaysLeads/Opportunities/Proposals`, `MaxFailedLoginAttempts`, `AccountLockoutMinutes`, `SessionTimeoutMinutes`, `MonthlySalesTarget`).

> Estado compilado a partir do histórico de desenvolvimento fornecido junto com a blueprint — convém confirmar/atualizar à medida que o trabalho avança.

**Próximos passos (Fase 7):**
- Consolidar os cerca de 16 scripts SQL incrementais (`001_...sql` a `016_...sql`) num único `Setup_Completo.sql` ordenado.
- Criar `Changelog.md`.
- Criar `Limitacoes.md` (limitações conhecidas e trabalho futuro).
- Restante checklist de fecho: testes unitários/integração, revisão de segurança e performance, manual de instalação em IIS, manual técnico, manual do utilizador, diagrama da base de dados, dados de demonstração, lista de utilizadores/perfis de demo, checklist final do projeto.

## 🧱 Stack Tecnológico

**Obrigatório**
- ASP.NET WebForms sobre .NET Framework 4.8
- C# (code-behind e todas as camadas da solução)
- SQL Server + Entity Framework 6 (`CrmDbContext`)
- Bootstrap 5, HTML5, CSS3, JavaScript, jQuery
- Chart.js (gráficos do dashboard e relatórios)
- IIS (publicação)

**Não permitido**
- ASP.NET MVC, ASP.NET Core MVC, Razor Pages ou Blazor
- React, Angular, Vue ou outra SPA como aplicação principal
- SQL escrito diretamente nas páginas ASPX
- Lógica de negócio relevante em JavaScript ou no code-behind
- Passwords guardadas em texto simples
- Eliminação física de dados comerciais sem aprovação

## 🏗️ Arquitetura em Camadas

| Camada | Responsabilidade |
|---|---|
| `CRM.Web` | Páginas ASPX, MasterPages, UserControls, recursos front-end |
| `CRM.Business` | Regras de negócio, validações, autorização |
| `CRM.Data` | Contexto Entity Framework (`CrmDbContext`), repositories, transações |
| `CRM.Models` | Entidades, DTOs, enums, modelos de filtros |
| `CRM.Services` | Email, ficheiros, PDF, importação/exportação |
| `CRM.Tests` | Testes unitários e de integração |

Fluxo de dependências: `CRM.Web → CRM.Business → CRM.Data → CRM.Models`, com `CRM.Services` a apoiar operações transversais. Sem dependências circulares entre projetos.

## 📁 Estrutura da Solução

```
CRM.sln
├── CRM.Web
│   ├── Account
│   ├── Dashboard
│   ├── Clientes
│   ├── Leads
│   ├── Oportunidades
│   ├── Catalogo
│   ├── Vendas
│   ├── Atividades
│   ├── Relatorios
│   ├── Administracao
│   ├── Controls
│   ├── MasterPages
│   ├── Content
│   ├── Scripts
│   ├── App_Start
│   └── Global.asax / Web.config
├── CRM.Business
├── CRM.Data
├── CRM.Models
├── CRM.Services
└── CRM.Tests
```

## 🗄️ Modelo de Dados (visão geral)

Base de dados: **CRM** (SQL Server).

Entidades principais: `Users`, `Roles`, `Permissions`, `Clients`, `Contacts`, `Leads`, `Opportunities`, `OpportunityStages`, `Products`, `Proposals`, `ProposalLines`, `Sales`, `SaleLines`, `Payments`, `Activities`, `Tasks`, `Documents`, `Notifications`, `AuditLogs`, `Settings`.

Todas as tabelas incluem os campos técnicos obrigatórios:

| Campo | Tipo | Regra |
|---|---|---|
| `CreatedDate` / `CreatedBy` | `datetime2` / `int` | Preenchido na criação |
| `UpdatedDate` / `UpdatedBy` | `datetime2?` / `int?` | Atualizado em alterações |
| `IsDeleted` | `bit` | Soft delete |
| `DeletedDate` / `DeletedBy` | `datetime2?` / `int?` | Eliminação lógica |
| `RowVersion` | `rowversion` | Concorrência otimista |

## 👤 Perfis e Permissões

| Perfil | Utilizadores | Clientes | Leads | Oportunidades | Propostas | Vendas | Relatórios | Configurações |
|---|---|---|---|---|---|---|---|---|
| Administrador | Total | Total | Total | Total | Total | Total | Total | Total |
| Diretor | Consulta | Total | Total | Total | Total | Consulta | Total | Consulta |
| Comercial | Não | Próprios | Próprios | Próprios | Próprios | Próprios | Próprios | Não |
| Financeiro | Não | Consulta | Consulta | Consulta | Consulta | Total | Financeiros | Não |
| Consulta | Não | Consulta | Consulta | Consulta | Consulta | Consulta | Consulta | Não |

> "Próprios" = registos atribuídos ao utilizador ou à sua equipa, conforme configuração.

## 🔐 Segurança

- HTTPS em produção
- Passwords com hash forte + salt (nunca texto simples)
- Bloqueio de conta após 5 tentativas falhadas
- Sessão expira por inatividade (configurável no `Web.config`)
- Permissões sempre validadas no servidor
- Proteção contra XSS, CSRF, SQL Injection, upload malicioso e acesso direto a ficheiros
- Consultas parametrizadas / Entity Framework — nunca SQL direto nas páginas
- Sem segredos no código-fonte

## 📐 Convenções de Nomenclatura

| Elemento | Convenção | Exemplo |
|---|---|---|
| Listagem | `EntidadeLista.aspx` | `ClienteLista.aspx` |
| Edição | `EntidadeEditar.aspx` | `ClienteEditar.aspx` |
| Detalhe | `EntidadeDetalhe.aspx` | `ClienteDetalhe.aspx` |
| UserControl | `NomeControlo.ascx` | `SeletorCliente.ascx` |
| Service | `EntidadeService` | `ClienteService` |
| Repository | `EntidadeRepository` | `ClienteRepository` |
| Chave Primária | `EntidadeId` | `ClientId` |

## ⚙️ Como Correr o Projeto

1. Abrir `CRM.sln` no Visual Studio (com suporte para .NET Framework 4.8).
2. Criar a base de dados `CRM` no SQL Server e correr os scripts SQL numerados pela ordem correta.
3. Configurar a connection string e as definições de SMTP no `Web.config` de `CRM.Web`.
4. Compilar a solução e publicar em IIS (ou correr localmente via IIS Express).
5. Autenticar com um utilizador de demonstração.

> Manual de instalação detalhado, script SQL consolidado e ficheiro de configuração de exemplo (sem credenciais reais) são entregáveis previstos para a Fase 7.

## 📄 Documentação do Projeto

- [`CRM Comercial - Estágio 2026.pdf`](./CRM%20Comercial%20-%20Estágio%202026.pdf) — blueprint funcional e técnica completa (campos, regras de negócio e critérios de aceitação por módulo)
- [`CRM_Comercial_Checklist_Fases.md`](./CRM_Comercial_Checklist_Fases.md) — checklist de páginas/ficheiros por fase

## 🥇 Regras de Ouro

- Não avançar módulos em paralelo — concluir, demonstrar e corrigir cada fase antes de iniciar a seguinte.
- Sem SQL direto nas páginas ASPX.
- Sem lógica de negócio relevante no code-behind ou em JavaScript.
- Sem eliminação física de dados comerciais.
