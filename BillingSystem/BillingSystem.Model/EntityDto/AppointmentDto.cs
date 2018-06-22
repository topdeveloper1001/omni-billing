using System;

namespace BillingSystem.Model.EntityDto
{
    public class AppointmentDto
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public string AppointmentDetails { get; set; }

        public string AppointmentTypeId { get; set; }

        public long ClinicianId { get; set; }

        public long Specialty { get; set; }

        public long PatientId { get; set; }

        public long? ClinicianReferredBy { get; set; }

        public string Status { get; set; }

        public DateTime ScheduleDate { get; set; }

        public string TimeFrom { get; set; }

        public string TimeTill { get; set; }

        //public string Comments { get; set; }

        public long? FacilityId { get; set; }

        public bool IsAddedToMain { get; set; }

        //public long UserId { get; set; }

        public long CountryId { get; set; }

        public long StateId { get; set; }

        public long CityId { get; set; }
    }
}
