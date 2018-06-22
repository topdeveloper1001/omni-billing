using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class AppointmentTypesView
    {
     
        public AppointmentTypes CurrentAppointmentTypes { get; set; }
        public List<AppointmentTypesCustomModel> AppointmentTypesList { get; set; }

    }
}
