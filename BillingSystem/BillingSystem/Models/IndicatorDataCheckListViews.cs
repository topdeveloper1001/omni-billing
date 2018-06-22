using System.Collections.Generic;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class IndicatorDataCheckListView
    {
     
        public IndicatorDataCheckList CurrentIndicatorDataCheckList { get; set; }
        public List<IndicatorDataCheckListCustomModel> IndicatorDataCheckListList { get; set; }
        public string BudgetType { get; set; }
        public string Year { get; set; }
        public List<GlobalCodes> DdYearList { get; set; }
        public List<GlobalCodes> DdMonthList { get; set; }
        public string Month { get; set; }
    }
}
