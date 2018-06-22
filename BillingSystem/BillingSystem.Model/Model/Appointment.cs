using BillingSystem.Model.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;

namespace BillingSystem.Model.Model
{
    public class Appointment : ICommonEntity<long>
    {
        [Key]
        public long Id { get; set; }

        public string Title { get; set; }

        public string AppointmentDetails { get; set; }

        public string AppointmentTypeId { get; set; }

        public long ClinicianId { get; set; }

        public long Specialty { get; set; }

        public char PatientId { get; set; }

        public long? ClinicianReferredBy { get; set; }

        public string Status { get; set; }

        public DateTime ScheduleDate { get; set; }

        public string TimeFrom { get; set; }

        public string TimeTill { get; set; }

        public string Comments { get; set; }

        public string Address { get; set; }

        public long CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public long? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public bool IsActive { get; set; }

        public long? DeletedBy { get; set; }

        public DateTime? DeletedDate { get; set; }

        public bool IsRecurring { get; set; }

        public string RecurringInterval { get; set; }

        public string ExtValue1 { get; set; }

        public string ExtValue2 { get; set; }
    }
}
