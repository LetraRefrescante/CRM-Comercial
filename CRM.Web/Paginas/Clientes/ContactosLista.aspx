<%@ Page Title="Contactos" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="ContactosLista.aspx.cs" Inherits="CRM.Web.Paginas.Clientes.ContactosLista" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item active">Contactos</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="crm-list-header">
        <h2>Contactos</h2>
        <asp:PlaceHolder ID="phNovoContacto" runat="server">
            <uc:SeletorCliente ID="ucSeletorCliente" runat="server" Obrigatorio="false"
                OcultarCampoTexto="true" TextoBotao="Novo Contacto" IconeBotao="fa-plus"
                CssClassBotao="btn btn-primary" OnClienteSelecionado="ucSeletorCliente_ClienteSelecionado" />
        </asp:PlaceHolder>
    </div>

    <div class="crm-filter-card">
        <div class="row g-2 align-items-end">
            <div class="col-md-6">
                <label class="form-label">Pesquisar</label>
                <asp:TextBox ID="txtPesquisa" runat="server" CssClass="form-control" placeholder="Nome, email ou cliente..." />
            </div>
            <div id="colComercial" runat="server" class="col-md-3">
                <asp:Label ID="lblComercial" runat="server" CssClass="form-label" Text="Comercial" AssociatedControlID="ddlComercial" />
                <asp:DropDownList ID="ddlComercial" runat="server" CssClass="form-select" />
            </div>
            <div class="col-md-3">
                <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="btn btn-outline-secondary w-100" OnClick="btnFiltrar_Click" />
            </div>
        </div>
    </div>

    <div class="crm-table-card">
        <asp:Repeater ID="rptContactos" runat="server" OnItemCommand="rptContactos_ItemCommand" OnItemDataBound="rptContactos_ItemDataBound">
            <HeaderTemplate>
                <table class="table table-hover mb-0 align-middle">
                    <thead>
                        <tr>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="Name" OnCommand="lnkOrdenar_Command">Nome</asp:LinkButton></th>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="Client" OnCommand="lnkOrdenar_Command">Cliente</asp:LinkButton></th>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="JobTitle" OnCommand="lnkOrdenar_Command">Cargo</asp:LinkButton></th>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="Email" OnCommand="lnkOrdenar_Command">Email</asp:LinkButton></th>
                            <th>Telefone</th>
                            <th></th>
                            <th class="text-end">Ações</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# Eval("Name") %></td>
                    <td>
                        <a href="ClienteDetalhe.aspx?id=<%# Eval("ClientId") %>"><%# Eval("Client.TradeName") %></a>
                    </td>
                    <td><%# Eval("JobTitle") ?? "—" %></td>
                    <td><%# Eval("Email") ?? "—" %></td>
                    <td><%# Eval("Phone") ?? Eval("MobilePhone") ?? "—" %></td>
                    <td>
                        <asp:PlaceHolder runat="server" Visible='<%# (bool)Eval("IsPrimary") %>'>
                            <span class="badge bg-primary">Principal</span>
                        </asp:PlaceHolder>
                    </td>
                    <td class="text-end crm-row-actions">
                        <a href="ContactoDetalhe.aspx?id=<%# Eval("ContactId") %>" class="btn btn-sm btn-outline-secondary" title="Ver">
                            <i class="fas fa-eye"></i>
                        </a>
                        <asp:PlaceHolder ID="phEditar" runat="server">
                            <a href="ContactoEditar.aspx?id=<%# Eval("ContactId") %>&clienteId=<%# Eval("ClientId") %>"
                                class="btn btn-sm btn-outline-secondary" title="Editar">
                                <i class="fas fa-pen"></i>
                            </a>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="phEliminar" runat="server">
                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-danger" ToolTip="Eliminar"
                                CommandName="Eliminar" CommandArgument='<%# Eval("ContactId") + "|" + Eval("ClientId") %>'
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

        <asp:PlaceHolder ID="phVazio" runat="server" Visible="false">
            <div class="crm-empty-state">
                <i class="fas fa-address-book"></i>
                <p class="mb-0">Nenhum contacto encontrado com os filtros atuais.</p>
            </div>
        </asp:PlaceHolder>
    </div>

    <uc:Paginacao ID="ucPaginacao" runat="server" OnPaginaAlterada="ucPaginacao_PaginaAlterada" />

</asp:Content>