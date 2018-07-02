using System.Reflection;
using BillingSystem.Models;
using BillingSystem.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class XclaimController : BaseController
    {
        private readonly IXclaimService _service;  
        private readonly IXPaymentFileXMLService _pfService;

        public XclaimController(IXclaimService service, IXPaymentFileXMLService pfService)
        {
            _service = service;
            _pfService = pfService;
        }

        /// <summary>
        /// Get the details of the Xclaim View in the Model Xclaim such as XclaimList, list of countries etc.
        /// </summary>
        /// <param name="claimid">The claimid.</param>
        /// <param name="encid">The encid.</param>
        /// <param name="Pid">The pid.</param>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model Xclaim to be passed to View Xclaim
        /// </returns>
        public ActionResult XclaimMain(int? claimid, int? encid, int? Pid)
        {
            var facilityid = Helpers.GetDefaultFacilityId();
            //Get the Entity list
            var xclaimList = _service.GetXclaim(facilityid.ToString());

            //Intialize the View Model i.e. XclaimView which is binded to Main View Index.cshtml under Xclaim
            var xclaimView = new XclaimView
            {
                XclaimList = xclaimList,
                CurrentXclaim = new XClaim(),
                EncounterId = encid ?? 0,
                ClaimId = claimid ?? 0,
                PatientId = Pid ?? 0
            };

            PropertyInfo Isreadonly = typeof(System.Collections.Specialized.NameValueCollection).GetProperty("IsReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);

            Isreadonly.SetValue(Request.QueryString, false, null);

            Request.QueryString.Clear();
            //Pass the View Model in ActionResult to View Xclaim
            return View(xclaimView);
        }

        /// <summary>
        /// Bind all the Xclaim list 
        /// </summary>
        /// <returns>action result with the partial view containing the Xclaim list object</returns>
        [HttpPost]
        public ActionResult BindXclaimList()
        {
            var facilityid = Helpers.GetDefaultFacilityId();
            //Get the facilities list
            var xclaimList = _service.GetXclaim(facilityid.ToString());

            //Pass the ActionResult with List of XclaimViewModel object to Partial View XclaimList
            return PartialView(PartialViews.XclaimList, xclaimList);
        }

        /// <summary>
        /// Add New or Update the Xclaim based on if we pass the Xclaim ID in the XclaimViewModel object.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// returns the newly added or updated ID of Xclaim row
        /// </returns>
        public ActionResult SaveXclaim(XClaim model)
        {
            //Initialize the newId variable 
            var newId = -1;
            var userId = Helpers.GetLoggedInUserId();

            //Check if Model is not null 
            if (model != null)
            {
                if (model.ClaimID > 0)
                {
                    model.ModifiedBy = userId.ToString();
                    model.ModifiedDate = CurrentDateTime;
                }
                //Call the AddXclaim Method to Add / Update current Xclaim
                newId = _service.SaveXclaim(model);
            }
            return Json(newId);
        }

        /// <summary>
        /// Get the details of the current Xclaim in the view model by ID
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult GetXclaim(int id)
        {
            //Call the AddXclaim Method to Add / Update current Xclaim
            var currentXclaim = _service.GetXclaimByID(id);

            //Pass the ActionResult with the current XclaimViewModel object as model to PartialView XclaimAddEdit
            return PartialView(PartialViews.XclaimAddEdit, currentXclaim);
        }

        /// <summary>
        /// Delete the current Xclaim based on the Xclaim ID passed in the XclaimModel
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteXclaim(int id)
        {
            //Get Xclaim model object by current Xclaim ID
            var currentXclaim = _service.GetXclaimByID(id);
            var userId = Helpers.GetLoggedInUserId();

            //Check If Xclaim model is not null
            if (currentXclaim != null)
            {
                currentXclaim.ModifiedBy = userId.ToString();
                currentXclaim.ModifiedDate = CurrentDateTime;

                //Update Operation of current Xclaim
                var result = _service.SaveXclaim(currentXclaim);

                //return deleted ID of current Xclaim as Json Result to the Ajax Call.
                return Json(result);
            }

            //Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }

        /// <summary>
        /// Reset the Xclaim View Model and pass it to XclaimAddEdit Partial View.
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetXclaimForm()
        {
            //Intialize the new object of Xclaim ViewModel
            var xclaimViewModel = new Model.XClaim();
            //Pass the View Model as XclaimViewModel to PartialView XclaimAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.XclaimAddEdit, xclaimViewModel);
        }

        /// <summary>
        /// Gets the x claim list.
        /// </summary>
        /// <param name="pid">The pid.</param>
        /// <param name="eid">The eid.</param>
        /// <param name="claimid">The claimid.</param>
        /// <returns></returns>
        public ActionResult GetXClaimList(string pid, Int64 eid, Int64 claimid)
        {
            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId().ToString();
            var claimlist = corporateId == 6
                ? _service.GetXclaimByParameters(pid, eid, claimid)
                : _service.GetXclaimByFacilityParameters(facilityId, pid, eid, claimid);
            return PartialView(PartialViews.XclaimList, claimlist);
        }


        /// <summary>
        /// Gets the claims by encounter identifier.
        /// </summary>
        /// <param name="encounterid">The encounterid.</param>
        /// <returns></returns>
        public ActionResult GetClaimsByEncounterId(Int64 encounterid)
        {
            var list = new List<DropdownListData>();
            //Call the AddXclaim Method to Add / Update current Xclaim
            var xclaimList = _service.GetXclaimByEncounterId(encounterid);
            if (xclaimList.Any())
            {
                list.AddRange(xclaimList.Select(item => new DropdownListData
                {
                    Text = item.ClaimID.ToString(),
                    Value = Convert.ToString(item.ClaimID),
                }));
            }
            return Json(list);
        }

        /// <summary>
        /// Applies the charges.
        /// </summary>
        /// <returns></returns>
        public ActionResult ApplyCharges()
        {
                var corporateid = Helpers.GetSysAdminCorporateID();
                var facilityid = Helpers.GetDefaultFacilityId();
                var xclaimList = _service.ApplyAdvicePayment(corporateid, facilityid);
                return Json(xclaimList);
        }


        /// <summary>
        /// Views the file.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public string ViewFile(int id)
        {
            var xPaymentFileXmlfirstObj = _pfService.GetFirstXPaymentFileXML(id);
            return xPaymentFileXmlfirstObj;
        }

        /// <summary>
        /// Applies the chargeson file.
        /// </summary>
        /// <param name="fileid">The fileid.</param>
        /// <returns></returns>
        public ActionResult ApplyChargesonFile(int fileid)
        {
                var corporateid = Helpers.GetSysAdminCorporateID();
                var facilityid = Helpers.GetDefaultFacilityId();
                //var xclaimList = _service.ApplyAdvicePayment(corporateid, facilityid);
                var xclaimList = _service.ApplyAdvicePaymentInRemittanceAdvice(corporateid, facilityid, fileid);
                return Json(xclaimList);
        }

    }
}