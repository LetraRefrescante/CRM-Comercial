<%@ Page Title="Relatórios" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="Relatorios.aspx.cs" Inherits="CRM.Web.Paginas.Relatorios.Relatorios" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item active">Relatórios</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="crm-list-header"><h2>Relatórios</h2></div>
    <div class="row g-3">
        <asp:Repeater ID="rptRelatorios" runat="server">
            <ItemTemplate>
                <div class="col-md-4">
                    <a href='<%# Eval("Url") %>' class="text-decoration-none">
                        <div class="crm-form-card h-100">
                            <h5 class="mb-1"><%# Eval("Nome") %></h5>
                            <p class="text-muted small mb-0"><%# Eval("Descricao") %></p>
                        </div>
                    </a>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</asp:Content>