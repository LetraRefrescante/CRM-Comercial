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
            <div class="col-md-3">
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
                <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="btn btn-outline-secondary w-100" OnClick="btnFiltrar_Click" />
            </div>
        </div>
    </div>

    <div class="crm-table-card">
        <asp:Repeater ID="rptClientes" runat="server" OnItemCommand="rptClientes_ItemCommand">
            <HeaderTemplate>
                <table class="table table-hover mb-0 align-middle">
                    <thead>
                        <tr>
                            <th>Código</th>
                            <th>Nome Comercial</th>
                            <th>NIF</th>
                            <th>Cidade</th>
                            <th>Comercial</th>
                            <th>Estado</th>
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