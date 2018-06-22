using BillingSystem.Model;
using BillingSystem.Models;
using BillingSystem.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Model.CustomModel;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace BillingSystem.Controllers
{
    public class MedicalVitalController : BaseController
    {
        /// <summary>
        /// Get the details of the MedicalVital View in the Model MedicalVital such as MedicalVitalList, list of countries etc.
        /// </summary>
        /// <param name="shared">passed the input object</param>
        /// <returns>returns the actionresult in the form of current object of the Model MedicalVital to be passed to View MedicalVital</returns>
        public ActionResult MedicalVitalMain()
        {
            //Initialize the MedicalVital BAL object
            var MedicalVitalBal = new MedicalVitalBal();

            //Get the Entity list
            var MedicalVitalList = MedicalVitalBal.GetMedicalVital();

            //Intialize the View Model i.e. MedicalVitalView which is binded to Main View Index.cshtml under MedicalVital
            var MedicalVitalView = new MedicalVitalView
            {
                MedicalVitalList = new List<MedicalVitalCustomModel>(),
                CurrentMedicalVital = new MedicalVitalCustomModel()
            };

            //Pass the View Model in ActionResult to View MedicalVital
            return View(MedicalVitalView);
        }

        /// <summary>
        /// Bind all the MedicalVital list
        /// </summary>
        /// <param name="PatientID">The patient identifier.</param>
        /// <returns>
        /// action result with the partial view containing the MedicalVital list object
        /// </returns>
        public ActionResult BindMedicalVitalList(int PatientID)
        {
            //Initialize the MedicalNotes BAL object
            using (var medicalVitalBal = new MedicalVitalBal())
            {
                var medicalVitalType = Convert.ToInt32(Common.Common.MedicalRecordType.Vitals);
                //Get the facilities list
                //var medicalNotesList = medicalNotesBal.GetMedicalNotes();
                var medicalVitalList = medicalVitalBal.GetCustomMedicalVitals(PatientID, medicalVitalType);

                //Pass the ActionResult with List of MedicalNotesViewModel object to Partial View MedicalNotesList
                return PartialView(PartialViews.MedicalVitalList, medicalVitalList);
            }
        }

        /// <summary>
        /// Add New or Update the MedicalVital based on if we pass the MedicalVital ID in the MedicalVitalViewModel object.
        /// </summary>
        /// <param name="medicalVitalModel">pass the details of MedicalVital in the view model</param>
        /// <returns>
        /// returns the newly added or updated ID of MedicalVital row
        /// </returns>
        public ActionResult SaveMedicalVital(Model.MedicalVital medicalVitalModel)
        {
            //Initialize the newId variable 
            var newId = -1;
            //corporate Id
            var corporateId = Helpers.GetSysAdminCorporateID();
            //Facility Id
            var facilityId = Helpers.GetDefaultFacilityId();
            //User Id
            var userId = Helpers.GetLoggedInUserId();
            if (medicalVitalModel != null)
            {
                medicalVitalModel.FacilityID = facilityId;
                medicalVitalModel.CorporateID = corporateId;
                medicalVitalModel.CommentBy = userId;
                medicalVitalModel.CommentDate = Helpers.GetInvariantCultureDateTime();
                using (var medicalVitalBal = new MedicalVitalBal())
                {
                    if (medicalVitalModel.MedicalVitalID > 0)
                    {
                        medicalVitalModel.ModifiedBy = userId;
                        medicalVitalModel.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                    }
                    else
                    {
                        medicalVitalModel.CreatedBy = userId;
                        medicalVitalModel.CreatedDate = Helpers.GetInvariantCultureDateTime();
                    }
                    //Call the AddMedicalVital Method to Add / Update current MedicalVital
                    newId = medicalVitalBal.AddUptdateMedicalVital(medicalVitalModel);
                }
            }
            return Json(newId);
        }

        /// <summary>
        /// Add New or Update the ModuleAccess based on if we pass the ModuleAccess ID in the ModuleAccessViewModel object.
        /// </summary>
        /// <param name="medicalVitalModelList">The medical vital model list.</param>
        /// <returns>
        /// returns the newly added or updated ID of ModuleAccess row
        /// </returns>
        public ActionResult SaveMedicalVitals(List<MedicalVital> medicalVitalModelList)
        {
            try
            {
                var userid = Helpers.GetLoggedInUserId();
                var CorporateId = Helpers.GetDefaultCorporateId();
                var FacilityId = Helpers.GetDefaultFacilityId();
                var objListModuleAccessPermission = (from item in medicalVitalModelList
                    where item != null
                    select new MedicalVital()
                    {
                        MedicalVitalID = item.MedicalVitalID,
                        MedicalVitalType = item.MedicalVitalType,
                        PatientID = item.PatientID,
                        EncounterID = item.EncounterID,
                        MedicalRecordNumber = item.MedicalRecordNumber,
                        GlobalCodeCategoryID = item.GlobalCodeCategoryID,
                        GlobalCode = item.GlobalCode,
                        AnswerValueMin = item.AnswerValueMin,
                        AnswerValueMax = item.AnswerValueMax,
                        AnswerUOM = item.AnswerUOM,
                        Comments = item.Comments,
                        CorporateID = CorporateId,
                        FacilityID = FacilityId,
                        CommentBy = userid,
                        CommentDate = Helpers.GetInvariantCultureDateTime(),
                        CreatedBy = userid,
                        CreatedDate = Helpers.GetInvariantCultureDateTime()
                    }).ToList();
                var moduleaccessbal = new MedicalVitalBal();
                moduleaccessbal.AddUpdateModuleAccess(objListModuleAccessPermission);
                return Json(1);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Bind all the MedicalNotes list
        /// </summary>
        /// <param name="PatientID">The patient identifier.</param>
        /// <param name="Encounterid">The encounterid.</param>
        /// <returns>
        /// action result with the partial view containing the MedicalNotes list object
        /// </returns>
        public ActionResult BindLabTestList(int PatientID, int Encounterid)
        {
            //Initialize the MedicalNotes BAL object
            using (var medicalVitalBal = new MedicalVitalBal())
            {
                var medicalVitalType = Convert.ToInt32(Common.Common.MedicalRecordType.LabTest);
                //Get the facilities list
                //var medicalNotesList = medicalNotesBal.GetMedicalNotes();
                var medicalVitalList = medicalVitalBal.GetCustomLabTest(PatientID, Encounterid, medicalVitalType);

                //Pass the ActionResult with List of MedicalNotesViewModel object to Partial View MedicalNotesList
                return PartialView(PartialViews.MedicalVitalList, medicalVitalList);
            }
        }

        /// <summary>
        /// Get the details of the current MedicalVital in the view model by ID
        /// </summary>
        /// <param name="MedicalVitalID">The medical vital identifier.</param>
        /// <returns></returns>
        public ActionResult GetMedicalVital(int MedicalVitalID)
        {
            using (var MedicalVitalBal = new MedicalVitalBal())
            {
                //Call the AddMedicalVital Method to Add / Update current MedicalVital
                var currentMedicalVital = MedicalVitalBal.GetMedicalVitalByID(Convert.ToInt32(MedicalVitalID));

                //If the view is shown in ViewMode only, then ViewBag.ViewOnly is set to true otherwise false.
                //ViewBag.ViewOnly = !string.IsNullOrEmpty(model.ViewOnly);

                //Pass the ActionResult with the current MedicalVitalViewModel object as model to PartialView MedicalVitalAddEdit
                return PartialView(PartialViews.MedicalVitalAddEdit, currentMedicalVital);
            }
        }

        /// <summary>
        /// Delete the current MedicalVital based on the MedicalVital ID passed in the MedicalVitalModel
        /// </summary>
        /// <param name="MedicalVitalID">The medical vital identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteMedicalVital(int MedicalVitalID)
        {
            using (var MedicalVitalBal = new MedicalVitalBal())
            {
                //Get MedicalVital model object by current MedicalVital ID
                var currentMedicalVital = MedicalVitalBal.GetMedicalVitalByID(Convert.ToInt32(MedicalVitalID));

                //Check If MedicalVital model is not null
                if (currentMedicalVital != null)
                {
                    currentMedicalVital.IsDeleted = true;
                    currentMedicalVital.DeletedBy = Helpers.GetLoggedInUserId();
                    currentMedicalVital.DeletedDate = Helpers.GetInvariantCultureDateTime();

                    //Update Operation of current MedicalVital
                    var result = MedicalVitalBal.AddUptdateMedicalVital(currentMedicalVital);

                    //return deleted ID of current MedicalVital as Json Result to the Ajax Call.
                    return Json(result);
                }
            }

            //Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }

        /// <summary>
        /// Reset the MedicalVital View Model and pass it to MedicalVitalAddEdit Partial View.
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetMedicalVitalForm()
        {
            //Intialize the new object of MedicalVital ViewModel
            var MedicalVitalViewModel = new MedicalVitalCustomModel();

            //Pass the View Model as MedicalVitalViewModel to PartialView MedicalVitalAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.MedicalVitalAddEdit, MedicalVitalViewModel);
        }

        /// <summary>
        /// Get Collection for the chart
        /// </summary>
        /// <param name="ChartObject">The chart object.</param>
        /// <param name="PatientId">The patient identifier.</param>
        /// <param name="DisplayType">The display type.</param>
        /// <param name="SelectedDate">The selected date.</param>
        /// <param name="ResultType">Type of the result.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public ActionResult GetChartDataCollection(string ChartObject, string PatientId, string DisplayType, string SelectedDate, string ResultType, [DataSourceRequest] DataSourceRequest request)
        {
            try
            {
                int displayTypeId = 0;
                if (DisplayType == "Day")
                {
                    displayTypeId = 900;
                }
                else if (DisplayType == "Week")
                {
                    displayTypeId = 901;
                }
                else if (DisplayType == "MonthWeek")
                {
                    displayTypeId = 902;
                }
                else if (DisplayType == "MonthDays")
                {
                    displayTypeId = 903;
                }
                else if (DisplayType == "Year")
                {
                    displayTypeId = 904;
                }
                DateTime selectedDateTime = Convert.ToDateTime(SelectedDate);

                var JsonResult = Json(GetChartData(ChartObject, Convert.ToInt32(PatientId), displayTypeId, selectedDateTime, ResultType).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
                JsonResult.MaxJsonLength = int.MaxValue;
                return JsonResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the chart data.
        /// </summary>
        /// <param name="chartObject">The chart object.</param>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="displayTypeId">The display type identifier.</param>
        /// <param name="selectedDate">The selected date.</param>
        /// <param name="ResultType">Type of the result.</param>
        /// <returns></returns>
        public IEnumerable<Result> GetChartData(string chartObject, int patientId, int displayTypeId, DateTime selectedDate, string ResultType)
        {
            List<Result> objList = new List<Result>();
            var medicalVitalBal = new MedicalVitalBal();
            List<MedicalVitalCustomModel> medicalVitalList = new List<MedicalVitalCustomModel>();
            medicalVitalList = medicalVitalBal.GetMedicalVitalsChartData(patientId, displayTypeId, selectedDate);
            medicalVitalList = medicalVitalList.Where(x => x.VitalName == chartObject).ToList();
            foreach (var item in medicalVitalList)
            {
                if (ResultType == "Average")
                {
                    objList.Add(new Result { ChartType = chartObject, XAxisFieldValue = item.XAxis, YAxisFieldValue = item.Average.ToString(), MaxLimit = item.UpperLimit.ToString(), MinLimit = item.LowerLimit.ToString() });
                }
                else if (ResultType == "Maximum")
                {
                    objList.Add(new Result { ChartType = chartObject, XAxisFieldValue = item.XAxis, YAxisFieldValue = item.Maximum.ToString(), MaxLimit = item.UpperLimit.ToString(), MinLimit = item.LowerLimit.ToString() });
                }
                else if (ResultType == "Minumum")
                {
                    objList.Add(new Result { ChartType = chartObject, XAxisFieldValue = item.XAxis, YAxisFieldValue = item.Minimum.ToString(), MaxLimit = item.UpperLimit.ToString(), MinLimit = item.LowerLimit.ToString() });
                }
            }
            return objList;
        }

        /// <summary>
        /// Gets the vital chart2.
        /// </summary>
        /// <param name="vitalCode">The vital code.</param>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="tillDate">The till date.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public ActionResult GetVitalChart2(int vitalCode, int patientId, string fromDate, string tillDate, [DataSourceRequest] DataSourceRequest request)
        {

            DateTime? selectedFrom = null;
            if (!string.IsNullOrEmpty(fromDate))
                selectedFrom = Convert.ToDateTime(fromDate);

            DateTime? selectedTillDate = null;
            if (!string.IsNullOrEmpty(tillDate))
                selectedTillDate = Convert.ToDateTime(tillDate);

            using (var bal = new MedicalVitalBal())
            {
                var chartData = bal.GetMedicalVitalChart2(vitalCode, patientId, selectedFrom, selectedTillDate).ToDataSourceResult(request);
                var jsonResult = Json(chartData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
        }

        /// <summary>
        /// Gets the high chart vital chart data.
        /// </summary>
        /// <param name="vitalCode">The vital code.</param>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="tillDate">The till date.</param>
        /// <returns></returns>
        public ActionResult GetHighChartVitalChartData(int vitalCode, int patientId, string fromDate, string tillDate)
        {

            DateTime? selectedFrom = null;
            if (!string.IsNullOrEmpty(fromDate))
                selectedFrom = Convert.ToDateTime(fromDate);

            DateTime? selectedTillDate = null;
            if (!string.IsNullOrEmpty(tillDate))
                selectedTillDate = Convert.ToDateTime(tillDate);

            using (var bal = new MedicalVitalBal())
            {
                var chartData = bal.GetMedicalVitalChart2(vitalCode, patientId, selectedFrom, selectedTillDate);
                var objToReturn = new
                                      {
                    BloodPressureSystolic = chartData.Where(x => x.VitalCode == 1).ToList(),
                    BloodPressureDiastolic = chartData.Where(x => x.VitalCode == 5).ToList(),
                    Weight = chartData.Where(x => x.VitalCode == 2).ToList(),
                    Temperature = chartData.Where(x => x.VitalCode == 3).ToList(),
                    HeartRate = chartData.Where(x => x.VitalCode == 4).ToList()
                };
                var jsonResult = Json(objToReturn, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
        }

        public class Result
        {
            public string XAxisFieldValue { get; set; }
            public string YAxisFieldValue { get; set; }
            public string YAxisFieldValue2 { get; set; }
            public string MaxLimit { get; set; }
            public string MinLimit { get; set; }
            public string ChartType { get; set; }
        }
    }
}
