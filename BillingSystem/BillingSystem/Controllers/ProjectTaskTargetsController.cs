using BillingSystem.Common;
using System.Web.Mvc;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class ProjectTaskTargetsController : BaseController
    {
        private readonly IProjectTaskTargetsService _service;

        public ProjectTaskTargetsController(IProjectTaskTargetsService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get the details of the current ProjectTaskTargets in the view model by ID 
        /// </summary>
        /// <param name="shared">pass the input parameters such as ID</param>
        /// <returns></returns>
        public JsonResult GetProjectTaskTargetsDetails(int id)
        {
            var current = _service.GetProjectTaskTargetsById(id);

            var jsonResult = new
            {
                current.Id,
                current.TaskNumber,
                TaskDate1 = current.TaskDate.GetShortDateString3(),
                current.TargetedCompletionValue,
                current.ExternalValue1,
                current.ExternalValue2,
                current.ExternalValue3,
                current.ExternalValue5,
                current.IsActive
            };

            //Pass the ActionResult with the current ProjectTaskTargetsViewModel object as model to PartialView ProjectTaskTargetsAddEdit
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
    }
}
