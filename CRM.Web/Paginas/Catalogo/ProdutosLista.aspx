<%@ Page Title="Produtos e Serviços" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="ProdutosLista.aspx.cs" Inherits="CRM.Web.Paginas.Catalogo.ProdutosLista" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item active">Catálogo</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="crm-list-header">
        <h2>Produtos e Serviços</h2>
        <div class="d-flex gap-2">
            <asp:HyperLink ID="lnkCategorias" runat="server" NavigateUrl="~/Catalogo/CategoriasLista.aspx" CssClass="btn btn-outline-secondary">
                <i class="fas fa-tags"></i> Categorias
            </asp:HyperLink>
            <asp:HyperLink ID="lnkNovo" runat="server" NavigateUrl="~/Catalogo/ProdutoEditar.aspx" CssClass="btn btn-primary">
                <i class="fas fa-plus"></i> Novo Produto
            </asp:HyperLink>
        </div>
    </div>

    <div class="crm-filter-card">
        <div class="row g-2 align-items-end">
            <div class="col-md-3">
                <label class="form-label">Pesquisar</label>
                <asp:TextBox ID="txtPesquisa" runat="server" CssClass="form-control" placeholder="Código ou nome..." />
            </div>
            <div class="col-md-2">
                <label class="form-label">Tipo</label>
                <asp:DropDownList ID="ddlTipo" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Todos" Value="" />
                    <asp:ListItem Text="Produto" Value="Produto" />
                    <asp:ListItem Text="Serviço" Value="Serviço" />
                </asp:DropDownList>
            </div>
            <div class="col-md-3">
                <label class="form-label">Categoria</label>
                <asp:DropDownList ID="ddlCategoria" runat="server" CssClass="form-select" />
            </div>
            <div class="col-md-2">
                <label class="form-label">Estado</label>
                <asp:DropDownList ID="ddlEstado" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Todos" Value="" />
                    <asp:ListItem Text="Ativo" Value="Ativo" />
                    <asp:ListItem Text="Inativo" Value="Inativo" />
                </asp:DropDownList>
            </div>
            <div class="col-md-2">
                <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="btn btn-outline-secondary w-100" OnClick="btnFiltrar_Click" />
            </div>
        </div>
    </div>

    <div class="crm-table-card">
        <asp:Repeater ID="rptProdutos" runat="server" OnItemCommand="rptProdutos_ItemCommand" OnItemDataBound="rptProdutos_ItemDataBound">
            <HeaderTemplate>
                <table class="table table-hover mb-0 align-middle">
                    <thead>
                        <tr>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="Code" OnCommand="lnkOrdenar_Command">Código</asp:LinkButton></th>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="Name" OnCommand="lnkOrdenar_Command">Nome</asp:LinkButton></th>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="Type" OnCommand="lnkOrdenar_Command">Tipo</asp:LinkButton></th>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="Category" OnCommand="lnkOrdenar_Command">Categoria</asp:LinkButton></th>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="BasePrice" OnCommand="lnkOrdenar_Command">Preço Base</asp:LinkButton></th>
                            <th>IVA</th>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="IsActive" OnCommand="lnkOrdenar_Command">Estado</asp:LinkButton></th>
                            <th class="text-end">Ações</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# Eval("Code") %></td>
                    <td><%# Eval("Name") %></td>
                    <td><%# Eval("Type") %></td>
                    <td><%# Eval("Category.Name") %></td>
                    <td><%# Eval("BasePrice", "{0:C}") %></td>
                    <td><%# Eval("TaxRate.Percentage", "{0}%") %></td>
                    <td>
                        <span class="badge <%# (bool)Eval("IsActive") ? "bg-success" : "bg-secondary" %>">
                            <%# (bool)Eval("IsActive") ? "Ativo" : "Inativo" %>
                        </span>
                    </td>
                    <td class="text-end crm-row-actions">
                        <asp:PlaceHolder ID="phEditar" runat="server">
                            <a href="ProdutoEditar.aspx?id=<%# Eval("ProductId") %>" class="btn btn-sm btn-outline-secondary" title="Editar">
                                <i class="fas fa-pen"></i>
                            </a>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="phEliminar" runat="server">
                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-danger" ToolTip="Eliminar"
                                CommandName="Eliminar" CommandArgument='<%# Eval("ProductId") %>'
                                data-confirm='<%# "Eliminar o produto " + Eval("Name") + "?" %>'>
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
                <i class="fas fa-box"></i>
                <p class="mb-0">Nenhum produto encontrado com os filtros atuais.</p>
            </div>
        </asp:PlaceHolder>
    </div>

    <uc:Paginacao ID="ucPaginacao" runat="server" OnPaginaAlterada="ucPaginacao_PaginaAlterada" />

</asp:Content>