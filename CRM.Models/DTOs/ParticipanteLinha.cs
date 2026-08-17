using System;

namespace CRM.Models.DTOs
{
    [Serializable]
    public class ParticipanteLinha
    {
        public int? UserId { get; set; }
        public string NomeExibicao { get; set; }
        public string ExternalName { get; set; }
        public string ExternalEmail { get; set; }

        public bool EhInterno => UserId.HasValue;
    }
}