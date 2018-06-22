using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class BillFormatCustomModel
    {
        public Facility FacilityDetails { get; set; }
        public PatientInfo PatientDetails { get; set; }
        public Encounter EncounterDetails { get; set; }
        public List<BillDetailCustomModel> BillDetails { get; set; }
        public BillHeader BillHeaderDeatils { get; set; }
    }
}
