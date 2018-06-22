// /*  ----------------------------------------------------------------------------------------------------------------- */
// To Do: DischargeController.cs
// FileName :DischargeController.cs
// CreatedDate: 2015-10-05 11:34 AM
// ModifiedDate: 2016-05-12 6:26 PM
// CreatedBy: Shashank Awasthy
// /*  ----------------------------------------------------------------------------------------------------------------- */

namespace BillingSystem.Controllers
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using BillingSystem.Bal.BusinessAccess;
    using BillingSystem.Common;
    using BillingSystem.Common.Common;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;
    using BillingSystem.Models;

    #endregion

    /// <summary>
    ///     TODO The discharge controller.
    /// </summary>
    public class DischargeController : BaseController
    {
        ///// <summary>
        ///// Get the details of the PatientDischargeSummary View in the Model PatientDischargeSummary such as PatientDischargeSummaryList, list of countries etc.
        ///// </summary>
        ///// <returns>
        ///// returns the actionresult in the form of current object of the Model PatientDischargeSummary to be passed to View PatientDischargeSummary
        ///// </returns>
        // public ActionResult DischargeView(int? patientId)
        // {
        // //Initialize the PatientDischargeSummary BAL object
        // using (var bal = new PatientDischargeSummaryBal())
        // {
        // //Intialize the View Model i.e. PatientDischargeSummaryView which is binded to Main View Index.cshtml under PatientDischargeSummary
        // var dischargeView = new PatientDischargeSummaryView
        // {
        // CurrentPatientDischargeSummary = new PatientDischargeSummary
        // {
        // PatientId = Convert.ToInt32(patientId)
        // },
        // DiagnosisList = new List<DiagnosisCustomModel>(),
        // DischargeMedicationsList = new List<OpenOrderCustomModel>(),
        // ComplicationsList = new List<MedicalNotesCustomModel>(),
        // LabTestsList = new List<MedicalVitalCustomModel>(),
        // MedicationsInHouseList = new List<OpenOrderCustomModel>(),
        // PatientInstructionsList = new List<DropdownListData>(),
        // ProceduresList = new List<OpenOrderCustomModel>()
        // };

        // //Pass the View Model in ActionResult to View PatientDischargeSummary
        // return View(dischargeView);
        // }
        // }

        ///// <summary>
        ///// Bind all the PatientDischargeSummary list
        ///// </summary>
        ///// <returns>
        ///// action result with the partial view containing the PatientDischargeSummary list object
        ///// </returns>
        // [HttpPost]
        // public ActionResult BindPatientDischargeSummaryList()
        // {
        // //Initialize the PatientDischargeSummary BAL object
        // using (var patientDischargeSummaryBal = new PatientDischargeSummaryBal())
        // {
        // //Get the facilities list
        // var patientDischargeSummaryList = patientDischargeSummaryBal.GetPatientDischargeSummary();

        // //Pass the ActionResult with List of PatientDischargeSummaryViewModel object to Partial View PatientDischargeSummaryList
        // return PartialView(PartialViews.PatientDischargeSummaryList, patientDischargeSummaryList);
        // }
        // }

        ///// <summary>
        ///// Add New or Update the PatientDischargeSummary based on if we pass the PatientDischargeSummary ID in the PatientDischargeSummaryViewModel object.
        ///// </summary>
        ///// <param name="model">The model.</param>
        ///// <returns>
        ///// returns the newly added or updated ID of PatientDischargeSummary row
        ///// </returns>
        // public ActionResult SavePatientDischargeSummary(PatientDischargeSummary model)
        // {
        // //Initialize the newId variable 
        // var newId = -1;
        // var userId = Helpers.GetLoggedInUserId();

        // //Check if Model is not null 
        // if (model != null)
        // {
        // using (var bal = new PatientDischargeSummaryBal())
        // {
        // if (model.Id > 0)
        // {
        // model.ModifiedBy = userId;
        // model.Modifieddate = DateTime.Now;
        // }
        // //Call the AddPatientDischargeSummary Method to Add / Update current PatientDischargeSummary
        // newId = bal.SavePatientDischargeSummary(model);
        // }
        // }
        // return Json(newId);
        // }

        ///// <summary>
        ///// Get the details of the current PatientDischargeSummary in the view model by ID 
        ///// </summary>
        ///// <param name="shared">pass the input parameters such as ID</param>
        ///// <returns></returns>
        // public ActionResult GetPatientDischargeSummary(int id)
        // {
        // using (var bal = new PatientDischargeSummaryBal())
        // {
        // //Call the AddPatientDischargeSummary Method to Add / Update current PatientDischargeSummary
        // var currentPatientDischargeSummary = bal.GetPatientDischargeSummaryByID(id);

        // //Pass the ActionResult with the current PatientDischargeSummaryViewModel object as model to PartialView PatientDischargeSummaryAddEdit
        // return PartialView(PartialViews.PatientDischargeSummaryAddEdit, currentPatientDischargeSummary);
        // }
        // }

        ///// <summary>
        ///// Delete the current PatientDischargeSummary based on the PatientDischargeSummary ID passed in the PatientDischargeSummaryModel
        ///// </summary>
        ///// <param name="id">The identifier.</param>
        ///// <returns></returns>
        // public ActionResult DeletePatientDischargeSummary(int id)
        // {
        // using (var bal = new PatientDischargeSummaryBal())
        // {
        // //Get PatientDischargeSummary model object by current PatientDischargeSummary ID
        // var currentPatientDischargeSummary = bal.GetPatientDischargeSummaryByID(id);
        // var userId = Helpers.GetLoggedInUserId();

        // //Check If PatientDischargeSummary model is not null
        // if (currentPatientDischargeSummary != null)
        // {
        // currentPatientDischargeSummary.ModifiedBy = userId;
        // currentPatientDischargeSummary.Modifieddate = DateTime.Now;

        // //Update Operation of current PatientDischargeSummary
        // var result = bal.SavePatientDischargeSummary(currentPatientDischargeSummary);

        // //return deleted ID of current PatientDischargeSummary as Json Result to the Ajax Call.
        // return Json(result);
        // }
        // }

        // //Return the Json result as Action Result back JSON Call Success
        // return Json(null);
        // }

        ///// <summary>
        ///// Reset the PatientDischargeSummary View Model and pass it to PatientDischargeSummaryAddEdit Partial View.
        ///// </summary>
        ///// <returns></returns>
        // public ActionResult ResetPatientDischargeSummaryForm()
        // {
        // //Intialize the new object of PatientDischargeSummary ViewModel
        // var PatientDischargeSummaryViewModel = new Model.PatientDischargeSummary();

        // //Pass the View Model as PatientDischargeSummaryViewModel to PartialView PatientDischargeSummaryAddEdit just to update the AddEdit partial view.
        // return PartialView(PartialViews.PatientDischargeSummaryAddEdit, PatientDischargeSummaryViewModel);
        // }

        /// <summary>
        ///     Get the details of the PatientDischargeSummary View in the Model PatientDischargeSummary such as
        ///     PatientDischargeSummaryList, list of countries etc.
        /// </summary>
        /// <param name="patientId">
        ///     The patient Id.
        /// </param>
        /// <param name="encounterId">
        ///     The encounter Id.
        /// </param>
        /// <returns>
        ///     returns the actionresult in the form of current object of the Model PatientDischargeSummary to be passed to View
        ///     PatientDischargeSummary
        /// </returns>
        public ActionResult DischargePartialView(int? patientId, int? encounterId)
        {
            // Initialize the PatientDischargeSummary BAL object
            using (var bal = new PatientDischargeSummaryBal())
            {
                var daignosisBal = new DiagnosisBal(Helpers.DefaultDiagnosisTableNumber, Helpers.DefaultDrgTableNumber);
                var diagnosislist = daignosisBal.GetDiagnosisList(
                    Convert.ToInt32(patientId),
                    Convert.ToInt32(encounterId));

                if (diagnosislist.Count > 0)
                {
                    diagnosislist = diagnosislist.OrderByDescending(d => d.CreatedDate).ToList();
                }

                var mdeicalNotesBal = new MedicalNotesBal();
                var complicationsList =
                    mdeicalNotesBal.GetMedicalNotesByPatientIdEncounterId(
                        Convert.ToInt32(patientId),
                        Convert.ToInt32(encounterId)).Where(x => x.MedicalNotes.MarkedComplication).ToList();
                var medicalVitalsBal = new MedicalVitalBal();
                var labTestsList = medicalVitalsBal.GetCustomMedicalVitalsByPidEncounterId(
                    Convert.ToInt32(patientId),
                    Convert.ToInt32(MedicalRecordType.Vitals),
                    Convert.ToInt32(encounterId));
                var openorderBal = new OpenOrderBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber);
                var openOrdersList = openorderBal.GetAllOrdersByEncounterId(Convert.ToInt32(encounterId));
                var patientDischargeSummary = bal.GetPatientDischargeSummaryByEncounterId(Convert.ToInt32(patientId));

                var dischargeDetailBal = new DischargeSummaryDetailBal();

                // Intialize the View Model i.e. PatientDischargeSummaryView which is binded to Main View Index.cshtml under PatientDischargeSummary
                var dischargeView = new PatientDischargeSummaryView
                                        {
                                            CurrentPatientDischargeSummary =
                                                patientDischargeSummary,
                                            DiagnosisList = diagnosislist,
                                            DischargeMedicationsList =
                                                openOrdersList.Where(
                                                    x =>
                                                    x.CategoryId
                                                    == Convert.ToInt32(
                                                        OrderTypeCategory.Pharmacy)
                                                    && x.Status
                                                    != Convert.ToInt32(OrderStatus.Open)
                                                           .ToString()).ToList(),
                                            ComplicationsList = complicationsList,
                                            LabTestsList =
                                                new List<MedicalVitalCustomModel>(),
                                            //labTestsList, 
                                            MedicationsInHouseList =
                                                new List<OpenOrderCustomModel>(),
                                            PatientInstructionsList =
                                                new List<DropdownListData>(),
                                            ProceduresList =
                                                openOrdersList.Where(
                                                    x =>
                                                    x.OrderType
                                                    == Convert.ToInt32(OrderType.CPT)
                                                           .ToString()).ToList(),
                                            ActiveMedicalProblemsList =
                                                dischargeDetailBal
                                                .GetDischargeSummaryDetailListByTypeId(
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            GlobalCodeCategoryValue
                                                .ActiveMedicalProblems))),
                                            TypeOfFollowupsList =
                                                dischargeDetailBal
                                                .GetDischargeSummaryDetailListByTypeId(
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            GlobalCodeCategoryValue
                                                .TypesOfFollowup))),
                                            PatientInstructions =
                                                dischargeDetailBal
                                                .GetDischargeSummaryDetailListByTypeId(
                                                    Convert.ToString(
                                                        Convert.ToInt32(
                                                            GlobalCodeCategoryValue
                                                .PatientInstructions)))
                                        };

                // Pass the View Model in ActionResult to View PatientDischargeSummary
                var gccCodes = openorderBal.GetGlobalCodesByCategoriesSp("960,961,962");

                var jsonvariable =
                    new
                        {
                            MedicalProblems = gccCodes.Where(x => x.GlobalCodeCategoryValue == "960").ToList(),
                            PatientInstructions = gccCodes.Where(x => x.GlobalCodeCategoryValue == "961").ToList(),
                            FollowupTypes = gccCodes.Where(x => x.GlobalCodeCategoryValue == "962").ToList(),
                            partialView =
                                this.RenderPartialViewToStringBase(PartialViews.DischargeSummaryTab, dischargeView)
                        };

                //var jsonvariable =
                //   new
                //   {
                //       MedicalProblems = gccCodes.Where(x => x.GlobalCodeCategoryValue == "960").ToList(),
                //       PatientInstructions = gccCodes.Where(x => x.GlobalCodeCategoryValue == "961").ToList(),
                //       FollowupTypes = gccCodes.Where(x => x.GlobalCodeCategoryValue == "962").ToList(),
                //       partialView = this.RenderPartialViewToStringBase(PartialViews.DischargeSummaryTab, dischargeView),
                //       Diagnosisview = this.RenderPartialViewToStringBase(string.Format("../Summary/{0}", PartialViews.EHRDiagnosisList), diagnosislist),
                //       MedicationReceiveInHouseview = this.RenderPartialViewToStringBase(string.Format("../Summary/{0}", PartialViews.DischargeOpenOrderList1), new List<OpenOrderCustomModel>()),
                //       ProceduresPerformedview = this.RenderPartialViewToStringBase(string.Format("../Summary/{0}", PartialViews.DischargeOpenOrderList), openOrdersList.Where(x =>x.OrderType== Convert.ToInt32(OrderType.CPT).ToString()).ToList()),
                //   };

                //return this.PartialView(PartialViews.DischargeSummaryTab, dischargeView);
                return this.Json(jsonvariable, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        ///     TODO The update discharge details.
        /// </summary>
        /// <param name="model">
        ///     TODO The model.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult UpdateDischargeDetails(PatientDischargeSummary model)
        {
            using (var bal = new PatientDischargeSummaryBal())
            {
                var userId = Helpers.GetLoggedInUserId();
                var currentDateTime = Helpers.GetInvariantCultureDateTime();
                if (model.Id == 0)
                {
                    model.CreatedBy = userId;
                    model.CreatedDate = currentDateTime;
                }
                else
                {
                    model.ModifiedBy = userId;
                    model.Modifieddate = currentDateTime;
                }

                var result = bal.SavePatientDischargeSummary(model);
                return this.Json(result);
            }
        }

        /// <summary>
        ///     TODO The add discharge summary detail.
        /// </summary>
        /// <param name="model">
        ///     TODO The model.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult AddDischargeSummaryDetail(DischargeSummaryDetail model)
        {
            using (var bal = new DischargeSummaryDetailBal())
            {
                var result = bal.SaveDischargeSummaryDetail(model);
                switch (model.AssociatedTypeId)
                {
                    case "960":
                        return this.PartialView(PartialViews.ActiveMedicareProblem, result);
                    case "961":
                        return this.PartialView(PartialViews.PatientInstructions, result);
                    case "962":
                        return this.PartialView(PartialViews.FollowsType, result);
                }

                return this.PartialView(PartialViews.DischargeDetailsListView, result);
            }
        }

        /// <summary>
        /// TODO The delete discharge detail.
        /// </summary>
        /// <param name="id">TODO The id.</param>
        /// <param name="typeId">TODO The type id.</param>
        /// <returns>
        /// The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult DeleteDischargeDetail(int id, string typeId)
        {
            using (var bal = new DischargeSummaryDetailBal())
            {
                var result = bal.DeleteDischargeDetail(id, typeId);
                return this.PartialView(PartialViews.DischargeDetailsListView, result);
            }
        }

        /// <summary>
        ///     TODO The check duplicate summary detail.
        /// </summary>
        /// <param name="model">
        ///     TODO The model.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult CheckDuplicateSummaryDetail(DischargeSummaryDetail model)
        {
            using (var bal = new DischargeSummaryDetailBal())
            {
                var result = bal.CheckIfRecordAlreadyAdded(model.AssociatedId, model.AssociatedTypeId);
                return this.Json(result);
            }
        }

        #region To Sort the Medicare Active Problem

        /// <summary>
        ///     TODO The sort medicare active problem.
        /// </summary>
        /// <param name="patientId">
        ///     TODO The patient id.
        /// </param>
        /// <param name="encounterId">
        ///     TODO The encounter id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult SortMedicareActiveProblem(int? patientId, int? encounterId)
        {
            using (var bal = new DischargeSummaryDetailBal())
            {
                var activeMedicalProblemsList =
                    bal.GetDischargeSummaryDetailListByTypeId(
                        Convert.ToString(Convert.ToInt32(GlobalCodeCategoryValue.ActiveMedicalProblems)));

                // Pass the View Model in ActionResult to View PatientDischargeSummary
                return this.PartialView(PartialViews.ActiveMedicareProblem, activeMedicalProblemsList);
            }
        }

        /// <summary>
        ///     TODO The sort follows up.
        /// </summary>
        /// <param name="patientId">
        ///     TODO The patient id.
        /// </param>
        /// <param name="encounterId">
        ///     TODO The encounter id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult SortFollowsUp(int? patientId, int? encounterId)
        {
            using (var bal = new DischargeSummaryDetailBal())
            {
                var followsUpList =
                    bal.GetDischargeSummaryDetailListByTypeId(
                        Convert.ToString(Convert.ToInt32(GlobalCodeCategoryValue.TypesOfFollowup)));

                // Pass the View Model in ActionResult to View PatientDischargeSummary
                return this.PartialView(PartialViews.FollowsType, followsUpList);
            }
        }

        /// <summary>
        ///     TODO The sort patient instruction.
        /// </summary>
        /// <param name="patientId">
        ///     TODO The patient id.
        /// </param>
        /// <param name="encounterId">
        ///     TODO The encounter id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult SortPatientInstruction(int? patientId, int? encounterId)
        {
            using (var bal = new DischargeSummaryDetailBal())
            {
                var patientInstructions =
                    bal.GetDischargeSummaryDetailListByTypeId(
                        Convert.ToString(Convert.ToInt32(GlobalCodeCategoryValue.PatientInstructions)));

                // Pass the View Model in ActionResult to View PatientDischargeSummary
                return this.PartialView(PartialViews.PatientInstructions, patientInstructions);
            }
        }

        /// <summary>
        ///     TODO The sort diagnosis grid.
        /// </summary>
        /// <param name="patientId">
        ///     TODO The patient id.
        /// </param>
        /// <param name="encounterId">
        ///     TODO The encounter id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult SortDiagnosisGrid(int? patientId, int? encounterId)
        {
            var daignosisBal = new DiagnosisBal(Helpers.DefaultDiagnosisTableNumber, Helpers.DefaultDrgTableNumber);
            var diagnosislist = daignosisBal.GetDiagnosisList(Convert.ToInt32(patientId), Convert.ToInt32(encounterId));
            var viewpath = string.Format("../Summary/{0}", PartialViews.EHRDiagnosisList);
            return this.PartialView(viewpath, diagnosislist);
        }

        /// <summary>
        ///     Binds the discharge encounter order list sorted.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="encId">The enc identifier.</param>
        /// <returns></returns>
        public ActionResult BindDischargeEncounterOrderListSorted(int patientId, int encId)
        {
            using (
                var orderBal = new OpenOrderBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var openOrdersList = orderBal.GetAllOrdersByEncounterId(Convert.ToInt32(encId)).ToList();
                openOrdersList =
                    openOrdersList.Where(x => x.OrderType == Convert.ToInt32(OrderType.CPT).ToString()).ToList();
                //orderBal.GetPhysicianOrders(encId, orderStatus);
                var viewpath = string.Format("../Summary/{0}", PartialViews.DischargeOpenOrderList);
                return this.PartialView(viewpath, openOrdersList);
            }
        }

        /// <summary>
        ///     TODO The sort procedures performed grid.
        /// </summary>
        /// <param name="encounterId">
        ///     TODO The encounter id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult SortProceduresPerformedGrid(int? encounterId)
        {
            var openorderBal = new OpenOrderBal(
                Helpers.DefaultCptTableNumber,
                Helpers.DefaultServiceCodeTableNumber,
                Helpers.DefaultDrgTableNumber,
                Helpers.DefaultDrugTableNumber,
                Helpers.DefaultHcPcsTableNumber,
                Helpers.DefaultDiagnosisTableNumber);
            var openOrdersList = openorderBal.GetAllOrdersByEncounterId(Convert.ToInt32(encounterId));
            return this.PartialView(PartialViews.LabOpenOrderList, openOrdersList);
        }

        /// <summary>
        ///     TODO The sort lab test.
        /// </summary>
        /// <param name="patientId">
        ///     TODO The patient id.
        /// </param>
        /// <param name="encounterId">
        ///     TODO The encounter id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult SortLabTest(int? patientId, int? encounterId)
        {
            var medicalVitalsBal = new MedicalVitalBal();
            var labTestsList = medicalVitalsBal.GetCustomMedicalVitalsByPidEncounterId(
                Convert.ToInt32(patientId),
                Convert.ToInt32(MedicalRecordType.Vitals),
                Convert.ToInt32(encounterId));
            return this.PartialView(PartialViews.LabTestList, labTestsList);
        }

        /// <summary>
        ///     Binds the discharge open order by sort.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="encId">The enc identifier.</param>
        /// <returns></returns>
        public ActionResult BindDischargeOpenOrderBySort(int patientId, int encId)
        {
            using (
                var orderBal = new OpenOrderBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var openOrdersList = orderBal.GetAllOrdersByEncounterId(Convert.ToInt32(encId)).ToList();
                openOrdersList =
                    openOrdersList.Where(
                        x =>
                        x.CategoryId == Convert.ToInt32(OrderTypeCategory.Pharmacy)
                        && x.Status != Convert.ToInt32(OrderStatus.Open).ToString()).ToList();
                //orderBal.GetPhysicianOrders(encId, orderStatus);
                var viewpath = string.Format("../Summary/{0}", PartialViews.DischargeOpenOrderList1);
                return this.PartialView(viewpath, openOrdersList);
            }
        }

        /// <summary>
        ///     Binds the discharge medication by sort.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="encId">The enc identifier.</param>
        /// <returns></returns>
        public ActionResult BindDischargeMedicationBySort(int patientId, int encId)
        {
            using (
                var orderBal = new OpenOrderBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var openOrdersList = orderBal.GetAllOrdersByEncounterId(Convert.ToInt32(encId)).ToList();
                openOrdersList =
                    openOrdersList.Where(
                        x =>
                        x.CategoryId == Convert.ToInt32(OrderTypeCategory.Pharmacy)
                        && x.Status != Convert.ToInt32(OrderStatus.Open).ToString()).ToList();
                //orderBal.GetPhysicianOrders(encId, orderStatus);
                var viewpath = string.Format("../Summary/{0}", PartialViews.DischargeMedicationList);
                return this.PartialView(viewpath, openOrdersList);
            }
        }

        #endregion
    }
}