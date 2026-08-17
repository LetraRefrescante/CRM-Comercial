using System;
using System.Web.UI.WebControls;
using CRM.Models.Entities.Documentos;
using CRM.Models.Filtros;
using CRM.Services;
using CRM.Web.Helpers;

namespace CRM.Web.Paginas.Documentos
{
    public partial class DocumentosLista : PaginaBase
    {
        private readonly DocumentService _documentService = new DocumentService();

        private string SortColumn
        {
            get => ViewState["SortColumn"] as string ?? "CreatedDate";
            set => ViewState["SortColumn"] = value;
        }

        private bool SortAscending
        {
            get => ViewState["SortAscending"] as bool? ?? false;
            set => ViewState["SortAscending"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!_documentService.PodeAcederListaGlobal(Perfil))
            {
                NotificacaoService.Erro("Não tens permissão para aceder à pesquisa global de documentos.");
                Response.Redirect("~/Dashboard/Dashboard.aspx");
                return;
            }

            if (!IsPostBack)
                CarregarDocumentos();
        }

        private void CarregarDocumentos()
        {
            var filtro = new DocumentFiltro
            {
                Pesquisa = txtPesquisa.Text.Trim(),
                Category = ddlCategoria.SelectedValue,
                EntityType = ddlTipoEntidade.SelectedValue,
                IsConfidential = ddlConfidencial.SelectedValue == "Sim" ? true
                    : ddlConfidencial.SelectedValue == "Nao" ? false : (bool?)null,
                DataInicio = ucFiltroDatas.DataInicial,
                DataFim = ucFiltroDatas.DataFinal
            };

            var documentos = _documentService.Pesquisar(
                filtro, ucPaginacao.PaginaAtual, ucPaginacao.TamanhoPagina, out int total,
                SortColumn, SortAscending);

            ucPaginacao.TotalRegistos = total;

            rptDocumentos.DataSource = documentos;
            rptDocumentos.DataBind();

            phVazio.Visible = documentos.Count == 0;
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            ucPaginacao.PaginaAtual = 1;
            CarregarDocumentos();
        }

        protected void lnkOrdenar_Command(object sender, CommandEventArgs e)
        {
            string coluna = e.CommandArgument.ToString();
            if (SortColumn == coluna) SortAscending = !SortAscending;
            else { SortColumn = coluna; SortAscending = true; }

            ucPaginacao.PaginaAtual = 1;
            CarregarDocumentos();
        }

        protected void ucPaginacao_PaginaAlterada(object sender, EventArgs e) => CarregarDocumentos();

        protected void rptDocumentos_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Eliminar") return;

            int documentId = int.Parse(e.CommandArgument.ToString());
            _documentService.Eliminar(documentId, UserId, Request.UserHostAddress);

            NotificacaoService.Sucesso("Documento eliminado.");
            CarregarDocumentos();
        }

        protected string GetRelacionadoTexto(object dataItem) =>
            _documentService.ObterDescricaoRelacionado((Document)dataItem);

        protected string GetTamanhoFormatado(object bytesObj)
        {
            long bytes = (long)bytesObj;
            if (bytes >= 1024 * 1024) return $"{bytes / (1024.0 * 1024.0):0.0} MB";
            if (bytes >= 1024) return $"{bytes / 1024.0:0.0} KB";
            return $"{bytes} B";
        }
    }
}