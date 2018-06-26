using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Models;

namespace BillingSystem.Controllers
{
    using BillingSystem.Bal.Interfaces;
    using System.Linq;

    public class PatientSearchController : BaseController
    {
        private readonly IPatientInfoService _piService;

        public PatientSearchController(IPatientInfoService piService)
        {
            _piService = piService;
        }

        #region Patient search main
        /// <summary>
        /// Patients the search.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <returns></returns>
        public ActionResult PatientSearch(string messageId)
        {
            if (!string.IsNullOrEmpty(messageId))
            {
                ViewBag.Message = GetMessage(Convert.ToInt32(messageId));
                ViewBag.MessageId = Convert.ToInt32(messageId);
            }
            var session = Session[SessionNames.SessionClass.ToString()] as SessionClass;
            ViewBag.Role = session == null ? string.Empty : session.RoleId.ToString();//for view bag role session value
            ViewBag.FirstTimeLogin = session == null || session.FirstTimeLogin;
            var patientSearchView = new PatientSearchView { PatientSearchList = new List<PatientInfoCustomModel>() };
            return View(patientSearchView);
        }

        /// <summary>
        /// Gets the patient information search result.
        /// </summary>
        /// <param name="common">The common.</param>
        /// <returns></returns>
        public ActionResult GetPatientInfoSearchResult(CommonModel common)
        {
            var facilityid = Helpers.GetDefaultFacilityId();
            var corporateid = Helpers.GetSysAdminCorporateID();
            common.FacilityId = facilityid;
            common.CorporateId = corporateid;
            common.UserId = Helpers.GetLoggedInUserId();
            common.RoleKey = Helpers.GetDefaultRoleId();
            common.ShowAccessedTabs = true;
            ViewBag.Message = null;

            var list = new List<PatientInfoCustomModel>();

            list = _piService.GetPatientSearchResultAndOtherData(common);

            if (list != null && list.Any())
            {
                var first = list[0];
                if (Session[SessionNames.SessionClass.ToString()] is SessionClass objSession)
                    first.SchedularViewAccessible = objSession.SchedularAccessible;
            }


            return PartialView(PartialViews.PatientSearchList, list);
        }

        /// <summary>
        /// Gets the patient information search custom result.
        /// </summary>
        /// <param name="Ln">The ln.</param>
        /// <param name="EID">The eid.</param>
        /// <param name="PassNo">The pass no.</param>
        /// <param name="BD">The bd.</param>
        /// <param name="MobileNo">The mobile no.</param>
        /// <returns></returns>
        public ActionResult GetPatientInfoSearchCustomResult(string Ln, string EID, string PassNo, string BD, string MobileNo)
        {
            var facilityIdAssignedTologgedinUser = 0;
            var common = new CommonModel()
            {
                PersonLastName = Ln,
                PersonEmiratesIDNumber = EID,
                PersonPassportNumber = PassNo,
                PersonBirthDate =
                    string.IsNullOrEmpty(BD) ? Helpers.GetInvariantCultureDateTime() : Convert.ToDateTime(BD),
                ContactMobilePhone = MobileNo
            };
            if (Session[SessionNames.SessionClass.ToString()] != null)
            {
                var session = Session[SessionNames.SessionClass.ToString()] as SessionClass;
                facilityIdAssignedTologgedinUser = session.FacilityId;
                common.FacilityId = facilityIdAssignedTologgedinUser;
                common.CorporateId = session.CorporateId;
            }

            var objPatientInfoData = _piService.GetPatientSearchResult(common);
            ViewBag.Message = null;

            if (objPatientInfoData.Count > 0)
            {
                using (var rolebal = new RoleTabsBal())
                {
                    var roleId = Helpers.GetDefaultRoleId();
                    objPatientInfoData[0].EhrViewAccessible = rolebal.CheckIfTabNameAccessibleToGivenRole("EHR", ControllerAccess.Summary.ToString(), ActionNameAccess.PatientSummary.ToString(), Convert.ToInt32(roleId));
                    //objPatientInfoData[0].TransactionsViewAccessible = rolebal.CheckIfTabNameAccessibleToGivenRole(ControllerAccess.PreliminaryBill.ToString(), Convert.ToInt32(roleId)); ; ;
                    objPatientInfoData[0].AuthorizationViewAccessible = rolebal.CheckIfTabNameAccessibleToGivenRole("Obtain Insurance Authorization", ControllerAccess.Authorization.ToString(), ActionNameAccess.PatientSummary.ToString(), Convert.ToInt32(roleId));
                    objPatientInfoData[0].BillHeaderViewAccessible =
                        rolebal.CheckIfTabNameAccessibleToGivenRole("Generate Preliminary Bill",
                            ControllerAccess.BillHeader.ToString(), ActionNameAccess.Index.ToString(),
                            Convert.ToInt32(roleId));
                }
            }

            return PartialView(PartialViews.PatientSearchList, objPatientInfoData);
        }

