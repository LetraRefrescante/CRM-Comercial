<%@ Page Title="Auditoria" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="Auditoria.aspx.cs" Inherits="CRM.Web.Paginas.Administracao.Auditoria" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item">Administração</li>
    <li class="breadcrumb-item active">Auditoria</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2 class="mb-4">Auditoria</h2>

    <div class="card p-3 mb-3">
        <div class="row g-2 align-items-end">
            <div class="col-md-3">
                <label class="form-label small text-muted">Utilizador</label>
                <asp:DropDownList ID="ddlUtilizador" runat="server" CssClass="form-select" />
            </div>
            <div class="col-md-2">
                <label class="form-label small text-muted">Ação</label>
                <asp:DropDownList ID="ddlAcao" runat="server" CssClass="form-select" />
            </div>
            <div class="col-md-2">
                <label class="form-label small text-muted">Entidade</label>
                <asp:TextBox ID="txtEntidade" runat="server" CssClass="form-control" placeholder="ex: Client" />
            </div>
            <div class="col-md-2">
                <label class="form-label small text-muted">De</label>
                <asp:TextBox ID="txtDataInicial" runat="server" CssClass="form-control" TextMode="Date" />
            </div>
            <div class="col-md-2">
                <label class="form-label small text-muted">Até</label>
                <asp:TextBox ID="txtDataFinal" runat="server" CssClass="form-control" TextMode="Date" />
            </div>
            <div class="col-md-1">
                <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="btn btn-primary w-100" OnClick="btnFiltrar_Click" />
            </div>
        </div>
    </div>

    <asp:Repeater ID="rptLogs" runat="server">
        <HeaderTemplate>
            <table class="table table-hover mb-0 align-middle">
                <thead>
                    <tr>
                        <th>Data</th>
                        <th>Utilizador</th>
                        <th>Ação</th>
                        <th>Entidade</th>
                        <th>Id</th>
                        <th>Detalhes</th>
                        <th>IP</th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td class="mono small"><%# Eval("CreatedDate", "{0:dd/MM/yyyy HH:mm}") %></td>
                <td><%# Eval("User.Name") ?? "Sistema" %></td>
                <td><%# Eval("Action") %></td>
                <td><%# Eval("EntityName") ?? "—" %></td>
                <td class="mono small"><%# Eval("EntityId") ?? "—" %></td>
                <td class="small"><%# Eval("Details") ?? "—" %></td>
                <td class="mono small"><%# Eval("IpAddress") ?? "—" %></td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
                </tbody>
            </table>
        </FooterTemplate>
    </asp:Repeater>

    <asp:PlaceHolder ID="phVazio" runat="server" Visible="false">
        <div class="crm-empty-state">
            <p class="mb-0">Nenhum registo de auditoria encontrado.</p>
        </div>
    </asp:PlaceHolder>

    <uc:Paginacao ID="ucPaginacao" runat="server" OnPaginaAlterada="ucPaginacao_PaginaAlterada" />

</asp:Content>