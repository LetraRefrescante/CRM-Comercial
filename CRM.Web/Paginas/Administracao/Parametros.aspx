<%@ Page Title="Parâmetros" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="Parametros.aspx.cs" Inherits="CRM.Web.Paginas.Administracao.Parametros" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item">Administração</li>
    <li class="breadcrumb-item active">Parâmetros</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2 class="mb-3">Parâmetros</h2>

    <div class="crm-card mb-3">
        <h5 class="crm-card-title">Empresa</h5>
        <div class="row g-3">
            <div class="col-md-6">
                <label class="form-label">Nome da Empresa *</label>
                <asp:TextBox ID="txtNomeEmpresa" runat="server" CssClass="form-control" MaxLength="150" Enabled="false" />
            </div>
            <div class="col-md-3">
                <label class="form-label">Moeda *</label>
                <asp:TextBox ID="txtMoeda" runat="server" CssClass="form-control" MaxLength="3" Enabled="false" />
            </div>
            <div class="col-md-3">
                <label class="form-label">Fuso Horário *</label>
                <asp:TextBox ID="txtFusoHorario" runat="server" CssClass="form-control" MaxLength="50" Enabled="false" />
            </div>
        </div>
    </div>

    <div class="crm-card mb-3">
        <h5 class="crm-card-title">Alertas (dias)</h5>
        <div class="row g-3">
            <div class="col-md-4">
                <label class="form-label">Leads</label>
                <asp:TextBox ID="txtAlertaLeads" runat="server" CssClass="form-control" Enabled="false" />
            </div>
            <div class="col-md-4">
                <label class="form-label">Oportunidades</label>
                <asp:TextBox ID="txtAlertaOportunidades" runat="server" CssClass="form-control" Enabled="false" />
            </div>
            <div class="col-md-4">
                <label class="form-label">Propostas</label>
                <asp:TextBox ID="txtAlertaPropostas" runat="server" CssClass="form-control" Enabled="false" />
            </div>
        </div>
    </div>

    <div class="crm-card mb-3">
        <h5 class="crm-card-title">Segurança</h5>
        <div class="row g-3">
            <div class="col-md-4">
                <label class="form-label">Tentativas Falhadas Máximas</label>
                <asp:TextBox ID="txtMaxTentativas" runat="server" CssClass="form-control" Enabled="false" />
            </div>
            <div class="col-md-4">
                <label class="form-label">Bloqueio de Conta (minutos)</label>
                <asp:TextBox ID="txtBloqueioMinutos" runat="server" CssClass="form-control" Enabled="false" />
            </div>
            <div class="col-md-4">
                <label class="form-label">Expiração de Sessão (minutos)</label>
                <asp:TextBox ID="txtSessaoMinutos" runat="server" CssClass="form-control" Enabled="false" />
            </div>
        </div>
    </div>

    <div class="crm-form-actions">
        <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-primary" OnClick="btnGuardar_Click" Visible="false" />
    </div>

</asp:Content>