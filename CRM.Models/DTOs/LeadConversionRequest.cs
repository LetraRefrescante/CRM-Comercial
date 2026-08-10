using System;

namespace CRM.Models.DTOs
{
    public class LeadConversionRequest
    {
        public int LeadId { get; set; }
        public int UserId { get; set; }
        public int? ClienteExistenteId { get; set; }

        public string NovoClienteNif { get; set; }
        public string NovoClienteNomeComercial { get; set; }
        public string NovoClienteNomeLegal { get; set; }
        public string NovoClienteEmail { get; set; }
        public string NovoClienteTelefone { get; set; }
        public int? NovoClienteCountryId { get; set; }
        public int? NovoClienteSectorId { get; set; }
        public int NovoClienteAccountManagerId { get; set; }

        public bool CriarContacto { get; set; }
        public string ContactoNome { get; set; }
        public string ContactoCargo { get; set; }
        public string ContactoEmail { get; set; }
        public string ContactoTelefone { get; set; }

        public bool CriarOportunidade { get; set; }
        public string OportunidadeTitulo { get; set; }
        public int OportunidadeStageId { get; set; }
        public decimal OportunidadeValorEstimado { get; set; }
        public DateTime OportunidadeDataFechoPrevista { get; set; }
        public int OportunidadeOwnerId { get; set; }
    }
}