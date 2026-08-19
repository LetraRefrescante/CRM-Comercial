<%@ Page Title="Listas Auxiliares" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="ListasAuxiliares.aspx.cs" Inherits="CRM.Web.Paginas.Administracao.ListasAuxiliares" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item">Administração</li>
    <li class="breadcrumb-item active">Listas Auxiliares</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2 class="mb-3">Listas Auxiliares</h2>
    <p class="text-muted small mb-4">Nenhum item é eliminado fisicamente — só ativado/inativado. Itens inativos deixam de aparecer em novos registos, mas continuam nos registos já criados.</p>

    <div class="row g-3">

        <div class="col-md-6">
            <div class="crm-table-card">
                <div class="p-3 pb-0"><h5 class="mb-0">Setores</h5></div>
                <asp:Repeater ID="rptSetores" runat="server" OnItemCommand="rptSetores_ItemCommand">
                    <HeaderTemplate><ul class="list-group list-group-flush"></HeaderTemplate>
                    <ItemTemplate>
                        <li class="list-group-item d-flex justify-content-between align-items-center">
                            <span class="<%# GetTextoClasse(Eval("IsActive")) %>"><%# Eval("Name") %></span>
                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-secondary" CommandName="Alternar" CommandArgument='<%# Eval("SectorId") %>'>
                                <%# GetTextoBotaoEstado(Eval("IsActive")) %>
                            </asp:LinkButton>
                        </li>
                    </ItemTemplate>
                    <FooterTemplate></ul></FooterTemplate>
                </asp:Repeater>
                <asp:PlaceHolder ID="phFormSetor" runat="server">
                    <div class="p-3 border-top d-flex gap-2">
                        <asp:TextBox ID="txtNovoSetor" runat="server" CssClass="form-control form-control-sm" placeholder="Novo setor..." />
                        <asp:Button ID="btnAddSetor" runat="server" Text="Adicionar" CssClass="btn btn-sm btn-primary" OnClick="btnAddSetor_Click" CausesValidation="false" />
                    </div>
                </asp:PlaceHolder>
            </div>
        </div>

        <div class="col-md-6">
            <div class="crm-table-card">
                <div class="p-3 pb-0"><h5 class="mb-0">Origens de Lead</h5></div>
                <asp:Repeater ID="rptOrigens" runat="server" OnItemCommand="rptOrigens_ItemCommand">
                    <HeaderTemplate><ul class="list-group list-group-flush"></HeaderTemplate>
                    <ItemTemplate>
                        <li class="list-group-item d-flex justify-content-between align-items-center">
                            <span class="<%# GetTextoClasse(Eval("IsActive")) %>"><%# Eval("Name") %></span>
                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-secondary" CommandName="Alternar" CommandArgument='<%# Eval("LeadSourceId") %>'>
                                <%# GetTextoBotaoEstado(Eval("IsActive")) %>
                            </asp:LinkButton>
                        </li>
                    </ItemTemplate>
                    <FooterTemplate></ul></FooterTemplate>
                </asp:Repeater>
                <asp:PlaceHolder ID="phFormOrigem" runat="server">
                    <div class="p-3 border-top d-flex gap-2">
                        <asp:TextBox ID="txtNovaOrigem" runat="server" CssClass="form-control form-control-sm" placeholder="Nova origem..." />
                        <asp:Button ID="btnAddOrigem" runat="server" Text="Adicionar" CssClass="btn btn-sm btn-primary" OnClick="btnAddOrigem_Click" CausesValidation="false" />
                    </div>
                </asp:PlaceHolder>
            </div>
        </div>

        <div class="col-md-6">
            <div class="crm-table-card">
                <div class="p-3 pb-0"><h5 class="mb-0">Motivos de Perda</h5></div>
                <asp:Repeater ID="rptMotivos" runat="server" OnItemCommand="rptMotivos_ItemCommand">
                    <HeaderTemplate><ul class="list-group list-group-flush"></HeaderTemplate>
                    <ItemTemplate>
                        <li class="list-group-item d-flex justify-content-between align-items-center">
                            <span class="<%# GetTextoClasse(Eval("IsActive")) %>"><%# Eval("Name") %></span>
                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-secondary" CommandName="Alternar" CommandArgument='<%# Eval("LossReasonId") %>'>
                                <%# GetTextoBotaoEstado(Eval("IsActive")) %>
                            </asp:LinkButton>
                        </li>
                    </ItemTemplate>
                    <FooterTemplate></ul></FooterTemplate>
                </asp:Repeater>
                <asp:PlaceHolder ID="phFormMotivo" runat="server">
                    <div class="p-3 border-top d-flex gap-2">
                        <asp:TextBox ID="txtNovoMotivo" runat="server" CssClass="form-control form-control-sm" placeholder="Novo motivo..." />
                        <asp:Button ID="btnAddMotivo" runat="server" Text="Adicionar" CssClass="btn btn-sm btn-primary" OnClick="btnAddMotivo_Click" CausesValidation="false" />
                    </div>
                </asp:PlaceHolder>
            </div>
        </div>

        <div class="col-md-6">
            <div class="crm-table-card">
                <div class="p-3 pb-0"><h5 class="mb-0">Taxas de IVA</h5></div>
                <asp:Repeater ID="rptTaxas" runat="server" OnItemCommand="rptTaxas_ItemCommand">
                    <HeaderTemplate><ul class="list-group list-group-flush"></HeaderTemplate>
                    <ItemTemplate>
                        <li class="list-group-item d-flex justify-content-between align-items-center">
                            <span class="<%# GetTextoClasse(Eval("IsActive")) %>"><%# Eval("Name") %> (<%# Eval("Percentage") %>%)</span>
                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-secondary" CommandName="Alternar" CommandArgument='<%# Eval("TaxRateId") %>'>
                                <%# GetTextoBotaoEstado(Eval("IsActive")) %>
                            </asp:LinkButton>
                        </li>
                    </ItemTemplate>
                    <FooterTemplate></ul></FooterTemplate>
                </asp:Repeater>
                <asp:PlaceHolder ID="phFormTaxa" runat="server">
                    <div class="p-3 border-top d-flex gap-2">
                        <asp:TextBox ID="txtNovaTaxaNome" runat="server" CssClass="form-control form-control-sm" placeholder="Nome..." />
                        <asp:TextBox ID="txtNovaTaxaPercentagem" runat="server" CssClass="form-control form-control-sm" placeholder="%" style="max-width:80px;" />
                        <asp:Button ID="btnAddTaxa" runat="server" Text="Adicionar" CssClass="btn btn-sm btn-primary" OnClick="btnAddTaxa_Click" CausesValidation="false" />
                    </div>
                </asp:PlaceHolder>
            </div>
        </div>

        <div class="col-md-6">
            <div class="crm-table-card">
                <div class="p-3 pb-0"><h5 class="mb-0">Condições de Pagamento</h5></div>
                <asp:Repeater ID="rptCondicoes" runat="server" OnItemCommand="rptCondicoes_ItemCommand">
                    <HeaderTemplate><ul class="list-group list-group-flush"></HeaderTemplate>
                    <ItemTemplate>
                        <li class="list-group-item d-flex justify-content-between align-items-center">
                            <span class="<%# GetTextoClasse(Eval("IsActive")) %>"><%# Eval("Name") %><%# GetDiasVencimentoTexto(Eval("DaysDue")) %></span>
                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-secondary" CommandName="Alternar" CommandArgument='<%# Eval("PaymentTermId") %>'>
                                <%# GetTextoBotaoEstado(Eval("IsActive")) %>
                            </asp:LinkButton>
                        </li>
                    </ItemTemplate>
                    <FooterTemplate></ul></FooterTemplate>
                </asp:Repeater>
                <asp:PlaceHolder ID="phFormCondicao" runat="server">
                    <div class="p-3 border-top d-flex gap-2">
                        <asp:TextBox ID="txtNovaCondicaoNome" runat="server" CssClass="form-control form-control-sm" placeholder="Nome..." />
                        <asp:TextBox ID="txtNovaCondicaoDias" runat="server" CssClass="form-control form-control-sm" placeholder="Dias" style="max-width:80px;" />
                        <asp:Button ID="btnAddCondicao" runat="server" Text="Adicionar" CssClass="btn btn-sm btn-primary" OnClick="btnAddCondicao_Click" CausesValidation="false" />
                    </div>
                </asp:PlaceHolder>
            </div>
        </div>

        <div class="col-md-6">
            <div class="crm-table-card">
                <div class="p-3 pb-0"><h5 class="mb-0">Países</h5></div>
                <asp:Repeater ID="rptPaises" runat="server" OnItemCommand="rptPaises_ItemCommand">
                    <HeaderTemplate><ul class="list-group list-group-flush"></HeaderTemplate>
                    <ItemTemplate>
                        <li class="list-group-item d-flex justify-content-between align-items-center">
                            <span class="<%# GetTextoClasse(Eval("IsActive")) %>"><%# Eval("Code") %> — <%# Eval("Name") %></span>
                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-secondary" CommandName="Alternar" CommandArgument='<%# Eval("CountryId") %>'>
                                <%# GetTextoBotaoEstado(Eval("IsActive")) %>
                            </asp:LinkButton>
                        </li>
                    </ItemTemplate>
                    <FooterTemplate></ul></FooterTemplate>
                </asp:Repeater>
                <asp:PlaceHolder ID="phFormPais" runat="server">
                    <div class="p-3 border-top d-flex gap-2">
                        <asp:TextBox ID="txtNovoPaisCodigo" runat="server" CssClass="form-control form-control-sm" placeholder="Código" style="max-width:80px;" MaxLength="3" />
                        <asp:TextBox ID="txtNovoPaisNome" runat="server" CssClass="form-control form-control-sm" placeholder="Nome..." />
                        <asp:Button ID="btnAddPais" runat="server" Text="Adicionar" CssClass="btn btn-sm btn-primary" OnClick="btnAddPais_Click" CausesValidation="false" />
                    </div>
                </asp:PlaceHolder>
            </div>
        </div>

    </div>

</asp:Content>