        /// <summary>
        /// Edits the patient.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="viewOnly">The view only.</param>
        public void EditPatient(int id, string viewOnly)
        {
            Session["PatientId"] = id;
            Session["ViewOnly"] = viewOnly;
        }
        #endregion

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <returns></returns>
        private string GetMessage(int messageId)
        {
            var message = string.Empty;

            var messageType = (MessageType)Enum.Parse(typeof(MessageType), messageId.ToString());
            switch (messageType)
            {
                case MessageType.AdmitPatient:
                    message = " Please Search for a Patient that you want to admit";
                    break;
                case MessageType.StartOutPatient:
                    message = " Please Search for a Patient whom you want to start an Encounter";
                    break;
                case MessageType.Discharge:
                    message = " Please Search for a Patient that you want to discharge";
                    break;
                case MessageType.EndEncounter:
                    message = " Please Search for a Patient whom you want to end an Encounter";
                    break;
                case MessageType.EncounterDetails:
                    message = " Please Select the Encounter for a Patient to see the details.";
                    break;
                case MessageType.ViewEHR:
                    message = " Please Search for a Patient whom you want to view the EHR Details!";
                    break;
                case MessageType.ViewAuthorization:
                    message = " Please Search for a Patient whom you want to view the Authorization Details!";
                    break;
                case MessageType.ViewBillingHeader:
                    message = " Please Search for a Patient whom you want to view the Billing Details!";
                    break;
                case MessageType.ViewScrubReport:
                    message = " Please select the Bill Header from the list to view the Scrub Report!";
                    break;
                case MessageType.EnterManualPayment:
                    message = " Please Search for a Patient whom you want to Enter the Manual Payment for Bill Header!";
                    break;
                case MessageType.PatientSchedular:
                    message = " Please Search for a Patient for Preliminary Appointments!";
                    break;
                case MessageType.Other:
                    message = " Please Search for a Patient whom you want to see the Details!";
                    break;
                default:
                    break;
            }
            return message;
        }

        #region Patient search Custom
        /// <summary>
        /// Patients the search.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <returns></returns>
        public ActionResult PatientSearch1(string messageId)
        {
            if (!string.IsNullOrEmpty(messageId))
            {
                ViewBag.Message = GetMessage1(Convert.ToInt32(messageId));
                ViewBag.MessageId = Convert.ToInt32(messageId);
            }
            var session = Session[SessionNames.SessionClass.ToString()] as SessionClass;
            ViewBag.Role = session == null ? string.Empty : session.RoleId.ToString();//for view bag role session value
            ViewBag.FirstTimeLogin = session == null || session.FirstTimeLogin;
            var patientSearchView = new PatientSearchView { PatientSearchList = new List<PatientInfoCustomModel>() };
            return View(patientSearchView);
        }

        /// <summary>
        /// Gets the patient information search result.
        /// </summary>
        /// <param name="common">The common.</param>
        /// <returns></returns>
        public ActionResult GetPatientInfoSearchResult1(CommonModel common)
        {
            var facilityid = Helpers.GetDefaultFacilityId();
            var corporateid = Helpers.GetSysAdminCorporateID();
            common.FacilityId = facilityid;
            common.CorporateId = corporateid;

            var objPatientInfoData = _piService.GetPatientSearchResultByCId(common);
            ViewBag.Message = null;

            if (objPatientInfoData.Count > 0)
            {
                using (var rolebal = new RoleTabsBal())
                {
                    var roleId = Helpers.GetDefaultRoleId();
                    objPatientInfoData[0].PatientInfoAccessible = rolebal.CheckIfTabNameAccessibleToGivenRole("Register new patient", ControllerAccess.PatientInfo.ToString(), ActionNameAccess.RegisterPatient.ToString(), Convert.ToInt32(roleId));
                    objPatientInfoData[0].EhrViewAccessible = rolebal.CheckIfTabNameAccessibleToGivenRole("EHR", ControllerAccess.Summary.ToString(), ActionNameAccess.PatientSummary.ToString(), Convert.ToInt32(roleId));
                    objPatientInfoData[0].AuthorizationViewAccessible = rolebal.CheckIfTabNameAccessibleToGivenRole("Obtain Insurance Authorization", ControllerAccess.Authorization.ToString(), ActionNameAccess.AuthorizationMain.ToString(), Convert.ToInt32(roleId));
                    objPatientInfoData[0].BillHeaderViewAccessible =
                        rolebal.CheckIfTabNameAccessibleToGivenRole("Generate Preliminary Bill",
                            ControllerAccess.BillHeader.ToString(), ActionNameAccess.Index.ToString(),
                            Convert.ToInt32(roleId));

                    var objSession = Session[SessionNames.SessionClass.ToString()] as SessionClass;
                    if (objSession != null)
                    {
                        objPatientInfoData[0].SchedularViewAccessible = objSession.SchedularAccessible;
                    }
                }
            }
            var facilityPatients = RenderPartialViewToStringBase(PartialViews.PatientSearchList, objPatientInfoData.Where(x => x.PatientInfo.FacilityId == facilityid).ToList());
            var otherfacilityPatients = RenderPartialViewToStringBase(PartialViews.PatientSearchListCustom, objPatientInfoData.Where(x => x.PatientInfo.FacilityId != facilityid).ToList());
            //return PartialView(PartialViews.PatientSearchList, objPatientInfoData);
            var jsonToReturn =
                Json(new { facilityPatients = facilityPatients, otherfacilityPatients = otherfacilityPatients });
            jsonToReturn.MaxJsonLength = int.MaxValue;
            jsonToReturn.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonToReturn;
        }

