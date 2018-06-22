using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common;
using BillingSystem.Model;
using BillingSystem.Model.Model;
using BillingSystem.Models;

namespace BillingSystem.Controllers
{
    public class PDFTemplatesController : Controller
    {
        //
        // GET: /PDFTemplates/
        public ActionResult Index(int? pId)
        {

            var pdftemplatesView = new PDFTemplatesView
            {
                VisitDetailList = new List<OtherPatientForm>(),
                MeasurementsList = new List<OtherPatientForm>(),
                AllergyList = new List<OtherPatientForm>(),
                PainAssessmentList = new List<OtherPatientForm>(),
                ESILevel = new List<OtherPatientForm>(),
                IllnessList = new List<OtherPatientForm>(),
                NursingAssessmentList = new List<OtherPatientForm>(),
                EconomicalHistory = new List<OtherPatientForm>(),
                VaccinationHistory = new List<OtherPatientForm>(),
                NutritionalScreening = new List<OtherPatientForm>(),
                FunctionalScreening = new List<OtherPatientForm>(),
                RiskList = new List<OtherPatientForm>(),
                EducationNeeds = new List<OtherPatientForm>(),
                NurseNotes = new List<OtherPatientForm>(),
                PainAssessmentLevels = new List<OtherPatientForm>()
            };
            return View(pdftemplatesView);
        }


