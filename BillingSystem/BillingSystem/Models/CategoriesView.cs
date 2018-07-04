using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class CategoriesView
    {
     
        public Categories CurrentCategory { get; set; }
        public List<CategoriesCustomModel> CategoriesList { get; set; }

    }
}
