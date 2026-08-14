<%@ Page Title="Venda" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="VendaDetalhe.aspx.cs" Inherits="CRM.Web.Paginas.Vendas.VendaDetalhe" %>
<%@ Register TagPrefix="uc" TagName="Anexos" Src="~/Controls/Anexos.ascx" %>
<%@ Register TagPrefix="uc" TagName="Historico" Src="~/Controls/Historico.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item"><a href="VendasLista.aspx">Vendas</a></li>
    <li class="breadcrumb-item active">Detalhe</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="crm-list-header">
        <h2>Venda <asp:Label ID="lblNumero" runat="server" CssClass="text-muted fs-6" /></h2>
        <span id="spanStatus" runat="server" class="badge bg-secondary"></span>
    </div>

    <div class="crm-filter-card">
        <div class="row g-3">
            <div class="col-md-3"><span class="text-muted small">Cliente</span><div class="fw-semibold"><asp:Label ID="lblCliente" runat="server" /></div></div>
            <div class="col-md-2"><span class="text-muted small">Data</span><div class="fw-semibold"><asp:Label ID="lblData" runat="server" /></div></div>
            <div class="col-md-2"><span class="text-muted small">Comercial</span><div class="fw-semibold"><asp:Label ID="lblComercial" runat="server" /></div></div>
            <div class="col-md-2"><span class="text-muted small">Origem</span><div class="fw-semibold"><asp:Label ID="lblOrigem" runat="server" /></div></div>
            <div class="col-md-3">
                <asp:PlaceHolder ID="phProposta" runat="server" Visible="false">
                    <span class="text-muted small">Proposta de Origem</span>
                    <div class="fw-semibold"><asp:HyperLink ID="lnkProposta" runat="server" /></div>
                </asp:PlaceHolder>
            </div>

            <div class="col-md-3"><span class="text-muted small">Método de Pagamento</span><div class="fw-semibold"><asp:Label ID="lblMetodoPagamento" runat="server" /></div></div>
            <div class="col-md-3"><span class="text-muted small">Data de Vencimento</span><div class="fw-semibold"><asp:Label ID="lblDataVencimento" runat="server" /></div></div>
            <div class="col-md-3"><span class="text-muted small">Comissão</span><div class="fw-semibold"><asp:Label ID="lblComissao" runat="server" /></div></div>

            <div class="col-12">
                <asp:PlaceHolder ID="phMotivoCancelamento" runat="server" Visible="false">
                    <div class="alert alert-secondary mb-0">
                        <strong>Cancelada.</strong> Motivo: <asp:Label ID="lblMotivoCancelamento" runat="server" />
                    </div>
                </asp:PlaceHolder>
            </div>
        </div>
    </div>

    <div class="crm-table-card mt-3">
        <div class="p-3 pb-0"><h5 class="mb-0">Linhas</h5></div>
        <asp:Repeater ID="rptLinhas" runat="server">
            <HeaderTemplate>
                <table class="table mb-0 align-middle">
                    <thead>
                        <tr><th>Produto</th><th>Descrição</th><th>Qtd.</th><th>Preço Unit.</th><th>Desc. %</th><th>IVA</th><th>Total Linha</th></tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# Eval("Product.Name") %></td>
                    <td><%# Eval("Description") %></td>
                    <td><%# Eval("Quantity") %></td>
                    <td><%# Eval("UnitPrice", "{0:C}") %></td>
                    <td><%# Eval("DiscountPercent") %>%</td>
                    <td><%# Eval("TaxRate.Percentage") %>%</td>
                    <td><%# Eval("LineTotal", "{0:C}") %></td>
                </tr>
            </ItemTemplate>
            <FooterTemplate></tbody></table></FooterTemplate>
        </asp:Repeater>
    </div>

    <div class="crm-filter-card mt-3" style="max-width: 340px; margin-left: auto;">
        <div class="d-flex justify-content-between"><span>Subtotal</span><asp:Label ID="lblSubTotal" runat="server" /></div>
        <div class="d-flex justify-content-between"><span>IVA</span><asp:Label ID="lblIvaTotal" runat="server" /></div>
        <div class="d-flex justify-content-between fw-bold"><span>Total</span><asp:Label ID="lblTotalGeral" runat="server" /></div>
    </div>

    <!-- ===================== Pagamentos ===================== -->
    <div class="crm-table-card mt-4">
        <div class="p-3 pb-0 d-flex justify-content-between align-items-center">
            <h5 class="mb-0">Pagamentos</h5>
            <asp:HyperLink ID="lnkGerirPagamentos" runat="server" CssClass="btn btn-sm btn-outline-primary" Visible="false">Gerir Pagamentos</asp:HyperLink>
        </div>
        <asp:Repeater ID="rptPagamentos" runat="server">
            <HeaderTemplate>
                <table class="table mb-0 align-middle">
                    <thead><tr><th>Data</th><th>Valor</th><th>Método</th><th>Referência</th></tr></thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# Eval("PaymentDate", "{0:dd/MM/yyyy}") %></td>
                    <td><%# Eval("Amount", "{0:C}") %></td>
                    <td><%# Eval("PaymentMethod") %></td>
                    <td><%# Eval("Reference") %></td>
                </tr>
            </ItemTemplate>
            <FooterTemplate></tbody></table></FooterTemplate>
        </asp:Repeater>
        <asp:PlaceHolder ID="phSemPagamentos" runat="server" Visible="false">
            <div class="crm-empty-state"><p class="mb-0 text-muted small p-3">Ainda sem pagamentos registados.</p></div>
        </asp:PlaceHolder>
        <div class="p-3 border-top d-flex justify-content-between">
            <span>Total Pago</span><span class="fw-bold"><asp:Label ID="lblTotalPago" runat="server" /></span>
        </div>
        <div class="px-3 pb-3 d-flex justify-content-between">
            <span>Saldo em Falta</span><span class="fw-bold"><asp:Label ID="lblSaldo" runat="server" /></span>
        </div>
    </div>

    <!-- ===================== Ações de ciclo de vida ===================== -->
    <asp:PlaceHolder ID="phCancelar" runat="server" Visible="false">
        <div class="crm-filter-card mt-4">
            <h5>Cancelar Venda</h5>
            <label class="form-label small">Motivo *</label>
            <asp:TextBox ID="txtMotivoCancelamento" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" />
            <asp:Button ID="btnCancelar" runat="server" Text="Cancelar Venda" CssClass="btn btn-outline-danger mt-2"
                OnClick="btnCancelar_Click" CausesValidation="false"
                OnClientClick="return confirm('Tens a certeza que queres cancelar esta venda? Esta ação não elimina o histórico, mas não pode ser revertida.');" />
        </div>
    </asp:PlaceHolder>

    <div class="mt-3 d-flex gap-2">
        <asp:HyperLink ID="lnkEditar" runat="server" CssClass="btn btn-outline-secondary" Visible="false">Editar</asp:HyperLink>
        <asp:Button ID="btnConfirmar" runat="server" Text="Confirmar Venda" CssClass="btn btn-primary"
            OnClick="btnConfirmar_Click" CausesValidation="false" Visible="false" />
        <a href="VendasLista.aspx" class="btn btn-outline-secondary">Voltar</a>
    </div>

    <!-- ===================== Anexos e Histórico ===================== -->
    <div class="row mt-4">
        <div class="col-md-7">
            <h5>Documentos</h5>
            <uc:Anexos ID="ucAnexos" runat="server" />
        </div>
        <div class="col-md-5">
            <h5>Histórico</h5>
            <uc:Historico ID="ucHistorico" runat="server" />
        </div>
    </div>

</asp:Content>