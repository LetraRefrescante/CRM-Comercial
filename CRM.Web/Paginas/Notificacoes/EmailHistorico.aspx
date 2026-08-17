<%@ Page Title="Histórico de Emails" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="EmailHistorico.aspx.cs" Inherits="CRM.Web.Paginas.Notificacoes.EmailHistorico" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item active">Histórico de Emails</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="crm-list-header">
        <h2>Histórico de Emails</h2>
    </div>

    <div class="crm-filter-card">
        <div class="row g-2 align-items-end">
            <div class="col-md-4">
                <label class="form-label">Pesquisar</label>
                <asp:TextBox ID="txtPesquisa" runat="server" CssClass="form-control" placeholder="Destinatário ou assunto..." />
            </div>
            <div class="col-md-2">
                <label class="form-label">Estado</label>
                <asp:DropDownList ID="ddlEstado" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Todos" Value="" />
                    <asp:ListItem Text="Enviado" Value="Enviado" />
                    <asp:ListItem Text="Falhou" Value="Falhou" />
                </asp:DropDownList>
            </div>
            <div class="col-md-4">
                <label class="form-label">Período</label>
                <uc:FiltroDatas ID="ucFiltroDatas" runat="server" />
            </div>
            <div class="col-md-2">
                <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="btn btn-outline-secondary w-100" OnClick="btnFiltrar_Click" />
            </div>
        </div>
    </div>

    <div class="crm-table-card">
        <asp:Repeater ID="rptHistorico" runat="server">
            <HeaderTemplate>
                <table class="table table-hover mb-0 align-middle">
                    <thead>
                        <tr>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="SentDate" OnCommand="lnkOrdenar_Command">Data</asp:LinkButton></th>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="ToAddress" OnCommand="lnkOrdenar_Command">Destinatário</asp:LinkButton></th>
                            <th>Assunto</th>
                            <th>Estado</th>
                            <th>Motivo da Falha</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# Eval("SentDate", "{0:dd/MM/yyyy HH:mm}") %></td>
                    <td><%# Eval("ToAddress") %></td>
                    <td><%# Eval("Subject") %></td>
                    <td><span class="badge <%# GetBadgeClasse(Eval("Status").ToString()) %>"><%# Eval("Status") %></span></td>
                    <td class="text-muted small"><%# Eval("FailureReason") %></td>
                </tr>
            </ItemTemplate>
            <FooterTemplate></tbody></table></FooterTemplate>
        </asp:Repeater>

        <asp:PlaceHolder ID="phVazio" runat="server" Visible="false">
            <div class="crm-empty-state"><p class="mb-0 text-muted small p-3">Nenhum email encontrado com os filtros atuais.</p></div>
        </asp:PlaceHolder>
    </div>

    <uc:Paginacao ID="ucPaginacao" runat="server" OnPaginaAlterada="ucPaginacao_PaginaAlterada" />

</asp:Content>