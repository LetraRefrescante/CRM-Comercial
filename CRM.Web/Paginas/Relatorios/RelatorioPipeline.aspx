<%@ Page Title="Relatório de Pipeline" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="RelatorioPipeline.aspx.cs" Inherits="CRM.Web.Paginas.Relatorios.RelatorioPipeline" %>
<%@ Register TagPrefix="uc" TagName="FiltroDatas" Src="~/Controls/FiltroDatas.ascx" %>
<%@ Register TagPrefix="uc" TagName="SeletorCliente" Src="~/Controls/SeletorCliente.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item"><a href="Relatorios.aspx">Relatórios</a></li>
    <li class="breadcrumb-item active">Pipeline</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<style media="print">
    .crm-filter-card, .app-header, .crm-sidebar, .app-footer, #btnImprimir, #btnExportarCsv { display: none !important; }
</style>

    <div class="crm-list-header"><h2>Relatório de Pipeline</h2></div>

    <div class="crm-filter-card">
        <div class="row g-2 align-items-end">
            <div class="col-md-3">
                <label class="form-label">Fecho Previsto (De/Até)</label>
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
                <label class="form-label">Estado</label>
                <asp:DropDownList ID="ddlEstado" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Só Abertas" Value="aberta" Selected="True" />
                    <asp:ListItem Text="Todas" Value="" />
                    <asp:ListItem Text="Só Fechadas" Value="fechada" />
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
                <thead><tr><th>Fase</th><th class="text-end">Nº Oportunidades</th><th class="text-end">Valor Estimado</th><th class="text-end">Valor Ponderado</th></tr></thead>
                <tbody>
                    <asp:Repeater ID="rptLinhas" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("Fase") %></td>
                                <td class="text-end"><%# Eval("Quantidade") %></td>
                                <td class="text-end"><%# Eval("ValorTotal", "{0:C}") %></td>
                                <td class="text-end fw-semibold"><%# Eval("ValorPonderado", "{0:C}") %></td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
                <tfoot>
                    <tr class="fw-bold">
                        <td>Total Geral</td>
                        <td class="text-end"><asp:Literal ID="litQuantidadeGeral" runat="server" /></td>
                        <td class="text-end"><asp:Literal ID="litValorGeral" runat="server" /></td>
                        <td class="text-end"><asp:Literal ID="litValorPonderadoGeral" runat="server" /></td>
                    </tr>
                </tfoot>
            </table>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="phVazio" runat="server" Visible="false">
        <div class="crm-empty-state mt-3"><p class="mb-0">Sem oportunidades nos filtros selecionados.</p></div>
    </asp:PlaceHolder>

</asp:Content>