using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class ClinicianAppTypesCustomModel : ClinicianAppointmentType
    {
        public string ClinicianName { get; set; }
        public string AppointmentType { get; set; }
        public string Facility { get; set; }

        public IEnumerable<DropdownListData> AppointmentTypes { get; set; } = new List<DropdownListData>();

        public List<DropdownListData> Physicians { get; set; }
    }
}