        public ActionResult SaveOutPatientNurseAssessment(List<string> basicInfo, string patientId, string encounterId, int setId, string estatus, string formNumber, string imagePath)
        {
            var rowExists = string.Empty;
            int eNumId = -1;
            if (setId > 0)
            {
                rowExists = "1";
            }
            using (var oPdfTemplates = new PDFTemplatesBal())
            {
                using (var oPatientEvaluationBal = new PatientEvaluationBal())
                {
                    var evaluationSet = new PatientEvaluationSet
                    {
                        SetId = setId,
                        PatientId = Convert.ToInt32(patientId),
                        EncounterId = Convert.ToInt32(encounterId),
                        CreatedBy = Helpers.GetLoggedInUserId(),
                        CreatedDate = Helpers.GetInvariantCultureDateTime(),
                        FormType = "Out Patient Nurse Assessment",
                        ExtValue2 = "99",
                        FormNumber = formNumber
                    };
                    var id = oPatientEvaluationBal.SaveEvaluationSet(evaluationSet);
                    foreach (var item in basicInfo)
                    {
                        var oOutPatientNursing = new OtherPatientForm
                        {
                            ExternalValue2 = Convert.ToString(id),
                            CorporateId = Helpers.GetSysAdminCorporateID(),
                            FacilityId = Helpers.GetDefaultFacilityId(),
                            PatientId = Convert.ToInt32(patientId),
                            EncounterId = Convert.ToInt32(encounterId),
                            CategoryValue = item.Split('-')[2],
                            CodeValue = item.Split('-')[1],
                            ParentCodeValue = item.Split('-')[3],
                            Value = item.Split('-')[0],
                            CreatedBy = Helpers.GetLoggedInUserId(),
                            CreatedDate = Helpers.GetInvariantCultureDateTime(),
                            ExternalValue1 = estatus,
                            ExternalValue3 = imagePath
                        };
                        //if (rowExists == "0")
                        //{
                        //    eNumId = oPdfTemplates.SavePDFTemplates(oOutPatientNursing);
                        //}
                        //else
                        //{
                        //    eNumId = oPatientEvaluationBal.UpdateEvaluationManagement(oOutPatientNursing);
                        //}
                        if (rowExists == "1")
                        {
                            eNumId = oPdfTemplates.UpdateOutPatientAssessment(oOutPatientNursing);
                        }
                        else
                        {
                            eNumId = oPdfTemplates.SavePDFTemplates(oOutPatientNursing);
                        }




                    }
                }
            }

            return Json(eNumId, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PdfTemplatesData(int pId, string setId)
        {
            var personAge = 0;
            var currentDate = Helpers.GetInvariantCultureDateTime();
            var pInfoBal = new PatientInfoBal();

            var pBal = new PDFTemplatesBal();
            var sId = Convert.ToInt32(setId);
            var patientId = Convert.ToInt32(pId);
            using (
                var orderBal = new OpenOrderBal(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var enList = orderBal.GetEncountersListByPatientId(patientId);
                var currentEncounterId = (enList != null && enList.Any() &&
                                          enList.First().EncounterEndTime == null)
                    ? enList.First().EncounterID
                    : 0;


                #region List Of Assessment Forms

                PatientInfo currentPatient = null;
                List<OtherPatientForm> listVisitDetail;
                List<OtherPatientForm> vaccinationHistory;
                List<OtherPatientForm> nutritionalScreeningList;
                List<OtherPatientForm> functionalScreeningList;
                List<OtherPatientForm> riskList;
                List<OtherPatientForm> educationNeedsList;
                List<OtherPatientForm> nurseNotes;
                List<OtherPatientForm> measurementsList;
                List<OtherPatientForm> painAssessmentLevels;
                List<OtherPatientForm> allergyList;
                List<OtherPatientForm> painAssessmentList;
                List<OtherPatientForm> eSiLevelList;
                List<OtherPatientForm> illnessList;
                List<OtherPatientForm> nursingAssessmentList;
                List<OtherPatientForm> economicalHistory;

                using (var bal = new PDFTemplatesBal())
                {
                    var enId = Convert.ToInt32(currentEncounterId);
                    currentPatient = bal.GetPatientInfoByEncounterId(currentEncounterId);
                    var allItems = bal.GetNursingAssessmentFormData(patientId, enId, setId);
                    personAge = pInfoBal.CalculatePersonAge(currentPatient.PersonBirthDate, currentDate);
                    listVisitDetail = allItems.Where(c => c.CategoryValue == "6100").ToList(); //bal.ListNurseAssessmentForm(patientId, Convert.ToInt32(currentEncounterId), "6100", setId);
                    measurementsList = allItems.Where(c => c.CategoryValue == "6101").ToList(); //bal.ListNurseAssessmentForm(patientId, Convert.ToInt32(currentEncounterId), "6101", setId);
                    allergyList = allItems.Where(c => c.CategoryValue == "6102").ToList(); //bal.ListNurseAssessmentForm(patientId, Convert.ToInt32(currentEncounterId), "6102", setId);
                    painAssessmentList = allItems.Where(c => c.CategoryValue == "6103").ToList(); //bal.ListNurseAssessmentForm(patientId, Convert.ToInt32(currentEncounterId), "6103", setId);
                    eSiLevelList = allItems.Where(c => c.CategoryValue == "6104").ToList(); //bal.ListNurseAssessmentForm(patientId, Convert.ToInt32(currentEncounterId), "6104", setId);
                    illnessList = allItems.Where(c => c.CategoryValue == "6105").ToList(); //bal.ListNurseAssessmentForm(patientId, Convert.ToInt32(currentEncounterId), "6105", setId);
                    nursingAssessmentList = allItems.Where(c => c.CategoryValue == "6106").ToList(); //bal.ListNurseAssessmentForm(patientId, Convert.ToInt32(currentEncounterId), "6106", setId);
                    economicalHistory = allItems.Where(c => c.CategoryValue == "6107").ToList(); //bal.ListNurseAssessmentForm(patientId, Convert.ToInt32(currentEncounterId), "6107", setId);
                    vaccinationHistory = allItems.Where(c => c.CategoryValue == "6108").ToList(); //bal.ListNurseAssessmentForm(patientId, Convert.ToInt32(currentEncounterId), "6108", setId);
                    nutritionalScreeningList = allItems.Where(c => c.CategoryValue == "6109").ToList(); //bal.ListNurseAssessmentForm(patientId, Convert.ToInt32(currentEncounterId), "6109", setId);
                    functionalScreeningList = allItems.Where(c => c.CategoryValue == "6110").ToList(); //bal.ListNurseAssessmentForm(patientId, Convert.ToInt32(currentEncounterId), "6110", setId);
                    riskList = allItems.Where(c => c.CategoryValue == "6111").ToList(); //bal.ListNurseAssessmentForm(patientId, Convert.ToInt32(currentEncounterId), "6111", setId);
                    educationNeedsList = allItems.Where(c => c.CategoryValue == "6112").ToList(); //bal.ListNurseAssessmentForm(patientId, Convert.ToInt32(currentEncounterId), "6112", setId);
                    nurseNotes = allItems.Where(c => c.CategoryValue == "6113").ToList(); //bal.ListNurseAssessmentForm(patientId, Convert.ToInt32(currentEncounterId), "6113", setId);
                    painAssessmentLevels = allItems.Where(c => c.CategoryValue == "6116").ToList(); //bal.ListNurseAssessmentForm(patientId, Convert.ToInt32(currentEncounterId), "6116", setId);
                }

                #endregion End List

                var pdftemplatesView = new PDFTemplatesView
                {
                    VisitDetailList = listVisitDetail,
                    MeasurementsList = measurementsList,
                    AllergyList = allergyList,
                    PainAssessmentList = painAssessmentList,
                    ESILevel = eSiLevelList,
                    IllnessList = illnessList,
                    NursingAssessmentList = nursingAssessmentList,
                    EconomicalHistory = economicalHistory,
                    VaccinationHistory = vaccinationHistory,
                    NutritionalScreening = nutritionalScreeningList,
                    FunctionalScreening = functionalScreeningList,
                    RiskList = riskList,
                    EducationNeeds = educationNeedsList,
                    NurseNotes = nurseNotes,
                    SetId = sId,
                    PatientId = currentPatient.PatientID,
                    PatientFirstName = currentPatient.PersonFirstName,
                    PatientLastName = currentPatient.PersonLastName,
                    EncounterId = currentEncounterId,
                    FormNumber = pBal.GetNewFormDetailsByFormType(),
                    DateOfBirth = Convert.ToString(currentPatient.PersonBirthDate),
                    PersonAge = Convert.ToString(personAge),
                    PainAssessmentLevels = painAssessmentLevels
                };
                return PartialView(PartialViews.OPNurseAssessmentForm, pdftemplatesView);
            }
        }

        public ActionResult GetSignatureData(int ecounterId, int patinetId, string setId)
        {
            using (var eBal = new PDFTemplatesBal())
            {
                var list = eBal.GetSignaturePathNurseForm(ecounterId, patinetId, setId);
                return Json(list);

            }
        }

    }
}