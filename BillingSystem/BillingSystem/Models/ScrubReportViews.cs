using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class ScrubReportView
    {
        public ScrubReport CurrentScrubReport { get; set; }
        public List<ScrubReportCustomModel> ScrubReportList { get; set; }
        public List<ScrubHeaderCustomModel> ScrubHeaderList { get; set; }
        public List<BillHeader> ListOfBillHeader { get; set; }
        public int BillHeaderId { get; set; }
        public int BillsCount { get; set; }

    }
}
