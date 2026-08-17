using System;
using System.IO;
using System.Web;
using CRM.Services;
using CRM.Web.Helpers;

namespace CRM.Web.Paginas.Documentos
{
    public partial class DocumentoDownload : PaginaBase
    {
        private readonly DocumentService _documentService = new DocumentService();
        private const string PastaArmazenamento = "~/App_Data/DocumentUploads/";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!int.TryParse(Request.QueryString["id"], out int documentId))
            {
                Response.Redirect("~/AcessoNegado.aspx");
                return;
            }

            var document = _documentService.GetById(documentId);
            if (document == null || !_documentService.PodeAceder(document, UserId, Perfil))
            {
                Response.Redirect("~/AcessoNegado.aspx");
                return;
            }

            string caminhoFisico = Server.MapPath(PastaArmazenamento + document.StoredFileName);
            if (!File.Exists(caminhoFisico))
            {
                NotificacaoService.Erro("O ficheiro já não está disponível no servidor.");
                Response.Redirect("~/Documentos/DocumentosLista.aspx");
                return;
            }

            _documentService.RegistarDownload(documentId, UserId, Request.UserHostAddress);

            Response.Clear();
            Response.ContentType = string.IsNullOrEmpty(document.MimeType) ? "application/octet-stream" : document.MimeType;
            Response.AppendHeader("Content-Disposition", "attachment; filename=\"" + document.OriginalFileName + "\"");
            Response.TransmitFile(caminhoFisico);

            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
    }
}