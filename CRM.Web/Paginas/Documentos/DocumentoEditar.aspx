<%@ Page Title="Carregar Documento" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="DocumentoEditar.aspx.cs" Inherits="CRM.Web.Paginas.Documentos.DocumentoEditar" %>
<%@ Register TagPrefix="uc" TagName="SeletorCliente" Src="~/Controls/SeletorCliente.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item"><a href="DocumentosLista.aspx">Documentos</a></li>
    <li class="breadcrumb-item active">Carregar</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Carregar Documento</h2>

    <div class="crm-card mb-3">
        <h5 class="crm-card-title">Ficheiro</h5>
        <div class="row g-3">
            <div class="col-md-6">
                <label class="form-label">Ficheiro *</label>
                <asp:FileUpload ID="fuFicheiro" runat="server" CssClass="form-control" />
                <small class="text-muted">Máximo 10 MB. Extensões aceites: pdf, doc, docx, xls, xlsx, png, jpg, jpeg.</small>
            </div>
            <div class="col-md-6">
                <label class="form-label">Título *</label>
                <asp:TextBox ID="txtTitulo" runat="server" CssClass="form-control" MaxLength="180" />
            </div>
            <div class="col-md-4">
                <label class="form-label">Categoria *</label>
                <asp:DropDownList ID="ddlCategoria" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Contrato" Value="Contrato" />
                    <asp:ListItem Text="Proposta" Value="Proposta" />
                    <asp:ListItem Text="Identificação" Value="Identificação" />
                    <asp:ListItem Text="Outro" Value="Outro" />
                </asp:DropDownList>
            </div>
            <div class="col-md-4 d-flex align-items-end">
                <div class="form-check">
                    <asp:CheckBox ID="chkConfidencial" runat="server" CssClass="form-check-input" />
                    <label class="form-check-label" for="chkConfidencial">Confidencial</label>
                </div>
            </div>
        </div>
    </div>

    <div class="crm-card mb-3">
        <h5 class="crm-card-title">Relacionado Com *</h5>
        <div class="row g-3">
            <div class="col-md-3">
                <asp:DropDownList ID="ddlTipoRelacao" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlTipoRelacao_SelectedIndexChanged">
                    <asp:ListItem Text="Selecione..." Value="" />
                    <asp:ListItem Text="Cliente" Value="Client" />
                    <asp:ListItem Text="Lead" Value="Lead" />
                    <asp:ListItem Text="Oportunidade" Value="Opportunity" />
                    <asp:ListItem Text="Proposta" Value="Proposal" />
                    <asp:ListItem Text="Venda" Value="Sale" />
                </asp:DropDownList>
            </div>
            <asp:Panel ID="pnlCliente" runat="server" CssClass="col-md-6" Visible="false">
                <uc:SeletorCliente ID="ucCliente" runat="server" />
            </asp:Panel>
            <asp:Panel ID="pnlLead" runat="server" CssClass="col-md-6" Visible="false">
                <asp:DropDownList ID="ddlLead" runat="server" CssClass="form-select" />
            </asp:Panel>
            <asp:Panel ID="pnlOportunidade" runat="server" CssClass="col-md-6" Visible="false">
                <asp:DropDownList ID="ddlOportunidade" runat="server" CssClass="form-select" />
            </asp:Panel>
            <asp:Panel ID="pnlProposta" runat="server" CssClass="col-md-6" Visible="false">
                <asp:DropDownList ID="ddlProposta" runat="server" CssClass="form-select" />
            </asp:Panel>
            <asp:Panel ID="pnlVenda" runat="server" CssClass="col-md-6" Visible="false">
                <asp:DropDownList ID="ddlVenda" runat="server" CssClass="form-select" />
            </asp:Panel>
        </div>
    </div>

    <div class="crm-form-actions">
        <asp:Button ID="btnGuardar" runat="server" Text="Carregar" CssClass="btn btn-primary" OnClick="btnGuardar_Click" />
        <asp:HyperLink ID="lnkCancelar" runat="server" NavigateUrl="~/Documentos/DocumentosLista.aspx" CssClass="btn btn-outline-secondary">Cancelar</asp:HyperLink>
    </div>

</asp:Content>