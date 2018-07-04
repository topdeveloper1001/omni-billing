using System;

namespace BillingSystem.Model.CustomModel
{
    public class DrugAllergyLogCustomModel
    {
        public int Id { get; set; }
        public string OrderCode { get; set; }
        public string OrderCategory { get; set; }
        public string OrderSubCategory { get; set; }
        public string OrderType { get; set; }
        public string OrderName { get; set; }
        public string AllergyType { get; set; }
        public string ReactionType { get; set; }
        public int? PatientId { get; set; }
        public int? EncounterId { get; set; }
        public string ReactionOrderCode { get; set; }
        public string AllergyFromName { get; set; }
        public bool? IsOrderCancel { get; set; }
        public int? OrderBy { get; set; }
        public DateTime? OrderStartDate { get; set; }
        public DateTime? OrderEndDate { get; set; }
        public DateTime? OrderedDate { get; set; }
        public int? CorporateId { get; set; }
        public int? FacilityId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
