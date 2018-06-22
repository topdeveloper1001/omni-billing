using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class PatientLoginDetailCustomModel : PatientLoginDetail
    {
        public string ConfirmPassword { get; set; }
        public string Facility { get; set; }
        public string PatientName { get; set; }
        public bool FirstTimeUser { get; set; }
        public bool DeleteVerificationToken { get; set; }
        public string NewCodeValue { get; set; }

        public int FacilityId { get; set; }
        public string FacilityNumber { get; set; }
        public int CorporateId { get; set; }

        public DateTime? BirthDate { get; set; }
        public string EmriateId { get; set; }
    }
}
