using System.Collections.Generic;

using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class DRGCodesView
    {
        public List<DRGCodes> DRGCodesList { get; set; }
        public DRGCodes CurrentDRGCodes { get; set; }
        public int UserId { get; set; }
    }
}