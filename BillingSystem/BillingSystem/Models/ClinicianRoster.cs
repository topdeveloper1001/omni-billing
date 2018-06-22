using BillingSystem.Model;
using System;

namespace BillingSystem.Model
{
    public class ClinicianRoster : BaseEntity<long>
    {
        public long ClinicianId { get; set; }

        public string ReasonId { get; set; }

        public string Comments { get; set; }

        public string RosterTypeId { get; set; }

        public DateTime DateFrom { get; set; }

        public string TimeFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public string TimeTo { get; set; }

        public long FacilityId { get; set; }

        public long? CorporateId { get; set; }

        public string RepeatitiveDaysInWeek { get; set; }

        public bool IsActive { get; set; }

        public long CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public long? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string ExtValue1 { get; set; }

        public string ExtValue2 { get; set; }

    }
}