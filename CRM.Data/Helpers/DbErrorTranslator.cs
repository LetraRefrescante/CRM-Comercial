using System;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace CRM.Data.Helpers
{
    public static class DbErrorTranslator
    {
        public static string Traduzir(Exception ex)
        {
            var sqlEx = ExtrairSqlException(ex);

            if (sqlEx == null)
                return "Ocorreu um erro inesperado ao guardar os dados. Tenta novamente.";

            switch (sqlEx.Number)
            {
                case 2601:
                case 2627:
                    return TraduzirPorNomeConstraint(sqlEx.Message, "Já existe um registo com estes dados.");
                case 547:
                    return TraduzirPorNomeConstraint(sqlEx.Message, "Os dados introduzidos não cumprem as regras exigidas.");

                default:
                    return "Ocorreu um erro ao guardar os dados. Verifica a informação introduzida.";
            }
        }

        private static string TraduzirPorNomeConstraint(string mensagemSql, string mensagemGenerica)
        {
            if (mensagemSql.Contains("CK_Clients_CommercialName_Length"))
                return "O Nome Comercial tem de ter pelo menos 2 caracteres.";

            if (mensagemSql.Contains("UX_Clients_VatNumber"))
                return "Já existe um cliente ativo com este NIF.";

            if (mensagemSql.Contains("CK_Contacts_BirthDate"))
                return "A data de nascimento não pode ser uma data futura.";

            if (mensagemSql.Contains("CK_Contacts_Preference"))
                return "A preferência de contacto indicada não é válida.";

            if (mensagemSql.Contains("UX_EmailTemplates_Name"))
                return "Já existe um template de email com este nome.";

            return mensagemGenerica;
        }

        private static SqlException ExtrairSqlException(Exception ex)
        {
            while (ex != null)
            {
                if (ex is SqlException sqlEx) return sqlEx;
                ex = ex.InnerException;
            }
            return null;
        }
    }
}