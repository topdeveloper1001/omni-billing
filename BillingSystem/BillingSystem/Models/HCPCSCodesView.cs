using System.Collections.Generic;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class HCPCSCodesView
    {
        //public List<HCPCSCodesViewModel> HCPCSCodesList { get; set; }
        //public HCPCSCodesViewModel CurrentHCPCSCodes { get; set; }
        //public HCPCSCodesViewModel HCPCSCodesViewModel { get; set; }


        public List<HCPCSCodes> HCPCSCodesList { get; set; }
        public HCPCSCodes CurrentHCPCSCodes { get; set; }
        public int UserId { get; set; }
       // public HCPCSCodesViewModel HCPCSCodesViewModel { get; set; }
    }
}