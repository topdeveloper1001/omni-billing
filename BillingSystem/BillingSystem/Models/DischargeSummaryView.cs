using System.Collections.Generic;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class DischargeSummaryView
    {
        public List<MedicalNotesCustomModel> SummaryNotes { get; set; }
    }
}