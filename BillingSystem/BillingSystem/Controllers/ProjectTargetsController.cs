using BillingSystem.Common;
using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class ProjectTargetsController : BaseController
    {
        private readonly IProjectTargetsService _service;

        public ProjectTargetsController(IProjectTargetsService service)
        {
            _service = service;
        }


        /// <summary>
        /// Get the details of the current ProjectTargets in the view model by ID 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetProjectTargetsDetails(int id)
        {
            //Call the AddProjectTargets Method to Add / Update current ProjectTargets
            var current = _service.GetProjectTargetsById(id);

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
