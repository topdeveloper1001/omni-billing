using BillingSystem.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class ClinicianRosterCustomModel : ClinicianRoster
    {
        public string Reason { get; set; }
        public string RosterType { get; set; } = "1";
        public string ClinicianName { get; set; }
        public string ClinicianDepartment { get; set; }
        public string FacilityNumber { get; set; }
    }
}
