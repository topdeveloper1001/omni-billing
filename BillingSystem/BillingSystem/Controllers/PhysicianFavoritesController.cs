using System.Collections.Generic;
using System.Web.Mvc;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Models;

namespace BillingSystem.Controllers
{
    public class PhysicianFavoritesController : BaseController
    {
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
            using (var bal = new FavoritesService())
            {
                var isDeleted = bal.DeleteFav(Id);
                return Json(isDeleted);
            }
        }

        /// <summary>
        /// Gets the physician orders.
        /// </summary>
        /// <param name="physicianId">The physician identifier.</param>
        /// <returns></returns>
        public ActionResult GetPhysicianOrders(int physicianId)
        {
            using (var orderBal = new OpenOrderService(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber))
            {
                var fav = new PhysicianFavoritesView
                {
                    MostRecentOrders = orderBal.GetOrdersByPhysicianId(physicianId),
                    FavoriteOrders = orderBal.GetFavoriteOrders(physicianId),
                    SearchedOrders = new List<OpenOrderCustomModel>()
                };
                return PartialView(PartialViews.PhysicianFavoriteCustom, fav);
            }
        }

        /// <summary>
        /// Gets the physician favorites.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <returns></returns>
        public ActionResult GetPhysicianFavorites(int userid)
        {
            using (var favBal = new FavoritesService(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber))
            {
                //var fav = new PhysicianFavoritesView
                //{
                //    MostRecentOrders = orderBal.GetOrdersByPhysicianId(userid),
                //    FavoriteOrders = orderBal.GetFavoriteOrders(userid),
                //    SearchedOrders = new List<OpenOrderCustomModel>(),
                //};
                var phyFavoriteOrders = favBal.GetFavoriteOrders(userid);
                //return PartialView(PartialViews.PhysicianFavoriteCustom, fav);PhyFavoriteOrders
                return PartialView(PartialViews.PhyFavoriteOrders, phyFavoriteOrders);
            }
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
            using (var bal = new FavoritesService())
            {
                UserDefinedDescriptions model;
                if (id > 0)
                    model = bal.GetFavoriteById(id);
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
                var result = bal.AddToFavorites(model, isFavorite);
                if (screentype == "1")
                    return Json(result);

                var ordersBal = new OpenOrderService(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber);
                var list = ordersBal.GetFavoriteOrders(UserId);
                return PartialView(PartialViews.FavoriteOrders, list);
            }
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
            using (var bal = new FavoritesService())
            {
                return bal.CheckIfAlreadyFav(userid, codeId, categoryId);
            }
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
                using (var bal = new OpenOrderService(Helpers.DefaultCptTableNumber, Helpers.DefaultServiceCodeTableNumber, Helpers.DefaultDrgTableNumber, Helpers.DefaultDrugTableNumber, Helpers.DefaultHcPcsTableNumber, Helpers.DefaultDiagnosisTableNumber))
                    list = bal.GetSearchedOrders(text);
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
            using (var bal = new OpenOrderService())
                list = bal.GetOrdersByPhysician(Helpers.GetLoggedInUserId(),corportaeid,facilityid);
            return PartialView(PartialViews.PhyAllOrders, list);
        }
        #endregion
    }
}