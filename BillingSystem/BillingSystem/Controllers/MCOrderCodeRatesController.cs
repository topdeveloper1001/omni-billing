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
    public class MCOrderCodeRatesController : BaseController
    {
        private readonly IMCOrderCodeRatesService _service;
        private readonly IMcContractService _mcService;

        public MCOrderCodeRatesController(IMCOrderCodeRatesService service, IMcContractService mcService)
        {
            _service = service;
            _mcService = mcService;
        }

        /// <summary>
        /// Get the details of the MCOrderCodeRates View in the Model MCOrderCodeRates such as MCOrderCodeRatesList, list of countries etc.
        /// </summary>
        /// <param name="shared">passed the input object</param>
        /// <returns>returns the actionresult in the form of current object of the Model MCOrderCodeRates to be passed to View MCOrderCodeRates</returns>
        public ActionResult Index()
        {
            var list = _service.GetMCOrderCodeRatesList();
            var viewModel = new MCOrderCodeRatesView
            {
                MCOrderCodeRatesList = list,
                CurrentMCOrderCodeRates = new MCOrderCodeRates()
            };
            return View(viewModel);
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
                list = _service.SaveMCOrderCodeRates(model);

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
            var current = _service.GetMCOrderCodeRatesByID(id);

            return Json(current);
        }


        /// <summary>
        /// Delete the current MCRulesTable based on the MCRulesTable ID passed in the MCRulesTableModel
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteMCOrderCodeRates(int id)
        {
            var list = new List<MCOrderCodeRatesCustomModel>();
            var obj = _service.GetMCOrderCodeRatesByID(id);
            var result = _service.DeleteMCOrderCodeRates(id);
            list = _service.GetMcOrderCodeRatesListByMcCode(Convert.ToInt32(obj.MCCode));
            return PartialView(PartialViews.MCOrderCodeRatesList, list);
        }

        /// <summary>
        /// Binds the mc order code rates list.
        /// </summary>
        /// <param name="McContractID">The mc contract identifier.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BindMCOrderCodeRatesList(int McContractID)
        {
            var mcOrderCodeRatesList = _service.GetMcOrderCodeRatesListByMcCode(McContractID);
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
            var mccontractObj = _mcService.GetMcContractDetail(McContractID);
            //Get the Entity list
            var mcOrderCodeRatesList = _service.GetMcOrderCodeRatesListByMcCode(Convert.ToInt32(mccontractObj.MCCode));
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
            var mcOrderCodeRatesList = _service.GetMcOrderCodeRatesListByMcCode(mcCode);
            mcOrderCodeRatesList = ordrtype == 0
                ? mcOrderCodeRatesList
                : mcOrderCodeRatesList.Where(x => x.OrderType == ordrtype).ToList();
            return PartialView(PartialViews.MCOrderCodeRatesList, mcOrderCodeRatesList);
        }
    }
}
