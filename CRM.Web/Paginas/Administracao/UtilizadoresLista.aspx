<%@ Page Title="Utilizadores" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="UtilizadoresLista.aspx.cs" Inherits="CRM.Web.Paginas.Administracao.UtilizadoresLista" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item">Administração</li>
    <li class="breadcrumb-item active">Utilizadores</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="d-flex justify-content-between align-items-center mb-3">
        <h2 class="mb-0" style="font-family:'Sora',sans-serif;">Utilizadores</h2>
        <asp:HyperLink ID="lnkNovo" runat="server" NavigateUrl="~/Administracao/UtilizadorEditar.aspx" CssClass="btn btn-success">
            <i class="fas fa-plus"></i> Novo Utilizador
        </asp:HyperLink>
    </div>

    <div class="card p-3 mb-3">
        <div class="row g-2 align-items-end">
            <div class="col-md-4">
                <label class="form-label small text-muted">Pesquisar</label>
                <asp:TextBox ID="txtPesquisa" runat="server" CssClass="form-control" placeholder="Nome ou email..." />
            </div>
            <div class="col-md-3">
                <label class="form-label small text-muted">Perfil</label>
                <asp:DropDownList ID="ddlPerfil" runat="server" CssClass="form-select" />
            </div>
            <div class="col-md-3">
                <label class="form-label small text-muted">Estado</label>
                <asp:DropDownList ID="ddlEstado" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Todos" Value="" />
                    <asp:ListItem Text="Ativo" Value="Ativo" />
                    <asp:ListItem Text="Bloqueado" Value="Bloqueado" />
                    <asp:ListItem Text="Inativo" Value="Inativo" />
                </asp:DropDownList>
            </div>
            <div class="col-md-2">
                <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="btn btn-outline-secondary w-100" OnClick="btnFiltrar_Click" />
            </div>
        </div>
    </div>

    <div class="card p-0">
        <asp:Repeater ID="rptUtilizadores" runat="server" OnItemCommand="rptUtilizadores_ItemCommand" OnItemDataBound="rptUtilizadores_ItemDataBound">
            <HeaderTemplate>
                <table class="table table-hover mb-0 align-middle">
                    <thead class="table-light">
                        <tr>
                            <th>Nome</th>
                            <th>Email</th>
                            <th>Perfil</th>
                            <th>Estado</th>
                            <th>Último login</th>
                            <th class="text-end">Ações</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# Eval("Name") %></td>
                    <td><%# Eval("Email") %></td>
                    <td><%# Eval("Role.Name") %></td>
                    <td>
                        <span class="badge <%# GetBadgeClasse(Eval("Status").ToString()) %>">
                            <%# Eval("Status") %>
                        </span>
                    </td>
                    <td>
                        <%# Eval("LastLoginDate") == null ? "Nunca" : Eval("LastLoginDate", "{0:dd/MM/yyyy HH:mm}") %>
                    </td>
                    <td class="text-end">
                        <asp:PlaceHolder ID="phAcoesGestao" runat="server">
                            <a href="UtilizadorEditar.aspx?id=<%# Eval("UserId") %>" class="btn btn-sm btn-outline-secondary" title="Editar">
                                <i class="fas fa-pen"></i>
                            </a>

                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-warning" ToolTip="Bloquear"
                                CommandName="Bloquear" CommandArgument='<%# Eval("UserId") %>'
                                Visible='<%# Eval("Status").ToString() == "Ativo" %>'
                                data-confirm='<%# "Bloquear o utilizador " + Eval("Name") + "? Vai perder acesso imediato ao sistema." %>'>
                                <i class="fas fa-lock"></i>
                            </asp:LinkButton>

                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-success" ToolTip="Ativar"
                                CommandName="Ativar" CommandArgument='<%# Eval("UserId") %>'
                                Visible='<%# Eval("Status").ToString() != "Ativo" %>'>
                                <i class="fas fa-lock-open"></i>
                            </asp:LinkButton>

                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-danger" ToolTip="Eliminar"
                                CommandName="Eliminar" CommandArgument='<%# Eval("UserId") %>'
                                data-confirm='<%# "Eliminar o utilizador " + Eval("Name") + "? Esta ação não pode ser anulada." %>'>
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
            <div class="text-center text-muted py-5">
                <i class="fas fa-users-slash mb-2" style="font-size:2rem;"></i>
                <p class="mb-0">Nenhum utilizador encontrado com os filtros atuais.</p>
            </div>
        </asp:PlaceHolder>
    </div>

</asp:Content>