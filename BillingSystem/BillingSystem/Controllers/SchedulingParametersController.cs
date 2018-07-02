using BillingSystem.Bal.Interfaces;
using BillingSystem.Common;
using BillingSystem.Model.CustomModel;
using System.Web.Mvc;

namespace BillingSystem.Controllers
{
    public class SchedulingParametersController : BaseController
    {
        private readonly ISchedulingParametersService _service;

        public SchedulingParametersController(ISchedulingParametersService service)
        {
            _service = service;
        }

        // GET: SchedulingParameters
        public ActionResult Index()
        {
            return View(new SchedulingParametersCustomModel());
        }

        public JsonResult GetDataByFacilityId(long fId)
        {
            //Call the AddBillingSystemParameters Method to Add / Update current BillingSystemParameters
            var data = _service.GetDataByFacilityId(fId);

            var jsonResult = new
            {
                data.Id,
                FacilityId = fId,
                data.CorporateId,
                data.StartHour,
                data.EndHour
            };

            //Pass the ActionResult with the current BillingSystemParametersViewModel object as model to PartialView BillingSystemParametersAddEdit
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Save(SchedulingParametersCustomModel vm)
        {
            long id = 0;

            //Initialize the newId variable 
            var userId = Helpers.GetLoggedInUserId();
            var currentDate = Helpers.GetInvariantCultureDateTime();

            //Check if Model is not null
            if (vm != null)
            {
                vm.IsActive = true;
                if (vm.Id > 0)
                {
                    vm.ModifiedBy = userId;
                    vm.ModifiedDate = currentDate;
                }
                else
                {
                    vm.CreatedBy = userId;
                    vm.CreatedDate = currentDate;
                }

                //Call the AddBillingSystemParameters Method to Add / Update current BillingSystemParameters
                id = _service.SaveRecord(vm);
            }
            return Json(id);
        }
    }
}