using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Models.Entities.Seguranca
{
    [Table("PasswordResetTokens")]
    public class PasswordResetToken
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataExpiracao { get; set; }
        public bool Utilizado { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public bool EstaValido()
        {
            return !Utilizado && DateTime.UtcNow <= DataExpiracao;
        }
    }
}