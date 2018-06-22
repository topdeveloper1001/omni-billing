using System;

namespace BillingSystem.Model
{
    public class SchedulingParameters : BaseEntity<long>
    {
        public decimal? StartHour { get; set; }

        public decimal? EndHour { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public bool IsActive { get; set; }

        public long FacilityId { get; set; }

        public long CorporateId { get; set; }
    }
}
