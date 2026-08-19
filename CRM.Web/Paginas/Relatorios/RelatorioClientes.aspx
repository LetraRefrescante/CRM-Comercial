<%@ Page Title="Relatório de Clientes" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="RelatorioClientes.aspx.cs" Inherits="CRM.Web.Paginas.Relatorios.RelatorioClientes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item"><a href="Relatorios.aspx">Relatórios</a></li>
    <li class="breadcrumb-item active">Clientes</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<style media="print">
    .crm-filter-card, .app-header, .crm-sidebar, .app-footer, #btnImprimir, #btnExportarCsv { display: none !important; }
</style>

    <div class="crm-list-header"><h2>Relatório de Clientes (Carteira e Segmentação)</h2></div>

    <div class="crm-filter-card">
        <div class="row g-2 align-items-end">
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
                <label class="form-label">Comercial</label>
                <asp:DropDownList ID="ddlComercial" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Todos" Value="" />
                </asp:DropDownList>
            </div>
        </div>
        <div class="mt-2 d-flex gap-2">
            <asp:Button ID="btnFiltrar" runat="server" Text="Gerar Relatório" CssClass="btn btn-primary" OnClick="btnFiltrar_Click" />
            <asp:Button ID="btnExportarCsv" runat="server" Text="Exportar (Excel/CSV)" CssClass="btn btn-outline-secondary" OnClick="btnExportarCsv_Click" CausesValidation="false" />
            <button id="btnImprimir" type="button" class="btn btn-outline-secondary" onclick="window.print();">Imprimir / PDF</button>
        </div>
    </div>

    <asp:PlaceHolder ID="phResultado" runat="server" Visible="false">
        <div class="crm-table-card mt-3">
            <table class="table table-hover mb-0 align-middle">
                <thead><tr><th>Setor</th><th class="text-end">Total</th><th class="text-end">Potenciais</th><th class="text-end">Ativos</th><th class="text-end">Inativos</th><th class="text-end">Bloqueados</th></tr></thead>
                <tbody>
                    <asp:Repeater ID="rptLinhas" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("Setor") %></td>
                                <td class="text-end fw-semibold"><%# Eval("Total") %></td>
                                <td class="text-end"><%# Eval("Potenciais") %></td>
                                <td class="text-end"><%# Eval("Ativos") %></td>
                                <td class="text-end"><%# Eval("Inativos") %></td>
                                <td class="text-end"><%# Eval("Bloqueados") %></td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
                <tfoot>
                    <tr class="fw-bold">
                        <td>Total Geral</td>
                        <td class="text-end" colspan="5"><asp:Literal ID="litTotalGeral" runat="server" /></td>
                    </tr>
                </tfoot>
            </table>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="phVazio" runat="server" Visible="false">
        <div class="crm-empty-state mt-3"><p class="mb-0">Sem clientes nos filtros selecionados.</p></div>
    </asp:PlaceHolder>

</asp:Content>