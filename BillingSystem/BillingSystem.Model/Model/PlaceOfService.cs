using BillingSystem.Model.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;

namespace BillingSystem.Model
{
    public class PlaceOfService : BaseEntity<long>, IEntityUpdatable, IEntityCreatable
    {
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(10)]
        public string Code { get; set; }


        [MaxLength(1000)]
        public string Description { get; set; }

        public DateTime? EffectiveStartDate { get; set; }
        public DateTime? EffectiveEndDate { get; set; }

        [MaxLength(50)]
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
