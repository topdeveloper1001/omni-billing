using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class CPTCodesView
    {
        public List<CPTCodes> CPTCodesList { get; set; }
        public CPTCodes CurrentCPTCode { get; set; }
        public List<CPTCodesCustomModel> CPTCodesCustomList { get; set; }
        public CPTCodesCustomModel CurrentCPTCodeCustom { get; set; }
        public int UserId { get; set; }
    }
}