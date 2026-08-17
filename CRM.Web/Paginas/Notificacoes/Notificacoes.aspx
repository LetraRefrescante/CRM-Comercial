<%@ Page Title="Notificações" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="Notificacoes.aspx.cs" Inherits="CRM.Web.Paginas.Notificacoes.Notificacoes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item active">Notificações</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<style>
    .crm-notification-item { display:flex; justify-content:space-between; align-items:flex-start; gap:12px; padding:12px 16px; border-bottom:1px solid #eee; }
    .crm-notification-item:last-child { border-bottom:none; }
    .crm-notification-nao-lida { background:#f5fbf8; border-left:3px solid #1F7A5C; }
    .crm-notification-acoes { display:flex; gap:6px; flex-shrink:0; white-space:nowrap; }
</style>

    <div class="crm-list-header">
        <h2>Notificações</h2>
    </div>

    <div class="crm-filter-card">
        <div class="row g-2 align-items-end">
            <div class="col-md-3">
                <label class="form-label">Mostrar</label>
                <asp:DropDownList ID="ddlFiltro" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlFiltro_SelectedIndexChanged">
                    <asp:ListItem Text="Não lidas" Value="NaoLidas" />
                    <asp:ListItem Text="Todas (exceto arquivadas)" Value="Todas" />
                    <asp:ListItem Text="Incluir arquivadas" Value="ComArquivadas" />
                </asp:DropDownList>
            </div>
        </div>
    </div>

    <div class="crm-table-card">
        <asp:Repeater ID="rptNotificacoes" runat="server" OnItemCommand="rptNotificacoes_ItemCommand">
            <ItemTemplate>
                <div class="crm-notification-item <%# GetLidaClasse(Container.DataItem) %>">
                    <div>
                        <div class="fw-semibold"><%# Eval("Title") %></div>
                        <div class="text-muted small"><%# Eval("Message") %></div>
                        <div class="text-muted small"><%# Eval("CreatedDate", "{0:dd/MM/yyyy HH:mm}") %></div>
                    </div>
                    <div class="crm-notification-acoes">
                        <asp:HyperLink runat="server" CssClass="btn btn-sm btn-outline-secondary"
                            NavigateUrl='<%# GetUrlRelacionada(Container.DataItem) %>'
                            Visible='<%# GetTemUrlRelacionada(Container.DataItem) %>'>Ver</asp:HyperLink>
                        <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-primary"
                            CommandName="MarcarLida" CommandArgument='<%# Eval("NotificationId") %>'
                            Visible='<%# GetNaoLida(Container.DataItem) %>'>Marcar como lida</asp:LinkButton>
                        <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-secondary"
                            CommandName="Arquivar" CommandArgument='<%# Eval("NotificationId") %>'
                            Visible='<%# GetNaoArquivada(Container.DataItem) %>'>Arquivar</asp:LinkButton>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <asp:PlaceHolder ID="phVazio" runat="server" Visible="false">
            <div class="crm-empty-state">
                <i class="fas fa-bell-slash"></i>
                <p class="mb-0">Sem notificações para mostrar.</p>
            </div>
        </asp:PlaceHolder>
    </div>

</asp:Content>