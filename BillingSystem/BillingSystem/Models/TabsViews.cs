using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class TabsView
    {
        public Tabs CurrentTabs { get; set; }
        public List<TabsCustomModel> TabsList { get; set; }
    }
}
