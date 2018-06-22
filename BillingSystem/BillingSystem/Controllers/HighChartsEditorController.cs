using BillingSystem.Bal.BusinessAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BillingSystem.Controllers
{
    public class HighChartsEditorController : Controller
    {
        // GET: HighChartsEditor
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetMDashboardList()
        {
            using(var bal = new ManualDashboardBal())
            {
                var list = bal.GetManualDashboardList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }
    }
}