// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HolidayPlannerController.cs" company="SPadez">
//   OmniHealthcare
// </copyright>
// <summary>
//   The holiday planner controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model.CustomModel;
using Microsoft.Ajax.Utilities;

namespace BillingSystem.Controllers
{
    using System.Web.Mvc;
    using Bal.BusinessAccess;
    using BillingSystem.Bal.Interfaces;
    using Common;

    /// <summary>
    /// AppointmentTypes controller.
    /// </summary>
    public class MarFormController : BaseController
    {
        private readonly IOrderActivityService _service;

        public MarFormController(IOrderActivityService service)
        {
            _service = service;
        }
        #region Public Methods and Operators

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Get MAR View detail
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        public ActionResult GetMarView(MarViewCustomModel vm)
        {
            List<MarGroupCustomModel> list;
            var date = vm.FromDate == Convert.ToDateTime("1/1/0001")
                ? CurrentDateTime
                : vm.FromDate;
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            vm.FromDate = firstDayOfMonth;
            vm.TillDate = lastDayOfMonth;
            var lstMarViewCustomModel = _service.GetMARView(vm);

            list = (lstMarViewCustomModel.GroupJoin(lstMarViewCustomModel,
                    c => c.OrderInfo,
                    o => o.OrderStatus,
                    (c, result) => new MarGroupCustomModel(c.OrderInfo, c.OrderStatus, lstMarViewCustomModel))
                    .DistinctBy(x => x.OrderInfo)).ToList();
            return PartialView(PartialViews.MarFormList, list);
        }

        #endregion
    }
}
