<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Paginacao.ascx.cs" Inherits="CRM.Web.Controls.Paginacao" %>

<div class="d-flex justify-content-between align-items-center flex-wrap gap-2 mt-3">
    <div class="text-muted small">
        <asp:Literal ID="litResumo" runat="server" />
    </div>
    <nav>
        <ul class="pagination pagination-sm mb-0">
            <li class="page-item">
                <asp:LinkButton ID="lnkPrimeira" runat="server" CssClass="page-link" OnClick="lnkPrimeira_Click">&laquo;</asp:LinkButton>
            </li>
            <li class="page-item">
                <asp:LinkButton ID="lnkAnterior" runat="server" CssClass="page-link" OnClick="lnkAnterior_Click">&lsaquo;</asp:LinkButton>
            </li>
            <li class="page-item disabled">
                <span class="page-link"><asp:Literal ID="litPaginaAtual" runat="server" /></span>
            </li>
            <li class="page-item">
                <asp:LinkButton ID="lnkSeguinte" runat="server" CssClass="page-link" OnClick="lnkSeguinte_Click">&rsaquo;</asp:LinkButton>
            </li>
            <li class="page-item">
                <asp:LinkButton ID="lnkUltima" runat="server" CssClass="page-link" OnClick="lnkUltima_Click">&raquo;</asp:LinkButton>
            </li>
        </ul>
    </nav>
</div>