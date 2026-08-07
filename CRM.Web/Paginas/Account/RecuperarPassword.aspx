<%@ Page Title="Recuperar Password" Language="C#" AutoEventWireup="true" CodeBehind="RecuperarPassword.aspx.cs" Inherits="CRM.Web.Account.RecuperarPassword" %>

<!DOCTYPE html>
<html lang="pt">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>CRM Comercial - Recuperar Password</title>
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
                        <p class="login-tagline mt-2">Recuperação de acesso à conta.</p>
                    </div>
                    <p class="login-tagline" style="font-size:0.8rem;">© <%= DateTime.Now.Year %> - Uso interno</p>
                </div>

                <div class="login-formpanel">
                    <div class="login-form-inner">

                        <asp:Panel ID="pnlPedido" runat="server">
                            <h4 class="mb-1">Recuperar Password</h4>
                            <p class="text-muted mb-4" style="font-size:0.9rem;">
                                Indica o teu email. Se existir conta associada, vais receber uma ligação para repor a password.
                            </p>

                            <div class="mb-3">
                                <label for="txtEmail" class="form-label">Email</label>
                                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" placeholder="nome@empresa.pt" />
                                <asp:RequiredFieldValidator ID="rfvEmail" runat="server"
                                    ControlToValidate="txtEmail"
                                    ErrorMessage="O email é obrigatório."
                                    CssClass="text-danger small"
                                    Display="Dynamic" />
                            </div>

                            <asp:Button ID="btnSolicitar" runat="server" Text="Enviar ligação de recuperação"
                                CssClass="btn btn-primary w-100 touch-target" OnClick="btnSolicitar_Click" />
                        </asp:Panel>

                        <asp:Panel ID="pnlConfirmacao" runat="server" Visible="false">
                            <h4 class="mb-1">Verifica o teu email</h4>
                            <p class="text-muted mb-0" style="font-size:0.9rem;">
                                Se o email indicado corresponder a uma conta, foi enviada uma mensagem com as
                                instruções para repor a password. A ligação é válida durante 30 minutos.
                            </p>
                        </asp:Panel>

                        <div class="text-center mt-3">
                            <a href="Login.aspx" style="font-size:0.85rem;">Voltar ao login</a>
                        </div>

                    </div>
                </div>

            </div>
        </div>
    </form>
</body>
</html>