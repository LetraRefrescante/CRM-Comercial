<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Mensagens.ascx.cs" Inherits="CRM.Web.Controls.Mensagens" %>

<div class="toast-container position-fixed bottom-0 end-0 p-3" style="z-index: 1080;" id="toastContainer">
    <asp:Repeater ID="rptMensagens" runat="server">
        <ItemTemplate>
            <div class="toast align-items-center text-bg-<%# Eval("Tipo") %> border-0 mb-2"
                 role="alert" aria-live="assertive" aria-atomic="true"
                 data-bs-delay="<%# Eval("Duracao") %>">
                <div class="d-flex">
                    <div class="toast-body">
                        <i class="<%# Eval("Icone") %> me-2"></i><%# Eval("Texto") %>
                    </div>
                    <button type="button" class="btn-close me-2 m-auto" data-bs-dismiss="toast" aria-label="Fechar"></button>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>

<script>
    document.addEventListener("DOMContentLoaded", function () {
        document.querySelectorAll("#toastContainer .toast").forEach(function (el) {
            new bootstrap.Toast(el).show();
        });
    });
</script>