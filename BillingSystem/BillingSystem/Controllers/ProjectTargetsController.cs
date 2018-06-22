using BillingSystem.Models;
using BillingSystem.Common;
using System.Collections.Generic;
using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Controllers
{
    public class ProjectTargetsController : BaseController
    {
        /// <summary>
        /// Get the details of the current ProjectTargets in the view model by ID 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetProjectTargetsDetails(int id)
        {
            using (var bal = new ProjectTargetsBal())
            {
                //Call the AddProjectTargets Method to Add / Update current ProjectTargets
                var current = bal.GetProjectTargetsById(id);

                var jsonResult = new
                {
                    current.Id,
                    ProjectDate1 = current.ProjectDate.GetShortDateString3(),
                    current.ProjectNumber,
                    current.TargetedCompletionValue,
                    current.IsActive,
                };

                //Pass the ActionResult with the current ProjectTargetsViewModel object as model to PartialView ProjectTargetsAddEdit
                return Json(jsonResult, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
