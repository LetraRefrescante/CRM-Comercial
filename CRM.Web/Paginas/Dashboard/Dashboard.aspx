<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="CRM.Web.Paginas.Dashboard.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item active">Dashboard</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<style>
    .crm-kpi-card { background:#fff; border:1px solid #eee; border-radius:8px; padding:16px; height:100%; }
    .crm-kpi-valor { font-size:1.6rem; font-weight:700; font-family:'Sora',sans-serif; }
    .crm-kpi-label { color:#777; font-size:.8rem; text-transform:uppercase; }
    .crm-kpi-card.crm-kpi-alerta .crm-kpi-valor { color:#c0392b; }
    .crm-chart-card { background:#fff; border:1px solid #eee; border-radius:8px; padding:16px; height:100%; }
    .crm-dashboard-list-item { display:flex; justify-content:space-between; border-bottom:1px solid #f0f0f0; padding:8px 0; }
    .crm-dashboard-list-item:last-child { border-bottom:none; }
</style>

    <h2 class="mb-3">Dashboard</h2>

    <div class="row g-3 mb-4">
        <div class="col-md-3 col-6">
            <div class="crm-kpi-card">
                <div class="crm-kpi-label">Clientes Ativos</div>
                <div class="crm-kpi-valor"><asp:Literal ID="litClientesAtivos" runat="server" /></div>
                <div class="text-muted small"><asp:Literal ID="litNovosClientes" runat="server" /> novos este mês</div>
            </div>
        </div>
        <div class="col-md-3 col-6">
            <div class="crm-kpi-card">
                <div class="crm-kpi-label">Leads</div>
                <div class="crm-kpi-valor"><asp:Literal ID="litLeadsNovos" runat="server" /></div>
                <div class="text-muted small">
                    <asp:Literal ID="litLeadsEmContacto" runat="server" /> em contacto ·
                    <asp:Literal ID="litLeadsQualificados" runat="server" /> qualificados
                </div>
            </div>
        </div>
        <div class="col-md-3 col-6">
            <div class="crm-kpi-card">
                <div class="crm-kpi-label">Oportunidades Abertas</div>
                <div class="crm-kpi-valor"><asp:Literal ID="litOportunidadesAbertas" runat="server" /></div>
                <div class="text-muted small">Valor ponderado: <asp:Literal ID="litValorPonderado" runat="server" /></div>
            </div>
        </div>
        <div class="col-md-3 col-6">
            <div class="crm-kpi-card">
                <div class="crm-kpi-label">Vendas (Mês / Ano)</div>
                <div class="crm-kpi-valor"><asp:Literal ID="litVendasMes" runat="server" /></div>
                <div class="text-muted small">Ano: <asp:Literal ID="litVendasAno" runat="server" /></div>
            </div>
        </div>

        <div class="col-md-4 col-6">
            <a href="~/Atividades/TarefasLista.aspx" class="text-decoration-none">
                <div class="crm-kpi-card crm-kpi-alerta">
                    <div class="crm-kpi-label">Tarefas Vencidas</div>
                    <div class="crm-kpi-valor"><asp:Literal ID="litTarefasVencidas" runat="server" /></div>
                </div>
            </a>
        </div>
        <div class="col-md-4 col-6">
            <a href="~/Atividades/TarefasLista.aspx" class="text-decoration-none">
                <div class="crm-kpi-card">
                    <div class="crm-kpi-label">Tarefas para Hoje</div>
                    <div class="crm-kpi-valor"><asp:Literal ID="litTarefasHoje" runat="server" /></div>
                </div>
            </a>
        </div>
        <div class="col-md-4 col-6">
            <a href="~/Catalogo/PropostasLista.aspx" class="text-decoration-none">
                <div class="crm-kpi-card crm-kpi-alerta">
                    <div class="crm-kpi-label">Propostas a Expirar</div>
                    <div class="crm-kpi-valor"><asp:Literal ID="litPropostasAExpirar" runat="server" /></div>
                </div>
            </a>
        </div>
    </div>

    <div class="row g-3 mb-4">
        <div class="col-md-7">
            <div class="crm-chart-card">
                <h5 class="mb-3">Vendas por Mês</h5>
                <canvas id="chartVendas" height="110"></canvas>
            </div>
        </div>
        <div class="col-md-5">
            <div class="crm-chart-card">
                <h5 class="mb-3">Pipeline por Fase</h5>
                <canvas id="chartPipeline" height="110"></canvas>
            </div>
        </div>
    </div>

    <div class="row g-3 mb-4">
        <div class="col-md-5">
            <div class="crm-chart-card">
                <h5 class="mb-3">Origem dos Leads</h5>
                <canvas id="chartOrigemLeads" height="110"></canvas>
            </div>
        </div>
        <asp:PlaceHolder ID="phTopComerciais" runat="server">
            <div class="col-md-7">
                <div class="crm-chart-card">
                    <h5 class="mb-3">Top Comerciais (Mês)</h5>
                    <asp:Repeater ID="rptTopComerciais" runat="server">
                        <ItemTemplate>
                            <div class="crm-dashboard-list-item">
                                <span class="fw-semibold"><%# Container.ItemIndex + 1 %>. <%# Eval("Comercial") %></span>
                                <span class="fw-semibold"><%# Eval("TotalVendido", "{0:C}") %></span>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:PlaceHolder ID="phSemTopComerciais" runat="server" Visible="false">
                        <p class="text-muted small mb-0">Sem vendas este mês.</p>
                    </asp:PlaceHolder>
                </div>
            </div>
        </asp:PlaceHolder>
    </div>

    <div class="row g-3">
        <div class="col-md-4">
            <div class="crm-chart-card">
                <h5 class="mb-3">Últimas Atividades</h5>
                <asp:Repeater ID="rptUltimasAtividades" runat="server">
                    <ItemTemplate>
                        <div class="crm-dashboard-list-item">
                            <div>
                                <div class="fw-semibold"><%# Eval("Subject") %></div>
                                <div class="text-muted small"><%# Eval("Type") %> · <%# Eval("AssignedTo.Name") %></div>
                            </div>
                            <div class="text-muted small"><%# Eval("StartDateTime", "{0:dd/MM HH:mm}") %></div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:PlaceHolder ID="phSemAtividades" runat="server" Visible="false">
                    <p class="text-muted small mb-0">Sem atividades recentes.</p>
                </asp:PlaceHolder>
            </div>
        </div>

        <div class="col-md-4">
            <div class="crm-chart-card">
                <h5 class="mb-3">Próximas Reuniões</h5>
                <asp:Repeater ID="rptProximasReunioes" runat="server">
                    <ItemTemplate>
                        <div class="crm-dashboard-list-item">
                            <div>
                                <div class="fw-semibold"><%# Eval("Subject") %></div>
                                <div class="text-muted small"><%# GetRelacionado(Container.DataItem) %></div>
                            </div>
                            <div class="text-muted small"><%# Eval("StartDateTime", "{0:dd/MM HH:mm}") %></div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:PlaceHolder ID="phSemReunioes" runat="server" Visible="false">
                    <p class="text-muted small mb-0">Sem reuniões agendadas.</p>
                </asp:PlaceHolder>
            </div>
        </div>

        <div class="col-md-4">
            <div class="crm-chart-card">
                <h5 class="mb-3">Oportunidades sem Atividade Recente</h5>
                <asp:Repeater ID="rptOportunidadesSemAtividade" runat="server">
                    <ItemTemplate>
                        <div class="crm-dashboard-list-item">
                            <div>
                                <div class="fw-semibold"><%# Eval("Title") %></div>
                                <div class="text-muted small"><%# Eval("Client.TradeName") %> · <%# Eval("Stage.Name") %></div>
                            </div>
                            <div class="text-muted small fw-semibold"><%# Eval("EstimatedValue", "{0:C}") %></div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:PlaceHolder ID="phSemOportunidadesPendentes" runat="server" Visible="false">
                    <p class="text-muted small mb-0">Sem oportunidades nesta situação.</p>
                </asp:PlaceHolder>
            </div>
        </div>
    </div>

    <asp:Literal ID="litScriptGraficos" runat="server" />

</asp:Content>