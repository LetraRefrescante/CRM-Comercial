<%@ Page Title="Leads" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="LeadsLista.aspx.cs" Inherits="CRM.Web.Paginas.Leads.LeadsLista" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item active">Leads</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="crm-list-header">
        <h2>Leads</h2>
        <asp:HyperLink ID="lnkNovo" runat="server" NavigateUrl="~/Leads/LeadEditar.aspx" CssClass="btn btn-primary">
            <i class="fas fa-plus"></i> Novo Lead
        </asp:HyperLink>
    </div>

    <div class="crm-filter-card">
        <div class="row g-2 align-items-end">
            <div class="col-md-3">
                <label class="form-label">Pesquisar</label>
                <asp:TextBox ID="txtPesquisa" runat="server" CssClass="form-control" placeholder="Nome, empresa, email ou telefone..." />
            </div>
            <div class="col-md-2">
                <label class="form-label">Origem</label>
                <asp:DropDownList ID="ddlOrigem" runat="server" CssClass="form-select" />
            </div>
            <div class="col-md-2">
                <label class="form-label">Estado</label>
                <asp:DropDownList ID="ddlEstado" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Todos" Value="" />
                    <asp:ListItem Text="Novo" Value="Novo" />
                    <asp:ListItem Text="Em Contacto" Value="Em Contacto" />
                    <asp:ListItem Text="Qualificado" Value="Qualificado" />
                    <asp:ListItem Text="Não Qualificado" Value="Não Qualificado" />
                    <asp:ListItem Text="Convertido" Value="Convertido" />
                </asp:DropDownList>
            </div>
            <div class="col-md-2">
                <asp:Label ID="lblComercial" runat="server" CssClass="form-label" Text="Comercial" AssociatedControlID="ddlComercial" />
                <asp:DropDownList ID="ddlComercial" runat="server" CssClass="form-select" />
            </div>
            <div class="col-md-1">
                <label class="form-label">Pont. Min</label>
                <asp:TextBox ID="txtPontuacaoMin" runat="server" CssClass="form-control" TextMode="Number" />
            </div>
            <div class="col-md-1">
                <label class="form-label">Pont. Máx</label>
                <asp:TextBox ID="txtPontuacaoMax" runat="server" CssClass="form-control" TextMode="Number" />
            </div>
            <div class="col-md-1">
                <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="btn btn-outline-secondary w-100" OnClick="btnFiltrar_Click" />
            </div>
        </div>
        <div class="row g-2 align-items-end mt-2">
            <div class="col-md-6">
                <label class="form-label">Criado entre</label>
                <uc:FiltroDatas ID="ucFiltroDatas" runat="server" />
            </div>
        </div>
    </div>

    <div class="crm-table-card">
        <asp:Repeater ID="rptLeads" runat="server" OnItemCommand="rptLeads_ItemCommand" OnItemDataBound="rptLeads_ItemDataBound">
            <HeaderTemplate>
                <table class="table table-hover mb-0 align-middle">
                    <thead>
                        <tr>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="Name" OnCommand="lnkOrdenar_Command">Nome</asp:LinkButton></th>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="CompanyName" OnCommand="lnkOrdenar_Command">Empresa</asp:LinkButton></th>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="LeadSource" OnCommand="lnkOrdenar_Command">Origem</asp:LinkButton></th>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="Status" OnCommand="lnkOrdenar_Command">Estado</asp:LinkButton></th>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="Score" OnCommand="lnkOrdenar_Command">Pontuação</asp:LinkButton></th>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="Owner" OnCommand="lnkOrdenar_Command">Comercial</asp:LinkButton></th>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="NextContactDate" OnCommand="lnkOrdenar_Command">Próx. Contacto</asp:LinkButton></th>
                            <th class="text-end">Ações</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# Eval("Name") %></td>
                    <td><%# Eval("CompanyName") ?? "—" %></td>
                    <td><%# Eval("LeadSource.Name") %></td>
                    <td>
                        <span class="badge <%# GetBadgeClasse(Eval("Status").ToString()) %>">
                            <%# Eval("Status") %>
                        </span>
                    </td>
                    <td><%# Eval("Score") ?? "—" %></td>
                    <td><%# Eval("Owner.Name") %></td>
                    <td><%# Eval("NextContactDate", "{0:dd/MM/yyyy HH:mm}") %></td>
                    <td class="text-end crm-row-actions">
                        <a href="LeadDetalhe.aspx?id=<%# Eval("LeadId") %>" class="btn btn-sm btn-outline-secondary" title="Ver">
                            <i class="fas fa-eye"></i>
                        </a>
                        <asp:PlaceHolder ID="phEditar" runat="server">
                            <a href="LeadEditar.aspx?id=<%# Eval("LeadId") %>" class="btn btn-sm btn-outline-secondary" title="Editar">
                                <i class="fas fa-pen"></i>
                            </a>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="phConverter" runat="server">
                            <a href="LeadConverter.aspx?id=<%# Eval("LeadId") %>" class="btn btn-sm btn-outline-success" title="Converter">
                                <i class="fas fa-right-left"></i>
                            </a>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="phEliminar" runat="server">
                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-danger" ToolTip="Eliminar"
                                CommandName="Eliminar" CommandArgument='<%# Eval("LeadId") %>'
                                data-confirm='<%# "Eliminar o lead " + Eval("Name") + "?" %>'>
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
                <i class="fas fa-bullseye"></i>
                <p class="mb-0">Nenhum lead encontrado com os filtros atuais.</p>
            </div>
        </asp:PlaceHolder>
    </div>

    <uc:Paginacao ID="ucPaginacao" runat="server" OnPaginaAlterada="ucPaginacao_PaginaAlterada" />

</asp:Content>