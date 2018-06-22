// /*  ----------------------------------------------------------------------------------------------------------------- */
// To Do: SummaryController.cs
// FileName :SummaryController.cs
// CreatedDate: 2016-05-11 6:56 PM
// ModifiedDate: 2016-05-11 7:10 PM
// CreatedBy: Shashank Awasthy
// /*  ----------------------------------------------------------------------------------------------------------------- */

namespace BillingSystem.Controllers
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Web.Hosting;
    using System.Web.Http;
    using System.Web.Mvc;

    using BillingSystem.Bal.BusinessAccess;
    using BillingSystem.Bal.Mapper;
    using BillingSystem.Common;
    using BillingSystem.Common.Common;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;
    using BillingSystem.Model.Model;
    using BillingSystem.Models;

    using iTextSharp.text;
    using iTextSharp.text.pdf;

    using Microsoft.Ajax.Utilities;

    using PDFTemplateMaster;

    #endregion

    /// <summary>
    ///     The summary controller.
    /// </summary>
    public class SummaryController : BaseController
    {
        //public static string BarCodePath = "/Codes/BarCode/";
        /// <summary>
        ///     The bar code path
        /// </summary>
        private static readonly string BarCodePath = Helpers.BarCodeImagePathForSaving;

        //public JsonResult GetDataByCategories( )
        //{
        //    using (var gbal = new GlobalCodeBal())
        //    {
        //        var categories = new[] { "1024", "3102", "1011" };
        //        var list = gbal.GetListByCategoriesRange(categories);
        //        var jsonData = new
        //        {
        //            listFrequency = list.Where(g => g.ExternalValue1.Equals("1024")).ToList(),
        //            listOrderStatus = list.Where(g => g.ExternalValue1.Equals("3102")).ToList(),
        //            listQuantity = list.Where(g => g.ExternalValue1.Equals("1011")).OrderBy(m => Convert.ToDecimal(m.Value)).ToList(),
        //        };
        //        return Json(jsonData, JsonRequestBehavior.AllowGet);
        //    }
        //}

        /****************************Methods Below NOT IN USE***************************************/

        /// <summary>
        ///     Generates the bar code images.
        /// </summary>
        /// <param name="oGenerateBarCode">The o generate bar code.</param>
        /// <returns></returns>
        public ActionResult GenerateBarCodeImages(GenerateBarCode oGenerateBarCode)
        {
            return CreateCode(oGenerateBarCode);
        }

        /// <summary>
        ///     Creates the code.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <returns></returns>
        public ActionResult CreateCode(GenerateBarCode vm)
        {
            var facilityId = Helpers.GetDefaultFacilityId();
            var currentDateTime = Helpers.GetInvariantCultureDateTime(facilityId);
            var orderBal = new OpenOrderBal(
                Helpers.DefaultCptTableNumber,
                Helpers.DefaultServiceCodeTableNumber,
                Helpers.DefaultDrgTableNumber,
                Helpers.DefaultDrugTableNumber,
                Helpers.DefaultHcPcsTableNumber,
                Helpers.DefaultDiagnosisTableNumber);
            var patientInfo = orderBal.GetPatientDetailsByPatientId(Convert.ToInt32(vm.PatientId));
            vm.PatientName = patientInfo.PatientName;
            vm.Age = Convert.ToString(patientInfo.PersonAge);
            vm.DateOfBirth = Convert.ToDateTime(patientInfo.PatientInfo.PersonBirthDate).ToString("dd/MM/yyyy HH:mm:ss");
            vm.Gender = patientInfo.PatientInfo.PersonGender;
            vm.BarCodeNumbering = patientInfo.CurrentEncounter.EncounterNumber;
            vm.Site = string.Empty;
            //vm.CollectionDateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            vm.CollectionDateTime = currentDateTime.ToString("dd/MM/yyyy HH:mm:ss");
            vm.UserId = Helpers.GetLoggedInUserId();
            vm.EncounterId = patientInfo.CurrentEncounter.EncounterID;
            vm.PatientEncounterId = vm.PatientId + Convert.ToString(patientInfo.CurrentEncounter.EncounterID);
            var imageName = vm.PatientId + "_" + vm.EncounterId + "_" + vm.PatientEncounterId + "_"
                            + vm.BarCodeNumbering + ".png";
            var path = Server.MapPath("~") + BarCodePath + imageName;

            GenerateCode.GenerateBarCode(vm.BarCodeReadValue, path);

            vm.BarCode = BarCodePath + imageName;
            vm.BarCodeReadValue = vm.PatientId + "`" + Convert.ToString(patientInfo.CurrentEncounter.EncounterID) + "`"
                                  + patientInfo.CurrentEncounter.EncounterNumber;

            return PartialView(PartialViews.BarCodeView, vm);
        }
        /****************************Methods Above NOT IN USE***************************************/

        #region  Medical Vitals Tab

        /// <summary>
        ///     Binds the medical vitals tab data.
        /// </summary>
        /// <param name="patientId">
        ///     The patient identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindMedicalVitalsTabData(int patientId)
        {
            using (var medicalVitals = new MedicalVitalBal())
            {
                var medicalvitals = medicalVitals.GetCustomMedicalVitals(patientId, (int)MedicalRecordType.Vitals);

                var medicalvitalsview = new MedicalVitalView
                {
                    CurrentMedicalVital = new MedicalVitalCustomModel(),
                    MedicalVitalList = medicalvitals
                };
                return PartialView(PartialViews.VitalsTabView, medicalvitalsview);
            }
        }

        #endregion

        #region Physician Tab

        /// <summary>
        ///     Binds the lab orders list by physician.
        /// </summary>
        /// <param name="orderType">
        ///     Type of the order.
        /// </param>
        /// <param name="orderStatus">
        ///     The order status.
        /// </param>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindLabOrdersListByPhysician(string orderType, int orderStatus, int encounterId)
        {
            using (
                var bal = new OrderActivityBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var list = bal.GetLabOrderActivitiesByPhysician(
                    Helpers.GetLoggedInUserId(),
                    orderStatus,
                    orderType,
                    1,
                    encounterId);
                list = list != null ? list.OrderBy(x => x.LabResultTypeStr).ToList() : null;
                return PartialView(PartialViews.PhysicianLabTestView, list);
            }
        }


        /// <summary>
        ///     Binds the medical notes data.
        /// </summary>
        /// <param name="patientId">
        ///     The patient identifier.
        /// </param>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="encounterid">
        ///     The encounterid.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindAllPhysicianNurseTaskTabs(int patientId, int type, int encounterid)
        {
            using (var medicalnotesbal = new MedicalNotesBal())
            {
                using (
                    var orderbal = new OpenOrderBal(
                        Helpers.DefaultCptTableNumber,
                        Helpers.DefaultServiceCodeTableNumber,
                        Helpers.DefaultDrgTableNumber,
                        Helpers.DefaultDrugTableNumber,
                        Helpers.DefaultHcPcsTableNumber,
                        Helpers.DefaultDiagnosisTableNumber))
                {
                    var enBal = new EncounterBal();
                    var encounterListData = enBal.GetPreActiveEncounters(encounterid, patientId);
                    var nurseAssessmentfrom = enBal.GetNurseAssessmentData(encounterid, patientId);

                    var notesList = medicalnotesbal.GetCustomMedicalNotes(
                        patientId,
                        type == Convert.ToInt32(NotesUserType.Physician)
                            ? Convert.ToInt32(NotesUserType.Physician)
                            : Convert.ToInt32(NotesUserType.Nurse));
                    var openOrdersList = orderbal.GetPhysicianOrders(encounterid, OrderStatus.Open.ToString());
                    var closedOrdersList = orderbal.GetPhysicianOrders(encounterid, OrderStatus.Closed.ToString());
                    using (
                        var orderActivityBal = new OrderActivityBal(
                            Helpers.DefaultCptTableNumber,
                            Helpers.DefaultServiceCodeTableNumber,
                            Helpers.DefaultDrgTableNumber,
                            Helpers.DefaultDrugTableNumber,
                            Helpers.DefaultHcPcsTableNumber,
                            Helpers.DefaultDiagnosisTableNumber))
                    {
                        // var activitesobj = orderActivityBal.GetOrderActivitiesByEncounterId(encounterid);
                        var opNurseAssessmentList = new DocumentsTemplatesBal().GetNurseDocuments(
                            patientId,
                            encounterid);
                        var activitesobj1 = orderActivityBal.GetPCActivitiesByEncounterId(encounterid);
                        var activitesListObj = activitesobj1;

                        // activitesobj.Where(x => x.OrderCategoryID != Convert.ToInt32(OrderTypeCategory.PathologyandLaboratory))
                        // .ToList();
                        var activitesClosedListObj =
                            activitesListObj.Where(
                                x =>
                                x.OrderActivityStatus != 0
                                && x.OrderActivityStatus != Convert.ToInt32(OpenOrderActivityStatus.Open)
                                && x.OrderActivityStatus
                                != Convert.ToInt32(OpenOrderActivityStatus.PartiallyExecutedForResult))
                                .OrderBy(x => x.ExecutedDate)
                                .ToList();
                        var activitesOpenListObj =
                            activitesListObj.Where(
                                x =>
                                x.OrderActivityStatus == 0
                                || x.OrderActivityStatus == Convert.ToInt32(OpenOrderActivityStatus.Open)
                                || x.OrderActivityStatus
                                == Convert.ToInt32(OpenOrderActivityStatus.PartiallyExecutedForResult))
                                .OrderBy(x => x.OrderScheduleDate)
                                .ToList();
                        var medicalVitals = new MedicalVitalBal();
                        var medicalvitals = medicalVitals.GetCustomMedicalVitals(
                            patientId,
                            Convert.ToInt32(MedicalRecordType.Vitals));

                        var medicalnotesview = new MedicalNotesView
                        {
                            CurrentMedicalNotes = new MedicalNotes
                            {
                                NotesUserType = type,
                                IsDeleted = false
                            },
                            MedicalNotesList = notesList,
                            OpenOrdersList = openOrdersList,
                            ClosedOrdersList = closedOrdersList,
                            OpenActvitiesList = activitesOpenListObj,
                            ClosedActvitiesList = activitesClosedListObj,
                            ClosedLabOrdersActivitesList = new List<OrderActivityCustomModel>(),
                            LabOpenOrdersActivitesList = new List<OrderActivityCustomModel>(),
                            EncounterOrder = new OpenOrder(),
                            IsLabTest = false,
                            CurrentMedicalVital =
                                                           new MedicalVitalCustomModel(),
                            MedicalVitalList = medicalvitals,
                            PatientInfoId = patientId,
                            PatientEncounterId = encounterid,
                            EncounterList = encounterListData,
                            NurseEnteredFormList =
                                                           opNurseAssessmentList.ToList(),
                            NurseDocList = nurseAssessmentfrom
                        };
                        return PartialView(PartialViews.NotesTabView, medicalnotesview);
                    }
                }
            }
        }

        #endregion

        #region Medical History Allergy

        /// <summary>
        ///     Binds the medical history allergy data.
        /// </summary>
        /// <param name="patientId">
        ///     The patient identifier.
        /// </param>
        /// <param name="encounterid">
        ///     The encounterid.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindMedicalHistoryAllergyData(int patientId, int encounterid)
        {
            var mhBal = new MedicalHistoryBal(Helpers.DefaultDrugTableNumber);
            //var gccBal = new GlobalCodeCategoryBal();
            //var gcBal = new GlobalCodeBal();
            //var gcList = new List<GlobalCodes>();
            //var aList = new List<GlobalCodes>();
            //var mrBal = new MedicalRecordBal();


            //// Get the Entity list
            //var mhList = mhBal.GetMedicalHistory(patientId, encounterid);

            //// Intialize the View Model i.e. MedicalHistoryView which is binded to Main View Index.cshtml under MedicalHistory
            //var mhView = new MedicalHistoryView
            //{
            //    MedicalHistoryList = mhList,
            //    CurrentMedicalHistory = new MedicalHistory()
            //};



            //var gccList = gccBal.GetGlobalCodeCategoriesRange((int)GlobalCodeCategoryValue.AlergyStartRange
            //                , (int)GlobalCodeCategoryValue.AlergyEndRange);


            //foreach (var item in gccList)
            //{
            //    gcList = gcBal.GetGCodesListByCategoryValue(item.GlobalCodeCategoryValue);
            //    aList.AddRange(gcList);
            //}

            //var objAlergyView = new AlergyView
            //{
            //    AlergyList = mrBal.GetAlergyRecords(patientId, Convert.ToInt32(MedicalRecordType.Allergies)),
            //    AllergiesHistoriesGCC = gccList,
            //    AllergiesHistoryGC = aList,
            //    MedicalHistoryView = mhView
            //};
            var result = mhBal.GetAlergyAndMedicalHistorDataOnLoad(patientId, Helpers.GetLoggedInUserId(), encounterid);
            return PartialView(PartialViews.AllergiesHistoryView, result);
        }

        #endregion

        #region BarCodeBlock

        /// <summary>
        ///     Creates the code in lab specimen.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <returns></returns>
        public GenerateBarCode CreateCodeInLabSpecimen(GenerateBarCode vm)
        {
            var facilityId = Helpers.GetDefaultFacilityId();
            var currentDateTime = Helpers.GetInvariantCultureDateTime(facilityId);
            var orderBal = new OpenOrderBal(
                Helpers.DefaultCptTableNumber,
                Helpers.DefaultServiceCodeTableNumber,
                Helpers.DefaultDrgTableNumber,
                Helpers.DefaultDrugTableNumber,
                Helpers.DefaultHcPcsTableNumber,
                Helpers.DefaultDiagnosisTableNumber);
            var patientInfo = orderBal.GetPatientDetailsByPatientId(Convert.ToInt32(vm.PatientId), vm.EncounterId, vm.EncounterId > 0);
            vm.PatientName = patientInfo.PatientName;
            vm.Age = Convert.ToString(patientInfo.PersonAge) + "y";
            vm.DateOfBirth = Convert.ToDateTime(patientInfo.PatientInfo.PersonBirthDate).ToString("dd/MM/yyyy HH:mm:ss");
            vm.Gender = patientInfo.PatientInfo.PersonGender;
            vm.BarCodeNumbering = patientInfo.CurrentEncounter.EncounterNumber;
            vm.Site = string.Empty;
            //vm.CollectionDateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            vm.CollectionDateTime = currentDateTime.ToString("dd/MM/yyyy HH:mm:ss");
            vm.UserId = Helpers.GetLoggedInUserId();
            vm.EncounterId = patientInfo.CurrentEncounter.EncounterID;
            vm.PatientEncounterId = vm.PatientId + Convert.ToString(patientInfo.CurrentEncounter.EncounterID);

            var imageName = vm.PatientId + "_" + vm.EncounterId + "_" + vm.PatientEncounterId + "_"
                            + vm.BarCodeNumbering + "_" + vm.OrderActivityId + "_"
                            + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", currentDateTime) + ".png";
            vm.BarCode = BarCodePath + imageName;
            vm.BarCodeReadValue = vm.PatientId + "`" + Convert.ToString(patientInfo.CurrentEncounter.EncounterID) + "`"
                                  + patientInfo.CurrentEncounter.EncounterNumber + "`" + vm.OrderActivityId;
            var path = Server.MapPath("~") + BarCodePath + imageName;
            GenerateCode.GenerateBarCode(vm, path);

            //vm.BarCodeHtml = RenderPartialViewToString(PartialViews.BarCodeView, vm);
            vm.BarCodeHtml = BarCodePath + imageName;
            return vm;
        }

        #endregion

        #region EHR Performance Events

        /// <summary>
        ///     P_s the orders view data.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public ActionResult P_OrdersViewData(string encounterId)
        {
            var userId = Helpers.GetLoggedInUserId();
            var corporateid = Helpers.GetSysAdminCorporateID();
            var facilityid = Helpers.GetDefaultFacilityId();
            var orderBal = new OpenOrderBal(
                Helpers.DefaultCptTableNumber,
                Helpers.DefaultServiceCodeTableNumber,
                Helpers.DefaultDrgTableNumber,
                Helpers.DefaultDrugTableNumber,
                Helpers.DefaultHcPcsTableNumber,
                Helpers.DefaultDiagnosisTableNumber);
            var mostRecentOrders = orderBal.GetMostOrderedList(userId, 100);
            var allPhysicianOrders = orderBal.GetOrdersByPhysician(userId, corporateid, facilityid);

            // var favoriteOrders = orderBal.GetFavoriteOrders(userId);
            var favoriteOrders = orderBal.GetPhysicanFavoriteOrderedList(userId, facilityid, corporateid);

            var allEncounterOrders = orderBal.GetAllOrdersByEncounterId(Convert.ToInt32(encounterId));
            var closedOrdersList = new List<OpenOrderCustomModel>();
            var openOrderList = new List<OpenOrderCustomModel>();

            var closedOrderActivityList = new List<OrderActivityCustomModel>();
            var openOrderActivityList = new List<OrderActivityCustomModel>();

            using (
                var orderActivityBal = new OrderActivityBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var encounterActivitesobj = orderActivityBal.GetOrderActivitiesByEncounterIdSP(Convert.ToInt32(encounterId));
                var encounterActivitesClosedListObj =
                    encounterActivitesobj.Where(
                        x =>
                        x.OrderActivityStatus != 0
                        && x.OrderActivityStatus != Convert.ToInt32(OpenOrderActivityStatus.Open)
                        && x.OrderActivityStatus != Convert.ToInt32(OpenOrderActivityStatus.LabSectionWaitingForResult)
                        && x.OrderActivityStatus
                        != Convert.ToInt32(OpenOrderActivityStatus.LabSectionWaitingForSpecimen)
                        && x.OrderActivityStatus != Convert.ToInt32(OpenOrderActivityStatus.PartiallyExecutedForResult))
                        .OrderBy(x => x.ExecutedDate)
                        .ToList();
                var encounterActivitesOpenListObj =
                    encounterActivitesobj.Where(
                        x =>
                        x.OrderActivityStatus == 0
                        || x.OrderActivityStatus == Convert.ToInt32(OpenOrderActivityStatus.Open)
                        || x.OrderActivityStatus == Convert.ToInt32(OpenOrderActivityStatus.LabSectionWaitingForResult)
                        || x.OrderActivityStatus
                        == Convert.ToInt32(OpenOrderActivityStatus.LabSectionWaitingForSpecimen)
                        || x.OrderActivityStatus == Convert.ToInt32(OpenOrderActivityStatus.PartiallyExecutedForResult))
                        .OrderBy(x => x.OrderScheduleDate)
                        .ToList();
                closedOrderActivityList = encounterActivitesClosedListObj;
                openOrderActivityList = encounterActivitesOpenListObj;
            }

            if (allEncounterOrders.Count > 0)
            {
                openOrderList = allEncounterOrders.Where(a => a.OrderStatus.Equals("1")).ToList();
                closedOrdersList =
                    allEncounterOrders.Where(
                        a =>
                        a.OrderStatus.Equals("3") || a.OrderStatus.Equals("4") || a.OrderStatus.Equals("2")
                        || a.OrderStatus.Equals("9")).ToList();
            }
            var patientId = orderBal.GetPatientIdByEncounterId(Convert.ToInt32(encounterId));
            var futureOrdersList =
                new FutureOpenOrderBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber).GetFutureOpenOrderByPatientId(Convert.ToInt32(patientId));

            var ordersFullView = new OrdersFullView
            {
                AllPhysicianOrders =
                                             allPhysicianOrders.DistinctBy(x => x.OrderCode).ToList(),
                ClosedOrdersList = closedOrdersList,
                EncounterOrder =
                                             new OpenOrder
                                             {
                                                 StartDate =
                                                         Helpers.GetInvariantCultureDateTime(),
                                                 EndDate =
                                                         Helpers.GetInvariantCultureDateTime(),
                                                 OrderStatus =
                                                         Convert.ToInt32(OrderStatus.Open)
                                                         .ToString()
                                             },
                FavoriteOrders = favoriteOrders,
                MostRecentOrders = mostRecentOrders,
                OpenOrdersList = openOrderList,
                SearchedOrders = new List<OpenOrderCustomModel>(),
                ClosedOrderActivityList = closedOrderActivityList,
                OpenOrderActivityList = openOrderActivityList,
                CurrentOrderActivity = new OrderActivity(),
                FutureOpenOrdersList = futureOrdersList
            };
            return PartialView(PartialViews.OrdersFullView, ordersFullView);
        }

        #endregion

        #region PDF related properties

        public List<string> PdfUrlList { get; set; }

        public string PdfBrowserurl { get; set; }

        public List<PdfControlList> ControlList { get; set; }

        public string FileName { get; set; }

        private readonly string _pdfurl = string.Empty;

        private int _Width = 0;

        private int _Height = 0;

        private byte[] _bytes;

        private readonly bool _print = false;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds the order.
        /// </summary>
        /// <param name="order">
        ///     The order.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        /// <summary>
        ///     Get the Patient Patient Summary
        /// </summary>
        /// <param name="order">
        ///     The order.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult AddPhysicianOrder1(OpenOrder order)
        {
            using (
                var encounterComm = new OpenOrderBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var userId = Helpers.GetLoggedInUserId();
                var corporateId = Helpers.GetSysAdminCorporateID();
                var facilityId = Helpers.GetDefaultFacilityId();
                var days = (Convert.ToDateTime(order.EndDate) - Convert.ToDateTime(order.StartDate)).TotalDays;
                //var periodDays = days <= 0 ? 1.0 : days + 1;
                var periodDays = days + 1;
                order.PeriodDays = Convert.ToString(periodDays);
                order.FacilityID = facilityId;
                order.CorporateID = corporateId;
                if (order.OrderStatus == Convert.ToString((int)OrderStatus.Closed)) order.IsActivitySchecduled = true;
                else order.IsApproved = order.CategoryId != Convert.ToInt32(OrderTypeCategory.Pharmacy);

                if (order.OpenOrderID > 0)
                {
                    order.ModifiedBy = userId;
                    order.PhysicianID = userId;
                    order.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                    order.OpenOrderPrescribedDate = Helpers.GetInvariantCultureDateTime();

                    // order.CreatedBy = userId;
                    // order.CreatedDate = Helpers.GetInvariantCultureDateTime();
                }
                else
                {
                    order.CreatedBy = userId;
                    order.CreatedDate = Helpers.GetInvariantCultureDateTime();
                    order.PhysicianID = userId;
                    order.OpenOrderPrescribedDate = Helpers.GetInvariantCultureDateTime();
                    order.IsActivitySchecduled = null;
                    order.ActivitySchecduledOn = null;

                    // order.EncounterID = EncounterId;
                }

                var orderId = encounterComm.AddUpdatePhysicianOpenOrder(order);
                return Json(orderId);
            }
        }

        /// <summary>
        ///     Administers the care plan activity.
        /// </summary>
        /// <param name="careplanActivityId">
        ///     The careplan activity identifier.
        /// </param>
        /// <param name="encounterid">
        ///     The encounterid.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult AdministerCarePlanActivity(int careplanActivityId, int encounterid)
        {
            var carePlanActivityBal = new PatientCareActivitiesBal();
            var isUpdated = carePlanActivityBal.AddUptdatePatientCareActivity(
                careplanActivityId,
                Helpers.GetDefaultFacilityId(),
                "3");

            // ... Send 3 as status for Administring the activity
            if (isUpdated != -1)
            {
                var getClosedOrderActivities = new OrderActivityBal().GetPCClosedActivitiesByEncounterId(encounterid);
                return PartialView(PartialViews.OrderClosedActivityScheduleList, getClosedOrderActivities);
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     Cancels the care plan activity.
        /// </summary>
        /// <param name="careplanActivityId">The careplan activity identifier.</param>
        /// <param name="encounterid">The encounterid.</param>
        /// <returns></returns>
        public ActionResult CancelCarePlanActivity(int careplanActivityId, int encounterid)
        {
            var carePlanActivityBal = new PatientCareActivitiesBal();
            var isUpdated = carePlanActivityBal.AddUptdatePatientCareActivity(
                careplanActivityId,
                Helpers.GetDefaultFacilityId(),
                "9");

            // ... Send 9 as status for canceling the activity
            if (isUpdated != -1)
            {
                var getClosedOrderActivities = new OrderActivityBal().GetPCClosedActivitiesByEncounterId(encounterid);
                return PartialView(PartialViews.OrderClosedActivityScheduleList, getClosedOrderActivities);
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     Binds the closed activity list.
        /// </summary>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindClosedActivityList(int encounterId)
        {
            using (
                var bal = new OrderActivityBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var list =
                    bal.GetOrderActivitiesByEncounterId(encounterId)
                        .Where(
                            x =>
                            x.OrderActivityStatus != 0
                            && x.OrderActivityStatus != Convert.ToInt32(OpenOrderActivityStatus.Open))
                        .OrderBy(x => x.ExecutedDate)
                        .ToList();
                return PartialView(PartialViews.OrderClosedActivityScheduleList, list);
            }
        }

        /// <summary>
        ///     Binds the closed orders.
        /// </summary>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindClosedOrders(int encounterId)
        {
            using (
                var bal = new OpenOrderBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var list = bal.GetPhysicianOrders(encounterId, OrderStatus.Closed.ToString())
                    .OrderBy(x => x.OpenOrderID);
                return PartialView(PartialViews.ClosedOrdersList, list);
            }
        }

        /// <summary>
        ///     Binds the encounter closed activity list.
        /// </summary>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindEncounterClosedActivityList(int encounterId, int type)
        {
            using (
                var bal = new OrderActivityBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var list =
                    bal.GetOrderActivitiesByEncounterId(encounterId)
                        .Where(
                            x =>
                            x.OrderActivityStatus != 0
                            && x.OrderActivityStatus != Convert.ToInt32(OpenOrderActivityStatus.Open)
                            && x.OrderActivityStatus
                            != Convert.ToInt32(OpenOrderActivityStatus.LabSectionWaitingForResult)
                            && x.OrderActivityStatus
                            != Convert.ToInt32(OpenOrderActivityStatus.LabSectionWaitingForSpecimen)
                            && x.OrderActivityStatus
                            != Convert.ToInt32(OpenOrderActivityStatus.PartiallyExecutedForResult))
                        .OrderBy(x => x.ExecutedDate)
                        .ToList();
                list = type == 0 ? list : list.Where(x => x.OrderCategoryID == type).ToList();
                return
                    PartialView(
                        type == Convert.ToInt32(OrderTypeCategory.PathologyandLaboratory)
                            ? PartialViews.LabClosedActivtiesList
                            : PartialViews.OrderClosedActivityScheduleList,
                        list);
            }
        }

        /// <summary>
        ///     Binds the encounter closed activity pc list.
        /// </summary>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindEncounterClosedActivityPCList(int encounterId, int type)
        {
            using (
                var bal = new OrderActivityBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                // var activitesobj1 = orderActivityBal.GetPCActivitiesByEncounterId(encounterid);
                var list =
                    bal.GetPCActivitiesByEncounterId(encounterId)
                        .Where(
                            x =>
                            x.OrderActivityStatus != 0
                            && x.OrderActivityStatus != Convert.ToInt32(OpenOrderActivityStatus.Open)
                            && x.OrderActivityStatus
                            != Convert.ToInt32(OpenOrderActivityStatus.LabSectionWaitingForResult)
                            && x.OrderActivityStatus
                            != Convert.ToInt32(OpenOrderActivityStatus.LabSectionWaitingForSpecimen)
                            && x.OrderActivityStatus
                            != Convert.ToInt32(OpenOrderActivityStatus.PartiallyExecutedForResult))
                        .OrderBy(x => x.ExecutedDate)
                        .ToList();
                list = type == 0 ? list : list.Where(x => x.OrderCategoryID == type).ToList();
                return
                    PartialView(
                        type == Convert.ToInt32(OrderTypeCategory.PathologyandLaboratory)
                            ? PartialViews.LabClosedActivtiesList
                            : PartialViews.OrderClosedActivityScheduleList,
                        list);
            }
        }

        /// <summary>
        ///     Binds the type of the encounter closed orders by.
        /// </summary>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindEncounterClosedOrdersByType(int encounterId, int type)
        {
            using (
                var bal = new OpenOrderBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var list =
                    bal.GetPhysicianOrders(encounterId, OrderStatus.Closed.ToString())
                        .OrderBy(x => x.OpenOrderID)
                        .ToList();
                list = type == 0 ? list : list.Where(x => x.CategoryId == type).ToList();
                if (type == 11080)
                {
                    return PartialView(PartialViews.LabClosedOrderList, list);
                }

                return PartialView(PartialViews.ClosedOrdersList, list);
            }
        }

        /// <summary>
        ///     Binds the encounter diagnosis list sorted.
        /// </summary>
        /// <param name="sort">
        ///     The sort.
        /// </param>
        /// <param name="sortdir">
        ///     The sortdir.
        /// </param>
        /// <param name="patientId">
        ///     The patient identifier.
        /// </param>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindEncounterDiagnosisListSorted(
            string sort,
            string sortdir,
            int patientId,
            int encounterId)
        {
            List<DiagnosisCustomModel> listOfOrders;
            using (var dbal = new DiagnosisBal(Helpers.DefaultDiagnosisTableNumber, Helpers.DefaultDrgTableNumber)) listOfOrders = dbal.GetDiagnosisList(patientId, encounterId);
            if (listOfOrders.Count > 0)
            {
                listOfOrders = listOfOrders.OrderBy(string.Format("{0} {1}", sort, sortdir)).ToList();
            }

            return PartialView("../Diagnosis/" + PartialViews.DiagnosisList, listOfOrders);
        }

        /// <summary>
        ///     Binds the encounter pharmacy order list.
        /// </summary>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindEncounterLabOrderList(string encounterId)
        {
            var encounterIdint = Convert.ToInt32(encounterId);
            var encounterOrderbal = new OpenOrderBal(
                Helpers.DefaultCptTableNumber,
                Helpers.DefaultServiceCodeTableNumber,
                Helpers.DefaultDrgTableNumber,
                Helpers.DefaultDrugTableNumber,
                Helpers.DefaultHcPcsTableNumber,
                Helpers.DefaultDiagnosisTableNumber);
            var status = OrderStatus.Open.ToString();
            var listOfOrders =
                encounterOrderbal.GetPhysicianOrders(encounterIdint, status)
                    .Where(x => x.CategoryId == Convert.ToInt32(OrderTypeCategory.PathologyandLaboratory))
                    .OrderBy(x => x.OpenOrderID)
                    .ToList();
            return PartialView(PartialViews.PhysicianOpenOrderList, listOfOrders);
        }

        /// <summary>
        ///     Binds the encounter open activity list.
        /// </summary>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindEncounterOpenActivityList(int encounterId, int type)
        {
            using (
                var bal = new OrderActivityBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var list =
                    bal.GetOrderActivitiesByEncounterId(encounterId)
                        .Where(
                            x =>
                            x.OrderActivityStatus == 0
                            || x.OrderActivityStatus == Convert.ToInt32(OpenOrderActivityStatus.Open)
                            || x.OrderActivityStatus
                            == Convert.ToInt32(OpenOrderActivityStatus.LabSectionWaitingForResult)
                            || x.OrderActivityStatus
                            == Convert.ToInt32(OpenOrderActivityStatus.LabSectionWaitingForSpecimen)
                            || x.OrderActivityStatus
                            == Convert.ToInt32(OpenOrderActivityStatus.PartiallyExecutedForResult))
                        .OrderBy(x => x.OrderScheduleDate)
                        .ToList();
                list = type == 0 ? list : list.Where(x => x.OrderCategoryID == type).ToList();
                return
                    PartialView(
                        type == Convert.ToInt32(OrderTypeCategory.PathologyandLaboratory)
                            ? PartialViews.LabOpenActivtiesList
                            : PartialViews.OrderActivityScheduleList,
                        list);
            }
        }

        /// <summary>
        ///     Binds the encounter open activity pc list.
        /// </summary>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindEncounterOpenActivityPCList(int encounterId, int type)
        {
            using (
                var bal = new OrderActivityBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var list =
                    bal.GetPCActivitiesByEncounterId(encounterId)
                        .Where(
                            x =>
                            x.OrderActivityStatus == 0
                            || x.OrderActivityStatus == Convert.ToInt32(OpenOrderActivityStatus.Open)
                            || x.OrderActivityStatus
                            == Convert.ToInt32(OpenOrderActivityStatus.LabSectionWaitingForResult)
                            || x.OrderActivityStatus
                            == Convert.ToInt32(OpenOrderActivityStatus.LabSectionWaitingForSpecimen)
                            || x.OrderActivityStatus
                            == Convert.ToInt32(OpenOrderActivityStatus.PartiallyExecutedForResult))
                        .OrderBy(x => x.OrderScheduleDate)
                        .ToList();
                list = type == 0 ? list : list.Where(x => x.OrderCategoryID == type).ToList();
                return
                    PartialView(
                        type == Convert.ToInt32(OrderTypeCategory.PathologyandLaboratory)
                            ? PartialViews.LabOpenActivtiesList
                            : PartialViews.OrderActivityScheduleList,
                        list);
            }
        }

        /// <summary>
        ///     Binds the type of the encounter open orders by.
        /// </summary>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindEncounterOpenOrdersByType(int encounterId, int type)
        {
            using (
                var bal = new OpenOrderBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var list =
                    bal.GetPhysicianOrders(encounterId, OrderStatus.Open.ToString())
                        .OrderBy(x => x.OpenOrderID)
                        .ToList();
                list = type == 0 ? list : list.Where(x => x.CategoryId == type).ToList();
                if (type == 11080)
                {
                    return PartialView(PartialViews.LabOpenOrderList, list);
                }

                return PartialView(PartialViews.PhysicianOpenOrderList, list);
            }
        }

        /// <summary>
        ///     Binds the encounter order list.
        /// </summary>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindEncounterOrderList(string encounterId)
        {
            var encounterIdint = Convert.ToInt32(encounterId);
            var encounterOrderbal = new OpenOrderBal(
                Helpers.DefaultCptTableNumber,
                Helpers.DefaultServiceCodeTableNumber,
                Helpers.DefaultDrgTableNumber,
                Helpers.DefaultDrugTableNumber,
                Helpers.DefaultHcPcsTableNumber,
                Helpers.DefaultDiagnosisTableNumber);
            var status = Convert.ToInt32(OrderStatus.Open).ToString();
            var listOfOrders = encounterOrderbal.GetOrdersByEncounterid(encounterIdint).ToList();
            listOfOrders = listOfOrders.Where(x => x.OrderStatus == status || x.OrderStatus == "0").ToList();
            return PartialView(PartialViews.PhysicianOpenOrderList, listOfOrders);
        }

        /// <summary>
        ///     Binds the encounter order list sorted.
        /// </summary>
        /// <param name="sort">
        ///     The sort.
        /// </param>
        /// <param name="sortdir">
        ///     The sortdir.
        /// </param>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindEncounterOrderListSorted(string sort, string sortdir, int encounterId, int type)
        {
            var encounterOrderbal = new OpenOrderBal(
                Helpers.DefaultCptTableNumber,
                Helpers.DefaultServiceCodeTableNumber,
                Helpers.DefaultDrgTableNumber,
                Helpers.DefaultDrugTableNumber,
                Helpers.DefaultHcPcsTableNumber,
                Helpers.DefaultDiagnosisTableNumber);
            var listOfOrders = encounterOrderbal.GetPhysicianOrders(encounterId, OrderStatus.Open.ToString());
            listOfOrders = type == 0 ? listOfOrders : listOfOrders.Where(x => x.CategoryId == type).ToList();
            if (listOfOrders.Count > 0)
            {
                listOfOrders = listOfOrders.OrderBy(string.Format("{0} {1}", sort, sortdir)).ToList();
            }

            return PartialView(PartialViews.PhysicianOpenOrderList, listOfOrders);
        }

        /// <summary>
        ///     Binds the encounter pharmacy order list.
        /// </summary>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindEncounterPharmacyOrderList(string encounterId)
        {
            var encounterIdint = Convert.ToInt32(encounterId);
            var encounterOrderbal = new OpenOrderBal(
                Helpers.DefaultCptTableNumber,
                Helpers.DefaultServiceCodeTableNumber,
                Helpers.DefaultDrgTableNumber,
                Helpers.DefaultDrugTableNumber,
                Helpers.DefaultHcPcsTableNumber,
                Helpers.DefaultDiagnosisTableNumber);
            var status = OrderStatus.Open.ToString();
            var listOfOrders =
                encounterOrderbal.GetPhysicianOrders(encounterIdint, status)
                    .Where(x => x.CategoryId == Convert.ToInt32(OrderTypeCategory.Pharmacy))
                    .OrderBy(x => x.OpenOrderID)
                    .ToList();
            return PartialView(PartialViews.PhysicianOpenOrderList, listOfOrders);
        }

        /// <summary>
        ///     Binds the encounter pharmacy order list.
        /// </summary>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindEncounterRadOrderList(string encounterId)
        {
            var encounterIdint = Convert.ToInt32(encounterId);
            var encounterOrderbal = new OpenOrderBal(
                Helpers.DefaultCptTableNumber,
                Helpers.DefaultServiceCodeTableNumber,
                Helpers.DefaultDrgTableNumber,
                Helpers.DefaultDrugTableNumber,
                Helpers.DefaultHcPcsTableNumber,
                Helpers.DefaultDiagnosisTableNumber);
            var status = OrderStatus.Open.ToString();
            var listOfOrders =
                encounterOrderbal.GetPhysicianOrders(encounterIdint, status)
                    .Where(x => x.CategoryId == Convert.ToInt32(OrderTypeCategory.Radiology))
                    .OrderBy(x => x.OpenOrderID)
                    .ToList();
            return PartialView(PartialViews.PhysicianOpenOrderList, listOfOrders);
        }

        /// <summary>
        ///     Binds the medical notes data.
        /// </summary>
        /// <param name="patientId">
        ///     The patient identifier.
        /// </param>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="encounterid">
        ///     The encounterid.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindMedicalNotesData(int patientId, int type, int encounterid)
        {
            var openOrdersList = new List<OpenOrderCustomModel>();
            var closedOrdersList = new List<OpenOrderCustomModel>();
            var openActivities = new List<OrderActivityCustomModel>();
            var closedActivities = new List<OrderActivityCustomModel>();



            //Get Encounters List and Nurse Assessment Data
            var enBal = new EncounterBal();
            var encounterListData = enBal.GetPreActiveEncounters(encounterid, patientId);
            var nurseAssessmentfrom = enBal.GetNurseAssessmentData(encounterid, patientId);

            //Get Medical Notes 
            var medicalnotesbal = new MedicalNotesBal();
            var notesList = medicalnotesbal.GetCustomMedicalNotes(patientId, type);

            using (var orderbal = new OpenOrderBal(Helpers.DefaultCptTableNumber,
                         Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber,
                         Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber))
            {
                // ################### Get Open Orders ##################
                var ordersData = orderbal.GetOrdersAndActivitiesByEncounter(encounterid);
                var orders = ordersData != null && ordersData.OpenOrders != null && ordersData.OpenOrders.Any()
                               ? ordersData.OpenOrders : new List<OpenOrderCustomModel>();

                if (orders.Any())
                {
                    openOrdersList = orders.Where(a => a.Status.Equals(OrderStatus.Open.ToString())).ToList();
                    closedOrdersList = orders.Where(a => !a.Status.Equals(OrderStatus.Open.ToString())).ToList();
                }
                // ################### Get Open Orders ##################


                // ################### Get Order Activities ##################
                var orderActivities = ordersData != null && ordersData.OrderActivities != null && ordersData.OrderActivities.Any()
                               ? ordersData.OrderActivities : new List<OrderActivityCustomModel>();
                if (orderActivities.Any())
                {
                    var activityOpenStatuses = new[] {
                                                0,
                                                (int)OpenOrderActivityStatus.Open,
                                                (int)OpenOrderActivityStatus.LabSectionWaitingForResult,
                                                (int)OpenOrderActivityStatus.LabSectionWaitingForSpecimen,
                                                (int)OpenOrderActivityStatus.PartiallyExecutedForResult
                                               };

                    openActivities = orderActivities.Where(x => activityOpenStatuses.Any(o => o == x.OrderActivityStatus.Value))
                                                    .OrderBy(x => x.ExecutedDate).ToList();

                    closedActivities = orderActivities.Where(x => !activityOpenStatuses.Any(o => o == x.OrderActivityStatus.Value))
                                                    .OrderBy(x => x.ExecutedDate).ToList();
                }
                // ################### Get Order Activities ##################

                //Get Medical Vitals...
                var medicalVitals = new MedicalVitalBal();
                var medicalvitals = medicalVitals.GetCustomMedicalVitals(patientId, Convert.ToInt32(MedicalRecordType.Vitals));

                //Get Nurse Assessment Forms and Other Docs....
                var docsBal = new DocumentsTemplatesBal();
                var opNurseAssessmentList = docsBal.GetNurseDocuments(patientId, encounterid);

                var medicalnotesview = new MedicalNotesView
                {
                    CurrentMedicalNotes = new MedicalNotes { NotesUserType = type, IsDeleted = false },
                    MedicalNotesList = notesList,
                    OpenOrdersList = openOrdersList,
                    ClosedOrdersList = closedOrdersList,
                    OpenActvitiesList = openActivities,
                    ClosedActvitiesList = closedActivities,
                    ClosedLabOrdersActivitesList = new List<OrderActivityCustomModel>(),
                    LabOpenOrdersActivitesList = new List<OrderActivityCustomModel>(),
                    EncounterOrder = new OpenOrder(),
                    IsLabTest = false,
                    CurrentMedicalVital = new MedicalVitalCustomModel(),
                    MedicalVitalList = medicalvitals,
                    PatientInfoId = patientId,
                    PatientEncounterId = encounterid,
                    EncounterList = encounterListData,
                    NurseEnteredFormList = opNurseAssessmentList.ToList(),
                    NurseDocList = nurseAssessmentfrom
                };

                return PartialView(PartialViews.NotesTabView, medicalnotesview);
            }
        }

        /// <summary>
        ///     Binds the open order activity list.
        /// </summary>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindOpenOrderActivityList(int encounterId)
        {
            using (
                var bal = new OrderActivityBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var list =
                    bal.GetOrderActivitiesByEncounterId(encounterId)
                        .Where(
                            x =>
                            x.OrderCategoryID != Convert.ToInt32(OrderTypeCategory.LabTest)
                            && x.OrderCategoryID != Convert.ToInt32(OrderTypeCategory.PathologyandLaboratory)
                            && (x.OrderActivityStatus == 0
                                || x.OrderActivityStatus == Convert.ToInt32(OpenOrderActivityStatus.Open)))
                        .OrderBy(x => x.OrderScheduleDate);
                return PartialView(PartialViews.OrderActivityScheduleList, list);
            }
        }

        /// <summary>
        ///     Binds the phy fav orders by sort.
        /// </summary>
        /// <param name="sort">
        ///     The sort.
        /// </param>
        /// <param name="sortdir">
        ///     The sortdir.
        /// </param>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindPhyFavOrdersBySort(string sort, string sortdir, int encounterId)
        {
            var encounterOrderbal = new OpenOrderBal(
                Helpers.DefaultCptTableNumber,
                Helpers.DefaultServiceCodeTableNumber,
                Helpers.DefaultDrgTableNumber,
                Helpers.DefaultDrugTableNumber,
                Helpers.DefaultHcPcsTableNumber,
                Helpers.DefaultDiagnosisTableNumber);
            var userId = Helpers.GetLoggedInUserId();
            var listOfOrders = encounterOrderbal.GetFavoriteOrders(userId);
            if (listOfOrders.Count > 0)
            {
                listOfOrders = listOfOrders.OrderBy(string.Format("{0} {1}", sort, sortdir)).ToList();
            }

            return PartialView("../PhysicianFavorites/" + PartialViews.PhyFavoriteOrders, listOfOrders);
        }

        /// <summary>
        ///     Binds the phy most orders by sort.
        /// </summary>
        /// <param name="sort">
        ///     The sort.
        /// </param>
        /// <param name="sortdir">
        ///     The sortdir.
        /// </param>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindPhyMostOrdersBySort(string sort, string sortdir, int encounterId)
        {
            var encounterOrderbal = new OpenOrderBal(
                Helpers.DefaultCptTableNumber,
                Helpers.DefaultServiceCodeTableNumber,
                Helpers.DefaultDrgTableNumber,
                Helpers.DefaultDrugTableNumber,
                Helpers.DefaultHcPcsTableNumber,
                Helpers.DefaultDiagnosisTableNumber);
            var userId = Helpers.GetLoggedInUserId();
            var listOfOrders = encounterOrderbal.GetMostOrderedList(userId, 0);
            if (listOfOrders.Count > 0)
            {
                listOfOrders = listOfOrders.OrderBy(string.Format("{0} {1}", sort, sortdir)).ToList();
            }

            return PartialView("../PhysicianFavorites/" + PartialViews.PhyAllOrders, listOfOrders);
        }

        /// <summary>
        ///     Binds the physician fav diagnosis list sorted.
        /// </summary>
        /// <param name="sort">
        ///     The sort.
        /// </param>
        /// <param name="sortdir">
        ///     The sortdir.
        /// </param>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindPhysicianFavDiagnosisListSorted(string sort, string sortdir, int encounterId)
        {
            var favDiagnosisBal = new FavoritesBal(
                Helpers.DefaultCptTableNumber,
                Helpers.DefaultServiceCodeTableNumber,
                Helpers.DefaultDrgTableNumber,
                Helpers.DefaultDrugTableNumber,
                Helpers.DefaultHcPcsTableNumber,
                Helpers.DefaultDiagnosisTableNumber);
            var userid = Helpers.GetLoggedInUserId();
            var favDiagnosisList = favDiagnosisBal.GetFavoriteOrders(userid);
            var listOfOrders =
                favDiagnosisList.Where(
                    _ =>
                    _.CategoryId == Convert.ToInt32(OrderType.Diagnosis).ToString()
                    || _.CategoryId == Convert.ToInt32(OrderType.DRG).ToString()
                    || _.CategoryId == Convert.ToInt32(OrderType.CPT).ToString()).ToList();
            if (listOfOrders.Count > 0)
            {
                listOfOrders = listOfOrders.OrderBy(string.Format("{0} {1}", sort, sortdir)).ToList();
            }

            return PartialView("../Diagnosis/" + PartialViews.PhyFavDiagnosisList, listOfOrders);
        }

        /// <summary>
        ///     Binds the previous encounter diagnosis list sorted.
        /// </summary>
        /// <param name="sort">
        ///     The sort.
        /// </param>
        /// <param name="sortdir">
        ///     The sortdir.
        /// </param>
        /// <param name="patientId">
        ///     The patient identifier.
        /// </param>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindPreviousEncounterDiagnosisListSorted(
            string sort,
            string sortdir,
            int patientId,
            int encounterId)
        {
            List<DiagnosisCustomModel> listOfOrders;
            using (var dbal = new DiagnosisBal(Helpers.DefaultDiagnosisTableNumber, Helpers.DefaultDrgTableNumber)) listOfOrders = dbal.GetPreviousDiagnosisList(patientId, encounterId);
            if (listOfOrders.Count > 0)
            {
                listOfOrders = listOfOrders.OrderBy(string.Format("{0} {1}", sort, sortdir)).ToList();
            }

            return PartialView("../Diagnosis/" + PartialViews.PreviousDiagnosisList, listOfOrders);
        }

        /// <summary>
        ///     Binds the pharmacy closed activity list.
        /// </summary>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindRadClosedActivityList(int encounterId)
        {
            using (
                var bal = new OrderActivityBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var list =
                    bal.GetOrderActivitiesByEncounterId(encounterId)
                        .Where(
                            x =>
                            x.OrderCategoryID == Convert.ToInt32(OrderTypeCategory.Radiology)
                            && x.OrderActivityStatus != 0
                            && x.OrderActivityStatus != Convert.ToInt32(OpenOrderActivityStatus.Open))
                        .OrderBy(x => x.ExecutedDate)
                        .ToList();
                return PartialView(PartialViews.OrderClosedActivityScheduleList, list);
            }
        }

        /// <summary>
        ///     Binds the pharmacy closed orders.
        /// </summary>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindRadClosedOrders(int encounterId)
        {
            using (
                var bal = new OpenOrderBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var list =
                    bal.GetPhysicianOrders(encounterId, OrderStatus.Closed.ToString())
                        .Where(x => x.CategoryId == Convert.ToInt32(OrderTypeCategory.Radiology))
                        .OrderBy(x => x.OpenOrderID)
                        .ToList();
                return PartialView(PartialViews.ClosedOrdersList, list);
            }
        }

        /// <summary>
        ///     Binds the pharmacy activity list.
        /// </summary>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindRadOpenActivityList(int encounterId)
        {
            using (
                var bal = new OrderActivityBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var list =
                    bal.GetOrderActivitiesByEncounterId(encounterId)
                        .Where(
                            x =>
                            x.OrderCategoryID == Convert.ToInt32(OrderTypeCategory.Radiology)
                            && (x.OrderActivityStatus == Convert.ToInt32(OpenOrderActivityStatus.Open)
                                || x.OrderActivityStatus == 0))
                        .OrderBy(x => x.OrderScheduleDate)
                        .ToList();
                return PartialView(PartialViews.OrderActivityScheduleList, list);
            }
        }

        /// <summary>
        ///     Binds the summary notes.
        /// </summary>
        /// <param name="patientId">
        ///     The patient identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindSummaryNotes(int patientId)
        {
            using (var medicalnotesbal = new MedicalNotesBal())
            {
                var patientSummaryNotes = medicalnotesbal.GetMedicalNotesByPatientId(patientId);
                var view = "../MedicalNotes/" + PartialViews.MedicalNotesListPatientSummary;
                return PartialView(view, patientSummaryNotes);
            }
        }

        /// <summary>
        ///     The cancel order activity.
        /// </summary>
        /// <param name="OpenOrderActivityId">The open order activity identifier.</param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult CancelOrderActivity(int OpenOrderActivityId)
        {
            // ... Send 9 as status for canceling the activity
            var encounterId = new OrderActivityBal().CloseOrderActivity(OpenOrderActivityId); // Returns EncounterId for Binding of List --------- Changes Done by Abhishek 11 June 2018


            var orderBal = new OpenOrderBal(
                Helpers.DefaultCptTableNumber,
                Helpers.DefaultServiceCodeTableNumber,
                Helpers.DefaultDrgTableNumber,
                Helpers.DefaultDrugTableNumber,
                Helpers.DefaultHcPcsTableNumber,
                Helpers.DefaultDiagnosisTableNumber);


            //DB Call to get all records related to Orders tab
            var orderActivities = orderBal.OrderActivitiesByEncounterId(encounterId);

            // Order Activities Section, starts here
            var openActStatuses = new[] { 0, 1, 30, 20, 40 };
            var openOrderActivityList = orderActivities.Where(a => openActStatuses.Contains(Convert.ToInt32(a.OrderActivityStatus))).ToList();

            //json data to return to view
            var jsonData =
                new
                {
                    Status = encounterId > 0 ? true : false,
                    OpenOrderActivityList = openOrderActivityList.Select(x => new[] {x.Status, Convert.ToString(x.ShowEditAction), Convert.ToString(x.OrderActivityID), Convert.ToString(x.OrderCategoryID),
                                x.OrderTypeName, x.OrderCode, x.OrderDescription, x.CategoryName, x.SubCategoryName, x.OrderScheduleDate.HasValue ? x.OrderScheduleDate.Value.ToString("d") : string.Empty, string.Empty })
                };
            //json data to return to view


            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
            //return Json(actvitystatus, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     Checks the code for fav.
        /// </summary>
        /// <param name="codeid">
        ///     The codeid.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult CheckCodeForFav(string codeid)
        {
            var openOrder = new OpenOrderBal(
                Helpers.DefaultCptTableNumber,
                Helpers.DefaultServiceCodeTableNumber,
                Helpers.DefaultDrgTableNumber,
                Helpers.DefaultDrugTableNumber,
                Helpers.DefaultHcPcsTableNumber,
                Helpers.DefaultDiagnosisTableNumber);
            var userid = Helpers.GetLoggedInUserId();
            var orderCode = openOrder.GetOrderIdByOrderCode(userid, codeid);
            if (orderCode != 0)
            {
                var userDefDescBal = new FavoritesBal();
                var isFav = userDefDescBal.GetFavoriteByCodeIdPhyId(orderCode.ToString(), userid);
                if (isFav != null)
                {
                    return Json(isFav);
                }

                isFav = userDefDescBal.GetFavoriteByCodeIdPhyId(codeid, userid);

                if (isFav != null)
                {
                    return Json(isFav);
                }

                return Json(-1);
            }

            return Json(-1);
        }

        /// <summary>
        ///     Checks the durg allergy.
        /// </summary>
        /// <param name="ordercode">
        ///     The ordercode.
        /// </param>
        /// <param name="patientId">
        ///     The patient identifier.
        /// </param>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult CheckDurgAllergy(string ordercode, int patientId, int encounterId)
        {
            var openOrderBal = new OpenOrderBal(
                Helpers.DefaultCptTableNumber,
                Helpers.DefaultServiceCodeTableNumber,
                Helpers.DefaultDrgTableNumber,
                Helpers.DefaultDrugTableNumber,
                Helpers.DefaultHcPcsTableNumber,
                Helpers.DefaultDiagnosisTableNumber);
            var isAllergic = openOrderBal.CheckDurgAllergy(ordercode, patientId, encounterId);
            var jsonResult = new { isAllergic };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     Checks if any primary diagnosis exists.
        /// </summary>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult CheckIfAnyPrimaryDiagnosisExists(int encounterId)
        {
            using (var bal = new DiagnosisBal(Helpers.DefaultDiagnosisTableNumber, Helpers.DefaultDrgTableNumber))
            {
                var primaryDiagnosisId = bal.CheckIfAnyDiagnosisExists(encounterId);
                return Json(primaryDiagnosisId);
            }
        }

        /// <summary>
        ///     Checks the mulitple open activites.
        /// </summary>
        /// <param name="orderId">
        ///     The order identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult CheckMulitpleOpenActivites(int orderId)
        {
            using (
                var openOrderBaL = new OpenOrderBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var isMulitpleActivitesOpen = openOrderBaL.CheckMulitpleOpenActivites(orderId);
                return Json(isMulitpleActivitesOpen);
            }
        }

        /// <summary>
        ///     Edits the lab open order activity.
        /// </summary>
        /// <param name="OpenOrderActivityId">
        ///     The open order activity identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult EditLabOpenOrderActivity(int OpenOrderActivityId)
        {
            using (
                var bal = new OrderActivityBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var result = bal.GetLabOrderActivityByActivityId(OpenOrderActivityId);
                return PartialView(PartialViews.LabAdministerOrder, result);
            }
        }

        /// <summary>
        ///     Edits the lab speciman open order activity.
        /// </summary>
        /// <param name="OpenOrderActivityId">
        ///     The open order activity identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult EditLabSpecimanOpenOrderActivity(int OpenOrderActivityId)
        {
            using (
                var bal = new OrderActivityBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var result = bal.GetLabOrderActivityByActivityId(OpenOrderActivityId);
                return PartialView(PartialViews.LabSpecimanMGT, result);
            }
        }

        /// <summary>
        ///     Edits the open order activity.
        /// </summary>
        /// <param name="OpenOrderActivityId">
        ///     The open order activity identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult EditOpenOrderActivity(int OpenOrderActivityId)
        {
            using (
                var bal = new OrderActivityBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var result = bal.GetOrderActivityByID(OpenOrderActivityId);
                return PartialView(PartialViews.AdministerOrdersByNurse, result);
            }
        }

        /// <summary>
        ///     Gets the code custom value by identifier.
        /// </summary>
        /// <param name="id">
        ///     The identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetCodeCustomValueById(int id)
        {
            using (var bal = new GlobalCodeBal())
            {
                var globalCode =
                    bal.GetGlobalCodeByCategoryAndCodeValue(
                        Convert.ToInt32(GlobalCodeCategoryValue.CodeTypes).ToString(),
                        id.ToString());
                var value = globalCode != null ? globalCode.GlobalCodeName : string.Empty;
                return Json(value);
            }
        }

        /// <summary>
        ///     Gets the code value by identifier.
        /// </summary>
        /// <param name="id">
        ///     The identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetCodeValueById(int id)
        {
            using (var bal = new GlobalCodeBal())
            {
                var globalCode = bal.GetGlobalCodeByGlobalCodeId(id);
                var value = globalCode != null ? globalCode.GlobalCodeName : string.Empty;
                return Json(value);
            }
        }

        /// <summary>
        ///     The get em list.
        /// </summary>
        /// <param name="eId">
        ///     The e id.
        /// </param>
        /// <param name="pId">
        ///     The p id.
        /// </param>
        /// <returns>
        ///     The <see cref="PartialViewResult" />.
        /// </returns>
        public PartialViewResult GetEmList(int eId, int pId)
        {
            using (var bal = new PatientEvaluationBal())
            {
                var list = bal.ListPatientEvaluation(pId, eId);
                return PartialView(PartialViews.EvaluationList, list);
            }
        }

        /// <summary>
        ///     Gets the fav order detail by identifier.
        /// </summary>
        /// <param name="favorderId">
        ///     The favorder identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetFavOrderDetailById(int favorderId)
        {
            var fId = Helpers.GetDefaultFacilityId();
            var cList = new List<GlobalCodeCategory>();
            var bal = new OpenOrderBal(
                Helpers.DefaultCptTableNumber,
                Helpers.DefaultServiceCodeTableNumber,
                Helpers.DefaultDrgTableNumber,
                Helpers.DefaultDrugTableNumber,
                Helpers.DefaultHcPcsTableNumber,
                Helpers.DefaultDiagnosisTableNumber);

            //Get Favorite Order 
            var order = bal.GetFavOpenOrderDetail(favorderId, fId, Helpers.GetLoggedInUserId());

            ////Get the list of Order Frequency, Order Statuses, Order Quantity, Order Document Types and Order Note Types (All From GlobalCodes)
            //using (var gbal = new GlobalCodeBal())
            //{
            //    var categories = new[] { "1024", "3102", "1011", "2305", "963" };
            //    var list = gbal.GetListByCategoriesRange(categories);
            //    var jsonData =
            //        new
            //        {
            //            listFrequency = list.Where(g => g.ExternalValue1.Equals("1024")).ToList(),
            //            listOrderStatus = list.Where(g => g.ExternalValue1.Equals("3102")).ToList(),
            //            listQuantity =
            //                    list.Where(g => g.ExternalValue1.Equals("1011"))
            //                        .OrderBy(m => Convert.ToDecimal(m.Value))
            //                        .ToList(),
            //            listDocumentType = list.Where(g => g.ExternalValue1.Equals("2305")).ToList(),
            //            listNoteType = list.Where(g => g.ExternalValue1.Equals("963")).ToList()
            //        };
            //}

            ////Get Categories and SubCategories list to fill up the dropdowns in the Orders Tabs or others wherever applicable in EHR
            //using (var cBal = new GlobalCodeCategoryBal())
            //{
            //    cList = cBal.GetGlobalCodeCategoriesByExternalValue(Convert.ToString(fId));
            //}

            return PartialView(PartialViews.PhysicianOpenOrderAddEdit, order);
        }

        /// <summary>
        ///     Gets the favorite by code identifier.
        /// </summary>
        /// <param name="codeId">
        ///     The code identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetFavoriteByCodeId(string codeId)
        {
            using (var bal = new FavoritesBal())
            {
                var fav = bal.GetFavoriteByCodeId(codeId);
                if (fav != null)
                {
                    var jsonResult =
                        new
                        {
                            id = fav.UserDefinedDescriptionID,
                            isFavorite = fav.IsDeleted == null || !(bool)fav.IsDeleted,
                            description = fav.UserDefineDescription
                        };
                    return Json(jsonResult, JsonRequestBehavior.AllowGet);
                }

                return Json(null);
            }
        }

        /// <summary>
        ///     Gets the favorites orders.
        /// </summary>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetFavoritesOrders()
        {
            var list = new List<OpenOrderCustomModel>();
            using (
                var bal = new OpenOrderBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber)) list = bal.GetFavoriteOrders(Helpers.GetLoggedInUserId());
            return PartialView(PartialViews.FavoriteOrders, list);
        }

        /// <summary>
        ///     Gets the frequency code detail.
        /// </summary>
        /// <param name="frequencyCodeId">
        ///     The frequency code identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetFrequencyCodeDetail(string frequencyCodeId)
        {
            string value;
            var categoryValue = Convert.ToString(Convert.ToInt32(GlobalCodeCategoryValue.OrderFrequencyType));
            using (var bal = new GlobalCodeBal())
            {
                var gc = bal.GetGlobalCodeByCategoryAndCodeValue(categoryValue, frequencyCodeId);
                value = gc != null ? gc.ExternalValue6 : string.Empty;
            }

            return Json(value);
        }

        /// <summary>
        ///     Gets the lab speciman order.
        /// </summary>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetLabSpecimanOrder(int encounterId)
        {
            using (
                var bal = new OrderActivityBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var list =
                    bal.GetOrderActivitiesByEncounterId(encounterId)
                        .Where(
                            x =>
                            x.OrderCategoryID == Convert.ToInt32(OrderTypeCategory.PathologyandLaboratory)
                            && (x.OrderActivityStatus == Convert.ToInt32(OpenOrderActivityStatus.Open)
                                || x.OrderActivityStatus == 0))
                        .OrderBy(x => x.OrderScheduleDate)
                        .ToList();
                return PartialView(PartialViews.LabSpecimanListing, list);
            }
        }

        /// <summary>
        ///     Gets the most recent orders.
        /// </summary>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetMostRecentOrders()
        {
            var list = new List<OpenOrderCustomModel>();
            using (
                var bal = new OpenOrderBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber)) list = bal.GetMostRecentOrders(Helpers.GetLoggedInUserId());

            return PartialView(PartialViews.MostRecentOrders, list);
        }

        /// <summary>
        ///     Gets the open order activity detail by identifier.
        /// </summary>
        /// <param name="id">
        ///     The identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetOpenOrderActivityDetailById(int id)
        {
            using (var bal = new OpenOrderActivityScheduleBal())
            {
                var model = bal.GetOpenOrderActivityScheduleById(id);
                return PartialView(PartialViews.AddressAddEdit, model);
            }
        }

        /// <summary>
        ///     Gets the open order detail by order identifier.
        /// </summary>
        /// <param name="orderId">
        ///     The order identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetOpenOrderDetailByOrderId(int orderId)
        {
            using (
                var bal = new OpenOrderBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var current = bal.GetOpenOrderDetail(orderId);
                var openOrderActivity = new OrderActivity
                {
                    OrderType = Convert.ToInt32(current.OrderType),
                    OrderCode = current.OrderCode,
                    CorporateID = current.CorporateID,
                    FacilityID = current.FacilityID,
                    PatientID = current.PatientID,
                    EncounterID = current.EncounterID,
                    OrderScheduleDate = current.StartDate,
                    OrderActivityQuantity = current.Quantity,
                    IsActive = current.IsActive,
                    ModifiedBy = current.ModifiedBy,
                    ModifiedDate = current.ModifiedDate,
                    CreatedDate = current.CreatedDate,
                    OrderSubCategoryID = current.SubCategoryId,
                    OrderCategoryID = current.CategoryId
                };
                return PartialView(PartialViews.AdministerOrdersByNurse, openOrderActivity);
            }
        }

        /// <summary>
        ///     Gets the order codes.
        /// </summary>
        /// <param name="codetypeid">
        ///     The codetypeid.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetOrderCodes(int codetypeid)
        {
            switch (codetypeid)
            {
                case 1:
                    var cptCodesBal = new CPTCodesBal(Helpers.DefaultCptTableNumber);
                    var cptcodeslist = cptCodesBal.GetCPTCodes();
                    return Json(cptcodeslist);
                case 2:
                    var hcpcsCodesBal = new HCPCSCodesBal(Helpers.DefaultHcPcsTableNumber);
                    var hcpcsCodeslist = hcpcsCodesBal.GetHCPCSCodes();
                    return Json(hcpcsCodeslist);
                case 3:
                    return Json(null);
                case 4:
                case 5:
                    var drgCodesBal = new DRGCodesBal(Helpers.DefaultDrgTableNumber);
                    var drgCodeslist = drgCodesBal.GetDrgCodes();
                    return Json(drgCodeslist);
            }

            return Json(null);
        }

        /// <summary>
        ///     Gets the order codes by sub category.
        /// </summary>
        /// <param name="subCategoryId">
        ///     The sub category identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetOrderCodesBySubCategory(int subCategoryId)
        {
            using (var bal = new GlobalCodeBal())
            {
                // Get Sub-category Details from table Global Codes
                var subCategory = bal.GetGlobalCodeByGlobalCodeId(subCategoryId);

                // Get Order Type Name by Order Type Id, from table Global Codes
                var codeType =
                    bal.GetGlobalCodeByCategoryAndCodeValue(
                        Convert.ToString((int)GlobalCodeCategoryValue.CodeTypes),
                        subCategory.ExternalValue1);

                var type = (OrderType)Enum.Parse(typeof(OrderType), subCategory.ExternalValue1);
                switch (type)
                {
                    case OrderType.CPT:
                        using (var cbal = new CPTCodesBal(Helpers.DefaultCptTableNumber))
                        {
                            var result = cbal.GetCodesByRange(
                                Convert.ToInt32(subCategory.ExternalValue2),
                                Convert.ToInt32(subCategory.ExternalValue3));
                            var filteredList = result.Select(
                                item => new
                                {
                                    Value = item.CodeNumbering,

                                    // Text = !string.IsNullOrEmpty(item.CodeDescription) ? item.CodeDescription:string.Empty
                                    // Text = string.Format("{0} - {1}", !string.IsNullOrEmpty(item.CodeDescription) && item.CodeDescription.Length > 25 ? item.CodeDescription.Substring(0, 25) + "..." : item.CodeDescription, item.CodeNumbering)
                                    Text =
                                            string.Format("{0} - {1}", item.CodeNumbering, item.CodeDescription)
                                })
                                .ToList();
                            var jsonResult =
                                new
                                {
                                    codeList = filteredList,
                                    codeTypeName = codeType.GlobalCodeName,
                                    codeTypeId = codeType.GlobalCodeValue
                                };
                            return Json(jsonResult, JsonRequestBehavior.AllowGet);
                        }

                    case OrderType.HCPCS:
                        using (var cbal = new HCPCSCodesBal(Helpers.DefaultHcPcsTableNumber))
                        {
                            var result = cbal.GetHCPCSCodes();
                            var filteredList = result.Select(
                                item => new
                                {
                                    Value = item.CodeNumbering,

                                    // Text = !string.IsNullOrEmpty(item.CodeDescription) ? item.CodeDescription : string.Empty
                                    // Text = string.Format("{0} - {1}", !string.IsNullOrEmpty(item.CodeDescription) && item.CodeDescription.Length > 25 ? item.CodeDescription.Substring(0, 25) + "..." : item.CodeDescription, item.CodeNumbering)
                                    Text =
                                            string.Format("{0} - {1}", item.CodeNumbering, item.CodeDescription)
                                })
                                .ToList();
                            var jsonResult =
                                new
                                {
                                    codeList = filteredList,
                                    codeTypeName = codeType.GlobalCodeName,
                                    codeTypeId = codeType.GlobalCodeValue
                                };
                            return Json(jsonResult, JsonRequestBehavior.AllowGet);
                        }

                    case OrderType.DRG:
                        break;
                    case OrderType.DRUG:
                        using (var dbal = new DrugBal(Helpers.DefaultDrugTableNumber))
                        {
                            var result = dbal.GetDrugList();
                            var filteredList = result.Select(
                                item => new
                                {
                                    Value = item.DrugCode,

                                    // Text = !string.IsNullOrEmpty(item.DrugPackageName) ? item.DrugPackageName : string.Empty
                                    // Text = string.Format("{0} - {1}", !string.IsNullOrEmpty(item.DrugPackageName) && item.DrugPackageName.Length > 25 ? item.DrugPackageName.Substring(0, 25) + "..." : item.DrugPackageName, item.DrugCode),
                                    Text = string.Format("{0} - {1}", item.DrugCode, item.DrugPackageName)
                                })
                                .ToList();
                            var jsonResult =
                                new
                                {
                                    codeList = filteredList,
                                    codeTypeName = codeType.GlobalCodeName,
                                    codeTypeId = codeType.GlobalCodeValue
                                };
                            return Json(jsonResult, JsonRequestBehavior.AllowGet);
                        }

                    default:
                        break;
                }

                return Json(null);
            }
        }

        /// <summary>
        ///     Gets the order detail by identifier.
        /// </summary>
        /// <param name="orderId">
        ///     The order identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        [System.Web.Mvc.HttpPost]
        public ActionResult GetOrderDetailById(string orderId)
        {
            var encounterOrderbal = new OpenOrderBal(
                Helpers.DefaultCptTableNumber,
                Helpers.DefaultServiceCodeTableNumber,
                Helpers.DefaultDrgTableNumber,
                Helpers.DefaultDrugTableNumber,
                Helpers.DefaultHcPcsTableNumber,
                Helpers.DefaultDiagnosisTableNumber);
            var id = 0;
            if (!string.IsNullOrEmpty(orderId))
            {
                id = Convert.ToInt32(orderId);
            }

            var orderDetail = encounterOrderbal.GetOpenOrderDetail(Convert.ToInt32(orderId));

            return PartialView(PartialViews.PhysicianOpenOrderAddEdit, orderDetail);
        }

        /// <summary>
        ///     Gets the order details.
        /// </summary>
        /// <param name="orderId">
        ///     The order identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetOrderDetails(int orderId)
        {
            var encounterOrderbal = new OpenOrderBal(
                Helpers.DefaultCptTableNumber,
                Helpers.DefaultServiceCodeTableNumber,
                Helpers.DefaultDrgTableNumber,
                Helpers.DefaultDrugTableNumber,
                Helpers.DefaultHcPcsTableNumber,
                Helpers.DefaultDiagnosisTableNumber);
            var orderDetail = encounterOrderbal.GetOpenOrderDetail(Convert.ToInt32(orderId));
            if (orderDetail != null)
            {
                return Json(orderDetail, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        /// <summary>
        ///     Gets the order details by activity identifier.
        /// </summary>
        /// <param name="orderActivityId">
        ///     The order activity identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetOrderDetailsByActivityId(int orderActivityId)
        {
            var encounterOrderbal = new OpenOrderBal(
                Helpers.DefaultCptTableNumber,
                Helpers.DefaultServiceCodeTableNumber,
                Helpers.DefaultDrgTableNumber,
                Helpers.DefaultDrugTableNumber,
                Helpers.DefaultHcPcsTableNumber,
                Helpers.DefaultDiagnosisTableNumber);
            var orderActivitybal = new OrderActivityBal(
                Helpers.DefaultCptTableNumber,
                Helpers.DefaultServiceCodeTableNumber,
                Helpers.DefaultDrgTableNumber,
                Helpers.DefaultDrugTableNumber,
                Helpers.DefaultHcPcsTableNumber,
                Helpers.DefaultDiagnosisTableNumber);
            var orderActivityDetail = orderActivitybal.GetOrderActivityByID(Convert.ToInt32(orderActivityId));
            if (orderActivityDetail != null)
            {
                var orderDetail = encounterOrderbal.GetOpenOrderDetail(Convert.ToInt32(orderActivityDetail.OrderID));
                if (orderDetail != null)
                {
                    return Json(orderDetail, JsonRequestBehavior.AllowGet);
                }
            }

            return null;
        }

        /// <summary>
        ///     Gets the orders tab data.
        /// </summary>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetOrdersTabData(int encounterId)
        {
            var userId = Helpers.GetLoggedInUserId();
            var orderBal = new OpenOrderBal(
                Helpers.DefaultCptTableNumber,
                Helpers.DefaultServiceCodeTableNumber,
                Helpers.DefaultDrgTableNumber,
                Helpers.DefaultDrugTableNumber,
                Helpers.DefaultHcPcsTableNumber,
                Helpers.DefaultDiagnosisTableNumber);
            var allEncounterOrders = orderBal.GetAllOrdersByEncounterId(Convert.ToInt32(encounterId));
            var closedOrdersList = new List<OpenOrderCustomModel>();
            var openOrderList = new List<OpenOrderCustomModel>();
            var closedOrderActivityList = new List<OrderActivityCustomModel>();
            var openOrderActivityList = new List<OrderActivityCustomModel>();
            using (
                var orderActivityBal = new OrderActivityBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var encounterActivitesobj =
                    orderActivityBal.GetOrderActivitiesByEncounterId(Convert.ToInt32(encounterId));
                var encounterActivitesClosedListObj =
                    encounterActivitesobj.Where(
                        x =>
                        x.OrderActivityStatus != 0
                        && x.OrderActivityStatus != Convert.ToInt32(OpenOrderActivityStatus.Open)).ToList();
                var encounterActivitesOpenListObj =
                    encounterActivitesobj.Where(
                        x =>
                        x.OrderActivityStatus == 0
                        || x.OrderActivityStatus == Convert.ToInt32(OpenOrderActivityStatus.Open)).ToList();
                closedOrderActivityList = encounterActivitesClosedListObj;
                openOrderActivityList = encounterActivitesOpenListObj;
            }

            if (allEncounterOrders.Count > 0)
            {
                openOrderList = allEncounterOrders.Where(a => a.OrderStatus.Equals("1")).ToList();
                closedOrdersList =
                    allEncounterOrders.Where(
                        a => a.OrderStatus.Equals("3") || a.OrderStatus.Equals("4") || a.OrderStatus.Equals("2"))
                        .ToList();
            }

            var openorderHtml = PartialView(PartialViews.PhysicianOpenOrderList, openOrderList);
            var closedorderHtml = PartialView(PartialViews.ClosedOrdersList, closedOrdersList);
            var closedorderActivityHtml = PartialView(
                PartialViews.OrderActivityScheduleList,
                closedOrderActivityList);
            var openorderActivityHtml = PartialView(PartialViews.OrderActivityScheduleList, openOrderActivityList);

            var jsonResult =
                new
                {
                    openorderList = openorderHtml,
                    closedorderList = closedorderHtml,
                    openorderActivityList = openorderActivityHtml,
                    closedorderActivityList = closedorderActivityHtml
                };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);

            // var ordersFullView = new OrdersFullView
            // {
            // AllPhysicianOrders = allPhysicianOrders.DistinctBy(x => x.OrderCode).ToList(),
            // ClosedOrdersList = closedOrdersList,
            // EncounterOrder = new OpenOrder { StartDate = Helpers.GetInvariantCultureDateTime(), EndDate = Helpers.GetInvariantCultureDateTime(), OrderStatus = Convert.ToInt32(OrderStatus.Open).ToString() },
            // FavoriteOrders = favoriteOrders,
            // MostRecentOrders = mostRecentOrders,
            // OpenOrdersList = openOrderList,
            // SearchedOrders = new List<OpenOrderCustomModel>(),
            // ClosedOrderActivityList = closedOrderActivityList,
            // OpenOrderActivityList = openOrderActivityList,
            // CurrentOrderActivity = new OrderActivity()
            // }; 
        }

        /// <summary>
        ///     Gets the patient notes.
        /// </summary>
        /// <param name="patientId">
        ///     The patient identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetPatientNotes(int patientId)
        {
            using (var bal = new MedicalNotesBal())
            {
                var patientSummaryNotes = bal.GetMedicalNotesByPatientId(patientId);
                return PartialView(PartialViews.MedicalNotesListPatientSummary, patientSummaryNotes);
            }
        }

        /// <summary>
        ///     Gets the pharmacy order codes by sub category.
        /// </summary>
        /// <param name="subCategoryId">
        ///     The sub category identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetPharmacyOrderCodesBySubCategory(int subCategoryId)
        {
            using (var bal = new GlobalCodeBal())
            {
                // Get Sub-category Details from table Global Codes
                var subCategory = bal.GetGlobalCodeByGlobalCodeId(subCategoryId);
                using (var dbal = new DrugBal(Helpers.DefaultDrugTableNumber))
                {
                    /*
                     * Earlier it was fetching the Orders based on 
                     * GlobalCodeID (which is Sub-Category ID) which should be GlobalCode Value
                     */

                    // Old Code
                    // var result = dbal.GetDrugListbyBrandCode(Convert.ToString(subCategoryId));

                    // New Code
                    var result = dbal.GetDrugListbyBrandCode(subCategory.GlobalCodeValue);

                    var filteredList =
                        result.Select(
                            item =>
                            new
                            {
                                Value = item.DrugCode,
                                Text =
                                string.Format(
                                    "{0} - {1} - {2} - {3}",
                                    item.DrugCode,
                                    item.DrugGenericName,
                                    item.DrugStrength,
                                    item.DrugDosage)
                            }).ToList();
                    var jsonResult =
                        new
                        {
                            codeList = filteredList,
                            codeTypeName = OrderType.DRUG.ToString(),
                            codeTypeId = Convert.ToString((int)OrderType.DRUG)
                        };
                    return Json(jsonResult, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(null);
        }

        /// <summary>
        ///     Gets the searched orders.
        /// </summary>
        /// <param name="text">
        ///     The text.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetSearchedOrders(string text)
        {
            var list = new List<OpenOrderCustomModel>();
            if (!string.IsNullOrEmpty(text))
            {
                using (
                    var bal = new OpenOrderBal(
                        Helpers.DefaultCptTableNumber,
                        Helpers.DefaultServiceCodeTableNumber,
                        Helpers.DefaultDrgTableNumber,
                        Helpers.DefaultDrugTableNumber,
                        Helpers.DefaultHcPcsTableNumber,
                        Helpers.DefaultDiagnosisTableNumber)) list = bal.GetSearchedOrders(text);
            }

            return PartialView(PartialViews.OpenOrdersInSearch);

            // return Json(list);
        }

        public ActionResult GetSearchedOrdersList(string text)
        {
            var list = new List<OpenOrderCustomModel>();
            if (!string.IsNullOrEmpty(text))
            {
                using (
                    var bal = new OpenOrderBal(
                        Helpers.DefaultCptTableNumber,
                        Helpers.DefaultServiceCodeTableNumber,
                        Helpers.DefaultDrgTableNumber,
                        Helpers.DefaultDrugTableNumber,
                        Helpers.DefaultHcPcsTableNumber,
                        Helpers.DefaultDiagnosisTableNumber)) list = bal.GetSearchedOrders(text);
            }

            //json data to return to view
            var jsonData =
                new
                {
                    SearchOrders = list.Select(x => new[] { Convert.ToString(x.OpenOrderID), x.ActivityCode, x.OrderCode, x.OrderDescription, x.CategoryName, x.SubCategoryName,
                        x.Status, x.FrequencyText, x.Quantity.HasValue ? Convert.ToString(x.Quantity.Value) : string.Empty, x.PeriodDays, x.OrderNotes, string.Empty })
                };
            //json data to return to view


            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
            // return Json(list);
        }
        /// <summary>
        ///     Gets the speciman string.
        /// </summary>
        /// <param name="orderCode">
        ///     The order code.
        /// </param>
        /// <returns>
        ///     The <see cref="JsonResult" />.
        /// </returns>
        public JsonResult GetSpecimanString(string orderCode)
        {
            using (
                var openOrderBaL = new OrderActivityBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var specimanrquired = openOrderBaL.CalculateLabResultSpecimanType(orderCode, null, null);
                return Json(specimanrquired, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        ///     Determines whether [is already fav] [the specified userid].
        /// </summary>
        /// <param name="userid">
        ///     The userid.
        /// </param>
        /// <param name="codeId">
        ///     The code identifier.
        /// </param>
        /// <param name="categoryId">
        ///     The category identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool IsAlreadyFav(int userid, string codeId, string categoryId)
        {
            using (var bal = new FavoritesBal())
            {
                return bal.CheckIfAlreadyFav(userid, codeId, categoryId);
            }
        }



        /// <summary>
        ///     Reset the Facility View Model and pass it to FacilityAddEdit Partial View.
        /// </summary>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult ResetPhysicianOrderForm()
        {
            // Intialize the new object of Facility ViewModel
            var encounterOrder = new OpenOrder
            {
                StartDate = Helpers.GetInvariantCultureDateTime(),
                EndDate = Helpers.GetInvariantCultureDateTime(),
                OrderStatus = Convert.ToInt32(OrderStatus.Open).ToString()
            };

            // Pass the View Model as FacilityViewModel to PartialView FacilityAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.PhysicianOpenOrderAddEdit, encounterOrder);
        }

        // public ActionResult SaveCarePlanTask(CarePlanTask model)
        // {
        // // Initialize the newId variable 
        // var corporateId = Helpers.GetSysAdminCorporateID();
        // var facilityId = Helpers.GetDefaultFacilityId();
        // var dateTime = Helpers.GetInvariantCultureDateTime();
        // int newId = -1;
        // int userId = Helpers.GetLoggedInUserId();

        // // Check if Model is not null 
        // if (model != null)
        // {
        // using (var bal = new CarePlanTaskBal())
        // {
        // if (model.Id > 0)
        // {
        // model.ModifiedBy = userId;
        // model.ModifiedDate = dateTime;
        // model.FacilityId = facilityId;
        // model.CorporateId = corporateId;
        // }
        // else
        // {
        // model.CreatedBy = userId;
        // model.CreatedDate = dateTime;
        // model.FacilityId = facilityId;
        // model.CorporateId = corporateId;
        // model.CreatedDate = dateTime;
        // }
        // // Call the AddCarePlanTask Method to Add / Update current CarePlanTask
        // newId = bal.SaveCarePlanTask(model);

        // //var cModel = bal.GetCarePlanTaskById(Convert.ToInt32(model.TaskNumber));
        // var pcBal = new PatientCarePlanBal();
        // var patinetCareModel = new PatientCarePlan
        // {
        // TaskId = model.TaskNumber,
        // CorporateId = model.CorporateId,
        // FacilityId = model.FacilityId,
        // PatientId = Convert.ToString(model.PatientId),
        // EncounterId = model.EncounterId,
        // CreatedBy = model.CreatedBy,
        // CreatedDate = model.CreatedDate,
        // IsActive = model.IsActive,

        // };
        // pcBal.SavePatientCarePlan(patinetCareModel);
        // }
        // }

        // return Json(newId);
        // }

        /// <summary>
        ///     Saves the lab speciman order activity.
        /// </summary>
        /// <param name="model">
        ///     The model.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult SaveLabSpecimanOrderActivity(OrderActivity model)
        {
            var result = -1;
            using (
                var bal = new OrderActivityBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var userId = Helpers.GetLoggedInUserId();
                var corporateId = Helpers.GetSysAdminCorporateID();
                var facilityId = Helpers.GetDefaultFacilityId();
                var currentdatetime = Helpers.GetInvariantCultureDateTime();
                var activityId = model.OrderActivityID;
                var labOrderactivity = bal.GetOrderActivityByID(activityId);
                labOrderactivity.CorporateID = corporateId;
                labOrderactivity.FacilityID = facilityId;
                if (model.OrderActivityID > 0)
                {
                    labOrderactivity.ModifiedBy = userId;
                    labOrderactivity.ModifiedDate = currentdatetime;
                    labOrderactivity.OrderActivityStatus = labOrderactivity.OrderActivityStatus != 4
                                                           || labOrderactivity.OrderActivityStatus != 3
                                                               ? 30
                                                               : model.OrderActivityStatus;
                    labOrderactivity.ResultValueMax = model.ResultValueMax;
                    bal.AddUptdateOrderActivity(labOrderactivity);
                }

                result = labOrderactivity.OrderActivityID;
            }

            return Json(result);
        }

        /// <summary>
        ///     Saves the open order activity schedule.
        /// </summary>
        /// <param name="model">
        ///     The model.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult SaveOpenOrderActivitySchedule(OrderActivityCustomModel model)
        {
            var result = -1;
            using (
                var bal = new OrderActivityBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var userId = Helpers.GetLoggedInUserId();
                var corporateId = Helpers.GetSysAdminCorporateID();
                var facilityId = Helpers.GetDefaultFacilityId();
                var currentdatetime = Helpers.GetInvariantCultureDateTime();
                var activityId = model.OrderActivityID;
                model.ExecutedBy = userId;
                //model.ExecutedDate = DateTime.Now;
                //model.ExecutedDate = Helpers.GetInvariantCultureDateTime();
                model.CorporateID = corporateId;
                model.FacilityID = facilityId;
                if (model.OrderActivityID > 0)
                {
                    var objOrderActivityBal = new OrderActivityBal();
                    var obj = objOrderActivityBal.GetOrderActivityByID(model.OrderActivityID);
                    model.ModifiedBy = userId;
                    model.ModifiedDate = currentdatetime;
                    model.ExecutedQuantity = model.OrderCategoryID
                                             == Convert.ToInt32(OrderTypeCategory.PathologyandLaboratory)
                                                 ? 1
                                                 : model.ExecutedQuantity;
                    model.OrderActivityStatus = model.OrderActivityStatus == 2 || model.OrderActivityStatus == 4
                                                    ? model.OrderCategoryID
                                                      == Convert.ToInt32(OrderTypeCategory.PathologyandLaboratory)
                                                          ? model.OrderActivityStatus == 4
                                                                ? model.OrderActivityStatus
                                                                : 2
                                                          : model.OrderActivityStatus
                                                    : model.OrderActivityStatus;
                    model.BarCodeValue = obj.BarCodeValue;
                    model.BarCodeHtml = obj.BarCodeHtml;
                    var openActivityItemObj = new OrderActivityMapper().MapCustomModelToModel(model);
                    bal.AddUptdateOrderActivity(openActivityItemObj);
                }
                else
                {
                    model.CreatedBy = userId;
                    model.CreatedDate = currentdatetime;
                }

                if (model.PartiallyExecutedBool != null && model.ExecutedQuantity != model.OrderActivityQuantity
                    && (bool)model.PartiallyExecutedBool)
                {
                    var quantityRemaining = Convert.ToDecimal(model.OrderActivityQuantity - model.ExecutedQuantity);
                    var isExecuted = bal.CreatePartiallyexecutedActivity(
                        model.OrderActivityID,
                        quantityRemaining,
                        model.PartiallyExecutedstatus);
                }
                // Apply Order Activity To Bill, added by Amit Jain on 24122014
                bal.ApplyOrderActivityToBill(
                    corporateId,
                    facilityId,
                    Convert.ToInt32(model.EncounterID),
                    string.Empty,
                    0);
                var orderActivities = bal.GetOrderActivitiesByOrderId(Convert.ToInt32(model.OrderID));
                var openorderactivties =
                    orderActivities.Any(
                        x =>
                        x.OrderActivityStatus == Convert.ToInt32(OpenOrderActivityStatus.Open)
                        || x.OrderActivityStatus == 0 || x.OrderActivityStatus == 40);

                // Update the Order Status
                if (openorderactivties)
                {
                    if (activityId == 0)
                    {
                        openorderactivties = !UpdateCurrentOpenOrderActivties(Convert.ToInt32(model.OrderID));
                    }
                }

                if (!openorderactivties)
                {
                    using (
                        var ordersBal = new OpenOrderBal(
                            Helpers.DefaultCptTableNumber,
                            Helpers.DefaultServiceCodeTableNumber,
                            Helpers.DefaultDrgTableNumber,
                            Helpers.DefaultDrugTableNumber,
                            Helpers.DefaultHcPcsTableNumber,
                            Helpers.DefaultDiagnosisTableNumber))
                    {
                        /*
                         * Changes By: Amit Jain
                         * On: 01 March, 2016
                         * Purpose: Just updates Current Open Order Status, nothing to do with Order Activities here below
                         * Earlier, Order Activities having ExecutedQuantities were deleted by using the Method AddUpdatePhysicianOrder
                         * And that method has been updated and so, now commented below
                         * Now, changed the Method with the new one i.e. UpdateOpenOrderStatus
                         */

                        //***************************Code Changes start here*******************************************************

                        //var openorder = ordersBal.GetOpenOrderDetail(Convert.ToInt32(model.OrderID));
                        //openorder.OrderStatus = Convert.ToString((int)OrderStatus.OnBill);
                        //openorder.ModifiedBy = userId;
                        //openorder.ModifiedDate = currentdatetime;
                        //ordersBal.AddUpdatePhysicianOpenOrder(openorder);

                        ordersBal.UpdateOpenOrderStatus(
                            Convert.ToInt32(model.OrderID),
                            Convert.ToString((int)OrderStatus.OnBill),
                            userId,
                            currentdatetime);

                        //***************************Code Changes End here*********************************************************
                    }
                }

                result = model.OrderActivityID;
            }
            if (result > 0)
            {
                var encounterId = model.EncounterID.Value;
                var userId = Helpers.GetLoggedInUserId();
                var corporateid = Helpers.GetSysAdminCorporateID();
                var facilityid = Helpers.GetDefaultFacilityId();
                var orderBal = new OpenOrderBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber);


                //DB Call to get all records related to Orders tab
                var ordersViewData = orderBal.OrdersViewData(userId, 100, corporateid, facilityid, Convert.ToInt32(encounterId), "1024,3102,1011,2305,963", 0, "7", string.Empty);

                var orderActivities = ordersViewData.OrderActivities;

                // Order Activities Section, starts here
                var openActStatuses = new[] { 0, 1, 30, 20, 40 };
                var openOrderActivityList = orderActivities.Where(a => openActStatuses.Contains(Convert.ToInt32(a.OrderActivityStatus))).ToList();
                //Orders Section
                var orders = ordersViewData.OpenOrders.ToList();
                var openOrderList = orders.Where(a => a.OrderStatus.Equals("1")).ToList();

                var closedOrderStatuses = new[] { "2", "3", "4", "9" };
                var closedOrdersList = orders.Where(a => closedOrderStatuses.Contains(a.OrderStatus)).ToList();
                //Orders Section

                //json data to return to view
                var jsonData =
                    new
                    {
                        Status = encounterId > 0 ? true : false,
                        OpenOrdersList = openOrderList.Select(x => new[] { Convert.ToString(x.OpenOrderID), x.OrderTypeName, x.OrderCode, x.OrderDescription, x.CategoryName, x.SubCategoryName,
                        x.Status, x.FrequencyText, x.Quantity.HasValue ? Convert.ToString(x.Quantity.Value) : string.Empty, x.PeriodDays, x.OrderNotes, string.Empty }),
                        ClosedOrdersList = closedOrdersList.Select(x => new[] {x.Quantity.HasValue?x.Quantity.Value.ToString():string.Empty,x.OrderCode,x.OrderDescription,x.FrequencyText,x.CategoryName,
                    x.SubCategoryName,x.PeriodDays,x.OrderNotes,x.Status}),
                        OpenOrderActivityList = openOrderActivityList.Select(x => new[] {x.Status, Convert.ToString(x.ShowEditAction), Convert.ToString(x.OrderActivityID), Convert.ToString(x.OrderCategoryID),
                                x.OrderTypeName, x.OrderCode, x.OrderDescription, x.CategoryName, x.SubCategoryName, x.OrderScheduleDate.HasValue ? x.OrderScheduleDate.Value.ToString("d") : string.Empty, string.Empty })
                    };
                //json data to return to view


                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            return Json(result);
        }

        //public ActionResult SavePatientCarePlan(List<PatientCarePlanCustomModel> model)
        //{
        //    // Initialize the newId variable 
        //    int corporateId = Helpers.GetSysAdminCorporateID();
        //    int facilityId = Helpers.GetDefaultFacilityId();
        //    DateTime dateTime = Helpers.GetInvariantCultureDateTime();
        //    int newId = -1;
        //    int userId = Helpers.GetLoggedInUserId();
        //    var cBal = new CarePlanTaskBal();

        //    // Check if Model is not null 
        //    if (model != null)
        //    {
        //        using (var bal = new PatientCarePlanBal())
        //        {
        //            foreach (var patientCarePlan in model)
        //            {
        //                var careId = cBal.CarePlanId(corporateId, facilityId, Convert.ToInt32(patientCarePlan.TaskId));
        //                patientCarePlan.CorporateId = corporateId;
        //                patientCarePlan.FacilityId = facilityId;
        //                patientCarePlan.IsActive = true;
        //                if (patientCarePlan.Id > 0)
        //                {
        //                    patientCarePlan.ModifiedBy = userId;
        //                    patientCarePlan.ModifiedDate = dateTime;
        //                    patientCarePlan.CarePlanId = careId.ToString();
        //                }
        //                else
        //                {
        //                    patientCarePlan.CreatedDate = dateTime;
        //                    patientCarePlan.CreatedBy = userId;
        //                    patientCarePlan.CarePlanId = careId.ToString();
        //                }
        //            }
        //            // Call the AddPatientCarePlan Method to Add / Update current PatientCarePlan
        //            newId = bal.SavePatientCarePlanData(model, true);
        //        }
        //    }

        //    return Json(newId);
        //}

        /// <summary>
        ///     Updates the lab order actvity status.
        /// </summary>
        /// <param name="OrderActivityID">
        ///     The order activity identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult UpdateLabOrderActvityStatus(int OrderActivityID)
        {
            using (
                var bal = new OrderActivityBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var orderActivity = bal.GetOrderActivityByID(OrderActivityID);
                orderActivity.OrderActivityStatus = Convert.ToInt32(OpenOrderActivityStatus.LabSectionWaitingForResult);
                var oGenerateBarCode = new GenerateBarCode
                {
                    PatientId = Convert.ToString(orderActivity.PatientID),
                    EncounterId = Convert.ToInt32(orderActivity.EncounterID),
                    OrderActivityId = OrderActivityID
                };
                var obj = CreateCodeInLabSpecimen(oGenerateBarCode);
                orderActivity.BarCodeHtml = obj.BarCodeHtml;
                orderActivity.BarCodeValue = obj.BarCodeReadValue;
                bal.AddUptdateOrderActivity(orderActivity);
            }

            return Json(1);
        }

        /// <summary>
        ///     Get generated bar code html data according to order activity
        /// </summary>
        /// <param name="orderActivityId"></param>
        /// <returns></returns>
        public ActionResult ShowGeneratedBarCode(string orderActivityId)
        {
            //string retValue;
            using (var bal = new OrderActivityBal())
            {
                //var bc = bal.GetOrderActivityByID(Convert.ToInt32(orderActivityId));
                //retValue = bc.BarCodeHtml;

                var bc = bal.GetBarCodeDetails(Convert.ToInt32(orderActivityId));
                if (bc != null && !string.IsNullOrEmpty(bc.BarCodeReadValue))
                {
                    var barCodeHtmlTemplate = ResourceKeyValues.GetFileText("barcodeview");
                    if (!string.IsNullOrEmpty(barCodeHtmlTemplate))
                    {
                        var html =
                            barCodeHtmlTemplate.Replace("@PatientFullName", bc.PatientName)
                                .Replace("@BarCodeDate", bc.CollectionDateTime)
                                .Replace("@PatientAge", bc.Age)
                                .Replace("@Gender", bc.Gender)
                                .Replace("@ImageUrl", Helpers.ResolveUrl2(bc.BarCodeHtml))
                                .Replace("@EncounterNumber", bc.BarCodeNumbering)
                                .Replace("@CurrentSite", string.Empty)
                                .Replace("@CurrentDateTime", Helpers.GetInvariantCultureDateTime().ToString("g"))
                                .Replace("@User", bc.LoggedInUserName)
                                .Replace("@OrderType", bc.OrderType);

                        return Json(html, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json("0", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     Updates the labtest order.
        /// </summary>
        /// <param name="Id">
        ///     The identifier.
        /// </param>
        /// <param name="orserstatus">
        ///     The orserstatus.
        /// </param>
        /// <param name="comments">
        ///     The comments.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult UpdateLabtestOrder(int Id, int orserstatus, string comments)
        {
            // var EncounterId = 100220141;
            using (
                var encounterComm = new OpenOrderBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var order = encounterComm.GetOpenOrderDetail(Id);
                var userId = Helpers.GetLoggedInUserId();
                var corporateId = Helpers.GetDefaultCorporateId();
                if (corporateId == 0 && userId > 0)
                {
                    corporateId = Helpers.GetSysAdminCorporateID();
                }

                var facilityId = Helpers.GetDefaultFacilityId();
                var days = (Convert.ToDateTime(order.EndDate) - Convert.ToDateTime(order.StartDate)).TotalDays;
                var periodDays = days <= 0 ? 1.0 : days;
                order.PeriodDays = Convert.ToString(periodDays);
                order.FacilityID = facilityId;
                order.CorporateID = corporateId;
                if (order.OpenOrderID > 0)
                {
                    order.ModifiedBy = userId;
                    order.PhysicianID = userId;
                    order.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                    order.OpenOrderPrescribedDate = Helpers.GetInvariantCultureDateTime();
                    order.OrderStatus = Convert.ToString((int)OrderStatus.Closed);
                }
                else
                {
                    order.CreatedBy = userId;
                    order.CreatedDate = Helpers.GetInvariantCultureDateTime();
                    order.PhysicianID = userId;
                    order.OpenOrderPrescribedDate = Helpers.GetInvariantCultureDateTime();

                    // order.EncounterID = EncounterId;
                }

                var orderId = encounterComm.AddUpdatePhysicianOpenOrder(order);
                return Json(orderId);
            }
        }

        /// <summary>
        ///     Updates the open order activities.
        /// </summary>
        /// <param name="orderid">
        ///     The orderid.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult UpdateOpenOrderActivities(int orderid)
        {
            return Json(UpdateCurrentOpenOrderActivties(orderid));
        }

        public ActionResult GetPreENcounterList(int ecounterId, int patinetId)
        {
            using (var bal = new EncounterBal())
            {
                var list = bal.GetPreActiveEncounters(ecounterId, patinetId);
                return PartialView(PartialViews.PreEvaluationList, list);
            }
        }

        /// <summary>
        ///     Fills the and download PDF.
        /// </summary>
        /// <param name="PatientId">The patient identifier.</param>
        /// <param name="EncounterId">The encounter identifier.</param>
        /// <param name="NurseFormId">The nurse form identifier.</param>
        /// <returns></returns>
        public ActionResult fillAndDownloadPDF(string PatientId, string EncounterId, string NurseFormId)
        {
            var currentDateTime = Helpers.GetInvariantCultureDateTime();
            /*string pathToPdf = Server.MapPath("~") +
                               "/AssessmentForm/635895167513330496-1095-1112-635895167036800496NurseAssessmentTriageForm.pdf";
            string pathToHtml = Path.ChangeExtension(pathToPdf, ".htm");

            // Convert PDF file to HTML file
            SautinSoft.PdfFocus f = new SautinSoft.PdfFocus();
            // Let's force the component to store images inside HTML document
            // using base-64 encoding
            f.HtmlOptions.IncludeImageInHtml = true;
            f.HtmlOptions.Title = "Simple text";
            f.OpenPdf(pathToPdf);

            if (f.PageCount > 0)
            {
                int result = f.ToHtml(pathToHtml);

                //Show HTML document in browser
                if (result == 0)
                {
                    
                }
            }
            return Json(1);*/

            #region "STEP1GETTHEVALUEFORMDATABASE"

            var argumentList = new ArgumentList();
            var cid = Convert.ToInt32(Session["clientID"]);
            if (cid == 0)
            {
                cid = 1;
            }
            argumentList.ClientID = 1;
            var pdfControlList = new List<PdfControlList>();
            var mySetting = ConfigurationManager.ConnectionStrings["BillingEntities"];
            foreach (var item in
                new PDFTextFields().AddTextFields(argumentList, mySetting.ConnectionString, PatientId, EncounterId))
            {
                pdfControlList.Add(
                    new PdfControlList { ControlName = item.ControlName, ControlValue = item.ControlValue });
            }

            #endregion

            var url = new List<string>();
            var fileName = string.Empty;
            if (NurseFormId == "101")
            {
                //url.Add("http://" + Convert.ToString(Request.Url.Authority) + @"/Documents/PDFTemplates/" + @"NurseAssessment.pdf");
                //url.Add("http://staging.omnipresenthealthcare.com/Documents/PDFTemplates/" + @"NurseAssessment.pdf");
                url.Add(Server.MapPath("~") + "/Documents/PDFTemplates/" + @"NurseAssessment.pdf");
                //STEP 1: SET THE TEMPLATE PATH
                fileName = PatientId + "-" + EncounterId + "-" + currentDateTime.Ticks + "NurseAssessmentForm.pdf";
                //STEP 2: GENERATE THE UNIQUE FILE NAME
                FileName = fileName; //STEP 3: PASS FILE NAME IN CORE
            }
            else if (NurseFormId == "103")
            {
                //url.Add("http://staging.omnipresenthealthcare.com/Documents/PDFTemplates/" + @"TriageForm.pdf");
                url.Add(Server.MapPath("~") + "/Documents/PDFTemplates/" + @"TriageForm.pdf");
                //STEP 1: SET THE TEMPLATE PATH
                fileName = PatientId + "-" + EncounterId + "-" + currentDateTime.Ticks + "TriageForm.pdf";
                //STEP 2: GENERATE THE UNIQUE FILE NAME
                FileName = fileName; //STEP 3: PASS FILE NAME IN CORE
            }
            PdfBrowserurl = "http://" + Convert.ToString(Request.Url.Authority) + @"/AssessmentForm/"
                                 + fileName;
            // THIS WILL DISPALY IN IFRAME
            PdfUrlList = url; // TEMPLATE SOURCE.
            ControlList = pdfControlList; // SET KEY VALUE PAIR
            //bindPdfViewer();// CALL THIS METOD AND END
            var UrlAuth = "http://" + Convert.ToString(Request.Url.Authority) + "/AssessmentForm/";
            var value = bindPdfViewer(UrlAuth);
            return Json(value);
        }

        /// <summary>
        ///     Binds the PDF viewer.
        /// </summary>
        /// <param name="UrlAuth">The URL authentication.</param>
        /// <returns></returns>
        public string bindPdfViewer(string UrlAuth)
        {
            var printstatus = false;
            if (Session["docprintstatus"] != null)
            {
                printstatus = Convert.ToBoolean(Session["docprintstatus"]);
            }
            if (_pdfurl != string.Empty) PdfUrlList.Add(_pdfurl);
            if (_bytes == null)
            {
                var pdfbytes = new List<byte[]>();
                if (PdfUrlList != null)
                {
                    foreach (var p in PdfUrlList)
                    {
                        byte[] pdf = null;
                        try
                        {
                            if (!_print) pdf = GeneratePDF(p);
                            else pdf = GeneratePDF(p);
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                        pdfbytes.Add(pdf);
                    }
                    _bytes = concatAndAddContent(pdfbytes);
                }
            }
            System.IO.File.WriteAllBytes(Server.MapPath("~") + "/AssessmentForm/" + FileName, _bytes);
            return UrlAuth + FileName + "`" + FileName;
        }

        /// <summary>
        ///     Generates the PDF.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        protected byte[] GeneratePDF(string path)
        {
            var pdfPath = Path.Combine(path);
            var formFieldMap = PDFHelper.GetFormFieldNames(pdfPath);
            var str = string.Empty;
            foreach (var item in ControlList)
            {
                try
                {
                    formFieldMap[item.ControlName] = item.ControlValue;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return PDFHelper.GeneratePDF(pdfPath, formFieldMap);
        }

        /// <summary>
        ///     Concats the content of the and add.
        /// </summary>
        /// <param name="sourceFiles">The source files.</param>
        /// <returns></returns>
        public static byte[] concatAndAddContent(List<byte[]> sourceFiles)
        {
            var document = new Document();
            var output = new MemoryStream();
            PdfWriter writer = null;
            try
            {
                // Initialize pdf writer
                writer = PdfWriter.GetInstance(document, output);
                writer.PageEvent = new PdfPageEvents();
                // Open document to write
                document.Open();
                var content = writer.DirectContent;

                // Iterate through all pdf documents
                for (var fileCounter = 0; fileCounter < sourceFiles.Count; fileCounter++)
                {
                    // Create pdf reader
                    var reader = new PdfReader(sourceFiles[fileCounter]);
                    var numberOfPages = reader.NumberOfPages;

                    // Iterate through all pages
                    for (var currentPageIndex = 1; currentPageIndex <= numberOfPages; currentPageIndex++)
                    {
                        // Determine page size for the current page
                        document.SetPageSize(reader.GetPageSizeWithRotation(currentPageIndex));

                        // Create page
                        document.NewPage();
                        var importedPage = writer.GetImportedPage(reader, currentPageIndex);

                        // Determine page orientation
                        var pageOrientation = reader.GetPageRotation(currentPageIndex);
                        if ((pageOrientation == 90) || (pageOrientation == 270))
                        {
                            content.AddTemplate(
                                importedPage,
                                0,
                                -1f,
                                1f,
                                0,
                                0,
                                reader.GetPageSizeWithRotation(currentPageIndex).Height);
                        }
                        else
                        {
                            content.AddTemplate(importedPage, 1f, 0, 0, 1f, 0, 0);
                        }
                    }
                    reader.Close();
                }
                // result = output.GetBuffer();
            }
            catch (Exception ex)
            {
                throw ex;
                //throw new Exception("There has an unexpected exception" +
                //      " occured during the pdf merging process.", exception);
                //CommonClass.ShowError(ex, Page);
            }
            finally
            {
                document.Close();
                writer.Close();
                output.Close();
                output.Dispose();
            }
            return output.GetBuffer();
            //return result;
        }

        /// <summary>
        ///     Signatures the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public ActionResult Signature([FromBody] Signature data)
        {
            var photo = Convert.FromBase64String(data.Value);

            var dir = new DirectoryInfo(HostingEnvironment.ApplicationPhysicalPath);

            //using (System.IO.FileStream fs = System.IO.File.Create(Path.Combine(dir.FullName, string.Format("Img_{0}.png", Guid.NewGuid()))))
            using (
                var fs =
                    System.IO.File.Create(
                        Path.Combine(
                            dir.FullName + "AssessmentForm/",
                            string.Format("{0}.png", data.FileName.Replace(".pdf", "")))))
            {
                fs.Write(photo, 0, photo.Length);
            }

            #region unusedcode

            /*Document doc = new Document(PageSize.A4, 10f, 10f, 100f, 0f);

            string pdfFilePath = Server.MapPath("~");

            //PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(pdfFilePath + "/" + data.FileName, FileMode.Create));

            doc.Open();

            try
            {

                Paragraph paragraph = new Paragraph("Getting Started ITextSharp.");

                string imageURL = Server.MapPath("~") + "/" + data.FileName.Replace(".pdf", ".png");

                iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imageURL);

                //Resize image depend upon your need

                jpg.ScaleToFit(140f, 120f);

                //Give space before image

                jpg.SpacingBefore = 10f;

                //Give some space after the image

                jpg.SpacingAfter = 1f;

                jpg.Alignment = Element.ALIGN_LEFT;



                doc.Add(paragraph);

                doc.Add(jpg);



            }

            catch (Exception ex)

            { }

            finally
            {

                doc.Close();

            }*/

            #endregion

            var serverPath = Server.MapPath("~");
            /*using (Stream inputPdfStream = new FileStream(serverPath + "/1095-1112-635894194955615895NurseAssessmentForm.pdf", FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream inputImageStream = new FileStream(serverPath + "/1095-1112-635894194955615895NurseAssessmentForm.png", FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream outputPdfStream = new FileStream(serverPath + "/1095-1112-635894194955615895NurseAssessmentForm.pdf", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var reader = new PdfReader(inputPdfStream);
                var stamper = new PdfStamper(reader, outputPdfStream);
                var pdfContentByte = stamper.GetOverContent(2);

                Image image = Image.GetInstance(inputImageStream);
                float t = 500;
                float t1 = 12;
                image.SetAbsolutePosition(t, t1);
                image.WidthPercentage = 20;
                image.ScaleToFit(30, 30);
                image.SpacingBefore = 50f;
                image.SpacingAfter = 10f;
                image.Alignment = Element.ALIGN_CENTER;
                pdfContentByte.AddImage(image);
                stamper.Close();
            }*/
            var currentDateTime = Helpers.GetInvariantCultureDateTime();
            var ticks = currentDateTime.Ticks;
            using (
                Stream inputPdfStream = new FileStream(
                    serverPath + "/AssessmentForm/" + data.FileName,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read))
            using (
                Stream inputImageStream =
                    new FileStream(
                        serverPath + "/AssessmentForm/" + data.FileName.Replace(".pdf", ".png"),
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.Read))
            using (
                Stream outputPdfStream =
                    new FileStream(
                        serverPath + "/AssessmentForm/" + Convert.ToString(ticks) + "-" + data.FileName,
                        FileMode.Create,
                        FileAccess.Write,
                        FileShare.None))
            {
                var reader = new PdfReader(inputPdfStream);
                var stamper = new PdfStamper(reader, outputPdfStream);
                var pdfContentByte = stamper.GetOverContent(Convert.ToInt32(data.PageNumber));

                var image = Image.GetInstance(inputImageStream);
                if (Convert.ToInt32(data.PageNumber) == 1)
                {
                    float absoluteX = 430;
                    float absoluteY = -15;
                    image.SetAbsolutePosition(absoluteX, absoluteY);
                }
                else if (Convert.ToInt32(data.PageNumber) == 2)
                {
                    float absoluteX = 430;
                    float absoluteY = 25;
                    image.SetAbsolutePosition(absoluteX, absoluteY);
                }
                image.WidthPercentage = 100;
                image.ScaleToFit(100, 100);
                image.SpacingBefore = 50f;
                image.SpacingAfter = 10f;
                image.Alignment = Element.ALIGN_CENTER;
                pdfContentByte.AddImage(image);
                stamper.Close();
            }

            var oDocumentsTemplates = new DocumentsTemplates
            {
                DocumentTypeID = Convert.ToInt32(data.NurseFormId),
                DocumentName = data.NurseFormText,
                DocumentNotes = data.NurseFormText,
                AssociatedID = Convert.ToInt32(data.PatientId),
                AssociatedType = 189,
                FileName = data.FileName,
                FilePath =
                                                  "http://"
                                                  + Convert.ToString(Request.Url.Authority)
                                                  + "/AssessmentForm/" + Convert.ToString(ticks) + "-"
                                                  + data.FileName,
                CreatedDate = currentDateTime,
                CreatedBy = Helpers.GetLoggedInUserId(),
                CorporateID = Helpers.GetSysAdminCorporateID(),
                FacilityID = Helpers.GetDefaultFacilityId(),
                PatientID = Convert.ToInt32(data.PatientId),
                EncounterID = Convert.ToInt32(data.EncounterId),
                ExternalValue3 = "4950"
            };
            var oDocumentsTemplatesBal = new DocumentsTemplatesBal();
            oDocumentsTemplatesBal.AddUpdateDocumentTempate(oDocumentsTemplates);
            if (System.IO.File.Exists(serverPath + "/AssessmentForm/" + data.FileName))
            {
                System.IO.File.Delete(serverPath + "/AssessmentForm/" + data.FileName);
            }
            return
                Json(
                    "http://" + Convert.ToString(Request.Url.Authority) + "/AssessmentForm/"
                    + Convert.ToString(ticks) + "-" + data.FileName);
        }

        /// <summary>
        ///     Saves the signature in enm.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public ActionResult SaveSignatureInENM([FromBody] Signature data)
        {
            var currentDateTime = Helpers.GetInvariantCultureDateTime();

            var serverPath = Server.MapPath("~");
            var ticks = currentDateTime.Ticks;
            var photo = Convert.FromBase64String(data.Value);

            var dir = new DirectoryInfo(HostingEnvironment.ApplicationPhysicalPath);

            using (
                var fs =
                    System.IO.File.Create(
                        Path.Combine(
                            dir.FullName + "EvaluationForm/",
                            string.Format("{0}.png", Convert.ToString(ticks) + "-" + data.EnmFileName))))
            {
                fs.Write(photo, 0, photo.Length);
            }

            // using (Stream inputImageStream = new FileStream(serverPath + "/EvaluationForm/" + data.EnmFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            //{

            //  Image image = Image.GetInstance(inputImageStream);
            //     if (Convert.ToInt32(data.PageNumber) == 1)
            //     {
            //         float absoluteX = 430;
            //         float absoluteY = -15;
            //         image.SetAbsolutePosition(absoluteX, absoluteY);
            //     }
            //     else if (Convert.ToInt32(data.PageNumber) == 2)
            //     {
            //         float absoluteX = 430;
            //         float absoluteY = 25;
            //         image.SetAbsolutePosition(absoluteX, absoluteY);
            //     }
            //     image.WidthPercentage = 100;
            //     image.ScaleToFit(100, 100);
            //     image.SpacingBefore = 50f;
            //     image.SpacingAfter = 10f;
            //     image.Alignment = Element.ALIGN_CENTER;
            //    }

            var oDocumentsTemplates = new DocumentsTemplates
            {
                DocumentTypeID = Convert.ToInt32(data.EnmFromId),
                DocumentName = data.EnmFormText,
                DocumentNotes = data.EnmFormText,
                AssociatedID = Convert.ToInt32(data.PatientId),
                AssociatedType = 25,
                FileName = data.EnmFileName,
                FilePath =
                                                  "http://"
                                                  + Convert.ToString(Request.Url.Authority)
                                                  + "/EvaluationForm/" + Convert.ToString(ticks) + "-"
                                                  + data.EnmFileName + ".png",
                CreatedDate = currentDateTime,
                CreatedBy = Helpers.GetLoggedInUserId(),
                CorporateID = Helpers.GetSysAdminCorporateID(),
                FacilityID = Helpers.GetDefaultFacilityId(),
                PatientID = Convert.ToInt32(data.PatientId),
                EncounterID = Convert.ToInt32(data.EncounterId)
                //ExternalValue3 = "4950"
            };
            var oDocumentsTemplatesBal = new DocumentsTemplatesBal();
            oDocumentsTemplatesBal.AddUpdateDocumentTempate(oDocumentsTemplates);
            if (System.IO.File.Exists(serverPath + "/EvaluationForm/" + data.EnmFileName))
            {
                System.IO.File.Delete(serverPath + "/EvaluationForm/" + data.EnmFileName);
            }
            return
                Json(
                    "http://" + Convert.ToString(Request.Url.Authority) + "/EvaluationForm/"
                    + Convert.ToString(ticks) + "-" + data.EnmFileName + ".png");
        }

        #endregion

        #region patient Summary tab

        /// <summary>
        ///     Patients the summary.
        /// </summary>
        /// <param name="pId">
        ///     The p identifier.
        /// </param>
        /// <param name="sTab">
        ///     The s Tab.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult PatientSummary(int? pId, int? sTab)
        {
            if (!pId.HasValue || pId.Value <= 0)
            {
                return RedirectToAction(
                    ActionResults.patientSearch,
                    ControllerNames.patientSearch,
                    new { messageId = Convert.ToInt32(MessageType.ViewEHR) });
            }

            var vmData = GetSummaryDetails(pId.Value, Convert.ToInt32(sTab));
            return View(vmData);
        }

        /// <summary>
        ///     Patients the summary tab data.
        /// </summary>
        /// <param name="patientId">
        ///     The patient identifier.
        /// </param>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult PatientSummaryTabData(int patientId, int encounterId)
        {
            //using (var orderBal = new OpenOrderBal(
            //        Helpers.DefaultCptTableNumber,
            //        Helpers.DefaultServiceCodeTableNumber,
            //        Helpers.DefaultDrgTableNumber,
            //        Helpers.DefaultDrugTableNumber,
            //        Helpers.DefaultHcPcsTableNumber,
            //        Helpers.DefaultDiagnosisTableNumber))
            //{
            //    var enList = orderBal.GetEncountersListByPatientId(patientId);
            //    using (var mrBal = new MedicalRecordBal())
            //    {
            //        // Updated by Shashank on Oct 28, 2014
            //        using (var nBal = new MedicalNotesBal())
            //        {
            //            // Updated by Shashank on Oct 28, 2014
            //            using (var medicalVitals = new MedicalVitalBal())
            //            {
            //                using (var diagnosisBal = new DiagnosisBal(Helpers.DefaultDiagnosisTableNumber
            //                    , Helpers.DefaultDrgTableNumber))
            //                {
            //                    var currentEncounterId = encounterId;
            //                    var medicalrecords = mrBal.GetMedicalRecord();
            //                    var medicalvitals = medicalVitals.GetCustomMedicalVitalsByPidEncounterId(patientId
            //                        , (int)MedicalRecordType.Vitals, currentEncounterId);

            //                    var mNotes = nBal.GetMedicalNotesByPatientIdEncounterId(patientId, currentEncounterId);
            //                    var allergies = mrBal.GetAlergyRecords(patientId, (int)MedicalRecordType.Allergies);

            //                    var orderStatus = Convert.ToString(OrderStatus.Open);
            //                    var openOrdersList = orderBal.GetPhysicianOrders(currentEncounterId, orderStatus);
            //                    var primarydiagnosisId = 0;
            //                    var dList = diagnosisBal.GetDiagnosisList(patientId, currentEncounterId);
            //                    if (dList.Any())
            //                    {
            //                        var dVm = dList.FirstOrDefault(x => x.DiagnosisType == (int)DiagnosisType.Primary);
            //                        if (dVm != null)
            //                            primarydiagnosisId = dVm.DiagnosisID;
            //                    }

            //                    var riskFactors = medicalVitals.GetRiskFactors(patientId);
            //                    var summaryView = new PatientSummaryView
            //                    {
            //                        PatientInfo = orderBal.GetPatientDetailsByPatientId(patientId),
            //                        OpenOrdersList = openOrdersList,
            //                        EncountersList = enList,
            //                        CurrentEncounterId = currentEncounterId,
            //                        PatientId = patientId,
            //                        MedicalRecordList = medicalrecords,
            //                        DiagnosisId = primarydiagnosisId,
            //                        MedicalVitalList = medicalvitals,
            //                        PatientSummaryNotes = mNotes,
            //                        ClosedOrdersList = orderBal.GetPhysicianOrders(currentEncounterId, OrderStatus.Closed.ToString()),
            //                        AlergyList = allergies,
            //                        DiagnosisList = dList,
            //                        Riskfactors = riskFactors
            //                    };
            //                    return PartialView(PartialViews.PatientSummaryTabView, summaryView);
            //                }
            //            }
            //        }
            //    }
            //}
            var vmData = GetSummaryDetails(patientId, 0);
            return PartialView(PartialViews.PatientSummaryTabView, vmData);
        }

        /// <summary>
        ///     The sort encounter grid.
        /// </summary>
        /// <param name="patientId">
        ///     The patient id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult SortEncounterGrid(int patientId)
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
                var enList = orderBal.GetEncountersListByPatientId(patientId);
                return PartialView(PartialViews.EncounterList, enList);
            }
        }

        /// <summary>
        ///     Sorts the diagnosis grid.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="encId">The enc identifier.</param>
        /// <returns></returns>
        public ActionResult SortDiagnosisGrid(int patientId, int encId)
        {
            using (
                var diagnosisBal = new DiagnosisBal(Helpers.DefaultDiagnosisTableNumber, Helpers.DefaultDrgTableNumber))
            {
                var diagnosisList = diagnosisBal.GetDiagnosisList(patientId, encId);
                return PartialView(PartialViews.PatientDiagnosisList, diagnosisList);
            }
        }

        /// <summary>
        ///     Sorts the patient open orders.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="encId">The enc identifier.</param>
        /// <returns></returns>
        public ActionResult SortPatientOpenOrders(int patientId, int encId)
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
                var orderStatus = OrderStatus.Open.ToString();
                var openOrdersList = orderBal.GetPhysicianOrders(encId, orderStatus);
                return PartialView(PartialViews.PatientOpenOrderList, openOrdersList);
            }
        }

        /// <summary>
        ///     Sorts the patient medical vital.
        /// </summary>
        /// <param name="pId">The p identifier.</param>
        /// <returns></returns>
        public ActionResult SortPatientMedicalVital(int pId)
        {
            var medicalVitals = new MedicalVitalBal();
            var medicalvitals = medicalVitals.GetCustomMedicalVitals(pId, Convert.ToInt32(MedicalRecordType.Vitals));
            return PartialView(PartialViews.PatientMedicalVitalList, medicalvitals);
        }

        /// <summary>
        ///     The sort allergies list.
        /// </summary>
        /// <param name="patientId">
        ///     The patient id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult SortPatientAllergiesList(int patientId)
        {
            var medicalRecordbal = new MedicalRecordBal();
            var viewpath = string.Format("../MedicalRecord/{0}", PartialViews.AlergiesList);
            var allergiesList = medicalRecordbal.GetAlergyRecords(
                patientId,
                Convert.ToInt32(MedicalRecordType.Allergies));
            return PartialView(viewpath, allergiesList);
        }

        #endregion

        #region Sorting Methods

        /// <summary>
        ///     The sort allergies list.
        /// </summary>
        /// <param name="patientId">
        ///     The patient id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult SortAllergiesList(int patientId)
        {
            var medicalRecordbal = new MedicalRecordBal();
            var viewpath = string.Format("../MedicalRecord/{0}", PartialViews.AlergiesList);
            var allergiesList = medicalRecordbal.GetAlergyRecords(
                patientId,
                Convert.ToInt32(MedicalRecordType.Allergies));
            return PartialView(viewpath, allergiesList);
        }

        /// <summary>
        ///     The sort current notesby physician.
        /// </summary>
        /// <param name="patientId">
        ///     The patient id.
        /// </param>
        /// <param name="encounterId">
        ///     The encounter id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult SortCurrentNotesbyPhysician(int patientId, int encounterId)
        {
            var medicalnotesbal = new MedicalNotesBal();
            var viewpath = string.Format("../MedicalNotes/{0}", PartialViews.MedicalNotesListPatientSummary);
            var currentEncounterId = encounterId;
            var patientSummaryNotes = medicalnotesbal.GetMedicalNotesByPatientIdEncounterId(
                patientId,
                currentEncounterId);
            return PartialView(viewpath, patientSummaryNotes);
        }

        /// <summary>
        ///     The sort current order grid.
        /// </summary>
        /// <param name="patientId">
        ///     The patient id.
        /// </param>
        /// <param name="encounterId">
        ///     The encounter id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult SortCurrentOrderGrid(int patientId, int encounterId)
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
                var currentEncounterId = encounterId;
                var orderStatus = OrderStatus.Open.ToString();
                var openOrdersList = orderBal.GetPhysicianOrders(currentEncounterId, orderStatus);
                return PartialView(PartialViews.EncounterList, openOrdersList);
            }
        }

        /// <summary>
        ///     The sort lab closed order list.
        /// </summary>
        /// <param name="encounterid">
        ///     The encounterid.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult SortLabClosedOrderList(int encounterid)
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
                var allEncounterOrders = orderBal.GetOrdersByEncounterid(Convert.ToInt32(encounterid));
                var labordersList =
                    allEncounterOrders.Where(
                        x => x.CategoryId == Convert.ToInt32(OrderTypeCategory.PathologyandLaboratory)).ToList();
                var labclosedordersList =
                    labordersList.Where(
                        x => x.OrderStatus != "0" && x.OrderStatus != Convert.ToInt32(OrderStatus.Open).ToString())
                        .ToList();
                return PartialView(PartialViews.LabClosedOrderList, labclosedordersList);
            }
        }

        /// <summary>
        ///     The sort lab open order list.
        /// </summary>
        /// <param name="encounterid">
        ///     The encounterid.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult SortLabOpenOrderList(int encounterid)
        {
            using (
                var orderbal = new OpenOrderBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var allPhysicianOrders = orderbal.GetOrdersByEncounterid(encounterid);
                var labordersList =
                    allPhysicianOrders.Where(
                        x =>
                        x.EncounterID == encounterid && x.CategoryId == Convert.ToInt32(GlobalCodeCategoryValue.LabTest))
                        .ToList();
                var labopenordersList =
                    labordersList.Where(
                        x => x.OrderStatus == "0" || x.OrderStatus == Convert.ToInt32(OrderStatus.Open).ToString())
                        .ToList();
                return PartialView(PartialViews.LabOpenOrderList, labopenordersList);
            }
        }

        /// <summary>
        ///     The sort medical notes in nurse tab.
        /// </summary>
        /// <param name="patientId">
        ///     The patient id.
        /// </param>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="currentEncounterId">
        ///     The current encounter id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult SortMedicalNotesInNurseTab(int patientId, int type, int currentEncounterId)
        {
            using (var medicalnotesbal = new MedicalNotesBal())
            {
                // var notesList = medicalnotesbal.GetCustomMedicalNotes(patientId,
                // type == Convert.ToInt32(NotesUserType.Physician)
                // ? Convert.ToInt32(NotesUserType.Physician)
                // : Convert.ToInt32(NotesUserType.Nurse));
                var patientSummaryNotes = medicalnotesbal.GetMedicalNotesByPatientIdEncounterId(
                    patientId,
                    currentEncounterId);

                return PartialView(PartialViews.MedicalNotesList, patientSummaryNotes);
            }
        }

        /// <summary>
        ///     The sort medical vital.
        /// </summary>
        /// <param name="pId">
        ///     The p id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult SortMedicalVital(int pId)
        {
            var medicalVitals = new MedicalVitalBal();
            var medicalvitals = medicalVitals.GetCustomMedicalVitals(pId, Convert.ToInt32(MedicalRecordType.Vitals));
            var viewpath = String.Format("../MedicalVital/{0}", PartialViews.MedicalVitalList);
            return PartialView(viewpath, medicalvitals);
        }

        /// <summary>
        ///     The sort note type.
        /// </summary>
        /// <param name="patientId">
        ///     The patient id.
        /// </param>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult SortNoteType(int patientId, int type)
        {
            using (var medicalnotesbal = new MedicalNotesBal())
            {
                // Updated by Krishna on Sep 22, 2015
                var notesList = medicalnotesbal.GetCustomMedicalNotes(
                    patientId,
                    type == Convert.ToInt32(NotesUserType.Physician)
                        ? Convert.ToInt32(NotesUserType.Physician)
                        : Convert.ToInt32(NotesUserType.Nurse));
                var viewpath = string.Format("../MedicalNotes/{0}", PartialViews.MedicalNotesList);
                return PartialView(viewpath, notesList);
            }
        }

        /// <summary>
        ///     The sort orders view data.
        /// </summary>
        /// <param name="encounterId">
        ///     The encounter id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult SortOrdersViewData(string encounterId)
        {
            var userId = Helpers.GetLoggedInUserId();
            var corporateid = Helpers.GetSysAdminCorporateID();
            var facilityid = Helpers.GetDefaultFacilityId();
            using (
                var orderBal = new OpenOrderBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var favoriteOrders = orderBal.GetPhysicanFavoriteOrderedList(userId, facilityid, corporateid);
                return PartialView(PartialViews.FavoriteOrders, favoriteOrders);
            }
        }

        /// <summary>
        ///     Get Previous Diagnosis Data
        /// </summary>
        /// <param name="patientId"></param>
        /// <param name="encounterId"></param>
        /// <returns></returns>
        public ActionResult GetPreviousDiagnosisData(int patientId, int encounterId)
        {
            List<DiagnosisCustomModel> previouslist;
            var pid = Convert.ToInt32(patientId);
            var eid = Convert.ToInt32(encounterId);
            using (
                var bal = new DiagnosisBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var previousDlist = bal.GetPreviousDiagnosisList(pid, eid);
                previouslist = previousDlist.Any() ? previousDlist : new List<DiagnosisCustomModel>();
            }
            var viewpath = string.Format("../Diagnosis/{0}", PartialViews.PreviousDiagnosisList);
            return PartialView(viewpath, previouslist);
        }

        /// <summary>
        ///     The sort lab open order list.
        /// </summary>
        /// <param name="encounterid">
        ///     The encounterid.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult SortLabSpecimanOpenOrderList(int encounterid)
        {
            using (
                var orderActivityBal = new OrderActivityBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var labactivitesobj =
                    //orderActivityBal.GetOrderActivitiesByEncounterId(encounterid);
                    orderActivityBal.GetOrderActivitiesByEncounterIdSP(encounterid);
                var labActivitesListObj =
                    labactivitesobj.Where(
                        x => x.OrderCategoryID == Convert.ToInt32(OrderTypeCategory.PathologyandLaboratory)).ToList();
                var labActivitesOpenListObj =
                    labActivitesListObj.Where(
                        x => x.OrderActivityStatus == 0 || x.OrderActivityStatus == 1 || x.OrderActivityStatus == 20)
                        .ToList();
                return PartialView(PartialViews.LabSpecimanListing, labActivitesOpenListObj);
            }
        }

        /// <summary>
        ///     Sort Lab Closed Order Activity List
        /// </summary>
        /// <param name="encounterid">The Encounter id parameter</param>
        /// <returns></returns>
        public ActionResult SortLabClosedOrderActivityList(int encounterid)
        {
            using (
                var orderActivityBal = new OrderActivityBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var labactivitesobj = orderActivityBal.GetOrderActivitiesByEncounterIdSP(encounterid);
                var labActivitesListObj =
                    labactivitesobj.Where(
                        x => x.OrderCategoryID == Convert.ToInt32(OrderTypeCategory.PathologyandLaboratory)).ToList();
                var labActivitesList =
                    labActivitesListObj.Where(
                        x =>
                        x.OrderActivityStatus != 0
                        && x.OrderActivityStatus != Convert.ToInt32(OpenOrderActivityStatus.Open)
                        && x.OrderActivityStatus != Convert.ToInt32(OpenOrderActivityStatus.LabSectionWaitingForResult)
                        && x.OrderActivityStatus
                        != Convert.ToInt32(OpenOrderActivityStatus.LabSectionWaitingForSpecimen)
                        && x.OrderActivityStatus != Convert.ToInt32(OpenOrderActivityStatus.PartiallyExecutedForResult))
                        .OrderBy(x => x.ExecutedDate)
                        .ToList();
                return PartialView(PartialViews.LabClosedActivtiesList, labActivitesList);
            }
        }

        /// <summary>
        ///     Sorts the lab orders list by physician.
        /// </summary>
        /// <param name="orderType">Type of the order.</param>
        /// <param name="orderStatus">The order status.</param>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public ActionResult SortLabOrdersListByPhysician(string orderType, int orderStatus, int encounterId)
        {
            using (
                var bal = new OrderActivityBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var list = bal.GetLabOrderActivitiesByPhysician(
                    Helpers.GetLoggedInUserId(),
                    orderStatus,
                    orderType,
                    1,
                    encounterId);
                list = list != null ? list.OrderBy(x => x.LabResultTypeStr).ToList() : null;
                return PartialView(PartialViews.PhysicianLabTestView, list);
            }
        }

        /// <summary>
        ///     Sorts the pre evaluation list.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="patientid">The patientid.</param>
        /// <returns></returns>
        public ActionResult SortPreEvaluationList(int encounterId, int patientid)
        {
            var enBal = new EncounterBal();
            var nurseAssessmentfrom = enBal.GetNurseAssessmentData(encounterId, patientid);
            return PartialView(PartialViews.PreEvaluationList, nurseAssessmentfrom);
        }

        /// <summary>
        ///     Sorts the medical history list.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="patientid">The patientid.</param>
        /// <returns></returns>
        public ActionResult SortMedicalHistoryList(int encounterId, int patientid)
        {
            using (var bal = new MedicalHistoryBal(Helpers.DefaultDrugTableNumber))
            {
                var list = bal.GetMedicalHistory(patientid, encounterId);
                var viewpath = string.Format("../MedicalHistory/{0}", PartialViews.MedicalHistoryList);
                return PartialView(viewpath, list);
            }
        }

        /// <summary>
        /// Sorts the physicians allorders.
        /// </summary>
        /// <param name="encounterid">The encounterid.</param>
        /// <returns></returns>
        public ActionResult SortPhysiciansAllorders(int encounterid)
        {
            var orderBal = new OpenOrderBal(
                Helpers.DefaultCptTableNumber,
                Helpers.DefaultServiceCodeTableNumber,
                Helpers.DefaultDrgTableNumber,
                Helpers.DefaultDrugTableNumber,
                Helpers.DefaultHcPcsTableNumber,
                Helpers.DefaultDiagnosisTableNumber);
            var userId = Helpers.GetLoggedInUserId();
            var corporateid = Helpers.GetSysAdminCorporateID();
            var facilityid = Helpers.GetDefaultFacilityId();
            var allPhysicianOrders = orderBal.GetOrdersByPhysician(userId, corporateid, facilityid);
            return PartialView(PartialViews.PhyAllOrdersSummary, allPhysicianOrders);

        }

        /// <summary>
        ///     The physician fav.
        /// </summary>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult PhysicianFav()
        {
            var userId = Helpers.GetLoggedInUserId();
            var corporateid = Helpers.GetSysAdminCorporateID();
            var facilityid = Helpers.GetDefaultFacilityId();
            var orderBal = new OpenOrderBal(
                Helpers.DefaultCptTableNumber,
                Helpers.DefaultServiceCodeTableNumber,
                Helpers.DefaultDrgTableNumber,
                Helpers.DefaultDrugTableNumber,
                Helpers.DefaultHcPcsTableNumber,
                Helpers.DefaultDiagnosisTableNumber);
            var favoriteOrders = orderBal.GetPhysicanFavoriteOrderedList(userId, facilityid, corporateid);
            var viewpath = string.Format("../PhysicianFavorites/{0}", PartialViews.FavoriteOrders);
            return PartialView(viewpath, favoriteOrders);
        }

        /// <summary>
        ///     The physician fav.
        /// </summary>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult PhysicianFavSearch()
        {
            var userId = Helpers.GetLoggedInUserId();
            var corporateid = Helpers.GetSysAdminCorporateID();
            var facilityid = Helpers.GetDefaultFacilityId();
            var orderBal = new OpenOrderBal(
                Helpers.DefaultCptTableNumber,
                Helpers.DefaultServiceCodeTableNumber,
                Helpers.DefaultDrgTableNumber,
                Helpers.DefaultDrugTableNumber,
                Helpers.DefaultHcPcsTableNumber,
                Helpers.DefaultDiagnosisTableNumber);
            var favoriteOrders = orderBal.GetPhysicanFavoriteOrderedList(userId, facilityid, corporateid);
            return PartialView(PartialViews.FavoriteOrdersSearch, favoriteOrders);
        }

        /// <summary>
        /// Sorts the physician all search.
        /// </summary>
        /// <param name="encounterid">The encounterid.</param>
        /// <returns></returns>
        public ActionResult SortPhysicianAllSearch(int encounterid)
        {
            var orderBal = new OpenOrderBal(
                Helpers.DefaultCptTableNumber,
                Helpers.DefaultServiceCodeTableNumber,
                Helpers.DefaultDrgTableNumber,
                Helpers.DefaultDrugTableNumber,
                Helpers.DefaultHcPcsTableNumber,
                Helpers.DefaultDiagnosisTableNumber);
            var userId = Helpers.GetLoggedInUserId();
            var corporateid = Helpers.GetSysAdminCorporateID();
            var facilityid = Helpers.GetDefaultFacilityId();
            var allPhysicianOrders = orderBal.GetOrdersByPhysician(userId, corporateid, facilityid);
            var viewpath = string.Format("../PhysicianFavorites/{0}", PartialViews.PhyAllOrders);
            return PartialView(viewpath, allPhysicianOrders);

        }

        /// <summary>
        /// Sorts the diagnosis tab grid.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public ActionResult SortDiagnosisTabGrid(int? patientId, int? encounterId)
        {
            var daignosisBal = new DiagnosisBal(Helpers.DefaultDiagnosisTableNumber, Helpers.DefaultDrgTableNumber);
            var diagnosislist = daignosisBal.GetDiagnosisList(Convert.ToInt32(patientId), Convert.ToInt32(encounterId));
            return PartialView(PartialViews.DiagnosisListEHR, diagnosislist);
        }

        #endregion

        #region Orders Tab Data

        /// <summary>
        ///     Orderses the view data.
        /// </summary>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        //public ActionResult OrdersViewData(string encounterId)
        //{
        //    var userId = Helpers.GetLoggedInUserId();
        //    var corporateid = Helpers.GetSysAdminCorporateID();
        //    var facilityid = Helpers.GetDefaultFacilityId();
        //    var orderBal = new OpenOrderBal(
        //        Helpers.DefaultCptTableNumber,
        //        Helpers.DefaultServiceCodeTableNumber,
        //        Helpers.DefaultDrgTableNumber,
        //        Helpers.DefaultDrugTableNumber,
        //        Helpers.DefaultHcPcsTableNumber,
        //        Helpers.DefaultDiagnosisTableNumber);
        //    var mostRecentOrders = orderBal.GetMostOrderedList(userId, 100); //---- Complexity has reduced to 1
        //    var allPhysicianOrders = orderBal.GetOrdersByPhysician(userId, corporateid, facilityid);
        //    //---- Complexity has reduced to 1

        //    var favoriteOrders = orderBal.GetPhysicanFavoriteOrderedList(userId, facilityid, corporateid);
        //    //---- Complexity has reduced to 1

        //    var allEncounterOrders = orderBal.GetOpenOrdersByEncounterId(Convert.ToInt32(encounterId));
        //    //---- Complexity was 7 now reduced to 1
        //    var closedOrdersList = new List<OpenOrderCustomModel>();
        //    var openOrderList = new List<OpenOrderCustomModel>();

        //    var closedOrderActivityList = new List<OrderActivityCustomModel>();
        //    var openOrderActivityList = new List<OrderActivityCustomModel>();

        //    using (
        //        var orderActivityBal = new OrderActivityBal(
        //            Helpers.DefaultCptTableNumber,
        //            Helpers.DefaultServiceCodeTableNumber,
        //            Helpers.DefaultDrgTableNumber,
        //            Helpers.DefaultDrugTableNumber,
        //            Helpers.DefaultHcPcsTableNumber,
        //            Helpers.DefaultDiagnosisTableNumber))
        //    {
        //        var encounterActivitesobj =
        //            orderActivityBal.GetOrderActivitiesByEncounterIdSP(Convert.ToInt32(encounterId));
        //        var encounterActivitesClosedListObj =
        //            encounterActivitesobj.Where(
        //                x =>
        //                x.OrderActivityStatus != 0
        //                && x.OrderActivityStatus != Convert.ToInt32(OpenOrderActivityStatus.Open)
        //                && x.OrderActivityStatus != Convert.ToInt32(OpenOrderActivityStatus.LabSectionWaitingForResult)
        //                && x.OrderActivityStatus
        //                != Convert.ToInt32(OpenOrderActivityStatus.LabSectionWaitingForSpecimen)
        //                && x.OrderActivityStatus != Convert.ToInt32(OpenOrderActivityStatus.PartiallyExecutedForResult))
        //                .OrderBy(x => x.ExecutedDate)
        //                .ToList();
        //        var encounterActivitesOpenListObj =
        //            encounterActivitesobj.Where(
        //                x =>
        //                x.OrderActivityStatus == 0
        //                || x.OrderActivityStatus == Convert.ToInt32(OpenOrderActivityStatus.Open)
        //                || x.OrderActivityStatus == Convert.ToInt32(OpenOrderActivityStatus.LabSectionWaitingForResult)
        //                || x.OrderActivityStatus
        //                == Convert.ToInt32(OpenOrderActivityStatus.LabSectionWaitingForSpecimen)
        //                || x.OrderActivityStatus == Convert.ToInt32(OpenOrderActivityStatus.PartiallyExecutedForResult))
        //                .OrderBy(x => x.OrderScheduleDate)
        //                .ToList();
        //        closedOrderActivityList = encounterActivitesClosedListObj;
        //        openOrderActivityList = encounterActivitesOpenListObj;
        //    }

        //    if (allEncounterOrders.Count > 0)
        //    {
        //        openOrderList = allEncounterOrders.Where(a => a.OrderStatus.Equals("1")).ToList();
        //        closedOrdersList =
        //            allEncounterOrders.Where(
        //                a =>
        //                a.OrderStatus.Equals("3") || a.OrderStatus.Equals("4") || a.OrderStatus.Equals("2")
        //                || a.OrderStatus.Equals("9")).ToList();
        //    }
        //    var patientId = orderBal.GetPatientIdByEncounterId(Convert.ToInt32(encounterId));
        //    var futureOrdersList =
        //        new FutureOpenOrderBal(
        //            Helpers.DefaultCptTableNumber,
        //            Helpers.DefaultServiceCodeTableNumber,
        //            Helpers.DefaultDrgTableNumber,
        //            Helpers.DefaultDrugTableNumber,
        //            Helpers.DefaultHcPcsTableNumber,
        //            Helpers.DefaultDiagnosisTableNumber).GetFutureOpenOrderByPatientId(Convert.ToInt32(patientId));

        //    var ordersFullView = new OrdersFullView
        //    {
        //        AllPhysicianOrders =
        //                                     allPhysicianOrders.DistinctBy(x => x.OrderCode).ToList(),
        //        ClosedOrdersList = closedOrdersList,
        //        EncounterOrder =
        //                                     new OpenOrder
        //                                     {
        //                                         StartDate =
        //                                                 Helpers.GetInvariantCultureDateTime(),
        //                                         EndDate =
        //                                                 Helpers.GetInvariantCultureDateTime(),
        //                                         OrderStatus =
        //                                                 Convert.ToInt32(OrderStatus.Open)
        //                                                 .ToString()
        //                                     },
        //        FavoriteOrders = favoriteOrders,
        //        MostRecentOrders = mostRecentOrders,
        //        OpenOrdersList = openOrderList,
        //        SearchedOrders = new List<OpenOrderCustomModel>(),
        //        ClosedOrderActivityList = closedOrderActivityList,
        //        OpenOrderActivityList = openOrderActivityList,
        //        CurrentOrderActivity = new OrderActivity(),
        //        FutureOpenOrdersList = futureOrdersList
        //    };
        //    var gccvalues = orderBal.GetGlobalCodesByCategoriesSp("1024,3102,1011,2305,963");
        //    var jsonData =
        //        new
        //        {
        //            listFrequency = gccvalues.Where(g => g.GlobalCodeCategoryValue.Equals("1024")).OrderBy(d=>d.GlobalCodeName).ToList(),
        //            listOrderStatus = gccvalues.Where(g => g.GlobalCodeCategoryValue.Equals("3102")).OrderBy(d => d.GlobalCodeName).ToList(),
        //            listQuantity =
        //                    gccvalues.Where(g => g.GlobalCodeCategoryValue.Equals("1011"))
        //                        .OrderBy(m => Convert.ToDecimal(m.GlobalCodeValue))
        //                        .ToList(),
        //            listDocumentType = gccvalues.Where(g => g.GlobalCodeCategoryValue.Equals("2305")).ToList(),
        //            listNoteType = gccvalues.Where(g => g.GlobalCodeCategoryValue.Equals("963")).ToList(),
        //            partialView = RenderPartialViewToStringBase(PartialViews.OrdersFullView, ordersFullView)
        //        };
        //    //return PartialView(PartialViews.OrdersFullView, ordersFullView);
        //    return Json(jsonData, JsonRequestBehavior.AllowGet);
        //}



        public ActionResult OrdersViewData(string encounterId)
        {
            //var userId = Helpers.GetLoggedInUserId();
            //var corporateid = Helpers.GetSysAdminCorporateID();
            //var facilityid = Helpers.GetDefaultFacilityId();
            //var orderBal = new OpenOrderBal(
            //    Helpers.DefaultCptTableNumber,
            //    Helpers.DefaultServiceCodeTableNumber,
            //    Helpers.DefaultDrgTableNumber,
            //    Helpers.DefaultDrugTableNumber,
            //    Helpers.DefaultHcPcsTableNumber,
            //    Helpers.DefaultDiagnosisTableNumber);


            ////DB Call to get all records related to Orders tab
            //var ordersViewData = orderBal.OrdersViewData(userId, 100, corporateid, facilityid, Convert.ToInt32(encounterId), "1024,3102,1011,2305,963", 0, "7", string.Empty);

            //// Order Activities Section, starts here
            //var orderActivities = ordersViewData.OrderActivities;
            //var openActStatuses = new[] { 0, 1, 30, 20, 40 };
            //var openOrderActivityList = orderActivities.Where(a => openActStatuses.Contains(Convert.ToInt32(a.OrderActivityStatus))).ToList();
            //var closedOrderActivityList = orderActivities.Where(a => !openActStatuses.Contains(Convert.ToInt32(a.OrderActivityStatus))).ToList();
            //// Order Activities Section, ends here

            ////Orders Section
            //var orders = ordersViewData.OpenOrders.ToList();
            //var closedOrderStatuses = new[] { "2", "3", "4", "9" };
            //var openOrderList = orders.Where(a => a.OrderStatus.Equals("1")).ToList();
            //var closedOrdersList = orders.Where(a => closedOrderStatuses.Contains(a.OrderStatus)).ToList();
            ////Orders Section

            //var newEncOrder = new OpenOrder
            //{
            //    StartDate = Helpers.GetInvariantCultureDateTime(),
            //    EndDate = Helpers.GetInvariantCultureDateTime(),
            //    OrderStatus = Convert.ToString((int)OrderStatus.Open)
            //};

            //View Model, containing data
            var ordersFullView = new OrdersFullView
            {
                AllPhysicianOrders = new List<OpenOrderCustomModel>(),// ordersViewData.PreviousOrders,
                ClosedOrdersList = new List<OpenOrderCustomModel>(),// closedOrdersList,
                EncounterOrder = new OpenOrder(),// newEncOrder,
                FavoriteOrders = new List<OpenOrderCustomModel>(),// ordersViewData.FavoriteOrders,
                MostRecentOrders = new List<OpenOrderCustomModel>(),//   ordersViewData.MostRecentOrders,
                OpenOrdersList = new List<OpenOrderCustomModel>(),// openOrderList,
                SearchedOrders = new List<OpenOrderCustomModel>(),
                ClosedOrderActivityList = new List<OrderActivityCustomModel>(),// closedOrderActivityList,
                OpenOrderActivityList = new List<OrderActivityCustomModel>(),// openOrderActivityList,
                CurrentOrderActivity = new OrderActivity(),
                FutureOpenOrdersList = new List<FutureOpenOrderCustomModel>()// ordersViewData.FutureOpenOrders
            };
            //View Model, containing data

            ////GC Values 
            //var gccvalues = ordersViewData.GlobalCodes;

            //json data to return to view
            var jsonData =
                new
                {
                    //listFrequency = gccvalues.Where(g => g.GlobalCodeCategoryValue.Equals("1024")).OrderBy(d => d.GlobalCodeName).ToList(),
                    //listOrderStatus = gccvalues.Where(g => g.GlobalCodeCategoryValue.Equals("3102")).OrderBy(d => d.GlobalCodeName).ToList(),
                    //listQuantity = gccvalues.Where(g => g.GlobalCodeCategoryValue.Equals("1011")).OrderBy(m => Convert.ToDecimal(m.GlobalCodeValue)).ToList(),
                    //listDocumentType = gccvalues.Where(g => g.GlobalCodeCategoryValue.Equals("2305")).ToList(),
                    //listNoteType = gccvalues.Where(g => g.GlobalCodeCategoryValue.Equals("963")).ToList(),
                    partialView = RenderPartialViewToStringBase(PartialViews.OrdersFullView, ordersFullView)
                };
            //json data to return to view


            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        /// <summary>
        ///     Gets the order detail json by identifier.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns></returns>
        public ActionResult GetOrderDetailJsonById(string orderId)
        {
            var encounterOrderbal = new OpenOrderBal(
                Helpers.DefaultCptTableNumber,
                Helpers.DefaultServiceCodeTableNumber,
                Helpers.DefaultDrgTableNumber,
                Helpers.DefaultDrugTableNumber,
                Helpers.DefaultHcPcsTableNumber,
                Helpers.DefaultDiagnosisTableNumber);
            var id = 0;
            if (!string.IsNullOrEmpty(orderId))
            {
                id = Convert.ToInt32(orderId);
            }

            var orderDetail = encounterOrderbal.GetOpenOrderDetail(Convert.ToInt32(orderId));
            var jsonObject =
                new
                {
                    orderDetail,
                    orderStartDate = orderDetail.StartDate.Value.ToString("MM/dd/yyyy"),
                    orderEndDate = orderDetail.EndDate.Value.ToString("MM/dd/yyyy")
                };
            return Json(jsonObject, JsonRequestBehavior.AllowGet);
            //return PartialView(PartialViews.PhysicianOpenOrderAddEdit, orderDetail);
        }

        #endregion

        #region Future Order

        /// <summary>
        ///     Adds the physician future order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        public ActionResult AddPhysicianFutureOrder(FutureOpenOrder order)
        {
            // var EncounterId = 100220141;
            using (
                var futureOpenOrderBal = new FutureOpenOrderBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var userId = Helpers.GetLoggedInUserId();
                var corporateId = Helpers.GetSysAdminCorporateID();
                var facilityId = Helpers.GetDefaultFacilityId();
                var days = (Convert.ToDateTime(order.EndDate) - Convert.ToDateTime(order.StartDate)).TotalDays;
                var periodDays = days <= 0 ? 1.0 : days + 1;
                order.PeriodDays = Convert.ToString(periodDays);
                order.FacilityID = facilityId;
                order.CorporateID = corporateId;
                if (order.OrderStatus == Convert.ToString((int)OrderStatus.Closed)) order.IsActivitySchecduled = true;

                if (order.FutureOpenOrderID > 0)
                {
                    order.ModifiedBy = userId;
                    order.PhysicianID = userId;
                    order.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                    order.OpenOrderPrescribedDate = Helpers.GetInvariantCultureDateTime();
                }
                else
                {
                    order.CreatedBy = userId;
                    order.CreatedDate = Helpers.GetInvariantCultureDateTime();
                    order.PhysicianID = userId;
                    order.OpenOrderPrescribedDate = Helpers.GetInvariantCultureDateTime();
                    order.IsActivitySchecduled = null;
                    order.ActivitySchecduledOn = null;
                }
                var orderId = futureOpenOrderBal.SaveFutureOpenOrder(order);
                return Json(orderId);
            }
        }

        /// <summary>
        ///     Gets the patient future order.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public ActionResult GetPatientFutureOrder(int patientId)
        {
            using (
                var futureOpenOrderBal = new FutureOpenOrderBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var futureOrderList = futureOpenOrderBal.GetFutureOpenOrderByPatientId(patientId);
                return PartialView(PartialViews.FutureOpenOrders, futureOrderList);
            }
        }

        /// <summary>
        ///     Gets the future open orders.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public ActionResult GetFutureOpenOrders(int patientId)
        {
            using (
                var futureOpenOrderBal = new FutureOpenOrderBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var futureOrderList = futureOpenOrderBal.GetFutureOpenOrderByPatientId(patientId);
                return PartialView(PartialViews.FutureOpenOrders, futureOrderList);
            }
        }

        #endregion

        #region Common Methods to View Order/Activites in EHR

        /// <summary>
        ///     Adds the physician order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        public ActionResult AddPhysicianOrder(OpenOrderCustomModel order)
        {
            var userId = Helpers.GetLoggedInUserId();
            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();

            var orderId = AddOpenOrder(order); // ---- Add orders
            var tabid = Convert.ToInt32(order.TabId);
            var orderBal = new OpenOrderBal(
                Helpers.DefaultCptTableNumber,
                Helpers.DefaultServiceCodeTableNumber,
                Helpers.DefaultDrgTableNumber,
                Helpers.DefaultDrugTableNumber,
                Helpers.DefaultHcPcsTableNumber,
                Helpers.DefaultDiagnosisTableNumber);

            var mostRecentOrders = orderBal.GetMostOrderedList(userId, 100); //---- Complexity has reduced to 1
            var allPhysicianOrders = orderBal.GetOrdersByPhysician(userId, corporateId, facilityId);
            //---- Complexity has reduced to 1

            var favoriteOrders = orderBal.GetPhysicanFavoriteOrderedList(userId, corporateId, facilityId);
            //---- Complexity has reduced to 1

            var allEncounterOrders = orderBal.GetAllOrdersByEncounterId(Convert.ToInt32(order.EncounterID));
            //---- Complexity was 7 now reduced to 1
            var closedOrdersList = new List<OpenOrderCustomModel>();
            var openOrderList = new List<OpenOrderCustomModel>();

            var closedOrderActivityList = new List<OrderActivityCustomModel>();
            var openOrderActivityList = new List<OrderActivityCustomModel>();
            var labWaitingSpecimenList = new List<OrderActivityCustomModel>();
            using (
                var orderActivityBal = new OrderActivityBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var encounterActivitesobj =
                    orderActivityBal.GetOrderActivitiesByEncounterIdSP(Convert.ToInt32(order.EncounterID));
                var encounterActivitesClosedListObj =
                    encounterActivitesobj.Where(
                        x =>
                        x.OrderActivityStatus != 0
                        && x.OrderActivityStatus != Convert.ToInt32(OpenOrderActivityStatus.Open)
                        && x.OrderActivityStatus != Convert.ToInt32(OpenOrderActivityStatus.LabSectionWaitingForResult)
                        && x.OrderActivityStatus
                        != Convert.ToInt32(OpenOrderActivityStatus.LabSectionWaitingForSpecimen)
                        && x.OrderActivityStatus != Convert.ToInt32(OpenOrderActivityStatus.PartiallyExecutedForResult))
                        .OrderBy(x => x.ExecutedDate)
                        .ToList();
                var encounterActivitesOpenListObj =
                    encounterActivitesobj.Where(
                        x =>
                        x.OrderActivityStatus == 0
                        || x.OrderActivityStatus == Convert.ToInt32(OpenOrderActivityStatus.Open)
                        || x.OrderActivityStatus == Convert.ToInt32(OpenOrderActivityStatus.LabSectionWaitingForResult)
                        || x.OrderActivityStatus
                        == Convert.ToInt32(OpenOrderActivityStatus.LabSectionWaitingForSpecimen)
                        || x.OrderActivityStatus == Convert.ToInt32(OpenOrderActivityStatus.PartiallyExecutedForResult))
                        .OrderBy(x => x.OrderScheduleDate)
                        .ToList();

                labWaitingSpecimenList =
                    encounterActivitesOpenListObj.Where(
                        x => x.OrderActivityStatus == 0 || x.OrderActivityStatus == 1 || x.OrderActivityStatus == 20)
                        .ToList();
                closedOrderActivityList = encounterActivitesClosedListObj;
                openOrderActivityList = encounterActivitesOpenListObj;
            }

            if (allEncounterOrders.Count > 0)
            {
                openOrderList = allEncounterOrders.Where(a => a.OrderStatus.Equals("1")).ToList();
                closedOrdersList =
                    allEncounterOrders.Where(
                        a =>
                        a.OrderStatus.Equals("3") || a.OrderStatus.Equals("4") || a.OrderStatus.Equals("2")
                        || a.OrderStatus.Equals("9")).ToList();
            }
            var patientId = orderBal.GetPatientIdByEncounterId(Convert.ToInt32(order.EncounterID));
            var futureOrdersList =
                new FutureOpenOrderBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber).GetFutureOpenOrderByPatientId(Convert.ToInt32(patientId));

            closedOrdersList = tabid == 0
                                   ? closedOrdersList
                                   : closedOrdersList.Where(x => x.CategoryId == tabid).ToList();
            openOrderList = tabid == 0 ? openOrderList : openOrderList.Where(x => x.CategoryId == tabid).ToList();
            closedOrderActivityList = tabid == 0
                                          ? closedOrderActivityList
                                          : closedOrderActivityList.Where(x => x.OrderCategoryID == tabid).ToList();
            openOrderActivityList = tabid == 0
                                        ? openOrderActivityList
                                        : openOrderActivityList.Where(x => x.OrderCategoryID == tabid).ToList();
            labWaitingSpecimenList = tabid == 0
                                         ? labWaitingSpecimenList
                                         : labWaitingSpecimenList.Where(x => x.OrderCategoryID == tabid).ToList();
            var ordersFullView =
                new
                {
                    AllPhysicianOrders =
                            RenderPartialViewToStringBase(
                                PartialViews.MostRecentOrders,
                                allPhysicianOrders.DistinctBy(x => x.OrderCode).ToList()),
                    ClosedOrdersList =
                            RenderPartialViewToStringBase(
                                tabid == 11080 ? PartialViews.LabClosedOrderList : PartialViews.ClosedOrdersList,
                                closedOrdersList),
                    FavoriteOrders = favoriteOrders,
                    MostRecentOrders =
                            RenderPartialViewToStringBase(PartialViews.MostRecentOrders, mostRecentOrders),
                    OpenOrdersList =
                            RenderPartialViewToStringBase(
                                tabid == 11080 ? PartialViews.LabOpenOrderList : PartialViews.PhysicianOpenOrderList,
                                openOrderList),
                    ClosedOrderActivityList =
                            RenderPartialViewToStringBase(
                                tabid == 11080
                                    ? PartialViews.LabClosedActivtiesList
                                    : PartialViews.OrderClosedActivityScheduleList,
                                closedOrderActivityList),
                    OpenOrderActivityList =
                            RenderPartialViewToStringBase(
                                tabid == 11080
                                    ? PartialViews.LabOpenActivtiesList
                                    : PartialViews.OrderActivityScheduleList,
                                openOrderActivityList),
                    orderid = orderId,
                    futureOrdersList =
                            RenderPartialViewToStringBase(PartialViews.FutureOpenOrders, futureOrdersList),
                    labWaitingSpecimenList =
                            RenderPartialViewToStringBase(PartialViews.LabSpecimanListing, labWaitingSpecimenList)
                };
            return Json(ordersFullView);
        }

        /// <summary>
        ///     Binds the orders grid.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public JsonResult BindOrdersGrid(int encounterId, int type)
        {
            using (var openOrdersbal = new OpenOrderBal(
                    Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber))
            {
                #region Commented Code
                //using (
                //    var orderActivitybal = new OrderActivityBal(
                //        Helpers.DefaultCptTableNumber,
                //        Helpers.DefaultServiceCodeTableNumber,
                //        Helpers.DefaultDrgTableNumber,
                //        Helpers.DefaultDrugTableNumber,
                //        Helpers.DefaultHcPcsTableNumber,
                //        Helpers.DefaultDiagnosisTableNumber))
                //{
                //    var listopenOrders =
                //        openOrdersbal.GetPhysicianOrders(encounterId, OrderStatus.Open.ToString())
                //            .OrderBy(x => x.OpenOrderID)
                //            .ToList();
                //    listopenOrders = type == 0
                //                         ? listopenOrders
                //                         : listopenOrders.Where(x => x.CategoryId == type).ToList();

                //    var closedorderlist =
                //        openOrdersbal.GetPhysicianOrders(encounterId, OrderStatus.Closed.ToString())
                //            .OrderBy(x => x.OpenOrderID)
                //            .ToList();
                //    closedorderlist = type == 0
                //                          ? closedorderlist
                //                          : closedorderlist.Where(x => x.CategoryId == type).ToList();

                //    var openorderActivitylist = orderActivitybal.GetOrderActivitiesByEncounterId(encounterId)
                //            .Where(
                //                x =>
                //                x.OrderActivityStatus == 0
                //                || x.OrderActivityStatus == Convert.ToInt32(OpenOrderActivityStatus.Open)
                //                || x.OrderActivityStatus
                //                == Convert.ToInt32(OpenOrderActivityStatus.LabSectionWaitingForResult)
                //                || x.OrderActivityStatus
                //                == Convert.ToInt32(OpenOrderActivityStatus.LabSectionWaitingForSpecimen)
                //                || x.OrderActivityStatus
                //                == Convert.ToInt32(OpenOrderActivityStatus.PartiallyExecutedForResult))
                //            .OrderBy(x => x.OrderScheduleDate)
                //            .ToList();
                //    openorderActivitylist = type == 0
                //                                ? openorderActivitylist
                //                                : openorderActivitylist.Where(x => x.OrderCategoryID == type).ToList();

                //    var closedorderActivitylist =
                //        orderActivitybal.GetOrderActivitiesByEncounterId(encounterId)
                //            .Where(
                //                x =>
                //                x.OrderActivityStatus != 0
                //                && x.OrderActivityStatus != Convert.ToInt32(OpenOrderActivityStatus.Open)
                //                && x.OrderActivityStatus
                //                != Convert.ToInt32(OpenOrderActivityStatus.LabSectionWaitingForResult)
                //                && x.OrderActivityStatus
                //                != Convert.ToInt32(OpenOrderActivityStatus.LabSectionWaitingForSpecimen)
                //                && x.OrderActivityStatus
                //                != Convert.ToInt32(OpenOrderActivityStatus.PartiallyExecutedForResult))
                //            .OrderBy(x => x.ExecutedDate)
                //            .ToList();
                //    closedorderActivitylist = type == 0
                //                                  ? closedorderActivitylist
                //                                  : closedorderActivitylist.Where(x => x.OrderCategoryID == type)
                //                                        .ToList();
                //    var labWaitingSpecimenList =
                //        openorderActivitylist.Where(
                //            x => x.OrderActivityStatus == 0 || x.OrderActivityStatus == 1 || x.OrderActivityStatus == 20)
                //            .ToList();
                //    labWaitingSpecimenList = type == 0
                //                                 ? labWaitingSpecimenList
                //                                 : labWaitingSpecimenList.Where(x => x.OrderCategoryID == type).ToList();
                //} 
                #endregion


                var listopenOrders = new List<OpenOrderCustomModel>();
                var closedorderlist = new List<OpenOrderCustomModel>();
                var oActivities = new List<OrderActivityCustomModel>();
                var cActivities = new List<OrderActivityCustomModel>();
                var labWActivities = new List<OrderActivityCustomModel>();

                var orderActivityStatuses = new[] { 0, 1, 30, 20, 40 };
                var labOrderActStatuses = new[] { 0, 1, 20 };

                //Get all orders and Activities by Encounter ID, from the database
                var vm = openOrdersbal.GetPhysicianOrderPlusActivityList(encounterId, withActivities: true, categoryId: type);

                if (vm != null)
                {
                    //Bind Orders Data
                    if (vm.OpenOrders.Any())
                    {
                        listopenOrders = vm.OpenOrders.Where(a => a.OrderStatus.Equals(Convert.ToString((int)OrderStatus.Open))).OrderBy(o => o.OpenOrderID).ToList();
                        closedorderlist = vm.OpenOrders.Where(a => !a.OrderStatus.Equals(Convert.ToString((int)OrderStatus.Open))).OrderBy(o => o.OpenOrderID).ToList();
                    }


                    //Bind Orders Activities Data
                    if (vm.OrderActivities.Any())
                    {
                        var allActivities = vm.OrderActivities;
                        oActivities = allActivities.Where(a => orderActivityStatuses.Contains(a.OrderActivityStatus.Value) && (type == 0 || a.OrderCategoryID == type)).OrderBy(x => x.OrderScheduleDate).ToList();
                        cActivities = allActivities.Where(a => !orderActivityStatuses.Contains(a.OrderActivityStatus.Value) && (type == 0 || a.OrderCategoryID == type)).OrderBy(x => x.ExecutedDate).ToList();
                        labWActivities = allActivities.Where(a => labOrderActStatuses.Contains(a.OrderActivityStatus.Value) && (type == 0 || a.OrderCategoryID == type)).OrderBy(x => x.OrderScheduleDate).ToList();
                    }
                }

                //Bind JSON Data
                var jsonData = new
                {
                    closedorderslist = RenderPartialViewToStringBase(type == 11080 ? PartialViews.LabClosedOrderList
                                        : PartialViews.ClosedOrdersList, closedorderlist),
                    closedorderActivityslist = RenderPartialViewToStringBase(type == 11080 ? PartialViews.LabClosedActivtiesList
                                        : PartialViews.OrderClosedActivityScheduleList, cActivities),
                    openorderActivityslist = RenderPartialViewToStringBase(
                                    type == 11080 ? PartialViews.LabOpenActivtiesList
                                        : PartialViews.OrderActivityScheduleList, oActivities),
                    openOrderslist = RenderPartialViewToStringBase(
                                    type == 11080 ? PartialViews.LabOpenOrderList
                                        : PartialViews.PhysicianOpenOrderList, listopenOrders),
                    labWaitingSpecimenList = RenderPartialViewToStringBase(
                                    PartialViews.LabSpecimanListing, labWActivities)
                };

                //Bind JSON Result to view
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
        }


        ///// <summary>
        /////     Binds the orders grid.
        ///// </summary>
        ///// <param name="encounterId">The encounter identifier.</param>
        ///// <param name="type">The type.</param>
        ///// <returns></returns>
        //public ActionResult BindOrdersGrid(int encounterId, int type)
        //{
        //    using (
        //        var openOrdersbal = new OpenOrderBal(
        //            Helpers.DefaultCptTableNumber,
        //            Helpers.DefaultServiceCodeTableNumber,
        //            Helpers.DefaultDrgTableNumber,
        //            Helpers.DefaultDrugTableNumber,
        //            Helpers.DefaultHcPcsTableNumber,
        //            Helpers.DefaultDiagnosisTableNumber))
        //    {
        //        using (
        //            var orderActivitybal = new OrderActivityBal(
        //                Helpers.DefaultCptTableNumber,
        //                Helpers.DefaultServiceCodeTableNumber,
        //                Helpers.DefaultDrgTableNumber,
        //                Helpers.DefaultDrugTableNumber,
        //                Helpers.DefaultHcPcsTableNumber,
        //                Helpers.DefaultDiagnosisTableNumber))
        //        {
        //            var listopenOrders =
        //                openOrdersbal.GetPhysicianOrders(encounterId, OrderStatus.Open.ToString())
        //                    .OrderBy(x => x.OpenOrderID)
        //                    .ToList();
        //            listopenOrders = type == 0
        //                                 ? listopenOrders
        //                                 : listopenOrders.Where(x => x.CategoryId == type).ToList();
        //            var openorderActivitylist =
        //                orderActivitybal.GetOrderActivitiesByEncounterId(encounterId)
        //                    .Where(
        //                        x =>
        //                        x.OrderActivityStatus == 0
        //                        || x.OrderActivityStatus == Convert.ToInt32(OpenOrderActivityStatus.Open)
        //                        || x.OrderActivityStatus
        //                        == Convert.ToInt32(OpenOrderActivityStatus.LabSectionWaitingForResult)
        //                        || x.OrderActivityStatus
        //                        == Convert.ToInt32(OpenOrderActivityStatus.LabSectionWaitingForSpecimen)
        //                        || x.OrderActivityStatus
        //                        == Convert.ToInt32(OpenOrderActivityStatus.PartiallyExecutedForResult))
        //                    .OrderBy(x => x.OrderScheduleDate)
        //                    .ToList();
        //            openorderActivitylist = type == 0
        //                                        ? openorderActivitylist
        //                                        : openorderActivitylist.Where(x => x.OrderCategoryID == type).ToList();

        //            var closedorderActivitylist =
        //                orderActivitybal.GetOrderActivitiesByEncounterId(encounterId)
        //                    .Where(
        //                        x =>
        //                        x.OrderActivityStatus != 0
        //                        && x.OrderActivityStatus != Convert.ToInt32(OpenOrderActivityStatus.Open)
        //                        && x.OrderActivityStatus
        //                        != Convert.ToInt32(OpenOrderActivityStatus.LabSectionWaitingForResult)
        //                        && x.OrderActivityStatus
        //                        != Convert.ToInt32(OpenOrderActivityStatus.LabSectionWaitingForSpecimen)
        //                        && x.OrderActivityStatus
        //                        != Convert.ToInt32(OpenOrderActivityStatus.PartiallyExecutedForResult))
        //                    .OrderBy(x => x.ExecutedDate)
        //                    .ToList();
        //            closedorderActivitylist = type == 0
        //                                          ? closedorderActivitylist
        //                                          : closedorderActivitylist.Where(x => x.OrderCategoryID == type)
        //                                                .ToList();
        //            var labWaitingSpecimenList =
        //                openorderActivitylist.Where(
        //                    x => x.OrderActivityStatus == 0 || x.OrderActivityStatus == 1 || x.OrderActivityStatus == 20)
        //                    .ToList();
        //            labWaitingSpecimenList = type == 0
        //                                         ? labWaitingSpecimenList
        //                                         : labWaitingSpecimenList.Where(x => x.OrderCategoryID == type).ToList();
        //            var closedorderlist =
        //                openOrdersbal.GetPhysicianOrders(encounterId, OrderStatus.Closed.ToString())
        //                    .OrderBy(x => x.OpenOrderID)
        //                    .ToList();
        //            closedorderlist = type == 0
        //                                  ? closedorderlist
        //                                  : closedorderlist.Where(x => x.CategoryId == type).ToList();
        //            var objToRerturn =
        //                new
        //                {
        //                    closedorderslist =
        //                            RenderPartialViewToStringBase(
        //                                type == 11080 ? PartialViews.LabClosedOrderList : PartialViews.ClosedOrdersList,
        //                                closedorderlist),
        //                    closedorderActivityslist =
        //                            RenderPartialViewToStringBase(
        //                                type == 11080
        //                                    ? PartialViews.LabClosedActivtiesList
        //                                    : PartialViews.OrderClosedActivityScheduleList,
        //                                closedorderActivitylist),
        //                    openorderActivityslist =
        //                            RenderPartialViewToStringBase(
        //                                type == 11080
        //                                    ? PartialViews.LabOpenActivtiesList
        //                                    : PartialViews.OrderActivityScheduleList,
        //                                openorderActivitylist),
        //                    openOrderslist =
        //                            RenderPartialViewToStringBase(
        //                                type == 11080
        //                                    ? PartialViews.LabOpenOrderList
        //                                    : PartialViews.PhysicianOpenOrderList,
        //                                listopenOrders),
        //                    labWaitingSpecimenList =
        //                            RenderPartialViewToStringBase(
        //                                PartialViews.LabSpecimanListing,
        //                                labWaitingSpecimenList)
        //                };
        //            return Json(objToRerturn, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //}

        /// <summary>
        ///     Gets the data by categories.
        /// </summary>
        /// <returns></returns>
        public JsonResult GetDataByCategories()
        {
            using (var gbal = new GlobalCodeBal())
            {
                var categories = new[] { "1024", "3102", "1011", "2305", "963" };
                var list = gbal.GetListByCategoriesRange(categories);
                var jsonData =
                    new
                    {
                        listFrequency = list.Where(g => g.ExternalValue1.Equals("1024")).ToList(),
                        listOrderStatus = list.Where(g => g.ExternalValue1.Equals("3102")).ToList(),
                        listQuantity =
                                list.Where(g => g.ExternalValue1.Equals("1011"))
                                    .OrderBy(m => Convert.ToDecimal(m.Value))
                                    .ToList(),
                        listDocumentType = list.Where(g => g.ExternalValue1.Equals("2305")).ToList(),
                        listNoteType = list.Where(g => g.ExternalValue1.Equals("963")).ToList()
                    };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        ///     Cancels the open order.
        /// </summary>
        /// <param name="cancelOrderId">The cancel order identifier.</param>
        /// <returns></returns>
        public JsonResult CancelOpenOrder(int cancelOrderId)
        {
            using (
                var openorderBal = new OpenOrderBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                openorderBal.CancelOpenOrder(cancelOrderId, Helpers.GetLoggedInUserId());
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     Adds to favorites.
        /// </summary>
        /// <param name="codeId">
        ///     The code identifier.
        /// </param>
        /// <param name="categoryId">
        ///     The category identifier.
        /// </param>
        /// <param name="id">
        ///     The identifier.
        /// </param>
        /// <param name="isFavorite">
        ///     if set to <c>true</c> [is favorite].
        /// </param>
        /// <param name="favoriteDesc">
        ///     The favorite desc.
        /// </param>
        /// <param name="Dtype">
        ///     The dtype.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult AddToFavorites(
            string codeId,
            string categoryId,
            int id,
            bool isFavorite,
            string favoriteDesc,
            string Dtype)
        {
            UserDefinedDescriptions model = null;
            using (var bal = new FavoritesBal(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber))
            {
                if (id > 0) model = bal.GetFavoriteById(id);
                else
                {
                    if (IsAlreadyFav(Helpers.GetLoggedInUserId(), codeId, categoryId)) return Json(1);
                    model = new UserDefinedDescriptions { CategoryId = categoryId, CodeId = codeId };
                }

                model.UserDefineDescription = favoriteDesc;
                model.UserID = Helpers.GetLoggedInUserId();
                model.RoleID = Helpers.GetDefaultRoleId();
                if (model.UserDefinedDescriptionID > 0)
                {
                    model.ModifiedBy = model.UserID;
                    model.Modifieddate = Helpers.GetInvariantCultureDateTime();
                }
                else
                {
                    model.CreatedBy = model.UserID;
                    model.CreatedDate = Helpers.GetInvariantCultureDateTime();
                }

                var result = bal.AddToFavorites(model, isFavorite);
                if (Dtype != "true")
                {
                    var ordersBal = new OpenOrderBal(
                        Helpers.DefaultCptTableNumber,
                        Helpers.DefaultServiceCodeTableNumber,
                        Helpers.DefaultDrgTableNumber,
                        Helpers.DefaultDrugTableNumber,
                        Helpers.DefaultHcPcsTableNumber,
                        Helpers.DefaultDiagnosisTableNumber);
                    var list = ordersBal.GetFavoriteOrders(Helpers.GetLoggedInUserId());
                    list = list.Where(_ => _.CategoryId == Convert.ToInt32(OrderType.Diagnosis)).ToList();
                    return PartialView(PartialViews.FavoriteOrders, list);
                }

                //var favDiagnosisBal = new FavoritesBal(
                //    Helpers.DefaultCptTableNumber,
                //    Helpers.DefaultServiceCodeTableNumber,
                //    Helpers.DefaultDrgTableNumber,
                //    Helpers.DefaultDrugTableNumber,
                //    Helpers.DefaultHcPcsTableNumber,
                //    Helpers.DefaultDiagnosisTableNumber);
                //var favDiagnosisList = favDiagnosisBal.GetFavoriteOrders(Helpers.GetLoggedInUserId());
                //favDiagnosisList =
                //    favDiagnosisList.Where(
                //        _ =>
                //        _.CategoryId == Convert.ToInt32(OrderType.Diagnosis).ToString()
                //        || _.CategoryId == Convert.ToInt32(OrderType.DRG).ToString())

                //        // || _.CategoryId == Convert.ToInt32(OrderType.CPT).ToString())
                //        .ToList();
                //return PartialView("../Diagnosis/" + PartialViews.PhyFavDiagnosisList, favDiagnosisList);

                var dlist = bal.GetFavoriteDiagnosisData(Helpers.GetLoggedInUserId());
                var jsonData = dlist.Select(f => new[] { Convert.ToString(f.UserDefinedDescriptionID), f.CategoryName, f.CodeId, f.CodeDesc, f.UserDefineDescription, f.CodeId });
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Diagnosis Tab Data

        /// <summary>
        ///     Gets the diagnosis codes.
        /// </summary>
        /// <param name="patientId">
        ///     The patient identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetDiagnosisCodes(int patientId)
        {
            var facilityId = Helpers.GetDefaultFacilityId();
            var bal = new DiagnosisBal(Helpers.DefaultDiagnosisTableNumber, Helpers.DefaultDrgTableNumber);
            var list = bal.GetAllDiagnosisCodes(facilityId);
            var diagnosisBal = new DiagnosisBal(Helpers.DefaultDiagnosisTableNumber, Helpers.DefaultDrgTableNumber);
            var ClientDiagnosislst = diagnosisBal.GetDiagnosisList(patientId).ToList();
            if (ClientDiagnosislst.Count > 0)
            {
                list =
                    (from y in list
                     join q in ClientDiagnosislst on y.DiagnosisTableNumberId equals q.DiagnosisCodeId
                     select y).ToList();
            }
            else
            {
                list = new List<DiagnosisCode>();
            }

            return Json(list);
        }

        ///// <summary>
        /////     Gets the diagnosis tab data.
        ///// </summary>
        ///// <param name="patientId">
        /////     The patient identifier.
        ///// </param>
        ///// <param name="encounterId">
        /////     The encounter identifier.
        ///// </param>
        ///// <returns>
        /////     The <see cref="ActionResult" />.
        ///// </returns>
        //public ActionResult GetDiagnosisTabData(int patientId, int encounterId)
        //{
        //    DiagnosisCustomModel diagnosisModel;
        //    PatientInfoCustomModel patientInfo;
        //    List<DiagnosisCustomModel> list;
        //    List<DiagnosisCustomModel> previouslist;

        //    // string patientId, string encounterId
        //    var pid = Convert.ToInt32(patientId);
        //    var eid = Convert.ToInt32(encounterId);
        //    bool isPrimary;
        //    var userid = Helpers.GetLoggedInUserId();
        //    using (
        //        var bal = new DiagnosisBal(
        //            Helpers.DefaultCptTableNumber,
        //            Helpers.DefaultServiceCodeTableNumber,
        //            Helpers.DefaultDrgTableNumber,
        //            Helpers.DefaultDrugTableNumber,
        //            Helpers.DefaultHcPcsTableNumber,
        //            Helpers.DefaultDiagnosisTableNumber))
        //    {
        //        var dModel = bal.GetNewDiagnosisByEncounterId(eid, pid);
        //        var dList = bal.GetDiagnosisList(pid, eid);
        //        var previousDlist = bal.GetPreviousDiagnosisList(pid, eid);
        //        var isMajorCpt = dList.All(x => x.DiagnosisType != 4);
        //        dModel.IsMajorCPT = isMajorCpt;
        //        dModel.IsMajorDRG = dList.All(x => x.DiagnosisType != 3);
        //        list = dList != null && dList.Count > 0 ? dList : new List<DiagnosisCustomModel>();
        //        previouslist = previousDlist.Any() ? previousDlist : new List<DiagnosisCustomModel>();
        //        diagnosisModel = dModel;
        //        patientInfo = bal.GetPatientDetailsByPatientId(pid);
        //        isPrimary = list.Count(x => x.DiagnosisType == Convert.ToInt32(DiagnosisType.Primary)) == 0;
        //    }

        //    if (patientInfo != null)
        //    {
        //        diagnosisModel.PatientID = patientInfo.PatientInfo.PatientID;
        //        diagnosisModel.EncounterID = eid > 0
        //                                         ? eid
        //                                         : (patientInfo.CurrentEncounter != null
        //                                                ? patientInfo.CurrentEncounter.EncounterID
        //                                                : 0);
        //        diagnosisModel.CorporateID = patientInfo.CorporateId;
        //        diagnosisModel.FacilityID = patientInfo.PatientInfo.FacilityId;
        //    }

        //    var primarydiagnosisId = 0;
        //    if (list.Any())
        //    {
        //        var diagnosisCustomModel =
        //            list.SingleOrDefault(x => x.DiagnosisType == Convert.ToInt32(DiagnosisType.Primary));
        //        if (diagnosisCustomModel != null)
        //        {
        //            primarydiagnosisId = diagnosisCustomModel.DiagnosisID;
        //        }
        //    }

        //    diagnosisModel.IsPrimary = isPrimary;
        //    diagnosisModel.PrimaryDiagnosisId = primarydiagnosisId;
        //    var favDiagnosisBal = new FavoritesBal(
        //        Helpers.DefaultCptTableNumber,
        //        Helpers.DefaultServiceCodeTableNumber,
        //        Helpers.DefaultDrgTableNumber,
        //        Helpers.DefaultDrugTableNumber,
        //        Helpers.DefaultHcPcsTableNumber,
        //        Helpers.DefaultDiagnosisTableNumber);
        //    var favDiagnosisList = favDiagnosisBal.GetFavoriteOrders(userid);
        //    favDiagnosisList =
        //        favDiagnosisList.Where(
        //            _ =>
        //            _.CategoryId == Convert.ToInt32(OrderType.Diagnosis).ToString()
        //            || _.CategoryId == Convert.ToInt32(OrderType.DRG).ToString())

        //            // || _.CategoryId == Convert.ToInt32(OrderType.CPT).ToString())
        //            .ToList();
        //    var diagnosisView = new DiagnosisView
        //    {
        //        CurrentDiagnosis = diagnosisModel,
        //        PatientInfo = patientInfo,
        //        DiagnosisList = list,
        //        previousDiagnosisList = previouslist,
        //        FavoriteDiagnosisList = favDiagnosisList
        //    };

        //    return PartialView(PartialViews.DiagnosisViewUC, diagnosisView);
        //}


        ///// <summary>
        /////     Gets the diagnosis tab data.
        ///// </summary>
        ///// <param name="patientId">
        /////     The patient identifier.
        ///// </param>
        ///// <param name="encounterId">
        /////     The encounter identifier.
        ///// </param>
        ///// <returns>
        /////     The <see cref="ActionResult" />.
        ///// </returns>
        //public ActionResult GetDiagnosisTabData(int patientId, int encounterId)
        //{
        //    DiagnosisCustomModel diagnosisModel;
        //    PatientInfoCustomModel patientInfo;
        //    List<DiagnosisCustomModel> list;
        //    List<DiagnosisCustomModel> previouslist;

        //    // string patientId, string encounterId
        //    var pid = Convert.ToInt32(patientId);
        //    var eid = Convert.ToInt32(encounterId);
        //    bool isPrimary;
        //    var userid = Helpers.GetLoggedInUserId();
        //    using (
        //        var bal = new DiagnosisBal(
        //            Helpers.DefaultCptTableNumber,
        //            Helpers.DefaultServiceCodeTableNumber,
        //            Helpers.DefaultDrgTableNumber,
        //            Helpers.DefaultDrugTableNumber,
        //            Helpers.DefaultHcPcsTableNumber,
        //            Helpers.DefaultDiagnosisTableNumber))
        //    {
        //        var dModel = bal.GetNewDiagnosisByEncounterId(eid, pid);
        //        var dList = bal.GetDiagnosisList(pid, eid);
        //        var previousDlist = bal.GetPreviousDiagnosisList(pid, eid);
        //        var isMajorCpt = dList.All(x => x.DiagnosisType != 4);
        //        dModel.IsMajorCPT = isMajorCpt;
        //        dModel.IsMajorDRG = dList.All(x => x.DiagnosisType != 3);
        //        list = dList != null && dList.Count > 0 ? dList : new List<DiagnosisCustomModel>();
        //        previouslist = previousDlist.Any() ? previousDlist : new List<DiagnosisCustomModel>();
        //        diagnosisModel = dModel;
        //        //patientInfo = bal.GetPatientDetailsByPatientId(pid);
        //        isPrimary = list.Count(x => x.DiagnosisType == Convert.ToInt32(DiagnosisType.Primary)) == 0;
        //    }

        //    diagnosisModel.PatientID = patientId;
        //    diagnosisModel.EncounterID = eid;
        //    diagnosisModel.CorporateID = Helpers.GetSysAdminCorporateID();
        //    diagnosisModel.FacilityID = Helpers.GetDefaultFacilityId();
        //    //if (patientInfo != null)
        //    //{
        //    //    diagnosisModel.PatientID = patientInfo.PatientInfo.PatientID;
        //    //    diagnosisModel.EncounterID = eid > 0
        //    //                                     ? eid
        //    //                                     : (patientInfo.CurrentEncounter != null
        //    //                                            ? patientInfo.CurrentEncounter.EncounterID
        //    //                                            : 0);
        //    //    diagnosisModel.CorporateID = patientInfo.CorporateId;
        //    //    diagnosisModel.FacilityID = patientInfo.PatientInfo.FacilityId;
        //    //}

        //    var primarydiagnosisId = 0;
        //    if (list.Any())
        //    {
        //        var diagnosisCustomModel =
        //            list.SingleOrDefault(x => x.DiagnosisType == Convert.ToInt32(DiagnosisType.Primary));
        //        if (diagnosisCustomModel != null)
        //        {
        //            primarydiagnosisId = diagnosisCustomModel.DiagnosisID;
        //        }
        //    }

        //    diagnosisModel.IsPrimary = isPrimary;
        //    diagnosisModel.PrimaryDiagnosisId = primarydiagnosisId;
        //    var favDiagnosisBal = new FavoritesBal(
        //        Helpers.DefaultCptTableNumber,
        //        Helpers.DefaultServiceCodeTableNumber,
        //        Helpers.DefaultDrgTableNumber,
        //        Helpers.DefaultDrugTableNumber,
        //        Helpers.DefaultHcPcsTableNumber,
        //        Helpers.DefaultDiagnosisTableNumber);
        //    var favDiagnosisList = favDiagnosisBal.GetFavoriteOrders(userid);
        //    favDiagnosisList =
        //        favDiagnosisList.Where(
        //            _ =>
        //            _.CategoryId == Convert.ToInt32(OrderType.Diagnosis).ToString()
        //            || _.CategoryId == Convert.ToInt32(OrderType.DRG).ToString())

        //            // || _.CategoryId == Convert.ToInt32(OrderType.CPT).ToString())
        //            .ToList();
        //    var diagnosisView = new DiagnosisView
        //    {
        //        CurrentDiagnosis = diagnosisModel,
        //        //PatientInfo = patientInfo,
        //        DiagnosisList = list,
        //        previousDiagnosisList = previouslist,
        //        FavoriteDiagnosisList = favDiagnosisList
        //    };

        //    return PartialView(PartialViews.DiagnosisViewUC, diagnosisView);
        //}



        /// <summary>
        ///     Gets the diagnosis tab data.
        /// </summary>
        /// <param name="patientId">
        ///     The patient identifier.
        /// </param>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetDiagnosisTabData(int patientId, int encounterId)
        {
            var vm = new DiagnosisCustomModel();
            //List<DiagnosisCustomModel> list;
            //List<DiagnosisCustomModel> previouslist;
            //DiagnosisTabData viewData = null;
            //var favOrders = new List<FavoritesCustomModel>();

            // string patientId, string encounterId
            var pid = Convert.ToInt64(patientId);
            var eid = Convert.ToInt32(encounterId);
            var userid = Helpers.GetLoggedInUserId();

            vm.PatientID = patientId;
            vm.EncounterID = eid;
            vm.CorporateID = Helpers.GetSysAdminCorporateID();
            vm.FacilityID = Helpers.GetDefaultFacilityId();

            return PartialView(PartialViews.DiagnosisViewUC, vm);


            //using (var bal = new DiagnosisBal(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber,
            //        Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber))
            //{
            //    viewData = bal.GetDiagnosisTabData(pid, eid, userid, Helpers.DefaultDiagnosisTableNumber, Helpers.DefaultDrgTableNumber);
            //    list = viewData.CurrentDiagnosisList;
            //    previouslist = viewData.PreviousDiagnosisList;

            //    var isMajorCpt = list.All(x => x.DiagnosisType != 4);
            //    vm.IsMajorCPT = isMajorCpt;
            //    vm.IsMajorDRG = list.All(x => x.DiagnosisType != 3);
            //}


            //vm.IsPrimary = !list.Any(x => x.DiagnosisType == (int)DiagnosisType.Primary);

            //if (list.Any(x => x.DiagnosisType == Convert.ToInt32(DiagnosisType.Primary)))
            //    vm.PrimaryDiagnosisId = list.Where(a => a.DiagnosisType == (int)DiagnosisType.Primary).Select(d => d.DiagnosisID).FirstOrDefault();

            //var diagnosisView = new DiagnosisView
            //{
            //    CurrentDiagnosis = vm,
            //    DiagnosisList = list,
            //    previousDiagnosisList = previouslist,
            //    FavoriteDiagnosisList = favOrders
            //};

            //return PartialView(PartialViews.DiagnosisViewUC, diagnosisView);
        }
        #endregion

        #region Lab Test Tab

        /// <summary>
        ///     Binds the pharmacy closed activity list.
        /// </summary>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindLabClosedActivityList(int encounterId)
        {
            using (
                var bal = new OrderActivityBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var list =
                    bal.GetOrderActivitiesByEncounterId(encounterId)
                        .Where(
                            x =>
                            x.OrderCategoryID == Convert.ToInt32(OrderTypeCategory.PathologyandLaboratory)
                            && x.OrderActivityStatus != 0
                            && x.OrderActivityStatus != Convert.ToInt32(OpenOrderActivityStatus.Open))
                        .OrderBy(x => x.ExecutedDate)
                        .ToList();
                return PartialView(PartialViews.LabClosedActivtiesList, list);
            }
        }

        /// <summary>
        ///     Binds the pharmacy closed orders.
        /// </summary>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindLabClosedOrders(int encounterId)
        {
            using (
                var bal = new OpenOrderBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var list =
                    bal.GetPhysicianOrders(encounterId, OrderStatus.Closed.ToString())
                        .Where(x => x.CategoryId == Convert.ToInt32(OrderTypeCategory.PathologyandLaboratory))
                        .OrderBy(x => x.OpenOrderID)
                        .ToList();
                return PartialView(PartialViews.LabClosedOrderList, list);
            }
        }

        /// <summary>
        ///     Binds the pharmacy activity list.
        /// </summary>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindLabOpenActivityList(int encounterId)
        {
            using (
                var bal = new OrderActivityBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var list =
                    bal.GetOrderActivitiesByEncounterId(encounterId)
                        .Where(
                            x =>
                            x.OrderCategoryID == Convert.ToInt32(OrderTypeCategory.PathologyandLaboratory)
                            && (x.OrderActivityStatus == Convert.ToInt32(OpenOrderActivityStatus.Open)
                                || x.OrderActivityStatus == 0))
                        .OrderBy(x => x.OrderScheduleDate)
                        .ToList();
                return PartialView(PartialViews.LabOpenActivtiesList, list);
            }
        }

        /// <summary>
        ///     Binds the pharmacy open order list.
        /// </summary>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindLabOpenOrderList(string encounterId)
        {
            var encounterIdint = Convert.ToInt32(encounterId);
            var encounterOrderbal = new OpenOrderBal(
                Helpers.DefaultCptTableNumber,
                Helpers.DefaultServiceCodeTableNumber,
                Helpers.DefaultDrgTableNumber,
                Helpers.DefaultDrugTableNumber,
                Helpers.DefaultHcPcsTableNumber,
                Helpers.DefaultDiagnosisTableNumber);
            var status = OrderStatus.Open.ToString();
            var listOfOrders =
                encounterOrderbal.GetPhysicianOrders(encounterIdint, status).OrderBy(x => x.OpenOrderID).ToList();
            return PartialView(PartialViews.LabOpenOrderList, listOfOrders);
        }

        /// <summary>
        ///     Binds the lab test data.
        /// </summary>
        /// <param name="patientId">
        ///     The patient identifier.
        /// </param>
        /// <param name="encounterid">
        ///     The encounterid.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindLabTestData(int patientId, int encounterid)
        {
            var openStatuses = new[] {  0,
                                        (int)OpenOrderActivityStatus.Open,
                                        (int)OpenOrderActivityStatus.LabSectionWaitingForResult,
                                        (int)OpenOrderActivityStatus.LabSectionWaitingForSpecimen,
                                        (int)OpenOrderActivityStatus.PartiallyExecutedForResult
                                      };

            var vitals = new List<MedicalVitalCustomModel>();
            var openOrders = new List<OpenOrderCustomModel>();
            var closedOrders = new List<OpenOrderCustomModel>();
            var openActivities = new List<OrderActivityCustomModel>();
            var closedActivities = new List<OrderActivityCustomModel>();

            using (var medicalVitals = new MedicalVitalBal())
                vitals = medicalVitals.GetCustomMedicalVitals(patientId, Convert.ToInt32(MedicalRecordType.LabTest), encounterid);


            using (var orderbal = new OpenOrderBal(Helpers.DefaultCptTableNumber,
                        Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber,
                        Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber))
            {
                //var allPhysicianOrders = orderbal.GetAllOrdersByEncounterId(encounterid);
                //var labordersList = allPhysicianOrders.Where(x => x.EncounterID == encounterid
                //                    && x.CategoryId == (int)GlobalCodeCategoryValue.LabTest);
                //var labopenordersList = labordersList.Where(
                //        x => x.OrderStatus == "0" || x.OrderStatus == Convert.ToInt32(OrderStatus.Open).ToString())
                //        .ToList();

                //var labclosedordersList =
                //    labordersList.Where(
                //        x => x.OrderStatus != "0" && x.OrderStatus != Convert.ToInt32(OrderStatus.Open).ToString())
                //        .ToList();


                //using (var aBal = new OrderActivityBal(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber,
                //        Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber
                //        , Helpers.DefaultDiagnosisTableNumber))
                //{
                //    var allActivities = aBal.GetOrderActivitiesByEncounterIdSP(encounterid);




                //    var labActivitesListObj = allActivities.Where(x => x.OrderCategoryID == (int)OrderTypeCategory.PathologyandLaboratory);
                //    closedActivities = labActivitesListObj.Where(x => !openStatuses.Any(o => o == x.OrderActivityStatus.Value))
                //                                .OrderBy(x => x.ExecutedDate).ToList();

                //    openActivities = labActivitesListObj.Where(x => openStatuses.Any(o => o == x.OrderActivityStatus.Value))
                //                            .OrderBy(x => x.OrderScheduleDate).ToList();


                //}


                var ordersAndActivities = orderbal.GetOrdersAndActivitiesByEncounter(encounterid);
                var labOrders = ordersAndActivities.OpenOrders.Where(a => a.CategoryId == (int)GlobalCodeCategoryValue.LabTest);

                openOrders = labOrders
                    .Where(a => a.OrderStatus == "0" || a.OrderStatus.Equals(Convert.ToString((int)OrderStatus.Open))).ToList();
                closedOrders = labOrders
                    .Where(a => a.OrderStatus != "0" && !a.OrderStatus.Equals(Convert.ToString((int)OrderStatus.Open))).ToList();


                var activities = ordersAndActivities.OrderActivities
                    .Where(x => x.OrderCategoryID == (int)OrderTypeCategory.PathologyandLaboratory);

                openActivities = activities.Where(x => openStatuses.Any(o => o == x.OrderActivityStatus.Value))
                                            .OrderBy(x => x.OrderScheduleDate).ToList();
                closedActivities = activities.Where(x => !openStatuses.Any(o => o == x.OrderActivityStatus.Value))
                                            .OrderBy(x => x.ExecutedDate).ToList();
            }

            var medicalvitalsview = new MedicalVitalView
            {
                CurrentMedicalVital = new MedicalVitalCustomModel(),
                MedicalVitalList = vitals,
                ClosedOrdersList = closedOrders,
                OpenOrdersList = openOrders,
                LabOpenOrdersActivitesList = openActivities,
                ClosedOrdersActivitesList = new List<OrderActivityCustomModel>(),
                OpenOrdersActivitesList = new List<OrderActivityCustomModel>(),
                ClosedLabOrdersActivitesList = closedActivities,
                EncounterOrder = new OpenOrder(),
                IsLabTest = true
            };

            return PartialView(PartialViews.LabTestView, medicalvitalsview);
        }

        #endregion

        #region Pharmacy Tab Data

        /// <summary>
        ///     Binds the pharmacy closed activity list.
        /// </summary>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindPharmacyClosedActivityList(int encounterId)
        {
            using (
                var bal = new OrderActivityBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var list =
                    bal.GetOrderActivitiesByEncounterId(encounterId)
                        .Where(
                            x =>
                            x.OrderCategoryID == Convert.ToInt32(OrderTypeCategory.Pharmacy)
                            && x.OrderActivityStatus != 0
                            && x.OrderActivityStatus != Convert.ToInt32(OpenOrderActivityStatus.Open))
                        .OrderBy(x => x.ExecutedDate)
                        .ToList();
                return PartialView(PartialViews.OrderClosedActivityScheduleList, list);
            }
        }

        /// <summary>
        ///     Binds the pharmacy closed orders.
        /// </summary>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindPharmacyClosedOrders(int encounterId)
        {
            using (
                var bal = new OpenOrderBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var list =
                    bal.GetPhysicianOrders(encounterId, OrderStatus.Closed.ToString())
                        .Where(x => x.CategoryId == Convert.ToInt32(OrderTypeCategory.Pharmacy))
                        .OrderBy(x => x.OpenOrderID)
                        .ToList();
                return PartialView(PartialViews.ClosedOrdersList, list);
            }
        }

        /// <summary>
        ///     Binds the pharmacy activity list.
        /// </summary>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindPharmacyOpenActivityList(int encounterId)
        {
            using (
                var bal = new OrderActivityBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var list =
                    bal.GetOrderActivitiesByEncounterId(encounterId)
                        .Where(
                            x =>
                            x.OrderCategoryID == Convert.ToInt32(OrderTypeCategory.Pharmacy)
                            && (x.OrderActivityStatus == Convert.ToInt32(OpenOrderActivityStatus.Open)
                                || x.OrderActivityStatus == 0))
                        .OrderBy(x => x.OrderScheduleDate)
                        .ToList();
                return PartialView(PartialViews.OrderActivityScheduleList, list);
            }
        }

        /// <summary>
        ///     Binds the pharmacy open order list.
        /// </summary>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindPharmacyOpenOrderList(string encounterId)
        {
            var encounterIdint = Convert.ToInt32(encounterId);
            var encounterOrderbal = new OpenOrderBal(
                Helpers.DefaultCptTableNumber,
                Helpers.DefaultServiceCodeTableNumber,
                Helpers.DefaultDrgTableNumber,
                Helpers.DefaultDrugTableNumber,
                Helpers.DefaultHcPcsTableNumber,
                Helpers.DefaultDiagnosisTableNumber);
            var status = OrderStatus.Open.ToString();
            var listOfOrders =
                encounterOrderbal.GetPhysicianOrders(encounterIdint, status).OrderBy(x => x.OpenOrderID).ToList();
            return PartialView(PartialViews.PhysicianOpenOrderList, listOfOrders);
        }

        ///// <summary>
        /////     Binds the pharmacy tab data.
        ///// </summary>
        ///// <param name="patientId">
        /////     The patient identifier.
        ///// </param>
        ///// <param name="encounterid">
        /////     The encounterid.
        ///// </param>
        ///// <returns>
        /////     The <see cref="ActionResult" />.
        ///// </returns>
        //public ActionResult BindPharmacyTabData(int patientId, int encounterid)
        //{
        //    using (
        //        var orderbal = new OpenOrderBal(
        //            Helpers.DefaultCptTableNumber,
        //            Helpers.DefaultServiceCodeTableNumber,
        //            Helpers.DefaultDrgTableNumber,
        //            Helpers.DefaultDrugTableNumber,
        //            Helpers.DefaultHcPcsTableNumber,
        //            Helpers.DefaultDiagnosisTableNumber))
        //    {
        //        var openOrdersList = orderbal.GetPhysicianOrdersList(
        //            encounterid,
        //            Convert.ToInt32(OrderStatus.Open).ToString());
        //        var closedOrdersList = orderbal.GetPhysicianOrdersList(
        //            encounterid,
        //            Convert.ToInt32(OrderStatus.Closed).ToString());
        //        using (
        //            var orderActivityBal = new OrderActivityBal(
        //                Helpers.DefaultCptTableNumber,
        //                Helpers.DefaultServiceCodeTableNumber,
        //                Helpers.DefaultDrgTableNumber,
        //                Helpers.DefaultDrugTableNumber,
        //                Helpers.DefaultHcPcsTableNumber,
        //                Helpers.DefaultDiagnosisTableNumber))
        //        {
        //            var labactivitesobj = //new List<OrderActivityCustomModel>();
        //                                  //orderActivityBal.GetOrderActivitiesByEncounterId(encounterid);
        //                orderActivityBal.GetOrderActivitiesByEncounterIdSP(encounterid); //new changes
        //            var labActivitesListObj =
        //                labactivitesobj.Where(x => x.OrderCategoryID == Convert.ToInt32(OrderTypeCategory.Pharmacy))
        //                    .ToList();
        //            var labActivitesClosedListObj =
        //                labActivitesListObj.Where(
        //                    x =>
        //                    x.OrderActivityStatus != 0
        //                    && x.OrderActivityStatus != Convert.ToInt32(OpenOrderActivityStatus.Open)
        //                    && x.OrderActivityStatus
        //                    != Convert.ToInt32(OpenOrderActivityStatus.PartiallyExecutedForResult))
        //                    .OrderBy(x => x.ExecutedDate)
        //                    .ToList();
        //            var labActivitesOpenListObj =
        //                labActivitesListObj.Where(
        //                    x =>
        //                    x.OrderActivityStatus == 0
        //                    || x.OrderActivityStatus == Convert.ToInt32(OpenOrderActivityStatus.Open)
        //                    || x.OrderActivityStatus
        //                    == Convert.ToInt32(OpenOrderActivityStatus.PartiallyExecutedForResult))
        //                    .OrderBy(x => x.OrderScheduleDate)
        //                    .ToList();
        //            var pharmacyActivityView = new OrderActivityView
        //            {
        //                CurrentOrderActivity = new OrderActivity(),
        //                OpenOrdersList = openOrdersList,
        //                OrderActivityList = labActivitesOpenListObj,
        //                ClosedOrderActivityList =
        //                                                   labActivitesClosedListObj,
        //                ClosedOrdersList = closedOrdersList,
        //                ClosedLabOrderActivityList =
        //                                                   new List<OrderActivityCustomModel>(),
        //                labOrderActivityList =
        //                                                   new List<OrderActivityCustomModel>(),
        //                EncounterOrder = new OpenOrder(),
        //                IsLabTest = false
        //            };
        //            return PartialView(PartialViews.PharmacyActivitesView, pharmacyActivityView);
        //        }
        //    }
        //}

        /// <summary>
        ///     Binds the pharmacy tab data.
        /// </summary>
        /// <param name="patientId">
        ///     The patient identifier.
        /// </param>
        /// <param name="encounterid">
        ///     The encounterid.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindPharmacyTabData(int patientId, int encounterid)
        {
            using (
                var orderbal = new OpenOrderBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                #region Commented Code
                //var openOrdersList = orderbal.GetPhysicianOrdersList(
                //    encounterid,
                //    Convert.ToInt32(OrderStatus.Open).ToString());
                //var closedOrdersList = orderbal.GetPhysicianOrdersList(
                //    encounterid,
                //    Convert.ToInt32(OrderStatus.Closed).ToString());


                //using (
                //    var orderActivityBal = new OrderActivityBal(
                //        Helpers.DefaultCptTableNumber,
                //        Helpers.DefaultServiceCodeTableNumber,
                //        Helpers.DefaultDrgTableNumber,
                //        Helpers.DefaultDrugTableNumber,
                //        Helpers.DefaultHcPcsTableNumber,
                //        Helpers.DefaultDiagnosisTableNumber))
                //{
                //    var labactivitesobj = orderActivityBal.GetOrderActivitiesByEncounterIdSP(encounterid); //new changes
                //    var labActivitesListObj =
                //        labactivitesobj.Where(x => x.OrderCategoryID == Convert.ToInt32(OrderTypeCategory.Pharmacy))
                //            .ToList();
                //    var labActivitesClosedListObj =
                //        labActivitesListObj.Where(
                //            x =>
                //            x.OrderActivityStatus != 0
                //            && x.OrderActivityStatus != Convert.ToInt32(OpenOrderActivityStatus.Open)
                //            && x.OrderActivityStatus
                //            != Convert.ToInt32(OpenOrderActivityStatus.PartiallyExecutedForResult))
                //            .OrderBy(x => x.ExecutedDate)
                //            .ToList();
                //    var labActivitesOpenListObj =
                //        labActivitesListObj.Where(
                //            x =>
                //            x.OrderActivityStatus == 0
                //            || x.OrderActivityStatus == Convert.ToInt32(OpenOrderActivityStatus.Open)
                //            || x.OrderActivityStatus
                //            == Convert.ToInt32(OpenOrderActivityStatus.PartiallyExecutedForResult))
                //            .OrderBy(x => x.OrderScheduleDate)
                //            .ToList(); 
                #endregion

                //Get Orders and Activities From Database
                var vm = orderbal.GetPhysicianOrderPlusActivityList(encounterid, withActivities: true, categoryId: (int)OrderTypeCategory.Pharmacy);

                /* Intialize all required lists */
                var openOrdersList = new List<OpenOrderCustomModel>();
                var closedOrdersList = new List<OpenOrderCustomModel>();
                var oActivities = new List<OrderActivityCustomModel>();
                var cActivities = new List<OrderActivityCustomModel>();
                /* Intialize all required lists */


                //Initialize the activity statuses for further filterations
                var actStatuses = new[] { 0, 1, 40 };   //0: New, 1: Open and 40: PartiallyExecutedForResult

                //Check if object is null, If No, then filterations done, otherwise leave this block.
                if (vm != null)
                {
                    if (vm.OpenOrders.Any())
                    {
                        openOrdersList = vm.OpenOrders.Where(a => a.OrderStatus.Equals(Convert.ToString((int)OrderStatus.Open))).ToList();
                        closedOrdersList = vm.OpenOrders.Where(a => !a.OrderStatus.Equals(Convert.ToString((int)OrderStatus.Open))).ToList();
                    }
                    if (vm.OrderActivities.Any())
                    {
                        oActivities = vm.OrderActivities.Where(a => actStatuses.Contains(a.OrderActivityStatus.Value)).OrderBy(s => s.ExecutedDate).ToList();
                        cActivities = vm.OrderActivities.Where(a => !actStatuses.Contains(a.OrderActivityStatus.Value)).OrderBy(s => s.ExecutedDate).ToList();
                    }
                }

                var pharmacyActivityView = new OrderActivityView
                {
                    CurrentOrderActivity = new OrderActivity(),
                    OpenOrdersList = openOrdersList,
                    OrderActivityList = oActivities,            //labActivitesOpenListObj,
                    ClosedOrderActivityList = cActivities,      //labActivitesClosedListObj,
                    ClosedOrdersList = closedOrdersList,
                    ClosedLabOrderActivityList = new List<OrderActivityCustomModel>(),
                    labOrderActivityList = new List<OrderActivityCustomModel>(),
                    EncounterOrder = new OpenOrder(),
                    IsLabTest = false
                };

                return PartialView(PartialViews.PharmacyActivitesView, pharmacyActivityView);
            }
        }

        /// <summary>
        ///     Pharamacies the order activity administered.
        /// </summary>
        /// <Who>
        ///     Shashank Awasthy
        /// </Who>
        /// <when>
        ///     March 17 2016 - 11:52 AM
        /// </when>
        /// <Why>
        ///     Need to have a check in case of Pharmacy order: If any of the activity is on bill then
        ///     user can not edit the order, user can only cancel the order in that case.
        ///     This case is used to fix the problem of Editing order (In case of pharmacy the MAR view shows wrong data)
        /// </Why>
        /// <param name="orderId">The order identifier.</param>
        /// <returns></returns>
        public JsonResult PharamacyOrderActivityAdministered(int orderId)
        {
            using (
                var bal = new OrderActivityBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                bool isTrue = bal.PharamacyOrderActivityAdministered(orderId);
                return Json(isTrue, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Nurse Assessment form 

        /// <summary>
        ///     Binds the nurse assessment list.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="encounterid">The encounterid.</param>
        /// <returns></returns>
        public ActionResult BindNurseAssessmentList(int patientId, int encounterid)
        {
            var list = new DocumentsTemplatesBal().GetNurseDocuments(patientId, encounterid).ToList();
            // Pass the View Model in ActionResult to View PatientCarePlan
            return PartialView(PartialViews.NurseEnteredList, list);
        }

        /// <summary>
        ///     Gets the nurse assessment list.
        /// </summary>
        /// <param name="ecounterId">The ecounter identifier.</param>
        /// <param name="patinetId">The patinet identifier.</param>
        /// <returns></returns>
        public ActionResult GetNurseAssessmentList(int ecounterId, int patinetId)
        {
            using (var bal = new EncounterBal())
            {
                var list = bal.GetNurseAssessmentData(ecounterId, patinetId);
                return PartialView(PartialViews.PreEvaluationList, list);
            }
        }

        /// <summary>
        ///     Saves the nurse assessment.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public ActionResult SaveNurseAssessment([FromBody] Signature data)
        {
            var currentDateTime = Helpers.GetInvariantCultureDateTime();

            var serverPath = Server.MapPath("~");
            var ticks = currentDateTime.Ticks;
            var photo = Convert.FromBase64String(data.Value);

            var dir = new DirectoryInfo(HostingEnvironment.ApplicationPhysicalPath);

            using (
                var fs =
                    System.IO.File.Create(
                        Path.Combine(
                            dir.FullName + "AssessmentForm/",
                            string.Format("{0}.png", Convert.ToString(ticks) + "-" + data.EnmFileName))))
            {
                fs.Write(photo, 0, photo.Length);
            }

            var oDocumentsTemplates = new DocumentsTemplates
            {
                DocumentTypeID = Convert.ToInt32(data.EnmFromId),
                DocumentName = data.EnmFormText,
                DocumentNotes = data.EnmFormText,
                AssociatedID = Convert.ToInt32(data.PatientId),
                AssociatedType = 121,
                FileName = data.EnmFileName,
                FilePath =
                                                  "http://"
                                                  + Convert.ToString(Request.Url.Authority)
                                                  + "/AssessmentForm/" + Convert.ToString(ticks) + "-"
                                                  + data.EnmFileName + ".png",
                CreatedDate = currentDateTime,
                CreatedBy = Helpers.GetLoggedInUserId(),
                CorporateID = Helpers.GetSysAdminCorporateID(),
                FacilityID = Helpers.GetDefaultFacilityId(),
                PatientID = Convert.ToInt32(data.PatientId),
                EncounterID = Convert.ToInt32(data.EncounterId)
                //ExternalValue3 = "4950"
            };
            var oDocumentsTemplatesBal = new DocumentsTemplatesBal();
            oDocumentsTemplatesBal.AddUpdateDocumentTempate(oDocumentsTemplates);
            if (System.IO.File.Exists(serverPath + "/AssessmentForm/" + data.EnmFileName))
            {
                System.IO.File.Delete(serverPath + "/AssessmentForm/" + data.EnmFileName);
            }
            return
                Json(
                    "http://" + Convert.ToString(Request.Url.Authority) + "/AssessmentForm/"
                    + Convert.ToString(ticks) + "-" + data.EnmFileName + ".png");
        }

        #endregion

        #region Patient Care Plan Tab

        /// <summary>
        ///     Binds the care plan task data.
        /// </summary>
        /// <param name="careId">The care identifier.</param>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public ActionResult BindCarePlanTaskData(string careId, string patientId, int encounterId)
        {
            var cBal = new PatientCarePlanBal();
            var careTaskList = cBal.BindCarePlanTaskData(careId, patientId, encounterId);
            if (careTaskList.Count > 0)
            {
                var list = new List<PatientCarePlanCustomModel>();
                list.AddRange(
                    careTaskList.Select(
                        item =>
                        new PatientCarePlanCustomModel
                        {
                            Text =
                                    string.Format(
                                        "{0} - {1} (Plan:{2})",
                                        item.CarePlanTaskNumber,
                                        item.CarePlanTaskName,
                                        item.CarePlanName),
                            Value = item.TaskId.ToString(),
                            Id = item.Id
                        }));

                return Json(list, JsonRequestBehavior.AllowGet);
            }
            return Json(0);
        }

        /// <summary>
        ///     Patients the care plan main.
        /// </summary>
        /// <param name="patientId">
        ///     The patient identifier.
        /// </param>
        /// <param name="encounterid">
        ///     The encounterid.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult PatientCarePlanMain(int patientId, int encounterid)
        {
            // Initialize the PatientCarePlan BAL object
            var patientCarePlanBal = new PatientCarePlanBal();

            // Get the Entity list
            var patientCarePlanList =
                patientCarePlanBal.GetPatientCarePlanByPatientIdAndEncounterId(Convert.ToString(patientId), encounterid);

            // Intialize the View Model i.e. PatientCarePlanView which is binded to Main View Index.cshtml under PatientCarePlan
            var patientCarePlanView = new PatientCarePlanView
            {
                PatientCarePlanList = patientCarePlanList,
                CurrentPatientCarePlan = new PatientCarePlan(),
                CurrentCarePlanTask = new CarePlanTask()
            };

            // Pass the View Model in ActionResult to View PatientCarePlan
            return PartialView(PartialViews.PatientCarePlanView, patientCarePlanView);
        }

        /// <summary>
        ///     The bind patient care plan list.
        /// </summary>
        /// <param name="patientId">
        ///     The patient id.
        /// </param>
        /// <param name="encounterId">
        ///     The encounter id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindPatientCarePlanList(int patientId, int encounterId)
        {
            // Initialize the PatientCarePlan BAL object
            using (var patientCarePlanBal = new PatientCarePlanBal())
            {
                // Get the facilities list
                var patientCarePlanList =
                    patientCarePlanBal.GetPatientCarePlanByPatientIdAndEncounterId(
                        Convert.ToString(patientId),
                        encounterId);

                // Pass the ActionResult with List of PatientCarePlanViewModel object to Partial View PatientCarePlanList
                return PartialView(PartialViews.PatientCarePlanList, patientCarePlanList);
            }
        }

        /// <summary>
        ///     The delete patient care plan.
        /// </summary>
        /// <param name="id">
        ///     The id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult DeletePatientCarePlan(int id)
        {
            using (var bal = new PatientCarePlanBal())
            {
                // Get PatientCarePlan model object by current PatientCarePlan ID
                var currentPatientCarePlan = bal.GetPatientCarePlanById(id);
                var userId = Helpers.GetLoggedInUserId();

                // Check If PatientCarePlan model is not null
                if (currentPatientCarePlan != null)
                {
                    currentPatientCarePlan.IsActive = false;

                    currentPatientCarePlan.ModifiedBy = userId;
                    currentPatientCarePlan.ModifiedDate = Helpers.GetInvariantCultureDateTime();

                    // Update Operation of current PatientCarePlan
                    bal.DeletePatientCarePlan(currentPatientCarePlan);

                    // return deleted ID of current PatientCarePlan as Json Result to the Ajax Call.
                    return Json(true);
                }
            }

            // Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }

        /// <summary>
        ///     The get patient care plan data.
        /// </summary>
        /// <param name="id">
        ///     The id.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetPatientCarePlanData(int id)
        {
            using (var bal = new PatientCarePlanBal())
            {
                var currentCarePlanTask = bal.GetPatientCarePlanById(id);

                var fromdateStr = currentCarePlanTask.FromDate != null
                                      ? currentCarePlanTask.FromDate.Value.ToShortDateString()
                                      : "";
                var tillDateStr = currentCarePlanTask.TillDate != null
                                      ? currentCarePlanTask.TillDate.Value.ToShortDateString()
                                      : "";
                var jsonResult =
                    new
                    {
                        currentCarePlanTask.Id,
                        currentCarePlanTask.CorporateId,
                        currentCarePlanTask.FacilityId,
                        fromdateStr,
                        tillDateStr,
                        currentCarePlanTask.TaskId,
                        currentCarePlanTask.IsActive,
                        currentCarePlanTask.CarePlanId
                    };

                return Json(jsonResult, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        ///     The save care plan task.
        /// </summary>
        /// <param name="model">
        ///     The model.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult SaveCarePlanTask(CarePlanTaskCustomModel model)
        {
            // Initialize the newId variable 
            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            var dateTime = Helpers.GetInvariantCultureDateTime();
            var newId = -1;
            var userId = Helpers.GetLoggedInUserId();

            // Check if Model is not null 
            if (model != null)
            {
                using (var bal = new CarePlanTaskBal())
                {
                    model.FacilityId = facilityId;
                    model.CorporateId = corporateId;

                    // model.CarePlanId = 9999;// to be get from the table so that the new task will be saved under the single task care plan name.
                    if (model.Id > 0)
                    {
                        model.ModifiedBy = userId;
                        model.ModifiedDate = dateTime;
                    }
                    else
                    {
                        model.CreatedBy = userId;
                        model.CreatedDate = dateTime;
                    }

                    // Call the AddCarePlanTask Method to Add / Update current CarePlanTask
                    newId = bal.SaveCarePlanTaskCustomModel(model);

                    // var cModel = bal.GetCarePlanTaskById(Convert.ToInt32(model.TaskNumber));
                    var pcBal = new PatientCarePlanBal();
                    var listoadd = new List<PatientCarePlan>();

                    var patinetCareModel = new PatientCarePlan
                    {
                        //TaskId = model.TaskNumber, 
                        TaskId = Convert.ToString(newId),
                        CarePlanId = model.CarePlanId.ToString(),
                        CorporateId = model.CorporateId,
                        FacilityId = model.FacilityId,
                        PatientId = Convert.ToString(model.PatientId),
                        EncounterId = model.EncounterId,
                        CreatedBy = model.CreatedBy,
                        CreatedDate = model.CreatedDate,
                        IsActive = model.IsActive,
                        FromDate = model.StartDate,
                        TillDate = model.EndDate
                    };
                    listoadd.Add(patinetCareModel);

                    //pcBal.SavePatientCarePlanData(listoadd, false);
                    pcBal.SavePatientCarePlanData(listoadd, false);
                }
            }

            return Json(newId);
        }

        /// <summary>
        ///     The save patient care plan.
        /// </summary>
        /// <param name="model">
        ///     The model.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult SavePatientCarePlan(List<PatientCarePlan> model)
        {
            var pBal = new PatientCarePlanBal();
            // Initialize the newId variable 
            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            var dateTime = Helpers.GetInvariantCultureDateTime();
            var newId = -1;
            var userId = Helpers.GetLoggedInUserId();
            var cBal = new CarePlanTaskBal();

            // Check if Model is not null 
            if (model != null)
            {
                using (var bal = new PatientCarePlanBal())
                {
                    foreach (var patientCarePlan in model)
                    {
                        //bool check = pBal.CheckDuplicateTaskName(patientCarePlan.Id,
                        //    Convert.ToInt32(patientCarePlan.EncounterId), patientCarePlan.PatientId,
                        //    patientCarePlan.TaskId, Convert.ToDateTime(patientCarePlan.FromDate), Convert.ToDateTime(patientCarePlan.TillDate),
                        //    Convert.ToInt32(patientCarePlan.FacilityId), Convert.ToInt32(patientCarePlan.CorporateId));
                        //if (check)
                        //{
                        //    return Json("-1");
                        //}

                        //else
                        //{
                        var careId = cBal.CarePlanId(corporateId, facilityId, Convert.ToInt32(patientCarePlan.TaskId));
                        patientCarePlan.CorporateId = corporateId;
                        patientCarePlan.FacilityId = facilityId;
                        patientCarePlan.IsActive = true;
                        if (patientCarePlan.Id > 0)
                        {
                            patientCarePlan.ModifiedBy = userId;
                            patientCarePlan.ModifiedDate = dateTime;
                            patientCarePlan.CarePlanId = careId.ToString();
                        }
                        else
                        {
                            patientCarePlan.CreatedDate = dateTime;
                            patientCarePlan.CreatedBy = userId;
                            patientCarePlan.CarePlanId = careId.ToString();
                        }
                        //}
                    }
                    // Call the AddPatientCarePlan Method to Add / Update current PatientCarePlan
                    newId = bal.SavePatientCarePlanData(model, true);
                }
            }

            return Json(newId);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     The get diagnosis details.
        /// </summary>
        /// <param name="patientId">
        ///     The patient id.
        /// </param>
        /// <param name="encounterId">
        ///     The encounter id.
        /// </param>
        /// <returns>
        ///     The <see cref="DiagnosisView" />.
        /// </returns>
        private DiagnosisView GetDiagnosisDetails(int patientId, int encounterId)
        {
            DiagnosisCustomModel diagnosisModel = null;
            PatientInfoCustomModel patientInfo = null;
            List<DiagnosisCustomModel> list = null;
            List<DiagnosisCustomModel> previouslist = null;
            var isPrimary = true;

            using (
                var bal = new DiagnosisBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var dModel = bal.GetNewDiagnosisByEncounterId(encounterId, patientId);
                var dList = bal.GetDiagnosisList(patientId, encounterId);
                var previousDlist = bal.GetPreviousDiagnosisList(patientId, encounterId);
                list = dList != null && dList.Count > 0 ? dList : new List<DiagnosisCustomModel>();
                previouslist = previousDlist.Any() ? previousDlist : new List<DiagnosisCustomModel>();
                diagnosisModel = dModel ?? new DiagnosisCustomModel();
                patientInfo = bal.GetPatientDetailsByPatientId(patientId);
                isPrimary = list.Count(x => x.DiagnosisType == Convert.ToInt32(DiagnosisType.Primary)) == 0;
            }

            if (patientInfo != null)
            {
                diagnosisModel.PatientID = patientInfo.PatientInfo.PatientID;
                diagnosisModel.EncounterID = encounterId > 0
                                                 ? encounterId
                                                 : (patientInfo.CurrentEncounter != null
                                                        ? patientInfo.CurrentEncounter.EncounterID
                                                        : 0);
                diagnosisModel.CorporateID = patientInfo.CorporateId;
                diagnosisModel.FacilityID = patientInfo.PatientInfo.FacilityId;
            }

            diagnosisModel.IsPrimary = isPrimary;

            var diagnosisView = new DiagnosisView
            {
                CurrentDiagnosis = diagnosisModel,
                PatientInfo = patientInfo,
                DiagnosisList = list,
                previousDiagnosisList = previouslist
            };

            return diagnosisView;
        }

        /// <summary>
        ///     The get summary details.
        /// </summary>
        /// <param name="patientId">
        ///     The patient id.
        /// </param>
        /// <param name="sTab">
        ///     The s tab.
        /// </param>
        /// <returns>
        ///     The <see cref="PatientSummaryView" />.
        /// </returns>
        private PatientSummaryView GetSummaryDetails(int patientId, int sTab)
        {
            var vmData = new PatientSummaryView
            {
                ExternalLinkId = sTab,
                PatientId = patientId,
                OpenOrdersList = new List<OpenOrderCustomModel>(),
                EncountersList = new List<EncounterCustomModel>(),
                CurrentEncounterId = 0,
                MedicalRecordList = new List<MedicalRecord>(),
                DiagnosisId = 0,
                MedicalVitalList = new List<MedicalVitalCustomModel>(),
                PatientSummaryNotes = new List<MedicalNotesCustomModel>(),
                ClosedOrdersList = new List<OpenOrderCustomModel>(),
                AlergyList = new List<AlergyCustomModel>(),
                DiagnosisList = new List<DiagnosisCustomModel>(),
                Riskfactors = new RiskFactorViewModel()
            };

            using (var bal = new BillHeaderBal())
            {
                vmData.PatientInfo = bal.GetPatientDetailsByPatientId(patientId, 0, true);
                if (vmData.PatientInfo != null)
                    vmData.PatientInfo.PersonAge = Convert.ToInt32(vmData.PatientInfo.PatientInfo.PersonAge);

                vmData.CurrentEncounterId = vmData.PatientInfo != null && vmData.PatientInfo.CurrentEncounter != null
                    ? vmData.PatientInfo.CurrentEncounter.EncounterID : 0;
            }


            //using (var orderBal = new OpenOrderBal(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber,
            //        Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber))
            //{
            //    var vm = orderBal.GetPatientSummaryDataOnLoad(0, patientId);
            //    if (vm != null)
            //    {
            //        vmData.CurrentEncounterId = Convert.ToInt32(vm.CurrentEncounterId);
            //        vmData.PatientInfo = vm.PatientInfo;
            //        if (vmData.PatientInfo != null)
            //            vmData.PatientInfo.PersonAge = Convert.ToInt32(vmData.PatientInfo.PatientInfo.PersonAge);

            //        //if (sTab == 0)
            //        //{
            //        //    vmData.EncountersList = vm.Encounters;
            //        //    vmData.PatientId = patientId;
            //        //    vmData.MedicalRecordList = vm.MedicalRecords;
            //        //    vmData.MedicalVitalList = vm.Vitals;
            //        //    vmData.PatientSummaryNotes = vm.MedicalNotes;
            //        //    vmData.AlergyList = vm.AllergyRecords;
            //        //    vmData.DiagnosisList = vm.DiagnosisList;
            //        //    vmData.Riskfactors = vm.RiskFactor;

            //        //    if (vm.OpenOrders.Any())
            //        //    {
            //        //        vmData.OpenOrdersList = vm.OpenOrders.Where(a => a.OrderStatus.Equals(Convert.ToString((int)OrderStatus.Open))).ToList();
            //        //        vmData.ClosedOrdersList = vm.OpenOrders.Where(a => !a.OrderStatus.Equals(Convert.ToString((int)OrderStatus.Open))).ToList();
            //        //    }
            //        //    else
            //        //    {
            //        //        vmData.OpenOrdersList = new List<OpenOrderCustomModel>();
            //        //        vmData.ClosedOrdersList = new List<OpenOrderCustomModel>();
            //        //    }

            //        //    vmData.DiagnosisId = vm.DiagnosisList.Any(a => a.DiagnosisType == (int)DiagnosisType.Primary) ? vm.DiagnosisList.SingleOrDefault(d => d.DiagnosisType == (int)DiagnosisType.Primary).DiagnosisID : 0;
            //        //}
            //        //else
            //        //{
            //        //    vmData.OpenOrdersList = new List<OpenOrderCustomModel>();
            //        //    vmData.EncountersList = new List<EncounterCustomModel>();
            //        //    vmData.CurrentEncounterId = 0;
            //        //    vmData.MedicalRecordList = new List<MedicalRecord>();
            //        //    vmData.DiagnosisId = 0;
            //        //    vmData.MedicalVitalList = new List<MedicalVitalCustomModel>();
            //        //    vmData.PatientSummaryNotes = new List<MedicalNotesCustomModel>();
            //        //    vmData.ClosedOrdersList = new List<OpenOrderCustomModel>();
            //        //    vmData.AlergyList = new List<AlergyCustomModel>();
            //        //    vmData.DiagnosisList = new List<DiagnosisCustomModel>();
            //        //    vmData.Riskfactors = new RiskFactorViewModel();
            //        //}
            //    }
            //}

            return vmData;
        }

        /// <summary>
        ///     Updates the current open order activties.
        /// </summary>
        /// <param name="orderid">
        ///     The orderid.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        private bool UpdateCurrentOpenOrderActivties(int orderid)
        {
            var orderActivityBal = new OrderActivityBal(
                Helpers.DefaultCptTableNumber,
                Helpers.DefaultServiceCodeTableNumber,
                Helpers.DefaultDrgTableNumber,
                Helpers.DefaultDrugTableNumber,
                Helpers.DefaultHcPcsTableNumber,
                Helpers.DefaultDiagnosisTableNumber);
            var orderbal = new OpenOrderBal(
                Helpers.DefaultCptTableNumber,
                Helpers.DefaultServiceCodeTableNumber,
                Helpers.DefaultDrgTableNumber,
                Helpers.DefaultDrugTableNumber,
                Helpers.DefaultHcPcsTableNumber,
                Helpers.DefaultDiagnosisTableNumber);
            var orderDetail = orderActivityBal.GetOrderActivitiesByOrderId(Convert.ToInt32(orderid));
            var orderObj = orderbal.GetOpenOrderDetail(orderid);
            if (orderDetail.Any())
            {
                var openorderActivities =
                    orderDetail.Where(
                        x =>
                        x.OrderActivityStatus == 0
                        || x.OrderActivityStatus == Convert.ToInt32(OpenOrderActivityStatus.Open)).ToList();
                var userid = Helpers.GetLoggedInUserId();
                var currentDatetime = Helpers.GetInvariantCultureDateTime();
                var encounterId = 0;
                foreach (var orderactivityobj in openorderActivities)
                {
                    orderactivityobj.OrderActivityStatus = Convert.ToInt32(OpenOrderActivityStatus.Closed);
                    orderactivityobj.ModifiedBy = userid;
                    orderactivityobj.ModifiedDate = currentDatetime;
                    orderactivityobj.ExecutedBy = userid;
                    orderactivityobj.ExecutedDate = currentDatetime;
                    orderactivityobj.ExecutedQuantity = orderObj.Quantity;
                    encounterId = Convert.ToInt32(orderactivityobj.EncounterID);
                    orderActivityBal.AddUptdateOrderActivity(orderactivityobj);
                }

                var corporateId = Helpers.GetSysAdminCorporateID();
                var facilityId = Helpers.GetDefaultFacilityId();

                // Apply Order Activity To Bill, added by Amit Jain on 24122014
                return orderActivityBal.ApplyOrderActivityToBill(
                    corporateId,
                    facilityId,
                    Convert.ToInt32(encounterId),
                    string.Empty,
                    0);
            }

            return false;
        }

        /// <summary>
        ///     Adds the open order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        private int AddOpenOrder(OpenOrderCustomModel order)
        {
            using (
                var encounterComm = new OpenOrderBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var userId = Helpers.GetLoggedInUserId();
                var corporateId = Helpers.GetSysAdminCorporateID();
                var facilityId = Helpers.GetDefaultFacilityId();
                var days = (Convert.ToDateTime(order.EndDate) - Convert.ToDateTime(order.StartDate)).TotalDays;
                var periodDays = days + 1;
                order.PeriodDays = Convert.ToString(periodDays);
                order.FacilityID = facilityId;
                order.CorporateID = corporateId;
                if (order.OrderStatus == Convert.ToString((int)OrderStatus.Closed)) order.IsActivitySchecduled = true;
                else order.IsApproved = order.CategoryId != Convert.ToInt32(OrderTypeCategory.Pharmacy);

                if (order.OpenOrderID > 0)
                {
                    order.ModifiedBy = userId;
                    order.PhysicianID = userId;
                    order.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                    order.OpenOrderPrescribedDate = Helpers.GetInvariantCultureDateTime();
                }
                else
                {
                    order.CreatedBy = userId;
                    order.CreatedDate = Helpers.GetInvariantCultureDateTime();
                    order.PhysicianID = userId;
                    order.OpenOrderPrescribedDate = Helpers.GetInvariantCultureDateTime();
                    order.IsActivitySchecduled = null;
                    order.ActivitySchecduledOn = null;
                }

                var orderId = encounterComm.AddUpdateOpenOrderCustomModel(order);
                return orderId;
            }
        }



        #endregion

        #region Bind Order Administrator Js

        public ActionResult BindOrderAdministratorOrder()
        {
            var orderBal = new OpenOrderBal(
           Helpers.DefaultCptTableNumber,
           Helpers.DefaultServiceCodeTableNumber,
           Helpers.DefaultDrgTableNumber,
           Helpers.DefaultDrugTableNumber,
           Helpers.DefaultHcPcsTableNumber,
           Helpers.DefaultDiagnosisTableNumber);
            var gccvalues = orderBal.GetGlobalCodesByCategoriesSp("3103,1024,3102,1011");
            var jsonData =
                new
                {
                    listActivityList = gccvalues.Where(g => g.GlobalCodeCategoryValue.Equals("3103")).OrderBy(d => d.GlobalCodeName).ToList(),
                    listFrequencyList = gccvalues.Where(g => g.GlobalCodeCategoryValue.Equals("1024")).OrderBy(d => d.GlobalCodeName).ToList(),
                    listOrderStatus = gccvalues.Where(g => g.GlobalCodeCategoryValue.Equals("3102")).OrderBy(d => d.GlobalCodeName).ToList(),

                    listQualityList =
                            gccvalues.Where(g => g.GlobalCodeCategoryValue.Equals("1011"))
                                .OrderBy(m => Convert.ToDecimal(m.GlobalCodeValue))
                                .ToList(),
                    //listExecutedQuantityList =
                    // gccvalues.Where(g => g.GlobalCodeCategoryValue.Equals("1011"))
                    // .OrderBy(m => Convert.ToDecimal(m.GlobalCodeValue))
                    //.ToList(),
                };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region Approve Pharmacy Orders
        /// <summary>
        ///     Approves the pharmacy order.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="type">The type.</param>
        /// <param name="comment">The comment.</param>
        /// <returns></returns>
        public JsonResult ApprovePharmacyOrderOld(int id, string type, string comment)
        {
            var bal = new OpenOrderBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber);

            var isExecuted = bal.ApprovePharmacyOrder(id, type, comment);

            return Json(isExecuted, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     Approves the pharmacy order.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="type">The type.</param>
        /// <param name="comment">The comment.</param>
        /// <returns></returns>
        public JsonResult ApprovePharmacyOrder(int id, string type, string comment, int encounterId, int categoryId)
        {
            var bal = new OpenOrderBal(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber,
                            Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber);

            var vm = bal.ApprovePharmacyOrder(id, type, comment, encounterId, true, categoryId, Helpers.GetLoggedInUserId(), Helpers.GetSysAdminCorporateID(), Helpers.GetDefaultFacilityId());
            if (vm != null)
            {
                var listopenOrders = new List<OpenOrderCustomModel>();
                var closedorderlist = new List<OpenOrderCustomModel>();
                var oActivities = new List<OrderActivityCustomModel>();
                var cActivities = new List<OrderActivityCustomModel>();
                var labWActivities = new List<OrderActivityCustomModel>();

                var orderActivityStatuses = new[] { 0, 1, 30, 20, 40 };
                var labOrderActStatuses = new[] { 0, 1, 20 };

                //Bind Orders Data
                if (vm.OpenOrders.Any())
                {
                    listopenOrders = vm.OpenOrders.Where(a => a.OrderStatus.Equals(Convert.ToString((int)OrderStatus.Open))).OrderBy(o => o.OpenOrderID).ToList();
                    closedorderlist = vm.OpenOrders.Where(a => !a.OrderStatus.Equals(Convert.ToString((int)OrderStatus.Open))).OrderBy(o => o.OpenOrderID).ToList();
                }

                //Bind Orders Activities Data
                if (vm.OrderActivities.Any())
                {
                    var allActivities = vm.OrderActivities;
                    oActivities = allActivities.Where(a => orderActivityStatuses.Contains(a.OrderActivityStatus.Value) && (categoryId == 0 || a.OrderCategoryID == categoryId)).OrderBy(x => x.OrderScheduleDate).ToList();
                    cActivities = allActivities.Where(a => !orderActivityStatuses.Contains(a.OrderActivityStatus.Value) && (categoryId == 0 || a.OrderCategoryID == categoryId)).OrderBy(x => x.ExecutedDate).ToList();
                    labWActivities = allActivities.Where(a => labOrderActStatuses.Contains(a.OrderActivityStatus.Value) && (categoryId == 0 || a.OrderCategoryID == categoryId)).OrderBy(x => x.OrderScheduleDate).ToList();
                }


                //Bind JSON Data
                var jsonData = new
                {
                    closedorderslist = RenderPartialViewToStringBase(categoryId == 11080 ? PartialViews.LabClosedOrderList
                                        : PartialViews.ClosedOrdersList, closedorderlist),
                    closedorderActivityslist = RenderPartialViewToStringBase(categoryId == 11080 ? PartialViews.LabClosedActivtiesList
                                        : PartialViews.OrderClosedActivityScheduleList, cActivities),
                    openorderActivityslist = RenderPartialViewToStringBase(
                                    categoryId == 11080 ? PartialViews.LabOpenActivtiesList
                                        : PartialViews.OrderActivityScheduleList, oActivities),
                    openOrderslist = RenderPartialViewToStringBase(
                                    categoryId == 11080 ? PartialViews.LabOpenOrderList
                                        : PartialViews.PhysicianOpenOrderList, listopenOrders),
                    labWaitingSpecimenList = RenderPartialViewToStringBase(PartialViews.LabSpecimanListing, labWActivities),
                    mostRecentOrders = RenderPartialViewToStringBase(PartialViews.PhyAllOrdersSummary, vm.PreviousOrders),
                };


                var jsonResult = new JsonResult { Data = jsonData, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = int.MaxValue };
                return jsonResult;
            }
            else
                return Json("", JsonRequestBehavior.AllowGet);
        }
        #endregion


        public ActionResult PhysicianOrNurseTabData(int patientId, int type, int encounterid)
        {
            using (var oBal = new OpenOrderBal(
                        Helpers.DefaultCptTableNumber,
                        Helpers.DefaultServiceCodeTableNumber,
                        Helpers.DefaultDrgTableNumber,
                        Helpers.DefaultDrugTableNumber,
                        Helpers.DefaultHcPcsTableNumber,
                        Helpers.DefaultDiagnosisTableNumber))
            {
                var tData = oBal.GetPhysicianOrNurseTabData(encounterid, patientId, Helpers.GetLoggedInUserId(), type);
                var openOrdersList = new List<OpenOrderCustomModel>();
                var closedOrdersList = new List<OpenOrderCustomModel>();
                var oActivitiesList = new List<OrderActivityCustomModel>();
                var cActivitiesList = new List<OrderActivityCustomModel>();

                if (tData != null && tData.OpenOrders.Any())
                {
                    openOrdersList = tData.OpenOrders.Where(o => o.OrderStatus.Equals(Convert.ToString((int)OrderStatus.Open))).ToList();
                    closedOrdersList = tData.OpenOrders.Where(o => !o.OrderStatus.Equals(Convert.ToString((int)OrderStatus.Open))).ToList();

                    if (tData.PatientCareActivities.Any())
                    {
                        var orderActivityStatuses = new[] { 0, 1, 40 };
                        cActivitiesList = tData.PatientCareActivities.Where(a => !orderActivityStatuses.Contains(a.OrderActivityStatus.Value))
                            .OrderByDescending(a => a.ExecutedDate)
                            .ToList();
                        oActivitiesList = tData.PatientCareActivities.Where(a => orderActivityStatuses.Contains(a.OrderActivityStatus.Value))
                            .OrderByDescending(a => a.ExecutedDate)
                            .ToList();
                    }
                }

                var viewData = new MedicalNotesView
                {
                    CurrentMedicalNotes = new MedicalNotes { NotesUserType = type, IsDeleted = false },
                    MedicalNotesList = tData != null ? tData.MedicalNotes2 : new List<MedicalNotesCustomModel>(),
                    OpenOrdersList = openOrdersList,
                    ClosedOrdersList = closedOrdersList,
                    OpenActvitiesList = cActivitiesList,
                    ClosedActvitiesList = oActivitiesList,
                    ClosedLabOrdersActivitesList = new List<OrderActivityCustomModel>(),
                    LabOpenOrdersActivitesList = new List<OrderActivityCustomModel>(),
                    EncounterOrder = new OpenOrder(),
                    IsLabTest = false,
                    CurrentMedicalVital = new MedicalVitalCustomModel(),
                    MedicalVitalList = tData.Vitals2,
                    PatientInfoId = patientId,
                    PatientEncounterId = encounterid,
                    EncounterList = tData.EncounterListData,
                    NurseEnteredFormList = tData.NurseDocuments,
                    NurseDocList = tData.EncounterListData.Where(e => e.ExtValue2.Trim().Equals("99")).ToList(),
                };

                var pViewResult = RenderPartialViewToStringBase(PartialViews.NotesTabView, viewData);
                var jsonData = new
                {
                    pViewResult,
                    dropdownlistdata = tData.DropdownListData,
                    orderTypeCategories = tData.OrderTypes,
                    LabOrders = tData.LabOrders
                };
                var ss = Json(jsonData, JsonRequestBehavior.AllowGet);
                ss.MaxJsonLength = int.MaxValue;
                return ss;
            }

        }


        #region Add / Update Physician Orders
        /// <summary>
        ///     Adds the physician order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        public ActionResult AddPhysicianOrderNew(OpenOrderCustomModel order)
        {
            var tabid = Convert.ToInt32(order.TabId);
            var orderBal = new OpenOrderBal(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber,
                Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber);

            //Add / Update Order Details 
            var newOrder = orderBal.SavePhysicianOrder(order, Helpers.GetLoggedInUserId(), 100, Helpers.GetSysAdminCorporateID(),
                Helpers.GetDefaultFacilityId(), order.EncounterID.Value, "", order.PatientID.Value, "7");

            // Order Activities Section, starts here
            var orderActivities = newOrder.OrderActivities;
            var openActStatuses = new[] { 0, 1, 30, 20, 40 };
            var labOrderActStatuses = new[] { 0, 1, 20 };
            var openOrderActivityList = orderActivities.Where(a => openActStatuses.Contains(Convert.ToInt32(a.OrderActivityStatus))).ToList();
            var closedOrderActivityList = orderActivities.Where(a => !openActStatuses.Contains(Convert.ToInt32(a.OrderActivityStatus))).ToList();
            var labWActivities = orderActivities.Where(a => labOrderActStatuses.Contains(a.OrderActivityStatus.Value) && (tabid == 0 || a.OrderCategoryID == tabid)).OrderBy(x => x.OrderScheduleDate).ToList();
            // Order Activities Section, ends here

            //Orders Section
            var orders = newOrder.OpenOrders.ToList();
            var closedOrderStatuses = new[] { "2", "3", "4", "9" };
            var openOrderList = orders.Where(a => a.OrderStatus.Equals("1")).ToList();
            var closedOrdersList = orders.Where(a => closedOrderStatuses.Contains(a.OrderStatus)).ToList();
            //Orders Section

            var jsonData = new
            {
                AllPhysicianOrders = RenderPartialViewToStringBase(PartialViews.MostRecentOrders, newOrder.PreviousOrders),
                ClosedOrdersList = RenderPartialViewToStringBase(tabid == 11080 ? PartialViews.LabClosedOrderList : PartialViews.ClosedOrdersList, closedOrdersList),
                FavoriteOrders = newOrder.FavoriteOrders,
                MostRecentOrders = RenderPartialViewToStringBase(PartialViews.MostRecentOrders, newOrder.MostRecentOrders),
                OpenOrdersList = RenderPartialViewToStringBase(tabid == 11080 ? PartialViews.LabOpenOrderList : PartialViews.PhysicianOpenOrderList, openOrderList),
                ClosedOrderActivityList = RenderPartialViewToStringBase(tabid == 11080 ?
                                                PartialViews.LabClosedActivtiesList : PartialViews.OrderClosedActivityScheduleList,
                                                            closedOrderActivityList),
                OpenOrderActivityList = RenderPartialViewToStringBase(tabid == 11080 ? PartialViews.LabOpenActivtiesList
                                                : PartialViews.OrderActivityScheduleList, openOrderActivityList),
                orderid = newOrder.OrderId,
                futureOrdersList = RenderPartialViewToStringBase(PartialViews.FutureOpenOrders, newOrder.FutureOpenOrders),
                labWaitingSpecimenList = RenderPartialViewToStringBase(PartialViews.LabSpecimanListing, labWActivities)
            };

            var jsonResult = Json(jsonData);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }
        #endregion




        /// <summary>
        ///     Saves the open order activity schedule.
        /// </summary>
        /// <param name="model">
        ///     The model.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult AdministerOrderActivity(OrderActivityCustomModel model)
        {
            var result = -1;
            using (
                var bal = new OrderActivityBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var userId = Helpers.GetLoggedInUserId();
                var corporateId = Helpers.GetSysAdminCorporateID();
                var facilityId = Helpers.GetDefaultFacilityId();
                var currentdatetime = Helpers.GetInvariantCultureDateTime();
                var activityId = model.OrderActivityID;
                model.ExecutedBy = userId;
                model.ExecutedDate = Helpers.GetInvariantCultureDateTime();
                model.CorporateID = corporateId;
                model.FacilityID = facilityId;
                if (model.OrderActivityID > 0)
                {
                    var objOrderActivityBal = new OrderActivityBal();
                    var obj = objOrderActivityBal.GetOrderActivityByID(model.OrderActivityID);
                    model.ModifiedBy = userId;
                    model.ModifiedDate = currentdatetime;
                    model.ExecutedQuantity = model.OrderCategoryID
                                             == Convert.ToInt32(OrderTypeCategory.PathologyandLaboratory)
                                                 ? 1
                                                 : model.ExecutedQuantity;
                    model.OrderActivityStatus = model.OrderActivityStatus == 2 || model.OrderActivityStatus == 4
                                                    ? (model.OrderCategoryID
                                                      == Convert.ToInt32(OrderTypeCategory.PathologyandLaboratory)
                                                          ? (model.OrderActivityStatus == 4
                                                                ? model.OrderActivityStatus
                                                                : 2)
                                                          : model.OrderActivityStatus)
                                                    : model.OrderActivityStatus;
                    model.BarCodeValue = obj.BarCodeValue;
                    model.BarCodeHtml = obj.BarCodeHtml;
                    var openActivityItemObj = new OrderActivityMapper().MapCustomModelToModel(model);
                    bal.AddUptdateOrderActivity(openActivityItemObj);
                }
                else
                {
                    model.CreatedBy = userId;
                    model.CreatedDate = currentdatetime;
                }

                if (model.PartiallyExecutedBool != null && model.ExecutedQuantity != model.OrderActivityQuantity
                    && (bool)model.PartiallyExecutedBool)
                {
                    var quantityRemaining = Convert.ToDecimal(model.OrderActivityQuantity - model.ExecutedQuantity);
                    var isExecuted = bal.CreatePartiallyexecutedActivity(
                        model.OrderActivityID,
                        quantityRemaining,
                        model.PartiallyExecutedstatus);
                }
                // Apply Order Activity To Bill, added by Amit Jain on 24122014
                bal.ApplyOrderActivityToBill(
                    corporateId,
                    facilityId,
                    Convert.ToInt32(model.EncounterID),
                    string.Empty,
                    0);
                var orderActivities = bal.GetOrderActivitiesByOrderId(Convert.ToInt32(model.OrderID));
                var openorderactivties =
                    orderActivities.Any(
                        x =>
                        x.OrderActivityStatus == Convert.ToInt32(OpenOrderActivityStatus.Open)
                        || x.OrderActivityStatus == 0 || x.OrderActivityStatus == 40);

                // Update the Order Status
                if (openorderactivties)
                {
                    if (activityId == 0)
                    {
                        openorderactivties = !UpdateCurrentOpenOrderActivties(Convert.ToInt32(model.OrderID));
                    }
                }

                if (!openorderactivties)
                {
                    using (
                        var ordersBal = new OpenOrderBal(
                            Helpers.DefaultCptTableNumber,
                            Helpers.DefaultServiceCodeTableNumber,
                            Helpers.DefaultDrgTableNumber,
                            Helpers.DefaultDrugTableNumber,
                            Helpers.DefaultHcPcsTableNumber,
                            Helpers.DefaultDiagnosisTableNumber))
                    {
                        /*
                         * Changes By: Amit Jain
                         * On: 01 March, 2016
                         * Purpose: Just updates Current Open Order Status, nothing to do with Order Activities here below
                         * Earlier, Order Activities having ExecutedQuantities were deleted by using the Method AddUpdatePhysicianOrder
                         * And that method has been updated and so, now commented below
                         * Now, changed the Method with the new one i.e. UpdateOpenOrderStatus
                         */

                        //***************************Code Changes start here*******************************************************

                        //var openorder = ordersBal.GetOpenOrderDetail(Convert.ToInt32(model.OrderID));
                        //openorder.OrderStatus = Convert.ToString((int)OrderStatus.OnBill);
                        //openorder.ModifiedBy = userId;
                        //openorder.ModifiedDate = currentdatetime;
                        //ordersBal.AddUpdatePhysicianOpenOrder(openorder);

                        ordersBal.UpdateOpenOrderStatus(
                            Convert.ToInt32(model.OrderID),
                            Convert.ToString((int)OrderStatus.OnBill),
                            userId,
                            currentdatetime);

                        //***************************Code Changes End here*********************************************************
                    }
                }

                result = model.OrderActivityID;
            }

            return Json(result);
        }




        #region Changes done in Binding of Order Type Categories and Sub-Categories
        public ActionResult GetOrderTypeCategoriesInSummary(string startRange, string endRange)
        {
            var fn = Convert.ToString(Helpers.GetDefaultFacilityId());
            using (var bal = new GlobalCodeCategoryBal())
            {
                var list = bal.GetGlobalCodeCategoriesByExternalValue(fn);
                return Json(list);
            }
        }
        public ActionResult GetOrderTypeCategoriesInSummaryNew(string externalValue = "")
        {
            var fn = Convert.ToString(Helpers.GetDefaultFacilityId());
            using (var bal = new GlobalCodeCategoryBal())
            {
                var list = bal.GetGlobalCodeCategoriesByExternalValue(fn);
                if (externalValue != "0")
                    list = list.Where(x => x.ExternalValue4 != null && x.ExternalValue4.Contains(externalValue)).ToList();
                return Json(list);
            }
        }

        /// <summary>
        ///     Gets the order type sub categories.
        /// </summary>
        /// <param name="categoryId">
        ///     The category identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public JsonResult GetOrderTypeSubCategories(string categoryId)
        {
            using (var bal = new GlobalCodeBal())
            {
                var fn = Convert.ToString(Helpers.GetDefaultFacilityId());
                var globalCodes = bal.GetGlobalCodesByCategoryValue(categoryId, fn);
                var list = new List<DropdownListData>();

                /*
                 * Code Changes by Amit Jain on 02 December, 2015
                 * Issue: Order Codes were not coming based on the selected Sub-Category.
                 * Reason: Sub-Category ID which is considered as Global Code Value was using with Global Code ID instead of GC Value
                 */

                /*Old Code*/
                //list.AddRange(
                //    globalCodes.Select(
                //        items =>
                //        new DropdownListData
                //        {
                //            Text = items.GlobalCodeName,
                //            Value = Convert.ToString(items.GlobalCodeID),
                //            ExternalValue1 = items.ExternalValue1,
                //            ExternalValue2 = items.ExternalValue2,
                //            ExternalValue3 = items.ExternalValue3
                //        }));

                /*New Code*/
                list.AddRange(globalCodes.Select(items => new DropdownListData
                {
                    Text = items.GlobalCodeName,
                    Value = items.GlobalCodeValue,
                    ExternalValue1 = string.IsNullOrEmpty(items.ExternalValue1) ? string.Empty : items.ExternalValue1,
                    ExternalValue2 = string.IsNullOrEmpty(items.ExternalValue2) ? string.Empty : items.ExternalValue2,
                    ExternalValue3 = string.IsNullOrEmpty(items.ExternalValue3) ? string.Empty : items.ExternalValue3,
                    CategoryValue = string.IsNullOrEmpty(items.GlobalCodeCategoryValue) ? string.Empty : items.GlobalCodeCategoryValue,
                }));
                return Json(list);
            }
        }


        /// <summary>
        ///     Gets the order codes by sub category.
        /// </summary>
        /// <param name="subCategoryValue">
        ///     The sub category identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public JsonResult GetCodesBySubCategory(string subCategoryValue, string gcc, string orderCodeType, long startRange, long endRange)
        {

            using (var bal = new GlobalCodeBal())
            {
                var tableNumber = string.Empty;
                switch (orderCodeType)
                {
                    case "3":
                        tableNumber = Helpers.DefaultCptTableNumber;
                        break;
                    case "4":
                        tableNumber = Helpers.DefaultHcPcsTableNumber;
                        break;
                    case "5":
                        tableNumber = Helpers.DefaultDrugTableNumber;
                        break;
                    default:
                        break;
                }

                var vm = bal.GetOrderCodesByRange(tableNumber, gcc, subCategoryValue, orderCodeType, startRange, endRange, Helpers.GetDefaultFacilityId());
                var jsonResult =
                                new
                                {
                                    codeList = vm.OrderCodeList,
                                    codeTypeName = vm.GlobalCode.GlobalCodeName,
                                    codeTypeId = vm.GlobalCode.GlobalCodeValue
                                };
                return Json(jsonResult, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion



        #region All Orders Tab - Actions 
        /// <summary>
        ///     Patients the summary.
        /// </summary>
        /// <param name="pId">
        ///     The p identifier.
        /// </param>
        /// <param name="sTab">
        ///     The s Tab.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult Index(int? pId, int? sTab)
        {
            if (pId == null || Convert.ToInt32(pId) == 0)
            {
                return RedirectToAction(
                    ActionResults.patientSearch,
                    ControllerNames.patientSearch,
                    new { messageId = Convert.ToInt32(MessageType.ViewEHR) });
            }

            var patientId = Convert.ToInt32(pId);
            var vmData = GetSummaryDetails(patientId, Convert.ToInt32(sTab));
            return View(vmData);
        }
        #endregion


        public JsonResult BindSummaryTabData(long pId, long? eId)
        {
            var oBal = new OpenOrderBal();
            var vm = oBal.GetPatientSummaryDataOnLoad(eId ?? 0, pId);

            if (vm != null)
            {
                var jsonResult = new
                {
                    medications = vm.MedicalHistory.Select(f => new[] { f.Drug, f.DrugCode, f.DrugDuration
                    ,f.DrugVolume,f.DrugDosage,f.DrugFrequency }),

                    allergies = vm.AllergyRecords.Select(f => new[] { f.AlergyType,f.AlergyName,f.CurrentAlergy.DetailAnswer
                        ,Convert.ToString(f.AddedBy)
                        ,f.CurrentAlergy.CreatedDate.HasValue ? f.CurrentAlergy.CreatedDate.Value.ToString("d") : string.Empty }),

                    encounters = vm.Encounters.Select(e => new[] {
                    e.EncounterStartTime.Value.ToString("dd/MM/yyyy HH:mm:ss")
                    ,e.EncounterEndTime.HasValue ? e.EncounterStartTime.Value.ToString("dd/MM/yyyy HH:mm:ss") : "<span class='emtptyRow'>Encounter not end yet</span>"
                    ,e.EncounterNumber,e.EncounterTypeName,e.EncounterPatientTypeName,Convert.ToString(e.Charges)
                    ,Convert.ToString(e.Payment)
                    }),

                    diagList = vm.DiagnosisList.Select(d => new[] { d.DiagnosisTypeName, d.DiagnosisCode
                    ,d.DiagnosisCodeDescription,d.Notes
                    ,d.CreatedDate.HasValue ? d.CreatedDate.Value.ToString("dd/MM/yyyy"): string.Empty
                    ,d.EnteredBy
                    }),

                    currentOrders = vm.OpenOrders.Select(o => new[] {
                        o.OrderCode,o.OrderDescription,o.CategoryName,o.SubCategoryName,o.Status,Convert.ToString(o.Quantity)
                        ,o.FrequencyText,o.PeriodDays
                    }),

                    vitals = vm.Vitals.Select(v => new[] {
                        v.MedicalVitalName,v.PressureCustom,v.UnitOfMeasureName,v.VitalAddedBy,v.VitalAddedOn.ToString("dd/MM/yyyy")
                    }),

                    notes = vm.MedicalNotes.Select(n => new[] {
                        n.MedicalNotes.Notes,n.NotesUserTypeName,n.NotesAddedBy
                        ,n.MedicalNotes.NotesDate.HasValue ? n.MedicalNotes.NotesDate.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty
                    }),

                    currentEncounterId = eId
                };

                var s = Json(jsonResult, JsonRequestBehavior.AllowGet);
                s.MaxJsonLength = int.MaxValue;
                s.RecursionLimit = int.MaxValue;
                return s;
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }


        private PatientSummaryView GetSummaryDetails(int patientId, int sTab, int? encounterId)
        {
            var vmData = new PatientSummaryView { ExternalLinkId = sTab, PatientId = patientId };

            using (
                var orderBal = new OpenOrderBal(
                    Helpers.DefaultCptTableNumber,
                    Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber,
                    Helpers.DefaultDrugTableNumber,
                    Helpers.DefaultHcPcsTableNumber,
                    Helpers.DefaultDiagnosisTableNumber))
            {
                var currentEncounterId = encounterId ?? orderBal.GetActiveEncounterId(patientId);

                var patientInfo = orderBal.GetPatientDetailsByPatientId(patientId);
                vmData.PatientInfo = patientInfo;
                vmData.PatientInfo.PersonAge = Convert.ToInt32(vmData.PatientInfo.PatientInfo.PersonAge);
                vmData.CurrentEncounterId = currentEncounterId;
                vmData.PatientId = patientId;
                if (sTab == 0)
                {
                    var enList = orderBal.GetEncountersListByPatientId(patientId);
                    using (var medicalRecordbal = new MedicalRecordBal())
                    {
                        using (var medicalnotesbal = new MedicalNotesBal())
                        {
                            using (var medicalVitals = new MedicalVitalBal())
                            {
                                using (
                                    var diagnosisBal = new DiagnosisBal(
                                        Helpers.DefaultDiagnosisTableNumber,
                                        Helpers.DefaultDrgTableNumber))
                                {
                                    var medicalrecords = medicalRecordbal.GetMedicalRecord();
                                    var medicalvitals = medicalVitals.GetCustomMedicalVitalsByPidEncounterId(
                                        patientId,
                                        Convert.ToInt32(MedicalRecordType.Vitals),
                                        currentEncounterId);

                                    var patientSummaryNotes =
                                        medicalnotesbal.GetMedicalNotesByPatientIdEncounterId(
                                            patientId,
                                            currentEncounterId);
                                    var allergiesList = medicalRecordbal.GetAlergyRecords(
                                        patientId,
                                        Convert.ToInt32(MedicalRecordType.Allergies));

                                    var orderStatus = OrderStatus.Open.ToString();
                                    var openOrdersList = orderBal.GetPhysicianOrders(currentEncounterId, orderStatus);
                                    var primarydiagnosisId = 0;
                                    var diagnosisList = diagnosisBal.GetDiagnosisList(patientId, currentEncounterId);
                                    if (diagnosisList.Any())
                                    {
                                        var diagnosisCustomModel =
                                            diagnosisList.SingleOrDefault(
                                                x => x.DiagnosisType == Convert.ToInt32(DiagnosisType.Primary));
                                        if (diagnosisCustomModel != null)
                                        {
                                            primarydiagnosisId = diagnosisCustomModel.DiagnosisID;
                                        }
                                    }

                                    var riskFactors = medicalVitals.GetRiskFactors(patientId);

                                    vmData.OpenOrdersList = openOrdersList;
                                    vmData.EncountersList = enList;
                                    vmData.CurrentEncounterId = currentEncounterId;
                                    vmData.PatientId = patientId;
                                    vmData.MedicalRecordList = medicalrecords;
                                    vmData.DiagnosisId = primarydiagnosisId;
                                    vmData.MedicalVitalList = medicalvitals;
                                    vmData.PatientSummaryNotes = patientSummaryNotes;
                                    vmData.ClosedOrdersList = orderBal.GetPhysicianOrders(
                                        currentEncounterId,
                                        OrderStatus.Closed.ToString());
                                    vmData.AlergyList = allergiesList;
                                    vmData.DiagnosisList = diagnosisList;
                                    vmData.Riskfactors = riskFactors;
                                }
                            }
                        }
                    }
                }
                else
                {
                    vmData.OpenOrdersList = new List<OpenOrderCustomModel>();
                    vmData.EncountersList = new List<EncounterCustomModel>();
                    vmData.CurrentEncounterId = currentEncounterId;
                    vmData.MedicalRecordList = new List<MedicalRecord>();
                    vmData.DiagnosisId = 0;
                    vmData.MedicalVitalList = new List<MedicalVitalCustomModel>();
                    vmData.PatientSummaryNotes = new List<MedicalNotesCustomModel>();
                    vmData.ClosedOrdersList = new List<OpenOrderCustomModel>();
                    vmData.AlergyList = new List<AlergyCustomModel>();
                    vmData.DiagnosisList = new List<DiagnosisCustomModel>();
                    vmData.Riskfactors = new RiskFactorViewModel();
                }
            }

            return vmData;
        }


        /// <summary>
        ///     Gets the diagnosis tab data.
        /// </summary>
        /// <param name="patientId">
        ///     The patient identifier.
        /// </param>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult GetDiagnosisTabDetails(int patientId, int encounterId)
        {
            var vm = new DiagnosisCustomModel();
            List<DiagnosisCustomModel> list;
            List<DiagnosisCustomModel> previouslist;
            DiagnosisTabData viewData = null;
            var favOrders = new List<FavoritesCustomModel>();


            // string patientId, string encounterId
            var pid = Convert.ToInt64(patientId);
            var eid = Convert.ToInt32(encounterId);
            var userid = Helpers.GetLoggedInUserId();



            using (var bal = new DiagnosisBal(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber,
                    Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber))
            {
                viewData = bal.GetDiagnosisTabData(pid, eid, userid, Helpers.DefaultDiagnosisTableNumber, Helpers.DefaultDrgTableNumber);
                list = viewData.CurrentDiagnosisList;
                previouslist = viewData.PreviousDiagnosisList;
                favOrders = viewData.FavOrdersList;
                var isMajorCpt = list.All(x => x.DiagnosisType != 4);
                vm.IsMajorCPT = isMajorCpt;
                vm.IsMajorDRG = list.All(x => x.DiagnosisType != 3);
            }

            vm.PatientID = patientId;
            vm.EncounterID = eid;
            vm.CorporateID = Helpers.GetSysAdminCorporateID();
            vm.FacilityID = Helpers.GetDefaultFacilityId();
            vm.IsPrimary = !list.Any(x => x.DiagnosisType == (int)DiagnosisType.Primary);

            if (list.Any(x => x.DiagnosisType == Convert.ToInt32(DiagnosisType.Primary)))
                vm.PrimaryDiagnosisId = list.Where(a => a.DiagnosisType == (int)DiagnosisType.Primary).Select(d => d.DiagnosisID).FirstOrDefault();

            var jsonResult = new
            {
                CurrentDiagnosis = vm,
                DiagnosisList = list.Select(f => new[] {Convert.ToString(f.DiagnosisID),Convert.ToString(f.DiagnosisCodeId), f.DiagnosisTypeName, f.DiagnosisCode, f.DiagnosisCodeDescription,
                    f.Notes, f.CreatedDate.HasValue?f.CreatedDate.Value.ToString("d"):string.Empty, Convert.ToString( f.EnteredBy), Convert.ToString(f.DiagnosisType) }),

                previousDiagnosisList = previouslist.Select(f => new[] {Convert.ToString(f.DiagnosisID), f.EncounterNumber,f.DiagnosisTypeName,f.DiagnosisCode,f.DiagnosisCodeDescription
                ,!string.IsNullOrEmpty(f.Notes) ? f.Notes : string.Empty,
                 f.CreatedDate.HasValue?f.CreatedDate.Value.ToString("d"):string.Empty,Convert.ToString(f.DiagnosisID)}),
                FavoriteDiagnosisList = favOrders.Select(f => new[] { Convert.ToString(f.UserDefinedDescriptionID), f.CategoryName, f.CodeId, f.CodeDesc, f.UserDefineDescription, f.CodeId })
            };

            var s = Json(jsonResult, JsonRequestBehavior.AllowGet);
            s.MaxJsonLength = int.MaxValue;
            s.RecursionLimit = int.MaxValue;
            return s;
            //return PartialView(PartialViews.DiagnosisViewUC, diagnosisView);
            //return Json(diagnosisView);
        }

        /// <summary>
        ///     Gets the diagnosis tab data.
        /// </summary>
        /// <param name="patientId">
        ///     The patient identifier.
        /// </param>
        /// <param name="encounterId">
        ///     The encounter identifier.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult BindDiagnosisTabData(int patientId, int encounterId)
        {
            var vm = new DiagnosisCustomModel();
            List<DiagnosisCustomModel> list = new List<DiagnosisCustomModel>();
            List<DiagnosisCustomModel> previouslist = new List<DiagnosisCustomModel>();
            //DiagnosisTabData viewData = null;
            var favOrders = new List<FavoritesCustomModel>();


            // string patientId, string encounterId
            var pid = Convert.ToInt64(patientId);
            var eid = Convert.ToInt32(encounterId);
            var userid = Helpers.GetLoggedInUserId();



            //using (var bal = new DiagnosisBal(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber,
            //        Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber))
            //{
            //    viewData = bal.GetDiagnosisTabData(pid, eid, userid, Helpers.DefaultDiagnosisTableNumber, Helpers.DefaultDrgTableNumber);
            //    list = viewData.CurrentDiagnosisList;
            //    previouslist = viewData.PreviousDiagnosisList;

            //    var isMajorCpt = list.All(x => x.DiagnosisType != 4);
            //    vm.IsMajorCPT = isMajorCpt;
            //    vm.IsMajorDRG = list.All(x => x.DiagnosisType != 3);
            //}

            vm.PatientID = patientId;
            vm.EncounterID = eid;
            vm.CorporateID = Helpers.GetSysAdminCorporateID();
            vm.FacilityID = Helpers.GetDefaultFacilityId();
            vm.IsPrimary = !list.Any(x => x.DiagnosisType == (int)DiagnosisType.Primary);

            if (list.Any(x => x.DiagnosisType == Convert.ToInt32(DiagnosisType.Primary)))
                vm.PrimaryDiagnosisId = list.Where(a => a.DiagnosisType == (int)DiagnosisType.Primary).Select(d => d.DiagnosisID).FirstOrDefault();

            var diagnosisView = new DiagnosisView
            {
                CurrentDiagnosis = vm,
                DiagnosisList = list,
                previousDiagnosisList = previouslist,
                FavoriteDiagnosisList = favOrders
            };

            return PartialView(PartialViews.DiagnosisViewUC, diagnosisView);
        }


        public ActionResult GetOrdersViewData(string encounterId)
        {
            var userId = Helpers.GetLoggedInUserId();
            var corporateid = Helpers.GetSysAdminCorporateID();
            var facilityid = Helpers.GetDefaultFacilityId();
            var orderBal = new OpenOrderBal(
                Helpers.DefaultCptTableNumber,
                Helpers.DefaultServiceCodeTableNumber,
                Helpers.DefaultDrgTableNumber,
                Helpers.DefaultDrugTableNumber,
                Helpers.DefaultHcPcsTableNumber,
                Helpers.DefaultDiagnosisTableNumber);


            //DB Call to get all records related to Orders tab
            var ordersViewData = orderBal.OrdersViewData(userId, 100, corporateid, facilityid, Convert.ToInt32(encounterId), "1024,3102,1011,2305,963", 0, "7", string.Empty);

            // Order Activities Section, starts here
            var orderActivities = ordersViewData.OrderActivities;
            var openActStatuses = new[] { 0, 1, 30, 20, 40 };
            var openOrderActivityList = orderActivities.Where(a => openActStatuses.Contains(Convert.ToInt32(a.OrderActivityStatus))).ToList();
            var closedOrderActivityList = orderActivities.Where(a => !openActStatuses.Contains(Convert.ToInt32(a.OrderActivityStatus))).ToList();
            // Order Activities Section, ends here

            //Orders Section
            var orders = ordersViewData.OpenOrders.ToList();
            var closedOrderStatuses = new[] { "2", "3", "4", "9" };
            var openOrderList = orders.Where(a => a.OrderStatus.Equals("1")).ToList();
            var closedOrdersList = orders.Where(a => closedOrderStatuses.Contains(a.OrderStatus)).ToList();
            //Orders Section

            var newEncOrder = new OpenOrder
            {
                StartDate = Helpers.GetInvariantCultureDateTime(),
                EndDate = Helpers.GetInvariantCultureDateTime(),
                OrderStatus = Convert.ToString((int)OrderStatus.Open)
            };

            //GC Values 
            var gccvalues = ordersViewData.GlobalCodes;

            //json data to return to view
            var jsonData =
                new
                {
                    listFrequency = gccvalues.Where(g => g.GlobalCodeCategoryValue.Equals("1024")).OrderBy(d => d.GlobalCodeName).ToList(),
                    listOrderStatus = gccvalues.Where(g => g.GlobalCodeCategoryValue.Equals("3102")).OrderBy(d => d.GlobalCodeName).ToList(),
                    listQuantity = gccvalues.Where(g => g.GlobalCodeCategoryValue.Equals("1011")).OrderBy(m => Convert.ToDecimal(m.GlobalCodeValue)).ToList(),
                    listDocumentType = gccvalues.Where(g => g.GlobalCodeCategoryValue.Equals("2305")).ToList(),
                    listNoteType = gccvalues.Where(g => g.GlobalCodeCategoryValue.Equals("963")).ToList(),
                    FavoriteOrders = ordersViewData.FavoriteOrders.Select(x => new[] { Convert.ToString(x.UserDefinedDescriptionId), x.OrderTypeName, x.OrderCode, x.OrderDescription, x.UserDefinedDescription, string.Empty }),
                    MostRecentOrders = ordersViewData.MostRecentOrders.Select(x => new[] { Convert.ToString(x.OpenOrderID), x.OrderTypeName, x.OrderCode, x.OrderDescription, string.Empty }),
                    SearchOrders = ordersViewData.MostRecentOrders.Select(x => new[] { Convert.ToString(x.OpenOrderID), x.OrderTypeName, x.OrderCode, x.OrderDescription, x.CategoryName, x.SubCategoryName,
                        x.Status, x.FrequencyText, x.Quantity.HasValue ? Convert.ToString(x.Quantity.Value) : string.Empty, x.PeriodDays, x.OrderNotes, string.Empty }),
                    AllPhysicianOrders = ordersViewData.PreviousOrders.Select(x => new[] { x.OpenOrderID.ToString(), x.ActivityCode, x.OrderCode, x.OrderDescription, string.Empty }),
                    ClosedOrdersList = closedOrdersList.Select(x => new[] {x.Quantity.HasValue?x.Quantity.Value.ToString():string.Empty,x.OrderCode,x.OrderDescription,x.FrequencyText,x.CategoryName,
                    x.SubCategoryName,x.PeriodDays,x.OrderNotes,x.Status}),
                    EncounterOrder = newEncOrder,
                    OpenOrdersList = openOrderList.Select(x => new[] { Convert.ToString(x.OpenOrderID), x.OrderTypeName, x.OrderCode, x.OrderDescription, x.CategoryName, x.SubCategoryName,
                        x.Status, x.FrequencyText, x.Quantity.HasValue ? Convert.ToString(x.Quantity.Value) : string.Empty, x.PeriodDays, x.OrderNotes, string.Empty }),
                    ClosedOrderActivityList = closedOrderActivityList.Select(x => new[] { x.OrderTypeName, x.OrderCode, x.OrderDescription, x.CategoryName, x.SubCategoryName, x.OrderScheduleDate.HasValue?x.OrderScheduleDate.Value.ToString("d"):string.Empty,
                    x.ExecutedDate.HasValue?x.ExecutedDate.Value.ToString("d"):string.Empty,x.OrderActivityQuantity.HasValue?Convert.ToString(x.OrderActivityQuantity.Value):string.Empty,
                    x.ExecutedQuantity.HasValue?Convert.ToString(x.ExecutedQuantity.Value):string.Empty,x.Comments}),
                    OpenOrderActivityList = openOrderActivityList.Select(x => new[] {x.Status, Convert.ToString(x.ShowEditAction), Convert.ToString(x.OrderActivityID), Convert.ToString(x.OrderCategoryID),
                                x.OrderTypeName, x.OrderCode, x.OrderDescription, x.CategoryName, x.SubCategoryName, x.OrderScheduleDate.HasValue ? x.OrderScheduleDate.Value.ToString("d") : string.Empty, string.Empty }),
                    FutureOpenOrdersList = ordersViewData.FutureOpenOrders.Select(x => new[] { x.OrderCode,x.OrderDescription,x.CategoryName,x.SubCategoryName ,x.Status, x.Quantity.HasValue ? Convert.ToString(x.Quantity.Value) : string.Empty,
                        x.StartDate.HasValue?x.StartDate.Value.ToString("d"):string.Empty,x.CreatedDate.HasValue?x.CreatedDate.Value.ToString("d"):string.Empty,x.FrequencyText,x.PeriodDays,x.OrderNotes})
                };
            //json data to return to view


            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public ActionResult AddPhysicianOrderinSummary(OpenOrderCustomModel order)
        {
            var userId = Helpers.GetLoggedInUserId();
            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            var encounterId = order.EncounterID;
            var orderId = AddOpenOrder(order); // ---- Add orders
            var tabid = Convert.ToInt32(order.TabId);
            var orderBal = new OpenOrderBal(
                Helpers.DefaultCptTableNumber,
                Helpers.DefaultServiceCodeTableNumber,
                Helpers.DefaultDrgTableNumber,
                Helpers.DefaultDrugTableNumber,
                Helpers.DefaultHcPcsTableNumber,
                Helpers.DefaultDiagnosisTableNumber);


            //DB Call to get all records related to Orders tab
            var ordersViewData = orderBal.OrdersViewData(userId, 100, corporateId, facilityId, Convert.ToInt32(encounterId), "1024,3102,1011,2305,963", 0, "7", string.Empty);

            // Order Activities Section, starts here
            var orderActivities = ordersViewData.OrderActivities;
            var openActStatuses = new[] { 0, 1, 30, 20, 40 };
            var openOrderActivityList = orderActivities.Where(a => openActStatuses.Contains(Convert.ToInt32(a.OrderActivityStatus))).ToList();
            var closedOrderActivityList = orderActivities.Where(a => !openActStatuses.Contains(Convert.ToInt32(a.OrderActivityStatus))).ToList();
            // Order Activities Section, ends here

            //Orders Section
            var orders = ordersViewData.OpenOrders.ToList();
            var closedOrderStatuses = new[] { "2", "3", "4", "9" };
            var openOrderList = orders.Where(a => a.OrderStatus.Equals("1")).ToList();
            var closedOrdersList = orders.Where(a => closedOrderStatuses.Contains(a.OrderStatus)).ToList();
            //Orders Section
            var labWaitingSpecimenList = new List<OrderActivityCustomModel>();

            var newEncOrder = new OpenOrder
            {
                StartDate = Helpers.GetInvariantCultureDateTime(),
                EndDate = Helpers.GetInvariantCultureDateTime(),
                OrderStatus = Convert.ToString((int)OrderStatus.Open)
            };

            //GC Values 
            var gccvalues = ordersViewData.GlobalCodes;

            //json data to return to view
            var jsonData =
                new
                {
                    listFrequency = gccvalues.Where(g => g.GlobalCodeCategoryValue.Equals("1024")).OrderBy(d => d.GlobalCodeName).ToList(),
                    listOrderStatus = gccvalues.Where(g => g.GlobalCodeCategoryValue.Equals("3102")).OrderBy(d => d.GlobalCodeName).ToList(),
                    listQuantity = gccvalues.Where(g => g.GlobalCodeCategoryValue.Equals("1011")).OrderBy(m => Convert.ToDecimal(m.GlobalCodeValue)).ToList(),
                    listDocumentType = gccvalues.Where(g => g.GlobalCodeCategoryValue.Equals("2305")).ToList(),
                    listNoteType = gccvalues.Where(g => g.GlobalCodeCategoryValue.Equals("963")).ToList(),
                    FavoriteOrders = ordersViewData.FavoriteOrders.Select(x => new[] { Convert.ToString(x.UserDefinedDescriptionId), x.OrderTypeName, x.OrderCode, x.OrderDescription, x.UserDefinedDescription, string.Empty }),
                    MostRecentOrders = ordersViewData.MostRecentOrders.Select(x => new[] { Convert.ToString(x.OpenOrderID), x.OrderTypeName, x.OrderCode, x.OrderDescription, string.Empty }),
                    SearchOrders = ordersViewData.MostRecentOrders.Select(x => new[] { Convert.ToString(x.OpenOrderID), x.OrderTypeName, x.OrderCode, x.OrderDescription, x.CategoryName, x.SubCategoryName,
                        x.Status, x.FrequencyText, x.Quantity.HasValue ? Convert.ToString(x.Quantity.Value) : string.Empty, x.PeriodDays, x.OrderNotes, string.Empty }),
                    AllPhysicianOrders = ordersViewData.PreviousOrders.Select(x => new[] { x.OpenOrderID.ToString(), x.OrderTypeName, x.OrderCode, x.OrderDescription, string.Empty }),
                    ClosedOrdersList = closedOrdersList.Select(x => new[] {x.Quantity.HasValue?x.Quantity.Value.ToString():string.Empty,x.OrderCode,x.OrderDescription,x.FrequencyText,x.CategoryName,
                    x.SubCategoryName,x.PeriodDays,x.OrderNotes,x.Status}),
                    EncounterOrder = newEncOrder,
                    OpenOrdersList = openOrderList.Select(x => new[] { Convert.ToString(x.OpenOrderID), x.OrderTypeName, x.OrderCode, x.OrderDescription, x.CategoryName, x.SubCategoryName,
                        x.Status, x.FrequencyText, x.Quantity.HasValue ? Convert.ToString(x.Quantity.Value) : string.Empty, x.PeriodDays, x.OrderNotes, string.Empty }),
                    ClosedOrderActivityList = closedOrderActivityList.Select(x => new[] { x.OrderTypeName, x.OrderCode, x.OrderDescription, x.CategoryName, x.SubCategoryName, x.OrderScheduleDate.HasValue?x.OrderScheduleDate.Value.ToString("d"):string.Empty,
                    x.ExecutedDate.HasValue?x.ExecutedDate.Value.ToString("d"):string.Empty,x.OrderActivityQuantity.HasValue?Convert.ToString(x.OrderActivityQuantity.Value):string.Empty,
                    x.ExecutedQuantity.HasValue?Convert.ToString(x.ExecutedQuantity.Value):string.Empty,x.Comments}),
                    OpenOrderActivityList = openOrderActivityList.Select(x => new[] {x.Status, Convert.ToString(x.ShowEditAction), Convert.ToString(x.OrderActivityID), Convert.ToString(x.OrderCategoryID),
                                x.OrderTypeName, x.OrderCode, x.OrderDescription, x.CategoryName, x.SubCategoryName, x.OrderScheduleDate.HasValue ? x.OrderScheduleDate.Value.ToString("d") : string.Empty, string.Empty }),
                    FutureOpenOrdersList = ordersViewData.FutureOpenOrders.Select(x => new[] { x.OrderCode,x.OrderDescription,x.CategoryName,x.SubCategoryName ,x.Status, x.Quantity.HasValue ? Convert.ToString(x.Quantity.Value) : string.Empty,
                        x.StartDate.HasValue?x.StartDate.Value.ToString("d"):string.Empty,x.CreatedDate.HasValue?x.CreatedDate.Value.ToString("d"):string.Empty,x.FrequencyText,x.PeriodDays,x.OrderNotes}),
                    labWaitingSpecimenList = tabid == 0
                                         ? labWaitingSpecimenList
                                         : labWaitingSpecimenList.Where(x => x.OrderCategoryID == tabid).ToList()
                };

            //json data to return to view


            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;


        }
    }
}