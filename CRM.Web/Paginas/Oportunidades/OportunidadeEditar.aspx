<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OportunidadeEditar.aspx.cs" Inherits="CRM.Web.Oportunidades.OportunidadeEditar" MasterPageFile="~/MasterPages/Site.Master" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
<style>
    .cliente-busca-wrap { position: relative; }
    .cliente-busca-resultados {
        position: absolute; z-index: 20; top: 100%; left: 0; right: 0;
        background: #fff; border: 1px solid var(--line); border-radius: .375rem;
        box-shadow: 0 6px 16px rgba(18,33,59,.12); max-height: 260px; overflow-y: auto;
        display: none;
    }
    .cliente-busca-resultados .item { padding: .5rem .75rem; cursor: pointer; }
    .cliente-busca-resultados .item:hover { background: var(--accent-soft); }
    .cliente-busca-resultados .item .nome { font-weight: 600; font-size: .9rem; }
    .cliente-busca-resultados .item .meta { font-size: .78rem; color: var(--text-muted); }
    .cliente-selecionado-card {
        border: 1px solid var(--line); border-radius: .375rem; padding: .6rem .9rem;
        display: flex; justify-content: space-between; align-items: center; background: var(--accent-soft);
    }
</style>
</asp:Content>

<asp:Content ID="ContentBreadcrumb" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item"><a href="~/Dashboard/Dashboard.aspx" runat="server">Dashboard</a></li>
    <li class="breadcrumb-item"><a href="~/Oportunidades/Pipeline.aspx" runat="server">Oportunidades</a></li>
    <li class="breadcrumb-item active"><asp:Literal ID="litTituloBreadcrumb" runat="server" /></li>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="MainContent" runat="server">

    <h4 class="mb-3"><asp:Literal ID="litTitulo" runat="server" /></h4>

    <asp:UpdatePanel ID="upFormulario" runat="server">
        <ContentTemplate>
        <div class="card">
            <div class="card-body">
                <div class="row g-3">

                    <div class="col-md-8">
                        <label class="form-label">Título *</label>
                        <asp:TextBox ID="txtTitulo" runat="server" CssClass="form-control" MaxLength="200" />
                        <asp:CustomValidator ID="cvTitulo" runat="server" ControlToValidate="txtTitulo"
                            OnServerValidate="cvTitulo_ServerValidate" Display="Dynamic" CssClass="crm-validation-message text-danger small"
                            ErrorMessage="O título é obrigatório e deve ter entre 2 e 200 caracteres." />
                    </div>

                    <div class="col-md-4">
                        <label class="form-label">Fase *</label>
                        <asp:DropDownList ID="ddlFase" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlFase_SelectedIndexChanged" />
                        <asp:RequiredFieldValidator ID="rfvFase" runat="server" ControlToValidate="ddlFase"
                            Display="Dynamic" CssClass="crm-validation-message text-danger small" ErrorMessage="A fase é obrigatória." />
                    </div>

                    <div class="col-md-12">
                        <label class="form-label">Cliente *</label>

                        <asp:HiddenField ID="hdnClientId" runat="server" />

                        <asp:PlaceHolder ID="phClienteBusca" runat="server">
                            <div class="cliente-busca-wrap">
                                <asp:TextBox ID="txtClientePesquisa" runat="server" CssClass="form-control"
                                    placeholder="Pesquisar por nome comercial ou NIF..." autocomplete="off" />
                                <div id="divResultadosCliente" class="cliente-busca-resultados"></div>
                            </div>
                        </asp:PlaceHolder>

                        <asp:PlaceHolder ID="phClienteSelecionado" runat="server" Visible="false">
                            <div class="cliente-selecionado-card">
                                <div>
                                    <div class="fw-semibold"><asp:Literal ID="litClienteNome" runat="server" /></div>
                                    <div class="text-muted small"><asp:Literal ID="litClienteNif" runat="server" /></div>
                                </div>
                                <asp:LinkButton ID="lnkAlterarCliente" runat="server" CssClass="btn btn-sm btn-outline-secondary"
                                    OnClick="lnkAlterarCliente_Click" CausesValidation="false">Alterar</asp:LinkButton>
                            </div>
                        </asp:PlaceHolder>

                        <asp:CustomValidator ID="cvCliente" runat="server" ControlToValidate="hdnClientId"
                            OnServerValidate="cvCliente_ServerValidate" Display="Dynamic" CssClass="crm-validation-message text-danger small"
                            ErrorMessage="Tens de selecionar um cliente." />

                        <!-- Clicado via JS depois de escolher um resultado da pesquisa; despoleta
                             postback assíncrono (UpdatePanel) para recarregar os Contactos. -->
                        <asp:Button ID="btnClienteSelecionado" runat="server" Text="" style="display:none;"
                            CausesValidation="false" OnClick="btnClienteSelecionado_Click" />
                    </div>

                    <div class="col-md-6">
                        <label class="form-label">Contacto</label>
                        <asp:DropDownList ID="ddlContacto" runat="server" CssClass="form-select" />
                        <div class="form-text">Opcional. Só mostra contactos do cliente selecionado.</div>
                    </div>

                    <div class="col-md-6">
                        <label class="form-label">Comercial Responsável *</label>
                        <asp:DropDownList ID="ddlComercial" runat="server" CssClass="form-select" />
                        <asp:RequiredFieldValidator ID="rfvComercial" runat="server" ControlToValidate="ddlComercial"
                            Display="Dynamic" CssClass="crm-validation-message text-danger small" ErrorMessage="O comercial responsável é obrigatório." />
                    </div>

                    <div class="col-md-4">
                        <label class="form-label">Valor Estimado (€) *</label>
                        <asp:TextBox ID="txtValorEstimado" runat="server" CssClass="form-control money" TextMode="Number" step="0.01" />
                        <asp:CustomValidator ID="cvValorEstimado" runat="server" ControlToValidate="txtValorEstimado"
                            OnServerValidate="cvValorEstimado_ServerValidate" Display="Dynamic" CssClass="crm-validation-message text-danger small"
                            ErrorMessage="O valor estimado é obrigatório e deve ser superior a zero." />
                    </div>

                    <div class="col-md-4">
                        <label class="form-label">Probabilidade (%) *</label>
                        <asp:TextBox ID="txtProbabilidade" runat="server" CssClass="form-control" TextMode="Number" />
                        <asp:CustomValidator ID="cvProbabilidade" runat="server" ControlToValidate="txtProbabilidade"
                            OnServerValidate="cvProbabilidade_ServerValidate" Display="Dynamic" CssClass="crm-validation-message text-danger small"
                            ErrorMessage="A probabilidade deve estar entre 0 e 100." />
                        <div class="form-text">Preenchida automaticamente ao escolher a fase; podes ajustar.</div>
                    </div>

                    <div class="col-md-4">
                        <label class="form-label">Data de Fecho Prevista *</label>
                        <asp:TextBox ID="txtDataFechoPrevista" runat="server" CssClass="form-control" TextMode="Date" />
                        <asp:RequiredFieldValidator ID="rfvDataFecho" runat="server" ControlToValidate="txtDataFechoPrevista"
                            Display="Dynamic" CssClass="crm-validation-message text-danger small" ErrorMessage="A data de fecho prevista é obrigatória." />
                    </div>

                    <div class="col-md-12">
                        <label class="form-label">Concorrente</label>
                        <asp:TextBox ID="txtConcorrente" runat="server" CssClass="form-control" MaxLength="150" />
                    </div>

                </div>

                <hr class="my-4" />

                <div class="d-flex gap-2">
                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-primary" OnClick="btnGuardar_Click" />
                    <a href="~/Oportunidades/OportunidadesLista.aspx" runat="server" class="btn btn-outline-secondary">Cancelar</a>
                </div>

            </div>
        </div>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

