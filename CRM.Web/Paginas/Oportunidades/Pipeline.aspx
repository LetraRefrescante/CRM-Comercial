<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Pipeline.aspx.cs" Inherits="CRM.Web.Oportunidades.Pipeline" MasterPageFile="~/MasterPages/Site.Master" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
<style>
    .pipeline-scroll { display: flex; gap: 1rem; overflow-x: auto; padding-bottom: 1rem; }
    .pipeline-coluna { flex: 0 0 280px; background: #f8f9fa; border-radius: .5rem; display: flex; flex-direction: column; max-height: calc(100vh - 220px); }
    .pipeline-coluna-header { padding: .75rem; border-bottom: 1px solid #dee2e6; }
    .pipeline-coluna-header h6 { margin-bottom: .5rem; }
    .pipeline-coluna-body { flex: 1; overflow-y: auto; padding: .5rem; min-height: 80px; transition: background-color .15s; }
    .pipeline-coluna-body.a-receber { background: #e7f1ff; }
    .pipeline-cartao { background: #fff; border: 1px solid #dee2e6; border-radius: .375rem; padding: .6rem; margin-bottom: .5rem; cursor: grab; }
    .pipeline-cartao[draggable="false"] { cursor: default; }
    .pipeline-cartao.a-arrastar { opacity: .4; }
    .pipeline-cartao.a-mover { opacity: .6; pointer-events: none; }
    .pipeline-cartao-titulo { font-weight: 600; font-size: .9rem; }
</style>
</asp:Content>

<asp:Content ID="ContentBreadcrumb" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item"><a href="~/Dashboard/Dashboard.aspx" runat="server">Dashboard</a></li>
    <li class="breadcrumb-item active">Oportunidades — Pipeline</li>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="MainContent" runat="server">

    <div class="d-flex justify-content-between align-items-center mb-3">
        <h4 class="mb-0">Pipeline de Oportunidades</h4>
        <div class="d-flex gap-2">
            <a href="~/Oportunidades/OportunidadesLista.aspx" runat="server" class="btn btn-outline-secondary">
                <i class="fas fa-list"></i> Ver Lista
            </a>
            <asp:HyperLink ID="lnkNova" runat="server" CssClass="btn btn-primary" NavigateUrl="~/Oportunidades/OportunidadeEditar.aspx">
                <i class="fas fa-plus"></i> Nova Oportunidade
            </asp:HyperLink>
        </div>
    </div>

    <asp:PlaceHolder ID="phModoConsulta" runat="server" Visible="false">
        <div class="alert alert-secondary py-2 small">
            <i class="fas fa-lock"></i> Estás em modo de consulta — não podes mover oportunidades entre fases.
        </div>
    </asp:PlaceHolder>

    <div class="pipeline-scroll">
        <asp:Repeater ID="rptColunas" runat="server" OnItemDataBound="rptColunas_ItemDataBound">
            <ItemTemplate>
                <div class="pipeline-coluna">
                    <div class="pipeline-coluna-header">
                        <h6><%# Eval("Stage.Name") %></h6>
                        <div class="d-flex justify-content-between">
                            <span class="badge bg-secondary pipeline-contador"><%# ContarCartoes(Container.DataItem) %></span>
                            <span class="text-muted small pipeline-valor-total"><%# FormatarValorColuna(Container.DataItem) %></span>
                        </div>
                    </div>
                    <div class="pipeline-coluna-body" data-stage-id='<%# Eval("Stage.StageId") %>'>
                        <asp:Repeater ID="rptCartoes" runat="server">
                            <ItemTemplate>
                                <div class="pipeline-cartao"
                                     draggable="<%# PodeMover.ToString().ToLower() %>"
                                     data-opportunity-id='<%# Eval("OpportunityId") %>'
                                     data-valor='<%# ((decimal)Eval("EstimatedValue")).ToString(System.Globalization.CultureInfo.InvariantCulture) %>'>
                                    <div class="pipeline-cartao-titulo"><%# Eval("Title") %></div>
                                    <div class="text-muted small"><%# Eval("Client.TradeName") %></div>
                                    <div class="d-flex justify-content-between align-items-center mt-2">
                                        <span class="fw-semibold small"><%# Eval("EstimatedValue", "{0:N2} €") %></span>
                                        <span class="badge bg-light text-dark"><%# Eval("Probability") %>%</span>
                                    </div>
                                    <div class="d-flex justify-content-between align-items-center mt-1">
                                        <span class="text-muted small"><i class="fas fa-user"></i> <%# Eval("Owner.Name") %></span>
                                        <span class="text-muted small"><%# Eval("ExpectedCloseDate", "{0:dd/MM}") %></span>
                                    </div>
                                    <div class="text-end mt-2">
                                        <a class="btn btn-sm btn-outline-secondary" href='<%# "OportunidadeEditar.aspx?id=" + Eval("OpportunityId") %>'>
                                            <i class="fas fa-eye"></i>
                                        </a>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>

</asp:Content>

<asp:Content ID="ContentScripts" ContentPlaceHolderID="Scripts" runat="server">
<script>
(function () {
    var podeMover = <%= PodeMover.ToString().ToLower() %>;
    if (!podeMover) return;

    var handlerUrl = '<%= ResolveUrl("~/Oportunidades/MudarFaseHandler.ashx") %>';
    var cartaoArrastado = null;

    document.querySelectorAll('.pipeline-cartao[draggable="true"]').forEach(function (cartao) {
        cartao.addEventListener('dragstart', function () {
            cartaoArrastado = cartao;
            cartao.classList.add('a-arrastar');
        });
        cartao.addEventListener('dragend', function () {
            cartao.classList.remove('a-arrastar');
        });
    });

    document.querySelectorAll('.pipeline-coluna-body').forEach(function (coluna) {
        coluna.addEventListener('dragover', function (e) {
            e.preventDefault();
            coluna.classList.add('a-receber');
        });
        coluna.addEventListener('dragleave', function () {
            coluna.classList.remove('a-receber');
        });
        coluna.addEventListener('drop', function (e) {
            e.preventDefault();
            coluna.classList.remove('a-receber');
            if (!cartaoArrastado) return;

            var colunaOrigem = cartaoArrastado.closest('.pipeline-coluna-body');
            if (colunaOrigem === coluna) return;

            moverOportunidade(cartaoArrastado, colunaOrigem, coluna);
        });
    });

    function moverOportunidade(cartao, colunaOrigem, colunaDestino) {
        cartao.classList.add('a-mover');
        var corpo = 'opportunityId=' + encodeURIComponent(cartao.dataset.opportunityId)
                  + '&novaFaseId=' + encodeURIComponent(colunaDestino.dataset.stageId);

        fetch(handlerUrl, {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: corpo
        })
        .then(function (resp) { return resp.json(); })
        .then(function (dados) {
            cartao.classList.remove('a-mover');
            if (dados.sucesso) {
                colunaDestino.appendChild(cartao);
                atualizarCabecalho(colunaOrigem);
                atualizarCabecalho(colunaDestino);
            } else {
                alert(dados.mensagem || 'Não foi possível mover a oportunidade.');
            }
        })
        .catch(function () {
            cartao.classList.remove('a-mover');
            alert('Erro de comunicação. Tenta novamente.');
        });
    }

    function atualizarCabecalho(colunaBody) {
        var coluna = colunaBody.closest('.pipeline-coluna');
        var cartoes = colunaBody.querySelectorAll('.pipeline-cartao');
        var total = 0;
        cartoes.forEach(function (c) { total += parseFloat(c.dataset.valor) || 0; });

        coluna.querySelector('.pipeline-contador').textContent = cartoes.length;
        coluna.querySelector('.pipeline-valor-total').textContent =
            total.toLocaleString('pt-PT', { style: 'currency', currency: 'EUR' });
    }
})();
</script>
</asp:Content>