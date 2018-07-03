using BillingSystem.Common.Common;
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
    public class XPaymentReturnController : BaseController
    {
        private readonly IBillHeaderService _bhService;
        private readonly IGlobalCodeService _gService;
        private readonly IXPaymentReturnService _service;

        public XPaymentReturnController(IBillHeaderService bhService, IGlobalCodeService gService, IXPaymentReturnService service)
        {
            _bhService = bhService;
            _gService = gService;
            _service = service;
        }

        /// <summary>
        /// Get the details of the XPaymentReturn View in the Model XPaymentReturn such as XPaymentReturnList, list of countries etc.
        /// </summary>
        /// <param name="claimid">The claimid.</param>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model XPaymentReturn to be passed to View XPaymentReturn
        /// </returns>
        public ActionResult XPaymentReturnMain(int claimid, int? encid, int? Pid)
        {

            //Get the Entity list
            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            var xPaymentReturnList = _service.GetXPaymentReturnByClaimId(claimid);
            if (!xPaymentReturnList.Any())
            {
                var xpaymentgenObj = _service.GenerateRemittanceInfo(claimid, corporateId, facilityId);
                if (xpaymentgenObj)
                {
                    xPaymentReturnList = _service.GetXPaymentReturnByClaimId(claimid);
                }
            }
            //Intialize the View Model i.e. XPaymentReturnView which is binded to Main View Index.cshtml under XPaymentReturn
            var xPaymentReturnView = new XPaymentReturnView
            {
                XPaymentReturnList = xPaymentReturnList,
                CurrentXPaymentReturnCustomModel =
                    xPaymentReturnList.Any() ? xPaymentReturnList.FirstOrDefault() : new XPaymentReturnCustomModel(),
                CurrentXPaymentReturn = new Model.XPaymentReturn(),
                ClaimId = claimid,
                EncounterId = encid,
                PatientId = Pid
            };

            //Pass the View Model in ActionResult to View XPaymentReturn
            return View(xPaymentReturnView);
        }

        /// <summary>
        /// Bind all the XPaymentReturn list 
        /// </summary>
        /// <returns>action result with the partial view containing the XPaymentReturn list object</returns>
        [HttpPost]
        public ActionResult BindXPaymentReturnList()
        {
            //Get the facilities list
            var XPaymentReturnList = _service.GetXPaymentReturn();

            //Pass the ActionResult with List of XPaymentReturnViewModel object to Partial View XPaymentReturnList
            return PartialView(PartialViews.XPaymentReturnList, XPaymentReturnList);

        }

        /// <summary>
        /// Add New or Update the XPaymentReturn based on if we pass the XPaymentReturn ID in the XPaymentReturnViewModel object.
        /// </summary>
        /// <param name="XPaymentReturnModel">pass the details of XPaymentReturn in the view model</param>
        /// <returns>returns the newly added or updated ID of XPaymentReturn row</returns>
        public ActionResult SaveXPaymentReturn(XPaymentReturn model)
        {
            //Initialize the newId variable 
            var newId = -1;
            var userId = Helpers.GetLoggedInUserId();
            var currentDate = Helpers.GetInvariantCultureDateTime();
            //Check if Model is not null 
            if (model != null)
            {
                var getXpaymentObj = _service.GetXPaymentReturnModelByClaimId(Convert.ToInt32(model.ID));
                foreach (var xPaymentReturn in getXpaymentObj)
                {
                    xPaymentReturn.PaymentReference = model.PaymentReference;
                    xPaymentReturn.DenialCode = model.DenialCode;
                    xPaymentReturn.DateSettlement = model.DateSettlement;
                    xPaymentReturn.XModifiedBy = userId.ToString();
                    xPaymentReturn.XModifiedDate = currentDate;
                    _service.SaveXPaymentReturn(xPaymentReturn);
                    if (!string.IsNullOrEmpty(model.DenialCode))
                    {
                        var billheaderObj = _bhService.GetBillHeaderById(Convert.ToInt32(model.ID));
                        if (billheaderObj.Status == (BillHeaderStatus.RA1).ToString() ||
                            billheaderObj.Status == (BillHeaderStatus.RA2).ToString() ||
                            billheaderObj.Status == (BillHeaderStatus.RA3).ToString() ||
                            billheaderObj.Status == (BillHeaderStatus.S1).ToString() ||
                            billheaderObj.Status == (BillHeaderStatus.S2).ToString() ||
                            billheaderObj.Status == (BillHeaderStatus.S3).ToString())
                        {
                            var billheaderNewStatus = SetBillheaderStatus(billheaderObj.Status);
                            if (!string.IsNullOrEmpty(billheaderNewStatus))
                            {
                                var globalcodeObj = _gService.GetGCodesListByCategoryValue(Convert.ToInt32(GlobalCodeCategoryValue.BillHeaderStatus).ToString());
                                var newstatusval =
                                    globalcodeObj.FirstOrDefault(
                                        x => x.GlobalCodeName.Trim().Equals(billheaderObj.Status));
                                var newids = new List<int> { billheaderObj.BillHeaderID };
                                if (newstatusval != null)
                                    _bhService.SetBillHeaderStatus(newids, billheaderNewStatus,
                                        newstatusval.GlobalCodeValue);
                            }
                        }
                    }
                }

            }
            return Json(newId);
        }

        /// <summary>
        /// Saves the x payment detail return.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public ActionResult SaveXPaymentDetailReturn(XPaymentReturn model)
        {
            //Initialize the newId variable 
            var newId = -1;
            var userId = Helpers.GetLoggedInUserId();
            var currentDate = Helpers.GetInvariantCultureDateTime();
            //Check if Model is not null 
            if (model != null)
            {
                var getXpaymentObj = _service.GetXPaymentModelReturnById(Convert.ToInt32(model.XPaymentReturnID));
                getXpaymentObj.AADenialCode = model.AADenialCode;
                getXpaymentObj.AAPaymentAmount = model.AAPaymentAmount;
                getXpaymentObj.XModifiedBy = userId.ToString();
                getXpaymentObj.XModifiedDate = currentDate;
                newId = _service.SaveXPaymentReturn(getXpaymentObj);
                if (!string.IsNullOrEmpty(model.AADenialCode))
                {
                    var billheaderObj = _bhService.GetBillHeaderById(Convert.ToInt32(model.ID));
                    if (billheaderObj.Status == (BillHeaderStatus.RA1).ToString() ||
                        billheaderObj.Status == (BillHeaderStatus.RA2).ToString() ||
                        billheaderObj.Status == (BillHeaderStatus.RA3).ToString() ||
                        billheaderObj.Status == (BillHeaderStatus.S1).ToString() ||
                        billheaderObj.Status == (BillHeaderStatus.S2).ToString() ||
                        billheaderObj.Status == (BillHeaderStatus.S3).ToString())
                    {
                        var billheaderNewStatus = SetBillheaderStatus(billheaderObj.Status);
                        if (!string.IsNullOrEmpty(billheaderNewStatus))
                        {
                            var globalcodeObj = _gService.GetGCodesListByCategoryValue(Convert.ToInt32(GlobalCodeCategoryValue.BillHeaderStatus).ToString());
                            var newstatusval =
                                globalcodeObj.FirstOrDefault(
                                    x => x.GlobalCodeName.Trim().Equals(billheaderObj.Status));
                            var newids = new List<int> { billheaderObj.BillHeaderID };
                            if (newstatusval != null)
                                _bhService.SetBillHeaderStatus(newids, billheaderNewStatus,
                                    newstatusval.GlobalCodeValue);
                        }
                    }
                }
            }
            return Json(newId);
        }

        /// <summary>
        /// Get the details of the current XPaymentReturn in the view model by ID 
        /// </summary>
        /// <param name="shared">pass the input parameters such as ID</param>
        /// <returns></returns>
        public ActionResult GetXPaymentReturn(int id)
        {
            var currentXPaymentReturn = _service.GetXPaymentReturnById(id);
            return PartialView(PartialViews.XPaymentDetailEdit, currentXPaymentReturn);
        }

        /// <summary>
        /// Delete the current XPaymentReturn based on the XPaymentReturn ID passed in the XPaymentReturnModel
        /// </summary>
        /// <param name="shared"></param>
        /// <returns></returns>
        public ActionResult DeleteXPaymentReturn(int id)
        {
            //Get XPaymentReturn model object by current XPaymentReturn ID
            var currentXPaymentReturn = _service.GetXPaymentReturnById(id);
            var userId = Helpers.GetLoggedInUserId();

            //Check If XPaymentReturn model is not null
            if (currentXPaymentReturn != null)
            {
                //currentXPaymentReturn.IsActive = false;
                currentXPaymentReturn.XModifiedBy = userId.ToString();
                currentXPaymentReturn.XModifiedDate = CurrentDateTime;

                //Update Operation of current XPaymentReturn
                var result = _service.SaveXPaymentReturn(currentXPaymentReturn);

                //return deleted ID of current XPaymentReturn as Json Result to the Ajax Call.
                return Json(result);
            }

            //Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }

        /// <summary>
        /// Reset the XPaymentReturn View Model and pass it to XPaymentReturnAddEdit Partial View. 
        /// </summary>
        /// <param name="shared">pass the input parameters such as ID</param>
        /// <returns></returns>
        public ActionResult ResetXPaymentReturnForm()
        {
            //Intialize the new object of XPaymentReturn ViewModel
            var XPaymentReturnViewModel = new Model.XPaymentReturn();

            //Pass the View Model as XPaymentReturnViewModel to PartialView XPaymentReturnAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.XPaymentReturnAddEdit, XPaymentReturnViewModel);
        }

        /// <summary>
        /// Generates the remittance information.
        /// </summary>
        /// <param name="claimId">The claim identifier.</param>
        /// <returns></returns>
        public ActionResult GenerateRemittanceInfo(int claimId)
        {
            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            var xpaymentgenObj = _service.GenerateRemittanceInfo(claimId, corporateId, facilityId);
            if (xpaymentgenObj)
            {
                var getGenratedPayments = _service.GetXPaymentReturnByClaimId(claimId);
                return PartialView(PartialViews.XPaymentHeader, getGenratedPayments.Any() ? getGenratedPayments.FirstOrDefault() : new XPaymentReturnCustomModel());
            }
            return PartialView(PartialViews.XPaymentHeader, new XPaymentReturnCustomModel());
        }

        /// <summary>
        /// Gets the remittance information by claim identifier.
        /// </summary>
        /// <param name="claimId">The claim identifier.</param>
        /// <returns></returns>
        public ActionResult GetRemittanceInfoByClaimId(int claimId)
        {
            var getGenratedPayments = _service.GetXPaymentReturnByClaimId(claimId);
            return PartialView(PartialViews.XPaymentHeader, getGenratedPayments.Any() ? getGenratedPayments.FirstOrDefault() : new XPaymentReturnCustomModel());
        }

        /// <summary>
        /// Gets the remittance information list by claim identifier.
        /// </summary>
        /// <param name="claimId">The claim identifier.</param>
        /// <returns></returns>
        public ActionResult GetRemittanceInfoListByClaimId(int claimId)
        {
            var getGenratedPayments = _service.GetXPaymentReturnByClaimId(claimId);
            return PartialView(PartialViews.XPaymentReturnList, getGenratedPayments);
        }

        /// <summary>
        /// Generates the remittance XML file.
        /// </summary>
        /// <returns></returns>
        public ActionResult GenerateRemittanceXmlFile()
        {
            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            var getGenratedPayments = _service.GenerateRemittanceXmlFile(corporateId, facilityId);
            return Json(getGenratedPayments);
        }


        /// <summary>
        /// Sets the billheader status.
        /// </summary>
        /// <param name="currentStatus">The current status.</param>
        /// <returns></returns>
        private string SetBillheaderStatus(string currentStatus)
        {
            var globalcodeObj = _gService.GetGCodesListByCategoryValue(Convert.ToInt32(GlobalCodeCategoryValue.BillHeaderStatus).ToString());
            var newstatusval = globalcodeObj.FirstOrDefault(x => x.GlobalCodeName.Trim().Equals(currentStatus.Trim()));
            if (newstatusval != null)
            {
                var nextStatusval = newstatusval.ExternalValue2;
                var nextstatusglobalcodeVal =
                    globalcodeObj.SingleOrDefault(x => x.GlobalCodeName.Trim().Equals(nextStatusval.Trim()));
                if (nextstatusglobalcodeVal != null)
                {
                    return nextstatusglobalcodeVal.GlobalCodeValue;
                }
                return string.Empty;
            }
            return string.Empty;
        }
    }
}
