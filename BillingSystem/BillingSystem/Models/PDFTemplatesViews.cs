using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;
using BillingSystem.Model.Model;

namespace BillingSystem.Models
{
    public class PDFTemplatesView : PatientEvaluationSetCustomModel
    {
        public List<OtherPatientForm> BasicInformationList { get; set; }
        public List<OtherPatientForm> VisitDetailList { get; set; }
        public List<OtherPatientForm> MeasurementsList { get; set; }
        public List<OtherPatientForm> AllergyList { get; set; }
        public List<OtherPatientForm> PainAssessmentList { get; set; }
        public List<OtherPatientForm> ESILevel { get; set; }
        public List<OtherPatientForm> IllnessList { get; set; }
        public List<OtherPatientForm> NursingAssessmentList { get; set; }
        public List<OtherPatientForm> EconomicalHistory { get; set; }
        public List<OtherPatientForm> VaccinationHistory { get; set; }
        public List<OtherPatientForm> NutritionalScreening { get; set; }
        public List<OtherPatientForm> FunctionalScreening { get; set; }
        public List<OtherPatientForm> RiskList { get; set; }
        public List<OtherPatientForm> EducationNeeds { get; set; }
        public List<OtherPatientForm> NurseNotes { get; set; }
        public List<OtherPatientForm> NurseSignature { get; set; }
        public List<OtherPatientForm> PainAssessmentLevels { get; set; }

        //public string SetId { get; set; }
        //public int PatientId { get; set; }
        //public int EncounterId { get; set; }
        public string PatientFirstName { get; set; }
        public string PatientLastName { get; set; }
        public string DateOfBirth { get; set; }
        public string   PersonAge { get; set; }

    }
}
