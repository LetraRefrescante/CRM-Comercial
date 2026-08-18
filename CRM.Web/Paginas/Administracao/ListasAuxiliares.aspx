<%@ Page Title="Listas Auxiliares" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="ListasAuxiliares.aspx.cs" Inherits="CRM.Web.Paginas.Administracao.ListasAuxiliares" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item">Administração</li>
    <li class="breadcrumb-item active">Listas Auxiliares</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="crm-list-header"><h2>Listas Auxiliares</h2></div>
    <p class="text-muted small">Categorias de produto têm página própria em Catálogo. "Estados" (Cliente/Lead/Proposta/Venda) são fixos no schema, não geríveis aqui.</p>

    <!-- ===================== Origens de Lead ===================== -->
    <h5 class="mt-4">Origens de Lead</h5>
    <div class="crm-form-card mb-2">
        <asp:CustomValidator ID="cvLeadSource" runat="server" Display="None" ValidationGroup="LeadSource" OnServerValidate="cvLeadSource_ServerValidate" />
        <asp:ValidationSummary ID="vsLeadSource" runat="server" CssClass="alert alert-danger" DisplayMode="BulletList" ValidationGroup="LeadSource" />
        <div class="row g-2 align-items-end">
            <div class="col-md-4">
                <asp:TextBox ID="txtLeadSourceNome" runat="server" CssClass="form-control" placeholder="Nome da origem" />
            </div>
            <div class="col-md-auto">
                <asp:Button ID="btnLeadSourceGuardar" runat="server" Text="Adicionar" CssClass="btn btn-primary" ValidationGroup="LeadSource" OnClick="btnLeadSourceGuardar_Click" />
            </div>
            <div class="col-md-auto">
                <asp:Button ID="btnLeadSourceCancelar" runat="server" Text="Cancelar" CssClass="btn btn-outline-secondary" Visible="false"
                    CausesValidation="false" OnClick="btnLeadSourceCancelar_Click" />
            </div>
        </div>
    </div>
    <div class="crm-table-card mb-4">
        <asp:Repeater ID="rptLeadSources" runat="server" OnItemCommand="rptLeadSources_ItemCommand">
            <HeaderTemplate><table class="table table-hover mb-0"><thead><tr><th>Nome</th><th>Estado</th><th class="text-end">Ações</th></tr></thead><tbody></HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# Eval("Name") %></td>
                    <td><span class="badge <%# (bool)Eval("IsActive") ? "bg-success" : "bg-secondary" %>"><%# (bool)Eval("IsActive") ? "Ativa" : "Inativa" %></span></td>
                    <td class="text-end">
                        <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-secondary" CommandName="Editar" CommandArgument='<%# Eval("LeadSourceId") %>'><i class="fas fa-pen"></i></asp:LinkButton>
                        <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-warning" CommandName="AlternarEstado" CommandArgument='<%# Eval("LeadSourceId") %>'><i class="fas fa-power-off"></i></asp:LinkButton>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate></tbody></table></FooterTemplate>
        </asp:Repeater>
        <asp:PlaceHolder ID="phLeadSourcesVazio" runat="server" Visible="false"><p class="text-muted text-center p-3 mb-0">Sem origens registadas.</p></asp:PlaceHolder>
    </div>

    <!-- ===================== Motivos de Perda ===================== -->
    <h5>Motivos de Perda</h5>
    <div class="crm-form-card mb-2">
        <asp:CustomValidator ID="cvLossReason" runat="server" Display="None" ValidationGroup="LossReason" OnServerValidate="cvLossReason_ServerValidate" />
        <asp:ValidationSummary ID="vsLossReason" runat="server" CssClass="alert alert-danger" DisplayMode="BulletList" ValidationGroup="LossReason" />
        <div class="row g-2 align-items-end">
            <div class="col-md-4">
                <asp:TextBox ID="txtLossReasonNome" runat="server" CssClass="form-control" placeholder="Nome do motivo" />
            </div>
            <div class="col-md-auto">
                <asp:Button ID="btnLossReasonGuardar" runat="server" Text="Adicionar" CssClass="btn btn-primary" ValidationGroup="LossReason" OnClick="btnLossReasonGuardar_Click" />
            </div>
            <div class="col-md-auto">
                <asp:Button ID="btnLossReasonCancelar" runat="server" Text="Cancelar" CssClass="btn btn-outline-secondary" Visible="false"
                    CausesValidation="false" OnClick="btnLossReasonCancelar_Click" />
            </div>
        </div>
    </div>
    <div class="crm-table-card mb-4">
        <asp:Repeater ID="rptLossReasons" runat="server" OnItemCommand="rptLossReasons_ItemCommand">
            <HeaderTemplate><table class="table table-hover mb-0"><thead><tr><th>Nome</th><th>Estado</th><th class="text-end">Ações</th></tr></thead><tbody></HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# Eval("Name") %></td>
                    <td><span class="badge <%# (bool)Eval("IsActive") ? "bg-success" : "bg-secondary" %>"><%# (bool)Eval("IsActive") ? "Ativo" : "Inativo" %></span></td>
                    <td class="text-end">
                        <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-secondary" CommandName="Editar" CommandArgument='<%# Eval("LossReasonId") %>'><i class="fas fa-pen"></i></asp:LinkButton>
                        <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-warning" CommandName="AlternarEstado" CommandArgument='<%# Eval("LossReasonId") %>'><i class="fas fa-power-off"></i></asp:LinkButton>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate></tbody></table></FooterTemplate>
        </asp:Repeater>
        <asp:PlaceHolder ID="phLossReasonsVazio" runat="server" Visible="false"><p class="text-muted text-center p-3 mb-0">Sem motivos registados.</p></asp:PlaceHolder>
    </div>

    <!-- ===================== Condições de Pagamento ===================== -->
    <h5>Condições de Pagamento</h5>
    <div class="crm-form-card mb-2">
        <asp:CustomValidator ID="cvPaymentTerm" runat="server" Display="None" ValidationGroup="PaymentTerm" OnServerValidate="cvPaymentTerm_ServerValidate" />
        <asp:ValidationSummary ID="vsPaymentTerm" runat="server" CssClass="alert alert-danger" DisplayMode="BulletList" ValidationGroup="PaymentTerm" />
        <div class="row g-2 align-items-end">
            <div class="col-md-4">
                <asp:TextBox ID="txtPaymentTermNome" runat="server" CssClass="form-control" placeholder="ex: 30 dias" />
            </div>
            <div class="col-md-2">
                <asp:TextBox ID="txtPaymentTermDias" runat="server" CssClass="form-control" TextMode="Number" placeholder="Dias" />
            </div>
            <div class="col-md-auto">
                <asp:Button ID="btnPaymentTermGuardar" runat="server" Text="Adicionar" CssClass="btn btn-primary" ValidationGroup="PaymentTerm" OnClick="btnPaymentTermGuardar_Click" />
            </div>
            <div class="col-md-auto">
                <asp:Button ID="btnPaymentTermCancelar" runat="server" Text="Cancelar" CssClass="btn btn-outline-secondary" Visible="false"
                    CausesValidation="false" OnClick="btnPaymentTermCancelar_Click" />
            </div>
        </div>
    </div>
    <div class="crm-table-card mb-4">
        <asp:Repeater ID="rptPaymentTerms" runat="server" OnItemCommand="rptPaymentTerms_ItemCommand">
            <HeaderTemplate><table class="table table-hover mb-0"><thead><tr><th>Nome</th><th>Dias</th><th>Estado</th><th class="text-end">Ações</th></tr></thead><tbody></HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# Eval("Name") %></td>
                    <td><%# Eval("DaysDue") %></td>
                    <td><span class="badge <%# (bool)Eval("IsActive") ? "bg-success" : "bg-secondary" %>"><%# (bool)Eval("IsActive") ? "Ativa" : "Inativa" %></span></td>
                    <td class="text-end">
                        <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-secondary" CommandName="Editar" CommandArgument='<%# Eval("PaymentTermId") %>'><i class="fas fa-pen"></i></asp:LinkButton>
                        <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-warning" CommandName="AlternarEstado" CommandArgument='<%# Eval("PaymentTermId") %>'><i class="fas fa-power-off"></i></asp:LinkButton>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate></tbody></table></FooterTemplate>
        </asp:Repeater>
        <asp:PlaceHolder ID="phPaymentTermsVazio" runat="server" Visible="false"><p class="text-muted text-center p-3 mb-0">Sem condições registadas.</p></asp:PlaceHolder>
    </div>

    <!-- ===================== Taxas de IVA ===================== -->
    <h5>Taxas de IVA</h5>
    <div class="crm-form-card mb-2">
        <asp:CustomValidator ID="cvTaxRate" runat="server" Display="None" ValidationGroup="TaxRate" OnServerValidate="cvTaxRate_ServerValidate" />
        <asp:ValidationSummary ID="vsTaxRate" runat="server" CssClass="alert alert-danger" DisplayMode="BulletList" ValidationGroup="TaxRate" />
        <div class="row g-2 align-items-end">
            <div class="col-md-4">
                <asp:TextBox ID="txtTaxRateNome" runat="server" CssClass="form-control" placeholder="ex: Taxa Normal" />
            </div>
            <div class="col-md-2">
                <asp:TextBox ID="txtTaxRatePercentagem" runat="server" CssClass="form-control" TextMode="Number" step="0.01" placeholder="%" />
            </div>
            <div class="col-md-auto">
                <asp:Button ID="btnTaxRateGuardar" runat="server" Text="Adicionar" CssClass="btn btn-primary" ValidationGroup="TaxRate" OnClick="btnTaxRateGuardar_Click" />
            </div>
            <div class="col-md-auto">
                <asp:Button ID="btnTaxRateCancelar" runat="server" Text="Cancelar" CssClass="btn btn-outline-secondary" Visible="false"
                    CausesValidation="false" OnClick="btnTaxRateCancelar_Click" />
            </div>
        </div>
    </div>
    <div class="crm-table-card">
        <asp:Repeater ID="rptTaxRates" runat="server" OnItemCommand="rptTaxRates_ItemCommand">
            <HeaderTemplate><table class="table table-hover mb-0"><thead><tr><th>Nome</th><th>%</th><th>Estado</th><th class="text-end">Ações</th></tr></thead><tbody></HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# Eval("Name") %></td>
                    <td><%# Eval("Percentage") %>%</td>
                    <td><span class="badge <%# (bool)Eval("IsActive") ? "bg-success" : "bg-secondary" %>"><%# (bool)Eval("IsActive") ? "Ativa" : "Inativa" %></span></td>
                    <td class="text-end">
                        <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-secondary" CommandName="Editar" CommandArgument='<%# Eval("TaxRateId") %>'><i class="fas fa-pen"></i></asp:LinkButton>
                        <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-warning" CommandName="AlternarEstado" CommandArgument='<%# Eval("TaxRateId") %>'><i class="fas fa-power-off"></i></asp:LinkButton>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate></tbody></table></FooterTemplate>
        </asp:Repeater>
        <asp:PlaceHolder ID="phTaxRatesVazio" runat="server" Visible="false"><p class="text-muted text-center p-3 mb-0">Sem taxas registadas.</p></asp:PlaceHolder>
    </div>

</asp:Content>