using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class MCRulesTableView
    {
     
        public MCRulesTable CurrentMCRulesTable { get; set; }
        public List<MCRulesTableCustomModel> MCRulesTableList { get; set; }

    }
}
