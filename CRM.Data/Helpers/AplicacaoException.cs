using System;

namespace CRM.Data.Helpers
{
    public class AplicacaoException : Exception
    {
        public AplicacaoException(string mensagem, Exception inner) : base(mensagem, inner) { }
    }
}