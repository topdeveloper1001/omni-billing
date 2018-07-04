using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class CatalogView
    {
     
        public Catalog CurrentCatalog { get; set; }
        public List<CatalogCustomModel> CatalogList { get; set; }

    }
}
