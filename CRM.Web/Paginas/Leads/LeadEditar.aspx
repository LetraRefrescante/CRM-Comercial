<%@ Page Title="Lead" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="LeadEditar.aspx.cs" Inherits="CRM.Web.Paginas.Leads.LeadEditar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item"><a href="~/Leads/LeadsLista.aspx" runat="server">Leads</a></li>
    <li class="breadcrumb-item active"><%: TituloPagina %></li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2><%: TituloPagina %></h2>

    <asp:PlaceHolder ID="phBloqueado" runat="server" Visible="false">
        <div class="alert alert-info">
            Este lead já foi convertido e está bloqueado para edição comercial.
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="phDuplicados" runat="server" Visible="false">
        <div class="alert alert-warning">
            <i class="fas fa-triangle-exclamation"></i>
            Já existe(m) lead(s) com o mesmo email ou telefone: <asp:Literal ID="litDuplicados" runat="server" />.
            <div class="form-check mt-2">
                <asp:CheckBox ID="chkConfirmarDuplicado" runat="server" CssClass="form-check-input" />
                <label class="form-check-label">Confirmo que quero gravar mesmo assim.</label>
            </div>
        </div>
    </asp:PlaceHolder>

    <div class="crm-form-card">
        <asp:ValidationSummary ID="valSummary" runat="server" CssClass="alert alert-danger" DisplayMode="BulletList" />

        <div class="row g-3">
            <div class="col-md-6">
                <label class="form-label">Nome *</label>
                <asp:TextBox ID="txtNome" runat="server" CssClass="form-control" MaxLength="150" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtNome" CssClass="text-danger small"
                    ErrorMessage="O nome é obrigatório." Display="Dynamic" />
            </div>
            <div class="col-md-6">
                <label class="form-label">Empresa</label>
                <asp:TextBox ID="txtEmpresa" runat="server" CssClass="form-control" MaxLength="150" />
            </div>

            <div class="col-md-6">
                <label class="form-label">Email</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" MaxLength="150" TextMode="Email" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="txtEmail" CssClass="text-danger small"
                    ErrorMessage="Formato de email inválido." Display="Dynamic"
                    ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$" />
            </div>
            <div class="col-md-6">
                <label class="form-label">Telefone</label>
                <asp:TextBox ID="txtTelefone" runat="server" CssClass="form-control" MaxLength="30" />
            </div>

            <div class="col-md-4">
                <label class="form-label">Origem *</label>
                <asp:DropDownList ID="ddlOrigem" runat="server" CssClass="form-select" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlOrigem" InitialValue=""
                    CssClass="text-danger small" ErrorMessage="A origem é obrigatória." Display="Dynamic" />
            </div>
            <div class="col-md-4">
                <label class="form-label">Estado *</label>
                <asp:DropDownList ID="ddlEstado" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlEstado_SelectedIndexChanged">
                    <asp:ListItem Text="Novo" Value="Novo" />
                    <asp:ListItem Text="Em Contacto" Value="Em Contacto" />
                    <asp:ListItem Text="Qualificado" Value="Qualificado" />
                    <asp:ListItem Text="Não Qualificado" Value="Não Qualificado" />
                    <asp:ListItem Text="Convertido" Value="Convertido" Enabled="false" />
                </asp:DropDownList>
            </div>
            <div class="col-md-4">
                <label class="form-label">Pontuação (0-100)</label>
                <asp:TextBox ID="txtPontuacao" runat="server" CssClass="form-control" TextMode="Number" />
                <asp:RangeValidator runat="server" ControlToValidate="txtPontuacao" CssClass="text-danger small"
                    ErrorMessage="A pontuação tem de estar entre 0 e 100." Display="Dynamic"
                    Type="Integer" MinimumValue="0" MaximumValue="100" />
            </div>

            <div class="col-md-4">
                <asp:Label ID="lblComercial" runat="server" CssClass="form-label" Text="Comercial *" AssociatedControlID="ddlComercial" />
                <asp:DropDownList ID="ddlComercial" runat="server" CssClass="form-select" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlComercial" InitialValue=""
                    CssClass="text-danger small" ErrorMessage="O comercial responsável é obrigatório." Display="Dynamic" />
            </div>
            <div class="col-md-4">
                <label class="form-label">Próximo Contacto</label>
                <asp:TextBox ID="txtProximoContacto" runat="server" CssClass="form-control" TextMode="DateTimeLocal" />
            </div>
            <div class="col-md-4" id="divMotivoPerda" runat="server">
                <label class="form-label">Motivo de Perda *</label>
                <asp:DropDownList ID="ddlMotivoPerda" runat="server" CssClass="form-select" />
            </div>
        </div>

        <asp:CustomValidator ID="cvRegrasNegocio" runat="server" Display="Dynamic" CssClass="text-danger small d-block mt-2"
            OnServerValidate="cvRegrasNegocio_ServerValidate" />

        <div class="d-flex gap-2 mt-4">
            <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-primary" OnClick="btnGuardar_Click" />
            <asp:HyperLink ID="lnkCancelar" runat="server" NavigateUrl="~/Leads/LeadsLista.aspx" CssClass="btn btn-outline-secondary">Cancelar</asp:HyperLink>
        </div>
    </div>

</asp:Content>