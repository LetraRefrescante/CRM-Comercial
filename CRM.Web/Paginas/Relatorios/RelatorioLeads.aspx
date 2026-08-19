<%@ Page Title="Relatório de Leads" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="RelatorioLeads.aspx.cs" Inherits="CRM.Web.Paginas.Relatorios.RelatorioLeads" %>
<%@ Register TagPrefix="uc" TagName="FiltroDatas" Src="~/Controls/FiltroDatas.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item"><a href="Relatorios.aspx">Relatórios</a></li>
    <li class="breadcrumb-item active">Leads</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<style media="print">
    .crm-filter-card, .app-header, .crm-sidebar, .app-footer, #btnImprimir, #btnExportarCsv { display: none !important; }
</style>

    <div class="crm-list-header"><h2>Relatório de Leads</h2></div>

    <div class="crm-filter-card">
        <div class="row g-2 align-items-end">
            <div class="col-md-3">
                <label class="form-label">Período (Criação)</label>
                <uc:FiltroDatas ID="ucFiltroDatas" runat="server" />
            </div>
            <div class="col-md-2">
                <label class="form-label">Origem</label>
                <asp:DropDownList ID="ddlOrigem" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Todas" Value="" />
                </asp:DropDownList>
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
                <label class="form-label">Comercial</label>
                <asp:DropDownList ID="ddlComercial" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Todos" Value="" />
                </asp:DropDownList>
            </div>
            <div class="col-md-1">
                <label class="form-label">Pont. Mín.</label>
                <asp:TextBox ID="txtScoreMin" runat="server" CssClass="form-control" TextMode="Number" />
            </div>
            <div class="col-md-1">
                <label class="form-label">Pont. Máx.</label>
                <asp:TextBox ID="txtScoreMax" runat="server" CssClass="form-control" TextMode="Number" />
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
                <thead><tr><th>Origem</th><th class="text-end">Quantidade</th><th class="text-end">Convertidos</th><th class="text-end">Taxa de Conversão</th></tr></thead>
                <tbody>
                    <asp:Repeater ID="rptLinhas" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("Origem") %></td>
                                <td class="text-end"><%# Eval("Quantidade") %></td>
                                <td class="text-end"><%# Eval("Convertidos") %></td>
                                <td class="text-end fw-semibold"><%# Eval("TaxaConversao") %>%</td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
                <tfoot>
                    <tr class="fw-bold">
                        <td>Total Geral</td>
                        <td class="text-end"><asp:Literal ID="litQuantidadeGeral" runat="server" /></td>
                        <td class="text-end"><asp:Literal ID="litConvertidosGeral" runat="server" /></td>
                        <td class="text-end"><asp:Literal ID="litTaxaGeral" runat="server" /></td>
                    </tr>
                </tfoot>
            </table>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="phVazio" runat="server" Visible="false">
        <div class="crm-empty-state mt-3"><p class="mb-0">Sem leads nos filtros selecionados.</p></div>
    </asp:PlaceHolder>

</asp:Content>