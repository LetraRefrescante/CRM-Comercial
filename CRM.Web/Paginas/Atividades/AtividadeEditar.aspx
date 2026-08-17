<%@ Page Title="Atividade" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="AtividadeEditar.aspx.cs" Inherits="CRM.Web.Paginas.Atividades.AtividadeEditar" %>
<%@ Register TagPrefix="uc" TagName="SeletorCliente" Src="~/Controls/SeletorCliente.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item"><a href="AtividadesLista.aspx">Atividades</a></li>
    <li class="breadcrumb-item active"><%= ActivityId.HasValue ? "Editar" : "Nova" %></li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2><%= ActivityId.HasValue ? "Editar Atividade" : "Nova Atividade" %></h2>

    <div class="crm-card mb-3">
        <h5 class="crm-card-title">Dados da Atividade</h5>
        <div class="row g-3">
            <div class="col-md-3">
                <label class="form-label">Tipo *</label>
                <asp:DropDownList ID="ddlTipo" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlTipo_SelectedIndexChanged">
                    <asp:ListItem Text="Chamada" Value="Chamada" />
                    <asp:ListItem Text="Email" Value="Email" />
                    <asp:ListItem Text="Reunião" Value="Reunião" />
                    <asp:ListItem Text="Visita" Value="Visita" />
                    <asp:ListItem Text="Nota" Value="Nota" />
                </asp:DropDownList>
            </div>
            <div class="col-md-9">
                <label class="form-label">Assunto *</label>
                <asp:TextBox ID="txtAssunto" runat="server" CssClass="form-control" MaxLength="180" />
            </div>

            <div class="col-md-3">
                <label class="form-label">Início *</label>
                <asp:TextBox ID="txtInicio" runat="server" CssClass="form-control" TextMode="DateTimeLocal" />
            </div>
            <div class="col-md-3">
                <label class="form-label">Fim</label>
                <asp:TextBox ID="txtFim" runat="server" CssClass="form-control" TextMode="DateTimeLocal" />
            </div>
            <div class="col-md-3">
                <label class="form-label">Prioridade</label>
                <asp:DropDownList ID="ddlPrioridade" runat="server" CssClass="form-select">
                    <asp:ListItem Text="—" Value="" />
                    <asp:ListItem Text="Baixa" Value="Baixa" />
                    <asp:ListItem Text="Normal" Value="Normal" />
                    <asp:ListItem Text="Alta" Value="Alta" />
                    <asp:ListItem Text="Urgente" Value="Urgente" />
                </asp:DropDownList>
            </div>
            <div class="col-md-3">
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
            <div class="col-md-4">
                <label class="form-label">Lembrete</label>
                <asp:TextBox ID="txtLembrete" runat="server" CssClass="form-control" TextMode="DateTimeLocal" />
            </div>

            <div class="col-12">
                <label class="form-label">Descrição</label>
                <asp:TextBox ID="txtDescricao" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" MaxLength="4000" />
            </div>
        </div>
    </div>

    <!-- ===================== Participantes (só Reunião) ===================== -->
    <asp:Panel ID="pnlParticipantes" runat="server" CssClass="crm-card mb-3" Visible="false">
        <h5 class="crm-card-title">Participantes</h5>

        <div class="row g-2 align-items-end">
            <div class="col-md-4">
                <label class="form-label small">Utilizador Interno</label>
                <asp:DropDownList ID="ddlParticipanteInterno" runat="server" CssClass="form-select" />
            </div>
            <div class="col-md-1 text-center text-muted small">ou</div>
            <div class="col-md-3">
                <label class="form-label small">Nome Externo</label>
                <asp:TextBox ID="txtParticipanteExternoNome" runat="server" CssClass="form-control" MaxLength="150" />
            </div>
            <div class="col-md-3">
                <label class="form-label small">Email Externo</label>
                <asp:TextBox ID="txtParticipanteExternoEmail" runat="server" CssClass="form-control" MaxLength="150" />
            </div>
            <div class="col-md-1">
                <asp:Button ID="btnAdicionarParticipante" runat="server" Text="+" CssClass="btn btn-outline-primary w-100"
                    OnClick="btnAdicionarParticipante_Click" CausesValidation="false" ToolTip="Adicionar participante" />
            </div>
        </div>

        <asp:Repeater ID="rptParticipantes" runat="server" OnItemCommand="rptParticipantes_ItemCommand">
            <HeaderTemplate>
                <ul class="list-group mt-3">
            </HeaderTemplate>
            <ItemTemplate>
                <li class="list-group-item d-flex justify-content-between align-items-center py-1">
                    <span>
                        <i class="fas <%# ((bool)Eval("EhInterno")) ? "fa-user" : "fa-user-clock text-muted" %>"></i>
                        <%# GetNomeParticipante(Container.DataItem) %>
                    </span>
                    <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-danger" CommandName="Remover"
                        CommandArgument="<%# Container.ItemIndex %>" CausesValidation="false">
                        <i class="fas fa-times"></i>
                    </asp:LinkButton>
                </li>
            </ItemTemplate>
            <FooterTemplate>
                </ul>
            </FooterTemplate>
        </asp:Repeater>

        <asp:PlaceHolder ID="phSemParticipantes" runat="server">
            <p class="text-muted small mt-2 mb-0">Ainda sem participantes adicionados.</p>
        </asp:PlaceHolder>
    </asp:Panel>

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
        <asp:HyperLink ID="lnkCancelar" runat="server" NavigateUrl="~/Atividades/AtividadesLista.aspx" CssClass="btn btn-outline-secondary">Cancelar</asp:HyperLink>
    </div>

</asp:Content>