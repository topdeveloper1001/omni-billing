using System;

namespace BillingSystem.Model.EntityDto
{
    public class TimeSlotsDto
    {
        public int Id { get; set; }
        public string TimeSlot { get; set; }
        public int PhysicianId { get; set; }
        public int DeptId { get; set; }
        public string DeptName { get; set; }
        public string Clinician { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Location { get; set; }
        public long SpecialtyId { get; set; }
        public string Specialty { get; set; }
        public long FacilityId { get; set; }
        public string FacilityName { get; set; }
    }
}
