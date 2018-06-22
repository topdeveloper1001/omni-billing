using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using System.Collections.Generic;

namespace BillingSystem.Models
{
    public class LabTestOrderSetView
    {
        public GlobalCodeCategory CurrentGlobalCodeCategory1 { get; set; }
        public List<GlobalCodeCategory> GCCategoryList1 { get; set; }
        public string CategoryValue { get; set; }

        public GlobalCodeModel CurrentGlobalCodeCategory { get; set; }
        public List<GlobalCodeModel> GCCategoryList { get; set; }

        public GlobalCodes CurrentLabOrderCode { get; set; }
        public List<GlobalCodes> LabOrderCodesList { get; set; }
    }
}