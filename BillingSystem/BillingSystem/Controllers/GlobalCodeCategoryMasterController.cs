using BillingSystem.Models;
using System.Web.Mvc;
using BillingSystem.Common;
using BillingSystem.Model;
using BillingSystem.Common.Common;
using System;
using System.Linq;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class GlobalCodeCategoryMasterController : BaseController
    {
        private readonly IGlobalCodeCategoryMasterService _service;
        private readonly IGlobalCodeCategoryService _gcService;

        public GlobalCodeCategoryMasterController(IGlobalCodeCategoryMasterService service, IGlobalCodeCategoryService gcService)
        {
            _service = service;
            _gcService = gcService;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var viewData = new GlobalCodeCategoryMasterView
            {
                GCC = new GlobalCodeCategory
                {
                    IsActive = true,
                    FacilityNumber = Convert.ToString(Helpers.GetDefaultFacilityId())
                },
                //Categories = _service.GetAllGlobalCodeCategories(),
            };

            return View(viewData);
        }


        //Function to get all GlobalCodes List
        /// <summary>
        /// Gets the global code category list.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetGlobalCodeCategoryList()
        {
            return PartialView(PartialViews.GlobalCodeCategoryMasterList, _service.GetAllGlobalCodeCategories());
        }

        //Function to get  GlobalCode for editing
        /// <summary>
        /// Edits the global category code.
        /// </summary>
        /// <param name="GlobalCodeCategoryId">The global code category identifier.</param>
        /// <returns></returns>
        public ActionResult EditGlobalCategoryCode(int GlobalCodeCategoryId)
        {
            //Initialize the GlobalCode Bal
            var cId = Helpers.GetDefaultCorporateId();
            var viewData = new GlobalCodeCategoryMasterView
            {
                GCC = _service.GetGlobalCategoriesByGlobalCodeCategoryId(GlobalCodeCategoryId),
                //LstFacility = facilityBal.GetFacilities(cId)
            };
            return PartialView(PartialViews.AddUpdateGlobalCodeCategoryMaster, viewData);
        }

        //Add Update Global Code
        /// <summary>
        /// Adds the update global code category.
        /// </summary>
        /// <param name="m">The model.</param>
        /// <returns></returns>
        [HttpPost]
        public int AddUpdateGlobalCodeCategory(GlobalCodeCategory m)
        {
            m.FacilityNumber = Convert.ToString(Helpers.GetDefaultFacilityId());
            if (m.GlobalCodeCategoryID > 0)
            {
                m.ModifiedBy = Helpers.GetLoggedInUserId();
                m.ModifiedDate = Helpers.GetInvariantCultureDateTime();
            }
            else
            {
                m.CreatedBy = Helpers.GetLoggedInUserId();
                m.CreatedDate = Helpers.GetInvariantCultureDateTime();
            }
            return _service.AddUpdateGlobalCategory(m);
        }

        //Delete global code
        /// <summary>
        /// Deletes the global code category.
        /// </summary>
        /// <param name="globalCodeCategoryId">The global code category identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteGlobalCodeCategory(int globalCodeCategoryId)
        {
            var m = _service.GetGlobalCategoriesByGlobalCodeCategoryId(globalCodeCategoryId);
            m.IsDeleted = true;
            m.DeletedBy = Helpers.GetLoggedInUserId();
            m.DeletedDate = Helpers.GetInvariantCultureDateTime();
            var i = _service.AddUpdateGlobalCategory(m);

            return PartialView(PartialViews.GlobalCodeCategoryMasterList, _service.GetAllGlobalCodeCategories());
        }

        //Function To reset the User Form
        /// <summary>
        /// Resets the global code category form.
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetGlobalCodeCategoryForm()
        {
            //Initialize the GlobalCode Bal
            //var facilityBal = new FacilityBal();
            //var cId = Helpers.GetSysAdminCorporateID();
            var viewData = new GlobalCodeCategoryMasterView
            {
                GCC = new GlobalCodeCategory { IsActive = true, IsDeleted = false },
                //LstFacility = facilityBal.GetFacilities(cId)
            };
            return PartialView(PartialViews.AddUpdateGlobalCodeCategoryMaster, viewData);
        }

        #region Order Type Category

        //CPT OrderCategory
        /// <summary>
        /// Orders the category.
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderCategory()
        {
            //Initialize the GlobalCode Bal
            var objGlobalCodeCategoryMasterView = new GlobalCodeCategoryMasterView
            {
                GCC = new GlobalCodeCategory { IsActive = true },

                Categories = _service.GetAllGlobalCodeCategoriesByOrderType(OrderType.CPT.ToString(), Convert.ToString(Helpers.GetDefaultFacilityId())),
                //LstFacility = facilityBal.GetFacilities(cId)
            };

            return View(objGlobalCodeCategoryMasterView);
        }

        //Function to get globalcode list for OrderCategory
        public ActionResult GetGlobalCodeCatByExternalValue()
        {
            var list = _gcService.GetGlobalCodeCategoriesByExternalValue("0");
            return Json(list);
        }
        //Function to get all GlobalCodes List
        /// <summary>
        /// Gets the type of the global code category list order.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetGlobalCodeCategoryListOrderType()
        {
            return PartialView(PartialViews.OrderCategoryTypeList, _service.GetAllGlobalCodeCategoriesByOrderType(OrderType.CPT.ToString(), Convert.ToString(Helpers.GetDefaultFacilityId())));
        }


        //Function to get  GlobalCode for editing
        /// <summary>
        /// Edits the type of the global category code order.
        /// </summary>
        /// <param name="GlobalCodeCategoryId">The global code category identifier.</param>
        /// <returns></returns>
        public JsonResult EditGlobalCategoryCodeOrderType(int GlobalCodeCategoryId)
        {
            //Initialize the GlobalCode Bal
            var m = _service.GetGlobalCategoriesByGlobalCodeCategoryId(GlobalCodeCategoryId);
            var jsonData = new { m, SaveText = ResourceKeyValues.GetKeyValue("update") };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        //Delete global code
        /// <summary>
        /// Deletes the type of the global code category order.
        /// </summary>
        /// <param name="globalCodeCategoryId">The global code category identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteGlobalCodeCategoryOrderType(int globalCodeCategoryId)
        {
            var m = _service.GetGlobalCategoriesByGlobalCodeCategoryId(globalCodeCategoryId);
            m.IsDeleted = true;
            m.DeletedBy = Helpers.GetLoggedInUserId();
            m.DeletedDate = Helpers.GetInvariantCultureDateTime();
            var i = _service.AddUpdateGlobalCategory(m);

            return PartialView(PartialViews.OrderCategoryTypeList, _service.GetAllGlobalCodeCategoriesByOrderType(OrderType.CPT.ToString(), Convert.ToString(Helpers.GetDefaultFacilityId())));
        }

        //Function To reset the User Form
        /// <summary>
        /// Resets the type of the global code category form order.
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetGlobalCodeCategoryFormOrderType()
        {
            //Initialize the GlobalCode Bal
            var cId = Helpers.GetDefaultCorporateId();
            var objGlobalCodeCategoryMasterView = new GlobalCodeCategoryMasterView
            {
                GCC = new GlobalCodeCategory(),
                //LstFacility = facilityBal.GetFacilities(cId)
            };
            return PartialView(PartialViews.AddUpdateOrderCategoryType, objGlobalCodeCategoryMasterView);
        }
        //CPT 
        #endregion

        public JsonResult BindOrderTypeCategories()
        {
            var list = _service.GetOrderTypeCategoriesByFacility(Helpers.GetDefaultFacilityId(), Helpers.GetLoggedInUserId(), true);

            var jsonData = list.Select(a => new[] { Convert.ToString(a.GlobalCodeCategoryID),a.GlobalCodeCategoryName
                    , a.GlobalCodeCategoryValue, a.ExternalValue1, a.ExternalValue2, a.ExternalValue4,a.ExternalValue3  });

            var s = Json(jsonData, JsonRequestBehavior.AllowGet);
            s.MaxJsonLength = int.MaxValue;
            s.RecursionLimit = int.MaxValue;
            return s;
        }

    }
}