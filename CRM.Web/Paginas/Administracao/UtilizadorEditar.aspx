<%@ Page Title="Utilizador" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="UtilizadorEditar.aspx.cs" Inherits="CRM.Web.Paginas.Administracao.UtilizadorEditar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item"><a href="~/Administracao/UtilizadoresLista.aspx" runat="server">Utilizadores</a></li>
    <li class="breadcrumb-item active"><asp:Literal ID="litTituloBreadcrumb" runat="server" /></li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2 class="mb-4" style="font-family:'Sora',sans-serif;">
        <asp:Literal ID="litTitulo" runat="server" />
    </h2>

    <div class="card p-4" style="max-width: 640px;">

        <asp:PlaceHolder ID="phFormulario" runat="server">

            <asp:PlaceHolder ID="avisoAutoEdicao" runat="server" Visible="false">
                <div class="alert alert-warning">
                    <i class="fas fa-triangle-exclamation me-1"></i>
                    Estás a editar a tua própria conta: O perfil e o estado não podem ser alterados aqui, para evitar perderes acesso ao sistema.
                </div>
            </asp:PlaceHolder>

            <div class="mb-3">
                <label class="form-label">Nome</label>
                <asp:TextBox ID="txtNome" runat="server" CssClass="form-control" MaxLength="120" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtNome"
                    ErrorMessage="O nome é obrigatório." CssClass="text-danger small" Display="Dynamic" ValidationGroup="Guardar" />
            </div>

            <div class="mb-3">
                <label class="form-label">Email</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" MaxLength="150" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEmail"
                    ErrorMessage="O email é obrigatório." CssClass="text-danger small" Display="Dynamic" ValidationGroup="Guardar" />
            </div>

            <div class="mb-3">
                <label class="form-label">Perfil</label>
                <asp:DropDownList ID="ddlPerfil" runat="server" CssClass="form-select" />
            </div>

            <div class="mb-3">
                <label class="form-label">Estado</label>
                <asp:DropDownList ID="ddlEstado" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Ativo" Value="Ativo" />
                    <asp:ListItem Text="Bloqueado" Value="Bloqueado" />
                    <asp:ListItem Text="Inativo" Value="Inativo" />
                </asp:DropDownList>
            </div>

            <asp:PlaceHolder ID="phPasswordInicial" runat="server">
                <div class="alert alert-info" style="background:var(--accent-soft); border-color:var(--accent); color:var(--ink);">
                    <i class="fas fa-circle-info me-1"></i>
                    Uma password temporária será gerada automaticamente e apresentada após criares o utilizador.
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="phResetPassword" runat="server">
                <div class="mb-3 pt-2 border-top">
                    <asp:CheckBox ID="chkResetPassword" runat="server" AutoPostBack="true" OnCheckedChanged="chkResetPassword_CheckedChanged" />
                    <asp:Label runat="server" AssociatedControlID="chkResetPassword" Text=" Repor password (gera uma nova password temporária)" CssClass="form-check-label" />
                </div>
            </asp:PlaceHolder>

            <div class="d-flex gap-2 mt-3">
                <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-success" OnClick="btnGuardar_Click" ValidationGroup="Guardar" />
                <a href="~/Administracao/UtilizadoresLista.aspx" runat="server" class="btn btn-outline-secondary">Cancelar</a>
            </div>

        </asp:PlaceHolder>

        <asp:PlaceHolder ID="phPasswordGerada" runat="server" Visible="false">
            <div class="alert alert-warning">
                <strong>Password temporária:</strong>
                <span class="mono"><asp:Literal ID="litPasswordGerada" runat="server" /></span>
                <div class="small mt-1">Partilha esta password com o utilizador de forma segura. Não voltará a ser mostrada.</div>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="phAcoesPosCriacao" runat="server" Visible="false">
            <div class="d-flex gap-2 mt-3">
                <a href="~/Administracao/UtilizadoresLista.aspx" runat="server" class="btn btn-success">Voltar à lista</a>
                <a href="~/Administracao/UtilizadorEditar.aspx" runat="server" class="btn btn-outline-secondary">Criar outro utilizador</a>
            </div>
        </asp:PlaceHolder>

    </div>

</asp:Content>