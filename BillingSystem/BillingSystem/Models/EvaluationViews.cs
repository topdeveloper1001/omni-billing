using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class EvaluationViews
    {
        public List<MedicalHistoryCustomModel> MedicalHistoryList { get; set; }
        public List<AlergyCustomModel> AlergyList { get; set; }
        public OrderActivity CurrentOrderActivity { get; set; }
        public OrderActivityCustomModel CurrentLabOrderActivity { get; set; }
        public List<OrderActivityCustomModel> OrderActivityList { get; set; }
        public List<OpenOrderCustomModel> OpenOrdersList { get; set; }
        public List<OpenOrderCustomModel> ClosedOrdersList { get; set; }
        public List<OrderActivityCustomModel> ClosedOrderActivityList { get; set; }
        public List<OrderActivityCustomModel> labOrderActivityList { get; set; }
        public List<OrderActivityCustomModel> ClosedLabOrderActivityList { get; set; }
        public OpenOrder EncounterOrder { get; set; }
        public bool IsLabTest { get; set; }
        public List<MedicalVitalCustomModel> MedicalVitalList { get; set; }

        public List<ChiefComplaintReasonView> ChiefComplaintReasonItems { get; set; }
        public List<SelectListItem> HistoryPresentIllnessItems { get; set; }
        public List<SelectListItem> OrganSystemsItems { get; set; }
        public List<SelectListItem> BodyAreaItems { get; set; }
        public List<SelectListItem> AmtComplexityDataReviewedItems { get; set; }
        public List<SelectListItem> DiagnosisManagementOptionItems { get; set; }
        public List<SelectListItem> RiskOfComplicationItems { get; set; }
        public string PatientCounseledRe { get; set; }
        public string ElectronicSignature { get; set; }
        public List<PatientEvaluation> BasicInfoItems { get; set; }
        public List<PatientEvaluation> ChiefComplaintItems { get; set; }
        public List<PatientEvaluation> HistoryPresentIllnessList { get; set; }
        public List<PatientEvaluation> ReviewSystemsList { get; set; }
        public List<PatientEvaluation> OrganSystemsList { get; set; }
        public List<PatientEvaluation> BodyAreasList { get; set; }
        public List<PatientEvaluation> AmtComplexityReviewedList { get; set; }
        public List<PatientEvaluation> DiagnisosManagementList { get; set; }
        public List<PatientEvaluation> RiskComplicationsList { get; set; }
        public List<PatientEvaluation> PatientCounseledList { get; set; }
        public List<PatientEvaluation> ElecSignatureList { get; set; }
        public string SetId { get; set; }
    }




}