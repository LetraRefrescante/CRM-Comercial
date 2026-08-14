<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PropostaPDF.aspx.cs" Inherits="CRM.Web.Paginas.Catalogo.PropostaPDF" %>

<!DOCTYPE html>
<html lang="pt">
<head runat="server">
    <meta charset="utf-8" />
    <title>Proposta</title>
    <style>
        body { font-family: Arial, Helvetica, sans-serif; color: #222; margin: 40px; font-size: 13px; }
        .cabecalho { display: flex; justify-content: space-between; align-items: flex-start; border-bottom: 2px solid #333; padding-bottom: 16px; margin-bottom: 24px; }
        .empresa { font-size: 18px; font-weight: bold; }
        .proposta-numero { text-align: right; font-size: 16px; font-weight: bold; }
        .bloco-info { display: flex; justify-content: space-between; margin-bottom: 24px; }
        .bloco-info > div { width: 48%; }
        .rotulo { color: #666; font-size: 11px; text-transform: uppercase; }
        table { width: 100%; border-collapse: collapse; margin-bottom: 24px; }
        th, td { border: 1px solid #ccc; padding: 6px 8px; text-align: left; font-size: 12px; }
        th { background: #f2f2f2; }
        .totais { width: 280px; margin-left: auto; }
        .totais div { display: flex; justify-content: space-between; padding: 4px 0; }
        .totais .total-geral { font-weight: bold; font-size: 15px; border-top: 2px solid #333; padding-top: 8px; }
        .notas { margin-top: 24px; font-size: 12px; white-space: pre-wrap; }
        .rodape { margin-top: 40px; font-size: 11px; color: #888; text-align: center; }
        @media print {
            .acoes-nao-imprimir { display: none; }
            body { margin: 15mm; }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="acoes-nao-imprimir mb-3">
            <button type="button" onclick="window.print();">Imprimir / Guardar como PDF</button>
        </div>

        <div class="cabecalho">
            <div class="empresa"><asp:Literal ID="litEmpresa" runat="server" /></div>
            <div class="proposta-numero">
                Proposta <asp:Literal ID="litNumero" runat="server" /><br />
                <span style="font-size:12px; font-weight:normal;">Versão <asp:Literal ID="litVersao" runat="server" /></span>
            </div>
        </div>

        <div class="bloco-info">
            <div>
                <div class="rotulo">Cliente</div>
                <div><asp:Literal ID="litCliente" runat="server" /></div>
                <div><asp:Literal ID="litMoradaCliente" runat="server" /></div>
                <div><asp:Literal ID="litNifCliente" runat="server" /></div>
            </div>
            <div style="text-align:right;">
                <div class="rotulo">Data de Emissão</div>
                <div><asp:Literal ID="litEmissao" runat="server" /></div>
                <div class="rotulo" style="margin-top:8px;">Válida até</div>
                <div><asp:Literal ID="litValidade" runat="server" /></div>
            </div>
        </div>

        <asp:Repeater ID="rptLinhas" runat="server">
            <HeaderTemplate>
                <table>
                    <thead>
                        <tr>
                            <th>Descrição</th>
                            <th>Qtd.</th>
                            <th>Preço Unit.</th>
                            <th>Desc. %</th>
                            <th>IVA</th>
                            <th>Total</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# Eval("Description") %></td>
                    <td><%# Eval("Quantity") %></td>
                    <td><%# Eval("UnitPrice", "{0:C}") %></td>
                    <td><%# Eval("DiscountPercent") %>%</td>
                    <td><%# Eval("TaxRate.Percentage") %>%</td>
                    <td><%# Eval("LineTotal", "{0:C}") %></td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                    </tbody>
                </table>
            </FooterTemplate>
        </asp:Repeater>

        <div class="totais">
            <div><span>Subtotal</span><span><asp:Literal ID="litSubTotal" runat="server" /></span></div>
            <div><span>IVA</span><span><asp:Literal ID="litIvaTotal" runat="server" /></span></div>
            <div class="total-geral"><span>Total</span><span><asp:Literal ID="litTotalGeral" runat="server" /></span></div>
        </div>

        <asp:PlaceHolder ID="phCondicoes" runat="server" Visible="false">
            <div class="notas">
                <div class="rotulo">Condições de Pagamento</div>
                <div><asp:Literal ID="litCondicoesPagamento" runat="server" /></div>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="phNotas" runat="server" Visible="false">
            <div class="notas">
                <div class="rotulo">Notas</div>
                <div><asp:Literal ID="litNotas" runat="server" /></div>
            </div>
        </asp:PlaceHolder>

        <div class="rodape">
            Documento gerado por <asp:Literal ID="litEmpresaRodape" runat="server" /> em <asp:Literal ID="litDataGeracao" runat="server" />
        </div>
    </form>
</body>
</html>