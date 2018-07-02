using System.Collections.Generic;
using System.Web.Mvc;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Models;

namespace BillingSystem.Controllers
{
    public class PhysicianFavoritesController : BaseController
    {
        private readonly IOpenOrderService _ooService;
        private readonly IFavoritesService _service;

        public PhysicianFavoritesController(IOpenOrderService ooService, IFavoritesService service)
        {
            _ooService = ooService;
            _service = service;
        }

        //
        // GET: /PhysicianFavorites/
        /// <summary>
        /// Favoriteses this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Favorites()
        {
            var fav = new PhysicianFavoritesView
            {
                MostRecentOrders = new List<OpenOrderCustomModel>(),
                FavoriteOrders = new List<OpenOrderCustomModel>(),
                SearchedOrders = new List<OpenOrderCustomModel>(),
                PhyFavoriteOrders = new List<FavoritesCustomModel>()
            };
            return View(fav);

        }

        /// <summary>
        /// Deletes the fav.
        /// </summary>
        /// <param name="Id">The identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteFav(int Id)
        {
            var isDeleted = _service.DeleteFav(Id);
            return Json(isDeleted);
        }

        /// <summary>
        /// Gets the physician orders.
        /// </summary>
        /// <param name="physicianId">The physician identifier.</param>
        /// <returns></returns>
        public ActionResult GetPhysicianOrders(int physicianId)
        {
            var fav = new PhysicianFavoritesView
            {
                MostRecentOrders = _ooService.GetOrdersByPhysicianId(physicianId, Helpers.DefaultCptTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDiagnosisTableNumber),
                FavoriteOrders = _ooService.GetFavoriteOrders(physicianId, Helpers.DefaultCptTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDiagnosisTableNumber),
                SearchedOrders = new List<OpenOrderCustomModel>()
            };
            return PartialView(PartialViews.PhysicianFavoriteCustom, fav);
        }

        /// <summary>
        /// Gets the physician favorites.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <returns></returns>
        public ActionResult GetPhysicianFavorites(int userid)
        {
            var phyFavoriteOrders = _service.GetFavoriteOrders(userid, Helpers.DefaultCptTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDiagnosisTableNumber);
            return PartialView(PartialViews.PhyFavoriteOrders, phyFavoriteOrders);
        }

        /// <summary>
        /// Adds to phy favorites.
        /// </summary>
        /// <param name="codeId">The code identifier.</param>
        /// <param name="categoryId">The category identifier.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="isFavorite">if set to <c>true</c> [is favorite].</param>
        /// <param name="favoriteDesc">The favorite desc.</param>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="screentype">The screentype.</param>
        /// <returns></returns>
        public ActionResult AddToPhyFavorites(string codeId, string categoryId, int id, bool isFavorite, string favoriteDesc, int UserId, string screentype)
        {
                UserDefinedDescriptions model;
                if (id > 0)
                    model = _service.GetFavoriteById(id);
                else
                {
                    if (IsAlreadyFav(UserId, codeId, categoryId))
                    {
                        return Json(1);
                    }
                    model = new UserDefinedDescriptions { CategoryId = categoryId, CodeId = codeId };
                }

                model.UserDefineDescription = favoriteDesc;
                model.UserID = UserId;
                model.RoleID = Helpers.GetDefaultRoleId();
                if (model.UserDefinedDescriptionID > 0)
                {
                    model.ModifiedBy = UserId;
                    model.Modifieddate = Helpers.GetInvariantCultureDateTime();
                }
                else
                {
                    model.CreatedBy = UserId;
                    model.CreatedDate = Helpers.GetInvariantCultureDateTime();
                }
                var result = _service.AddToFavorites(model, isFavorite);
                if (screentype == "1")
                    return Json(result);

                var list = _ooService.GetFavoriteOrders(UserId, Helpers.DefaultCptTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDiagnosisTableNumber);
                return PartialView(PartialViews.FavoriteOrders, list);
        }

        /// <summary>
        /// Determines whether [is already fav] [the specified userid].
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <param name="codeId">The code identifier.</param>
        /// <param name="categoryId">The category identifier.</param>
        /// <returns></returns>
        public bool IsAlreadyFav(int userid, string codeId, string categoryId)
        { 
                return _service.CheckIfAlreadyFav(userid, codeId, categoryId);
             
        }

        /// <summary>
        /// Gets the searched orders.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public ActionResult GetSearchedOrders(string text)
        {
            var list = new List<OpenOrderCustomModel>();
            if (!string.IsNullOrEmpty(text))
            {
                    list = _ooService.GetSearchedOrders(text, Helpers.DefaultCptTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDiagnosisTableNumber);
            }
            return PartialView(PartialViews.OpenOrdersInSearch, list);
            // return Json(list);
        }

        #region Common To Summary and Physician Favorites View
        /// <summary>
        /// Gets the physician all orders.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetPhysicianAllOrders()
        {
            List<OpenOrderCustomModel> list;
            var corportaeid = Helpers.GetSysAdminCorporateID();
            var facilityid = Helpers.GetDefaultFacilityId();
                list = _ooService.GetOrdersByPhysician(Helpers.GetLoggedInUserId(), corportaeid, facilityid);
            return PartialView(PartialViews.PhyAllOrders, list);
        }
        #endregion
    }
}