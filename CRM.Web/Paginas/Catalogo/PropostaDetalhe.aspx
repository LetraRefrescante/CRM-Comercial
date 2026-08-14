<%@ Page Title="Proposta" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="PropostaDetalhe.aspx.cs" Inherits="CRM.Web.Paginas.Catalogo.PropostaDetalhe" %>
<%@ Register TagPrefix="uc" TagName="Anexos" Src="~/Controls/Anexos.ascx" %>
<%@ Register TagPrefix="uc" TagName="Historico" Src="~/Controls/Historico.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item"><a href="PropostasLista.aspx">Propostas</a></li>
    <li class="breadcrumb-item active">Detalhe</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="crm-list-header">
        <h2>Proposta <asp:Label ID="lblNumero" runat="server" CssClass="text-muted fs-6" /></h2>
        <span id="spanStatus" runat="server" class="badge bg-secondary"></span>
    </div>

    <div class="crm-filter-card">
        <div class="row g-3">
            <div class="col-md-3"><span class="text-muted small">Cliente</span><div class="fw-semibold"><asp:Label ID="lblCliente" runat="server" /></div></div>
            <div class="col-md-3"><span class="text-muted small">Data de Emissão</span><div class="fw-semibold"><asp:Label ID="lblEmissao" runat="server" /></div></div>
            <div class="col-md-3"><span class="text-muted small">Validade</span><div class="fw-semibold"><asp:Label ID="lblValidade" runat="server" /></div></div>
            <div class="col-md-3"><span class="text-muted small">Comercial</span><div class="fw-semibold"><asp:Label ID="lblComercial" runat="server" /></div></div>
            <div class="col-12">
                <asp:PlaceHolder ID="phNotas" runat="server" Visible="false">
                    <span class="text-muted small">Notas</span>
                    <div><asp:Label ID="lblNotas" runat="server" /></div>
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

    <!-- ===================== Histórico de Versões ===================== -->
    <asp:PlaceHolder ID="phVersoes" runat="server" Visible="false">
        <div class="crm-table-card mt-4">
            <div class="p-3 pb-0"><h5 class="mb-0">Versões</h5></div>
            <asp:Repeater ID="rptVersoes" runat="server">
                <HeaderTemplate>
                    <table class="table mb-0 align-middle">
                        <thead><tr><th>Versão</th><th>Estado</th><th>Emissão</th><th>Total</th><th></th></tr></thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr class="<%# GetVersaoRowClass(Eval("ProposalId")) %>">
                        <td>v<%# Eval("VersionNumber") %></td>
                        <td><span class="badge <%# GetBadgeClasse(Eval("Status").ToString()) %>"><%# Eval("Status") %></span></td>
                        <td><%# Eval("IssueDate", "{0:dd/MM/yyyy}") %></td>
                        <td><%# Eval("Total", "{0:C}") %></td>
                        <td><a href="PropostaDetalhe.aspx?id=<%# Eval("ProposalId") %>" class="btn btn-sm btn-outline-secondary">Abrir</a></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate></tbody></table></FooterTemplate>
            </asp:Repeater>
        </div>
    </asp:PlaceHolder>

    <!-- ===================== Ações: Aceitar / Recusar ===================== -->
    <asp:PlaceHolder ID="phAceitarRecusar" runat="server" Visible="false">
        <div class="crm-filter-card mt-4">
            <h5>Resposta do Cliente</h5>
            <div class="row g-3">
                <div class="col-md-6">
                    <label class="form-label small">Observação de aceitação</label>
                    <asp:TextBox ID="txtObservacaoAceitacao" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" />
                    <asp:Button ID="btnAceitar" runat="server" Text="Aceitar" CssClass="btn btn-success mt-2"
                        OnClick="btnAceitar_Click" CausesValidation="false" />
                </div>
                <div class="col-md-6">
                    <label class="form-label small">Motivo de recusa</label>
                    <asp:TextBox ID="txtMotivoRecusa" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" />
                    <asp:Button ID="btnRecusar" runat="server" Text="Recusar" CssClass="btn btn-outline-danger mt-2"
                        OnClick="btnRecusar_Click" CausesValidation="false"
                        OnClientClick="return confirm('Tens a certeza que queres recusar esta proposta?');" />
                </div>
            </div>
        </div>
    </asp:PlaceHolder>

    <!-- ===================== Info de aceitação ===================== -->
    <asp:PlaceHolder ID="phInfoAceitacao" runat="server" Visible="false">
        <div class="alert alert-success mt-4">
            Aceite em <asp:Label ID="lblDataAceitacao" runat="server" /> por <asp:Label ID="lblQuemAceitou" runat="server" />.
            <asp:Label ID="lblObservacaoAceitacao" runat="server" />
        </div>
    </asp:PlaceHolder>

    <div class="mt-3 d-flex gap-2">
        <asp:HyperLink ID="lnkEditar" runat="server" CssClass="btn btn-outline-secondary">Editar</asp:HyperLink>
        <asp:Button ID="btnNovaVersao" runat="server" Text="Criar Nova Versão" CssClass="btn btn-outline-primary"
            OnClick="btnNovaVersao_Click" CausesValidation="false" Visible="false" />
        <asp:HyperLink ID="lnkEnviar" runat="server" CssClass="btn btn-primary" Visible="false">Enviar Proposta</asp:HyperLink>
        <asp:HyperLink ID="lnkVerPdf" runat="server" CssClass="btn btn-outline-secondary" Target="_blank">Ver PDF</asp:HyperLink>
        <asp:HyperLink ID="lnkCriarVenda" runat="server" CssClass="btn btn-success" Visible="false">Criar Venda</asp:HyperLink>
        <a href="PropostasLista.aspx" class="btn btn-outline-secondary">Voltar</a>
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