<%@ Page Title="Tabelas de Preço" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="TabelasPreco.aspx.cs" Inherits="CRM.Web.Paginas.Catalogo.TabelasPreco" %>

<%@ Register TagPrefix="uc" TagName="SeletorProduto" Src="~/Controls/SeletorProduto.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item"><a href="~/Catalogo/ProdutosLista.aspx" runat="server">Catálogo</a></li>
    <li class="breadcrumb-item active">Tabelas de Preço</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="crm-list-header">
        <h2>Tabelas de Preço</h2>
    </div>

    <asp:PlaceHolder ID="phFormularioTabela" runat="server">
        <div class="crm-form-card">
            <asp:ValidationSummary ID="valSummaryTabela" runat="server" CssClass="alert alert-danger" DisplayMode="BulletList" ValidationGroup="Tabela" />
            <asp:CustomValidator ID="cvRegrasTabela" runat="server" Display="None" ValidationGroup="Tabela" OnServerValidate="cvRegrasTabela_ServerValidate" />
            <asp:Literal ID="litModoEdicaoTabela" runat="server" CssClass="text-muted small d-block mb-2" Visible="false" />

            <div class="row g-2 align-items-end">
                <div class="col-md-4">
                    <label class="form-label">Nome</label>
                    <asp:TextBox ID="txtNomeTabela" runat="server" CssClass="form-control" MaxLength="100" placeholder="ex: Tabela Revenda" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtNomeTabela" ValidationGroup="Tabela"
                        CssClass="text-danger small" ErrorMessage="O nome é obrigatório." Display="Dynamic" />
                </div>
                <div class="col-md-auto">
                    <div class="form-check mb-2">
                        <asp:CheckBox ID="chkPredefinida" runat="server" CssClass="form-check-input" />
                        <label class="form-check-label">Tabela predefinida</label>
                    </div>
                </div>
                <div class="col-md-auto">
                    <asp:Button ID="btnGuardarTabela" runat="server" Text="Guardar" CssClass="btn btn-primary" ValidationGroup="Tabela" OnClick="btnGuardarTabela_Click" />
                </div>
                <div class="col-md-auto">
                    <asp:Button ID="btnCancelarTabela" runat="server" Text="Cancelar" CssClass="btn btn-outline-secondary"
                        OnClick="btnCancelarTabela_Click" CausesValidation="false" Visible="false" />
                </div>
            </div>
        </div>
    </asp:PlaceHolder>

    <div class="crm-table-card mb-4">
        <asp:Repeater ID="rptTabelas" runat="server" OnItemCommand="rptTabelas_ItemCommand">
            <HeaderTemplate>
                <table class="table table-hover mb-0 align-middle">
                    <thead>
                        <tr>
                            <th>Nome</th>
                            <th>Predefinida</th>
                            <th>Estado</th>
                            <th class="text-end">Ações</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="<%# IsTabelaSelecionada(Eval("PriceTableId")) ? "table-active" : "" %>">
                    <td><%# Eval("Name") %></td>
                    <td>
                        <asp:PlaceHolder runat="server" Visible='<%# (bool)Eval("IsDefault") %>'>
                            <span class="badge bg-primary">Predefinida</span>
                        </asp:PlaceHolder>
                    </td>
                    <td>
                        <span class="badge <%# (bool)Eval("IsActive") ? "bg-success" : "bg-secondary" %>">
                            <%# (bool)Eval("IsActive") ? "Ativa" : "Inativa" %>
                        </span>
                    </td>
                    <td class="text-end crm-row-actions">
                        <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-primary" ToolTip="Gerir Preços"
                            CommandName="GerirItens" CommandArgument='<%# Eval("PriceTableId") %>'>
                            <i class="fas fa-euro-sign"></i> Preços
                        </asp:LinkButton>
                        <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-secondary" ToolTip="Editar"
                            CommandName="Editar" CommandArgument='<%# Eval("PriceTableId") %>'>
                            <i class="fas fa-pen"></i>
                        </asp:LinkButton>
                        <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-warning" ToolTip="Ativar/Desativar"
                            CommandName="AlternarEstado" CommandArgument='<%# Eval("PriceTableId") %>'
                            data-confirm='<%# ((bool)Eval("IsActive") ? "Desativar" : "Ativar") + " a tabela " + Eval("Name") + "?" %>'>
                            <i class="fas fa-power-off"></i>
                        </asp:LinkButton>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                    </tbody>
                </table>
            </FooterTemplate>
        </asp:Repeater>

        <asp:PlaceHolder ID="phVazioTabelas" runat="server" Visible="false">
            <div class="crm-empty-state">
                <i class="fas fa-euro-sign"></i>
                <p class="mb-0">Ainda não existem tabelas de preço.</p>
            </div>
        </asp:PlaceHolder>
    </div>

    <!-- ===================== Itens da tabela selecionada ===================== -->
    <asp:PlaceHolder ID="phItens" runat="server" Visible="false">
        <div class="crm-list-header">
            <h4 class="mb-0">Preços — <asp:Literal ID="litNomeTabelaItens" runat="server" /></h4>
            <asp:LinkButton ID="lnkFecharItens" runat="server" CssClass="btn btn-outline-secondary btn-sm" OnClick="lnkFecharItens_Click" CausesValidation="false">
                Fechar
            </asp:LinkButton>
        </div>

        <div class="crm-form-card">
            <asp:ValidationSummary ID="valSummaryItem" runat="server" CssClass="alert alert-danger" DisplayMode="BulletList" ValidationGroup="Item" />
            <asp:CustomValidator ID="cvRegrasItem" runat="server" Display="None" ValidationGroup="Item" OnServerValidate="cvRegrasItem_ServerValidate" />
            <asp:Literal ID="litModoEdicaoItem" runat="server" CssClass="text-muted small d-block mb-2" Visible="false" />

            <div class="row g-2 align-items-end">
                <div class="col-md-5">
                    <label class="form-label">Produto</label>
                    <uc:SeletorProduto ID="ucSeletorProduto" runat="server" />
                </div>
                <div class="col-md-3">
                    <label class="form-label">Preço (€)</label>
                    <asp:TextBox ID="txtPrecoItem" runat="server" CssClass="form-control" TextMode="Number" step="0.01" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPrecoItem" ValidationGroup="Item"
                        CssClass="text-danger small" ErrorMessage="O preço é obrigatório." Display="Dynamic" />
                </div>
                <div class="col-md-auto">
                    <asp:Button ID="btnGuardarItem" runat="server" Text="Adicionar" CssClass="btn btn-primary" ValidationGroup="Item" OnClick="btnGuardarItem_Click" />
                </div>
                <div class="col-md-auto">
                    <asp:Button ID="btnCancelarItem" runat="server" Text="Cancelar" CssClass="btn btn-outline-secondary"
                        OnClick="btnCancelarItem_Click" CausesValidation="false" Visible="false" />
                </div>
            </div>
        </div>

        <div class="crm-table-card">
            <asp:Repeater ID="rptItens" runat="server" OnItemCommand="rptItens_ItemCommand">
                <HeaderTemplate>
                    <table class="table table-hover mb-0 align-middle">
                        <thead>
                            <tr>
                                <th>Código</th>
                                <th>Produto</th>
                                <th>Categoria</th>
                                <th class="text-end">Preço Base</th>
                                <th class="text-end">Preço Nesta Tabela</th>
                                <th class="text-end">Ações</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td class="mono"><%# Eval("Product.Code") %></td>
                        <td><%# Eval("Product.Name") %></td>
                        <td><%# Eval("Product.Category.Name") %></td>
                        <td class="text-end"><%# Eval("Product.BasePrice", "{0:C}") %></td>
                        <td class="text-end fw-semibold"><%# Eval("Price", "{0:C}") %></td>
                        <td class="text-end crm-row-actions">
                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-secondary" ToolTip="Editar Preço"
                                CommandName="Editar" CommandArgument='<%# Eval("PriceTableItemId") %>'>
                                <i class="fas fa-pen"></i>
                            </asp:LinkButton>
                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-danger" ToolTip="Remover"
                                CommandName="Eliminar" CommandArgument='<%# Eval("PriceTableItemId") %>'
                                data-confirm='<%# "Remover " + Eval("Product.Name") + " desta tabela?" %>'>
                                <i class="fas fa-trash"></i>
                            </asp:LinkButton>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                        </tbody>
                    </table>
                </FooterTemplate>
            </asp:Repeater>

            <asp:PlaceHolder ID="phVazioItens" runat="server" Visible="false">
                <div class="crm-empty-state">
                    <i class="fas fa-box-open"></i>
                    <p class="mb-0">Ainda não há preços definidos nesta tabela.</p>
                </div>
            </asp:PlaceHolder>
        </div>
    </asp:PlaceHolder>

</asp:Content>