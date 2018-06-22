using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class PlaceOfServiceCustomModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

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
