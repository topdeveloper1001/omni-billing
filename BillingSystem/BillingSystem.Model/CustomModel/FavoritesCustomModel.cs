using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    public class FavoritesCustomModel
    {
        public int UserDefinedDescriptionID { get; set; }
        public string CategoryId { get; set; }
        public string CodeId { get; set; }
        public string RoleID { get; set; }
        public int? UserID { get; set; }
        public string UserDefineDescription { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? Modifieddate { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string RoleName { get; set; }
        public string UserName { get; set; }
        public string CategoryName { get; set; }
        public string CodeDesc { get; set; }
        public string CodeValue { get; set; }
    }
}
