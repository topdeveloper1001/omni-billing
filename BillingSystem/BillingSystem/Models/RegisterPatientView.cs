using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class RegisterPatientView
    {
        public PatientInfoCustomModel CurrentPatient { get; set; }
        public PatientInsuranceCustomModel Insurance { get; set; }
        public PatientPhone CurrentPhone { get; set; }
        public PatientLoginDetailCustomModel PatientLoginDetail { get; set; }
        //public PatientAddressRelation AddressRelation { get; set; }
        public PatientAddressRelation CurrentPatientAddressRelation { get; set; }
        public DocumentsTemplates DocumentsAttachment { get; set; }
        //public CommonDataView CountryStateList { get; set; }
        public List<Country> CountryList { get; set; }
        public List<State> StatesList { get; set; }
        public List<City> CityList { get; set; }
        
    }
}