        /// <summary>
        /// Gets the patient information search custom result.
        /// </summary>
        /// <param name="Ln">The ln.</param>
        /// <param name="EID">The eid.</param>
        /// <param name="PassNo">The pass no.</param>
        /// <param name="BD">The bd.</param>
        /// <param name="MobileNo">The mobile no.</param>
        /// <returns></returns>
        public ActionResult GetPatientInfoSearchCustomResult1(string Ln, string EID, string PassNo, string BD, string MobileNo)
        {
            var facilityIdAssignedTologgedinUser = 0;
            var common = new CommonModel()
            {
                PersonLastName = Ln,
                PersonEmiratesIDNumber = EID,
                PersonPassportNumber = PassNo,
                PersonBirthDate =
                    string.IsNullOrEmpty(BD) ? Helpers.GetInvariantCultureDateTime() : Convert.ToDateTime(BD),
                ContactMobilePhone = MobileNo
            };
            if (Session[SessionNames.SessionClass.ToString()] != null)
            {
                var session = Session[SessionNames.SessionClass.ToString()] as SessionClass;
                facilityIdAssignedTologgedinUser = session.FacilityId;
                common.FacilityId = facilityIdAssignedTologgedinUser;
                common.CorporateId = session.CorporateId;
            }

            var objPatientInfoData = _piService.GetPatientSearchResult(common);
            ViewBag.Message = null;

            if (objPatientInfoData.Count > 0)
            {
                using (var rolebal = new RoleTabsBal())
                {
                    var roleId = Helpers.GetDefaultRoleId();
                    objPatientInfoData[0].EhrViewAccessible = rolebal.CheckIfTabNameAccessibleToGivenRole("EHR", ControllerAccess.Summary.ToString(), ActionNameAccess.PatientSummary.ToString(), Convert.ToInt32(roleId));
                    //objPatientInfoData[0].TransactionsViewAccessible = rolebal.CheckIfTabNameAccessibleToGivenRole(ControllerAccess.PreliminaryBill.ToString(), Convert.ToInt32(roleId)); ; ;
                    objPatientInfoData[0].AuthorizationViewAccessible = rolebal.CheckIfTabNameAccessibleToGivenRole("Obtain Insurance Authorization", ControllerAccess.Authorization.ToString(), ActionNameAccess.PatientSummary.ToString(), Convert.ToInt32(roleId));
                    objPatientInfoData[0].BillHeaderViewAccessible =
                        rolebal.CheckIfTabNameAccessibleToGivenRole("Generate Preliminary Bill",
                            ControllerAccess.BillHeader.ToString(), ActionNameAccess.Index.ToString(),
                            Convert.ToInt32(roleId));
                }
            }

            return PartialView(PartialViews.PatientSearchList, objPatientInfoData);
        }

        /// <summary>
        /// Edits the patient.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="viewOnly">The view only.</param>
        public void EditPatient1(int id, string viewOnly)
        {
            Session["PatientId"] = id;
            Session["ViewOnly"] = viewOnly;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <returns></returns>
        private string GetMessage1(int messageId)
        {
            var message = string.Empty;

            var messageType = (MessageType)Enum.Parse(typeof(MessageType), messageId.ToString());
            switch (messageType)
            {
                case MessageType.AdmitPatient:
                    message = " Please Search for a Patient that you want to admit";
                    break;
                case MessageType.StartOutPatient:
                    message = " Please Search for a Patient whom you want to start an Encounter";
                    break;
                case MessageType.Discharge:
                    message = " Please Search for a Patient that you want to discharge";
                    break;
                case MessageType.EndEncounter:
                    message = " Please Search for a Patient whom you want to end an Encounter";
                    break;
                case MessageType.EncounterDetails:
                    message = " Please Select the Encounter for a Patient to see the details.";
                    break;
                case MessageType.ViewEHR:
                    message = " Please Search for a Patient whom you want to view the EHR Details!";
                    break;
                case MessageType.ViewAuthorization:
                    message = " Please Search for a Patient whom you want to view the Authorization Details!";
                    break;
                case MessageType.ViewBillingHeader:
                    message = " Please Search for a Patient whom you want to view the Billing Details!";
                    break;
                case MessageType.ViewScrubReport:
                    message = " Please select the Bill Header from the list to view the Scrub Report!";
                    break;
                case MessageType.EnterManualPayment:
                    message = " Please Search for a Patient whom you want to Enter the Manual Payment for Bill Header!";
                    break;
                case MessageType.PatientSchedular:
                    message = " Please Search for a Patient for Preliminary Appointments!";
                    break;
                case MessageType.Other:
                    message = " Please Search for a Patient whom you want to see the Details!";
                    break;
                default:
                    break;
            }
            return message;
        }

        #endregion
    }
}
