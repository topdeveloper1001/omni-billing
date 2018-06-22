using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class PreSchedulingLinkView
    {
     
        public PreSchedulingLink CurrentPreSchedulingLink { get; set; }
        public List<PreSchedulingLinkCustomModel> PreSchedulingLinkList { get; set; }

    }
}
