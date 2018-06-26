using BillingSystem.Models;
using BillingSystem.Common;
using System.Linq;
using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Model;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class DashboardBudgetController : BaseController
    {
        private readonly IDashboardBudgetService _service;

        public DashboardBudgetController(IDashboardBudgetService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get the details of the DashboardBudget View in the Model DashboardBudget such as DashboardBudgetList, list of countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model DashboardBudget to be passed to View DashboardBudget
        /// </returns>
        public ActionResult DashboardBudgetMain()
        {
            var corporateid = Helpers.GetSysAdminCorporateID();
            var facilityid = Helpers.GetDefaultFacilityId();
            //Get the Entity list
            var dashboardBudgetList = _service.GetDashboardBudget(corporateid, facilityid).Where(x => x.BudgetType == 1).ToList();

            //Intialize the View Model i.e. DashboardBudgetView which is binded to Main View Index.cshtml under DashboardBudget
            var dashboardBudgetView = new DashboardBudgetView
            {
                DashboardBudgetList = dashboardBudgetList,
                CurrentDashboardBudget = new Model.DashboardBudget()
            };

            //Pass the View Model in ActionResult to View DashboardBudget
            return View(dashboardBudgetView);
        }

        /// <summary>
        /// Bind all the DashboardBudget list 
        /// </summary>
        /// <returns>action result with the partial view containing the DashboardBudget list object</returns>
        [HttpPost]
        public ActionResult BindDashboardBudgetList()
        {
            var corporateid = Helpers.GetSysAdminCorporateID();
            var facilityid = Helpers.GetDefaultFacilityId();
            //Get the facilities list
            var dashboardBudgetList = _service.GetDashboardBudget(corporateid, facilityid).Where(x => x.BudgetType == 1).ToList();

            //Pass the ActionResult with List of DashboardBudgetViewModel object to Partial View DashboardBudgetList
            return PartialView(PartialViews.DashboardBudgetList, dashboardBudgetList);

        }

        /// <summary>
        /// Add New or Update the DashboardBudget based on if we pass the DashboardBudget ID in the DashboardBudgetViewModel object.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// returns the newly added or updated ID of DashboardBudget row
        /// </returns>
        public ActionResult SaveDashboardBudget(DashboardBudget model)
        {
            //Initialize the newId variable 
            var newId = -1;
            var userId = Helpers.GetLoggedInUserId();
            var corporateid = Helpers.GetSysAdminCorporateID();
            var facilityid = Helpers.GetDefaultFacilityId();
            var currentDatetime = Helpers.GetInvariantCultureDateTime();
            //Check if Model is not null 
            if (model != null)
            {
                if (model.BudgetId > 0)
                {
                    model.ModifiedBy = userId;
                    model.ModifiedDate = currentDatetime;
                }
                else
                {
                    model.CreatedBy = userId;
                    model.CreatedDate = currentDatetime;
                }
                model.CorporateId = corporateid;
                model.FacilityId = facilityid;
                //Call the AddDashboardBudget Method to Add / Update current DashboardBudget
                newId = _service.SaveDashboardBudget(model);
            }

            return Json(newId);
        }

        /// <summary>
        /// Get the details of the current DashboardBudget in the view model by ID
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult GetDashboardBudget(int id)
        {
            var currentDashboardBudget = _service.GetDashboardBudgetById(id);

            //Pass the ActionResult with the current DashboardBudgetViewModel object as model to PartialView DashboardBudgetAddEdit
            return PartialView(PartialViews.DashboardBudgetAddEdit, currentDashboardBudget);
        }

        /// <summary>
        /// Delete the current DashboardBudget based on the DashboardBudget ID passed in the DashboardBudgetModel
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteDashboardBudget(int id)
        {
            var currentDashboardBudget = _service.GetDashboardBudgetById(id);
            var userId = Helpers.GetLoggedInUserId();
            var result = _service.DeleteDashBoradBudget(currentDashboardBudget);
            //return deleted ID of current DashboardBudget as Json Result to the Ajax Call.
            return Json(result);
        }

        /// <summary>
        /// Reset the DashboardBudget View Model and pass it to DashboardBudgetAddEdit Partial View.
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetDashboardBudgetForm()
        {
            //Intialize the new object of DashboardBudget ViewModel
            var dashboardBudgetViewModel = new Model.DashboardBudget();

            //Pass the View Model as DashboardBudgetViewModel to PartialView DashboardBudgetAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.DashboardBudgetAddEdit, dashboardBudgetViewModel);
        }

        public ActionResult GetDashBoardBudgetData(int id)
        {
            var current = _service.GetDashboardBudgetById(id);
            var jsonData = new
            {
                current.AprilBudget,
                current.AugustBudget,
                current.BudgetDescription,
                current.BudgetFor,
                current.BudgetId,
                current.BudgetType,
                current.CorporateId,
                current.DecemberBudget,
                current.DepartmentNumber,
                current.FacilityId,
                current.FebruaryBudget,
                current.FiscalYear,
                current.IsActive,
                current.JanuaryBudget,
                current.JulyBudget,
                current.JuneBudget,
                current.MarchBudget,
                current.MayBudget,
                current.NovemberBudget,
                current.OctoberBudget,
                current.SeptemberBudget,

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
    }
}
