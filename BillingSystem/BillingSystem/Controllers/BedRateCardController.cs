using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BillingSystem.Controllers
{
    /// <summary>
    /// The bed rate card controller.
    /// </summary>
    public class BedRateCardController : BaseController
    {
        private readonly IBedRateCardService _service;

        public BedRateCardController(IBedRateCardService service)
        {
            _service = service;
        }

        // GET: /BedRateCard/

        /// <summary>
        /// Beds the rate card.
        /// </summary>
        /// <returns></returns>
        public ActionResult BedRateCard()
        {

            var list = GetBedRateCardCustom();
            var bedRateCardView = new BedRateCardView
            {
                BedRateCardsList = list,
                CurrentBedRateCard = new BedRateCardCustomModel
                {
                    BedRateCard = new BedRateCard()
                }
            };

            return View(bedRateCardView);
        }

        /// <summary>
        /// Adds the update bed rate card.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public ActionResult AddUpdateBedRateCard(BedRateCard model)
        {

            if (model != null)
            {

                if (model.BedRateCardID > 0)
                {
                    model.ModifiedBy = Helpers.GetLoggedInUserId();
                    model.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                    model.CorporateId = Helpers.GetSysAdminCorporateID();
                }
                else
                {
                    model.CreatedBy = Helpers.GetLoggedInUserId();
                    model.CreatedDate = Helpers.GetInvariantCultureDateTime();
                    model.CorporateId = Helpers.GetSysAdminCorporateID();
                }

                var newId = _service.AddUpdateBedRateCard(model);
                return Json(newId);
            }

            return Json(null);
        }

        /// <summary>
        /// Gets the bed rate card.
        /// </summary>
        /// <param name="bedRateCardId">The identifier.</param>
        /// <returns></returns>
        public JsonResult GetBedRateCard(int bedRateCardId)
        {
            var model = _service.GetBedRateCardById(bedRateCardId);
            var jsonResult = new
            {
                model.BedRateCardID,
                model.BedTypes,
                model.CreatedBy,
                model.CreatedDate,
                model.DayStart,
                model.DayEnd,
                model.IsActive,
                model.Rates,
                model.ServiceCodeValue,
                model.UnitType,
                EffectiveFrom = model.EffectiveFrom.GetShortDateString3(),
                EffectiveTill = model.EffectiveTill.GetShortDateString3(),
                model.FacilityId
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// Deletes the bed rate card.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="facilityId"></param>
        /// <returns></returns>
        public ActionResult DeleteBedRateCard(string id)
        {
            var currentBedRateCard = _service.GetBedRateCardById(Convert.ToInt32(id));
            if (currentBedRateCard != null)
            {
                currentBedRateCard.DeletedBy = Helpers.GetLoggedInUserId();
                currentBedRateCard.DeletedDate = Helpers.GetInvariantCultureDateTime();
                currentBedRateCard.IsDeleted = true;
                currentBedRateCard.IsActive = false;
                var result = _service.AddUpdateBedRateCard(currentBedRateCard);
                if (result > 0)
                {
                    var list = GetBedRateCardCustom();
                    return PartialView(PartialViews.BedRateCardList, list);
                }
            }

            return Json(null);

        }

        /// <summary>
        /// Gets the bed rate card custom.
        /// </summary>
        /// <returns></returns>
        public List<BedRateCardCustomModel> GetBedRateCardCustom()
        {
            var list = _service.GetBedRateCardsList(Helpers.DefaultServiceCodeTableNumber);
            return list;
        }

        /// <summary>
        /// Bind all the BedMaster list
        /// </summary>
        /// <returns>
        /// action result with the partial view containing the facility list object
        /// </returns>
        public ActionResult GetBedRateCardList()
        { 
            var bedRateCardList = GetBedRateCardCustom();
             
            return PartialView(PartialViews.BedRateCardList, bedRateCardList);
        }

        /// <summary>
        /// Gets the service code detail by identifier.
        /// </summary>
        /// <param name="serviceCodeValue">The service code value.</param>
        /// <returns></returns>
        public JsonResult GetServiceCodeDetailById(int serviceCodeId)
        {
            using (var bal = new ServiceCodeBal(Helpers.DefaultServiceCodeTableNumber))
            {
                var result = bal.GetServiceCodeById(serviceCodeId);
                return Json(result);
            }
        }


        public JsonResult GetServiceCodesList()
        {
            var list = new List<DropdownListData>();
            using (var sBal = new ServiceCodeBal(Helpers.DefaultServiceCodeTableNumber))
            {
                var sc = sBal.GetServiceCodesCustomModel();
                if (sc.Count > 0)
                {
                    list.AddRange(sc.Select(item => new DropdownListData
                    {
                        Text = string.Format("{0} {1} ({2} - {3})", item.ServiceCodeValue, item.ServiceCodeDescription, item.ServiceCodeEffectiveDate.GetShortDateString1(), item.ServiceExpiryDate.GetShortDateString1()),
                        Value = item.ServiceCodeValue.Trim(),
                        ExternalValue1 = Convert.ToString(item.ServiceCodeId)
                    }));
                    list = list.OrderBy(a => a.Text).ToList();
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetFacilitiesbyCorporate()
        {
            var corporateid = Helpers.GetSysAdminCorporateID();
            var finalList = new List<DropdownListData>();
            var bal = new FacilityBal();
            var list = bal.GetFacilitiesByCorporateId(corporateid);
            if (list.Count > 0)
            {
                var facilityId = Helpers.GetLoggedInUserIsAdmin() ? 0 : Helpers.GetDefaultFacilityId();
                if (facilityId > 0 && corporateid > 0)
                    list = list.Where(f => f.FacilityId == facilityId).ToList();

                finalList.AddRange(list.Select(item => new DropdownListData
                {
                    Text = item.FacilityName,
                    Value = Convert.ToString(item.FacilityId)
                }));
            }
            return Json(finalList);
        }


        [HttpPost]
        public ActionResult BindGlobalCodesDropdownData()
        {
            var categories = new List<string> { "1001", "18", };
            List<DropdownListData> list;
            var corporateid = Helpers.GetSysAdminCorporateID();
            using (var bal = new GlobalCodeBal())
                list = bal.GetListByCategoriesRange(categories);

            //****Bind facility
            //var finalList = new List<DropdownListData>();
            //var fBal = new FacilityBal();
            //var fList = fBal.GetFacilitiesByCorporateId(corporateid);
            //if (list.Count > 0)
            //{
            //    var facilityId = Helpers.GetLoggedInUserIsAdmin() ? 0 : Helpers.GetDefaultFacilityId();
            //    if (facilityId > 0 && corporateid > 0)
            //        fList = fList.Where(f => f.FacilityId == facilityId).ToList();

            //    finalList.AddRange(fList.Select(item => new DropdownListData
            //    {
            //        Text = item.FacilityName,
            //        Value = Convert.ToString(item.FacilityId)
            //    }));
            //}

            /***** End***/


            /****Bind Service Codes****/
            //var listData = new List<DropdownListData>();
            //using (var sBal = new ServiceCodeBal(Helpers.DefaultServiceCodeTableNumber))
            //{
            //    var sc = sBal.GetServiceCodesCustomModel();
            //    if (sc.Count > 0)
            //    {
            //        listData.AddRange(sc.Select(item => new DropdownListData
            //        {
            //            Text = string.Format("{0} {1} ({2} - {3})", item.ServiceCodeValue, item.ServiceCodeDescription, item.ServiceCodeEffectiveDate.GetShortDateString1(), item.ServiceExpiryDate.GetShortDateString1()),
            //            Value = item.ServiceCodeValue.Trim(),
            //            ExternalValue1 = Convert.ToString(item.ServiceCodeId)
            //        }));
            //        listData = listData.OrderBy(a => a.Text).ToList();
            //    }
            //}
            /*****END***/

            var jsonResult = new
            {
                listBedTypes = list.Where(g => g.ExternalValue1.Equals("1001")).ToList(),
                listUnitTypes = list.Where(g => g.ExternalValue1.Equals("18")).ToList(),
                //listServiceCodes = listData
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

    }
}
