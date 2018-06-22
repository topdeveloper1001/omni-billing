﻿using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using System.Collections.Generic;

namespace BillingSystem.Models
{
    public class GlobalCodeCategoryView
    {
        public List<GlobalCodeCategoryCustomModel> GlobalCodeCategoryList { get; set; }
        public List<Facility> FacilityList { get; set; }
        public List<GlobalCodeCategory> SelectedSourceGlobalCodeCategoryOptions { get; set; }
        public List<GlobalCodeCategory> SelectedGlobalCodeCategoryOptions { get; set; }
        public string FacilityNumber { get; set; }



        public GlobalCodeCategory CurrentGlobalCodeCategory { get; set; }
        public List<GlobalCodeCategory> GCCategoryList { get; set; }
        public string CategoryValue { get; set; }
    }
}