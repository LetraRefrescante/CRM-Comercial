<%@ Page Title="Produto" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="ProdutoEditar.aspx.cs" Inherits="CRM.Web.Paginas.Catalogo.ProdutoEditar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item"><a href="~/Catalogo/ProdutosLista.aspx" runat="server">Catálogo</a></li>
    <li class="breadcrumb-item active"><%: TituloPagina %></li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2><%: TituloPagina %></h2>

    <div class="crm-form-card">
        <asp:ValidationSummary ID="valSummary" runat="server" CssClass="alert alert-danger" DisplayMode="BulletList" />

        <div class="row g-3">
            <div class="col-md-3">
                <label class="form-label">Código *</label>
                <asp:TextBox ID="txtCodigo" runat="server" CssClass="form-control" MaxLength="30" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCodigo" CssClass="text-danger small"
                    ErrorMessage="O código é obrigatório." Display="Dynamic" />
            </div>
            <div class="col-md-3">
                <label class="form-label">Tipo *</label>
                <asp:DropDownList ID="ddlTipo" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Produto" Value="Produto" />
                    <asp:ListItem Text="Serviço" Value="Serviço" />
                </asp:DropDownList>
            </div>
            <div class="col-md-6">
                <label class="form-label">Nome *</label>
                <asp:TextBox ID="txtNome" runat="server" CssClass="form-control" MaxLength="180" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtNome" CssClass="text-danger small"
                    ErrorMessage="O nome é obrigatório." Display="Dynamic" />
            </div>

            <div class="col-md-4">
                <label class="form-label">Categoria *</label>
                <asp:DropDownList ID="ddlCategoria" runat="server" CssClass="form-select" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlCategoria" InitialValue=""
                    CssClass="text-danger small" ErrorMessage="A categoria é obrigatória." Display="Dynamic" />
            </div>
            <div class="col-md-4">
                <label class="form-label">Taxa IVA *</label>
                <asp:DropDownList ID="ddlTaxaIva" runat="server" CssClass="form-select" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlTaxaIva" InitialValue=""
                    CssClass="text-danger small" ErrorMessage="A taxa de IVA é obrigatória." Display="Dynamic" />
            </div>
            <div class="col-md-4">
                <label class="form-label">Unidade *</label>
                <asp:DropDownList ID="ddlUnidade" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Unidade" Value="Unidade" />
                    <asp:ListItem Text="Hora" Value="Hora" />
                    <asp:ListItem Text="Dia" Value="Dia" />
                    <asp:ListItem Text="Mês" Value="Mês" />
                    <asp:ListItem Text="Pacote" Value="Pacote" />
                </asp:DropDownList>
            </div>

            <div class="col-md-3">
                <label class="form-label">Preço Base (€) *</label>
                <asp:TextBox ID="txtPrecoBase" runat="server" CssClass="form-control" TextMode="Number" step="0.01" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPrecoBase" CssClass="text-danger small"
                    ErrorMessage="O preço base é obrigatório." Display="Dynamic" />
                <asp:RangeValidator runat="server" ControlToValidate="txtPrecoBase" CssClass="text-danger small"
                    ErrorMessage="O preço base não pode ser negativo." Display="Dynamic"
                    Type="Currency" MinimumValue="0" MaximumValue="999999999" />
            </div>
            <div class="col-md-3 d-flex align-items-end">
                <div class="form-check">
                    <asp:CheckBox ID="chkAtivo" runat="server" CssClass="form-check-input" Checked="true" />
                    <label class="form-check-label">Ativo</label>
                </div>
            </div>

            <div class="col-md-12">
                <label class="form-label">Descrição</label>
                <asp:TextBox ID="txtDescricao" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" MaxLength="4000" />
            </div>
        </div>

        <asp:CustomValidator ID="cvRegrasNegocio" runat="server" Display="Dynamic" CssClass="text-danger small d-block mt-2"
            OnServerValidate="cvRegrasNegocio_ServerValidate" />

        <div class="d-flex gap-2 mt-4">
            <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-primary" OnClick="btnGuardar_Click" />
            <asp:HyperLink ID="lnkCancelar" runat="server" NavigateUrl="~/Catalogo/ProdutosLista.aspx" CssClass="btn btn-outline-secondary">Cancelar</asp:HyperLink>
        </div>
    </div>

</asp:Content>