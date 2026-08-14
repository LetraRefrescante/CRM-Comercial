<%@ Page Title="Tarefas" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="TarefasLista.aspx.cs" Inherits="CRM.Web.Paginas.Atividades.TarefasLista" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item active">Tarefas</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="crm-list-header">
        <h2>Tarefas</h2>
        <asp:HyperLink ID="lnkNova" runat="server" NavigateUrl="~/Atividades/TarefaEditar.aspx" CssClass="btn btn-primary">
            <i class="fas fa-plus"></i> Nova Tarefa
        </asp:HyperLink>
    </div>

    <div class="crm-filter-card">
        <div class="row g-2 align-items-end">
            <div class="col-md-4">
                <label class="form-label">Pesquisar</label>
                <asp:TextBox ID="txtPesquisa" runat="server" CssClass="form-control" placeholder="Assunto..." />
            </div>
            <div class="col-md-2">
                <label class="form-label">Estado</label>
                <asp:DropDownList ID="ddlEstado" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Todos" Value="" />
                    <asp:ListItem Text="Planeada" Value="Planeada" />
                    <asp:ListItem Text="Em Curso" Value="Em Curso" />
                    <asp:ListItem Text="Concluída" Value="Concluída" />
                    <asp:ListItem Text="Cancelada" Value="Cancelada" />
                </asp:DropDownList>
            </div>
            <div class="col-md-2">
                <asp:Label ID="lblResponsavel" runat="server" CssClass="form-label" Text="Responsável" AssociatedControlID="ddlResponsavel" />
                <asp:DropDownList ID="ddlResponsavel" runat="server" CssClass="form-select" />
            </div>
            <div class="col-md-3">
                <label class="form-label">Período</label>
                <uc:FiltroDatas ID="ucFiltroDatas" runat="server" />
            </div>
            <div class="col-md-1">
                <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="btn btn-outline-secondary w-100" OnClick="btnFiltrar_Click" />
            </div>
        </div>
    </div>

    <div class="crm-table-card">
        <asp:Repeater ID="rptTarefas" runat="server" OnItemCommand="rptTarefas_ItemCommand" OnItemDataBound="rptTarefas_ItemDataBound">
            <HeaderTemplate>
                <table class="table table-hover mb-0 align-middle">
                    <thead>
                        <tr>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="Subject" OnCommand="lnkOrdenar_Command">Assunto</asp:LinkButton></th>
                            <th>Relacionado Com</th>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="DueDate" OnCommand="lnkOrdenar_Command">Data Limite</asp:LinkButton></th>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="Status" OnCommand="lnkOrdenar_Command">Estado</asp:LinkButton></th>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="AssignedTo" OnCommand="lnkOrdenar_Command">Responsável</asp:LinkButton></th>
                            <th class="text-end">Ações</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# Eval("Subject") %></td>
                    <td><%# GetRelacionado(Container.DataItem) %></td>
                    <td class="<%# GetVencidaClasse(Container.DataItem) %>"><%# Eval("DueDate", "{0:dd/MM/yyyy HH:mm}") %></td>
                    <td>
                        <span class="badge <%# GetBadgeClasse(Eval("Status").ToString()) %>">
                            <%# Eval("Status") %>
                        </span>
                    </td>
                    <td><%# Eval("AssignedTo.Name") %></td>
                    <td class="text-end crm-row-actions">
                        <asp:PlaceHolder ID="phEditar" runat="server">
                            <a href="TarefaEditar.aspx?id=<%# Eval("TaskId") %>" class="btn btn-sm btn-outline-secondary" title="Editar">
                                <i class="fas fa-pen"></i>
                            </a>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="phEliminar" runat="server">
                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-danger" ToolTip="Eliminar"
                                CommandName="Eliminar" CommandArgument='<%# Eval("TaskId") %>'
                                data-confirm='<%# "Eliminar a tarefa \"" + Eval("Subject") + "\"? O registo é mantido para auditoria." %>'>
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
                <i class="fas fa-list-check"></i>
                <p class="mb-0">Nenhuma tarefa encontrada com os filtros atuais.</p>
            </div>
        </asp:PlaceHolder>
    </div>

    <uc:Paginacao ID="ucPaginacao" runat="server" OnPaginaAlterada="ucPaginacao_PaginaAlterada" />

</asp:Content>