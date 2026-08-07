<%@ Page Title="Redefinir Password" Language="C#" AutoEventWireup="true" CodeBehind="RedefinirPassword.aspx.cs" Inherits="CRM.Web.Account.RedefinirPassword" %>

<!DOCTYPE html>
<html lang="pt">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>CRM Comercial - Redefinir Password</title>
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
                        <p class="login-tagline mt-2">Define a tua nova password.</p>
                    </div>
                    <p class="login-tagline" style="font-size:0.8rem;">© <%= DateTime.Now.Year %> - Uso interno</p>
                </div>

                <div class="login-formpanel">
                    <div class="login-form-inner">

                        <asp:Panel ID="pnlTokenInvalido" runat="server" Visible="false">
                            <h4 class="mb-1">Ligação inválida</h4>
                            <p class="text-muted mb-4" style="font-size:0.9rem;">
                                Esta ligação de recuperação é inválida ou já expirou.
                            </p>
                            <a href="RecuperarPassword.aspx" class="btn btn-primary w-100 touch-target">Pedir nova ligação</a>
                        </asp:Panel>

                        <asp:Panel ID="pnlFormulario" runat="server">
                            <h4 class="mb-3">Definir nova password</h4>

                            <div class="mb-3">
                                <label for="txtNovaPassword" class="form-label">Nova password</label>
                                <asp:TextBox ID="txtNovaPassword" runat="server" CssClass="form-control" TextMode="Password" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtNovaPassword"
                                    CssClass="text-danger small" ErrorMessage="Indica a nova password." Display="Dynamic" />
                            </div>

                            <div class="mb-3">
                                <label for="txtConfirmarPassword" class="form-label">Confirmar password</label>
                                <asp:TextBox ID="txtConfirmarPassword" runat="server" CssClass="form-control" TextMode="Password" />
                                <asp:CompareValidator runat="server" ControlToValidate="txtConfirmarPassword"
                                    ControlToCompare="txtNovaPassword" CssClass="text-danger small"
                                    ErrorMessage="As passwords não coincidem." Display="Dynamic" />
                            </div>

                            <asp:Label ID="lblErro" runat="server" CssClass="text-danger small d-block mb-2" Visible="false" />

                            <asp:Button ID="btnRedefinir" runat="server" Text="Redefinir password"
                                CssClass="btn btn-primary w-100 touch-target" OnClick="btnRedefinir_Click" />
                        </asp:Panel>

                    </div>
                </div>

            </div>
        </div>
    </form>
</body>
</html>
