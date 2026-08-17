<%@ Page Title="Modelos de Email" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="TemplatesEmail.aspx.cs" Inherits="CRM.Web.Paginas.Notificacoes.TemplatesEmail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item">Administração</li>
    <li class="breadcrumb-item active">Modelos de Email</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="crm-list-header">
        <h2>Modelos de Email</h2>
        <asp:Button ID="btnNovo" runat="server" Text="Novo Modelo" CssClass="btn btn-primary" OnClick="btnNovo_Click" CausesValidation="false" />
    </div>

    <div class="crm-table-card mb-4">
        <asp:Repeater ID="rptTemplates" runat="server" OnItemCommand="rptTemplates_ItemCommand">
            <HeaderTemplate>
                <table class="table table-hover mb-0 align-middle">
                    <thead><tr><th>Nome</th><th>Assunto</th><th>Estado</th><th class="text-end">Ações</th></tr></thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# Eval("Name") %></td>
                    <td><%# Eval("Subject") %></td>
                    <td><span class="badge <%# GetEstadoBadgeClasse(Eval("IsActive")) %>"><%# GetEstadoTexto(Eval("IsActive")) %></span></td>
                    <td class="text-end crm-row-actions">
                        <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-secondary" ToolTip="Editar"
                            CommandName="Editar" CommandArgument='<%# Eval("EmailTemplateId") %>'>
                            <i class="fas fa-pen"></i>
                        </asp:LinkButton>
                        <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-secondary" ToolTip="Ativar/Desativar"
                            CommandName="AlternarEstado" CommandArgument='<%# Eval("EmailTemplateId") %>'>
                            <i class="fas fa-power-off"></i>
                        </asp:LinkButton>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate></tbody></table></FooterTemplate>
        </asp:Repeater>

        <asp:PlaceHolder ID="phVazio" runat="server" Visible="false">
            <div class="crm-empty-state"><p class="mb-0 text-muted small p-3">Ainda não existem modelos de email.</p></div>
        </asp:PlaceHolder>
    </div>

    <asp:Panel ID="pnlFormulario" runat="server" CssClass="crm-card" Visible="false">
        <h5 class="crm-card-title"><asp:Literal ID="litTituloFormulario" runat="server" /></h5>
        <div class="row g-3">
            <div class="col-md-6">
                <label class="form-label">Nome *</label>
                <asp:TextBox ID="txtNome" runat="server" CssClass="form-control" MaxLength="150" />
            </div>
            <div class="col-md-6">
                <label class="form-label">Assunto *</label>
                <asp:TextBox ID="txtAssunto" runat="server" CssClass="form-control" MaxLength="200" />
            </div>
            <div class="col-12">
                <label class="form-label">Corpo *</label>
                <asp:TextBox ID="txtCorpo" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="8" />
                <small class="text-muted">Variáveis suportadas: {{ClientName}}, {{LeadName}}, {{OpportunityTitle}}, {{ProposalNumber}}, {{ProposalTotal}}, {{ContactName}}.</small>
            </div>
        </div>
        <div class="mt-3 d-flex gap-2">
            <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-primary" OnClick="btnGuardar_Click" />
            <asp:Button ID="btnCancelarEdicao" runat="server" Text="Cancelar" CssClass="btn btn-outline-secondary" OnClick="btnCancelarEdicao_Click" CausesValidation="false" />
        </div>
    </asp:Panel>

</asp:Content>