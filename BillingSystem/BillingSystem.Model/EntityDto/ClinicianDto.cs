using System.Collections.Generic;

namespace BillingSystem.Model.EntityDto
{
    public class ClinicianDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<TimeSlotsDto> TimeSlots { get; set; }
    }
}
