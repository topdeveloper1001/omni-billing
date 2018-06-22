using System;

namespace BillingSystem.Model
{
    public class ClinicianAppointmentType : BaseEntity<long>
    {
        public long ClinicianId { get; set; }

        public long AppointmentTypeId { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public bool IsDeleted { get; set; }
    }
}
