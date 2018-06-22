// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PatientSchedulerController.cs" company="Spadez">
//   OmniHealthcare 
// </copyright>
// <summary>
//   The patient scheduler controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using BillingSystem.Bal.BusinessAccess;
    using BillingSystem.Common;
    using BillingSystem.Common.Common;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;
    using BillingSystem.Models;

    /// <summary>
    /// The patient scheduler controller.
    /// </summary>
    public class PatientSchedulerController : BaseController
    {
        #region Public Methods and Operators

        /// <summary>
        /// Method to get the data for calendar binding
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns>
        /// The <see cref="JsonResult" />.
        /// </returns>
        public JsonResult BindUsersType(string corporateId, string facilityId)
        {
            using (var fRole = new FacilityRoleBal())
            {
                var list = new List<DropdownListData>();
                List<FacilityRoleCustomModel> roleList = fRole.GetUserTypeRoleDropDown(
                    Convert.ToInt32(corporateId), 
                    Convert.ToInt32(facilityId), 
                    true);
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

                return this.Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Indexes the specified pid.
        /// </summary>
        /// <param name="pid">The pid.</param>
        /// <returns>
        /// The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult Index(string pid)
        {
            int patientId = 0;
            if (string.IsNullOrEmpty(pid) && !int.TryParse(pid, out patientId) && patientId == 0)
            {
                return this.RedirectToAction(
                    ActionResults.patientSearch, 
                    ControllerNames.patientSearch, 
                    new { messageId = Convert.ToInt32(MessageType.PatientSchedular) });
            }

            var objPatientSchedulerView = new PatientSchedulerView { ObjScheduling = new Scheduling(), PatientId = pid };

            return View(objPatientSchedulerView);
        }

        /// <summary>
        /// The sample.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult Sample()
        {
            return this.View();
        }

        /// <summary>
        /// The save patient scheduling.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// The <see cref="JsonResult" />.
        /// </returns>
        public JsonResult SavePatientScheduling(List<Scheduling> model)
        {
            var list = new SchedulingCustomModelView();
            if (model != null)
            {
                var corporateId = Helpers.GetSysAdminCorporateID();
                using (var oSchedulingBal = new SchedulingBal())
                {
                    foreach (Scheduling item in model)
                    {
                        item.CorporateId = corporateId;
                        item.CreatedBy = Helpers.GetLoggedInUserId();
                        item.CreatedDate = Helpers.GetInvariantCultureDateTime();
                        item.WeekDay = Helpers.GetWeekOfYearISO8601(Convert.ToDateTime(item.ScheduleFrom)).ToString();
                        item.IsActive = true;
                    }

                    //SchedulingCustomModelView updatedList = oSchedulingBal.SavePatientScheduling(model);
                }
            }

            return this.Json(list, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}