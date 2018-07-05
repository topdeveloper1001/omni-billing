using System.Collections.Generic;
using System.Web.Mvc;
using BillingSystem.Model.CustomModel;
using BillingSystem.Models;
using BillingSystem.Model;
using BillingSystem.Common;
using System.Linq;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class GlobalCodeCategoryController : BaseController
    {
        private readonly ICPTCodesService _cptService;
        private readonly IGlobalCodeCategoryService _service;
        private readonly IGlobalCodeCategoryMasterService _gmService;
        private readonly IFacilityService _fService;

        public GlobalCodeCategoryController(ICPTCodesService cptService, IGlobalCodeCategoryService service, IGlobalCodeCategoryMasterService gmService, IFacilityService fService)
        {
            _cptService = cptService;
            _service = service;
            _gmService = gmService;
            _fService = fService;
        }

        //
        // GET: /GlobalCodeCategory/
        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var globalCodeCategories = _service.GetGlobalCodeCategoriesWithFacilityName();
            List<Facility> facilities;
            var cId = Helpers.GetDefaultCorporateId();
            facilities = _fService.GetFacilities(cId);
            var gModel = new GlobalCodeCategoryView
            {
                GlobalCodeCategoryList = globalCodeCategories,
                FacilityList = facilities,
                SelectedGlobalCodeCategoryOptions = new List<GlobalCodeCategory>(),
                FacilityNumber = "0",
                SelectedSourceGlobalCodeCategoryOptions = new List<GlobalCodeCategory>()
            };
            return View(gModel);
        }

        /// <summary>
        /// Add / Edit the Current Global Code Category into the database
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns>
        /// ID of newly added / updated GlobalCodeCategory
        /// </returns>
        public ActionResult SaveCategory(List<GlobalCodeCategory> list)
        {
            if (list != null)
            {
                foreach (var item in list)
                {
                    item.FacilityNumber = item.FacilityNumber;
                    var newId = _service.AddUpdateGlobalCodeCategory(item);
                    return Json(newId);
                }
            }
            return Json(null);
        }

        /// <summary>
        /// Gets the global code categories.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetGlobalCodeCategories()
        {
            var globalCodeCategories = _service.GetGlobalCodeCategories();
            return Json(globalCodeCategories);
        }

        /// <summary>
        /// Gets the global code categories by null facility.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetGlobalCodeCategoriesByNullFacility()
        {
            var globalCodeCategories = _service.GetGlobalCodeCategoriesByNullFacility();
            return Json(globalCodeCategories);
        }

        /// <summary>
        /// Gets the facility global code categories.
        /// </summary>
        /// <param name="FacilityNumber">The facility number.</param>
        /// <returns></returns>
        public ActionResult GetFacilityGlobalCodeCategories(string FacilityNumber)
        {
            var globalCodeCategories = _service.GetFacilityGlobalCodeCategories(FacilityNumber);
            return Json(globalCodeCategories);
        }

        /// <summary>
        /// Gets the type of the facility global code categories by order.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetFacilityGlobalCodeCategoriesByOrderType()
        {
            string orderType = Common.Common.OrderType.CPT.ToString();
            var globalCodeCategories = _gmService.GetAllGlobalCodeCategoriesByOrderType(orderType);
            return Json(globalCodeCategories);
        }

        #region Lab Test Order Set
        /// <summary>
        /// Labs the test order set view.
        /// </summary>
        /// <param name="categoryValue">The category value.</param>
        /// <returns></returns>
        public ActionResult LabTestOrderSetView(string categoryValue)
        {
            if (!string.IsNullOrEmpty(categoryValue))
            {
                var list = _service.GetListByCategoryValue(categoryValue);
                var view = new LabTestOrderSetView
                {
                    //CurrentGlobalCodeCategory = new GlobalCodeCategory { IsActive = true, IsDeleted = false },
                    //GCCategoryList = list,
                    //CategoryValue = categoryValue,
                    //CurrentLabOrderCode = new GlobalCodeModel { IsActive = true, IsDeleted = false },
                    //LabOrderCodesList = new List<GlobalCodeModel>()
                    CurrentGlobalCodeCategory = new GlobalCodeModel { IsActive = true, IsDeleted = false },
                    GCCategoryList = new List<GlobalCodeModel>(),
                    CategoryValue = categoryValue,

                };
                return View("LabTestOrderSetView", view);
            }

            return RedirectToAction("PatientSearch", "PatientSearch");
        }

        /// <summary>
        /// Gets the global code categories.
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRecordById(int id)
        {
            var current = _service.GetCurrentCategoryById(id);
            var jsonResult = new
            {
                current.GlobalCodeCategoryID,
                current.GlobalCodeCategoryName,
                current.GlobalCodeCategoryValue,
                current.ExternalValue1
            };
            return Json(jsonResult);
        }


        public JsonResult CheckDuplicateRecord(GlobalCodeCategory model)
        {
            var isExists = _service.CheckDuplicateCode(model);
            return Json(isExists);
        }

        /// <summary>
        /// Add / Edit the Current Global Code Category into the database
        /// </summary>
        /// <param name="model"></param>
        /// <returns>
        /// ID of newly added / updated GlobalCodeCategory
        /// </returns>
        public ActionResult SaveRecord(GlobalCodeCategory model)
        {
            var userId = Helpers.GetLoggedInUserId();

            var currentDateTime = _fService.GetInvariantCultureDateTime(Helpers.GetDefaultFacilityId());

            if (model.GlobalCodeCategoryID > 0)
            {
                model.CreatedBy = userId;
                model.CreatedDate = currentDateTime;
                model.ModifiedBy = userId;
                model.ModifiedDate = currentDateTime;
            }
            else
            {
                model.CreatedBy = userId;
                model.CreatedDate = currentDateTime;
            }
            _service.AddUpdateGlobalCodeCategory(model);
            var list = _service.GetListByCategoryValue(model.ExternalValue1);
            return PartialView(PartialViews.LabTestOrderSetList, list);
        }

        /// <summary>
        /// Add / Edit the Current Global Code Category into the database
        /// </summary>
        /// <param name="gccId"></param>
        /// <returns>
        /// ID of newly added / updated GlobalCodeCategory
        /// </returns>
        public ActionResult DeleteRecord(int gccId)
        {
            var userId = Helpers.GetLoggedInUserId();
            var currentDateTime = Helpers.GetInvariantCultureDateTime();
            var model = _service.GetCurrentCategoryById(gccId);
            if (model.GlobalCodeCategoryID > 0)
            {
                model.DeletedBy = userId;
                model.DeletedDate = currentDateTime;
                model.IsDeleted = true;
                _service.AddUpdateGlobalCodeCategory(model);
            }

            var list = _service.GetListByCategoryValue(model.ExternalValue1);
            return PartialView(PartialViews.LabTestOrderSetList, list);
        }

        public ActionResult BindCptCodesForLabOrderSet()
        {
            var list = new List<DropdownListData>();

            var cptCodesList = _cptService.GetCodesByRange(80047, 89356, Helpers.DefaultCptTableNumber);
            list.AddRange(cptCodesList.Select(item => new DropdownListData
            {
                Text = string.Format("{0} - {1}", item.CodeNumbering, item.CodeDescription),
                Value = item.CodeNumbering,
                ExternalValue1 = item.CodeDescription,
                SortOrder = int.Parse(item.CodeNumbering)
            }));

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckIfCptCodeExistsInRange(string value)
        {
            var status = _cptService.CheckIfCptCodeExistsInRange(value, 80047, 89356, Helpers.DefaultCptTableNumber);
            return Json(status);

        }


        /// <summary>
        /// Add / Edit the Current Lab Order Set into the database
        /// </summary>
        /// <param name="gccModel"></param>
        /// <param name="gcList"></param>
        /// <returns>
        /// ID of newly added / updated GlobalCodeCategory
        /// </returns>
        public ActionResult SaveLabOrderSet(GlobalCodeCategory gccModel, List<GlobalCodes> gcList)
        {
            var userId = Helpers.GetLoggedInUserId();
            var currentDateTime = Helpers.GetInvariantCultureDateTime();

            if (gccModel.GlobalCodeCategoryID > 0)
            {
                gccModel.CreatedBy = userId;
                gccModel.CreatedDate = currentDateTime;
                gccModel.ModifiedBy = userId;
                gccModel.ModifiedDate = currentDateTime;
            }
            else
            {
                gccModel.CreatedBy = userId;
                gccModel.CreatedDate = currentDateTime;
            }
            _service.AddUpdateGlobalCodeCategory(gccModel);
            var list = _service.GetListByCategoryValue(gccModel.ExternalValue1);
            return PartialView(PartialViews.LabTestOrderSetList, list);
        }
        #endregion

        #region Lab Test Order Set Version 1
        public ActionResult LabTestOrderSetView1(string categoryValue)
        {
            if (!string.IsNullOrEmpty(categoryValue))
            {
                var list = _service.GetListByCategoryValue(categoryValue);
                var view = new LabTestOrderSetView
                {
                    CurrentGlobalCodeCategory1 = new GlobalCodeCategory { IsActive = true, IsDeleted = false },
                    GCCategoryList1 = list,
                    CategoryValue = categoryValue,
                    CurrentLabOrderCode = new GlobalCodes { IsActive = true, IsDeleted = false },
                    LabOrderCodesList = new List<GlobalCodes>()
                };
                return View("LabTestOrderSetView1", view);
            }

            return RedirectToAction("PatientSearch", "PatientSearch");
        }

        /// <summary>
        /// Add / Edit the Current Global Code Category into the database
        /// </summary>
        /// <param name="model"></param>
        /// <returns>
        /// ID of newly added / updated GlobalCodeCategory
        /// </returns>
        public ActionResult SaveRecord1(GlobalCodeCategory model)
        {
            var userId = Helpers.GetLoggedInUserId();
            var currentDateTime = Helpers.GetInvariantCultureDateTime();

            if (model.GlobalCodeCategoryID > 0)
            {
                model.CreatedBy = userId;
                model.CreatedDate = currentDateTime;
                model.ModifiedBy = userId;
                model.ModifiedDate = currentDateTime;
            }
            else
            {
                model.CreatedBy = userId;
                model.CreatedDate = currentDateTime;
            }
            _service.AddUpdateGlobalCodeCategory(model);
            var list = _service.GetListByCategoryValue(model.ExternalValue1);
            return PartialView(PartialViews.LabTestOrderSetList, list);
        }

        /// <summary>
        /// Add / Edit the Current Global Code Category into the database
        /// </summary>
        /// <param name="gccId"></param>
        /// <returns>
        /// ID of newly added / updated GlobalCodeCategory
        /// </returns>
        public ActionResult DeleteRecord1(int gccId)
        {
            var userId = Helpers.GetLoggedInUserId();
            var currentDateTime = Helpers.GetInvariantCultureDateTime();
            var model = _service.GetCurrentCategoryById(gccId);
            if (model.GlobalCodeCategoryID > 0)
            {
                model.DeletedBy = userId;
                model.DeletedDate = currentDateTime;
                model.IsDeleted = true;
                _service.AddUpdateGlobalCodeCategory(model);
            }

            var list = _service.GetListByCategoryValue(model.ExternalValue1);
            return PartialView(PartialViews.LabTestOrderSetList, list);
        }
        #endregion

        public ActionResult GetGlobalCodeCatByExternalValue(string startRange, string endRange)
        {
            var list = _service.GetGlobalCodeCategoriesByExternalValue();
            return Json(list);
        }
    }
}
