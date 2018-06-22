using System;
using System.ComponentModel.DataAnnotations;

namespace BillingSystem.Model
{
    public class AppointmentTypes
    {
        [Key]
        public int Id { get; set; }

        public string Description { get; set; }

        public string CategoryNumber { get; set; }

        public string CptRangeFrom { get; set; }

        public string CptRangeTo { get; set; }

        public string DefaultTime { get; set; }

        public int? CorporateId { get; set; }

        public int? FacilityId { get; set; }

        public string ExtValue1 { get; set; }

        public string ExtValue2 { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CraetedDate { get; set; }

        public bool IsActive { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string Name { get; set; }
    }
}
