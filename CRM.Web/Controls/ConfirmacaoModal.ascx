<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConfirmacaoModal.ascx.cs" Inherits="CRM.Web.Controls.ConfirmacaoModal" %>

<div class="modal fade" id="confirmModal" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header border-0">
                <h5 class="modal-title">Confirmar ação</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Fechar"></button>
            </div>
            <div class="modal-body">
                <p id="confirmModalBody" class="mb-0">Tens a certeza?</p>
            </div>
            <div class="modal-footer border-0">
                <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">Cancelar</button>
                <button type="button" id="btnConfirmarAcao" class="btn btn-danger">Confirmar</button>
            </div>
        </div>
    </div>
</div>

<script>
    (function () {
        var elAlvo = null;

        document.addEventListener('click', function (e) {
            var el = e.target.closest('[data-confirm]');
            if (!el) return;

            e.preventDefault();

            document.getElementById('confirmModalBody').innerText = el.getAttribute('data-confirm');
            elAlvo = el;

            new bootstrap.Modal(document.getElementById('confirmModal')).show();
        });

        document.getElementById('btnConfirmarAcao').addEventListener('click', function () {
            bootstrap.Modal.getInstance(document.getElementById('confirmModal')).hide();

            if (elAlvo) {
                var mensagem = elAlvo.getAttribute('data-confirm');
                elAlvo.removeAttribute('data-confirm');
                elAlvo.click();
                elAlvo.setAttribute('data-confirm', mensagem);
                elAlvo = null;
            }
        });
    })();
</script>