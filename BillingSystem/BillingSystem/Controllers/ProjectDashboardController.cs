using BillingSystem.Models;
using BillingSystem.Common;
using System.Collections.Generic;
using System.Web.Mvc;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class ProjectDashboardController : BaseController
    {
        private readonly IProjectDashboardService _service;

        public ProjectDashboardController(IProjectDashboardService service)
        {
            _service = service;
        }


        /// <summary>
        /// Get the details of the ProjectDashboard View in the Model ProjectDashboard such as ProjectDashboardList, list of countries etc.
        /// </summary>
        /// <param name="shared">passed the input object</param>
        /// <returns>returns the actionresult in the form of current object of the Model ProjectDashboard to be passed to View ProjectDashboard</returns>
        public ActionResult Index()
        {
            //Get the Entity list
            var corpoarteid = Helpers.GetSysAdminCorporateID();
            var facilityid = Helpers.GetDefaultFacilityId();
            var list = _service.GetProjectDashboardList(corpoarteid, facilityid);

            //Intialize the View Model i.e. ProjectDashboardView which is binded to Main View Index.cshtml under ProjectDashboard
            var viewModel = new ProjectDashboardView
            {
                ProjectDashboardList = list,
                CurrentProjectDashboard = new ProjectDashboard()
            };

            //Pass the View Model in ActionResult to View ProjectDashboard
            return View(viewModel);
        }

        /// <summary>
        /// Add New or Update the ProjectDashboard based on if we pass the ProjectDashboard ID in the ProjectDashboardViewModel object.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// returns the newly added or updated ID of ProjectDashboard row
        /// </returns>
        public ActionResult SaveProjectDashboard(ProjectDashboard model)
        {
            //Initialize the newId variable 
            var userId = Helpers.GetLoggedInUserId();
            var currentDate = Helpers.GetInvariantCultureDateTime();
            var list = new List<ProjectDashboardCustomModel>();

            //Check if Model is not null 
            if (model != null)
            {
                if (model.Id > 0)
                {
                    model.ModifiedBy = userId;
                    model.ModifiedDate = currentDate;
                }
                else
                {
                    model.CreatedBy = userId;
                    model.CreatedDate = currentDate;
                }
                //Call the AddProjectDashboard Method to Add / Update current ProjectDashboard
                list = _service.SaveProjectDashboard(model);
            }
            //Pass the ActionResult with List of ProjectDashboardViewModel object to Partial View ProjectDashboardList
            return PartialView(PartialViews.ProjectDashboardList, list);
        }

        /// <summary>
        /// Get the details of the current ProjectDashboard in the view model by ID
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public JsonResult GetProjectDashboardDetails(int id)
        {
            //Call the AddProjectDashboard Method to Add / Update current ProjectDashboard
            var current = _service.GetProjectDashboardById(id);
            //Pass the ActionResult with the current ProjectDashboardViewModel object as model to PartialView ProjectDashboardAddEdit
            return Json(current);
        }

        /// <summary>
        /// Delete the current ProjectDashboard based on the ProjectDashboard ID passed in the ProjectDashboardModel
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteProjectDashboard(int id)
        {
            //Get ProjectDashboard model object by current ProjectDashboard ID
            var model = _service.GetProjectDashboardById(id);
            var userId = Helpers.GetLoggedInUserId();
            var list = new List<ProjectDashboardCustomModel>();
            var currentDate = Helpers.GetInvariantCultureDateTime();

            //Check If ProjectDashboard model is not null
            if (model != null)
            {
                model.IsActive = false;
                model.ModifiedBy = userId;
                model.ModifiedDate = currentDate;
                //Update Operation of current ProjectDashboard
                list = _service.SaveProjectDashboard(model);
            }
            //Pass the ActionResult with List of ProjectDashboardViewModel object to Partial View ProjectDashboardList
            return PartialView(PartialViews.ProjectDashboardList, list);
        }
    }
}
