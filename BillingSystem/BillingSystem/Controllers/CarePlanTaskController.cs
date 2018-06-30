using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Models;

namespace BillingSystem.Controllers
{
    /// <summary>
    /// CarePlanTask controller.
    /// </summary>
    public class CarePlanTaskController : BaseController
    {
        private readonly ICarePlanTaskService _service;

        public CarePlanTaskController(ICarePlanTaskService service)
        {
            _service = service;
        }
        #region Public Methods and Operators

        /// <summary>
        ///     Bind all the CarePlanTask list
        /// </summary>
        /// <returns>action result with the partial view containing the CarePlanTask list object</returns>
        [HttpPost]
        public ActionResult BindCarePlanTaskList(int val, string sort, string sortdir)
        {
            int corporateId = Helpers.GetSysAdminCorporateID();
            int facilityId = Helpers.GetDefaultFacilityId();

            // Get the facilities list
            // var carePlanTaskList = carePlanTaskBal.GetCarePlanTask();
            var carePlanTaskList = _service.GetActiveCarePlanTask(corporateId, facilityId, Convert.ToBoolean(val));
            if (carePlanTaskList.Count > 0)
            {
                carePlanTaskList = carePlanTaskList.OrderBy(string.Format("{0} {1}", sort, sortdir)).ToList();
            }
            // Pass the ActionResult with List of CarePlanTaskViewModel object to Partial View CarePlanTaskList
            return this.PartialView(PartialViews.CarePlanTaskList, carePlanTaskList);

        }

        /// <summary>
        /// Delete the current CarePlanTask based on the CarePlanTask ID passed in the CarePlanTaskModel
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="inActive"></param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult DeleteCarePlanTask(int id, bool inActive)
        {
            // Get CarePlanTask model object by current CarePlanTask ID
            var currentCarePlanTask = _service.GetCarePlanTaskById(id);
            var userId = Helpers.GetLoggedInUserId();

            // Check If CarePlanTask model is not null
            if (currentCarePlanTask != null)
            {
                currentCarePlanTask.IsActive = false;

                currentCarePlanTask.ModifiedBy = userId;
                currentCarePlanTask.ModifiedDate = Helpers.GetInvariantCultureDateTime();

                // Update Operation of current CarePlanTask
                int result = inActive
                    ? _service.SaveCarePlanTask(currentCarePlanTask, inActive ? 1 : 0)
                    : _service.DeleteCarePlanTask(currentCarePlanTask);

                // return deleted ID of current CarePlanTask as Json Result to the Ajax Call.
                return Json(result);
            }


            // Return the Json result as Action Result back JSON Call Success
            return this.Json(null);
        }

        /// <summary>
        /// Get the details of the CarePlanTask View in the Model CarePlanTask such as CarePlanTaskList, list of
        ///     countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model CarePlanTask to be passed to View
        ///     CarePlanTask
        /// </returns>
        public ActionResult Index()
        {
            int corporateId = Helpers.GetSysAdminCorporateID();
            int facilityId = Helpers.GetDefaultFacilityId();

            // Get the Entity list
            // var carePlanTaskList = carePlanTaskBal.GetCarePlanTask();
            var carePlanTaskList = _service.GetActiveCarePlanTask(corporateId, facilityId, true);

            // Intialize the View Model i.e. CarePlanTaskView which is binded to Main View Index.cshtml under CarePlanTask
            var carePlanTaskView = new CarePlanTaskView
            {
                CarePlanTaskList = carePlanTaskList,
                CurrentCarePlanTask = new CarePlanTask()
            };

            // Pass the View Model in ActionResult to View CarePlanTask
            return View(carePlanTaskView);
        }

        /// <summary>
        /// Reset the CarePlanTask View Model and pass it to CarePlanTaskAddEdit Partial View.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult ResetCarePlanTaskForm()
        {
            // Intialize the new object of CarePlanTask ViewModel
            var carePlanTaskViewModel = new CarePlanTask();

            // Pass the View Model as CarePlanTaskViewModel to PartialView CarePlanTaskAddEdit just to update the AddEdit partial view.
            return this.PartialView(PartialViews.CarePlanTaskAddEdit, carePlanTaskViewModel);
        }

