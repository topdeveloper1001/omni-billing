using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class TPXMLParsedDataView
    {
     
        public TPXMLParsedData CurrentTPXMLParsedData { get; set; }
        public List<TPXMLParsedDataCustomModel> TPXMLParsedDataList { get; set; }

    }
}
