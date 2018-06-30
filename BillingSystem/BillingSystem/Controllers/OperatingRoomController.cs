using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI.WebControls.WebParts;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common;
using BillingSystem.Common.Common;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;
using BillingSystem.Models;
using System;

namespace BillingSystem.Controllers
{
    public class OperatingRoomController : BaseController
    {
        /// <summary>
        /// Indexes the specified patient identifier.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public ActionResult Index(string patientId, string encounterId)
        {
            if (string.IsNullOrEmpty(patientId) || string.IsNullOrEmpty(encounterId))
                return RedirectToAction("PatientSearch", "PatientSearch", new { messageId = string.Empty });

            var vmData = new OperatingRoomView
            {
                OperatingRoom =
                            new OperatingRoom
                            {
                                IsDeleted = false,
                                OperatingType = (int)OperatingRoomTypes.Surgery,
                                EncounterId = Convert.ToInt32(encounterId),
                                PatientId = Convert.ToInt32(patientId),
                                CodeValueType = "8"
                            },
                AnesthesiaTime =
                    new OperatingRoom
                    {
                        IsDeleted = false,
                        OperatingType = (int)OperatingRoomTypes.Anesthesia,
                        EncounterId = Convert.ToInt32(encounterId),
                        PatientId = Convert.ToInt32(patientId),
                        CodeValueType = "3"
                    }
            };
            return View(vmData);
        }

        /// <summary>
        /// Saves the operating room data.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <returns></returns>
        public ActionResult SaveOperatingRoomData(OperatingRoomView vm)
        {
            List<OperatingRoomCustomModel> list;
            using (var bal = new OperatingRoomService(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber))
            {
                var model = vm.OperatingRoom != null && Convert.ToDecimal(vm.OperatingRoom.CalculatedHours) > 0
                    ? vm.OperatingRoom
                    : vm.AnesthesiaTime;
                var id = model.Id;
                var userId = Helpers.GetLoggedInUserId();
                var facilityId = Helpers.GetDefaultFacilityId();
                var corpoaretId = Helpers.GetSysAdminCorporateID();
                var currentDateTime = Helpers.GetInvariantCultureDateTime();
                model.FacilityId = facilityId;
                model.CorporateID = corpoaretId;
                model.Status = "1";
                if (id > 0)
                {
                    model.ModifiedBy = userId;
                    model.Modifieddate = currentDateTime;
                    if (model.IsDeleted)
                    {
                        model.DeletedBy = userId;
                        model.DeletedDate = currentDateTime;
                    }
                }
                else
                {
                    model.CreatedBy = userId;
                    model.CreatedDate = currentDateTime;
                }
                list = bal.SaveOperatingRoomData(model);
                list = bal.GetOperatingRoomsList(Convert.ToInt32(model.OperatingType),
                Convert.ToInt32(model.EncounterId),
                Convert.ToInt32(model.PatientId));
            }
            return PartialView(PartialViews.OperatingRoomsList, list);
        }

        /// <summary>
        /// Gets the operating room list.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public ActionResult GetOperatingRoomList(int type, int encounterId, int patientId)
        {
            using (var bal = new OperatingRoomService(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber))
            {
                var list = bal.GetOperatingRoomsList(type, encounterId, patientId);
                return PartialView(PartialViews.OperatingRoomsList, list);
            }
        }

        /// <summary>
        /// Gets the operating room details.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public JsonResult GetOperatingRoomDetails(int id)
        {
            using (var bal = new OperatingRoomService(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber))
            {
                var result = bal.GetOperatingRoomDetail(id);
                if (result != null)
                {
                    var jsonResult = new
                    {
                        result.Id,
                        result.OperatingType,
                        StartDay = result.StartDay.GetShortDateString1(),
                        EndDay = result.EndDay.GetShortDateString1(),
                        result.StartTime,
                        result.EndTime,
                        result.CalculatedHours,
                        result.PatientId,
                        result.EncounterId,
                        result.CodeValue,
                        result.CodeValueType
                    };
                    return Json(jsonResult, JsonRequestBehavior.AllowGet);
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Deletes the operating room data.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteOperatingRoomData(int id)
        {
            List<OperatingRoomCustomModel> list;
            using (var bal = new OperatingRoomService(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber))
            {
                var model = bal.GetOperatingRoomDetail(id);
                var userId = Helpers.GetLoggedInUserId();
                var currentDateTime = Helpers.GetInvariantCultureDateTime();
                if (id > 0)
                {
                    model.ModifiedBy = userId;
                    model.Modifieddate = currentDateTime;
                    model.IsDeleted = true;
                    model.DeletedBy = userId;
                    model.DeletedDate = currentDateTime;
                }
                list = bal.SaveOperatingRoomData(model);
            }
            return PartialView(PartialViews.OperatingRoomsList, list);
        }

        /// <summary>
        /// Checks the duplicate record.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public JsonResult CheckDuplicateRecord(OperatingRoom model)
        {
            using (var bal = new OperatingRoomService(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber))
            {
                var result = bal.CheckDuplicateRecord(model);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Loads the surgery section.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public ActionResult LoadSurgerySection(int patientId, int encounterId)
        {
            using (var bal = new OperatingRoomService(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber))
            {
                var list1 = bal.GetOperatingRoomsList((int)OperatingRoomTypes.Surgery, encounterId, patientId);
                var list2 = bal.GetOperatingRoomsList((int)OperatingRoomTypes.Anesthesia, encounterId, patientId);
                var vmData = new OperatingRoomView
                    {
                        OperatingRoom =
                            new OperatingRoom
                            {
                                IsDeleted = false,
                                OperatingType = (int)OperatingRoomTypes.Surgery,
                                EncounterId = encounterId,
                                PatientId = patientId,
                                CodeValueType = "8"
                            },
                        AnesthesiaTime =
                            new OperatingRoom
                            {
                                IsDeleted = false,
                                OperatingType = (int)OperatingRoomTypes.Anesthesia,
                                EncounterId = encounterId,
                                PatientId = patientId,
                                CodeValueType = "3"
                            },
                        OperatingRoomsList = list1,
                        AnesthesiaTimesList = list2
                    };
                return PartialView("~/Views/OperatingRoom/Index.cshtml", vmData);
            }
        }

        /// <summary>
        /// Sorts the operating room data.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public ActionResult SortOperatingRoomData(int encounterId,int patientId)
        {
           using (var bal = new OperatingRoomService(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber,Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber,Helpers.DefaultDiagnosisTableNumber))
            {
              
                List<OperatingRoomCustomModel> list;
                list = bal.GetOperatingRoomsList(1, encounterId, patientId);
                return PartialView(PartialViews.OperatingRoomsList, list);
            }
           
        }

        /// <summary>
        /// Sorts the anastsia room data.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public ActionResult SortAnastsiaRoomData(int encounterId, int patientId)
        {
            using (var bal = new OperatingRoomService(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber))
            {
                List<OperatingRoomCustomModel> list;
                list = bal.GetOperatingRoomsList(2, encounterId, patientId);
                return PartialView(PartialViews.AnasthesiaList, list);
            }

        }
    }
}