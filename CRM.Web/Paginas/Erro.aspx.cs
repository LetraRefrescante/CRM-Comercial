using System;
using System.Web.UI;

namespace CRM.Web
{
    public partial class Erro : Page
    {
        public string TituloErro { get; private set; }
        public string MensagemErro { get; private set; }
        public string IdOcorrencia { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            IdOcorrencia = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();

            string codigo = Request.QueryString["codigo"] ?? "500";

            switch (codigo)
            {
                case "404":
                    TituloErro = "Página não encontrada";
                    MensagemErro = "O recurso que procuras não existe ou foi movido.";
                    break;
                default:
                    TituloErro = "Ocorreu um erro inesperado";
                    MensagemErro = "A equipa técnica já foi notificada. Podes tentar novamente mais tarde.";
                    break;
            }

            // Aqui pode registar-se o IdOcorrencia + Server.GetLastError() num log/AuditLog
        }
    }
}