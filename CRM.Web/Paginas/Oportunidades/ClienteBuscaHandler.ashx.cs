using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using CRM.Data.Repositories;
using CRM.Services;

namespace CRM.Web.Oportunidades
{
    // Autocomplete de Clientes para OportunidadeEditar.aspx (pesquisa por nome comercial ou NIF).
    // Só devolve clientes Ativos; se o utilizador tiver âmbito "próprios" (Comercial),
    // restringe aos clientes atribuídos a ele — mesma regra que o ClientService aplica
    // na criação/edição de Clientes.
    public class ClienteBuscaHandler : IHttpHandler, System.Web.SessionState.IReadOnlySessionState
    {
        private readonly ClientRepository _clientRepository = new ClientRepository();
        private readonly OpportunityService _opportunityService = new OpportunityService();

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            if (context.Session["UserId"] == null)
            {
                context.Response.StatusCode = 401;
                context.Response.Write("[]");
                return;
            }

            string perfil = context.Session["RoleName"] as string ?? "";
            if (!_opportunityService.PodeEditar(perfil))
            {
                context.Response.StatusCode = 403;
                context.Response.Write("[]");
                return;
            }

            string termo = (context.Request.QueryString["q"] ?? "").Trim();
            if (termo.Length < 2)
            {
                context.Response.Write("[]");
                return;
            }

            int? accountManagerId = _opportunityService.TemAmbitoProprios(perfil)
                ? (int)context.Session["UserId"]
                : (int?)null;

            var clientes = _clientRepository.Listar(
                termo, "Ativo", accountManagerId,
                1, 10, out int totalRegistos,
                "TradeName", true);

            var resultado = clientes.Select(c => new
            {
                id = c.ClientId,
                nome = c.TradeName,
                nif = c.VatNumber,
                cidade = c.City
            });

            var serializer = new JavaScriptSerializer();
            context.Response.Write(serializer.Serialize(resultado));
        }

        public bool IsReusable => false;
    }
}