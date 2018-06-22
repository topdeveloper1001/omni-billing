// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActiveEncounterController.cs" company="Spadez Solutions PVT. LTD.">
//   Servicesdotcom
// </copyright>
// <summary>
//   Defines the Active Encounter Controller type.
//   This Will hold the Features of the Screen Active Encounters
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;

namespace BillingSystem.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    using Bal.BusinessAccess;
    using Common;
    using Common.Common;
    using Model;
    using Model.CustomModel;
    using Models;

    using Kendo.Mvc.Extensions;
    using Kendo.Mvc.UI;

    /// <summary>
    /// The active encounter controller.
    /// </summary>
    public class ActiveEncounterController : BaseController
    {
        /// <summary>
        /// Actives the encounter.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public ActionResult ActiveEncounter(int? message)
        {
            if (message != null && (int)message > 0)
            {
                ViewBag.Message = GetMessage(Convert.ToInt32(message));
                ViewBag.MessageId = message;
            }

            using (var encounterBal = new EncounterBal(Helpers.DefaultCptTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDiagnosisTableNumber, Helpers.DefaultDrugTableNumber))
            {
                //var facilitId = Helpers.GetDefaultFacilityId();
                //var activeEncounter = new ActiveEncounter
                //{ // In Patients List Where EncounterPatientType is In Patient
                //    ActiveInPatientEncounterList =
                //        encounterBal.GetActiveEncounters(new CommonModel
                //        {
                //            EncounterPatientType = Convert.ToInt32(EncounterPatientType.InPatient),
                //            FacilityNumber = facilitId.ToString()
                //        }),

                //    // In Patients List Where EncounterPatientType is Out Patient
                //    ActiveOutPatientEncounterList =
                //        encounterBal.GetActiveEncounters(new CommonModel
                //        {
                //            EncounterPatientType = Convert.ToInt32(EncounterPatientType.OutPatient),
                //            FacilityNumber = facilitId.ToString()
                //        }),



                //    // In Patients List Where EncounterPatientType is ER Patient
                //    ActiveEmergencyEncounterList =
                //        encounterBal.GetActiveEncounters(new CommonModel
                //        {
                //            EncounterPatientType = Convert.ToInt32(EncounterPatientType.ERPatient),
                //            FacilityNumber = facilitId.ToString()
                //        })
                //};

                // ......Section is written for the Role Based Icon view
                //using (var bal = new RoleTabsBal())
                //{
                //    var roleId = Helpers.GetDefaultRoleId();
                //    activeEncounter.EncounterViewAccessible =
                //        (bal.CheckIfTabNameAccessibleToGivenRole("Admit Patient",
                //            ControllerAccess.PatientSearch.ToString(), ActionNameAccess.PatientSearch.ToString(),
                //            Convert.ToInt32(roleId)) &&
                //         bal.CheckIfTabNameAccessibleToGivenRole("Start Outpatient visit",
                //             ControllerAccess.PatientSearch.ToString(), ActionNameAccess.PatientSearch.ToString(),
                //             Convert.ToInt32(roleId)));
                //    activeEncounter.EndEncounterViewAccessible =
                //        (bal.CheckIfTabNameAccessibleToGivenRole("Discharge patient",
                //            ControllerAccess.PatientSearch.ToString(), ActionNameAccess.PatientSearch.ToString(),
                //            Convert.ToInt32(roleId)) &&
                //         bal.CheckIfTabNameAccessibleToGivenRole("Close Outpatient visit",
                //             ControllerAccess.PatientSearch.ToString(), ActionNameAccess.PatientSearch.ToString(),
                //             Convert.ToInt32(roleId)));



                //    activeEncounter.EhrViewAccessible = bal.CheckIfTabNameAccessibleToGivenRole("EHR", ControllerAccess.Summary.ToString(), ActionNameAccess.PatientSummary.ToString(), Convert.ToInt32(roleId));
                //    activeEncounter.TransactionsViewAccessible = bal.CheckIfTabNameAccessibleToGivenRole("Bed Transactions", ControllerAccess.PreliminaryBill.ToString(), ActionNameAccess.Index.ToString(), Convert.ToInt32(roleId));
                //    activeEncounter.DiagnosisViewAccessible = bal.CheckIfTabNameAccessibleToGivenRole("Code Bill", ControllerAccess.ActiveEncounter.ToString(), ActionNameAccess.ActiveEncounter.ToString(), Convert.ToInt32(roleId));
                //    activeEncounter.BillHeaderViewAccessible = bal.CheckIfTabNameAccessibleToGivenRole("Generate Preliminary Bill", ControllerAccess.BillHeader.ToString(), ActionNameAccess.Index.ToString(), Convert.ToInt32(roleId));
                //    activeEncounter.AuthorizationViewAccessible = bal.CheckIfTabNameAccessibleToGivenRole("Obtain Insurance Authorization", ControllerAccess.Authorization.ToString(), ActionNameAccess.AuthorizationMain.ToString(), Convert.ToInt32(roleId));

                //    activeEncounter.PatientInfoViewAccessible = bal.CheckIfTabNameAccessibleToGivenRole("Patient Lookup", ControllerAccess.PatientSearch.ToString(), ActionNameAccess.PatientSearch.ToString(), Convert.ToInt32(roleId));
                //}

                var allEncounters = encounterBal.GetAllActiveEncounters(
                    Convert.ToString(Helpers.GetDefaultFacilityId()), new List<int> { 1, 2, 3 });

                var m = new ActiveEncounter
                {
                    ActiveInPatientEncounterList =
                        allEncounters.Where(f => f.EncounterPatientType == (int)EncounterPatientType.InPatient)
                            .ToList(),
                    ActiveOutPatientEncounterList =
                        allEncounters.Where(f => f.EncounterPatientType == (int)EncounterPatientType.OutPatient)
                            .ToList(),
                    ActiveEmergencyEncounterList =
                        allEncounters.Where(f => f.EncounterPatientType == (int)EncounterPatientType.ERPatient)
                            .ToList(),
                };

                SetAccessOfActions(m);
                return View(m);
            }
        }

        /// <summary>
        /// Gets the encounter detail by identifier.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public ActionResult GetEncounterDetailById(string encounterId)
        {
            using (var encounterBal = new EncounterBal())
            {
                var encounterData = encounterBal.GetEncounterDetailById(Convert.ToInt32(encounterId));
                if (encounterData != null)
                {
                    var redirectUrl = Request.RawUrl + "?patientId=" + encounterData.PatientID + "&encounterId=" +
                                      encounterId;
                    return Json(redirectUrl);
                }
                return Json(null);
            }

        }

        /// <summary>
        /// Gets the chart data collection.
        /// </summary>
        /// <param name="DisplayType">The display type.</param>
        /// <param name="FromDate">From date.</param>
        /// <param name="ToDate">To date.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public ActionResult GetChartDataCollection(string DisplayType, string FromDate, string ToDate, [DataSourceRequest] DataSourceRequest request)
        {
            try
            {
                int facilityId = Helpers.GetDefaultFacilityId();
                var JsonResult = Json(GetChartData(facilityId, Convert.ToInt32(DisplayType), FromDate, ToDate).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
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
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="displayTypeId">The display type identifier.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">To date.</param>
        /// <returns></returns>
        public IEnumerable<EncounterExtension> GetChartData(int facilityId, int displayTypeId, string fromDate, string toDate)
        {
            using (var encounterBal = new EncounterBal())
            {
                IEnumerable<EncounterExtension> objData = encounterBal.GetEncounterChartData(facilityId, displayTypeId, new DateTime(DateTime.Today.Year, 1, 1), DateTime.Today);
                return objData;
            }
        }

        /// <summary>
        /// Gets the active encounters chart data.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetActiveEncountersChartData()
        {
            using (var encounterBal = new EncounterBal())
            {
                var facilityid = Helpers.GetDefaultFacilityId();
                var objData = encounterBal.GetActiveEncounterChartData(facilityid, 0, new DateTime(DateTime.Today.Year, 1, 1), DateTime.Today);
                return Json(objData, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Unclosed the encounters.
        /// </summary>
        /// <returns></returns>
        public ActionResult UnclosedEncounters()
        {
            return View();
        }
        public ActionResult GetUnclosedEncounters()
        {
            var facilityId = Helpers.GetDefaultFacilityId();
            var corporateId = Helpers.GetSysAdminCorporateID();
            using (var bal = new EncounterBal())
            {
                var list = bal.GetUnclosedEncounters(facilityId, corporateId);

                var jsonResult = new
                {
                    unclosedEncounters = list.OrderBy(x => x.ErrorStatus).Where(x => !x.ErrorStatus.Equals("E")).Select(f => new[] { Convert.ToString(f.EncounterID), f.PatientIsVIP, f.FirstName, f.LastName,
                        f.BirthDate.HasValue ? f.BirthDate.Value.ToString("d") : string.Empty, f.EncounterNumber, f.PersonEmiratesIDNumber,
                        f.EncounterStartTime.HasValue ? f.EncounterStartTime.Value.ToString("d") : string.Empty,
                        f.EncounterEndTime.HasValue ? f.EncounterEndTime.Value.ToString("d") : string.Empty, f.EncounterPatientTypeName,
                        f.ErrorStatus, Convert.ToString(f.PatientID) })
                };

                var s = Json(jsonResult, JsonRequestBehavior.AllowGet);
                s.MaxJsonLength = int.MaxValue;
                s.RecursionLimit = int.MaxValue;
                return s;
            }
        }

        /// <summary>
        /// Gets the open orders.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult GetOpenOrders(int encounterId, int patientId)
        {
            var orderBal = new OpenOrderBal(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber);
            var openOrderList = orderBal.GetPhysicianOrders(Convert.ToInt32(encounterId), OrderStatus.Open.ToString());
            var closedOrdersList = orderBal.GetPhysicianOrders(Convert.ToInt32(encounterId), OrderStatus.Closed.ToString());
            var ordersFullView = new OrdersFullView()
            {
                AllPhysicianOrders = new List<OpenOrderCustomModel>(),
                ClosedOrdersList = closedOrdersList,
                EncounterOrder = new OpenOrder() { StartDate = Helpers.GetInvariantCultureDateTime(), EndDate = Helpers.GetInvariantCultureDateTime(), OrderStatus = Convert.ToInt32(OrderStatus.Open).ToString() },
                FavoriteOrders = new List<OpenOrderCustomModel>(),
                MostRecentOrders = new List<OpenOrderCustomModel>(),
                OpenOrdersList = openOrderList,
                SearchedOrders = new List<OpenOrderCustomModel>(),
            };
            return PartialView(PartialViews.OrdersCustomView, ordersFullView);
        }
        public ActionResult BindOpenOrdersData(int encounterId, int patientId)
        {
            var openActStatuses = new[] { 0, 1, 30, 20, 40 };

            var orderBal = new OpenOrderBal(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber);
            //var openOrderList = orderBal.GetOrdersAndActivitiesByEncounter(Convert.ToInt32(encounterId));
            var openOrderList = orderBal.GetPhysicianOrders(Convert.ToInt32(encounterId), OrderStatus.Open.ToString());
            var jsonResult = new
            {
                OpenOrdersList = openOrderList.Select(x => new[] { Convert.ToString(x.OpenOrderID), x.OrderCode, x.OrderDescription, x.CategoryName,
                    x.SubCategoryName, x.Status, Convert.ToString(x.Quantity), x.FrequencyText, x.PeriodDays, x.OrderNotes, string.Empty })
            };

            var s = Json(jsonResult, JsonRequestBehavior.AllowGet);
            s.MaxJsonLength = int.MaxValue;
            s.RecursionLimit = int.MaxValue;
            return s;
        }
        public ActionResult GetOrderActivitiesData(int openOrderId)
        {
            var openActStatuses = new[] { 0, 1, 30, 20, 40 };

            var orderBal = new OpenOrderBal(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber);
            var orderActivities = orderBal.GetOrderActivitiesByOpenOrder(Convert.ToInt32(openOrderId));
            var jsonResult = new
            {
                orderActivities = orderActivities.Where(a => openActStatuses.Contains(Convert.ToInt32(a.OrderActivityStatus))).Select(x => new[] {
                    x.Status,
                    Convert.ToString(x.ShowEditAction),
                    Convert.ToString(x.OrderActivityID),
                    Convert.ToString(x.OrderCategoryID),
                    x.OrderTypeName,
                    x.OrderCode,
                    x.OrderDescription,
                    x.CategoryName,
                    x.SubCategoryName,
                    x.OrderScheduleDate.HasValue ? x.OrderScheduleDate.Value.ToString("d") : string.Empty,
                    string.Empty
                })
            };

            var s = Json(jsonResult, JsonRequestBehavior.AllowGet);
            s.MaxJsonLength = int.MaxValue;
            s.RecursionLimit = int.MaxValue;
            return s;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <returns></returns>
        private static string GetMessage(int messageId)
        {
            var message = string.Empty;
            switch (messageId)
            {
                case 1:
                    message = " Select Patient from the list to view the EHR Details";
                    break;
                case 2:
                    message = " Select Patient from the list to view the Encounter's Bed Transactions";
                    break;
                case 3:
                    message = " Select encounter from the list for Diagnosis!";
                    break;
                case 4:
                    message = " Select encounter from the list for Additional Diagnosis!";
                    break;
                case 9:
                    message = " Please select the Bill Header from the list to view the Scrub Report!";
                    break;
                case 10:
                    message = " Please Search for a Patient whom you want to Enter the Manual Payment for Bill Header!";
                    break;
                default:
                    break;
            }

            return message;
        }
        
        public ActionResult UpdateTriageInEncounter(int encounterId, string triageLevel)
        {
            var latestTriageLevel = string.Empty;
            using (var ebal = new EncounterBal())
                latestTriageLevel = ebal.UpdateTriageLevelInEncounter(encounterId, triageLevel);

            return Json(triageLevel);
        }
        
        public JsonResult GetTriageData(int encounterId)
        {
            var eBal = new EncounterBal();
            var triValue = eBal.GetTriageData(encounterId);
            using (var ebal = new GlobalCodeBal())
            {
                var list = new List<DropdownListData>();
                var facilityId = Helpers.GetDefaultFacilityId();
                var elist = ebal.GetGlobalCodesByCategoryValue("4952");
                if (elist.Count > 0)
                {
                    list.AddRange(elist.Select(item => new DropdownListData
                    {
                        Text = string.Format("{0}", item.GlobalCodeName),
                        Value = Convert.ToString(item.GlobalCodeValue)
                    }));
                }
                var jsonData = new
                {
                    list,
                    triValue
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }

        }
        
        public JsonResult GetPatientStageData(int encounterId)
        {
            var enbal = new EncounterBal();
            var stateValue = enbal.GetPatientStateData(encounterId);
            using (var ebal = new GlobalCodeBal())
            {
                var list = new List<DropdownListData>();
                var elist = ebal.GetGlobalCodesByCategoryValue("4951");
                if (elist.Count > 0)
                {
                    list.AddRange(elist.Select(item => new DropdownListData
                    {
                        Text = string.Format("{0}", item.GlobalCodeName),
                        Value = Convert.ToString(item.GlobalCodeValue),

                    }));

                }
                var jsonData = new
                {
                    list,
                    stateValue
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }

        }
        
        public ActionResult UpdatePatientStageInEncounter(int encounterId, string patientState)
        {
            var latestPatientStage = string.Empty;
            using (var ebal = new EncounterBal())
                latestPatientStage = ebal.UpdatePatitentStageInEncounter(encounterId, patientState);

            return Json(latestPatientStage);
        }

        public ActionResult GetActiveMotherDropdownData()
        {
            var list = new List<PatientInfoCustomModel>();
            var newList = new List<DropdownListData>();
            var corporateId = Helpers.GetSysAdminCorporateID();
            using (var pInfo = new PatientInfoBal())
            {
                using (var eBal = new EncounterBal())
                {
                    var mList = eBal.GetActiveMotherDropdownData(corporateId);

                    foreach (var listData in mList)
                    {
                        var list1 = pInfo.PatientDetailsByPatientIdForDropdown(Convert.ToInt32(listData.PatientID));
                        if (list1.PatientInfo != null)
                            list.Add(list1);
                    }

                    if (list.Count > 0)
                    {
                        newList.AddRange(list.Select(item => new DropdownListData
                        {
                            Text = string.Format("{0} - {1}", item.PatientInfo.PersonFirstName, item.PatientInfo.PersonLastName),
                            Value = item.PatientInfo.PatientID.ToString()
                        }));
                        return Json(newList);
                    }
                }
            }

            return Json(0);
        }


        public JsonResult UpdateEncounterEndCheck(int encounterId)
        {
            var userId = Helpers.GetLoggedInUserId();
            using (var bal = new EncounterBal())
            {
                int encounterEndStatus = bal.GetEncounterEndCheck(encounterId, userId);
                return Json(encounterEndStatus, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CheckEncounterEndStatus(int encounterid)
        {
            var userId = Helpers.GetLoggedInUserId();
            using (var encounterbal = new EncounterBal())
            {
                int encounterEndStatus = encounterbal.GetEncounterEndCheck(encounterid, userId);
                switch (encounterEndStatus)
                {
                    case 1:
                        return this.Json("Success");
                    case 99:
                        return this.Json("AuthError");

                }
                return this.Json("Error");
            }
        }


        public List<Tabs> AccessibleIconsList()
        {
            var tabs = new List<Tabs>
            {
                new Tabs
                {
                    Action = "patientsummary",
                    Controller = "summary",
                    TabName = "ehr",
                    TabId =  (int)ModulesAccessible.EhrView
                },
                new Tabs
                {
                    Action = "index",
                    Controller = "preliminarybill",
                    TabName = "bed transactions",
                    TabId =  (int)ModulesAccessible.TransactionsView
                },
                new Tabs
                {
                    Action = "activeencounter",
                    Controller = "activeencounter",
                    TabName = "code bill",
                    TabId =  (int)ModulesAccessible.DiagnosisView
                },
                new Tabs
                {
                    Action = "index",
                    Controller = "billheader",
                    TabName = "generate preliminary bill",
                    TabId =  (int)ModulesAccessible.BillHeaderView
                },
                new Tabs
                {
                    Action = "authorizationmain",
                    Controller = "authorization",
                    TabName = "obtain insurance authorization",
                    TabId =  (int)ModulesAccessible.AuthorizationView
                },
                new Tabs
                {
                    Action = "patientsearch",
                    Controller = "patientsearch",
                    TabName = "patient lookup",
                    TabId =  (int)ModulesAccessible.PatientInfoView
                },
                new Tabs
                {
                    Action = "patientsearch",
                    Controller = "patientsearch",
                    TabName = "admit patient",
                    TabId =  (int)ModulesAccessible.AdmitPatientView
                },
                new Tabs
                {
                    Action = "patientsearch",
                    Controller = "patientsearch",
                    TabName = "start outpatient visit",
                    TabId =  (int)ModulesAccessible.StartOpView
                },
                new Tabs
                {
                    Action = "patientsearch",
                    Controller = "patientsearch",
                    TabName = "discharge patient",
                    TabId =  (int)ModulesAccessible.DischargePatientView
                },
                new Tabs
                {
                    Action = "patientsearch",
                    Controller = "patientsearch",
                    TabName = "close outpatient visit",
                    TabId =  (int)ModulesAccessible.EndOpView
                }
            };

            return tabs;
        }

        public void SetAccessOfActions(ActiveEncounter model)
        {
            var roleId = Helpers.GetDefaultRoleId();
            using (var bal = new RoleTabsBal())
            {
                var result = bal.CheckIfTabsAccessibleToGivenRole(Convert.ToInt32(roleId), AccessibleIconsList());
                foreach (var item in result)
                {
                    var tabId = (ModulesAccessible)Enum.Parse(typeof(ModulesAccessible), Convert.ToString(item.TabId));
                    switch (tabId)
                    {
                        case ModulesAccessible.AuthorizationView:
                            model.AuthorizationViewAccessible = item.IsAccessible;
                            break;
                        case ModulesAccessible.BillHeaderView:
                            model.BillHeaderViewAccessible = item.IsAccessible;
                            break;
                        case ModulesAccessible.DiagnosisView:
                            model.DiagnosisViewAccessible = item.IsAccessible;
                            break;
                        case ModulesAccessible.EhrView:
                            model.EhrViewAccessible = item.IsAccessible;
                            break;
                        case ModulesAccessible.PatientInfoView:
                            model.PatientInfoViewAccessible = item.IsAccessible;
                            break;
                        case ModulesAccessible.TransactionsView:
                            model.TransactionsViewAccessible = item.IsAccessible;
                            break;
                        case ModulesAccessible.AdmitPatientView:
                        case ModulesAccessible.StartOpView:
                            model.EncounterViewAccessible = item.IsAccessible;
                            break;
                        case ModulesAccessible.EndOpView:
                        case ModulesAccessible.DischargePatientView:
                            model.EndEncounterViewAccessible = item.IsAccessible;
                            break;
                    }
                }
            }
        }


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
                var result = bal.GetOrderActivityByIDVM(OpenOrderActivityId);
                return PartialView(PartialViews.AdministerOrdersinEncounter, result);
            }
        }
    }
}
