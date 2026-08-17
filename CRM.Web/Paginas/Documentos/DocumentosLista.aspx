<%@ Page Title="Documentos" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="DocumentosLista.aspx.cs" Inherits="CRM.Web.Paginas.Documentos.DocumentosLista" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item active">Documentos</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="crm-list-header">
        <h2>Documentos</h2>
        <asp:HyperLink ID="lnkNovo" runat="server" NavigateUrl="~/Documentos/DocumentoEditar.aspx" CssClass="btn btn-primary">
            <i class="fas fa-upload"></i> Carregar Documento
        </asp:HyperLink>
    </div>

    <div class="crm-filter-card">
        <div class="row g-2 align-items-end">
            <div class="col-md-3">
                <label class="form-label">Pesquisar</label>
                <asp:TextBox ID="txtPesquisa" runat="server" CssClass="form-control" placeholder="Título ou nome do ficheiro..." />
            </div>
            <div class="col-md-2">
                <label class="form-label">Categoria</label>
                <asp:DropDownList ID="ddlCategoria" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Todas" Value="" />
                    <asp:ListItem Text="Contrato" Value="Contrato" />
                    <asp:ListItem Text="Proposta" Value="Proposta" />
                    <asp:ListItem Text="Identificação" Value="Identificação" />
                    <asp:ListItem Text="Outro" Value="Outro" />
                </asp:DropDownList>
            </div>
            <div class="col-md-2">
                <label class="form-label">Relacionado Com</label>
                <asp:DropDownList ID="ddlTipoEntidade" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Todos" Value="" />
                    <asp:ListItem Text="Cliente" Value="Client" />
                    <asp:ListItem Text="Lead" Value="Lead" />
                    <asp:ListItem Text="Oportunidade" Value="Opportunity" />
                    <asp:ListItem Text="Proposta" Value="Proposal" />
                    <asp:ListItem Text="Venda" Value="Sale" />
                </asp:DropDownList>
            </div>
            <div class="col-md-2">
                <label class="form-label">Confidencial</label>
                <asp:DropDownList ID="ddlConfidencial" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Todos" Value="" />
                    <asp:ListItem Text="Sim" Value="Sim" />
                    <asp:ListItem Text="Não" Value="Nao" />
                </asp:DropDownList>
            </div>
            <div class="col-md-2">
                <label class="form-label">Período</label>
                <uc:FiltroDatas ID="ucFiltroDatas" runat="server" />
            </div>
            <div class="col-md-1">
                <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="btn btn-outline-secondary w-100" OnClick="btnFiltrar_Click" />
            </div>
        </div>
    </div>

    <div class="crm-table-card">
        <asp:Repeater ID="rptDocumentos" runat="server" OnItemCommand="rptDocumentos_ItemCommand">
            <HeaderTemplate>
                <table class="table table-hover mb-0 align-middle">
                    <thead>
                        <tr>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="Title" OnCommand="lnkOrdenar_Command">Título</asp:LinkButton></th>
                            <th>Categoria</th>
                            <th>Relacionado Com</th>
                            <th>Tamanho</th>
                            <th><asp:LinkButton runat="server" CssClass="crm-th-sort" CommandName="Ordenar" CommandArgument="CreatedDate" OnCommand="lnkOrdenar_Command">Data</asp:LinkButton></th>
                            <th></th>
                            <th class="text-end">Ações</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# Eval("Title") %></td>
                    <td><%# Eval("Category") %></td>
                    <td><%# GetRelacionadoTexto(Container.DataItem) %></td>
                    <td><%# GetTamanhoFormatado(Eval("FileSizeBytes")) %></td>
                    <td><%# Eval("CreatedDate", "{0:dd/MM/yyyy HH:mm}") %></td>
                    <td>
                        <asp:PlaceHolder runat="server" Visible='<%# (bool)Eval("IsConfidential") %>'>
                            <i class="fas fa-lock text-muted" title="Confidencial"></i>
                        </asp:PlaceHolder>
                    </td>
                    <td class="text-end crm-row-actions">
                        <a href="DocumentoDownload.aspx?id=<%# Eval("DocumentId") %>" class="btn btn-sm btn-outline-secondary" title="Transferir">
                            <i class="fas fa-download"></i>
                        </a>
                        <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-danger" ToolTip="Eliminar"
                            CommandName="Eliminar" CommandArgument='<%# Eval("DocumentId") %>'
                            OnClientClick="return confirm('Eliminar este documento? O registo é mantido para auditoria.');">
                            <i class="fas fa-trash"></i>
                        </asp:LinkButton>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                    </tbody>
                </table>
            </FooterTemplate>
        </asp:Repeater>

        <asp:PlaceHolder ID="phVazio" runat="server" Visible="false">
            <div class="crm-empty-state">
                <i class="fas fa-folder-open"></i>
                <p class="mb-0">Nenhum documento encontrado com os filtros atuais.</p>
            </div>
        </asp:PlaceHolder>
    </div>

    <uc:Paginacao ID="ucPaginacao" runat="server" OnPaginaAlterada="ucPaginacao_PaginaAlterada" />

</asp:Content>