// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BedRateCardController.cs" company="Spadez Solutions PVT. LTD.">
//  ServicesDotCom 
// </copyright>
// <summary>
//   The bed rate card controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Bal.BusinessAccess;
    using Common;
    using Model;
    using Model.CustomModel;
    using Models;

    /// <summary>
    /// The bed rate card controller.
    /// </summary>
    public class BedRateCardController : BaseController
    {
        // GET: /BedRateCard/

        /// <summary>
        /// Beds the rate card.
        /// </summary>
        /// <returns></returns>
        public ActionResult BedRateCard()
        {
            //var bedRateCardList = new List<BedRateCardCustomModel>();
            //using (var bal = new BedRateCardBal())
            //{
            //    //var list = bal.GetBedRateCardsList();
            //    //List<ServiceCodeCustomModel> serviceCodes;
            //    //using (var serviceBal = new ServiceCodeBal(Helpers.DefaultServiceCodeTableNumber))
            //    //{
            //    //    serviceCodes = serviceBal.GetServiceCodesCustomModel();
            //    //}

            //    //var list = GetBedRateCardCustom();

            //    //using (var globalBal = new GlobalCodeBal())
            //    //{
            //    //    //var bedTypeList =
            //    //    //    globalBal.GetGlobalCodesByCategoryValue(Convert.ToString((int)GlobalCodeCategoryValue.Bedtypes));
            //    //    //var unitTypesList =
            //    //    //    globalBal.GetGlobalCodesByCategoryValue(
            //    //    //        Convert.ToString((int)GlobalCodeCategoryValue.BedUnitType));



            //    //    //if (list.Count > 0 && bedTypeList.Count > 0 && unitTypesList.Count > 0 && serviceCodes.Count > 0)
            //    //    //{
            //    //    //    bedRateCardList.AddRange(from item in list
            //    //    //                             let sCode =
            //    //    //                                 serviceCodes.FirstOrDefault(a => a.ServiceCodeValue.Equals(item.ServiceCodeValue))
            //    //    //                             select new BedRateCardViewModel
            //    //    //                             {
            //    //    //                                 CurrentBedRateCard = item,
            //    //    //                                 BedTypeName =
            //    //    //                                     globalBal.GetGlobalCodeNameByIdAndCategoryId(
            //    //    //                                         Convert.ToString((int)GlobalCodeCategoryValue.Bedtypes),
            //    //    //                                         Convert.ToInt32(item.BedTypes)),
            //    //    //                                 UnitTypeName =
            //    //    //                                     globalBal.GetGlobalCodeNameByIdAndCategoryId(
            //    //    //                                         Convert.ToString((int)GlobalCodeCategoryValue.BedUnitType),
            //    //    //                                         Convert.ToInt32(item.UnitType)),
            //    //    //                                 ServiceCodeName = sCode != null ? sCode.ServiceCodeDescription : string.Empty
            //    //    //                             });
            //    //    //}


            //    //}

            //    var bedRateCardView = new BedRateCardView
            //    {
            //        BedRateCardsList = list,
            //        CurrentBedRateCard = new BedRateCardCustomModel
            //        {
            //            BedRateCard = new BedRateCard()
            //        }
            //    };

            //    return View(bedRateCardView);
            //}
            //var fId = Helpers.GetDefaultFacilityId();
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
                using (var bedRateCardBal = new BedRateCardBal())
                {

                    //if (model.BedRateCardID == 0)
                    //{
                    //    var list = bedRateCardBal.GetBedRateCardByServiceCode(model.ServiceCodeValue);
                    //    foreach (var modelList in list)
                    //    {
                    //    if (Convert.ToDateTime(modelList.EffectiveFrom) < Convert.ToDateTime(model.EffectiveFrom) &&
                    //        Convert.ToDateTime(modelList.EffectiveTill) > Convert.ToDateTime(model.EffectiveTill)) ;
                    //    return Json("-1");
                    //    }
                    //}


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

                    var newId = bedRateCardBal.AddUpdateBedRateCard(model);
                    return Json(newId);
                }
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
            using (var bal = new BedRateCardBal())
            {
                var model = bal.GetBedRateCardById(bedRateCardId);
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

                // using (var globalBal = new GlobalCodeBal())
                // {
                // using (var serviceBal = new ServiceCodeBal(Helpers.DefaultServiceCodeTableNumber))
                // {
                // var bedTypeList = globalBal.GetListByCategoryId(Convert.ToInt32(Common.Common.GlobalCodeCategoryValue.Bedtypes).ToString());
                // var unitTypesList = globalBal.GetListByCategoryId(Convert.ToInt32(Common.Common.GlobalCodeCategoryValue.BedUnitType).ToString());
                // var bedRateCardViewModel = new BedRateCardViewModel
                // {
                // CurrentBedRateCard = bedRateCard,
                // LstServiceCodeList = serviceBal.GetServiceCodes(),
                // LstUnitTypeList = unitTypesList,
                // LstBedTypesList = bedTypeList
                // };
                // return PartialView(PartialViews.BedRateCardAddEdit, bedRateCardViewModel);
                // }
                // }
            }
        }

        /// <summary>
        /// Deletes the bed rate card.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="facilityId"></param>
        /// <returns></returns>
        public ActionResult DeleteBedRateCard(string id)
        {
            using (var bal = new BedRateCardBal())
            {
                //facilityId = Helpers.GetLoggedInUserIsAdmin() ? 0 : facilityId;
                var currentBedRateCard = bal.GetBedRateCardById(Convert.ToInt32(id));
                if (currentBedRateCard != null)
                {
                    currentBedRateCard.DeletedBy = Helpers.GetLoggedInUserId();
                    currentBedRateCard.DeletedDate = Helpers.GetInvariantCultureDateTime();
                    currentBedRateCard.IsDeleted = true;
                    currentBedRateCard.IsActive = false;
                    var result = bal.AddUpdateBedRateCard(currentBedRateCard);
                    if (result > 0)
                    {
                        var list = GetBedRateCardCustom();
                        return PartialView(PartialViews.BedRateCardList, list);
                    }
                }

                return Json(null);
            }
        }

        /// <summary>
        /// Gets the bed rate card custom.
        /// </summary>
        /// <returns></returns>
        public List<BedRateCardCustomModel> GetBedRateCardCustom()
        {
            using (var bal = new BedRateCardBal())
            {
                //var corporateId = Helpers.GetSysAdminCorporateID();
                var list = bal.GetBedRateCardsList(Helpers.DefaultServiceCodeTableNumber);
                return list;
                //List<ServiceCode> serviceCodes;
                //using (var serviceBal = new ServiceCodeBal(Helpers.DefaultServiceCodeTableNumber))
                //{
                //    serviceCodes = serviceBal.GetServiceCodes();
                //}

                //using (var globalBal = new GlobalCodeBal())
                //{
                //    var bedTypeList = globalBal.GetGlobalCodesByCategoryValue(Convert.ToInt32(GlobalCodeCategoryValue.Bedtypes).ToString());
                //    var unitTypesList = globalBal.GetGlobalCodesByCategoryValue(Convert.ToInt32(GlobalCodeCategoryValue.BedUnitType).ToString());
                //    //if (list.Count > 0 && bedTypeList.Count > 0 && unitTypesList.Count > 0 && serviceCodes.Count > 0)
                //    //{
                //    //    bedRateCardList.AddRange(from item in list
                //    //                             let sCode = serviceCodes.FirstOrDefault(a => a.ServiceCodeValue.Equals(item.ServiceCodeValue))
                //    //                             select new BedRateCardViewModel
                //    //                             {
                //    //                                 CurrentBedRateCard = item,
                //    //                                 BedTypeName = globalBal.GetGlobalCodeNameByIdAndCategoryId(Convert.ToInt32(GlobalCodeCategoryValue.Bedtypes).ToString(), Convert.ToInt32(item.BedTypes)),
                //    //                                 UnitTypeName = globalBal.GetGlobalCodeNameByIdAndCategoryId(Convert.ToInt32(GlobalCodeCategoryValue.BedUnitType).ToString(), Convert.ToInt32(item.UnitType)),
                //    //                                 ServiceCodeName = sCode != null ? sCode.ServiceCodeDescription : string.Empty
                //    //                             });
                //    //}
                //}
            }

            // using (var bedRateCardBal = new BedRateCardBal())
            // {
            // var list = bedRateCardBal.GetBedRateCardsList();
            // //return PartialView(PartialViews.BedRateCardList, bedRateCardList);
            // using (var globalBal = new GlobalCodeBal())
            // {
            // var bedTypeList =
            // globalBal.GetListByCategoryId(
            // Convert.ToInt32(Common.Common.GlobalCodeCategoryValue.Bedtypes).ToString());
            // var unitTypesList =
            // globalBal.GetListByCategoryId(
            // Convert.ToInt32(Common.Common.GlobalCodeCategoryValue.BedUnitType).ToString());
            // List<ServiceCode> serviceCodes = null;
            // using (var serviceBal = new ServiceCodeBal(Helpers.DefaultServiceCodeTableNumber))
            // serviceCodes = serviceBal.GetServiceCodes();

            // if (list.Count > 0)
            // {
            // bedRateCardList.AddRange(from item in list
            // let bedType = bedTypeList.FirstOrDefault(a => a.GlobalCodeValue.Equals(item.BedTypes))
            // let unitType = unitTypesList.FirstOrDefault(a => a.GlobalCodeValue.Equals(item.UnitType))
            // let sCode =
            // serviceCodes.FirstOrDefault(a => a.ServiceCodeValue.Equals(item.ServiceCodeValue))
            // select new BedRateCardViewModel
            // {
            // CurrentBedRateCard = item,
            // BedTypeName = bedType != null ? bedType.GlobalCodeName : string.Empty,
            // UnitTypeName = unitType != null ? unitType.GlobalCodeName : string.Empty,
            // ServiceCodeName = sCode != null ? sCode.ServiceCodeDescription : string.Empty
            // });
            // }
            // }
            // }
            //return list;
        }

        /// <summary>
        /// Bind all the BedMaster list
        /// </summary>
        /// <returns>
        /// action result with the partial view containing the facility list object
        /// </returns>
        public ActionResult GetBedRateCardList()
        {
            //facilityId = Helpers.GetLoggedInUserIsAdmin() ? 0 : facilityId;
            // Initialize the BedMaster Bal object
            var bedRateCardList = GetBedRateCardCustom();

            // var bedRateCardList = new List<BedRateCardViewModel>();
            // using (var bedRateCardBal = new BedRateCardBal())
            // {
            // var list = bedRateCardBal.GetBedRateCardsList();
            // //return PartialView(PartialViews.BedRateCardList, bedRateCardList);
            // using (var globalBal = new GlobalCodeBal())
            // {
            // var bedTypeList =
            // globalBal.GetListByCategoryId(
            // Convert.ToInt32(Common.Common.GlobalCodeCategoryValue.Bedtypes).ToString());
            // var unitTypesList =
            // globalBal.GetListByCategoryId(
            // Convert.ToInt32(Common.Common.GlobalCodeCategoryValue.BedUnitType).ToString());
            // List<ServiceCode> serviceCodes = null;
            // using (var serviceBal = new ServiceCodeBal(Helpers.DefaultServiceCodeTableNumber))
            // serviceCodes = serviceBal.GetServiceCodes();

            // if (list.Count > 0)
            // {
            // bedRateCardList.AddRange(from item in list
            // let bedType = bedTypeList.FirstOrDefault(a => a.GlobalCodeValue.Equals(item.BedTypes))
            // let unitType = unitTypesList.FirstOrDefault(a => a.GlobalCodeValue.Equals(item.UnitType))
            // let sCode =
            // serviceCodes.FirstOrDefault(a => a.ServiceCodeValue.Equals(item.ServiceCodeValue))
            // select new BedRateCardViewModel
            // {
            // CurrentBedRateCard = item,
            // BedTypeName = bedType != null ? bedType.GlobalCodeName : string.Empty,
            // UnitTypeName = unitType != null ? unitType.GlobalCodeName : string.Empty,
            // ServiceCodeName = sCode != null ? sCode.ServiceCodeDescription : string.Empty
            // });
            // }
            // }
            // }
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
