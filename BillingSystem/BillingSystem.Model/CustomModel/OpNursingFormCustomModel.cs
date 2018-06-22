using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BillingSystem.Model;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class OpNursingFormCustomModel
    {
        [Key]
        public int Id { get; set; }
        public string  FormNumber { get; set; }
        public string Nursing { get; set; }
        public string PreparedBy { get; set; }
        public string ReviwedBy { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ReviweDate { get; set; }
        public DateTime? NextReviwedDate { get; set; }
        public int PatientId { get; set; }
        public DateTime? CurrentDateTime { get; set; }
        public string ReasonForVisit { get; set; }
        public bool? IsAppointment { get; set; }
        public string AssessmentTypes { get; set; }
        public bool AssessmentRequired { get; set; }
        public string Temp { get; set; }
        public string Pulse { get; set; }
        public string BloodPrasure { get; set; }
        public string RESP { get; set; }
        public string SPO2 { get; set; }
        public string Weight { get; set; }
        public string Height { get; set; }
        public string HC { get; set; }
        public string AllergyType { get; set; }
        public bool IsAllergy { get; set; }

       
    }
}
