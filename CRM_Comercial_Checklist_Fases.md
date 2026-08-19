# CRM Comercial — Checklist de Páginas/Ficheiros por Fase

## Fase 0 — Setup Inicial do Projeto
- **Repositório Git + README inicial** — controlo de versões e documentação inicial.
- **CRM.sln** — solução Visual Studio principal.
- **Projetos:** CRM.Web, CRM.Business, CRM.Data, CRM.Models, CRM.Services, CRM.Tests — camadas da arquitetura (ver notas gerais).
- **Estrutura de pastas em CRM.Web:** Account, Dashboard, Clientes, Leads, Oportunidades, Catalogo, Vendas, Atividades, Relatorios, Administracao, Controls, MasterPages, Content, Scripts, App_Start.
- **Base de dados vazia + scripts SQL versionados** — persistência inicial.
- **Entidades:** Users, Roles, Permissions, AuditLogs — modelo de dados base para autenticação/auditoria.

**Notas da fase:** configurar referências entre projetos sem dependências circulares; ainda sem UI funcional, é só esqueleto.

---

## Fase 1 — Fundação (Autenticação + MasterPage + Utilizadores)
**Autenticação:**
- **Login.aspx** — autenticar utilizador (email/utilizador, password, lembrar utilizador, recuperar password).
- **RecuperarPassword.aspx** — pedir reposição de password (email + mensagem genérica de confirmação).
- **RedefinirPassword.aspx** — definir nova password via token.
- **AlterarPassword.aspx** — alteração autenticada (password atual, nova, confirmação).
- **AcessoNegado.aspx** — informar falta de permissão.

**Layout e componentes reutilizáveis:**
- **Site.Master** — menu lateral, topo, área de conteúdo e rodapé; menu visível conforme permissões.
- **SeletorCliente.ascx** — pesquisar e selecionar um cliente.
- **SeletorProduto.ascx** — adicionar produtos a propostas/vendas.
- **Anexos.ascx** — upload, listagem e download de ficheiros.
- **Histórico.ascx** — mostrar alterações e atividades relacionadas.
- **Paginação.ascx** — paginação comum das listagens.
- **Mensagens.ascx** — alertas, confirmações e feedback.
- **FiltroDatas.ascx** — selecionar período inicial e final.

**Utilizadores (necessário cedo, usado como "Comercial Responsável" nos outros módulos):**
- **UtilizadoresLista.aspx** — gerir utilizadores.
- **UtilizadorEditar.aspx** — criar, editar, bloquear e atribuir perfil.
- **PerfisPermissoes.aspx** — configurar permissões.

**Erros:**
- **Erro.aspx** — página de erro com identificador de ocorrência (ligada ao tratamento global no Global.asax).

**Notas da fase:**
- Password com hash forte + salt; nunca texto simples.
- Bloquear conta após 5 tentativas falhadas.
- Registar login sucesso/falhado, logout e reposição de password.
- Sessão expira por inatividade (configurável no Web.config).
- Permissões validadas sempre no servidor.
- Implementar matriz de perfis: Administrador, Diretor, Comercial, Financeiro, Consulta.

---

## Fase 2 — CRM Base (Clientes, Contactos, Leads)
**Clientes:**
- **ClienteLista.aspx** — listar, pesquisar, filtrar, ordenar, exportar e abrir clientes.
- **ClienteEditar.aspx** — criar e editar dados gerais do cliente.
- **ClienteDetalhe.aspx** — resumo, contactos, oportunidades, vendas, atividades e documentos.
- **ClientesImportar.aspx** — importar clientes.

**Contactos:**
- **ContactosLista.aspx** — listagem global de contactos.
- **ContactoEditar.aspx** — criar/editar contacto associado a cliente.
- **ContactoDetalhe.aspx** — dados e histórico de interações.

**Leads:**
- **LeadsLista.aspx** — listar e gerir potenciais clientes.
- **LeadEditar.aspx** — criar e qualificar lead.
- **LeadDetalhe.aspx** — informação, atividades e histórico.
- **LeadConverter.aspx** — converter lead em cliente/contacto/oportunidade.

