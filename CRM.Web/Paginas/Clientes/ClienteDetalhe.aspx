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
        <li class="nav-item"><a class="nav-link" data-bs-toggle="tab" href="#tabHistorico">Histórico</a></li>
    </ul>
    <div class="tab-content">
        <div class="tab-pane fade show active" id="tabContactos">
            <div class="d-flex justify-content-end mb-2">
                <asp:HyperLink ID="lnkNovoContacto" runat="server" CssClass="btn btn-primary btn-sm">
                    <i class="fas fa-plus"></i> Novo Contacto
                </asp:HyperLink>
            </div>
            <asp:Repeater ID="rptContactos" runat="server" OnItemCommand="rptContactos_ItemCommand">
                <HeaderTemplate>
                    <table class="table table-hover mb-0 align-middle">
                        <thead>
                            <tr>
                                <th>Nome</th>
                                <th>Cargo</th>
                                <th>Email</th>
                                <th>Telefone</th>
                                <th>Telemóvel</th>
                                <th></th>
                                <th class="text-end">Ações</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%# Eval("Name") %></td>
                        <td><%# Eval("JobTitle") ?? "—" %></td>
                        <td><%# Eval("Email") ?? "—" %></td>
                        <td><%# Eval("Phone") ?? "—" %></td>
                        <td><%# Eval("MobilePhone") ?? "—" %></td>
                        <td>
                            <asp:PlaceHolder runat="server" Visible='<%# (bool)Eval("IsPrimary") %>'>
                                <span class="badge bg-primary">Principal</span>
                            </asp:PlaceHolder>
                        </td>
                        <td class="text-end crm-row-actions">
                            <a href="ContactoDetalhe.aspx?id=<%# Eval("ContactId") %>" class="btn btn-sm btn-outline-secondary" title="Ver">
                                <i class="fas fa-eye"></i>
                            </a>
                            <asp:PlaceHolder ID="phEditarContacto" runat="server">
                                <a href="ContactoEditar.aspx?id=<%# Eval("ContactId") %>&clienteId=<%# ClientIdAtual %>"
                                    class="btn btn-sm btn-outline-secondary" title="Editar">
                                    <i class="fas fa-pen"></i>
                                </a>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="phEliminarContacto" runat="server">
                                <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-danger" ToolTip="Eliminar"
                                    CommandName="Eliminar" CommandArgument='<%# Eval("ContactId") %>'
                                    data-confirm='<%# "Eliminar o contacto " + Eval("Name") + "?" %>'>
                                    <i class="fas fa-trash"></i>
                                </asp:LinkButton>
                            </asp:PlaceHolder>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                        </tbody>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
            <asp:PlaceHolder ID="phContactosVazio" runat="server" Visible="false">
                <div class="crm-empty-state">
                    <i class="fas fa-address-book"></i>
                    <p class="mb-0">Nenhum contacto associado a este cliente.</p>
                </div>
            </asp:PlaceHolder>
        </div>
        <div class="tab-pane fade" id="tabOportunidades">
            <p class="text-muted">Módulo de Oportunidades ainda por implementar.</p>
        </div>
        <div class="tab-pane fade" id="tabVendas">
            <p class="text-muted">Módulo de Vendas ainda por implementar.</p>
        </div>
        <div class="tab-pane fade" id="tabDocumentos">
            <uc:Anexos ID="ucAnexos" runat="server" />
        </div>
        <div class="tab-pane fade" id="tabHistorico">
            <uc:Historico ID="ucHistorico" runat="server" />
        </div>
    </div>
</asp:Content>