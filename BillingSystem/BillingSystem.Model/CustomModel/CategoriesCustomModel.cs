using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class CategoriesCustomModel
    {
        public int Id { get; set; }
        public string ProdCatNumber { get; set; }
        public string ProdCat { get; set; }
        public string ProdSubcat { get; set; }
        public string ProdSubcat2 { get; set; }
        public string ProdSubcat3 { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}
