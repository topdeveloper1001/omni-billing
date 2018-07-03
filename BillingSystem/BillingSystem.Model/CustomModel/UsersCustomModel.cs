using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class UsersCustomModel
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string FacilityNames { get; set; }
        public Users CurrentUser { get; set; }
        public string PhoneNumber { get; set; }
        public string HomePhoneNumber { get; set; }
        public string Name { get; set; }
        public string CodeValue { get; set; }
        public string CorporateName { get; set; }
    }

    [NotMapped]
    public class UsersViewModel : Users
    {
        public string CodeValue { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string EnterCode { get; set; }
        public string Name { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string ClinicianName { get; set; }
        public IEnumerable<Tabs> Tabs { get; set; }
        public bool IsFirstTimeLoggedIn { get; set; }
        public string FacilityNumber { get; set; }
        public string FacilityName { get; set; }
        public int RolesCount { get; set; }
        public string RoleKey { get; set; }
        public string TimeZone { get; set; }
        public int DefaultCountryId { get; set; }
        public bool IsEhrAccessible { get; set; }
        public bool IsActiveEncountersAccessible { get; set; }
        public bool IsAuthorizationAccessible { get; set; }
        public bool IsBillHeaderViewAccessible { get; set; }
        public bool IsPatientSearchAccessible { get; set; }
        public bool SchedularAccessible { get; set; }

        public string CptTableNumber { get; set; }

        public string ServiceCodeTableNumber { get; set; }

        public string DrugTableNumber { get; set; }

        public string DrgTableNumber { get; set; }

        public string DiagnosisCodeTableNumber { get; set; }

        public string BillEditRuleTableNumber { get; set; }

        public string HcPcsTableNumber { get; set; }
    }
}
