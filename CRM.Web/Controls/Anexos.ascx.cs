using System;
using System.IO;
using CRM.Services;

namespace CRM.Web.Controls
{
    public partial class Anexos : System.Web.UI.UserControl
    {
        private readonly DocumentService _documentService = new DocumentService();

        private string EntityType
        {
            get => ViewState["EntityType"] as string;
            set => ViewState["EntityType"] = value;
        }
        private int EntityId
        {
            get => ViewState["EntityId"] as int? ?? 0;
            set => ViewState["EntityId"] = value;
        }
        private int UserId
        {
            get => ViewState["UserId"] as int? ?? 0;
            set => ViewState["UserId"] = value;
        }

        // O Load deste UserControl corre antes do Page_Load da página que o
        // contém — a página chama Inicializar(...) assim que souber o EntityId.
        public void Inicializar(string entityType, int entityId, int userId)
        {
            EntityType = entityType;
            EntityId = entityId;
            UserId = userId;
            if (!Page.IsPostBack) CarregarAnexos();
        }

        private void CarregarAnexos()
        {
            var anexos = _documentService.Listar(EntityType, EntityId);
            rptAnexos.DataSource = anexos;
            rptAnexos.DataBind();
            phVazio.Visible = anexos.Count == 0;
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            lblErro.Visible = false;

            if (string.IsNullOrEmpty(EntityType) || EntityId == 0)
            {
                lblErro.Text = "Não é possível anexar ficheiros neste momento.";
                lblErro.Visible = true;
                return;
            }
            if (!fuAnexo.HasFile)
            {
                lblErro.Text = "Seleciona um ficheiro antes de carregar.";
                lblErro.Visible = true;
                return;
            }
            if (!_documentService.ExtensaoPermitida(fuAnexo.FileName))
            {
                lblErro.Text = "Tipo de ficheiro não permitido.";
                lblErro.Visible = true;
                return;
            }
            if (!_documentService.TamanhoPermitido(fuAnexo.PostedFile.ContentLength))
            {
                lblErro.Text = "O ficheiro excede o tamanho máximo permitido (10 MB).";
                lblErro.Visible = true;
                return;
            }

            string nomeArmazenado = Guid.NewGuid().ToString("N") + Path.GetExtension(fuAnexo.FileName);
            string pasta = Server.MapPath("~/App_Data/Documentos");
            if (!Directory.Exists(pasta)) Directory.CreateDirectory(pasta);
            fuAnexo.SaveAs(Path.Combine(pasta, nomeArmazenado));

            _documentService.Guardar(
                entityType: EntityType, entityId: EntityId, category: ddlCategoria.SelectedValue,
                storedFileName: nomeArmazenado, originalFileName: fuAnexo.FileName,
                mimeType: fuAnexo.PostedFile.ContentType, fileSizeBytes: fuAnexo.PostedFile.ContentLength,
                isConfidential: chkConfidencial.Checked, userId: UserId, ip: Request.UserHostAddress);

            CarregarAnexos();
        }

        protected void rptAnexos_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            int documentId = int.Parse(e.CommandArgument.ToString());

            if (e.CommandName == "Eliminar")
            {
                _documentService.Eliminar(documentId, UserId, Request.UserHostAddress);
                CarregarAnexos();
            }
            else if (e.CommandName == "Descarregar")
            {
                var documento = _documentService.GetById(documentId);
                if (documento == null) return;
                string caminho = Server.MapPath("~/App_Data/Documentos/" + documento.StoredFileName);
                if (!File.Exists(caminho)) return;

                _documentService.RegistarDownload(documentId, UserId, Request.UserHostAddress);
                Response.Clear();
                Response.ContentType = string.IsNullOrEmpty(documento.MimeType) ? "application/octet-stream" : documento.MimeType;
                Response.AddHeader("Content-Disposition", $"attachment; filename=\"{documento.OriginalFileName}\"");
                Response.TransmitFile(caminho);
                Response.End();
            }
        }
    }
}