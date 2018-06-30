using System;
using System.Linq;
using BillingSystem.Models;
using BillingSystem.Common;
using System.Collections.Generic;
using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;
using BillingSystem.Common.Common;

namespace BillingSystem.Controllers
{
    public class ProjectsController : BaseController
    {
        /// <summary>
        /// Get the details of the Projects View in the Model Projects such as ProjectsList, list of countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model Projects to be passed to View Projects
        /// </returns>
        public ActionResult Index(Int32? fId)
        {
            //Initialize the Projects BAL object
            using (var bal = new ProjectsService())
            {
                var oProjectTargetsBal = new ProjectTargetsService();
                var corporateid = Helpers.GetSysAdminCorporateID();
                Session[SessionNames.SelectedFacilityId.ToString()] = fId ?? 17;

                var facilityid = Helpers.SetDropDownSelectedFacilityId() == 0 ? Helpers.GetDefaultFacilityId() : Helpers.SetDropDownSelectedFacilityId();
                Session[SessionNames.SelectedFacilityId.ToString()] = facilityid;
                //Get the Entity list
                var list = bal.GetProjectsList(corporateid, facilityid);

                //Intialize the View Model i.e. ProjectsView which is binded to Main View Index.cshtml under Projects
                var viewModel = new ProjectsView
                {
                    ProjectsList = list,
                    CurrentProjects = new Projects { IsActive = true },
                    CurrentProjectTargets = new ProjectTargets { IsActive = true },
                    CurrentProjectTasks = new ProjectTasks { IsActive = true, ExternalValue1 = "1" },
                    CurrentProjectTaskTarget = new ProjectTaskTargets { IsActive = true },
                    ProjectTargetList = oProjectTargetsBal.GetProjectTargetsList(corporateid, facilityid),
                    FacilityId = facilityid
                };

                //Pass the View Model in ActionResult to View Projects
                return View(viewModel);
            }
        }

        /// <summary>
        /// Add New or Update the Projects based on if we pass the Projects ID in the ProjectsViewModel object.
        /// </summary>
        /// <returns>returns the newly added or updated ID of Projects row</returns>
        public ActionResult SaveProjects(Projects model)
        {
            //Initialize the newId variable 
            var userId = Helpers.GetLoggedInUserId();
            var currentDate = Helpers.GetInvariantCultureDateTime();
            var list = new List<ProjectsCustomModel>();

            //Check if Model is not null 
            if (model != null)
            {
                var corporateid = Helpers.GetSysAdminCorporateID();
                var facilityid = Helpers.GetDefaultFacilityId();
                model.CorporateId = corporateid;
                //model.FacilityId = facilityid;

                using (var bal = new ProjectsService())
                {
                    if (model.ProjectId > 0)
                    {
                        model.ModifiedBy = userId;
                        model.ModifiedDate = currentDate;
                    }
                    else
                    {
                        model.CreatedBy = userId;
                        model.CreatedDate = currentDate;
                    }

                    //Call the AddProjects Method to Add / Update current Projects
                    list = bal.SaveProjects(model);
                }
                //Pass the ActionResult with List of ProjectsViewModel object to Partial View ProjectsList
            }
            return PartialView(PartialViews.ProjectsList, list);
        }

