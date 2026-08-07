<%@ Page Title="Importar Clientes" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="ClientesImportar.aspx.cs" Inherits="CRM.Web.Paginas.Clientes.ClientesImportar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item"><a href="~/Clientes/ClienteLista.aspx" runat="server">Clientes</a></li>
    <li class="breadcrumb-item active">Importar</li>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2 class="mb-4">Importar Clientes</h2>

    <div class="card p-4 mb-3">
        <h5>Formato do ficheiro CSV</h5>
        <p class="text-muted mb-2">
            Ficheiro de texto separado por ponto e vírgula (;), codificação UTF-8, com linha de cabeçalho.
            Colunas, por ordem:
        </p>
        <p class="mono small text-muted">
            NomeComercial;NomeLegal;NIF;Email;Telefone;Morada;CodigoPostal;Cidade;PaisISO;Setor;EmailComercial;Estado;Observacoes
        </p>
        <ul class="text-muted small">
            <li><strong>NomeComercial</strong> e <strong>NIF</strong> são obrigatórios.</li>
            <li><strong>PaisISO</strong> tem de corresponder ao código de um país ativo (ex.: PT, ES).</li>
            <li><strong>EmailComercial</strong> tem de corresponder a um utilizador ativo com perfil Comercial.</li>
            <li><strong>Estado</strong> aceita Potencial, Ativo, Inativo ou Bloqueado; inválido ou vazio assume "Potencial".</li>
            <li>As restantes colunas são opcionais e podem ficar em branco.</li>
        </ul>
    </div>

    <div class="card p-4" style="max-width: 640px;">
        <label class="form-label">Ficheiro CSV</label>
        <asp:FileUpload ID="fileImportar" runat="server" CssClass="form-control mb-3" />
        <asp:Button ID="btnImportar" runat="server" Text="Importar" CssClass="btn btn-primary" OnClick="btnImportar_Click" />
    </div>

    <asp:PlaceHolder ID="phResumo" runat="server" Visible="false">
        <div class="card p-4 mt-3">
            <p><asp:Literal ID="litResumo" runat="server" /></p>

            <asp:PlaceHolder ID="phErros" runat="server" Visible="false">
                <h6>Linhas com erro</h6>
                <asp:Repeater ID="rptErros" runat="server">
                    <HeaderTemplate><ul class="text-danger small"></HeaderTemplate>
                    <ItemTemplate><li><%# Container.DataItem %></li></ItemTemplate>
                    <FooterTemplate></ul></FooterTemplate>
                </asp:Repeater>
            </asp:PlaceHolder>
        </div>
    </asp:PlaceHolder>

</asp:Content>