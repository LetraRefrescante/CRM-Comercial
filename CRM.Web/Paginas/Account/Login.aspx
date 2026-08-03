<%@ Page Title="Login" Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="CRM.Web.Account.Login" %>

<!DOCTYPE html>
<html lang="pt">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>CRM Comercial - Login</title>
    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link href="https://fonts.googleapis.com/css2?family=Sora:wght@600;700&family=Inter:wght@400;500;600&family=IBM+Plex+Mono:wght@500&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" />
    <link rel="stylesheet" href="~/Content/site.css" runat="server" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-shell">
            <div class="login-card">

                <div class="login-brandpanel">
                    <div>
                        <div class="brand">CRM Comercial</div>
                        <p class="login-tagline mt-2">Do primeiro contacto ao negócio fechado - tudo num só lugar.</p>
                    </div>

                    <div class="stage-tracker">
                        <div class="stage is-active"><span class="dot"></span><span class="label">Lead</span></div>
                        <div class="stage"><span class="dot"></span><span class="label">Cliente / Contacto</span></div>
                        <div class="stage"><span class="dot"></span><span class="label">Oportunidade</span></div>
                        <div class="stage"><span class="dot"></span><span class="label">Proposta</span></div>
                        <div class="stage"><span class="dot"></span><span class="label">Venda</span></div>
                    </div>

                    <p class="login-tagline" style="font-size:0.8rem;">© <%= DateTime.Now.Year %> - Uso interno</p>
                </div>

                <div class="login-formpanel">
                    <div class="login-form-inner">
                        <h4 class="mb-1">Entrar</h4>
                        <p class="text-muted mb-4" style="font-size:0.9rem;">Acede à tua conta para continuar.</p>

                        <asp:Panel ID="pnlErro" runat="server" CssClass="alert alert-danger py-2" Visible="false">
                            <asp:Literal ID="litErro" runat="server" />
                        </asp:Panel>

                        <div class="mb-3">
                            <label for="txtEmail" class="form-label">Email</label>
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" placeholder="nome@empresa.pt" />
                            <asp:RequiredFieldValidator ID="rfvEmail" runat="server"
                                ControlToValidate="txtEmail"
                                ErrorMessage="O email é obrigatório."
                                CssClass="text-danger small"
                                Display="Dynamic" />
                        </div>

                        <div class="mb-3">
                            <label for="txtPassword" class="form-label">Password</label>
                            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="••••••••" />
                            <asp:RequiredFieldValidator ID="rfvPassword" runat="server"
                                ControlToValidate="txtPassword"
                                ErrorMessage="A password é obrigatória."
                                CssClass="text-danger small"
                                Display="Dynamic" />
                        </div>

                        <div class="mb-3 form-check d-flex justify-content-between align-items-center">
                            <div class="form-check">
                                <asp:CheckBox ID="chkLembrar" runat="server" />
                                <asp:Label runat="server" AssociatedControlID="chkLembrar" CssClass="form-check-label" Text="Lembrar-me" />
                            </div>
                            <a href="RecuperarPassword.aspx" style="font-size:0.85rem;">Esqueceu a password?</a>
                        </div>

                        <asp:Button ID="btnLogin" runat="server" Text="Entrar" CssClass="btn btn-primary w-100 touch-target" OnClick="btnLogin_Click" />
                    </div>
                </div>

            </div>
        </div>
    </form>
</body>
</html>