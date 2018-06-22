using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillingSystem.Model.CustomModel
{

    [NotMapped]
    public class AgingReportCustomModel
    {
        public Int32 ID { get; set; }
        public string Name { get; set; }
        public string EmirateID { get; set; }
        public decimal OnTime { get; set; }
        public decimal Days1To30 { get; set; }
        public decimal Days31To60 { get; set; }
        public decimal Days61To90 { get; set; }
        public decimal Days91To120 { get; set; }
        public decimal Days121To150 { get; set; }
        public decimal Days151To180 { get; set; }
        public decimal Days181More { get; set; }
        public decimal Total { get; set; }
        public DateTime? EncounterEnd { get; set; }
        public DateTime? DueDate { get; set; }
        public string EncounterNumber { get; set; }
    }
}
