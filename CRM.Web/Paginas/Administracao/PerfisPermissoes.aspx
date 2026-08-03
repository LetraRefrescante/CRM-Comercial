<%@ Page Title="Perfis e Permissões" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="PerfisPermissoes.aspx.cs" Inherits="CRM.Web.Paginas.Administracao.PerfisPermissoes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item">Administração</li>
    <li class="breadcrumb-item active">Perfis e Permissões</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2 class="mb-4" style="font-family:'Sora',sans-serif;">Perfis e Permissões</h2>

    <div class="card p-3 mb-3" style="max-width: 400px;">
        <label class="form-label small text-muted">Perfil</label>
        <asp:DropDownList ID="ddlPerfil" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlPerfil_SelectedIndexChanged" />
    </div>

    <asp:Repeater ID="rptModulos" runat="server">
        <ItemTemplate>
            <div class="card p-3 mb-3">
                <h6 class="mb-3" style="font-family:'Sora',sans-serif;"><%# Eval("Modulo") %></h6>
                <div class="row">
                    <asp:Repeater ID="rptPermissoes" runat="server" DataSource='<%# Eval("Permissoes") %>'>
                        <ItemTemplate>
                            <div class="col-md-4 mb-2">
                                    <div class="form-check">
                                        <input type="checkbox" class="form-check-input"
                                               name="permissao" value="<%# Eval("PermissionId") %>"
                                               id="chk_<%# Eval("PermissionId") %>"
                                               <%# (bool)Eval("Selecionado") ? "checked=\"checked\"" : "" %>
                                               <%# !PodeGerirPublico ? "disabled=\"disabled\"" : "" %> />
                                        <label class="form-check-label" for="chk_<%# Eval("PermissionId") %>">
                                            <%# Eval("Description") %>
                                        </label>
                                    </div>
                                </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>

    <asp:PlaceHolder ID="phVazio" runat="server" Visible="false">
        <div class="alert alert-info">
            Ainda não existem permissões configuradas no sistema. É preciso criar registos na tabela <span class="mono">Permissions</span> antes de as poderes atribuir aqui.
        </div>
    </asp:PlaceHolder>

    <asp:Button ID="btnGuardar" runat="server" Text="Guardar Permissões" CssClass="btn btn-success" OnClick="btnGuardar_Click" Visible="false" />

</asp:Content>