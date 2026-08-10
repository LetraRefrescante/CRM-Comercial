<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OportunidadesLista.aspx.cs" Inherits="CRM.Web.Oportunidades.OportunidadesLista" MasterPageFile="~/MasterPages/Site.Master" %>

<asp:Content ID="ContentBreadcrumb" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item"><a href="~/Dashboard/Dashboard.aspx" runat="server">Dashboard</a></li>
    <li class="breadcrumb-item active">Oportunidades</li>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="MainContent" runat="server">

    <div class="d-flex justify-content-between align-items-center mb-3">
        <h4 class="mb-0">Oportunidades</h4>
        <div class="d-flex gap-2">
            <a href="~/Oportunidades/Pipeline.aspx" runat="server" class="btn btn-outline-secondary">
                <i class="fas fa-columns"></i> Ver Pipeline
            </a>
            <asp:HyperLink ID="lnkNova" runat="server" CssClass="btn btn-primary" NavigateUrl="~/Oportunidades/OportunidadeEditar.aspx">
                <i class="fas fa-plus"></i> Nova Oportunidade
            </asp:HyperLink>
        </div>
    </div>

    <div class="card mb-3">
        <div class="card-body">
            <asp:UpdatePanel ID="upFiltros" runat="server">
                <ContentTemplate>
                    <div class="row g-2 align-items-end">
                        <div class="col-md-3">
                            <label class="form-label small">Pesquisa</label>
                            <asp:TextBox ID="txtPesquisa" runat="server" CssClass="form-control" placeholder="Título ou cliente..." />
                        </div>
                        <div class="col-md-2">
                            <label class="form-label small">Fase</label>
                            <asp:DropDownList ID="ddlFase" runat="server" CssClass="form-select">
                                <asp:ListItem Text="Todas" Value="" />
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-2">
                            <label class="form-label small">Estado</label>
                            <asp:DropDownList ID="ddlEstado" runat="server" CssClass="form-select">
                                <asp:ListItem Text="Todas" Value="" />
                                <asp:ListItem Text="Abertas" Value="aberta" />
                                <asp:ListItem Text="Fechadas" Value="fechada" />
                            </asp:DropDownList>
                        </div>
                        <asp:PlaceHolder ID="phFiltroComercial" runat="server">
                            <div class="col-md-2">
                                <label class="form-label small">Comercial</label>
                                <asp:DropDownList ID="ddlComercial" runat="server" CssClass="form-select">
                                    <asp:ListItem Text="Todos" Value="" />
                                </asp:DropDownList>
                            </div>
                        </asp:PlaceHolder>
                        <div class="col-md-auto">
                            <asp:Button ID="btnPesquisar" runat="server" Text="Filtrar" CssClass="btn btn-primary" OnClick="btnPesquisar_Click" />
                        </div>
                        <div class="col-md-auto">
                            <asp:LinkButton ID="lnkLimpar" runat="server" CssClass="btn btn-link" OnClick="lnkLimpar_Click">Limpar</asp:LinkButton>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

    <div class="card">
        <div class="card-body p-0">
            <asp:UpdatePanel ID="upLista" runat="server">
                <ContentTemplate>
                    <div class="table-responsive">
                        <table class="table table-hover mb-0 align-middle">
                            <thead>
                                <tr>
                                    <th>Título</th>
                                    <th>Cliente</th>
                                    <th>Fase</th>
                                    <th class="text-end">Valor Estimado</th>
                                    <th class="text-end">Valor Ponderado</th>
                                    <th class="text-center">Probab.</th>
                                    <th>Fecho Previsto</th>
                                    <th>Comercial</th>
                                    <th>Estado</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptOportunidades" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%# Eval("Title") %></td>
                                            <td><%# Eval("Client.TradeName") %></td>
                                            <td><span class="badge bg-secondary"><%# Eval("Stage.Name") %></span></td>
                                            <td class="text-end"><%# FormatarMoeda(Eval("EstimatedValue")) %></td>
                                            <td class="text-end"><%# FormatarMoeda(CalcularValorPonderado((CRM.Models.Entities.Oportunidades.Opportunity)Container.DataItem)) %></td>
                                            <td class="text-center"><%# Eval("Probability") %>%</td>
                                            <td><%# Eval("ExpectedCloseDate", "{0:dd/MM/yyyy}") %></td>
                                            <td><%# Eval("Owner.Name") %></td>
                                            <td><%# ObterBadgeEstado((CRM.Models.Entities.Oportunidades.Opportunity)Container.DataItem) %></td>
                                            <td class="text-end text-nowrap">
                                                <a class="btn btn-sm btn-outline-secondary" href='<%# "OportunidadeEditar.aspx?id=" + Eval("OpportunityId") %>'>
                                                    <i class="fas fa-eye"></i>
                                                </a>
                                                <asp:PlaceHolder runat="server" Visible='<%# PodeFecharLinha((CRM.Models.Entities.Oportunidades.Opportunity)Container.DataItem) %>'>
                                                    <a class="btn btn-sm btn-outline-success" href='<%# "OportunidadeFechar.aspx?id=" + Eval("OpportunityId") %>'>
                                                        <i class="fas fa-flag-checkered"></i>
                                                    </a>
                                                </asp:PlaceHolder>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                    <asp:PlaceHolder ID="phVazio" runat="server" Visible="false">
                        <p class="text-muted text-center py-4 mb-0">Nenhuma oportunidade encontrada com estes filtros.</p>
                    </asp:PlaceHolder>

                    <div class="p-3">
                        <uc:Paginacao ID="ucPaginacao" runat="server" OnPaginaAlterada="ucPaginacao_PaginaAlterada" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

</asp:Content>