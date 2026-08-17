<%@ Page Title="Compor Email" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="EmailCompor.aspx.cs" Inherits="CRM.Web.Paginas.Notificacoes.EmailCompor" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item active">Compor Email</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Compor Email</h2>

    <asp:PlaceHolder ID="phContexto" runat="server" Visible="false">
        <div class="alert alert-light border mb-3">
            A propósito de: <strong><asp:Literal ID="litContexto" runat="server" /></strong>
        </div>
    </asp:PlaceHolder>

    <div class="crm-card mb-3">
        <div class="row g-3">
            <div class="col-md-6">
                <label class="form-label">Modelo</label>
                <asp:DropDownList ID="ddlTemplate" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlTemplate_SelectedIndexChanged">
                    <asp:ListItem Text="Nenhum (escrever manualmente)" Value="" />
                </asp:DropDownList>
            </div>
            <div class="col-md-6">
                <label class="form-label">Destinatário *</label>
                <asp:TextBox ID="txtDestinatario" runat="server" CssClass="form-control" />
            </div>
            <div class="col-12">
                <label class="form-label">Assunto *</label>
                <asp:TextBox ID="txtAssunto" runat="server" CssClass="form-control" MaxLength="200" />
            </div>
            <div class="col-12">
                <label class="form-label">Corpo *</label>
                <asp:TextBox ID="txtCorpo" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="10" />
            </div>
        </div>

        <div class="mt-3 d-flex gap-2">
            <asp:Button ID="btnEnviar" runat="server" Text="Enviar" CssClass="btn btn-primary" OnClick="btnEnviar_Click" />
            <a href="javascript:history.back();" class="btn btn-outline-secondary">Cancelar</a>
        </div>
    </div>

</asp:Content>