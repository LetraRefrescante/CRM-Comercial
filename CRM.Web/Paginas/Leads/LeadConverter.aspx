<%@ Page Title="Converter Lead" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="LeadConverter.aspx.cs" Inherits="CRM.Web.Paginas.Leads.LeadConverter" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item"><a href="~/Leads/LeadsLista.aspx" runat="server">Leads</a></li>
    <li class="breadcrumb-item"><asp:HyperLink ID="lnkBreadcrumbLead" runat="server">Lead</asp:HyperLink></li>
    <li class="breadcrumb-item active">Converter</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Converter Lead: <asp:Literal ID="litNomeLead" runat="server" /></h2>

    <div class="alert alert-info">
        A conversão cria o Cliente (ou associa a um existente), opcionalmente um Contacto
        e uma Oportunidade, e bloqueia este lead para edição comercial. Esta ação não pode ser desfeita.
    </div>

    <asp:Label ID="lblErros" runat="server" CssClass="alert alert-danger d-block" Visible="false" />

    <!-- Cliente -->
    <div class="crm-form-card mb-3">
        <h5 class="mb-3">Cliente</h5>

        <asp:RadioButtonList ID="rblTipoCliente" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"
            CssClass="mb-3" AutoPostBack="true" OnSelectedIndexChanged="rblTipoCliente_SelectedIndexChanged">
            <asp:ListItem Text="&nbsp;Criar cliente novo&nbsp;&nbsp;&nbsp;" Value="Novo" Selected="True" />
            <asp:ListItem Text="&nbsp;Associar a cliente existente" Value="Existente" />
        </asp:RadioButtonList>

        <asp:PlaceHolder ID="phClienteNovo" runat="server">
            <div class="row g-3">
                <div class="col-md-4">
                    <label class="form-label">NIF *</label>
                    <asp:TextBox ID="txtNif" runat="server" CssClass="form-control" MaxLength="20" />
                </div>
                <div class="col-md-8">
                    <label class="form-label">Nome Comercial *</label>
                    <asp:TextBox ID="txtNomeComercial" runat="server" CssClass="form-control" MaxLength="150" />
                </div>
                <div class="col-md-6">
                    <label class="form-label">Nome Legal</label>
                    <asp:TextBox ID="txtNomeLegal" runat="server" CssClass="form-control" MaxLength="200" />
                </div>
                <div class="col-md-3">
                    <label class="form-label">Email</label>
                    <asp:TextBox ID="txtClienteEmail" runat="server" CssClass="form-control" MaxLength="150" TextMode="Email" />
                </div>
                <div class="col-md-3">
                    <label class="form-label">Telefone</label>
                    <asp:TextBox ID="txtClienteTelefone" runat="server" CssClass="form-control" MaxLength="30" />
                </div>
                <div class="col-md-4">
                    <label class="form-label">País *</label>
                    <asp:DropDownList ID="ddlPais" runat="server" CssClass="form-select" />
                </div>
                <div class="col-md-4">
                    <label class="form-label">Setor</label>
                    <asp:DropDownList ID="ddlSetor" runat="server" CssClass="form-select" />
                </div>
                <div class="col-md-4">
                    <label class="form-label">Comercial Responsável *</label>
                    <asp:DropDownList ID="ddlComercialCliente" runat="server" CssClass="form-select" />
                </div>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="phClienteExistente" runat="server" Visible="false">
            <uc:SeletorCliente ID="ucSeletorCliente" runat="server" Obrigatorio="false" OcultarCampoTexto="false" />
        </asp:PlaceHolder>
    </div>

    <!-- Contacto -->
    <div class="crm-form-card mb-3">
        <div class="form-check mb-3">
            <asp:CheckBox ID="chkCriarContacto" runat="server" CssClass="form-check-input" Checked="true"
                AutoPostBack="true" OnCheckedChanged="chkCriarContacto_CheckedChanged" />
            <label class="form-check-label"><h5 class="d-inline">Criar Contacto</h5></label>
        </div>

        <asp:PlaceHolder ID="phContacto" runat="server">
            <div class="row g-3">
                <div class="col-md-4">
                    <label class="form-label">Nome *</label>
                    <asp:TextBox ID="txtContactoNome" runat="server" CssClass="form-control" MaxLength="120" />
                </div>
                <div class="col-md-4">
                    <label class="form-label">Cargo</label>
                    <asp:TextBox ID="txtContactoCargo" runat="server" CssClass="form-control" MaxLength="100" />
                </div>
                <div class="col-md-4">
                    <label class="form-label">Email</label>
                    <asp:TextBox ID="txtContactoEmail" runat="server" CssClass="form-control" MaxLength="150" TextMode="Email" />
                </div>
                <div class="col-md-4">
                    <label class="form-label">Telefone</label>
                    <asp:TextBox ID="txtContactoTelefone" runat="server" CssClass="form-control" MaxLength="30" />
                </div>
            </div>
        </asp:PlaceHolder>
    </div>

    <!-- Oportunidade -->
    <div class="crm-form-card mb-3">
        <div class="form-check mb-3">
            <asp:CheckBox ID="chkCriarOportunidade" runat="server" CssClass="form-check-input" Checked="true"
                AutoPostBack="true" OnCheckedChanged="chkCriarOportunidade_CheckedChanged" />
            <label class="form-check-label"><h5 class="d-inline">Criar Oportunidade</h5></label>
        </div>

        <asp:PlaceHolder ID="phOportunidade" runat="server">
            <div class="row g-3">
                <div class="col-md-6">
                    <label class="form-label">Título *</label>
                    <asp:TextBox ID="txtOportunidadeTitulo" runat="server" CssClass="form-control" MaxLength="180" />
                </div>
                <div class="col-md-3">
                    <label class="form-label">Fase Inicial *</label>
                    <asp:DropDownList ID="ddlFaseInicial" runat="server" CssClass="form-select" />
                </div>
                <div class="col-md-3">
                    <label class="form-label">Comercial Responsável *</label>
                    <asp:DropDownList ID="ddlComercialOportunidade" runat="server" CssClass="form-select" />
                </div>
                <div class="col-md-4">
                    <label class="form-label">Valor Estimado (€) *</label>
                    <asp:TextBox ID="txtValorEstimado" runat="server" CssClass="form-control" TextMode="Number" step="0.01" />
                </div>
                <div class="col-md-4">
                    <label class="form-label">Data Prevista de Fecho *</label>
                    <asp:TextBox ID="txtDataFecho" runat="server" CssClass="form-control" TextMode="Date" />
                </div>
            </div>
        </asp:PlaceHolder>
    </div>

    <div class="d-flex gap-2">
        <asp:Button ID="btnConverter" runat="server" Text="Converter" CssClass="btn btn-success" OnClick="btnConverter_Click"
            data-confirm="Converter este lead? Esta ação não pode ser desfeita." />
        <asp:HyperLink ID="lnkCancelar" runat="server" CssClass="btn btn-outline-secondary">Cancelar</asp:HyperLink>
    </div>

</asp:Content>