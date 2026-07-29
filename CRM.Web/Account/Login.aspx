<%@ Page Title="Login" Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="CRM.Web.Account.Login" %>

<!DOCTYPE html>
<html lang="pt">
<head runat="server">
    <meta charset="utf-8" />
    <title>CRM - Login</title>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" />
</head>
<body class="bg-light">
    <form id="form1" runat="server">
        <div class="d-flex align-items-center justify-content-center vh-100">
            <div class="card shadow-sm" style="width: 400px;">
                <div class="card-body p-4">
                    <h4 class="card-title text-center mb-4">CRM Comercial</h4>

                    <asp:Panel ID="pnlErro" runat="server" CssClass="alert alert-danger" Visible="false">
                        <asp:Literal ID="litErro" runat="server" />
                    </asp:Panel>

                    <div class="mb-3">
                        <label for="txtEmail" class="form-label">Email</label>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" />
                        <asp:RequiredFieldValidator ID="rfvEmail" runat="server"
                            ControlToValidate="txtEmail"
                            ErrorMessage="O email é obrigatório."
                            CssClass="text-danger small"
                            Display="Dynamic" />
                    </div>

                    <div class="mb-3">
                        <label for="txtPassword" class="form-label">Password</label>
                        <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" />
                        <asp:RequiredFieldValidator ID="rfvPassword" runat="server"
                            ControlToValidate="txtPassword"
                            ErrorMessage="A password é obrigatória."
                            CssClass="text-danger small"
                            Display="Dynamic" />
                    </div>

                    <div class="mb-3 form-check">
                        <asp:CheckBox ID="chkLembrar" runat="server" CssClass="form-check-input" />
                        <label class="form-check-label">Lembrar utilizador</label>
                    </div>

                    <asp:Button ID="btnLogin" runat="server" Text="Entrar" CssClass="btn btn-primary w-100 touch-target" OnClick="btnLogin_Click" />

                    <div class="text-center mt-3">
                        <a href="RecuperarPassword.aspx">Esqueceu-se da password?</a>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>