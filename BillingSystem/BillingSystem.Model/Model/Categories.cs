using System;
using System.ComponentModel.DataAnnotations;

namespace BillingSystem.Model
{
    public class Categories
    {
        [Key]
        public int Id { get; set; }

        public string ProdCatNumber { get; set; }

        public string ProdCat { get; set; }

        public string ProdSubcat { get; set; }

        public string ProdSubcat2 { get; set; }

        public string ProdSubcat3 { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

    }
}
