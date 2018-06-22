using BillingSystem.Model.CustomModel;
using System.Collections.Generic;

namespace BillingSystem.Models
{
    public class ClinicianRosterView
    {
        public ClinicianRosterCustomModel Current { get; set; }
        public IEnumerable<ClinicianRosterCustomModel> List { get; set; }
    }
}