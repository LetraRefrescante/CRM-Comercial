<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="CRM.Web.Paginas.Dashboard.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item active">Dashboard</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2 style="font-family:'Sora',sans-serif;">Bem-vindo, <asp:Literal ID="litNomeUtilizador" runat="server" /></h2>
    <p class="text-muted mb-4">Aqui terás uma visão geral da atividade comercial assim que os módulos estiverem implementados.</p>

    <div class="row g-3">
        <div class="col-md-3">
            <div class="card p-3 kpi-card" style="border-left: 4px solid #1F7A5C;">
                <div class="d-flex justify-content-between align-items-start">
                    <div>
                        <div class="text-muted small">Clientes ativos</div>
                        <div class="mono" style="font-size:1.8rem;font-weight:700;color:var(--ink);">—</div>
                    </div>
                    <i class="fas fa-users" style="color:#1F7A5C; font-size:1.5rem; opacity:0.6;"></i>
                </div>
            </div>
        </div>
        <div class="col-md-3">
            <div class="card p-3 kpi-card" style="border-left: 4px solid #12213B;">
                <div class="d-flex justify-content-between align-items-start">
                    <div>
                        <div class="text-muted small">Leads novos</div>
                        <div class="mono" style="font-size:1.8rem;font-weight:700;color:var(--ink);">—</div>
                    </div>
                    <i class="fas fa-bullseye" style="color:#12213B; font-size:1.5rem; opacity:0.6;"></i>
                </div>
            </div>
        </div>
        <div class="col-md-3">
            <div class="card p-3 kpi-card" style="border-left: 4px solid #E0A83E;">
                <div class="d-flex justify-content-between align-items-start">
                    <div>
                        <div class="text-muted small">Oportunidades abertas</div>
                        <div class="mono" style="font-size:1.8rem;font-weight:700;color:var(--ink);">—</div>
                    </div>
                    <i class="fas fa-handshake" style="color:#E0A83E; font-size:1.5rem; opacity:0.6;"></i>
                </div>
            </div>
        </div>
        <div class="col-md-3">
            <div class="card p-3 kpi-card" style="border-left: 4px solid #1F7A5C;">
                <div class="d-flex justify-content-between align-items-start">
                    <div>
                        <div class="text-muted small">Vendas do mês</div>
                        <div class="mono" style="font-size:1.8rem;font-weight:700;color:var(--ink);">—</div>
                    </div>
                    <i class="fas fa-euro-sign" style="color:#1F7A5C; font-size:1.5rem; opacity:0.6;"></i>
                </div>
            </div>
        </div>
    </div>

    <div class="alert alert-info mt-4" style="background:var(--accent-soft); border-color:var(--accent); color:var(--ink);">
        O dashboard completo (gráficos, listas, indicadores em tempo real) será implementado na fase "Gestão", segundo o plano de desenvolvimento.
    </div>
</asp:Content>