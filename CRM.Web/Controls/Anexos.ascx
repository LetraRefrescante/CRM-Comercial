<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Anexos.ascx.cs" Inherits="CRM.Web.Controls.Anexos" %>
<div class="crm-anexos">
    <div class="row g-2 align-items-end mb-3">
        <div class="col-md-5">
            <asp:FileUpload ID="fuAnexo" runat="server" CssClass="form-control" />
        </div>
        <div class="col-md-4">
            <asp:DropDownList ID="ddlCategoria" runat="server" CssClass="form-select">
                <asp:ListItem Text="Contrato" Value="Contrato" />
                <asp:ListItem Text="Proposta" Value="Proposta" />
                <asp:ListItem Text="Identificação" Value="Identificação" />
                <asp:ListItem Text="Outro" Value="Outro" Selected="True" />
            </asp:DropDownList>
        </div>
        <div class="col-md-3">
            <asp:Button ID="btnUpload" runat="server" Text="Carregar" CssClass="btn btn-outline-primary w-100" OnClick="btnUpload_Click" CausesValidation="false" />
        </div>
    </div>
    <div class="form-check mb-3">
        <asp:CheckBox ID="chkConfidencial" runat="server" CssClass="form-check-input" />
        <label class="form-check-label">Documento confidencial</label>
    </div>
    <asp:Label ID="lblErro" runat="server" CssClass="text-danger small d-block mb-2" Visible="false" />

    <asp:Repeater ID="rptAnexos" runat="server" OnItemCommand="rptAnexos_ItemCommand">
        <HeaderTemplate><ul class="list-group"></HeaderTemplate>
        <ItemTemplate>
            <li class="list-group-item d-flex justify-content-between align-items-center">
                <div>
                    <i class="fas fa-paperclip me-2"></i><%# Eval("OriginalFileName") %>
                    <span class="badge bg-secondary ms-2"><%# Eval("Category") %></span>
                    <asp:PlaceHolder runat="server" Visible='<%# (bool)Eval("IsConfidential") %>'>
                        <span class="badge bg-warning text-dark ms-1">Confidencial</span>
                    </asp:PlaceHolder>
                    <div class="text-muted small"><%# Eval("CreatedDate", "{0:dd/MM/yyyy HH:mm}") %></div>
                </div>
                <div class="d-flex gap-2">
                    <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-secondary" ToolTip="Descarregar"
                        CommandName="Descarregar" CommandArgument='<%# Eval("DocumentId") %>' CausesValidation="false">
                        <i class="fas fa-download"></i>
                    </asp:LinkButton>
                    <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-danger" ToolTip="Eliminar"
                        CommandName="Eliminar" CommandArgument='<%# Eval("DocumentId") %>' CausesValidation="false"
                        data-confirm='<%# "Eliminar o ficheiro " + Eval("OriginalFileName") + "?" %>'>
                        <i class="fas fa-trash"></i>
                    </asp:LinkButton>
                </div>
            </li>
        </ItemTemplate>
        <FooterTemplate></ul></FooterTemplate>
    </asp:Repeater>
    <asp:PlaceHolder ID="phVazio" runat="server" Visible="false">
        <p class="text-muted text-center mb-0">Ainda não existem anexos.</p>
    </asp:PlaceHolder>
</div>