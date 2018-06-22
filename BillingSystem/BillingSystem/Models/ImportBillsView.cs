using System.Collections.Generic;
using BillingSystem.Model.CustomModel;


namespace BillingSystem.Models
{
    public class ImportBillsView
    {
        public IEnumerable<TPFileHeaderCustomModel> TpFileHeaderList { get; set; }
    }
}