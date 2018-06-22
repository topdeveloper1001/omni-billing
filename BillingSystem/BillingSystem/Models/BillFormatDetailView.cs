using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using System.Collections.Generic;

namespace BillingSystem.Models
{
    public class BillFormatDetailView
    {
        public Facility FacilityDetails { get; set; }
        public PatientInfo PatientDetails { get; set; }
        public Encounter EncounterDetails { get; set; }
        public List<BillDetailCustomModel> BillDetails { get; set; }
        public BillHeader BillHeaderDeatils { get; set; }
    }
}