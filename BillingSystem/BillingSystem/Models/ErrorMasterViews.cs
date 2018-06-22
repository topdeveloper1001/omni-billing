using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class ErrorMasterView
    {
     
        public ErrorMaster CurrentErrorMaster { get; set; }
        public List<ErrorMasterCustomModel> ErrorMasterList { get; set; }

    }
}
