<%@ Page Title="Vendas" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="VendasLista.aspx.cs" Inherits="CRM.Web.Paginas.Vendas.VendasLista" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item active">Vendas</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="crm-list-header">
        <h2>Vendas</h2>
        <asp:HyperLink ID="lnkNova" runat="server" NavigateUrl="~/Vendas/VendaEditar.aspx" CssClass="btn btn-primary">
            <i class="fas fa-plus"></i> Nova Venda
        </asp:HyperLink>
    </div>

    <div class="crm-filter-card">
        <div class="row g-2 align-items-end">
            <div class="col-md-3">
                <label class="form-label">Pesquisar</label>
                <asp:TextBox ID="txtPesquisa" runat="server" CssClass="form-control" placeholder="Número ou cliente..." />
            </div>
            <div class="col-md-2">
                <label class="form-label">Estado</label>
                <asp:DropDownList ID="ddlEstado" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Todos" Value="" />
                    <asp:ListItem Text="Pendente" Value="Pendente" />
                    <asp:ListItem Text="Confirmada" Value="Confirmada" />
                    <asp:ListItem Text="Parcial" Value="Parcial" />
                    <asp:ListItem Text="Concluída" Value="Concluída" />
                    <asp:ListItem Text="Cancelada" Value="Cancelada" />
                </asp:DropDownList>
            </div>
            <div class="col-md-3">
                <asp:Label ID="lblComercial" runat="server" CssClass="form-label" Text="Comercial" AssociatedControlID="ddlComercial" />
                <asp:DropDownList ID="ddlComercial" runat="server" CssClass="form-select" />
            </div>
            <div class="col-md-3">
                <label class="form-label">Data entre</label>
                <uc:FiltroDatas ID="ucFiltroDatas" runat="server" />
            </div>
            <div class="col-md-1">
                <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="btn btn-outline-secondary w-100" OnClick="btnFiltrar_Click" />
            </div>
        </div>
    </div>

    <div class="crm-table-card">
        <asp:Repeater ID="rptVendas" runat="server" OnItemCommand="rptVendas_ItemCommand" OnItemDataBound="rptVendas_ItemDataBound">
            <HeaderTemplate>
                <table class="table table-hover mb-0 align-middle">
                    <thead>
                        <tr>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="SaleNumber" OnCommand="lnkOrdenar_Command">Número</asp:LinkButton></th>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="Client" OnCommand="lnkOrdenar_Command">Cliente</asp:LinkButton></th>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="SaleDate" OnCommand="lnkOrdenar_Command">Data</asp:LinkButton></th>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="Status" OnCommand="lnkOrdenar_Command">Estado</asp:LinkButton></th>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="Total" OnCommand="lnkOrdenar_Command">Total</asp:LinkButton></th>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="Owner" OnCommand="lnkOrdenar_Command">Comercial</asp:LinkButton></th>
                            <th class="text-end">Ações</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td class="mono"><%# Eval("SaleNumber") %></td>
                    <td><%# Eval("Client.TradeName") %></td>
                    <td><%# Eval("SaleDate", "{0:dd/MM/yyyy}") %></td>
                    <td>
                        <span class="badge <%# GetBadgeClasse(Eval("Status").ToString()) %>">
                            <%# Eval("Status") %>
                        </span>
                    </td>
                    <td><%# Eval("Total", "{0:C}") %></td>
                    <td><%# Eval("Owner.Name") %></td>
                    <td class="text-end crm-row-actions">
                        <a href="VendaDetalhe.aspx?id=<%# Eval("SaleId") %>" class="btn btn-sm btn-outline-secondary" title="Abrir">
                            <i class="fas fa-eye"></i>
                        </a>
                        <asp:PlaceHolder ID="phEliminar" runat="server">
                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-danger" ToolTip="Eliminar"
                                CommandName="Eliminar" CommandArgument='<%# Eval("SaleId") %>'
                                OnClientClick="return confirm('Eliminar esta venda? O registo é mantido para auditoria.');">
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
                <i class="fas fa-file-invoice-dollar"></i>
                <p class="mb-0">Nenhuma venda encontrada com os filtros atuais.</p>
            </div>
        </asp:PlaceHolder>
    </div>

    <uc:Paginacao ID="ucPaginacao" runat="server" OnPaginaAlterada="ucPaginacao_PaginaAlterada" />

</asp:Content>