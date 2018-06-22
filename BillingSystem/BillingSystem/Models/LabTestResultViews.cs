using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class LabTestResultView
    {
     
        public LabTestResult CurrentLabTestResult { get; set; }
        public List<LabTestResultCustomModel> LabTestResultList { get; set; }

    }
}
