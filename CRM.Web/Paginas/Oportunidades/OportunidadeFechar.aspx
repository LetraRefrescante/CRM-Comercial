<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OportunidadeFechar.aspx.cs" Inherits="CRM.Web.Oportunidades.OportunidadeFechar" MasterPageFile="~/MasterPages/Site.Master" %>

<asp:Content ID="ContentBreadcrumb" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item"><a href="~/Dashboard/Dashboard.aspx" runat="server">Dashboard</a></li>
    <li class="breadcrumb-item"><a href="~/Oportunidades/Pipeline.aspx" runat="server">Oportunidades</a></li>
    <li class="breadcrumb-item active">Fechar</li>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="MainContent" runat="server">

    <h4 class="mb-3">Fechar Oportunidade</h4>

    <div class="card mb-3">
        <div class="card-body">
            <h5 class="mb-3"><asp:Literal ID="litTitulo" runat="server" /></h5>
            <div class="row g-3 small">
                <div class="col-md-3">
                    <div class="text-muted">Cliente</div>
                    <div class="fw-semibold"><asp:Literal ID="litCliente" runat="server" /></div>
                </div>
                <div class="col-md-3">
                    <div class="text-muted">Comercial</div>
                    <div class="fw-semibold"><asp:Literal ID="litComercial" runat="server" /></div>
                </div>
                <div class="col-md-2">
                    <div class="text-muted">Fase Atual</div>
                    <div class="fw-semibold"><asp:Literal ID="litFase" runat="server" /></div>
                </div>
                <div class="col-md-2">
                    <div class="text-muted">Valor Estimado</div>
                    <div class="fw-semibold money"><asp:Literal ID="litValor" runat="server" /></div>
                </div>
                <div class="col-md-2">
                    <div class="text-muted">Fecho Previsto</div>
                    <div class="fw-semibold"><asp:Literal ID="litDataFecho" runat="server" /></div>
                </div>
            </div>
        </div>
    </div>

    <div class="card">
        <div class="card-body">

            <asp:UpdatePanel ID="upResultado" runat="server">
                <ContentTemplate>

                    <label class="form-label">Resultado *</label>
                    <asp:RadioButtonList ID="rblResultado" runat="server" AutoPostBack="true"
                        OnSelectedIndexChanged="rblResultado_SelectedIndexChanged" CssClass="mb-2">
                        <asp:ListItem Text="Ganha — o cliente aceitou a proposta" Value="ganho" />
                        <asp:ListItem Text="Perdida — o negócio não avançou" Value="perdido" />
                    </asp:RadioButtonList>
                    <asp:RequiredFieldValidator ID="rfvResultado" runat="server" ControlToValidate="rblResultado"
                        Display="Dynamic" CssClass="crm-validation-message text-danger small" ErrorMessage="Seleciona se a oportunidade foi ganha ou perdida." />

                    <asp:PlaceHolder ID="phMotivoPerda" runat="server" Visible="false">
                        <div class="mt-3">
                            <label class="form-label">Motivo de Perda *</label>
                            <asp:DropDownList ID="ddlMotivoPerda" runat="server" CssClass="form-select" />
                            <asp:CustomValidator ID="cvMotivoPerda" runat="server" ControlToValidate="ddlMotivoPerda"
                                OnServerValidate="cvMotivoPerda_ServerValidate" Display="Dynamic" CssClass="crm-validation-message text-danger small"
                                ErrorMessage="O motivo de perda é obrigatório." />
                        </div>
                    </asp:PlaceHolder>

                </ContentTemplate>
            </asp:UpdatePanel>

            <hr class="my-4" />

            <div class="d-flex gap-2">
                <asp:Button ID="btnConfirmar" runat="server" Text="Confirmar Fecho" CssClass="btn btn-primary"
                    OnClick="btnConfirmar_Click"
                    OnClientClick="return confirm('Depois de fechada, esta oportunidade só pode ser reaberta por um Administrador diretamente na base de dados. Confirmas?');" />
                <a href="~/Oportunidades/OportunidadesLista.aspx" runat="server" class="btn btn-outline-secondary">Cancelar</a>
            </div>

        </div>
    </div>

</asp:Content>