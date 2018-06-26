using BillingSystem.Models;
using BillingSystem.Common;
using System.Collections.Generic;
using System.Web.Mvc;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class DashboardDisplayOrderController : BaseController
    {
        private readonly IDashboardDisplayOrderService _service;

        public DashboardDisplayOrderController(IDashboardDisplayOrderService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get the details of the DashboardDisplayOrder View in the Model DashboardDisplayOrder such as DashboardDisplayOrderList, list of countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model DashboardDisplayOrder to be passed to View DashboardDisplayOrder
        /// </returns>
        public ActionResult Index()
        {
            //Get the Entity list
            var list = _service.GetDashboardDisplayOrderList(Helpers.GetSysAdminCorporateID(),Helpers.GetDefaultFacilityId());

            //Intialize the View Model i.e. DashboardDisplayOrderView which is binded to Main View Index.cshtml under DashboardDisplayOrder
            var viewModel = new DashboardDisplayOrderView
            {
                DashboardDisplayOrderList = list,
                CurrentDashboardDisplayOrder = new DashboardDisplayOrder { IsDeleted = false }
            };

            //Pass the View Model in ActionResult to View DashboardDisplayOrder
            return View(viewModel);
        }

        /// <summary>
        /// Add New or Update the DashboardDisplayOrder based on if we pass the DashboardDisplayOrder ID in the DashboardDisplayOrderViewModel object.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// returns the newly added or updated ID of DashboardDisplayOrder row
        /// </returns>
        public ActionResult SaveDashboardDisplayOrder(DashboardDisplayOrder model)
        {
            //Initialize the newId variable 
            var userId = Helpers.GetLoggedInUserId();
            var currentDate = Helpers.GetInvariantCultureDateTime();
            var list = new List<DashboardDisplayOrderCustomModel>();

            //Check if Model is not null 
            if (model != null)
            {
                var corporateid = Helpers.GetSysAdminCorporateID();
                model.CorporateId = corporateid;
                if (model.Id == 0)
                {
                    model.CreatedBy = userId;
                    model.CreatedDate = currentDate;
                }

                //Call the AddDashboardDisplayOrder Method to Add / Update current DashboardDisplayOrder
                list = _service.SaveDashboardDisplayOrder(model);
            }

            //Pass the ActionResult with List of DashboardDisplayOrderViewModel object to Partial View DashboardDisplayOrderList
            return PartialView(PartialViews.DashboardDisplayOrderList, list);
        }

        /// <summary>
        /// Get the details of the current DashboardDisplayOrder in the view model by ID
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public JsonResult GetDashboardDisplayOrderDetails(int id)
        {
            //Call the AddDashboardDisplayOrder Method to Add / Update current DashboardDisplayOrder
            var current = _service.GetDashboardDisplayOrderByID(id);

            //Pass the ActionResult with the current DashboardDisplayOrderViewModel object as model to PartialView DashboardDisplayOrderAddEdit
            return Json(current);
        }

        /// <summary>
        /// Delete the current DashboardDisplayOrder based on the DashboardDisplayOrder ID passed in the DashboardDisplayOrderModel
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteDashboardDisplayOrder(int id)
        {
            var list = _service.SaveDashboardDisplayOrder(new DashboardDisplayOrder { Id = id, IsDeleted = true }, true);

            //Pass the ActionResult with List of DashboardDisplayOrderViewModel object to Partial View DashboardDisplayOrderList
            return PartialView(PartialViews.DashboardDisplayOrderList, list);
        }
    }
}
