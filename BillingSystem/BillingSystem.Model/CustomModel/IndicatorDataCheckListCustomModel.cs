using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class IndicatorDataCheckListCustomModel : IndicatorDataCheckList
    {
        public string FacilityName { get; set; }
        public bool CusM1 { get; set; }
        public bool CusM2 { get; set; }
        public bool CusM3 { get; set; }
        public bool CusM4 { get; set; }
        public bool CusM5 { get; set; }
        public bool CusM6 { get; set; }
        public bool CusM7 { get; set; }
        public bool CusM8 { get; set; }
        public bool CusM9 { get; set; }
        public bool CusM10 { get; set; }
        public bool CusM11 { get; set; }
        public bool CusM12 { get; set; }
        public bool CusMonth { get; set; }
        public int Month { get; set; }
    }
}
