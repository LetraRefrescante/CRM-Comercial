using System;
using System.Web;
using System.Web.Script.Serialization;
using CRM.Services;

namespace CRM.Web.Oportunidades
{
    public class MudarFaseHandler : IHttpHandler, System.Web.SessionState.IReadOnlySessionState
    {
        private readonly OpportunityService _opportunityService = new OpportunityService();

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            if (context.Session["UserId"] == null)
            {
                context.Response.StatusCode = 401;
                EscreverResposta(context, false, "Sessão expirada.");
                return;
            }

            string perfil = context.Session["RoleName"] as string ?? "";
            if (!_opportunityService.PodeEditar(perfil))
            {
                context.Response.StatusCode = 403;
                EscreverResposta(context, false, "Sem permissão para mover oportunidades.");
                return;
            }

            if (!int.TryParse(context.Request.Form["opportunityId"], out int opportunityId) ||
                !int.TryParse(context.Request.Form["novaFaseId"], out int novaFaseId))
            {
                context.Response.StatusCode = 400;
                EscreverResposta(context, false, "Pedido inválido.");
                return;
            }

            int userId = (int)context.Session["UserId"];

            try
            {
                string erro = _opportunityService.MudarFase(opportunityId, novaFaseId, perfil, userId);
                if (erro != null)
                {
                    context.Response.StatusCode = 400;
                    EscreverResposta(context, false, erro);
                    return;
                }
                EscreverResposta(context, true, null);
            }
            catch (Exception)
            {
                context.Response.StatusCode = 500;
                EscreverResposta(context, false, "Não foi possível mover a oportunidade.");
            }
        }

        private void EscreverResposta(HttpContext context, bool sucesso, string mensagem)
        {
            var serializer = new JavaScriptSerializer();
            context.Response.Write(serializer.Serialize(new { sucesso, mensagem }));
        }

        public bool IsReusable => false;
    }
}