using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using System.Web.Mvc;
using System.Linq;
using System;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class BillingModifierController : BaseController
    {
        private readonly IBillingModifierService _service;

        public BillingModifierController(IBillingModifierService service)
        {
            _service = service;
        }

        public ActionResult Index()
        {
            var defaultStartDate = new DateTime(DateTime.Now.Year, 1, 1);
            var defaultEndDate = new DateTime(DateTime.Now.Year, 12, 31);

            var vm = new BillingModifierCustomModel
            {
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                EffectiveEndDate = defaultEndDate,
                EffectiveStartDate = defaultStartDate
            };
            return View(vm);
        }

        public JsonResult GetListByFacility()
        {
            var vm = _service.GetListByEntity(Helpers.GetDefaultFacilityId(), Helpers.GetDefaultCorporateId());
            var list = vm.Select(x => new[] { Convert.ToString(x.Id), x.Code, x.Name, x.Description, x.EffectiveStartDate.HasValue ? x.EffectiveStartDate.Value.ToString("d") : string.Empty,
                x.EffectiveEndDate.HasValue ? x.EffectiveEndDate.Value.ToString("d") : string.Empty,x.Type,x.IsFirst.ToString() ,x.CreatedDate.ToString("d"), });

            return Json(list);
        }

        public JsonResult GetCurrentBillingModifier(long id)
        {
            var vm = _service.GetById(id);
            var jsonResult = new
            {
                vm.Id,
                vm.Code,
                vm.Name,
                vm.Description,
                EffectiveStartDate = vm.EffectiveStartDate.GetShortDateString1(),
                EffectiveEndDate = vm.EffectiveEndDate.GetShortDateString1(),
                vm.ExtValue1,
                vm.FacilityId,
                vm.IsActive,
                vm.IsFirst,
                vm.Type,
                vm.CorporateId
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveBillingModifier(BillingModifierCustomModel vm)
        {
            if (vm.Id > 0)
            {
                vm.ModifiedBy = Helpers.GetLoggedInUserId();
                vm.ModifiedDate = Helpers.GetInvariantCultureDateTime();
            }
            else
            {
                vm.CreatedBy = Helpers.GetLoggedInUserId();
                vm.CreatedDate = Helpers.GetInvariantCultureDateTime();
            }

            vm.FacilityId = Helpers.GetDefaultFacilityId();
            vm.CorporateId = Helpers.GetDefaultCorporateId();
            vm.IsActive = true;
            var result = _service.SaveRecord(vm);
            if (result > 0)
            {
                var vmList = _service.GetListByEntity(vm.FacilityId, vm.CorporateId);
                var list = vmList.Select(x => new[] { Convert.ToString(x.Id), x.Code, x.Name, x.Description, x.EffectiveStartDate.HasValue ? x.EffectiveStartDate.Value.ToString("d") : string.Empty,
                x.EffectiveEndDate.HasValue?x.EffectiveEndDate.Value.ToString("d"):string.Empty,x.Type,x.IsFirst.ToString() ,x.CreatedDate.ToString("d"), });
                var jsonData = new { list, status = result };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);

                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            return Json(new { status = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteRecord(long id)
        {
            var m = _service.DeleteRecord(id, Helpers.GetLoggedInUserId(), Helpers.GetInvariantCultureDateTime());
            if (m > 0)
            {
                var vm = _service.GetListByEntity(Helpers.GetDefaultFacilityId(), Helpers.GetDefaultCorporateId());
                var list = vm.Select(x => new[] {
                    Convert.ToString(x.Id), x.Code, x.Name, x.Description, x.EffectiveStartDate.HasValue ? x.EffectiveStartDate.Value.ToString("d") : string.Empty,
                x.EffectiveEndDate.HasValue ? x.EffectiveEndDate.Value.ToString("d"):string.Empty
                ,x.Type,x.IsFirst.ToString() ,x.CreatedDate.ToString("d"), });

                var jsonData = new { list, status = m };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return jsonResult;
            }
            return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);
        }
    }
}

