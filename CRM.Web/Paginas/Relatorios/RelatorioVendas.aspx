<%@ Page Title="Relatório de Vendas" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="RelatorioVendas.aspx.cs" Inherits="CRM.Web.Paginas.Relatorios.RelatorioVendas" %>
<%@ Register TagPrefix="uc" TagName="FiltroDatas" Src="~/Controls/FiltroDatas.ascx" %>
<%@ Register TagPrefix="uc" TagName="SeletorCliente" Src="~/Controls/SeletorCliente.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item"><a href="Relatorios.aspx">Relatórios</a></li>
    <li class="breadcrumb-item active">Vendas</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<style media="print">
    .crm-filter-card, .app-header, .crm-sidebar, .app-footer, .crm-row-actions, #btnImprimir, #btnExportarCsv { display: none !important; }
</style>

    <div class="crm-list-header"><h2>Relatório de Vendas</h2></div>

    <div class="crm-filter-card">
        <div class="row g-2 align-items-end">
            <div class="col-md-3">
                <label class="form-label">Período *</label>
                <uc:FiltroDatas ID="ucFiltroDatas" runat="server" />
            </div>
            <div class="col-md-2">
                <label class="form-label">Comercial</label>
                <asp:DropDownList ID="ddlComercial" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Todos" Value="" />
                </asp:DropDownList>
            </div>
            <div class="col-md-3">
                <label class="form-label">Cliente</label>
                <uc:SeletorCliente ID="ucCliente" runat="server" />
            </div>
            <div class="col-md-2">
                <label class="form-label">Agrupamento</label>
                <asp:DropDownList ID="ddlAgrupamento" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Mês" Value="Mes" Selected="True" />
                    <asp:ListItem Text="Dia" Value="Dia" />
                    <asp:ListItem Text="Trimestre" Value="Trimestre" />
                    <asp:ListItem Text="Ano" Value="Ano" />
                </asp:DropDownList>
            </div>
        </div>
        <div class="row g-2 mt-1">
            <div class="col-12">
                <label class="form-label">Estado</label><br />
                <asp:CheckBoxList ID="cblEstados" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="d-inline">
                    <asp:ListItem Text="Pendente" Value="Pendente" />
                    <asp:ListItem Text="Confirmada" Value="Confirmada" />
                    <asp:ListItem Text="Parcial" Value="Parcial" />
                    <asp:ListItem Text="Concluída" Value="Concluída" />
                    <asp:ListItem Text="Cancelada" Value="Cancelada" />
                </asp:CheckBoxList>
                <div class="form-text">Nenhuma marcada = todos os estados.</div>
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
                <thead>
                    <tr><th>Período</th><th class="text-end">Nº Vendas</th><th class="text-end">Subtotal</th><th class="text-end">IVA</th><th class="text-end">Total</th></tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptLinhas" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("Periodo") %></td>
                                <td class="text-end"><%# Eval("Quantidade") %></td>
                                <td class="text-end"><%# Eval("SubTotal", "{0:C}") %></td>
                                <td class="text-end"><%# Eval("TaxTotal", "{0:C}") %></td>
                                <td class="text-end fw-semibold"><%# Eval("Total", "{0:C}") %></td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
                <tfoot>
                    <tr class="fw-bold">
                        <td>Total Geral</td>
                        <td class="text-end"><asp:Literal ID="litQuantidadeGeral" runat="server" /></td>
                        <td colspan="2"></td>
                        <td class="text-end"><asp:Literal ID="litTotalGeral" runat="server" /></td>
                    </tr>
                </tfoot>
            </table>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="phVazio" runat="server" Visible="false">
        <div class="crm-empty-state mt-3"><p class="mb-0">Sem vendas no período/filtros selecionados.</p></div>
    </asp:PlaceHolder>

</asp:Content>