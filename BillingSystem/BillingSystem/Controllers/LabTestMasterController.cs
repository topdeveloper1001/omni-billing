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
    public class LabTestMasterController : BaseController
    {
        #region Lab Test master Master
        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //Initialize the GlobalCode Bal
            using (var globalCodeBal = new GlobalCodeBal())
            {
                var cptcodeList = new List<CPTCodesCustomModel>();
                var labTestRange = globalCodeBal.GetRangeByCategoryType(Convert.ToInt32(GlobalCodeCategoryValue.LabTest).ToString());
                if (!string.IsNullOrEmpty(labTestRange))
                {
                    using (var cptcodebal = new CPTCodesBal(Helpers.DefaultCptTableNumber))
                    {
                      cptcodeList=  cptcodebal.GetCPTCustomCodesByRange(Convert.ToInt32(labTestRange.Split('-')[0]),
                            Convert.ToInt32(labTestRange.Split('-')[1]));
                    }
                }
                //var list = globalCodeBal.Get
                var globalCodeView = new CPTCodesView
                {
                    CurrentCPTCodeCustom = new CPTCodesCustomModel() { IsActive = true },
                    CPTCodesCustomList = cptcodeList,
                };

                //Pass the View Model in ActionResult to View Facility
                return View(globalCodeView);
            }
        }

        //Delete global code
        /// <summary>
        /// Deletes the lab test.
        /// </summary>
        /// <param name="globalCodeId">The global code identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteLabTest(int globalCodeId)
        {
            using (var bal = new CPTCodesBal(Helpers.DefaultCptTableNumber))
            {
                var gCode = bal.GetCPTCodesById(globalCodeId);
                if (gCode != null)
                {
                    gCode.IsDeleted = true;
                    gCode.DeletedBy = Helpers.GetLoggedInUserId();
                    gCode.DeletedDate = Helpers.GetInvariantCultureDateTime();
                    AddUpdateGlobalCode(gCode);
                    return Json(gCode.CPTCodesId);
                }
                return Json(null);
            }
        }

        /// <summary>
        /// Adds the update global code.
        /// </summary>
        /// <param name="objGlobalCode">The object global code.</param>
        /// <returns></returns>
        public int AddUpdateGlobalCode(CPTCodes objGlobalCode)
        {
            var globalCodeBal = new CPTCodesBal(Helpers.DefaultCptTableNumber);
            if (objGlobalCode.CPTCodesId > 0)
            {
                objGlobalCode.ModifiedBy = Helpers.GetLoggedInUserId();
                objGlobalCode.ModifiedDate = Helpers.GetInvariantCultureDateTime();
            }
            else
            {
                objGlobalCode.CreatedBy = Helpers.GetLoggedInUserId();
                objGlobalCode.CreatedDate = Helpers.GetInvariantCultureDateTime();
            }
            return globalCodeBal.AddUpdateCPTCodes(objGlobalCode);
        }

        /// <summary>
        /// Binds the lab test list.
        /// </summary>
        /// <returns></returns>
        public ActionResult BindLabTestList()
        {
            using (var globalCodeBal = new GlobalCodeBal())
            {
                var cptcodeList = new List<CPTCodesCustomModel>();
                var labTestRange = globalCodeBal.GetRangeByCategoryType(Convert.ToInt32(GlobalCodeCategoryValue.LabTest).ToString());
                if (!string.IsNullOrEmpty(labTestRange))
                {
                    using (var cptcodebal = new CPTCodesBal(Helpers.DefaultCptTableNumber))
                    {
                        cptcodeList = cptcodebal.GetCPTCustomCodesByRange(Convert.ToInt32(labTestRange.Split('-')[0]),
                              Convert.ToInt32(labTestRange.Split('-')[1]));
                    }
                }
                return PartialView(PartialViews.LabTestListView, cptcodeList);
            }
        }

        //Function to get  GlobalCode for editing
        /// <summary>
        /// Gets the current lab test.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult GetCurrentLabTest(int id)
        {
            using (var bal = new CPTCodesBal(Helpers.DefaultCptTableNumber))
            {
                var globalCode = bal.GetCPTCodesCustomById(id);
                return PartialView(PartialViews.LabTestAddEdit, globalCode);
            }
        }

        //Function To reset the User Form
        /// <summary>
        /// Resets the lab test result form.
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetLabTestResultForm()
        {
            var globalCode = new CPTCodesCustomModel() { IsActive = true };
            return PartialView(PartialViews.LabTestAddEdit, globalCode);
        }
        #endregion
	}
}