<%@ Page Title="Detalhe do Lead" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="LeadDetalhe.aspx.cs" Inherits="CRM.Web.Paginas.Leads.LeadDetalhe" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item"><a href="~/Leads/LeadsLista.aspx" runat="server">Leads</a></li>
    <li class="breadcrumb-item active"><%: NomeLead %></li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="crm-list-header">
        <h2><%: NomeLead %></h2>
        <div class="d-flex gap-2">
            <asp:PlaceHolder ID="phEditar" runat="server">
                <asp:HyperLink ID="lnkEditar" runat="server" CssClass="btn btn-outline-secondary">
                    <i class="fas fa-pen"></i> Editar
                </asp:HyperLink>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="phConverter" runat="server">
                <asp:HyperLink ID="lnkConverter" runat="server" CssClass="btn btn-success">
                    <i class="fas fa-right-left"></i> Converter
                </asp:HyperLink>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="phEliminar" runat="server">
                <asp:LinkButton ID="btnEliminar" runat="server" CssClass="btn btn-outline-danger" OnClick="btnEliminar_Click"
                    data-confirm="Eliminar este lead? O registo é mantido para auditoria.">
                    <i class="fas fa-trash"></i> Eliminar
                </asp:LinkButton>
            </asp:PlaceHolder>
        </div>
    </div>

    <asp:PlaceHolder ID="phBloqueado" runat="server" Visible="false">
        <div class="alert alert-info">
            Este lead já foi convertido<asp:PlaceHolder ID="phLinkClienteConvertido" runat="server" Visible="false">
                — <a href="ClienteDetalhe.aspx?id=<%# ClienteConvertidoId %>">ver cliente</a></asp:PlaceHolder> e está bloqueado para edição comercial.
        </div>
    </asp:PlaceHolder>

    <div class="row g-3">
        <!-- Resumo -->
        <div class="col-lg-6">
            <div class="crm-form-card h-100">
                <h5 class="mb-3">Resumo</h5>
                <dl class="row mb-0">
                    <dt class="col-sm-4">Nome</dt>
                    <dd class="col-sm-8"><asp:Literal ID="litNome" runat="server" /></dd>

                    <dt class="col-sm-4">Empresa</dt>
                    <dd class="col-sm-8"><asp:Literal ID="litEmpresa" runat="server" /></dd>

                    <dt class="col-sm-4">Email</dt>
                    <dd class="col-sm-8"><asp:Literal ID="litEmail" runat="server" /></dd>

                    <dt class="col-sm-4">Telefone</dt>
                    <dd class="col-sm-8"><asp:Literal ID="litTelefone" runat="server" /></dd>

                    <dt class="col-sm-4">Origem</dt>
                    <dd class="col-sm-8"><asp:Literal ID="litOrigem" runat="server" /></dd>

                    <dt class="col-sm-4">Estado</dt>
                    <dd class="col-sm-8">
                        <span class="badge <%# GetBadgeClasse(Lead.Status) %>"><%# Lead.Status %></span>
                    </dd>

                    <dt class="col-sm-4">Pontuação</dt>
                    <dd class="col-sm-8"><asp:Literal ID="litPontuacao" runat="server" /></dd>

                    <dt class="col-sm-4">Comercial</dt>
                    <dd class="col-sm-8"><asp:Literal ID="litComercial" runat="server" /></dd>

                    <dt class="col-sm-4">Próximo Contacto</dt>
                    <dd class="col-sm-8"><asp:Literal ID="litProximoContacto" runat="server" /></dd>

                    <asp:PlaceHolder ID="phMotivoPerda" runat="server" Visible="false">
                        <dt class="col-sm-4">Motivo de Perda</dt>
                        <dd class="col-sm-8"><asp:Literal ID="litMotivoPerda" runat="server" /></dd>
                    </asp:PlaceHolder>

                    <dt class="col-sm-4">Criado em</dt>
                    <dd class="col-sm-8"><asp:Literal ID="litCriadoEm" runat="server" /></dd>
                </dl>
            </div>
        </div>

        <!-- Histórico de Estados -->
        <div class="col-lg-6">
            <div class="crm-form-card h-100">
                <h5 class="mb-3">Histórico de Estados</h5>
                <asp:Repeater ID="rptHistoricoEstados" runat="server">
                    <HeaderTemplate>
                        <ul class="list-group">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li class="list-group-item">
                            <div class="d-flex justify-content-between">
                                <span>
                                    <%#: Eval("PreviousStatus") ?? "—" %>
                                    <i class="fas fa-arrow-right mx-1 text-muted"></i>
                                    <strong><%#: Eval("NewStatus") %></strong>
                                </span>
                                <span class="text-muted small"><%#: Eval("ChangedDate", "{0:dd/MM/yyyy HH:mm}") %></span>
                            </div>
                            <div class="text-muted small"><%#: Eval("ChangedByName") %></div>
                        </li>
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>

                <asp:PlaceHolder ID="phSemHistorico" runat="server" Visible="false">
                    <p class="text-muted text-center mb-0">Ainda sem alterações de estado registadas.</p>
                </asp:PlaceHolder>
            </div>
        </div>

        <!-- Atividades -->
        <div class="col-12">
            <div class="crm-form-card">
                <h5 class="mb-3">Atividades</h5>

                <asp:PlaceHolder ID="phNovaAtividade" runat="server">
                    <div class="row g-2 align-items-end mb-3 pb-3 border-bottom">
                        <div class="col-md-2">
                            <label class="form-label">Tipo</label>
                            <asp:DropDownList ID="ddlTipoAtividade" runat="server" CssClass="form-select">
                                <asp:ListItem Text="Chamada" Value="Chamada" />
                                <asp:ListItem Text="Email" Value="Email" />
                                <asp:ListItem Text="Reunião" Value="Reunião" />
                                <asp:ListItem Text="Visita" Value="Visita" />
                                <asp:ListItem Text="Nota" Value="Nota" Selected="True" />
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-3">
                            <label class="form-label">Assunto *</label>
                            <asp:TextBox ID="txtAssunto" runat="server" CssClass="form-control" MaxLength="180" />
                        </div>
                        <div class="col-md-2">
                            <label class="form-label">Responsável</label>
                            <asp:DropDownList ID="ddlResponsavelAtividade" runat="server" CssClass="form-select" />
                        </div>
                        <div class="col-md-2">
                            <label class="form-label">Data/Hora</label>
                            <asp:TextBox ID="txtDataHoraAtividade" runat="server" CssClass="form-control" TextMode="DateTimeLocal" />
                        </div>
                        <div class="col-md-2">
                            <label class="form-label">Estado</label>
                            <asp:DropDownList ID="ddlEstadoAtividade" runat="server" CssClass="form-select">
                                <asp:ListItem Text="Concluída" Value="Concluída" Selected="True" />
                                <asp:ListItem Text="Planeada" Value="Planeada" />
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-1">
                            <asp:Button ID="btnRegistarAtividade" runat="server" Text="Registar" CssClass="btn btn-primary w-100"
                                OnClick="btnRegistarAtividade_Click" CausesValidation="false" />
                        </div>
                        <div class="col-12">
                            <asp:TextBox ID="txtDescricaoAtividade" runat="server" CssClass="form-control mt-2" TextMode="MultiLine" Rows="2" placeholder="Descrição (opcional)" />
                        </div>
                        <div class="col-12">
                            <asp:Label ID="lblErroAtividade" runat="server" CssClass="text-danger small d-block mt-2" Visible="false" />
                        </div>
                    </div>
                </asp:PlaceHolder>

                 <asp:Repeater ID="rptAtividades" runat="server">
                    <HeaderTemplate>
                        <ul class="list-group">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li class="list-group-item">
                            <div class="d-flex justify-content-between">
                                <span>
                                    <span class="badge bg-secondary"><%#: Eval("Type") %></span>
                                    <strong class="ms-2"><%#: Eval("Subject") %></strong>
                                </span>
                                <span class="text-muted small"><%#: Eval("StartDateTime", "{0:dd/MM/yyyy HH:mm}") %></span>
                            </div>
                            <div class="text-muted small">
                                <%#: Eval("AssignedTo.Name") %> · <%#: Eval("Status") %>
                            </div>
                            <asp:PlaceHolder runat="server" Visible='<%# !string.IsNullOrEmpty(Eval("Description") as string) %>'>
                                <div class="small mt-1"><%#: Eval("Description") %></div>
                            </asp:PlaceHolder>
                        </li>
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>

                <asp:PlaceHolder ID="phSemAtividades" runat="server" Visible="false">
                    <p class="text-muted text-center mb-0">Ainda não existem atividades registadas para este lead.</p>
                </asp:PlaceHolder>
            </div>
        </div>
    </div>

</asp:Content>