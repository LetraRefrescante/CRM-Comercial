<%@ Page Title="Proposta" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="PropostaEditar.aspx.cs" Inherits="CRM.Web.Paginas.Catalogo.PropostaEditar" %>
<%@ Register TagPrefix="uc" TagName="SeletorProduto" Src="~/Controls/SeletorProduto.ascx" %>
<%@ Register TagPrefix="uc" TagName="SeletorCliente" Src="~/Controls/SeletorCliente.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item"><a href="PropostasLista.aspx">Propostas</a></li>
    <li class="breadcrumb-item active">Editar</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="crm-list-header">
        <h2>Proposta <asp:Label ID="lblNumero" runat="server" CssClass="text-muted fs-6" /></h2>
        <span id="spanStatus" runat="server" class="badge bg-secondary"></span>
    </div>

    <asp:PlaceHolder ID="phAvisoSoLeitura" runat="server" Visible="false">
        <div class="alert alert-warning">
            Esta proposta não está em Rascunho e não pode ser editada diretamente. Usa "Criar Nova Versão" para propor alterações.
        </div>
    </asp:PlaceHolder>

    <asp:Panel ID="pnlCamposEditaveis" runat="server" CssClass="crm-filter-card">
        <div class="row g-3">
            <div class="col-md-4">
                <label class="form-label">Cliente *</label>
                <uc:SeletorCliente ID="ucCliente" runat="server" OnClienteSelecionado="ucCliente_ClienteSelecionado" />
            </div>
            <div class="col-md-4">
                <label class="form-label">Oportunidade</label>
                <asp:DropDownList ID="ddlOportunidade" runat="server" CssClass="form-select" />
            </div>
            <div class="col-md-4">
                <label class="form-label">Condições de Pagamento</label>
                <asp:DropDownList ID="ddlCondicaoPagamento" runat="server" CssClass="form-select" />
            </div>
            <div class="col-md-3">
                <label class="form-label">Data de Emissão *</label>
                <asp:TextBox ID="txtEmissao" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-3">
                <label class="form-label">Validade *</label>
                <asp:TextBox ID="txtValidade" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-3">
                <label class="form-label">Desconto Global (%)</label>
                <asp:TextBox ID="txtDescontoGlobal" runat="server" CssClass="form-control" Text="0"
                    AutoPostBack="true" OnTextChanged="txtDescontoGlobal_TextChanged" />
            </div>
            <div class="col-12">
                <label class="form-label">Notas</label>
                <asp:TextBox ID="txtNotas" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
            </div>
        </div>
    </asp:Panel>

    <div class="crm-table-card mt-3">
        <div class="d-flex justify-content-between align-items-center p-3 pb-0">
            <h5 class="mb-0">Linhas</h5>
            <asp:Button ID="btnAdicionarLinha" runat="server" Text="+ Adicionar Linha" CssClass="btn btn-outline-primary btn-sm"
                OnClick="btnAdicionarLinha_Click" CausesValidation="false" />
        </div>

        <asp:Repeater ID="rptLinhas" runat="server" OnItemDataBound="rptLinhas_ItemDataBound" OnItemCommand="rptLinhas_ItemCommand">
            <HeaderTemplate>
                <table class="table mb-0 align-middle">
                    <thead>
                        <tr>
                            <th style="width:24%">Produto</th>
                            <th style="width:20%">Descrição</th>
                            <th style="width:10%">Qtd.</th>
                            <th style="width:12%">Preço Unit.</th>
                            <th style="width:10%">Desc. %</th>
                            <th style="width:8%">IVA</th>
                            <th style="width:12%">Total Linha</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td>
                        <uc:SeletorProduto ID="ucProduto" runat="server" TextoBotao="Escolher" IconeBotao="fa-box" />
                        <asp:HiddenField ID="hdnProposalLineId" runat="server" Value='<%# Eval("ProposalLineId") %>' />
                        <asp:HiddenField ID="hdnUnitPrice" runat="server" />
                        <asp:HiddenField ID="hdnTaxRateId" runat="server" />
                    </td>
                    <td><asp:TextBox ID="txtDescricao" runat="server" CssClass="form-control form-control-sm" Text='<%# Eval("Description") %>' /></td>
                    <td><asp:TextBox ID="txtQuantidade" runat="server" CssClass="form-control form-control-sm" Text='<%# Eval("Quantity") %>'
                            AutoPostBack="true" OnTextChanged="txtLinha_TextChanged" /></td>
                    <td><asp:Label ID="lblPrecoUnit" runat="server" /></td>
                    <td><asp:TextBox ID="txtDesconto" runat="server" CssClass="form-control form-control-sm" Text='<%# Eval("DiscountPercent") %>'
                            AutoPostBack="true" OnTextChanged="txtLinha_TextChanged" /></td>
                    <td><asp:Label ID="lblIva" runat="server" /></td>
                    <td><asp:Label ID="lblTotalLinha" runat="server" /></td>
                    <td>
                        <asp:LinkButton ID="lnkRemover" runat="server" CommandName="Remover" CssClass="btn btn-sm btn-outline-danger" CausesValidation="false">
                            <i class="fas fa-times"></i>
                        </asp:LinkButton>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                    </tbody>
                </table>
            </FooterTemplate>
        </asp:Repeater>

        <asp:PlaceHolder ID="phSemLinhas" runat="server" Visible="false">
            <div class="crm-empty-state">
                <p class="mb-0">Ainda não há linhas nesta proposta.</p>
            </div>
        </asp:PlaceHolder>
    </div>

    <div class="crm-filter-card mt-3" style="max-width: 340px; margin-left: auto;">
        <div class="d-flex justify-content-between"><span>Subtotal</span><asp:Label ID="lblSubTotal" runat="server" /></div>
        <div class="d-flex justify-content-between"><span>IVA</span><asp:Label ID="lblIvaTotal" runat="server" /></div>
        <div class="d-flex justify-content-between fw-bold"><span>Total</span><asp:Label ID="lblTotalGeral" runat="server" /></div>
    </div>

    <asp:CustomValidator ID="cvLinhas" runat="server" Display="Dynamic" CssClass="text-danger d-block mt-2"
        ErrorMessage="A proposta tem de ter pelo menos uma linha válida." OnServerValidate="cvLinhas_ServerValidate" />
    <asp:ValidationSummary ID="vsResumo" runat="server" CssClass="alert alert-danger mt-2" />

    <div class="mt-3">
        <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-primary" OnClick="btnGuardar_Click" />
        <asp:Button ID="btnCriarNovaVersao" runat="server" Text="Criar Nova Versão" CssClass="btn btn-outline-primary"
            OnClick="btnCriarNovaVersao_Click" CausesValidation="false" />
        <a href="PropostasLista.aspx" class="btn btn-outline-secondary">Cancelar</a>
    </div>

</asp:Content>