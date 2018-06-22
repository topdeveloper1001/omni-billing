using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class MissingDataView
    {
        public List<MissingDataCustomModel> MissingDataList { get; set; }
        public List<BillHeaderCustomModel> BillReadyForScrub { get; set; }
    }
}