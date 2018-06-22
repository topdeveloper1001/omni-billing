using BillingSystem.Model.CustomModel;
using System;
using System.Collections.Generic;

namespace BillingSystem.Model.CustomModel
{
    public class OrderCustomModel
    {
        public List<OpenOrderCustomModel> MostRecentOrders { get; set; }
        public List<OpenOrderCustomModel> PreviousOrders { get; set; }
        public List<OpenOrderCustomModel> FavoriteOrders { get; set; }
        public List<OpenOrderCustomModel> OpenOrders { get; set; }
        public List<OrderActivityCustomModel> OrderActivities { get; set; }
        public List<FutureOpenOrderCustomModel> FutureOpenOrders { get; set; }
        public List<GlobalCodes> GlobalCodes { get; set; }
        public long OrderId { get; set; }
    }



    public class PhysicianTabData
    {
        public List<PatientEvaluationSetCustomModel> EncounterListData { get; set; }
        public List<OpenOrderCustomModel> OpenOrders { get; set; }
        public List<DocumentsTemplates> NurseDocuments { get; set; }
        public List<OrderActivityCustomModel> PatientCareActivities { get; set; }
        public List<MedicalVital> Vitals { get; set; }
        public List<MedicalVitalCustomModel> Vitals2 { get; set; }
        public List<MedicalNotes> MedicalNotes { get; set; }
        public List<MedicalNotesCustomModel> MedicalNotes2 { get; set; }
        public List<GlobalCodes> DropdownListData { get; set; }
        public List<GlobalCodeCategory> OrderTypes { get; set; }
        public List<OpenOrderCustomModel> LabOrders { get; set; }
    }


    public class PatientSummaryTabData
    {
        public PatientInfoCustomModel PatientInfo { get; set; }
        public List<EncounterCustomModel> Encounters { get; set; }
        public List<MedicalRecord> MedicalRecords { get; set; }
        public List<AlergyCustomModel> AllergyRecords { get; set; }
        //public List<AlergyCustomModel> AllergyRecords2 { get; set; }
        public List<MedicalVitalCustomModel> Vitals { get; set; }
        //public List<MedicalVitalCustomModel> Vitals2 { get; set; }
        public List<MedicalNotesCustomModel> MedicalNotes { get; set; }
        //public List<MedicalNotesCustomModel> MedicalNotes2 { get; set; }
        public List<OpenOrderCustomModel> OpenOrders { get; set; }
        public List<DiagnosisCustomModel> DiagnosisList { get; set; }
        public IEnumerable<MedicalHistoryCustomModel> MedicalHistory { get; set; }
        public RiskFactorViewModel RiskFactor { get; set; }

        public long CurrentEncounterId { get; set; }
    }


    public class PatientInfoViewData
    {
        public PatientInfoCustomModel PatientInfo { get; set; }
        public PatientInsuranceCustomModel PatientInsurance { get; set; }
        public PatientLoginDetailCustomModel PatientLoginInfo { get; set; }
        public PatientPhone PatientPhone { get; set; }
        public bool EncounterOpen { get; set; }
    }

    public class OrderCodes
    {
        public GlobalCodes GlobalCode { get; set; }
        public IEnumerable<DropdownListData> OrderCodeList { get; set; }
    }

}
