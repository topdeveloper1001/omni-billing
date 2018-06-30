using BillingSystem.Models;
using BillingSystem.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Controllers
{
    public class MCOrderCodeRatesController : BaseController
    {
        /// <summary>
        /// Get the details of the MCOrderCodeRates View in the Model MCOrderCodeRates such as MCOrderCodeRatesList, list of countries etc.
        /// </summary>
        /// <param name="shared">passed the input object</param>
        /// <returns>returns the actionresult in the form of current object of the Model MCOrderCodeRates to be passed to View MCOrderCodeRates</returns>
        public ActionResult Index()
        {
            //Initialize the MCOrderCodeRates BAL object
            using (var bal = new MCOrderCodeRatesService())
            {
                //Get the Entity list
                var list = bal.GetMCOrderCodeRatesList();

                //Intialize the View Model i.e. MCOrderCodeRatesView which is binded to Main View Index.cshtml under MCOrderCodeRates
                var viewModel = new MCOrderCodeRatesView
                {
                    MCOrderCodeRatesList = list,
                    CurrentMCOrderCodeRates = new MCOrderCodeRates()
                };

                //Pass the View Model in ActionResult to View MCOrderCodeRates
                return View(viewModel);
            }
        }

        /// <summary>
        /// Add New or Update the MCOrderCodeRates based on if we pass the MCOrderCodeRates ID in the MCOrderCodeRatesViewModel object.
        /// </summary>
        /// <returns>returns the newly added or updated ID of MCOrderCodeRates row</returns>
        public ActionResult SaveMCOrderCodeRates(MCOrderCodeRates model)
        {
            //Initialize the newId variable 
            var list = new List<MCOrderCodeRatesCustomModel>();

            //Check if Model is not null 
            if (model != null)
            {
                //Call the AddMCOrderCodeRates Method to Add / Update current MCOrderCodeRates
                using (var bal = new MCOrderCodeRatesService())
                    list = bal.SaveMCOrderCodeRates(model);

            }
            //Pass the ActionResult with List of MCOrderCodeRatesViewModel object to Partial View MCOrderCodeRatesList
            return PartialView(PartialViews.MCOrderCodeRatesList, list);
        }

        /// <summary>
        /// Get the details of the current MCOrderCodeRates in the view model by ID 
        /// </summary>
        /// <param name="shared">pass the input parameters such as ID</param>
        /// <returns></returns>
        public JsonResult GetMCOrderCodeRatesDetails(int id)
        {
            using (var bal = new MCOrderCodeRatesService())
            {
                //Call the AddMCOrderCodeRates Method to Add / Update current MCOrderCodeRates
                var current = bal.GetMCOrderCodeRatesByID(id);

                //Pass the ActionResult with the current MCOrderCodeRatesViewModel object as model to PartialView MCOrderCodeRatesAddEdit
                return Json(current);
            }
        }

        /// <summary>
        /// Delete the current MCOrderCodeRates based on the MCOrderCodeRates ID passed in the MCOrderCodeRatesModel
        /// </summary>
        /// <param name="shared"></param>
        /// <returns></returns>
        //public ActionResult DeleteMCOrderCodeRates(int id)
        //{
        //    using (var bal = new MCOrderCodeRatesBal())
        //    {
        //        //Get MCOrderCodeRates model object by current MCOrderCodeRates ID
        //        var model = bal.GetMCOrderCodeRatesByID(id);
        //        var userId = Helpers.GetLoggedInUserId();
        //        var list = new List<MCOrderCodeRatesCustomModel>();
        //        var currentDate = Helpers.GetInvariantCultureDateTime();

        //        //Check If MCOrderCodeRates model is not null
        //        if (model != null)
        //        {
        //            //Update Operation of current MCOrderCodeRates
        //            var result = bal.SaveMCOrderCodeRates(model);
        //            list = bal.GetMCOrderCodeRatesList();
        //            //return deleted ID of current MCOrderCodeRates as Json Result to the Ajax Call.
        //            return Json(result);
        //        }
        //    }
        //    return Json(null);
        //    //Pass the ActionResult with List of MCOrderCodeRatesViewModel object to Partial View MCOrderCodeRatesList
        //    //return PartialView(PartialViews.MCOrderCodeRatesList, list);
        //}

        /// <summary>
        /// Delete the current MCRulesTable based on the MCRulesTable ID passed in the MCRulesTableModel
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteMCOrderCodeRates(int id)
        {
            var list = new List<MCOrderCodeRatesCustomModel>();
            using (var bal = new MCOrderCodeRatesService())
            {
                var obj = bal.GetMCOrderCodeRatesByID(id);
                var result = bal.DeleteMCOrderCodeRates(id);
                list = bal.GetMcOrderCodeRatesListByMcCode(Convert.ToInt32(obj.MCCode));
                return PartialView(PartialViews.MCOrderCodeRatesList, list);
            }
        }

        /// <summary>
        /// Binds the mc order code rates list.
        /// </summary>
        /// <param name="McContractID">The mc contract identifier.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BindMCOrderCodeRatesList(int McContractID)
        {
            //Initialize the RuleStep BAL object
            var mcOrderCodeRatesBal = new MCOrderCodeRatesService();
            //Get the Entity list
            var mcOrderCodeRatesList = mcOrderCodeRatesBal.GetMcOrderCodeRatesListByMcCode(McContractID);
            //Pass the ActionResult with List of RuleStepViewModel object to Partial View RuleStepList
            return PartialView(PartialViews.MCOrderCodeRatesList, mcOrderCodeRatesList);

        }


        /// <summary>
        /// Binds the mc order code list.
        /// </summary>
        /// <param name="McContractID">The mc contract identifier.</param>
        /// <returns></returns>
        public ActionResult BindMCOrderCodeList(int McContractID)
        {
            var mcContractbal = new McContractService(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber);
            var mccontractObj = mcContractbal.GetMcContractDetail(McContractID);
            //Initialize the RuleStep BAL object
            var mcOrderCodeRatesBal = new MCOrderCodeRatesService();
            //Get the Entity list
            var mcOrderCodeRatesList = mcOrderCodeRatesBal.GetMcOrderCodeRatesListByMcCode(Convert.ToInt32(mccontractObj.MCCode));
            //Pass the ActionResult with List of RuleStepViewModel object to Partial View RuleStepList
            return PartialView(PartialViews.MCOrderCodeRatesList, mcOrderCodeRatesList);
        }

        /// <summary>
        /// Views the type of the order rate list by.
        /// </summary>
        /// <param name="ordrtype">The ordrtype.</param>
        /// <param name="mcCode">The mc code.</param>
        /// <returns></returns>
        public ActionResult ViewOrderRateListByType(int ordrtype, int mcCode)
        {
            var mcOrderCodeRatesBal = new MCOrderCodeRatesService();
            var mcOrderCodeRatesList = mcOrderCodeRatesBal.GetMcOrderCodeRatesListByMcCode(mcCode);
            mcOrderCodeRatesList = ordrtype == 0
                ? mcOrderCodeRatesList
                : mcOrderCodeRatesList.Where(x => x.OrderType == ordrtype).ToList();
            return PartialView(PartialViews.MCOrderCodeRatesList, mcOrderCodeRatesList);
        }
    }
}
