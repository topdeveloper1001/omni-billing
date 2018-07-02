using BillingSystem.Common;
using System.Web.Mvc;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class ProjectTasksController : BaseController
    {
        private readonly IProjectTasksService _service;

        public ProjectTasksController(IProjectTasksService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get the details of the current ProjectTasks in the view model by ID 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetProjectTasksDetails(int id)
        {
            var current = _service.GetProjectTasksById(id);

            var jsonResult = new
            {
                current.ProjectTaskId,
                current.ProjectNumber,
                current.IsActive,
                StartDate = current.StartDate.GetShortDateString3(),
                CompletionDate = current.EstCompletionDate.GetShortDateString3(),
                current.TaskName,
                current.TaskNumber,
                current.TaskDescription,
                current.UserResponsible,
                current.ExternalValue1,
                current.ExternalValue2,
                current.ExternalValue3,
                current.ExternalValue4,
                current.ExternalValue5,
                current.Comments
            };

            //Pass the ActionResult with the current ProjectTasksViewModel object as model to PartialView ProjectTasksAddEdit
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
    }
}