        /// <summary>
        /// Add New or Update the CarePlanTask based on if we pass the CarePlanTask ID in the CarePlanTaskViewModel
        ///     object.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// returns the newly added or updated ID of CarePlanTask row
        /// </returns>
        public ActionResult SaveCarePlanTask(CarePlanTask model)
        {
            // Initialize the newId variable 
            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            var dateTime = Helpers.GetInvariantCultureDateTime();
            int newId = -1;
            int userId = Helpers.GetLoggedInUserId();
            var check = _service.CheckDuplicateTaskNumber(model.Id, model.TaskNumber, facilityId, corporateId);
            if (check)
            {
                return Json("-1");
            }
            else
            {
                if (model.Id > 0)
                {
                    model.ModifiedBy = userId;
                    model.ModifiedDate = dateTime;
                    model.FacilityId = facilityId;
                    model.CorporateId = corporateId;
                }
                else
                {
                    model.CreatedBy = userId;
                    model.CreatedDate = dateTime;
                    model.FacilityId = facilityId;
                    model.CorporateId = corporateId;
                    model.CreatedDate = dateTime;
                }

                // Call the AddCarePlanTask Method to Add / Update current CarePlanTask
                newId = _service.SaveCarePlanTask(model, 2);

            }

            // Check if Model is not null 
            return this.Json(newId);
        }

        /// <summary>
        /// Gets the care plan task data.
        /// </summary>
        /// <param name="id">
        /// The identifier.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult GetCarePlanTaskData(int id)
        {
            var currentCarePlanTask = _service.GetCarePlanTaskById(id);
            var jsonResult =
                new
                {
                    currentCarePlanTask.Id,
                    currentCarePlanTask.CorporateId,
                    currentCarePlanTask.FacilityId,
                    currentCarePlanTask.ActivityType,
                    currentCarePlanTask.EndTime,
                    currentCarePlanTask.StartTime,
                    currentCarePlanTask.IsRecurring,
                    currentCarePlanTask.RecTImeIntervalType,
                    currentCarePlanTask.RecTimeInterval,
                    currentCarePlanTask.ResponsibleUserType,
                    currentCarePlanTask.TaskDescription,
                    currentCarePlanTask.TaskNumber,
                    currentCarePlanTask.IsActive,
                    currentCarePlanTask.RecurranceType,
                    currentCarePlanTask.CarePlanId,
                    currentCarePlanTask.TaskName,
                    currentCarePlanTask.ExtValue1,
                    currentCarePlanTask.ExtValue2
                };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Gets the maximum task number.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult GetMaxTaskNumber()
        {
            int corporateId = Helpers.GetSysAdminCorporateID();
            int facilityId = Helpers.GetDefaultFacilityId();

            var list = _service.GetMaxTaskNumber(corporateId, facilityId);
            return Json(list);

        }

        /// <summary>
        /// Binds the type of the users.
        /// </summary>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        public JsonResult BindUsersType()
        {
            using (var fRole = new FacilityRoleService())
            {
                var list = new List<DropdownListData>();
                var corporateId = Helpers.GetSysAdminCorporateID();

                var facilityId = Helpers.GetDefaultFacilityId();
                var roleList = fRole.GetUserTypeRoleDropDownInTaskPlan(corporateId, facilityId, true);
                if (roleList.Count > 0)
                {
                    list.AddRange(
                        roleList.Select(
                            item =>
                            new DropdownListData
                            {
                                Text = string.Format("{0}", item.RoleName),
                                Value = Convert.ToString(item.RoleId)
                            }));
                }

                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Gets the roles by facility dropdown data.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult GetRolesByFacilityDropdownData()
        {
            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();
            using (var roleBal = new RoleService())
            {
                var roles = roleBal.GetRolesByCorporateIdFacilityId(
                    Convert.ToInt32(corporateId),
                    Convert.ToInt32(facilityId));
                if (roles.Count > 0)
                {
                    var list = new List<SelectListItem>();
                    list.AddRange(
                        roles.Select(
                            item => new SelectListItem { Text = item.RoleName, Value = item.RoleID.ToString() }));
                    list = list.OrderBy(x => x.Text).ToList();
                    return Json(list);
                }
            }

            return Json(0);
        }

        /// <summary>
        /// Binds the care plan dropdown.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult BindCarePlanDropdown()
        {
            var corporateId = Helpers.GetSysAdminCorporateID();
            var facilityId = Helpers.GetDefaultFacilityId();

            var carePaln = _service.BindCarePlan(corporateId, facilityId);

            if (carePaln.Count > 0)
            {
                var list = new List<SelectListItem>();
                list.AddRange(
                    carePaln.Select(
                        item =>
                        new SelectListItem
                        {
                            Text = string.Format("{0} - {1}", item.PlanNumber, item.Name),
                            Value = item.Id.ToString()
                        }));
                return Json(list);
            }


            return Json(0);
        }

        #endregion
    }
}