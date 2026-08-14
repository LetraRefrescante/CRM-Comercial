<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OportunidadeDetalhe.aspx.cs" Inherits="CRM.Web.Oportunidades.OportunidadeDetalhe" MasterPageFile="~/MasterPages/Site.Master" %>

<asp:Content ID="ContentBreadcrumb" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item"><a href="~/Dashboard/Dashboard.aspx" runat="server">Dashboard</a></li>
    <li class="breadcrumb-item"><a href="~/Oportunidades/Pipeline.aspx" runat="server">Oportunidades</a></li>
    <li class="breadcrumb-item active">Detalhe</li>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="MainContent" runat="server">

    <div class="d-flex justify-content-between align-items-start mb-3">
        <div>
            <h4 class="mb-1"><asp:Literal ID="litTitulo" runat="server" /></h4>
            <span id="spanEstado" runat="server" class="badge"></span>
            <asp:PlaceHolder ID="phMotivoPerda" runat="server" Visible="false">
                <span class="text-muted small ms-1">— <asp:Literal ID="litMotivoPerda" runat="server" /></span>
            </asp:PlaceHolder>
        </div>
        <div class="d-flex gap-2">
            <asp:HyperLink ID="lnkEditar" runat="server" CssClass="btn btn-outline-secondary" Visible="false">
                <i class="fas fa-pen"></i> Editar
            </asp:HyperLink>
            <asp:HyperLink ID="lnkFechar" runat="server" CssClass="btn btn-success" Visible="false">
                <i class="fas fa-flag-checkered"></i> Fechar
            </asp:HyperLink>
            <a href="~/Oportunidades/Pipeline.aspx" runat="server" class="btn btn-outline-secondary">Voltar</a>
        </div>
    </div>

    <div class="card mb-3">
        <div class="card-body">
            <div class="row g-3 small">
                <div class="col-md-3">
                    <div class="text-muted">Cliente</div>
                    <div class="fw-semibold"><asp:Literal ID="litCliente" runat="server" /></div>
                </div>
                <div class="col-md-3">
                    <div class="text-muted">Contacto</div>
                    <div class="fw-semibold"><asp:Literal ID="litContacto" runat="server" /></div>
                </div>
                <div class="col-md-3">
                    <div class="text-muted">Fase</div>
                    <div class="fw-semibold"><span class="badge bg-secondary"><asp:Literal ID="litFase" runat="server" /></span></div>
                </div>
                <div class="col-md-3">
                    <div class="text-muted">Comercial Responsável</div>
                    <div class="fw-semibold"><asp:Literal ID="litComercial" runat="server" /></div>
                </div>

                <div class="col-md-3">
                    <div class="text-muted">Valor Estimado</div>
                    <div class="fw-semibold money"><asp:Literal ID="litValor" runat="server" /></div>
                </div>
                <div class="col-md-3">
                    <div class="text-muted">Valor Ponderado</div>
                    <div class="fw-semibold money"><asp:Literal ID="litValorPonderado" runat="server" /></div>
                </div>
                <div class="col-md-3">
                    <div class="text-muted">Probabilidade</div>
                    <div class="fw-semibold"><asp:Literal ID="litProbabilidade" runat="server" />%</div>
                </div>
                <div class="col-md-3">
                    <div class="text-muted">Fecho Previsto</div>
                    <div class="fw-semibold"><asp:Literal ID="litDataFecho" runat="server" /></div>
                </div>

                <asp:PlaceHolder ID="phConcorrente" runat="server" Visible="false">
                    <div class="col-md-6">
                        <div class="text-muted">Concorrente</div>
                        <div class="fw-semibold"><asp:Literal ID="litConcorrente" runat="server" /></div>
                    </div>
                </asp:PlaceHolder>
            </div>
        </div>
    </div>

    <!-- ===================== Propostas ===================== -->
    <div class="card mb-3">
        <div class="card-header d-flex justify-content-between align-items-center bg-white">
            <h6 class="mb-0">Propostas</h6>
            <asp:HyperLink ID="lnkNovaProposta" runat="server" CssClass="btn btn-sm btn-outline-primary" Visible="false">
                <i class="fas fa-plus"></i> Nova Proposta
            </asp:HyperLink>
        </div>
        <div class="card-body p-0">
            <div class="table-responsive">
                <table class="table table-hover mb-0 align-middle">
                    <thead>
                        <tr>
                            <th>Número</th>
                            <th>Versão</th>
                            <th>Estado</th>
                            <th class="text-end">Total</th>
                            <th>Emissão</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rptPropostas" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td><%# Eval("ProposalNumber") %></td>
                                    <td>v<%# Eval("VersionNumber") %></td>
                                    <td><span class="badge <%# GetBadgeClassePropostaEstado(Eval("Status").ToString()) %>"><%# Eval("Status") %></span></td>
                                    <td class="text-end"><%# Eval("Total", "{0:N2} €") %></td>
                                    <td><%# Eval("IssueDate", "{0:dd/MM/yyyy}") %></td>
                                    <td class="text-end">
                                        <a class="btn btn-sm btn-outline-secondary" href='<%# "~/Catalogo/PropostaDetalhe.aspx?id=" + Eval("ProposalId") %>' runat="server">
                                            <i class="fas fa-eye"></i>
                                        </a>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </div>
            <asp:PlaceHolder ID="phPropostasVazio" runat="server" Visible="false">
                <p class="text-muted text-center py-3 mb-0">Ainda não há propostas para esta oportunidade.</p>
            </asp:PlaceHolder>
        </div>
    </div>

    <!-- ===================== Atividades ===================== -->
    <div class="card mb-3">
        <div class="card-header bg-white">
            <h6 class="mb-0">Atividades Relacionadas</h6>
        </div>
        <div class="card-body p-0">
            <div class="table-responsive">
                <table class="table table-hover mb-0 align-middle">
                    <thead>
                        <tr>
                            <th>Assunto</th>
                            <th>Tipo</th>
                            <th>Responsável</th>
                            <th>Início</th>
                            <th>Estado</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rptAtividades" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td><%# Eval("Subject") %></td>
                                    <td><%# Eval("Type") %></td>
                                    <td><%# Eval("AssignedTo.Name") %></td>
                                    <td><%# Eval("StartDateTime", "{0:dd/MM/yyyy HH:mm}") %></td>
                                    <td><span class="badge <%# GetBadgeClasseAtividadeEstado(Eval("Status").ToString()) %>"><%# Eval("Status") %></span></td>
                                    <td class="text-end">
                                        <a class="btn btn-sm btn-outline-secondary" href='<%# "~/Atividades/AtividadeEditar.aspx?id=" + Eval("ActivityId") %>' runat="server">
                                            <i class="fas fa-eye"></i>
                                        </a>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </div>
            <asp:PlaceHolder ID="phAtividadesVazio" runat="server" Visible="false">
                <p class="text-muted text-center py-3 mb-0">Ainda não há atividades relacionadas com esta oportunidade.</p>
            </asp:PlaceHolder>
            <div class="alert alert-warning small m-3 mb-3">
                <i class="fas fa-triangle-exclamation"></i>
                A <code>AtividadeEditar.aspx</code> atual só permite relacionar atividades com Cliente ou Lead —
                a opção "Oportunidade" ainda não foi adicionada ao formulário, por isso não há aqui um botão
                "Nova Atividade" (criaria uma atividade órfã, sem ligação real a esta oportunidade).
            </div>
        </div>
    </div>

    <!-- ===================== Histórico de Fases ===================== -->
    <div class="card mb-3">
        <div class="card-header bg-white">
            <h6 class="mb-0">Histórico de Fases</h6>
        </div>
        <div class="card-body p-0">
            <div class="table-responsive">
                <table class="table table-sm mb-0 align-middle">
                    <thead>
                        <tr>
                            <th>Data</th>
                            <th>De</th>
                            <th>Para</th>
                            <th>Utilizador</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rptHistorico" runat="server" OnItemDataBound="rptHistorico_ItemDataBound">
                            <ItemTemplate>
                                <tr>
                                    <td class="mono small"><%# Eval("ChangedDate", "{0:dd/MM/yyyy HH:mm}") %></td>
                                    <td><%# Eval("PreviousStage.Name") ?? "—" %></td>
                                    <td><%# Eval("NewStage.Name") %></td>
                                    <td><asp:Literal ID="litUtilizadorHistorico" runat="server" /></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </div>
            <asp:PlaceHolder ID="phHistoricoVazio" runat="server" Visible="false">
                <p class="text-muted text-center py-3 mb-0">Sem alterações de fase registadas.</p>
            </asp:PlaceHolder>
        </div>
    </div>

    <div class="alert alert-secondary small">
        <i class="fas fa-circle-info"></i>
        A blueprint da Fase 3 lista "Produtos" como secção desta página, mas não existe nenhuma tabela
        (ex. <code>OpportunityLine</code>) que associe produtos diretamente a uma Oportunidade — os
        produtos só ficam registados dentro de cada Proposta (secção acima). Se for suposto existir um
        registo de produtos ao nível da Oportunidade (antes de haver proposta), é preciso desenhar essa
        entidade primeiro.
    </div>

</asp:Content>