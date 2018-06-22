using System.Collections.Generic;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class DrugView
    {
        public Drug CurrentDrug { get; set; }
        public List<Drug> DrugList { get; set; }
        public int UserId { get; set; }
    }
}
