<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Erro.aspx.cs" Inherits="CRM.Web.Erro" %>
<!DOCTYPE html>
<html lang="pt">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Ocorreu um erro — CRM Comercial</title>
    <link href="https://fonts.googleapis.com/css2?family=Sora:wght@600;700&family=Inter:wght@400;500;600&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" />
    <style>
        body {
            font-family: 'Inter', sans-serif;
            background: #12213B;
            color: #fff;
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
        }
        .erro-card {
            background: #fff;
            color: #12213B;
            border-radius: 1rem;
            padding: 3rem;
            max-width: 480px;
            text-align: center;
            box-shadow: 0 20px 60px rgba(0,0,0,0.3);
        }
        .erro-card h1 {
            font-family: 'Sora', sans-serif;
            font-size: 2rem;
            color: #12213B;
        }
        .erro-codigo {
            display: inline-block;
            background: #f1f3f5;
            color: #6c757d;
            font-family: 'IBM Plex Mono', monospace;
            font-size: 0.85rem;
            padding: 0.35rem 0.75rem;
            border-radius: 0.5rem;
            margin-top: 1rem;
        }
        .btn-voltar {
            background: #1F7A5C;
            border: none;
        }
        .btn-voltar:hover {
            background: #17624a;
        }
    </style>
</head>
<body>
    <div class="erro-card">
        <h1><%: TituloErro %></h1>
        <p class="text-muted mb-4"><%: MensagemErro %></p>
        <a href="~/Dashboard/Dashboard.aspx" runat="server" class="btn btn-voltar text-white px-4">Voltar ao início</a>
        <div class="erro-codigo">Ref: <%: IdOcorrencia %></div>
    </div>
</body>
</html>