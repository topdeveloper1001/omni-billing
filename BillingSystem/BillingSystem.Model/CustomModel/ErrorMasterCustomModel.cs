
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class ErrorMasterCustomModel : ErrorMaster
    {
        public bool IsPredefinedDenial { get; set; }
        public string ErrorTypeString { get; set; }
    }
}
