using System.Collections.Generic;

namespace BillingSystem.Model.EntityDto
{
    public class ClinicianDetailDto
    {
        public IEnumerable<AppointmentTypeDto> AppointmentTypes { get; set; }
        public string ClinicianName { get; set; }
        public long ClinicianId { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public int CountryId { get; set; }
        public int StateId { get; set; }
        public int CityId { get; set; }
        public long SpecialtyId { get; set; }
        public string SpecialtyName { get; set; }
        public long FacilityId { get; set; }
        public string FacilityName { get; set; }
    }
}
