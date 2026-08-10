<%@ Page Title="Preços da Tabela" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="TabelaPrecoDetalhe.aspx.cs" Inherits="CRM.Web.Paginas.Catalogo.TabelaPrecoDetalhe" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item"><a href="~/Catalogo/ProdutosLista.aspx" runat="server">Catálogo</a></li>
    <li class="breadcrumb-item"><a href="~/Catalogo/TabelasPreco.aspx" runat="server">Tabelas de Preço</a></li>
    <li class="breadcrumb-item active"><%: NomeTabela %></li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="crm-list-header">
        <h2><%: NomeTabela %></h2>
    </div>

    <asp:PlaceHolder ID="phFormulario" runat="server">
        <div class="crm-form-card">
            <asp:ValidationSummary ID="valSummary" runat="server" CssClass="alert alert-danger" DisplayMode="BulletList" />
            <asp:CustomValidator ID="cvRegrasNegocio" runat="server" Display="None" OnServerValidate="cvRegrasNegocio_ServerValidate" />
            <asp:Literal ID="litModoEdicao" runat="server" CssClass="text-muted small d-block mb-2" Visible="false" />

            <div class="row g-2 align-items-end">
                <div class="col-md-5">
                    <label class="form-label">Produto</label>
                    <asp:DropDownList ID="ddlProduto" runat="server" CssClass="form-select" />
                    <asp:Literal ID="litProdutoSelecionado" runat="server" CssClass="form-control-plaintext fw-semibold" Visible="false" />
                </div>
                <div class="col-md-3">
                    <label class="form-label">Preço (€)</label>
                    <asp:TextBox ID="txtPreco" runat="server" CssClass="form-control" TextMode="Number" step="0.01" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPreco" CssClass="text-danger small"
                        ErrorMessage="O preço é obrigatório." Display="Dynamic" />
                    <asp:RangeValidator runat="server" ControlToValidate="txtPreco" CssClass="text-danger small"
                        ErrorMessage="O preço não pode ser negativo." Display="Dynamic"
                        Type="Currency" MinimumValue="0" MaximumValue="999999999" />
                </div>
                <div class="col-md-auto">
                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-primary" OnClick="btnGuardar_Click" />
                </div>
                <div class="col-md-auto">
                    <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="btn btn-outline-secondary"
                        OnClick="btnCancelar_Click" CausesValidation="false" Visible="false" />
                </div>
            </div>
        </div>
    </asp:PlaceHolder>

    <div class="crm-table-card">
        <asp:Repeater ID="rptItens" runat="server" OnItemCommand="rptItens_ItemCommand">
            <HeaderTemplate>
                <table class="table table-hover mb-0 align-middle">
                    <thead>
                        <tr>
                            <th>Produto</th>
                            <th>Categoria</th>
                            <th>Preço Base</th>
                            <th>Preço nesta Tabela</th>
                            <th class="text-end">Ações</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# Eval("Product.Name") %></td>
                    <td><%# Eval("Product.Category.Name") %></td>
                    <td><%# Eval("Product.BasePrice", "{0:C}") %></td>
                    <td><%# Eval("Price", "{0:C}") %></td>
                    <td class="text-end crm-row-actions">
                        <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-secondary" ToolTip="Editar"
                            CommandName="Editar" CommandArgument='<%# Eval("PriceTableItemId") %>'>
                            <i class="fas fa-pen"></i>
                        </asp:LinkButton>
                        <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-danger" ToolTip="Remover"
                            CommandName="Remover" CommandArgument='<%# Eval("PriceTableItemId") %>'
                            data-confirm='<%# "Remover o preço de " + Eval("Product.Name") + " desta tabela?" %>'>
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

        <asp:PlaceHolder ID="phVazio" runat="server" Visible="false">
            <div class="crm-empty-state">
                <i class="fas fa-euro-sign"></i>
                <p class="mb-0">Ainda não há preços definidos nesta tabela.</p>
            </div>
        </asp:PlaceHolder>
    </div>

</asp:Content>