**Notas da fase:**
- NIF único para clientes ativos; eliminação sempre lógica (soft delete).
- Só um contacto "principal" por cliente (desmarca automaticamente o anterior).
- Conversão de lead pode criar cliente+contacto+oportunidade na mesma operação; lead fica bloqueado depois de convertido.
- Evitar duplicados por email/telefone/NIF, com avisos.

---

## Fase 3 — Comercial (Oportunidades, Produtos, Propostas)
**Oportunidades / Pipeline:**
- **OportunidadesLista.aspx** — listagem tabular com filtros.
- **Pipeline.aspx** — vista Kanban por fase.
- **OportunidadeEditar.aspx** — criar/editar oportunidade.
- **OportunidadeDetalhe.aspx** — resumo, produtos, atividades, propostas e histórico.
- **OportunidadeFechar.aspx** — fechar como ganha ou perdida.

**Catálogo de Produtos:**
- **ProdutosLista.aspx** — listar produtos e serviços.
- **ProdutoEditar.aspx** — criar/editar item de catálogo.
- **CategoriasLista.aspx** — gerir categorias.
- **TabelasPreco.aspx** — gerir preços por tabela comercial.

**Propostas:**
- **PropostasLista.aspx** — listar, filtrar e acompanhar propostas.
- **PropostaEditar.aspx** — criar cabeçalho, condições e linhas.
- **PropostaDetalhe.aspx** — documentos, versões, atividades e estado.
- **PropostaPDF.aspx** — gerar visualização/PDF.
- **PropostaEnviar.aspx** — enviar por email e registar envio.

**Notas da fase:**
- Valor ponderado = valor estimado × probabilidade.
- Mover no Kanban grava histórico (fase anterior, nova fase, utilizador, data).
- Código de produto único; alterar preço não afeta propostas/vendas já gravadas; itens usados não podem ser eliminados fisicamente.
- Proposta: calcular subtotal, desconto, base tributável, IVA e total; alterações após envio criam nova versão; deteção diária de propostas expiradas.

---

## Fase 4 — Venda
- **VendasLista.aspx** — listar vendas e respetivo estado.
- **VendaEditar.aspx** — criar venda manual ou a partir de proposta.
- **VendaDetalhe.aspx** — linhas, pagamentos e documentos.
- **Pagamentos.aspx** — registar recebimentos e estado financeiro.

**Notas da fase:**
- Venda criada de proposta copia valores e mantém referência.
- Cancelamento exige motivo, não elimina histórico.
- Pagamentos podem ser parciais; estado financeiro deriva dos pagamentos registados.
- Relatório de comissões considera apenas estados configurados.

---

## Fase 5 — Produtividade (Agenda, Tarefas, Documentos, Notificações)
**Agenda / Atividades / Tarefas:**
- **Agenda.aspx** — vista diária, semanal e mensal.
- **AtividadesLista.aspx** — listar chamadas, emails, reuniões, visitas e notas.
- **AtividadeEditar.aspx** — registar atividade.
- **TarefasLista.aspx** — listar e filtrar tarefas.
- **TarefaEditar.aspx** — criar, atribuir e concluir tarefa.

**Documentos:**
- **DocumentosLista.aspx** — pesquisa global de documentos.
- **DocumentoEditar.aspx** — carregar ficheiro e metadados.
- **DocumentoDownload.aspx** — download controlado com validação de autorização.

**Notificações e Email:**
- **Notificacoes.aspx** — centro de notificações internas.
- **TemplatesEmail.aspx** — gerir modelos de email.
- **EmailCompor.aspx** — enviar email relacionado com um registo.
- **EmailHistorico.aspx** — consultar emails enviados pelo CRM.

**Notas da fase:**
- Atividades concluídas não podem ser eliminadas por utilizadores comuns.
- Ficheiros guardados fora de pastas públicas; validar extensão, MIME, tamanho e nome seguro.
- Configuração SMTP no Web.config ou storage seguro; falhas de envio registadas sem expor credenciais.
- Email enviado gera atividade automaticamente.

---

