using System.Collections.Generic;
using BillingSystem.Model.CustomModel;


namespace BillingSystem.Models
{
    public class ClinicianAppointmentTypesView
    {
        public IEnumerable<ClinicianAppTypesCustomModel> List { get; set; }
        public ClinicianAppTypesCustomModel CurrentClinician { get; set; }
    }
}