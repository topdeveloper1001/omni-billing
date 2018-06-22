using System.Reflection;
using BillingSystem.Models;
using BillingSystem.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Controllers
{
    using System.IO;

    public class XclaimController : BaseController
    {
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
            //Initialize the Xclaim BAL object
            var xclaimBal = new XclaimBal();
            var facilityid = Helpers.GetDefaultFacilityId();
            //Get the Entity list
            var xclaimList = xclaimBal.GetXclaim(facilityid.ToString());

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
            //Initialize the Xclaim BAL object
            using (var xclaimBal = new XclaimBal())
            {
                var facilityid = Helpers.GetDefaultFacilityId();
                //Get the facilities list
                var xclaimList = xclaimBal.GetXclaim(facilityid.ToString());

                //Pass the ActionResult with List of XclaimViewModel object to Partial View XclaimList
                return PartialView(PartialViews.XclaimList, xclaimList);
            }
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
                using (var bal = new XclaimBal())
                {
                    if (model.ClaimID > 0)
                    {
                        model.ModifiedBy = userId.ToString();
                        model.ModifiedDate = CurrentDateTime;
                    }
                    //Call the AddXclaim Method to Add / Update current Xclaim
                    newId = bal.SaveXclaim(model);
                }
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
            using (var bal = new XclaimBal())
            {
                //Call the AddXclaim Method to Add / Update current Xclaim
                var currentXclaim = bal.GetXclaimByID(id);

                //Pass the ActionResult with the current XclaimViewModel object as model to PartialView XclaimAddEdit
                return PartialView(PartialViews.XclaimAddEdit, currentXclaim);
            }
        }

        /// <summary>
        /// Delete the current Xclaim based on the Xclaim ID passed in the XclaimModel
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteXclaim(int id)
        {
            using (var bal = new XclaimBal())
            {
                //Get Xclaim model object by current Xclaim ID
                var currentXclaim = bal.GetXclaimByID(id);
                var userId = Helpers.GetLoggedInUserId();

                //Check If Xclaim model is not null
                if (currentXclaim != null)
                {
                    currentXclaim.ModifiedBy = userId.ToString();
                    currentXclaim.ModifiedDate = CurrentDateTime;

                    //Update Operation of current Xclaim
                    var result = bal.SaveXclaim(currentXclaim);

                    //return deleted ID of current Xclaim as Json Result to the Ajax Call.
                    return Json(result);
                }
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
            using (var bal = new XclaimBal())
            {
                var facilityId = Helpers.GetDefaultFacilityId().ToString();
                var claimlist = corporateId == 6
                    ? bal.GetXclaimByParameters(pid, eid, claimid)
                    : bal.GetXclaimByFacilityParameters(facilityId, pid, eid, claimid);
                return PartialView(PartialViews.XclaimList, claimlist);
            }

        }

        /// <summary>
        /// Gets the claims by encounter identifier.
        /// </summary>
        /// <param name="encounterid">The encounterid.</param>
        /// <returns></returns>
        public ActionResult GetClaimsByEncounterId(Int64 encounterid)
        {
            using (var bal = new XclaimBal())
            {
                var list = new List<DropdownListData>();
                //Call the AddXclaim Method to Add / Update current Xclaim
                var xclaimList = bal.GetXclaimByEncounterId(encounterid);
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
        }

        /// <summary>
        /// Applies the charges.
        /// </summary>
        /// <returns></returns>
        public ActionResult ApplyCharges()
        {
            using (var bal = new XclaimBal())
            {
                var corporateid = Helpers.GetSysAdminCorporateID();
                var facilityid = Helpers.GetDefaultFacilityId();
                var xclaimList = bal.ApplyAdvicePayment(corporateid, facilityid);
                return Json(xclaimList);
            }
        }


        /// <summary>
        /// Views the file.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public string ViewFile(int id)
        {
            var xPaymentFileXmlBal = new XPaymentFileXMLBal();
            // Get the Entity list
            var xPaymentFileXmlfirstObj = xPaymentFileXmlBal.GetFirstXPaymentFileXML(id);
            return xPaymentFileXmlfirstObj;
        }

        /// <summary>
        /// Applies the chargeson file.
        /// </summary>
        /// <param name="fileid">The fileid.</param>
        /// <returns></returns>
        public ActionResult ApplyChargesonFile(int fileid)
        {
            using (var bal = new XclaimBal())
            {
                var corporateid = Helpers.GetSysAdminCorporateID();
                var facilityid = Helpers.GetDefaultFacilityId();
                //var xclaimList = bal.ApplyAdvicePayment(corporateid, facilityid);
                var xclaimList = bal.ApplyAdvicePaymentInRemittanceAdvice(corporateid, facilityid, fileid);
                return Json(xclaimList);
            }
        }
       
    }
}