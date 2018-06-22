using System;
using System.Collections.Generic;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class PatientVolumeDashboardView
    {
        public List<DashboardChargesCustomModel> IPEncountersCustomModel { get; set; }
        public List<DashboardChargesCustomModel> OPEncountersCustomModel { get; set; }
        public List<DashboardChargesCustomModel> EREncountersCustomModel { get; set; }

        public List<DashboardChargesCustomModel> PatientDaysCustomModel { get; set; }
        public List<DashboardChargesCustomModel> DisChargesCustomModel { get; set; }
        public List<DashboardChargesCustomModel> ALOSCustomModel { get; set; }
        public List<DashboardChargesCustomModel> IPADCCustomModel { get; set; }

        public int CurrentFacilityId { get; set; }
        public DateTime CurrentDate { get; set; }
        public int NumberofCurrentDay { get; set; }
    }
}