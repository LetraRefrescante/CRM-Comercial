<%@ Page Title="Contacto" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="ContactoEditar.aspx.cs" Inherits="CRM.Web.Paginas.Clientes.ContactoEditar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item"><a href="~/Clientes/ClienteLista.aspx" runat="server">Clientes</a></li>
    <li class="breadcrumb-item"><a id="lnkClientePai" runat="server">Cliente</a></li>
    <li class="breadcrumb-item active"><asp:Literal ID="litTituloBreadcrumb" runat="server" /></li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2 class="mb-4"><asp:Literal ID="litTitulo" runat="server" /></h2>

    <div class="card p-4" style="max-width: 760px;">

        <div class="row g-3">
            <div class="col-md-8">
                <label class="form-label">Cliente</label>
                <uc:SeletorCliente ID="ucSeletorCliente" runat="server" Obrigatorio="true" OnClienteSelecionado="ucSeletorCliente_ClienteSelecionado" />
            </div>
        </div>

        <hr class="my-3" />

        <div class="row g-3">
            <div class="col-md-8">
                <label class="form-label">Nome</label>
                <asp:TextBox ID="txtNome" runat="server" CssClass="form-control" MaxLength="120" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtNome"
                    ErrorMessage="Obrigatório." CssClass="text-danger small" Display="Dynamic" ValidationGroup="Guardar" />
            </div>
            <div class="col-md-4">
                <label class="form-label">Cargo</label>
                <asp:TextBox ID="txtCargo" runat="server" CssClass="form-control" MaxLength="100" />
            </div>

            <div class="col-md-6">
                <label class="form-label">Departamento</label>
                <asp:TextBox ID="txtDepartamento" runat="server" CssClass="form-control" MaxLength="100" />
            </div>
            <div class="col-md-6">
                <label class="form-label">Data de Nascimento</label>
                <asp:TextBox ID="txtDataNascimento" runat="server" CssClass="form-control" TextMode="Date" />
                <asp:CustomValidator ID="cvDataNascimento" runat="server" ControlToValidate="txtDataNascimento"
                    OnServerValidate="cvDataNascimento_ServerValidate"
                    ErrorMessage="A data de nascimento não pode ser futura." CssClass="text-danger small" Display="Dynamic" ValidationGroup="Guardar" />
            </div>

            <div class="col-md-6">
                <label class="form-label">Email</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" MaxLength="150" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="txtEmail"
                    ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                    ErrorMessage="Email inválido." CssClass="text-danger small" Display="Dynamic" ValidationGroup="Guardar" />
            </div>
            <div class="col-md-3">
                <label class="form-label">Telefone</label>
                <asp:TextBox ID="txtTelefone" runat="server" CssClass="form-control" MaxLength="30" />
                <asp:CustomValidator ID="cvTelefone" runat="server" ControlToValidate="txtTelefone"
                    OnServerValidate="cvTelefone_ServerValidate"
                    ErrorMessage="Telefone inválido." CssClass="text-danger small" Display="Dynamic" ValidationGroup="Guardar" />
            </div>
            <div class="col-md-3">
                <label class="form-label">Telemóvel</label>
                <asp:TextBox ID="txtTelemovel" runat="server" CssClass="form-control" MaxLength="30" />
                <asp:CustomValidator ID="cvTelemovel" runat="server" ControlToValidate="txtTelemovel"
                    OnServerValidate="cvTelemovel_ServerValidate"
                    ErrorMessage="Telemóvel inválido." CssClass="text-danger small" Display="Dynamic" ValidationGroup="Guardar" />
            </div>

            <div class="col-md-6">
                <label class="form-label">Preferência de Contacto</label>
                <asp:DropDownList ID="ddlPreferencia" runat="server" CssClass="form-select">
                    <asp:ListItem Text="(Sem preferência)" Value="" />
                    <asp:ListItem Text="Email" Value="Email" />
                    <asp:ListItem Text="Telefone" Value="Telefone" />
                    <asp:ListItem Text="Telemóvel" Value="Telemóvel" />
                    <asp:ListItem Text="Reunião" Value="Reunião" />
                </asp:DropDownList>
            </div>
            <div class="col-md-6 d-flex align-items-end gap-4">
                <div class="form-check">
                    <asp:CheckBox ID="chkPrincipal" runat="server" CssClass="form-check-input" />
                    <label class="form-check-label">Contacto Principal</label>
                </div>
                <div class="form-check">
                    <asp:CheckBox ID="chkConsentimento" runat="server" CssClass="form-check-input" />
                    <label class="form-check-label">Consentimento de Contacto</label>
                </div>
            </div>

            <div class="col-12">
                <label class="form-label">Restrições de Contacto</label>
                <asp:TextBox ID="txtRestricoes" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" MaxLength="500" />
            </div>
        </div>

        <div class="d-flex gap-2 mt-4">
            <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-primary" OnClick="btnGuardar_Click" ValidationGroup="Guardar" />
            <asp:HyperLink ID="lnkCancelar" runat="server" CssClass="btn btn-outline-secondary">Cancelar</asp:HyperLink>
        </div>

    </div>

</asp:Content>