using System;
using System.ComponentModel.DataAnnotations;

namespace BillingSystem.Model
{
    public class Categories
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(12)]
        public string ProdCatNumber { get; set; }
        [MaxLength(40)]
        public string ProdCat { get; set; }
        [MaxLength(40)]
        public string ProdSubcat { get; set; }
        [MaxLength(40)]
        public string ProdSubcat2 { get; set; }
        [MaxLength(40)]
        public string ProdSubcat3 { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

    }
}
