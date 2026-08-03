<%@ Page Title="Detalhe do Cliente" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="ClienteDetalhe.aspx.cs" Inherits="CRM.Web.Paginas.Clientes.ClienteDetalhe" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item"><a href="~/Clientes/ClienteLista.aspx" runat="server">Clientes</a></li>
    <li class="breadcrumb-item active"><asp:Literal ID="litNomeBreadcrumb" runat="server" /></li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="crm-list-header">
        <h2><asp:Literal ID="litNome" runat="server" /></h2>
        <a href="ClienteEditar.aspx?id=<%= ClientIdAtual %>" class="btn btn-outline-secondary">
            <i class="fas fa-pen"></i> Editar
        </a>
    </div>

    <div class="card p-4 mb-3">
        <div class="row g-3">
            <div class="col-md-3"><strong>Código:</strong> <span class="mono"><asp:Literal ID="litCodigo" runat="server" /></span></div>
            <div class="col-md-3"><strong>NIF:</strong> <span class="mono"><asp:Literal ID="litNif" runat="server" /></span></div>
            <div class="col-md-3"><strong>Estado:</strong> <asp:Literal ID="litEstado" runat="server" /></div>
            <div class="col-md-3"><strong>Comercial:</strong> <asp:Literal ID="litComercial" runat="server" /></div>

            <div class="col-md-6"><strong>Email:</strong> <asp:Literal ID="litEmail" runat="server" /></div>
            <div class="col-md-6"><strong>Telefone:</strong> <asp:Literal ID="litTelefone" runat="server" /></div>

            <div class="col-12"><strong>Morada:</strong> <asp:Literal ID="litMorada" runat="server" /></div>
        </div>
    </div>

    <ul class="nav nav-tabs mb-3">
        <li class="nav-item"><a class="nav-link active" data-bs-toggle="tab" href="#tabContactos">Contactos</a></li>
        <li class="nav-item"><a class="nav-link" data-bs-toggle="tab" href="#tabOportunidades">Oportunidades</a></li>
        <li class="nav-item"><a class="nav-link" data-bs-toggle="tab" href="#tabVendas">Vendas</a></li>
        <li class="nav-item"><a class="nav-link" data-bs-toggle="tab" href="#tabDocumentos">Documentos</a></li>
    </ul>

    <div class="tab-content">
        <div class="tab-pane fade show active" id="tabContactos">
            <p class="text-muted">Módulo de Contactos ainda por implementar.</p>
        </div>
        <div class="tab-pane fade" id="tabOportunidades">
            <p class="text-muted">Módulo de Oportunidades ainda por implementar.</p>
        </div>
        <div class="tab-pane fade" id="tabVendas">
            <p class="text-muted">Módulo de Vendas ainda por implementar.</p>
        </div>
        <div class="tab-pane fade" id="tabDocumentos">
            <p class="text-muted">Módulo de Documentos ainda por implementar.</p>
        </div>
    </div>

</asp:Content>