<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SeletorProduto.ascx.cs" Inherits="CRM.Web.Controls.SeletorProduto" %>

<div id="divSeletor" runat="server" class="input-group" style="max-width: 340px;">
    <asp:TextBox ID="txtProdutoNome" runat="server" CssClass="form-control" ReadOnly="true" placeholder="Nenhum produto selecionado" />
    <button type="button" id="btnAbrirSeletor" runat="server" data-bs-toggle="modal"></button>
</div>
<asp:HiddenField ID="hdnProdutoId" runat="server" />
<asp:CustomValidator ID="cvProdutoObrigatorio" runat="server" OnServerValidate="cvProdutoObrigatorio_ServerValidate"
    ErrorMessage="Tens de selecionar um produto." CssClass="text-danger small" Display="Dynamic" Enabled="false" />

<div class="modal fade" id="mdlSeletor" runat="server">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Selecionar Produto</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body">
                <asp:UpdatePanel ID="upSeletor" runat="server">
                    <ContentTemplate>
                        <div class="row g-2 mb-3">
                            <div class="col-md-6">
                                <asp:TextBox ID="txtPesquisa" runat="server" CssClass="form-control" placeholder="Código ou nome..." />
                            </div>
                            <div class="col-md-3">
                                <asp:DropDownList ID="ddlTipo" runat="server" CssClass="form-select">
                                    <asp:ListItem Text="Todos os tipos" Value="" />
                                    <asp:ListItem Text="Produto" Value="Produto" />
                                    <asp:ListItem Text="Serviço" Value="Serviço" />
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-3">
                                <asp:Button ID="btnPesquisar" runat="server" Text="Pesquisar" CssClass="btn btn-primary w-100" OnClick="btnPesquisar_Click" />
                            </div>
                        </div>

                        <asp:Repeater ID="rptResultados" runat="server" OnItemCommand="rptResultados_ItemCommand">
                            <HeaderTemplate>
                                <table class="table table-hover mb-0">
                                    <thead><tr><th>Código</th><th>Nome</th><th>Categoria</th><th>Preço Base</th><th>IVA</th><th></th></tr></thead>
                                    <tbody>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td class="mono"><%# Eval("Code") %></td>
                                    <td><%# Eval("Name") %></td>
                                    <td><%# Eval("Category.Name") %></td>
                                    <td><%# Eval("BasePrice", "{0:C}") %></td>
                                    <td><%# Eval("TaxRate.Percentage", "{0}%") %></td>
                                    <td>
                                        <asp:LinkButton runat="server" CssClass="btn btn-sm btn-primary" CommandName="Escolher"
                                            CommandArgument='<%# Eval("ProductId") %>'>
                                            Escolher
                                        </asp:LinkButton>
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate></tbody></table></FooterTemplate>
                        </asp:Repeater>

                        <asp:PlaceHolder ID="phSemResultados" runat="server" Visible="false">
                            <p class="text-muted text-center py-3">Nenhum produto encontrado.</p>
                        </asp:PlaceHolder>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>
</div>
