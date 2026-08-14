<%@ Page Title="Enviar Proposta" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="PropostaEnviar.aspx.cs" Inherits="CRM.Web.Paginas.Catalogo.PropostaEnviar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item"><a href="PropostasLista.aspx">Propostas</a></li>
    <li class="breadcrumb-item active">Enviar</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="crm-list-header">
        <h2>Enviar Proposta <asp:Label ID="lblNumero" runat="server" CssClass="text-muted fs-6" /></h2>
    </div>

    <asp:PlaceHolder ID="phJaEnviada" runat="server" Visible="false">
        <div class="alert alert-info">
            Esta proposta já foi enviada em <asp:Label ID="lblDataEnvioAnterior" runat="server" /> para
            <asp:Label ID="lblEmailAnterior" runat="server" />. Podes reenviar abaixo se necessário.
        </div>
    </asp:PlaceHolder>

    <div class="crm-form-card">
        <div class="row g-3">
            <div class="col-md-8">
                <label class="form-label">Email do Destinatário *</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEmail" CssClass="text-danger small"
                    ErrorMessage="O email do destinatário é obrigatório." Display="Dynamic" />
            </div>
        </div>

        <div class="mt-3">
            <label class="form-label">Pré-visualização</label>
            <div class="border rounded p-3 bg-light small">
                Proposta <strong><asp:Literal ID="litNumeroPreview" runat="server" /></strong> — Cliente:
                <strong><asp:Literal ID="litClientePreview" runat="server" /></strong> — Total:
                <strong><asp:Literal ID="litTotalPreview" runat="server" /></strong>
                <br />
                <a href="#" id="lnkVerPdfPreview" runat="server" target="_blank">Ver PDF antes de enviar</a>
            </div>
        </div>

        <asp:ValidationSummary ID="vsResumo" runat="server" CssClass="alert alert-danger mt-3" />

        <div class="mt-3 d-flex gap-2">
            <asp:Button ID="btnEnviar" runat="server" Text="Enviar e Marcar como Enviada" CssClass="btn btn-primary" OnClick="btnEnviar_Click" />
            <a href="PropostaDetalhe.aspx?id=<%= Request.QueryString["id"] %>" class="btn btn-outline-secondary">Cancelar</a>
        </div>
    </div>

</asp:Content>