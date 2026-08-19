<%@ Page Title="Relatório de Comissões" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="RelatorioComissoes.aspx.cs" Inherits="CRM.Web.Paginas.Relatorios.RelatorioComissoes" %>
<%@ Register TagPrefix="uc" TagName="FiltroDatas" Src="~/Controls/FiltroDatas.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item"><a href="Relatorios.aspx">Relatórios</a></li>
    <li class="breadcrumb-item active">Comissões</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<style media="print">
    .crm-filter-card, .app-header, .crm-sidebar, .app-footer, #btnImprimir, #btnExportarCsv { display: none !important; }
</style>

    <div class="crm-list-header"><h2>Relatório de Comissões</h2></div>
    <p class="text-muted small">Considera apenas vendas com estado "Concluída".</p>

    <div class="crm-filter-card">
        <div class="row g-2 align-items-end">
            <div class="col-md-4">
                <label class="form-label">Período *</label>
                <uc:FiltroDatas ID="ucFiltroDatas" runat="server" />
            </div>
            <div class="col-md-4">
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
                <thead><tr><th>Comercial</th><th class="text-end">Nº Vendas</th><th class="text-end">Total Vendas</th><th class="text-end">Total Comissão</th></tr></thead>
                <tbody>
                    <asp:Repeater ID="rptLinhas" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("Comercial") %></td>
                                <td class="text-end"><%# Eval("QuantidadeVendas") %></td>
                                <td class="text-end"><%# Eval("TotalVendas", "{0:C}") %></td>
                                <td class="text-end fw-semibold"><%# Eval("TotalComissao", "{0:C}") %></td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
                <tfoot>
                    <tr class="fw-bold">
                        <td>Total Geral</td>
                        <td class="text-end"><asp:Literal ID="litQuantidadeGeral" runat="server" /></td>
                        <td class="text-end"><asp:Literal ID="litVendasGeral" runat="server" /></td>
                        <td class="text-end"><asp:Literal ID="litComissaoGeral" runat="server" /></td>
                    </tr>
                </tfoot>
            </table>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="phVazio" runat="server" Visible="false">
        <div class="crm-empty-state mt-3"><p class="mb-0">Sem vendas concluídas no período/filtros selecionados.</p></div>
    </asp:PlaceHolder>

</asp:Content>