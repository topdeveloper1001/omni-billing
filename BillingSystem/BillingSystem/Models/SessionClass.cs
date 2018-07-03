
using BillingSystem.Model;
using System.Collections.Generic;

namespace BillingSystem.Models
{
    public class SessionClass
    {
        public string UserName { get; set; }

        public string FacilityNumber { get; set; }

        public int RoleId { get; set; }

        public int FacilityId { get; set; }

        public int CorporateId { get; set; }

        public IEnumerable<Tabs> MenuSessionList { get; set; }

        public string RoleName { get; set; }

        public string FacilityName { get; set; }

        public int UserId { get; set; }

        public decimal AutoLogOffMinutes { get; set; }

        public string SelectedCulture { get; set; }

        public bool UserIsAdmin { get; set; }

        public bool FirstTimeLogin { get; set; }

        public int SysAdminCorporateId { get; set; }

        public string TimeZone { get; set; }

        public long DefaultCountryId { get; set; }

        public int LoginUserType { get; set; }

        public string NavTabId { get; set; }

        public string NavParentTabId { get; set; }

        public bool IsEhrAccessible { get; set; }

        public bool IsActiveEncountersAccessible { get; set; }

        public bool IsAuthorizationAccessible { get; set; }

        public bool IsBillHeaderViewAccessible { get; set; }

        public bool IsPatientSearchAccessible { get; set; }

        public bool SchedularAccessible { get; set; }

        public string UserEmail { get; set; }

        public string CptTableNumber { get; set; }

        public string ServiceCodeTableNumber { get; set; }

        public string DrugTableNumber { get; set; }

        public string DrgTableNumber { get; set; }

        public string DiagnosisCodeTableNumber { get; set; }

        public string BillEditRuleTableNumber { get; set; }

        public string HcPcsTableNumber { get; set; }

        public string RoleKey { get; set; }

        public int CountryId { get; set; }
    }
}