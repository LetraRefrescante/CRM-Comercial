using System;

namespace CRM.Data.Helpers
{
    public class AplicacaoException : Exception
    {
        public AplicacaoException(string message) : base(message) { }
        public AplicacaoException(string message, Exception innerException) : base(message, innerException) { }
    }
}