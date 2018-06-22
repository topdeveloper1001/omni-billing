using System;

namespace BillingSystem.Model.CustomModel
{
    public class BillingModifierCustomModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }

        public bool IsFirst { get; set; }

        public DateTime? EffectiveStartDate { get; set; }
        public DateTime? EffectiveEndDate { get; set; }

        public string ExtValue1 { get; set; }

        public bool IsActive { get; set; }

        public long FacilityId { get; set; }
        public long CorporateId { get; set; }

        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