## Fase 6 — Gestão (Dashboard, Relatórios, Configurações)
**Dashboard:**
- **Dashboard.aspx** — página inicial após login, adaptada ao perfil do utilizador (indicadores + gráficos).

**Relatórios:**
- **Relatorios.aspx** — catálogo de relatórios.
- **RelatorioVendas.aspx** — análise de vendas.
- **RelatorioPipeline.aspx** — análise de oportunidades.
- **RelatorioLeads.aspx** — conversão e origem de leads.
- **RelatorioAtividades.aspx** — produtividade comercial.
- **RelatorioClientes.aspx** — carteira e segmentação de clientes.
- **RelatorioComissoes.aspx** — comissões por comercial.

**Administração / Configurações restantes:**
- **Parametros.aspx** — configurações gerais (empresa, moeda, fuso horário, dias de alerta).
- **ListasAuxiliares.aspx** — estados, origens, categorias, motivos e taxas.
- **Auditoria.aspx** — consultar logs funcionais e de segurança.

**Notas da fase:**
- Dashboard: valores respeitam permissões/âmbito do utilizador; filtros de período atualizam todos os widgets; carregamento inicial não pode fazer consulta por registo.
- Gráficos com Chart.js: vendas por mês, pipeline por fase/valor, origem dos leads, top comerciais, últimas atividades, próximas reuniões, oportunidades sem atividade recente.
- Exportações devem reproduzir exatamente os dados apresentados (Excel/CSV/PDF), com data, filtros e utilizador que gerou.
- Listas auxiliares nunca são eliminadas fisicamente, só inativadas.

---

## Fase 7 — Fecho (Testes, Segurança, Instalação, Documentação)
Não é UI nova, mas entregáveis obrigatórios:
- **CRM.Tests** — testes unitários/integração (autenticação, permissões, CRUD, validação, fluxos, cálculos, ficheiros, exportação, concorrência, performance).
- **Scripts SQL finais** de criação/atualização da BD.
- **Ficheiro de configuração de exemplo** sem credenciais reais.
- **Dados de demonstração** para todos os módulos.
- **Manual de instalação em IIS.**
- **Manual técnico** (arquitetura e dependências).
- **Manual curto do utilizador.**
- **Diagrama da base de dados.**
- **Plano e evidências de testes.**
- **Lista de utilizadores/perfis de demonstração.**
- **Changelog das versões.**
- **Lista de limitações conhecidas e trabalho futuro.**

**Notas da fase:** rever segurança (HTTPS, inputs validados, output encoding, queries parametrizadas), performance (paginação, sem N+1, ViewState desativado onde não necessário) e o checklist final antes de fechar o projeto.

---

## Notas Importantes Gerais (transversais a todas as fases)
- **Camadas obrigatórias:** CRM.Web (UI) → CRM.Business (regras de negócio, validações, autorização) → CRM.Data (EF context, repositories, transações) → CRM.Models (entidades, DTOs, enums, filtros) → CRM.Services (email, ficheiros, PDF, import/export) → CRM.Tests.
- **Nunca fazer:** SQL direto nas páginas ASPX, lógica de negócio relevante no code-behind ou JavaScript, passwords em texto simples, eliminação física de dados comerciais.
- **Tecnologias proibidas:** ASP.NET MVC/Core MVC, Razor Pages, Blazor, React/Angular/Vue como aplicação principal.
- **Convenções de nomes:** Listagem → `EntidadeLista.aspx`; Edição → `EntidadeEditar.aspx`; Detalhe → `EntidadeDetalhe.aspx`; UserControl → `NomeControlo.ascx`; Service → `EntidadeService`; Repository → `EntidadeRepository`; PK → `EntidadeId`.
- **Campos técnicos obrigatórios em todas as tabelas:** CreatedDate/By, UpdatedDate/By, IsDeleted, DeletedDate/By, RowVersion (soft delete + auditoria + concorrência otimista).
- **Ordem de execução:** não avançar em vários módulos em paralelo — concluir uma fase, demonstrar, corrigir os pontos identificados e só depois iniciar a fase seguinte.
