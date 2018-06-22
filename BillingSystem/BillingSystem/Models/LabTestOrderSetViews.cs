using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class LabTestOrderSetViews
    {
     
        public LabTestOrderSet CurrentLabTestOrderSet { get; set; }
        public List<LabTestOrderSetCustomModel> LabTestOrderSetList { get; set; }

    }
}
