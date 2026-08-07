<%@ Page Title="Clientes" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="ClienteLista.aspx.cs" Inherits="CRM.Web.Paginas.Clientes.ClienteLista" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item active">Clientes</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="crm-list-header">
        <h2>Clientes</h2>
        <asp:HyperLink ID="lnkNovo" runat="server" NavigateUrl="~/Clientes/ClienteEditar.aspx" CssClass="btn btn-primary">
            <i class="fas fa-plus"></i> Novo Cliente
        </asp:HyperLink>
    </div>

    <div class="crm-filter-card">
        <div class="row g-2 align-items-end">
            <div class="col-md-4">
                <label class="form-label">Pesquisar</label>
                <asp:TextBox ID="txtPesquisa" runat="server" CssClass="form-control" placeholder="Nome, NIF, email ou cidade..." />
            </div>
            <div class="col-md-2">
                <label class="form-label">Estado</label>
                <asp:DropDownList ID="ddlEstado" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Todos" Value="" />
                    <asp:ListItem Text="Potencial" Value="Potencial" />
                    <asp:ListItem Text="Ativo" Value="Ativo" />
                    <asp:ListItem Text="Inativo" Value="Inativo" />
                    <asp:ListItem Text="Bloqueado" Value="Bloqueado" />
                </asp:DropDownList>
            </div>
            <div class="col-md-3">
                <asp:Label ID="lblComercial" runat="server" CssClass="form-label" Text="Comercial" AssociatedControlID="ddlComercial" />
                <asp:DropDownList ID="ddlComercial" runat="server" CssClass="form-select" />
            </div>
            <div class="col-md-3 d-flex gap-2">
                <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="btn btn-outline-secondary flex-fill" OnClick="btnFiltrar_Click" />
                <asp:Button ID="btnExportar" runat="server" Text="Exportar" CssClass="btn btn-outline-primary flex-fill" OnClick="btnExportar_Click" CausesValidation="false" />
            </div>
        </div>
    </div>

    <div class="crm-table-card">
        <asp:Repeater ID="rptClientes" runat="server" OnItemCommand="rptClientes_ItemCommand" OnItemDataBound="rptClientes_ItemDataBound">
            <HeaderTemplate>
                <table class="table table-hover mb-0 align-middle">
                    <thead>
                        <tr>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="InternalCode" OnCommand="lnkOrdenar_Command">Código</asp:LinkButton></th>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="TradeName" OnCommand="lnkOrdenar_Command">Nome Comercial</asp:LinkButton></th>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="VatNumber" OnCommand="lnkOrdenar_Command">NIF</asp:LinkButton></th>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="City" OnCommand="lnkOrdenar_Command">Cidade</asp:LinkButton></th>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="AccountManager" OnCommand="lnkOrdenar_Command">Comercial</asp:LinkButton></th>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="Status" OnCommand="lnkOrdenar_Command">Estado</asp:LinkButton></th>
                            <th class="text-end">Ações</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td class="mono"><%# Eval("InternalCode") %></td>
                    <td><%# Eval("TradeName") %></td>
                    <td class="mono"><%# Eval("VatNumber") %></td>
                    <td><%# Eval("City") %></td>
                    <td><%# Eval("AccountManager.Name") %></td>
                    <td>
                        <span class="badge <%# GetBadgeClasse(Eval("Status").ToString()) %>">
                            <%# Eval("Status") %>
                        </span>
                    </td>
                    <td class="text-end crm-row-actions">
                        <a href="ClienteDetalhe.aspx?id=<%# Eval("ClientId") %>" class="btn btn-sm btn-outline-secondary" title="Ver">
                            <i class="fas fa-eye"></i>
                        </a>
                        <asp:PlaceHolder ID="phEditar" runat="server">
                            <a href="ClienteEditar.aspx?id=<%# Eval("ClientId") %>" class="btn btn-sm btn-outline-secondary" title="Editar">
                                <i class="fas fa-pen"></i>
                            </a>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="phEliminar" runat="server">
                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-danger" ToolTip="Eliminar"
                                CommandName="Eliminar" CommandArgument='<%# Eval("ClientId") %>'
                                data-confirm='<%# "Eliminar o cliente " + Eval("TradeName") + "? O registo é mantido para auditoria." %>'>
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
                <i class="fas fa-building-circle-xmark"></i>
                <p class="mb-0">Nenhum cliente encontrado com os filtros atuais.</p>
            </div>
        </asp:PlaceHolder>
    </div>

    <uc:Paginacao ID="ucPaginacao" runat="server" OnPaginaAlterada="ucPaginacao_PaginaAlterada" />

</asp:Content>