<asp:Content ID="ContentScripts" ContentPlaceHolderID="Scripts" runat="server">
<script>
(function () {
    var handlerUrl = '<%= ResolveUrl("~/Oportunidades/ClienteBuscaHandler.ashx") %>';
    var idPesquisa = '<%= txtClientePesquisa.ClientID %>';
    var idResultados = 'divResultadosCliente';
    var idHidden = '<%= hdnClientId.ClientID %>';
    var idBotaoSelecionado = '<%= btnClienteSelecionado.ClientID %>';
    var timeoutId = null;

    // Delegação em "document": sobrevive aos refreshes parciais do UpdatePanel
    // (o próprio "document" nunca é substituído, ao contrário dos controlos lá dentro).
    document.addEventListener('input', function (e) {
        if (e.target.id !== idPesquisa) return;

        var termo = e.target.value.trim();
        clearTimeout(timeoutId);

        var divResultados = document.getElementById(idResultados);
        if (!divResultados) return;

        if (termo.length < 2) {
            divResultados.style.display = 'none';
            divResultados.innerHTML = '';
            return;
        }

        timeoutId = setTimeout(function () { pesquisar(termo); }, 300);
    });

    document.addEventListener('click', function (e) {
        var divResultados = document.getElementById(idResultados);
        var txtPesquisa = document.getElementById(idPesquisa);
        if (!divResultados) return;

        var item = e.target.closest ? e.target.closest('.item[data-id]') : null;
        if (item) {
            var hdnClientId = document.getElementById(idHidden);
            var botao = document.getElementById(idBotaoSelecionado);
            hdnClientId.value = item.dataset.id;
            divResultados.style.display = 'none';
            botao.click();
            return;
        }

        if (txtPesquisa && e.target !== txtPesquisa && !divResultados.contains(e.target)) {
            divResultados.style.display = 'none';
        }
    });

    function pesquisar(termo) {
        fetch(handlerUrl + '?q=' + encodeURIComponent(termo))
            .then(function (resp) { return resp.json(); })
            .then(function (dados) { mostrarResultados(dados); })
            .catch(function () {
                var divResultados = document.getElementById(idResultados);
                if (divResultados) divResultados.style.display = 'none';
            });
    }

    function mostrarResultados(clientes) {
        var divResultados = document.getElementById(idResultados);
        if (!divResultados) return;

        if (!clientes || clientes.length === 0) {
            divResultados.innerHTML = '<div class="item text-muted">Nenhum cliente encontrado.</div>';
            divResultados.style.display = 'block';
            return;
        }

        divResultados.innerHTML = clientes.map(function (c) {
            return '<div class="item" data-id="' + c.id + '">' +
                   '<div class="nome">' + escapeHtml(c.nome) + '</div>' +
                   '<div class="meta">NIF ' + escapeHtml(c.nif) + (c.cidade ? ' — ' + escapeHtml(c.cidade) : '') + '</div>' +
                   '</div>';
        }).join('');

        divResultados.style.display = 'block';
    }

    function escapeHtml(texto) {
        var div = document.createElement('div');
        div.textContent = texto || '';
        return div.innerHTML;
    }
})();
</script>
</asp:Content>