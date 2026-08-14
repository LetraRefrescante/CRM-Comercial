<%@ Page Title="Tarefa" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="TarefaEditar.aspx.cs" Inherits="CRM.Web.Paginas.Atividades.TarefaEditar" %>
<%@ Register TagPrefix="uc" TagName="SeletorCliente" Src="~/Controls/SeletorCliente.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item"><a href="TarefasLista.aspx">Tarefas</a></li>
    <li class="breadcrumb-item active"><%= TaskId.HasValue ? "Editar" : "Nova" %></li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2><%= TaskId.HasValue ? "Editar Tarefa" : "Nova Tarefa" %></h2>

    <div class="crm-card mb-3">
        <h5 class="crm-card-title">Dados da Tarefa</h5>
        <div class="row g-3">
            <div class="col-md-8">
                <label class="form-label">Assunto *</label>
                <asp:TextBox ID="txtAssunto" runat="server" CssClass="form-control" MaxLength="180" />
            </div>
            <div class="col-md-4">
                <label class="form-label">Data Limite *</label>
                <asp:TextBox ID="txtDataLimite" runat="server" CssClass="form-control" TextMode="DateTimeLocal" />
            </div>

            <div class="col-md-4">
                <label class="form-label">Prioridade</label>
                <asp:DropDownList ID="ddlPrioridade" runat="server" CssClass="form-select">
                    <asp:ListItem Text="—" Value="" />
                    <asp:ListItem Text="Baixa" Value="Baixa" />
                    <asp:ListItem Text="Normal" Value="Normal" />
                    <asp:ListItem Text="Alta" Value="Alta" />
                    <asp:ListItem Text="Urgente" Value="Urgente" />
                </asp:DropDownList>
            </div>
            <div class="col-md-4">
                <label class="form-label">Estado *</label>
                <asp:DropDownList ID="ddlEstado" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Planeada" Value="Planeada" />
                    <asp:ListItem Text="Em Curso" Value="Em Curso" />
                    <asp:ListItem Text="Concluída" Value="Concluída" />
                    <asp:ListItem Text="Cancelada" Value="Cancelada" />
                </asp:DropDownList>
            </div>
            <div class="col-md-4">
                <label class="form-label">Responsável *</label>
                <asp:DropDownList ID="ddlResponsavel" runat="server" CssClass="form-select" />
            </div>

            <div class="col-12">
                <label class="form-label">Descrição</label>
                <asp:TextBox ID="txtDescricao" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" MaxLength="4000" />
            </div>
        </div>
    </div>

    <div class="crm-card mb-3">
        <h5 class="crm-card-title">Relacionado Com</h5>
        <div class="row g-3">
            <div class="col-md-3">
                <label class="form-label">Tipo</label>
                <asp:DropDownList ID="ddlTipoRelacao" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlTipoRelacao_SelectedIndexChanged">
                    <asp:ListItem Text="Nenhum" Value="" />
                    <asp:ListItem Text="Cliente" Value="Cliente" />
                    <asp:ListItem Text="Lead" Value="Lead" />
                </asp:DropDownList>
            </div>
            <asp:Panel ID="pnlCliente" runat="server" CssClass="col-md-6" Visible="false">
                <label class="form-label">Cliente</label>
                <uc:SeletorCliente ID="ucCliente" runat="server" />
            </asp:Panel>
            <asp:Panel ID="pnlLead" runat="server" CssClass="col-md-6" Visible="false">
                <label class="form-label">Lead</label>
                <asp:DropDownList ID="ddlLead" runat="server" CssClass="form-select" />
            </asp:Panel>
        </div>
    </div>

    <div class="crm-form-actions">
        <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-primary" OnClick="btnGuardar_Click" />
        <asp:HyperLink ID="lnkCancelar" runat="server" NavigateUrl="~/Atividades/TarefasLista.aspx" CssClass="btn btn-outline-secondary">Cancelar</asp:HyperLink>
    </div>

</asp:Content>