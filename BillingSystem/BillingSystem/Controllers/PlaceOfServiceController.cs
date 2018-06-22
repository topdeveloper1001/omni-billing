using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common;
using BillingSystem.Model.CustomModel;
using System.Web.Mvc;
using System.Linq;
using System;
using BillingSystem.Model;

namespace BillingSystem.Controllers
{
    public class PlaceOfServiceController : BaseController
    {
        private readonly PlaceOfServiceBal _bal;

        public PlaceOfServiceController()
        {
            if (_bal == null)
                _bal = new PlaceOfServiceBal();
        }

        public ActionResult Index()
        {
            var vm = new PlaceOfServiceCustomModel();
            return View(vm);
        }

        public JsonResult GetListByFacility()
        {
            var vm = _bal.GetListByEntity(Helpers.GetDefaultFacilityId(), Helpers.GetDefaultCorporateId());
            var list = vm.Select(x => new[] { Convert.ToString(x.Id), x.Code, x.Name, x.Description, x.EffectiveStartDate.HasValue ? x.EffectiveStartDate.Value.ToString("d") : string.Empty,
                x.EffectiveEndDate.HasValue?x.EffectiveEndDate.Value.ToString("d"):string.Empty, x.CreatedDate.ToString("d") });
            return Json(list);
        }

        public JsonResult GetCurrentPlaceOfService(long id)
        {
            var vm = _bal.GetById(id);
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
                vm.CorporateId
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SavePlaceofService(PlaceOfServiceCustomModel vm)
        {
            vm.IsActive = true;
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

            var result = _bal.SaveRecord(vm);
            if (result > 0)
            {
                var placeOfServices = _bal.GetListByEntity(vm.FacilityId, vm.CorporateId);

                var list = placeOfServices.Select(x => new[] { Convert.ToString(x.Id), x.Code, x.Name
                    , x.Description, x.EffectiveStartDate.HasValue ? x.EffectiveStartDate.Value.ToString("d") : string.Empty,
                x.EffectiveEndDate.HasValue?x.EffectiveEndDate.Value.ToString("d"):string.Empty, x.CreatedDate.ToString("d") });
                var jsonData = new { list, status = result };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);

                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            return Json(new { status = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteRecord(long id)
        {
            var m = _bal.DeleteRecord(id, Helpers.GetLoggedInUserId(), Helpers.GetInvariantCultureDateTime());
            if (m > 0)
            {
                var vm = _bal.GetListByEntity(Helpers.GetDefaultFacilityId(), Helpers.GetDefaultCorporateId());
                var list = vm.Select(x => new[] { Convert.ToString(x.Id), x.Code, x.Name, x.Description, x.EffectiveStartDate.HasValue ? x.EffectiveStartDate.Value.ToString("d") : string.Empty,
                x.EffectiveEndDate.HasValue?x.EffectiveEndDate.Value.ToString("d"):string.Empty, x.CreatedDate.ToString("d") });
                var jsonData = new { list, status = m };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);
        }
    }
}
