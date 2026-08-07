<%@ Page Title="Acesso Negado" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="AcessoNegado.aspx.cs" Inherits="CRM.Web.Paginas.AcessoNegado" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item active">Acesso Negado</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="d-flex flex-column align-items-center justify-content-center text-center py-5">
        <i class="fas fa-lock" style="font-size:3rem; color:#E0A83E; opacity:0.7;"></i>
        <h3 class="mt-3" style="font-family:'Sora',sans-serif;">Acesso Negado</h3>
        <p class="text-muted mb-4" style="max-width:420px;">
            Não tens permissão para aceder a esta página ou executar esta ação.
            Se achas que isto é um erro, contacta o administrador do sistema.
        </p>
        <a href="~/Dashboard/Dashboard.aspx" runat="server" class="btn btn-primary">Voltar ao Dashboard</a>
    </div>
</asp:Content>