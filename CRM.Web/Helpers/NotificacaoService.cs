using System.Collections.Generic;
using System.Web;

namespace CRM.Web.Helpers
{
    public enum TipoMensagem
    {
        Sucesso,
        Erro,
        Aviso,
        Info
    }

    public class MensagemToast
    {
        public string Texto { get; set; }
        public string Tipo { get; set; }
        public string Icone { get; set; }
        public int Duracao { get; set; } = 4000;
    }

    public static class NotificacaoService
    {
        private const string SessionKey = "__ToastMensagens";

        public static void Sucesso(string texto, int duracaoMs = 4000) => Adicionar(texto, TipoMensagem.Sucesso, duracaoMs);
        public static void Erro(string texto, int duracaoMs = 6000) => Adicionar(texto, TipoMensagem.Erro, duracaoMs);
        public static void Aviso(string texto, int duracaoMs = 5000) => Adicionar(texto, TipoMensagem.Aviso, duracaoMs);
        public static void Info(string texto, int duracaoMs = 4000) => Adicionar(texto, TipoMensagem.Info, duracaoMs);

        private static void Adicionar(string texto, TipoMensagem tipo, int duracaoMs)
        {
            string classeBootstrap;
            string icone;

            switch (tipo)
            {
                case TipoMensagem.Sucesso:
                    classeBootstrap = "success";
                    icone = "fas fa-circle-check";
                    break;
                case TipoMensagem.Erro:
                    classeBootstrap = "danger";
                    icone = "fas fa-circle-exclamation";
                    break;
                case TipoMensagem.Aviso:
                    classeBootstrap = "warning";
                    icone = "fas fa-triangle-exclamation";
                    break;
                default:
                    classeBootstrap = "info";
                    icone = "fas fa-circle-info";
                    break;
            }

            var lista = ObterLista();
            lista.Add(new MensagemToast { Texto = texto, Tipo = classeBootstrap, Icone = icone, Duracao = duracaoMs });
            HttpContext.Current.Session[SessionKey] = lista;
        }

        private static List<MensagemToast> ObterLista()
        {
            return HttpContext.Current.Session[SessionKey] as List<MensagemToast> ?? new List<MensagemToast>();
        }

        public static List<MensagemToast> ObterELimpar()
        {
            var lista = ObterLista();
            HttpContext.Current.Session.Remove(SessionKey);
            return lista;
        }
    }
}