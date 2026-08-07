<%@ Page Title="Detalhe do Contacto" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="ContactoDetalhe.aspx.cs" Inherits="CRM.Web.Paginas.Clientes.ContactoDetalhe" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item"><a href="~/Clientes/ClienteLista.aspx" runat="server">Clientes</a></li>
    <li class="breadcrumb-item"><a id="lnkClientePai" runat="server"></a></li>
    <li class="breadcrumb-item active"><asp:Literal ID="litNomeBreadcrumb" runat="server" /></li>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="crm-list-header">
        <h2><asp:Literal ID="litNome" runat="server" /></h2>
        <div>
            <asp:PlaceHolder ID="phEditar" runat="server">
                <a href="ContactoEditar.aspx?id=<%= ContactIdAtual %>" class="btn btn-outline-secondary">
                    <i class="fas fa-pen"></i> Editar
                </a>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="phEliminar" runat="server">
                <asp:LinkButton ID="btnEliminar" runat="server" CssClass="btn btn-outline-danger"
                    OnClick="btnEliminar_Click" data-confirm='<%# "Eliminar o contacto " + litNome.Text + "?" %>'>
                    <i class="fas fa-trash"></i> Eliminar
                </asp:LinkButton>
            </asp:PlaceHolder>
        </div>
    </div>
    <div class="card p-4 mb-3">
        <div class="row g-3">
            <div class="col-md-6"><strong>Cliente:</strong> <asp:Literal ID="litClienteNome" runat="server" /></div>
            <div class="col-md-6"><strong>Contacto Principal:</strong> <asp:Literal ID="litPrincipal" runat="server" /></div>
            <div class="col-md-4"><strong>Cargo:</strong> <asp:Literal ID="litCargo" runat="server" /></div>
            <div class="col-md-4"><strong>Departamento:</strong> <asp:Literal ID="litDepartamento" runat="server" /></div>
            <div class="col-md-4"><strong>Data de Nascimento:</strong> <asp:Literal ID="litDataNascimento" runat="server" /></div>
            <div class="col-md-4"><strong>Email:</strong> <asp:Literal ID="litEmail" runat="server" /></div>
            <div class="col-md-4"><strong>Telefone:</strong> <asp:Literal ID="litTelefone" runat="server" /></div>
            <div class="col-md-4"><strong>Telemóvel:</strong> <asp:Literal ID="litTelemovel" runat="server" /></div>
            <div class="col-md-6"><strong>Preferência de Contacto:</strong> <asp:Literal ID="litPreferencia" runat="server" /></div>
            <div class="col-md-6"><strong>Consentimento dado:</strong> <asp:Literal ID="litConsentimento" runat="server" /></div>
            <div class="col-12"><strong>Restrições de contacto:</strong> <asp:Literal ID="litRestricoes" runat="server" /></div>
        </div>
    </div>
    <div class="card p-4 mb-3">
        <h6>Histórico de Atividades</h6>
        <p class="text-muted">Módulo de Atividades ainda por implementar.</p>
    </div>
    <div class="card p-4 mb-3">
        <h6>Histórico de Alterações</h6>
        <uc:Historico ID="ucHistorico" runat="server" />
    </div>
</asp:Content>