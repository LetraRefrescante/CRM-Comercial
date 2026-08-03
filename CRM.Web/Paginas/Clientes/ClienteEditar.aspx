<%@ Page Title="Cliente" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="ClienteEditar.aspx.cs" Inherits="CRM.Web.Paginas.Clientes.ClienteEditar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item"><a href="~/Clientes/ClienteLista.aspx" runat="server">Clientes</a></li>
    <li class="breadcrumb-item active"><asp:Literal ID="litTituloBreadcrumb" runat="server" /></li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2 class="mb-4"><asp:Literal ID="litTitulo" runat="server" /></h2>

    <div class="card p-4" style="max-width: 760px;">

        <div class="row g-3">
            <div class="col-md-8">
                <label class="form-label">Nome Comercial</label>
                <asp:TextBox ID="txtNomeComercial" runat="server" CssClass="form-control" MaxLength="150" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtNomeComercial"
                    ErrorMessage="Obrigatório." CssClass="text-danger small" Display="Dynamic" ValidationGroup="Guardar" />
            </div>
            <div class="col-md-4">
                <label class="form-label">NIF</label>
                <asp:TextBox ID="txtNif" runat="server" CssClass="form-control mono" MaxLength="30" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtNif"
                    ErrorMessage="Obrigatório." CssClass="text-danger small" Display="Dynamic" ValidationGroup="Guardar" />
            </div>

            <div class="col-md-8">
                <label class="form-label">Nome Legal</label>
                <asp:TextBox ID="txtNomeLegal" runat="server" CssClass="form-control" MaxLength="200" />
            </div>
            <div class="col-md-4">
                <label class="form-label">Estado</label>
                <asp:DropDownList ID="ddlEstado" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Potencial" Value="Potencial" />
                    <asp:ListItem Text="Ativo" Value="Ativo" />
                    <asp:ListItem Text="Inativo" Value="Inativo" />
                    <asp:ListItem Text="Bloqueado" Value="Bloqueado" />
                </asp:DropDownList>
            </div>

            <div class="col-md-6">
                <label class="form-label">Email</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" MaxLength="150" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="txtEmail"
                    ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                    ErrorMessage="Email inválido." CssClass="text-danger small" Display="Dynamic" ValidationGroup="Guardar" />
            </div>
            <div class="col-md-6">
                <label class="form-label">Telefone</label>
                <asp:TextBox ID="txtTelefone" runat="server" CssClass="form-control" MaxLength="30" />
            </div>

            <div class="col-12">
                <label class="form-label">Morada</label>
                <asp:TextBox ID="txtMorada" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" MaxLength="300" />
            </div>

            <div class="col-md-4">
                <label class="form-label">Código Postal</label>
                <asp:TextBox ID="txtCodigoPostal" runat="server" CssClass="form-control" MaxLength="20" />
            </div>
            <div class="col-md-4">
                <label class="form-label">Cidade</label>
                <asp:TextBox ID="txtCidade" runat="server" CssClass="form-control" MaxLength="100" />
            </div>
            <div class="col-md-4">
                <label class="form-label">País</label>
                <asp:DropDownList ID="ddlPais" runat="server" CssClass="form-select" />
            </div>

            <div class="col-md-6">
                <label class="form-label">Setor</label>
                <asp:DropDownList ID="ddlSetor" runat="server" CssClass="form-select" />
            </div>
            <div class="col-md-6">
                <label class="form-label">Comercial Responsável</label>
                <asp:DropDownList ID="ddlComercial" runat="server" CssClass="form-select" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlComercial"
                    InitialValue="" ErrorMessage="Obrigatório." CssClass="text-danger small" Display="Dynamic" ValidationGroup="Guardar" />
            </div>

            <div class="col-12">
                <label class="form-label">Observações</label>
                <asp:TextBox ID="txtObservacoes" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" MaxLength="4000" />
            </div>
        </div>

        <div class="d-flex gap-2 mt-4">
            <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-primary" OnClick="btnGuardar_Click" ValidationGroup="Guardar" />
            <a href="~/Clientes/ClienteLista.aspx" runat="server" class="btn btn-outline-secondary">Cancelar</a>
        </div>

    </div>

</asp:Content>