        /// <summary>
        /// Get the details of the current Projects in the view model by ID 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetProjectsDetails(int id)
        {
            using (var bal = new ProjectsService())
            {
                //Call the AddProjects Method to Add / Update current Projects
                var current = bal.GetProjectsById(id);

                var jsonResult = new
                {
                    current.ProjectId,
                    current.ProjectNumber,
                    StartDate = current.StartDate.GetShortDateString3(),
                    EstCompletionDate = current.EstCompletionDate.GetShortDateString3(),
                    current.Name,
                    current.ProjectDescription,
                    current.UserResponsible,
                    current.IsActive,
                    current.ExternalValue1,
                    current.ExternalValue2,
                    current.ExternalValue3,
                    current.ExternalValue4,
                    current.ExternalValue5,
                };
                //Pass the ActionResult with the current ProjectsViewModel object as model to PartialView ProjectsAddEdit
                return Json(jsonResult, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Delete the current Projects based on the Projects ID passed in the ProjectsModel
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteProjects(int id)
        {
            using (var bal = new ProjectsService())
            {
                //var list = bal.DeleteProjects(id, Helpers.GetDefaultCorporateId(), Helpers.GetDefaultFacilityId());
                var list = bal.DeleteProjects(id, Helpers.GetDefaultCorporateId(), Helpers.SetDropDownSelectedFacilityId());
                return PartialView(PartialViews.ProjectsList, list);
            }
        }

        /// <summary>
        /// Projects the targets list.
        /// </summary>
        /// <returns></returns>
        public PartialViewResult ProjectTargetsList()
        {
            using (var bal = new ProjectTargetsService())
            {
                //var list = bal.GetProjectTargetsList(Helpers.GetDefaultCorporateId(), Helpers.GetDefaultFacilityId());
                var list = bal.GetProjectTargetsList(Helpers.GetDefaultCorporateId(), Helpers.SetDropDownSelectedFacilityId());
                return PartialView(PartialViews.ProjectTargetsList, list);
            }
        }

        /// <summary>
        /// Projects the tasks list.
        /// </summary>
        /// <returns></returns>
        public ActionResult ProjectTasksList(string userId)
        {
            using (var bal = new ProjectTasksService())
            {
                //var list = bal.GetProjectTasksList(Helpers.GetDefaultCorporateId(), Helpers.GetDefaultFacilityId());
                var list = bal.GetProjectTasksList(Helpers.GetDefaultCorporateId(), Helpers.SetDropDownSelectedFacilityId(), userId);
                return PartialView(PartialViews.ProjectTasksList, list);
            }
        }

        /// <summary>
        /// Projects the task targets list.
        /// </summary>
        /// <returns></returns>
        public ActionResult ProjectTaskTargetsList()
        {
            using (var bal = new ProjectTaskTargetsService())
            {
                //var list = bal.GetProjectTaskTargetsList(Helpers.GetDefaultCorporateId(), Helpers.GetDefaultFacilityId());
                var list = bal.GetProjectTaskTargetsList(Helpers.GetDefaultCorporateId(), Helpers.SetDropDownSelectedFacilityId());
                return PartialView(PartialViews.ProjectTaskTargetsList, list);
            }
        }

        /// <summary>
        /// Gets the project numbers.
        /// </summary>
        /// <returns></returns>
        public JsonResult GetProjectNumbers()
        {
            var list = new List<SelectListItem>();
            using (var bal = new ProjectsService())
            {
                //var projects = bal.GetProjectNumbers(Helpers.GetDefaultCorporateId(), Helpers.GetDefaultFacilityId());
                var projects = bal.GetProjectNumbers(Helpers.GetDefaultCorporateId(), Helpers.SetDropDownSelectedFacilityId());
                if (projects.Count > 0)
                    list.AddRange(projects.Select(item => new SelectListItem
                    {
                        Text = string.Format("{0} - {1}", item.ProjectNumber, item.Name),
                        Value = item.ProjectNumber
                    }));
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        #region Project Targets
        /// <summary>
        /// Add New or Update the ProjectTargets based on if we pass the ProjectTargets ID in the ProjectTargetsViewModel object.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// returns the newly added or updated ID of ProjectTargets row
        /// </returns>
        public ActionResult SaveProjectTargets(ProjectTargets model)
        {
            var userId = Helpers.GetLoggedInUserId();
            var currentDate = Helpers.GetInvariantCultureDateTime();
            List<ProjectTargetsCustomModel> list;
            var corporateid = Helpers.GetSysAdminCorporateID();
            var facilityid = Helpers.GetDefaultFacilityId();

            model.CorporateId = corporateid;
            //model.FacilityId = facilityid;

            var month = Convert.ToDateTime(model.ProjectDate).Month;
            model.ExternalValue2 = Convert.ToString(month);

            using (var bal = new ProjectTargetsService())
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

                //Call the AddProjectTargets Method to Add / Update current ProjectTargets
                list = bal.SaveProjectTargets(model);
                var val = bal.SaveMonthWiseValuesInProjectDashboard(model.ProjectNumber, Convert.ToString(month),
                    Convert.ToString(model.CorporateId),
                    Convert.ToString(model.FacilityId), "");
            }

            //Pass the ActionResult with List of ProjectTargetsViewModel object to Partial View ProjectTargetsList
            return PartialView(PartialViews.ProjectTargetsList, list);
        }

        /// <summary>
        /// Delete the current ProjectTargets based on the ProjectTargets ID passed in the ProjectTargetsModel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteProjectTargets(int id)
        {
            using (var bal = new ProjectTargetsService())
            {
                //var list = bal.DeleteProjectTarget(id, Helpers.GetDefaultCorporateId(), Helpers.GetDefaultFacilityId());
                var list = bal.DeleteProjectTarget(id, Helpers.GetDefaultCorporateId(), Helpers.SetDropDownSelectedFacilityId());
                return PartialView(PartialViews.ProjectTargetsList, list);
            }
        }
        #endregion

        #region Project Tasks
        /// <summary>
        /// Add New or Update the ProjectTasks based on if we pass the ProjectTasks ID in the ProjectTasksViewModel object.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns the newly added or updated ID of ProjectTasks row</returns>
        public ActionResult SaveProjectTasks(ProjectTasks model)
        {
            var bal = new ProjectTasksService();
            var list = new List<ProjectTasksCustomModel>();

            if (model != null)
            {
                /*
                 * Check duplicate Task Number under the selected Project Number
                */
                var isExists = bal.CheckDuplicateTaskNumber(model.ProjectNumber, model.TaskNumber,
                    Convert.ToInt32(model.ProjectTaskId));

                if (!isExists)
                {
                    //Initialize the newId variable 
                    var userId = Helpers.GetLoggedInUserId();
                    var currentDate = Helpers.GetInvariantCultureDateTime();

                    var corporateid = Helpers.GetSysAdminCorporateID();
                    model.CorporateId = corporateid;
                    if (model.ProjectTaskId > 0)
                    {
                        model.ModifiedBy = userId;
                        model.ModifiedDate = currentDate;
                    }
                    else
                    {
                        model.CreatedBy = userId;
                        model.CreatedDate = currentDate;
                    }

                    //Call the AddProjectTasks Method to Add / Update current ProjectTasks
                    list = bal.SaveProjectTasks(model);
                }
            }
            //Pass the ActionResult with List of ProjectTasksViewModel object to Partial View ProjectTasksList
            return PartialView(PartialViews.ProjectTasksList, list);
        }

        /// <summary>
        /// Delete the current ProjectTasks based on the ProjectTasks ID passed in the ProjectTasksModel
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult DeleteProjectTasks(int id, string userId)
        {
            using (var bal = new ProjectTasksService())
            {
                //var list = bal.DeleteProjectTask(id, Helpers.GetDefaultCorporateId(), Helpers.GetDefaultFacilityId());
                var list = bal.DeleteProjectTask(id, Helpers.GetDefaultCorporateId(), Helpers.SetDropDownSelectedFacilityId(), userId);
                //Pass the ActionResult with List of ProjectTasksViewModel object to Partial View ProjectTasksList
                return PartialView(PartialViews.ProjectTasksList, list);
            }
        }
        #endregion

        /// <summary>
        /// Gets the global codes.
        /// </summary>
        /// <param name="categoryId">The category identifier.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        [LoginAuthorize]
        public ActionResult BindTargetCompletionValueDropDown(string categoryId)
        {
            var bal = new GlobalCodeService();
            var list = bal.GetGlobalCodesByCategoryValue(categoryId).OrderBy(x => x.SortOrder);
            return Json(list);
        }

        #region Project Task Targets
        /// <summary>
        /// Add New or Update the ProjectTaskTargets based on if we pass the ProjectTaskTargets ID in the ProjectTaskTargetsViewModel object.
        /// </summary>
        /// <param name="model">pass the details of ProjectTaskTargets in the view model</param>
        /// <returns>returns the newly added or updated ID of ProjectTaskTargets row</returns>
        public ActionResult SaveProjectTaskTargets(ProjectTaskTargets model)
        {
            //Initialize the newId variable 
            var userId = Helpers.GetLoggedInUserId();
            var currentDate = Helpers.GetInvariantCultureDateTime();
            var list = new List<ProjectTaskTargetsCustomModel>();

            model.CorporateId = Helpers.GetSysAdminCorporateID();
            //model.FacilityId = Helpers.GetDefaultFacilityId();

            var month = Convert.ToDateTime(model.TaskDate).Month;
            model.ExternalValue2 = Convert.ToString(month);
            //Check if Model is not null 
            if (model != null)
            {
                using (var bal = new ProjectTaskTargetsService())
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

                    //Call the AddProjectTaskTargets Method to Add / Update current ProjectTaskTargets
                    list = bal.SaveProjectTaskTargets(model);
                    var oProjectTargetsBal = new ProjectTargetsService();
                    var val = oProjectTargetsBal.SaveMonthWiseValuesInProjectDashboard("", Convert.ToString(month),
                   Convert.ToString(model.CorporateId),
                   Convert.ToString(model.FacilityId), Convert.ToString(model.TaskNumber));
                }
            }

            //Pass the ActionResult with List of ProjectTaskTargetsViewModel object to Partial View ProjectTaskTargetsList
            return PartialView(PartialViews.ProjectTaskTargetsList, list);
        }

        /// <summary>
        /// Delete the current ProjectTaskTargets based on the ProjectTaskTargets ID passed in the ProjectTaskTargetsModel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteProjectTaskTargets(int id)
        {
            using (var bal = new ProjectTaskTargetsService())
            {
                //var list = bal.DeleteProjectTaskTargets(id, Helpers.GetDefaultCorporateId(), Helpers.GetDefaultFacilityId());
                var list = bal.DeleteProjectTaskTargets(id, Helpers.GetDefaultCorporateId(), Helpers.SetDropDownSelectedFacilityId());
                //Pass the ActionResult with List of ProjectTaskTargetsViewModel object to Partial View ProjectTaskTargetsList
                return PartialView(PartialViews.ProjectTaskTargetsList, list);
            }
        }

        /// <summary>
        /// Binds the task numbers.
        /// </summary>
        /// <returns></returns>
        public JsonResult BindTaskNumbers()
        {
            var list = new List<SelectListItem>();
            using (var bal = new ProjectTasksService())
            {
                //var tn = bal.GetTaskNumbers(Helpers.GetDefaultCorporateId(), Helpers.GetDefaultFacilityId());
                var tn = bal.GetTaskNumbers(Helpers.GetDefaultCorporateId(), Helpers.SetDropDownSelectedFacilityId());
                if (tn.Count > 0)
                    list.AddRange(tn.Select(item => new SelectListItem { Text = item, Value = item }));
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public JsonResult CheckDuplicateProjectNumber(string projectNumber, int projectId)
        {
            var isExists = true;
            using (var bal = new ProjectsService())
                isExists = bal.CheckDuplicateProjectNumber(projectNumber, projectId);

            return Json(isExists, JsonRequestBehavior.AllowGet);
        }


        public JsonResult CheckDuplicateTaskNumber(string projectNumber, string taskNumber, int taskId)
        {
            var isExists = true;
            using (var bal = new ProjectTasksService())
                isExists = bal.CheckDuplicateTaskNumber(projectNumber, taskNumber, taskId);

            return Json(isExists, JsonRequestBehavior.AllowGet);
        }
    }
}
