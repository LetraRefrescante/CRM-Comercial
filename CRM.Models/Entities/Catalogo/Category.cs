using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Models.Entities.Catalogo
{
    [Table("Categories")]
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
    }
}