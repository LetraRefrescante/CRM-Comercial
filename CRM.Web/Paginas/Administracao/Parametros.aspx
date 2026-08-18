<%@ Page Title="Parâmetros" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="Parametros.aspx.cs" Inherits="CRM.Web.Paginas.Administracao.Parametros" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item">Administração</li>
    <li class="breadcrumb-item active">Parâmetros</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="crm-list-header"><h2>Parâmetros Gerais</h2></div>

    <div class="crm-form-card" style="max-width: 640px;">
        <asp:ValidationSummary ID="valSummary" runat="server" CssClass="alert alert-danger" DisplayMode="BulletList" />
        <asp:CustomValidator ID="cvRegrasNegocio" runat="server" Display="None" OnServerValidate="cvRegrasNegocio_ServerValidate" />

        <div class="row g-3">
            <div class="col-12">
                <label class="form-label">Nome da Empresa *</label>
                <asp:TextBox ID="txtEmpresa" runat="server" CssClass="form-control" MaxLength="150" />
                <div class="form-text">Usado nos documentos gerados (propostas, PDFs, emails).</div>
            </div>
            <div class="col-md-6">
                <label class="form-label">Moeda *</label>
                <asp:DropDownList ID="ddlMoeda" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Euro (€)" Value="EUR" />
                    <asp:ListItem Text="Dólar Americano ($)" Value="USD" />
                    <asp:ListItem Text="Libra Esterlina (£)" Value="GBP" />
                </asp:DropDownList>
            </div>
            <div class="col-md-6">
                <label class="form-label">Fuso Horário *</label>
                <asp:DropDownList ID="ddlFusoHorario" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Europe/Lisbon" Value="Europe/Lisbon" />
                    <asp:ListItem Text="Europe/Madrid" Value="Europe/Madrid" />
                    <asp:ListItem Text="UTC" Value="UTC" />
                </asp:DropDownList>
            </div>
            <div class="col-md-6">
                <label class="form-label">Dias de Alerta</label>
                <asp:TextBox ID="txtDiasAlerta" runat="server" CssClass="form-control" TextMode="Number" />
                <div class="form-text">Antecedência para avisos de Leads, Oportunidades e Propostas a expirar.</div>
            </div>
        </div>

        <div class="mt-4">
            <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-primary" OnClick="btnGuardar_Click" />
        </div>
    </div>

</asp:Content>