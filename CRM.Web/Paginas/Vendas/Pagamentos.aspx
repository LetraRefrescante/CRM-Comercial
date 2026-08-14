<%@ Page Title="Pagamentos" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="Pagamentos.aspx.cs" Inherits="CRM.Web.Paginas.Vendas.Pagamentos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item"><a href="VendasLista.aspx">Vendas</a></li>
    <li class="breadcrumb-item"><a href="VendaDetalhe.aspx?id=<%= Request.QueryString["saleId"] %>">Detalhe</a></li>
    <li class="breadcrumb-item active">Pagamentos</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="crm-list-header">
        <h2>Pagamentos — Venda <asp:Label ID="lblNumero" runat="server" CssClass="text-muted fs-6" /></h2>
    </div>

    <div class="crm-filter-card">
        <div class="row g-3">
            <div class="col-md-3"><span class="text-muted small">Total da Venda</span><div class="fw-semibold"><asp:Label ID="lblTotalVenda" runat="server" /></div></div>
            <div class="col-md-3"><span class="text-muted small">Total Pago</span><div class="fw-semibold"><asp:Label ID="lblTotalPago" runat="server" /></div></div>
            <div class="col-md-3"><span class="text-muted small">Saldo em Falta</span><div class="fw-semibold"><asp:Label ID="lblSaldo" runat="server" /></div></div>
            <div class="col-md-3"><span class="text-muted small">Estado</span><div><span id="spanStatus" runat="server" class="badge bg-secondary"></span></div></div>
        </div>
    </div>

    <asp:PlaceHolder ID="phFormulario" runat="server">
        <div class="crm-card mt-3">
            <h5 class="crm-card-title">Registar Pagamento</h5>
            <div class="row g-3">
                <div class="col-md-3">
                    <label class="form-label">Valor *</label>
                    <asp:TextBox ID="txtValor" runat="server" CssClass="form-control" />
                </div>
                <div class="col-md-3">
                    <label class="form-label">Data *</label>
                    <asp:TextBox ID="txtData" runat="server" CssClass="form-control" TextMode="Date" />
                </div>
                <div class="col-md-3">
                    <label class="form-label">Método</label>
                    <asp:DropDownList ID="ddlMetodo" runat="server" CssClass="form-select">
                        <asp:ListItem Text="—" Value="" />
                        <asp:ListItem Text="Transferência" Value="Transferência" />
                        <asp:ListItem Text="Referência" Value="Referência" />
                        <asp:ListItem Text="Cartão" Value="Cartão" />
                        <asp:ListItem Text="Outro" Value="Outro" />
                    </asp:DropDownList>
                </div>
                <div class="col-md-3">
                    <label class="form-label">Referência</label>
                    <asp:TextBox ID="txtReferencia" runat="server" CssClass="form-control" MaxLength="100" />
                </div>
                <div class="col-12">
                    <label class="form-label">Notas</label>
                    <asp:TextBox ID="txtNotas" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" MaxLength="500" />
                </div>
            </div>
            <asp:Button ID="btnRegistar" runat="server" Text="Registar Pagamento" CssClass="btn btn-primary mt-3" OnClick="btnRegistar_Click" />
        </div>
    </asp:PlaceHolder>

    <div class="crm-table-card mt-3">
        <div class="p-3 pb-0"><h5 class="mb-0">Histórico de Pagamentos</h5></div>
        <asp:Repeater ID="rptPagamentos" runat="server" OnItemCommand="rptPagamentos_ItemCommand">
            <HeaderTemplate>
                <table class="table mb-0 align-middle">
                    <thead><tr><th>Data</th><th>Valor</th><th>Método</th><th>Referência</th><th>Notas</th><th class="text-end">Ações</th></tr></thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# Eval("PaymentDate", "{0:dd/MM/yyyy}") %></td>
                    <td><%# Eval("Amount", "{0:C}") %></td>
                    <td><%# Eval("PaymentMethod") %></td>
                    <td><%# Eval("Reference") %></td>
                    <td><%# Eval("Notes") %></td>
                    <td class="text-end">
                        <asp:PlaceHolder ID="phEliminar" runat="server">
                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-danger" ToolTip="Eliminar"
                                CommandName="Eliminar" CommandArgument='<%# Eval("PaymentId") %>'
                                OnClientClick="return confirm('Eliminar este pagamento? O estado financeiro da venda é recalculado automaticamente.');">
                                <i class="fas fa-trash"></i>
                            </asp:LinkButton>
                        </asp:PlaceHolder>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate></tbody></table></FooterTemplate>
        </asp:Repeater>
        <asp:PlaceHolder ID="phVazio" runat="server" Visible="false">
            <div class="crm-empty-state"><p class="mb-0 text-muted small p-3">Ainda sem pagamentos registados.</p></div>
        </asp:PlaceHolder>
    </div>

    <div class="mt-3">
        <a href="VendaDetalhe.aspx?id=<%= Request.QueryString["saleId"] %>" class="btn btn-outline-secondary">Voltar à Venda</a>
    </div>

</asp:Content>