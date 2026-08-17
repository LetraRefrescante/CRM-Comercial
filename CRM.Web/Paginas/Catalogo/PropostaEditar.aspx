<%@ Page Title="Proposta" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="PropostaEditar.aspx.cs" Inherits="CRM.Web.Paginas.Catalogo.PropostaEditar" %>

<%@ Register TagPrefix="uc" TagName="SeletorCliente" Src="~/Controls/SeletorCliente.ascx" %>
<%@ Register TagPrefix="uc" TagName="SeletorProduto" Src="~/Controls/SeletorProduto.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item"><a href="~/Catalogo/PropostasLista.aspx" runat="server">Propostas</a></li>
    <li class="breadcrumb-item active"><%: TituloPagina %></li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="crm-list-header">
        <h2><%: TituloPagina %></h2>
        <asp:PlaceHolder ID="phBadgeEstado" runat="server" Visible="false">
            <span id="spanEstado" runat="server" class="badge fs-6"></span>
        </asp:PlaceHolder>
    </div>

    <asp:PlaceHolder ID="phSoLeituraAviso" runat="server" Visible="false">
        <div class="alert alert-warning">
            <i class="fas fa-lock"></i>
            Esta proposta já não está em Rascunho, por isso não pode ser editada diretamente.
            Cria uma nova versão para alterar linhas, condições ou datas.
        </div>
    </asp:PlaceHolder>

    <asp:ValidationSummary ID="valSummary" runat="server" CssClass="alert alert-danger" DisplayMode="BulletList" ValidationGroup="Cabecalho" />
    <asp:CustomValidator ID="cvRegrasNegocio" runat="server" Display="None" ValidationGroup="Cabecalho" OnServerValidate="cvRegrasNegocio_ServerValidate" />

    <!-- ===================== Cabeçalho ===================== -->
    <div class="crm-form-card mb-3">
        <div class="row g-3">
            <asp:PlaceHolder ID="phNumero" runat="server" Visible="false">
                <div class="col-md-3">
                    <label class="form-label">Número</label>
                    <asp:TextBox ID="txtNumero" runat="server" CssClass="form-control mono" ReadOnly="true" TabIndex="-1" />
                </div>
            </asp:PlaceHolder>

            <div class="col-md-6">
                <label class="form-label">Cliente *</label>
                <uc:SeletorCliente ID="ucCliente" runat="server" OnClienteSelecionado="ucCliente_ClienteSelecionado" />
            </div>

            <div class="col-md-3">
                <label class="form-label">Oportunidade</label>
                <asp:DropDownList ID="ddlOportunidade" runat="server" CssClass="form-select">
                    <asp:ListItem Text="(Sem oportunidade)" Value="" />
                </asp:DropDownList>
            </div>

            <div class="col-md-3">
                <label class="form-label">Data Emissão *</label>
                <asp:TextBox ID="txtDataEmissao" runat="server" CssClass="form-control" TextMode="Date" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDataEmissao" ValidationGroup="Cabecalho"
                    CssClass="text-danger small" ErrorMessage="A data de emissão é obrigatória." Display="Dynamic" />
            </div>
            <div class="col-md-3">
                <label class="form-label">Validade *</label>
                <asp:TextBox ID="txtValidade" runat="server" CssClass="form-control" TextMode="Date" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtValidade" ValidationGroup="Cabecalho"
                    CssClass="text-danger small" ErrorMessage="A validade é obrigatória." Display="Dynamic" />
            </div>
            <div class="col-md-3">
                <label class="form-label">Desconto Global (%)</label>
                <asp:TextBox ID="txtDescontoGlobal" runat="server" CssClass="form-control" TextMode="Number" step="0.01" Text="0" AutoPostBack="true" OnTextChanged="CamposCabecalho_TextChanged" />
            </div>
            <div class="col-md-3">
                <label class="form-label">Condições de Pagamento</label>
                <asp:DropDownList ID="ddlCondicoesPagamento" runat="server" CssClass="form-select">
                    <asp:ListItem Text="(Nenhuma)" Value="" />
                </asp:DropDownList>
            </div>

            <div class="col-12">
                <label class="form-label">Notas</label>
                <asp:TextBox ID="txtNotas" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" MaxLength="4000" />
                <div class="form-text">Visíveis no PDF enviado ao cliente.</div>
            </div>
        </div>
    </div>

    <!-- ===================== Linhas ===================== -->
    <div class="crm-list-header">
        <h4 class="mb-0">Linhas</h4>
    </div>

    <asp:PlaceHolder ID="phFormularioLinha" runat="server">
        <div class="crm-form-card mb-3">
            <asp:ValidationSummary ID="valSummaryLinha" runat="server" CssClass="alert alert-danger" DisplayMode="BulletList" ValidationGroup="Linha" />
            <asp:CustomValidator ID="cvRegrasLinha" runat="server" Display="None" ValidationGroup="Linha" OnServerValidate="cvRegrasLinha_ServerValidate" />
            <asp:Literal ID="litModoEdicaoLinha" runat="server" CssClass="text-muted small d-block mb-2" Visible="false" />

            <div class="row g-2 align-items-end">
                <div class="col-md-3">
                    <label class="form-label small">Produto</label>
                    <uc:SeletorProduto ID="ucSeletorProduto" runat="server" OnProdutoSelecionado="ucSeletorProduto_ProdutoSelecionado" />
                </div>
                <div class="col-md-3">
                    <label class="form-label small">Descrição</label>
                    <asp:TextBox ID="txtDescricaoLinha" runat="server" CssClass="form-control" MaxLength="300" />
                </div>
                <div class="col-md-1">
                    <label class="form-label small">Qtd.</label>
                    <asp:TextBox ID="txtQuantidadeLinha" runat="server" CssClass="form-control" TextMode="Number" step="0.01" Text="1" />
                </div>
                <div class="col-md-2">
                    <label class="form-label small">Preço Unit. (€)</label>
                    <asp:TextBox ID="txtPrecoLinha" runat="server" CssClass="form-control" TextMode="Number" step="0.01" />
                </div>
                <div class="col-md-1">
                    <label class="form-label small">Desc. (%)</label>
                    <asp:TextBox ID="txtDescontoLinha" runat="server" CssClass="form-control" TextMode="Number" step="0.01" Text="0" />
                </div>
                <div class="col-md-1">
                    <label class="form-label small">IVA</label>
                    <asp:DropDownList ID="ddlTaxaIvaLinha" runat="server" CssClass="form-select" />
                </div>
                <div class="col-md-1">
                    <asp:Button ID="btnGuardarLinha" runat="server" Text="Adicionar" CssClass="btn btn-primary w-100" ValidationGroup="Linha" OnClick="btnGuardarLinha_Click" />
                </div>
            </div>
            <div class="mt-2">
                <asp:Button ID="btnCancelarLinha" runat="server" Text="Cancelar edição da linha" CssClass="btn btn-sm btn-outline-secondary"
                    OnClick="btnCancelarLinha_Click" CausesValidation="false" Visible="false" />
            </div>
        </div>
    </asp:PlaceHolder>

    <div class="crm-table-card mb-3">
        <asp:Repeater ID="rptLinhas" runat="server" OnItemCommand="rptLinhas_ItemCommand">
            <HeaderTemplate>
                <table class="table table-hover mb-0 align-middle">
                    <thead>
                        <tr>
                            <th>Produto</th>
                            <th>Descrição</th>
                            <th class="text-end">Qtd.</th>
                            <th class="text-end">Preço Unit.</th>
                            <th class="text-end">Desc.</th>
                            <th>IVA</th>
                            <th class="text-end">Total Linha</th>
                            <th class="text-end">Ações</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td class="mono small"><%# Eval("ProductCode") %></td>
                    <td><%# Eval("Description") %></td>
                    <td class="text-end"><%# Eval("Quantity") %></td>
                    <td class="text-end"><%# Eval("UnitPrice", "{0:C}") %></td>
                    <td class="text-end"><%# Eval("DiscountPercent") %>%</td>
                    <td><%# Eval("TaxRateName") %></td>
                    <td class="text-end fw-semibold"><%# Eval("LineTotal", "{0:C}") %></td>
                    <td class="text-end crm-row-actions">
                        <asp:PlaceHolder runat="server" Visible='<%# PodeEditarLinhas %>'>
                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-secondary" ToolTip="Editar"
                                CommandName="Editar" CommandArgument="<%# Container.ItemIndex %>">
                                <i class="fas fa-pen"></i>
                            </asp:LinkButton>
                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-danger" ToolTip="Remover"
                                CommandName="Eliminar" CommandArgument="<%# Container.ItemIndex %>"
                                data-confirm="Remover esta linha da proposta?">
                                <i class="fas fa-trash"></i>
                            </asp:LinkButton>
                        </asp:PlaceHolder>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                    </tbody>
                </table>
            </FooterTemplate>
        </asp:Repeater>

        <asp:PlaceHolder ID="phVazioLinhas" runat="server" Visible="false">
            <div class="crm-empty-state">
                <i class="fas fa-list"></i>
                <p class="mb-0">Ainda não há linhas nesta proposta.</p>
            </div>
        </asp:PlaceHolder>
    </div>

    <!-- ===================== Totais ===================== -->
    <div class="crm-form-card mb-3">
        <div class="row g-2 justify-content-end text-end">
            <div class="col-md-3">
                <div class="text-muted small">Subtotal (com desconto)</div>
                <div class="fs-5"><asp:Literal ID="litSubTotal" runat="server" /></div>
            </div>
            <div class="col-md-3">
                <div class="text-muted small">IVA</div>
                <div class="fs-5"><asp:Literal ID="litTaxTotal" runat="server" /></div>
            </div>
            <div class="col-md-3">
                <div class="text-muted small">Total</div>
                <div class="fs-4 fw-bold"><asp:Literal ID="litTotal" runat="server" /></div>
            </div>
        </div>
        <div class="form-text text-end">Os totais recalculam ao adicionar, editar ou remover linhas, ou ao mudar o desconto global.</div>
    </div>

    <div class="d-flex gap-2">
        <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-primary" OnClick="btnGuardar_Click" />
        <asp:Button ID="btnCriarNovaVersao" runat="server" Text="Criar Nova Versão para Editar" CssClass="btn btn-primary" Visible="false"
            OnClick="btnCriarNovaVersao_Click" CausesValidation="false" />
        <asp:HyperLink ID="lnkCancelar" runat="server" NavigateUrl="~/Catalogo/PropostasLista.aspx" CssClass="btn btn-outline-secondary">Cancelar</asp:HyperLink>
    </div>

</asp:Content>