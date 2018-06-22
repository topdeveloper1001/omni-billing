using System.Collections.Generic;

namespace BillingSystem.Model.EntityDto
{
    public class AppointmentDetailsDto
    {
        public IEnumerable<SpecialityDto> Specialities { get; set; }
        public IEnumerable<ClinicianDetailDto> Clinicians { get; set; }
    }
}
