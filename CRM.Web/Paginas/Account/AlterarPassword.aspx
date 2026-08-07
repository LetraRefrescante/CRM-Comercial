<%@ Page Title="Alterar Password" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="AlterarPassword.aspx.cs" Inherits="CRM.Web.Paginas.Conta.AlterarPassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item active">Alterar Password</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="d-flex justify-content-center py-5">
        <div class="card p-4" style="max-width:420px; width:100%;">
            <h4 class="mb-3" style="font-family:'Sora',sans-serif;">Alterar Password</h4>

            <div class="mb-3">
                <label class="form-label">Password atual</label>
                <asp:TextBox ID="txtPasswordAtual" runat="server" CssClass="form-control" TextMode="Password" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPasswordAtual"
                    CssClass="text-danger small" ErrorMessage="Indica a password atual." Display="Dynamic" />
            </div>

            <div class="mb-3">
                <label class="form-label">Nova password</label>
                <asp:TextBox ID="txtNovaPassword" runat="server" CssClass="form-control" TextMode="Password" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtNovaPassword"
                    CssClass="text-danger small" ErrorMessage="Indica a nova password." Display="Dynamic" />
            </div>

            <div class="mb-3">
                <label class="form-label">Confirmar nova password</label>
                <asp:TextBox ID="txtConfirmarPassword" runat="server" CssClass="form-control" TextMode="Password" />
                <asp:CompareValidator runat="server" ControlToValidate="txtConfirmarPassword"
                    ControlToCompare="txtNovaPassword" CssClass="text-danger small"
                    ErrorMessage="As passwords não coincidem." Display="Dynamic" />
            </div>

            <asp:Label ID="lblErro" runat="server" CssClass="text-danger small d-block mb-2" Visible="false" />

            <asp:Button ID="btnAlterar" runat="server" Text="Alterar password"
                CssClass="btn btn-primary w-100" OnClick="btnAlterar_Click" />
        </div>
    </div>
</asp:Content>
