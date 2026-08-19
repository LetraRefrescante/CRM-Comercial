<%@ Page Title="Relatório de Atividades" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="RelatorioAtividades.aspx.cs" Inherits="CRM.Web.Paginas.Relatorios.RelatorioAtividades" %>
<%@ Register TagPrefix="uc" TagName="FiltroDatas" Src="~/Controls/FiltroDatas.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item"><a href="Relatorios.aspx">Relatórios</a></li>
    <li class="breadcrumb-item active">Atividades</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<style media="print">
    .crm-filter-card, .app-header, .crm-sidebar, .app-footer, #btnImprimir, #btnExportarCsv { display: none !important; }
</style>

    <div class="crm-list-header"><h2>Relatório de Atividades (Produtividade Comercial)</h2></div>

    <div class="crm-filter-card">
        <div class="row g-2 align-items-end">
            <div class="col-md-3">
                <label class="form-label">Período (Início)</label>
                <uc:FiltroDatas ID="ucFiltroDatas" runat="server" />
            </div>
            <div class="col-md-3">
                <label class="form-label">Responsável</label>
                <asp:DropDownList ID="ddlResponsavel" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Todos" Value="" />
                </asp:DropDownList>
            </div>
            <div class="col-md-3">
                <label class="form-label">Tipo</label>
                <asp:DropDownList ID="ddlTipo" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Todos" Value="" />
                    <asp:ListItem Text="Chamada" Value="Chamada" />
                    <asp:ListItem Text="Email" Value="Email" />
                    <asp:ListItem Text="Reunião" Value="Reunião" />
                    <asp:ListItem Text="Visita" Value="Visita" />
                    <asp:ListItem Text="Nota" Value="Nota" />
                </asp:DropDownList>
            </div>
            <div class="col-md-3">
                <label class="form-label">Estado</label>
                <asp:DropDownList ID="ddlEstado" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Todos" Value="" />
                    <asp:ListItem Text="Planeada" Value="Planeada" />
                    <asp:ListItem Text="Em Curso" Value="Em Curso" />
                    <asp:ListItem Text="Concluída" Value="Concluída" />
                    <asp:ListItem Text="Cancelada" Value="Cancelada" />
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
                <thead><tr><th>Responsável</th><th class="text-end">Total</th><th class="text-end">Planeadas</th><th class="text-end">Em Curso</th><th class="text-end">Concluídas</th><th class="text-end">Canceladas</th></tr></thead>
                <tbody>
                    <asp:Repeater ID="rptLinhas" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("Responsavel") %></td>
                                <td class="text-end fw-semibold"><%# Eval("Total") %></td>
                                <td class="text-end"><%# Eval("Planeadas") %></td>
                                <td class="text-end"><%# Eval("EmCurso") %></td>
                                <td class="text-end"><%# Eval("Concluidas") %></td>
                                <td class="text-end"><%# Eval("Canceladas") %></td>
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
        <div class="crm-empty-state mt-3"><p class="mb-0">Sem atividades nos filtros selecionados.</p></div>
    </asp:PlaceHolder>

</asp:Content>