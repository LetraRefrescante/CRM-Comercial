<%@ Page Title="Agenda" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="Agenda.aspx.cs" Inherits="CRM.Web.Paginas.Atividades.Agenda" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item active">Agenda</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<style>
    .crm-calendar-mes { table-layout: fixed; }
    .crm-calendar-mes th { text-align: center; background: #f8f9fa; font-weight: 600; font-size: .8rem; }
    .crm-calendar-dia { height: 110px; vertical-align: top; padding: 4px !important; overflow: hidden; }
    .crm-calendar-dia-numero { font-size: .8rem; font-weight: 600; margin-bottom: 2px; display: flex; justify-content: space-between; align-items: center; }
    .crm-calendar-hoje .crm-calendar-dia-numero span:first-child { color: #fff; background: #1F7A5C; border-radius: 4px; padding: 0 5px; }
    .crm-calendar-evento { display: block; font-size: .7rem; padding: 1px 4px; margin-bottom: 2px; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; text-decoration: none; }
    .crm-calendar-semana .crm-calendar-dia { height: 340px; }
    .crm-calendar-dia-add { font-size: .75rem; color: #999; text-decoration: none; }
    .crm-calendar-dia-add:hover { color: #1F7A5C; }
    .crm-calendar-dia-evento-lista { list-style: none; padding: 0; margin: 4px 0 0; }
</style>

    <div class="crm-list-header">
        <h2>Agenda</h2>
        <asp:HyperLink ID="lnkNova" runat="server" NavigateUrl="~/Atividades/AtividadeEditar.aspx" CssClass="btn btn-primary">
            <i class="fas fa-plus"></i> Nova Atividade
        </asp:HyperLink>
    </div>

    <div class="crm-filter-card">
        <div class="row g-2 align-items-end">
            <div class="col-md-3">
                <label class="form-label">Tipo</label>
                <asp:DropDownList ID="ddlTipo" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Todos" Value="" />
                    <asp:ListItem Text="Chamada" Value="Chamada" />
                    <asp:ListItem Text="Email" Value="Email" />
                    <asp:ListItem Text="Reunião" Value="Reunião" />
                    <asp:ListItem Text="Visita" Value="Visita" />
                    <asp:ListItem Text="Nota" Value="Nota" />
                </asp:DropDownList>
            </div>
            <div class="col-md-3">
                <label class="form-label">Estado</label>
                <asp:DropDownList ID="ddlEstado" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Todos" Value="" />
                    <asp:ListItem Text="Planeada" Value="Planeada" />
                    <asp:ListItem Text="Em Curso" Value="Em Curso" />
                    <asp:ListItem Text="Concluída" Value="Concluída" />
                    <asp:ListItem Text="Cancelada" Value="Cancelada" />
                </asp:DropDownList>
            </div>
            <div class="col-md-3">
                <asp:Label ID="lblResponsavel" runat="server" CssClass="form-label" Text="Responsável" AssociatedControlID="ddlResponsavel" />
                <asp:DropDownList ID="ddlResponsavel" runat="server" CssClass="form-select" />
            </div>
            <div class="col-md-3">
                <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="btn btn-outline-secondary w-100" OnClick="btnFiltrar_Click" />
            </div>
        </div>
    </div>

    <div class="crm-table-card p-3">
        <div class="d-flex justify-content-between align-items-center mb-3 flex-wrap gap-2">
            <div class="btn-group" role="group">
                <asp:LinkButton ID="lnkVistaDia" runat="server" CommandArgument="Dia" OnCommand="lnkVista_Command">Dia</asp:LinkButton>
                <asp:LinkButton ID="lnkVistaSemana" runat="server" CommandArgument="Semana" OnCommand="lnkVista_Command">Semana</asp:LinkButton>
                <asp:LinkButton ID="lnkVistaMes" runat="server" CommandArgument="Mes" OnCommand="lnkVista_Command">Mês</asp:LinkButton>
            </div>

            <h5 class="mb-0"><asp:Literal ID="litPeriodo" runat="server" /></h5>

            <div class="btn-group" role="group">
                <asp:LinkButton ID="lnkAnterior" runat="server" CssClass="btn btn-outline-secondary btn-sm" OnClick="lnkAnterior_Click"><i class="fas fa-chevron-left"></i></asp:LinkButton>
                <asp:LinkButton ID="lnkHoje" runat="server" CssClass="btn btn-outline-secondary btn-sm" OnClick="lnkHoje_Click">Hoje</asp:LinkButton>
                <asp:LinkButton ID="lnkSeguinte" runat="server" CssClass="btn btn-outline-secondary btn-sm" OnClick="lnkSeguinte_Click"><i class="fas fa-chevron-right"></i></asp:LinkButton>
            </div>
        </div>

        <!-- ===================== Vista Mensal ===================== -->
        <asp:PlaceHolder ID="phMes" runat="server">
            <table class="table table-bordered crm-calendar-mes">
                <thead>
                    <tr>
                        <th>Segunda</th><th>Terça</th><th>Quarta</th><th>Quinta</th><th>Sexta</th><th>Sábado</th><th>Domingo</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptSemanasMes" runat="server">
                        <ItemTemplate>
                            <tr>
                                <asp:Repeater ID="rptDiasMes" runat="server" DataSource='<%# Eval("Dias") %>'>
                                    <ItemTemplate>
                                        <td class="<%# GetDiaCellClass(Container.DataItem) %>">
                                            <div class="crm-calendar-dia-numero">
                                                <span><%# Eval("Data", "{0:dd}") %></span>
                                                <a class='crm-calendar-dia-add <%# GetAddLinkClass() %>'
                                                   href='AtividadeEditar.aspx?data=<%# Eval("Data", "{0:yyyy-MM-dd}") %>'
                                                   title="Nova atividade neste dia">+</a>
                                            </div>
                                            <asp:Repeater runat="server" DataSource='<%# Eval("Atividades") %>'>
                                                <ItemTemplate>
                                                    <a href='AtividadeEditar.aspx?id=<%# Eval("ActivityId") %>' class="crm-calendar-evento badge <%# GetBadgeClasse(Eval("Status").ToString()) %>" title="<%# Eval("Subject") %>">
                                                        <%# Eval("StartDateTime", "{0:HH:mm}") %> <%# Eval("Subject") %>
                                                    </a>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </td>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </asp:PlaceHolder>

        <!-- ===================== Vista Semanal ===================== -->
        <asp:PlaceHolder ID="phSemana" runat="server" Visible="false">
            <table class="table table-bordered crm-calendar-mes crm-calendar-semana">
                <thead>
                    <tr>
                        <asp:Repeater ID="rptCabecalhoSemana" runat="server">
                            <ItemTemplate>
                                <th class="<%# GetHojeHeaderClass(Container.DataItem) %>">
                                    <%# Eval("Data", "{0:ddd, dd/MM}") %>
                                </th>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <asp:Repeater ID="rptDiasSemana" runat="server">
                            <ItemTemplate>
                                <td class="crm-calendar-dia">
                                    <a class='crm-calendar-dia-add <%# GetAddLinkClass() %>'
                                       href='AtividadeEditar.aspx?data=<%# Eval("Data", "{0:yyyy-MM-dd}") %>'>+ Nova</a>
                                    <ul class="crm-calendar-dia-evento-lista">
                                        <asp:Repeater runat="server" DataSource='<%# Eval("Atividades") %>'>
                                            <ItemTemplate>
                                                <li>
                                                    <a href='AtividadeEditar.aspx?id=<%# Eval("ActivityId") %>' class="crm-calendar-evento badge <%# GetBadgeClasse(Eval("Status").ToString()) %>" title="<%# Eval("Subject") %>">
                                                        <%# Eval("StartDateTime", "{0:HH:mm}") %> <%# Eval("Subject") %>
                                                    </a>
                                                </li>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </ul>
                                </td>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tr>
                </tbody>
            </table>
        </asp:PlaceHolder>

        <!-- ===================== Vista Diária ===================== -->
        <asp:PlaceHolder ID="phDia" runat="server" Visible="false">
            <asp:Repeater ID="rptAtividadesDia" runat="server">
                <HeaderTemplate>
                    <table class="table table-hover align-middle">
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td style="width:110px;" class="fw-semibold"><%# GetHorario(Container.DataItem) %></td>
                        <td style="width:110px;"><%# Eval("Type") %></td>
                        <td>
                            <a href='AtividadeEditar.aspx?id=<%# Eval("ActivityId") %>'><%# Eval("Subject") %></a>
                            <div class="text-muted small"><%# GetRelacionado(Container.DataItem) %></div>
                        </td>
                        <td><%# Eval("AssignedTo.Name") %></td>
                        <td class="text-end">
                            <span class="badge <%# GetBadgeClasse(Eval("Status").ToString()) %>"><%# Eval("Status") %></span>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                        </tbody>
                    </table>
                </FooterTemplate>
            </asp:Repeater>

            <asp:PlaceHolder ID="phDiaVazio" runat="server" Visible="false">
                <div class="crm-empty-state">
                    <i class="fas fa-calendar-day"></i>
                    <p class="mb-0">Sem atividades agendadas para este dia.</p>
                </div>
            </asp:PlaceHolder>
        </asp:PlaceHolder>

    </div>

</asp:Content>