using System;

namespace BillingSystem.Model.CustomModel
{
    public class DrugInteractionsCustomModel
    {
        public long Id { get; set; }
        public string GreenrainCode { get; set; }
        public string ATCCode { get; set; }
        public string PackageName { get; set; }
        public string GenericName { get; set; }
        public string ReactionCategory { get; set; }
        public string OrderCode { get; set; }
        public string OrderCodeType { get; set; }
        public string OrderName { get; set; }
        public string Warning { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? Modifieddate { get; set; }
        public bool IsDeleted { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool IsActive { get; set; }
        public string ReactionCategoryStr { get; set; }
        public string WarningStr { get; set; }
        public string OrderTypeName { get; set; }
    }
}
