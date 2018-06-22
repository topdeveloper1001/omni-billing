using System.Collections.Generic;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class XMLBillFileView
    {
        public List<TPXMLParsedDataCustomModel> XAdviceXMLData { get; set; }
        public List<TPFileHeaderCustomModel> XMLBillFile { get; set; }
    }
}