<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SeletorCliente.ascx.cs" Inherits="CRM.Web.Controls.SeletorCliente" %>

<div id="divSeletor" runat="server" class="input-group" style="max-width: 340px;">
    <asp:TextBox ID="txtClienteNome" runat="server" CssClass="form-control" ReadOnly="true" placeholder="Nenhum cliente selecionado" />
    <button type="button" id="btnAbrirSeletor" runat="server" data-bs-toggle="modal"></button>
</div>
<asp:HiddenField ID="hdnClienteId" runat="server" />
<asp:CustomValidator ID="cvClienteObrigatorio" runat="server" OnServerValidate="cvClienteObrigatorio_ServerValidate"
    ErrorMessage="Tens de selecionar um cliente." CssClass="text-danger small" Display="Dynamic" Enabled="false" />

<div class="modal fade" id="mdlSeletor" runat="server">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Selecionar Cliente</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body">
                <asp:UpdatePanel ID="upSeletor" runat="server">
                    <ContentTemplate>
                        <div class="input-group mb-3">
                            <asp:TextBox ID="txtPesquisa" runat="server" CssClass="form-control" placeholder="Nome, NIF, cidade ou código..." />
                            <asp:Button ID="btnPesquisar" runat="server" Text="Pesquisar" CssClass="btn btn-primary" OnClick="btnPesquisar_Click" />
                        </div>

                        <asp:Repeater ID="rptResultados" runat="server" OnItemCommand="rptResultados_ItemCommand">
                            <HeaderTemplate>
                                <table class="table table-hover mb-0">
                                    <thead><tr><th>Nome</th><th>NIF</th><th>Cidade</th><th>Comercial</th><th></th></tr></thead>
                                    <tbody>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td><%# Eval("TradeName") %></td>
                                    <td><%# Eval("VatNumber") %></td>
                                    <td><%# Eval("City") ?? "—" %></td>
                                    <td><%# Eval("AccountManager.Name") ?? "—" %></td>
                                    <td>
                                        <asp:LinkButton runat="server" CssClass="btn btn-sm btn-primary" CommandName="Escolher"
                                            CommandArgument='<%# Eval("ClientId") + "|" + Eval("TradeName") %>'>
                                            Escolher
                                        </asp:LinkButton>
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate></tbody></table></FooterTemplate>
                        </asp:Repeater>

                        <asp:PlaceHolder ID="phSemResultados" runat="server" Visible="false">
                            <p class="text-muted text-center py-3">Nenhum cliente encontrado.</p>
                        </asp:PlaceHolder>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>
</div>