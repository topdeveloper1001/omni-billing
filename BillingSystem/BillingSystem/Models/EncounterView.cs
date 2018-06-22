using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class EncounterView
    {
        public List<EncounterCustomModel> EncounterList { get; set; }
        public Encounter CurrentEncounter { get; set; }
    }
}