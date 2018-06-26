
using System.Collections.Generic;
using System.Web.Mvc;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common;
using BillingSystem.Model.CustomModel;
using BillingSystem.Models;
namespace BillingSystem.Controllers
{

    /// <summary>
    /// The faculty dept controller.
    /// </summary>
    public class FacultyDeptController : Controller
    {
        private readonly IFacilityStructureService _fsService;

        public FacultyDeptController(IFacilityStructureService fsService)
        {
            _fsService = fsService;
        }

        // GET: /FacultyDept/
        #region Public Methods and Operators

        /// <summary>
        /// The index.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult Index()
        {
            var viewtoRetrun = new RoomEquipmentView()
            {
                GCCurrentDataView = new GlobalCodeCustomDModel(),
                GClistView = new List<GlobalCodeCustomDModel>()
            };
            return this.View(viewtoRetrun);
        }

        /// <summary>
        /// Gets the facility departments.
        /// </summary>
        /// <param name="coporateId">The coporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public ActionResult GetFacilityDepartments(int coporateId, int facilityId)
        {
            var lst = _fsService.GetFacilityDepartments(coporateId, facilityId.ToString());
            return PartialView(PartialViews.FacilityDepartmentListView, lst);

        }

        #endregion
    }
}