<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Historico.ascx.cs" Inherits="CRM.Web.Controls.Historico" %>
<div class="crm-historico">
    <asp:Repeater ID="rptHistorico" runat="server">
        <HeaderTemplate><ul class="list-group"></HeaderTemplate>
        <ItemTemplate>
            <li class="list-group-item d-flex justify-content-between align-items-start">
                <div>
                    <span class="fw-semibold"><%# Eval("Action") %></span>
                    <div class="text-muted small"><%# Eval("NomeUtilizador") %></div>
                </div>
                <span class="text-muted small text-nowrap"><%# Eval("CreatedDate", "{0:dd/MM/yyyy HH:mm}") %></span>
            </li>
        </ItemTemplate>
        <FooterTemplate></ul></FooterTemplate>
    </asp:Repeater>
    <asp:PlaceHolder ID="phVazio" runat="server" Visible="false">
        <p class="text-muted text-center mb-0">Ainda não existem registos de atividade.</p>
    </asp:PlaceHolder>
</div>