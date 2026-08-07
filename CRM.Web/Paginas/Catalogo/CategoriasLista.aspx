<%@ Page Title="Categorias" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="CategoriasLista.aspx.cs" Inherits="CRM.Web.Paginas.Catalogo.CategoriasLista" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item"><a href="~/Catalogo/ProdutosLista.aspx" runat="server">Catálogo</a></li>
    <li class="breadcrumb-item active">Categorias</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="crm-list-header">
        <h2>Categorias</h2>
    </div>

    <asp:PlaceHolder ID="phFormulario" runat="server">
        <div class="crm-form-card">
            <asp:ValidationSummary ID="valSummary" runat="server" CssClass="alert alert-danger" DisplayMode="BulletList" />
            <asp:CustomValidator ID="cvRegrasNegocio" runat="server" Display="None" OnServerValidate="cvRegrasNegocio_ServerValidate" />
            <asp:Literal ID="litModoEdicao" runat="server" CssClass="text-muted small d-block mb-2" Visible="false" />

            <div class="row g-2 align-items-end">
                <div class="col-md-4">
                    <label class="form-label">Nome</label>
                    <asp:TextBox ID="txtNome" runat="server" CssClass="form-control" MaxLength="100" placeholder="Nome da categoria" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtNome" CssClass="text-danger small"
                        ErrorMessage="O nome é obrigatório." Display="Dynamic" />
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
        <asp:Repeater ID="rptCategorias" runat="server" OnItemCommand="rptCategorias_ItemCommand">
            <HeaderTemplate>
                <table class="table table-hover mb-0 align-middle">
                    <thead>
                        <tr>
                            <th>Nome</th>
                            <th>Estado</th>
                            <th class="text-end">Ações</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# Eval("Name") %></td>
                    <td>
                        <span class="badge <%# (bool)Eval("IsActive") ? "bg-success" : "bg-secondary" %>">
                            <%# (bool)Eval("IsActive") ? "Ativa" : "Inativa" %>
                        </span>
                    </td>
                    <td class="text-end crm-row-actions">
                        <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-secondary" ToolTip="Editar"
                            CommandName="Editar" CommandArgument='<%# Eval("CategoryId") %>'>
                            <i class="fas fa-pen"></i>
                        </asp:LinkButton>
                        <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-warning" ToolTip="Ativar/Desativar"
                            CommandName="AlternarEstado" CommandArgument='<%# Eval("CategoryId") %>'
                            data-confirm='<%# ((bool)Eval("IsActive") ? "Desativar" : "Ativar") + " a categoria " + Eval("Name") + "?" %>'>
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

        <asp:PlaceHolder ID="phVazio" runat="server" Visible="false">
            <div class="crm-empty-state">
                <i class="fas fa-tags"></i>
                <p class="mb-0">Ainda não existem categorias.</p>
            </div>
        </asp:PlaceHolder>
    </div>

</asp:Content>