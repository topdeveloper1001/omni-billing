// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoomEquipmentController.cs" company="RoomEquipments">
//   Room Equipment assignment screen
// </copyright>
// <summary>
//   The room equipment controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using BillingSystem.Common;
    using BillingSystem.Model.CustomModel;
    using BillingSystem.Models;
    using BillingSystem.Model;
    using BillingSystem.Bal.Interfaces;

    /// <summary>
    /// The room equipment controller.
    /// </summary>
    public class RoomEquipmentController : Controller
    {
        private readonly IGlobalCodeService _gService;
        private readonly IEquipmentService _eqService;

        public RoomEquipmentController(IGlobalCodeService gService, IEquipmentService eqService)
        {
            _gService = gService;
            _eqService = eqService;
        }


        // GET: /RoomEquipment/
        #region Public Methods and Operators

        /// <summary>
        /// The index.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult Index()
        {
            var facilityEquipmentList = _gService.GetRoomEquipmentALLList(Helpers.GetDefaultFacilityId().ToString());
            var viewtoRetrun = new RoomEquipmentView()
            {
                GCCurrentDataView = new GlobalCodeCustomDModel(),
                GClistView = facilityEquipmentList
            };
            return this.View(viewtoRetrun);
        }

        /// <summary>
        /// Gets the facility equipments.
        /// </summary>
        /// <param name="coporateId">The coporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public ActionResult GetFacilityEquipments(int coporateId, int facilityId)
        {
            var facilityEquipmentList = _eqService.GetEquipmentList(false, facilityId.ToString());
            return PartialView(PartialViews.FacilityEquipmentListView, facilityEquipmentList);
        }

        /// <summary>
        /// Gets the room equipments.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="roomId">The room identifier.</param>
        /// <returns></returns>
        public ActionResult GetRoomEquipments(int facilityId, int roomId)
        {
            var facilityEquipmentList = _gService.GetRoomEquipmentList(facilityId.ToString(), roomId.ToString());
            var selectedObjects = facilityEquipmentList != null
                                      ? facilityEquipmentList.Select(x => x.GlobalCodeValue).ToList()
                                      : new List<string>() { "0" };
            var list = new
            {
                equipments = selectedObjects
            };
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Saves the room equipments.
        /// </summary>
        /// <param name="globalCodeObj">The global code object.</param>
        /// <returns></returns>
        public ActionResult SaveRoomEquipments(List<GlobalCodes> globalCodeObj)
        {
            foreach (var item in globalCodeObj)
            {
                item.CreatedDate = Helpers.GetInvariantCultureDateTime();
                item.CreatedBy = Helpers.GetLoggedInUserId();
            }

            var result = _gService.AddUpdateGlobalCodesList(globalCodeObj);
            //return Json(result, JsonRequestBehavior.AllowGet);
            var facilityEquipmentList = _gService.GetRoomEquipmentALLList(globalCodeObj[0].FacilityNumber);
            return PartialView(PartialViews.RoomEquipmentList, facilityEquipmentList);
        }

        /// <summary>
        /// Deletes the equipment.
        /// </summary>
        /// <param name="globalCodeid">The global codeid.</param>
        /// <param name="facilityNumber">The facility number.</param>
        /// <returns></returns>
        public ActionResult DeleteEquipment(int globalCodeid, string facilityNumber)
        {
            var objToReturn = _gService.DeleteGlobalCode(globalCodeid, facilityNumber);
            return PartialView(PartialViews.RoomEquipmentList, objToReturn);
        }

        #endregion
    }
}