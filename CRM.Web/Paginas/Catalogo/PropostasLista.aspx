<%@ Page Title="Propostas" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="PropostasLista.aspx.cs" Inherits="CRM.Web.Paginas.Catalogo.PropostasLista" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item active">Propostas</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="crm-list-header">
        <h2>Propostas</h2>
        <asp:HyperLink ID="lnkNova" runat="server" NavigateUrl="~/Catalogo/PropostaEditar.aspx" CssClass="btn btn-primary">
            <i class="fas fa-plus"></i> Nova Proposta
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
                    <asp:ListItem Text="Rascunho" Value="Rascunho" />
                    <asp:ListItem Text="Enviada" Value="Enviada" />
                    <asp:ListItem Text="Aceite" Value="Aceite" />
                    <asp:ListItem Text="Recusada" Value="Recusada" />
                    <asp:ListItem Text="Expirada" Value="Expirada" />
                    <asp:ListItem Text="Cancelada" Value="Cancelada" />
                </asp:DropDownList>
            </div>
            <div class="col-md-3">
                <asp:Label ID="lblComercial" runat="server" CssClass="form-label" Text="Comercial" AssociatedControlID="ddlComercial" />
                <asp:DropDownList ID="ddlComercial" runat="server" CssClass="form-select" />
            </div>
            <div class="col-md-3">
                <label class="form-label">Emissão entre</label>
                <uc:FiltroDatas ID="ucFiltroDatas" runat="server" />
            </div>
            <div class="col-md-1">
                <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="btn btn-outline-secondary w-100" OnClick="btnFiltrar_Click" />
            </div>
        </div>
    </div>

    <div class="crm-table-card">
        <asp:Repeater ID="rptPropostas" runat="server" OnItemCommand="rptPropostas_ItemCommand" OnItemDataBound="rptPropostas_ItemDataBound">
            <HeaderTemplate>
                <table class="table table-hover mb-0 align-middle">
                    <thead>
                        <tr>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="ProposalNumber" OnCommand="lnkOrdenar_Command">Número</asp:LinkButton></th>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="Client" OnCommand="lnkOrdenar_Command">Cliente</asp:LinkButton></th>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="IssueDate" OnCommand="lnkOrdenar_Command">Emissão</asp:LinkButton></th>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="ValidUntil" OnCommand="lnkOrdenar_Command">Validade</asp:LinkButton></th>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="Status" OnCommand="lnkOrdenar_Command">Estado</asp:LinkButton></th>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="Total" OnCommand="lnkOrdenar_Command">Total</asp:LinkButton></th>
                            <th>Comercial</th>
                            <th class="text-end">Ações</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td class="mono"><%# Eval("ProposalNumber") %></td>
                    <td><%# Eval("Client.TradeName") %></td>
                    <td><%# Eval("IssueDate", "{0:dd/MM/yyyy}") %></td>
                    <td><%# Eval("ValidUntil", "{0:dd/MM/yyyy}") %></td>
                    <td>
                        <span class="badge <%# GetBadgeClasse(Eval("Status").ToString()) %>">
                            <%# Eval("Status") %>
                        </span>
                    </td>
                    <td><%# Eval("Total", "{0:C}") %></td>
                    <td><%# Eval("Client.AccountManager.Name") %></td>
                    <td class="text-end crm-row-actions">
                        <asp:PlaceHolder ID="phEditar" runat="server">
                            <a href="PropostaEditar.aspx?id=<%# Eval("ProposalId") %>" class="btn btn-sm btn-outline-secondary" title="Editar">
                                <i class="fas fa-pen"></i>
                            </a>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="phEliminar" runat="server">
                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-danger" ToolTip="Eliminar"
                                CommandName="Eliminar" CommandArgument='<%# Eval("ProposalId") %>'
                                data-confirm='<%# "Eliminar a proposta " + Eval("ProposalNumber") + "? O registo é mantido para auditoria." %>'>
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
                <i class="fas fa-file-invoice"></i>
                <p class="mb-0">Nenhuma proposta encontrada com os filtros atuais.</p>
            </div>
        </asp:PlaceHolder>
    </div>

    <uc:Paginacao ID="ucPaginacao" runat="server" OnPaginaAlterada="ucPaginacao_PaginaAlterada" />

</asp:Content>