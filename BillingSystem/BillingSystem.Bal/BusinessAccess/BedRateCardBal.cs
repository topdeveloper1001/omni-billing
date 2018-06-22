using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;


namespace BillingSystem.Bal.BusinessAccess
{
    public class BedRateCardBal : BaseBal
    {
        /// <summary>
        /// Get the GetBedRateCardsList
        /// </summary>
        /// <returns>Return the BedRateCard List</returns>
        public List<BedRateCardCustomModel> GetBedRateCardsList(string serviceCodeTableNumber, int corporateId, int facilityId)
        {
            var list = new List<BedRateCardCustomModel>();
            using (var rep = UnitOfWork.BedRateCardRepository)
            {
                var lstBedRateCard =
                    rep.Where(
                        b =>
                            (b.IsDeleted != true) && b.IsActive && b.CorporateId == corporateId &&
                            (facilityId == 0 || b.FacilityId == facilityId)).ToList();

                var gcbal = new GlobalCodeBal();
                var sBal = new ServiceCodeBal(serviceCodeTableNumber);

                list.AddRange(from item in lstBedRateCard
                              let sc = sBal.GetServiceCodeByCodeValue(item.ServiceCodeValue.Trim())
                              select new BedRateCardCustomModel
                              {
                                  BedRateCard = item,
                                  UnitTypeName =
                                      gcbal.GetNameByGlobalCodeValueAndCategoryValue(
                                          Convert.ToString((int)GlobalCodeCategoryValue.BedUnitType), item.UnitType.Trim()),
                                  BedTypeName =
                                      gcbal.GetNameByGlobalCodeValueAndCategoryValue(
                                          Convert.ToString((int)GlobalCodeCategoryValue.Bedtypes),
                                          Convert.ToString(Convert.ToInt32(item.BedTypes))),
                                  ServiceCodeName = sc.ServiceCodeDescription,
                                  ServiceCodeTableNumber = serviceCodeTableNumber,
                                  ServiceCodeEffectiveFrom = Convert.ToString(sc.ServiceCodeEffectiveDate),
                                  ServiceCodeEffectiveTill = Convert.ToString(sc.ServiceExpiryDate),
                                  FacilityName = GetFacilityNameByFacilityId(Convert.ToInt32(item.FacilityId))
                              });

                return list;
            }
        }


        /// <summary>
        /// Get the GetBedRateCardsList
        /// </summary>
        /// <returns>Return the BedRateCard List</returns>
        public List<BedRateCardCustomModel> GetBedRateCardsList(string serviceCodeTableNumber)
        {
            var list = new List<BedRateCardCustomModel>();
            using (var rep = UnitOfWork.BedRateCardRepository)
            {
                var lstBedRateCard =
                    rep.Where(
                        b =>
                            (b.IsDeleted != true) && b.IsActive).ToList();

                var gcbal = new GlobalCodeBal();
                var sBal = new ServiceCodeBal(serviceCodeTableNumber);

                list.AddRange(from item in lstBedRateCard
                              let sc = sBal.GetServiceCodeByCodeValue(item.ServiceCodeValue.Trim())
                              select new BedRateCardCustomModel
                              {
                                  BedRateCard = item,
                                  UnitTypeName =
                                      gcbal.GetNameByGlobalCodeValueAndCategoryValue(
                                          Convert.ToString((int)GlobalCodeCategoryValue.BedUnitType), item.UnitType.Trim()),
                                  BedTypeName =
                                      gcbal.GetNameByGlobalCodeValueAndCategoryValue(
                                          Convert.ToString((int)GlobalCodeCategoryValue.Bedtypes),
                                          Convert.ToString(Convert.ToInt32(item.BedTypes))),
                                  ServiceCodeName = sc.ServiceCodeDescription,
                                  ServiceCodeTableNumber = serviceCodeTableNumber,
                                  ServiceCodeEffectiveFrom = Convert.ToString(sc.ServiceCodeEffectiveDate),
                                  ServiceCodeEffectiveTill = Convert.ToString(sc.ServiceExpiryDate),
                                  FacilityName = GetFacilityNameByFacilityId(Convert.ToInt32(item.FacilityId))
                              });

                return list;
            }
        }

        /// <summary>
        /// Method to add/Update the BedRateCard in the database.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddUpdateBedRateCard(BedRateCard model)
        {
            var currentDateTime = GetInvariantCultureDateTime(Convert.ToInt32(model.FacilityId));
            var effectiveFrom = Convert.ToDateTime(model.EffectiveFrom);
            var effectiveEnd = model.EffectiveTill ?? Convert.ToDateTime(model.EffectiveTill);
            //var effectiveEnd = Convert.ToDateTime(model.EffectiveTill);

            if (!model.EffectiveFrom.HasValue)
                model.EffectiveFrom = new DateTime(currentDateTime.Year, currentDateTime.Month, 1);

            var result = -1;
            using (var rep = UnitOfWork.BedRateCardRepository)
            {
                bool isExxistData = false;
                if (model.EffectiveTill == null)
                {
                    isExxistData = rep.Where(
                        m => m.BedTypes == model.BedTypes &&
                             (m.BedRateCardID != model.BedRateCardID || model.BedRateCardID == 0) &&
                             m.DayStart == model.DayStart && m.DayEnd == model.DayEnd && m.UnitType ==model.UnitType &&
                             m.ServiceCodeValue.Equals(model.ServiceCodeValue) &&
                             m.EffectiveFrom <= effectiveFrom && m.EffectiveTill==null).Any();
                }
                else
                {
                    isExxistData = rep.Where(
                        m => m.BedTypes == model.BedTypes &&
                             (m.BedRateCardID != model.BedRateCardID || model.BedRateCardID == 0) &&
                             m.DayStart == model.DayStart && m.DayEnd == model.DayEnd &&  m.UnitType ==model.UnitType &&
                             m.ServiceCodeValue.Equals(model.ServiceCodeValue) &&
                             m.EffectiveFrom <= effectiveFrom && m.EffectiveTill >= model.EffectiveTill).Any();
                }



                //var isExists =
                //    rep.Where(
                //        m => m.BedTypes == model.BedTypes &&
                //             (m.BedRateCardID != model.BedRateCardID || model.BedRateCardID == 0) &&
                //             m.DayStart == model.DayStart && m.DayEnd == model.DayEnd &&
                //             m.ServiceCodeValue.Equals(model.ServiceCodeValue) &&
                //             m.EffectiveFrom <= effectiveFrom && m.EffectiveTill >= model.EffectiveTill).Any();

                if (isExxistData)
                    return result;

                if (model.BedRateCardID > 0)
                {
                    var current = rep.GetSingle(model.BedRateCardID);
                    if (current != null)
                    {
                        model.CreatedBy = current.CreatedBy;
                        model.CreatedDate = current.CreatedDate;
                    }
                    rep.UpdateEntity(model, model.BedRateCardID);
                }
                else
                    rep.Create(model);

                result = model.BedRateCardID;
            }
            return result;
        }

        /// <summary>
        /// Method to add the BedRateCard in the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BedRateCard GetBedRateCardById(int id)
        {
            using (var bedRateCardRep = UnitOfWork.BedRateCardRepository)
            {
                var bedRateCard = bedRateCardRep.Where(x => x.BedRateCardID == id).FirstOrDefault();
                return bedRateCard;
            }
        }

        public IEnumerable<BedRateCardCustomModel> GetBedRateCardsListByBedType(string bedTypeid, bool nonChargeable)
        {
            var lstBedRateCard = new List<BedRateCardCustomModel>();
            var bedtypeidint = Convert.ToInt32(bedTypeid);
            using (var bedRateCardRep = UnitOfWork.BedRateCardRepository)
            {
                if (!nonChargeable)
                {
                    var bedRateCardList = bedRateCardRep.Where(x => x.BedTypes == bedtypeidint).ToList();
                    var globalBal = new GlobalCodeBal();
                    var unitTypesList = Convert.ToString((int)GlobalCodeCategoryValue.BedUnitType);
                    lstBedRateCard.AddRange(bedRateCardList.Select(item => new BedRateCardCustomModel
                    {
                        BedRateCard = item,
                        UnitTypeName =
                            globalBal.GetGlobalCodeNameByIdAndCategoryId(unitTypesList, Convert.ToInt32(item.UnitType))
                    }));
                }
                else
                {
                    var bedRateCard = new BedRateCard { Rates = 0, UnitType = "Non-Chargeable" };
                    lstBedRateCard.Add(new BedRateCardCustomModel
                    {
                        BedRateCard = bedRateCard,
                        UnitTypeName = "Non-Chargeable"
                    });
                }
            }
            return lstBedRateCard;
        }

        /// <summary>
        /// Method to Get the BedRate.
        /// </summary>
        /// <returns></returns>
        public string GetBedRateByBedTypeId(int id)
        {
            var ratetoreturn = "0.00";
            using (var bedRateCardRep = UnitOfWork.BedRateCardRepository)
            {
                var bedratecard = bedRateCardRep.Where(x => x.BedTypes == id).FirstOrDefault();
                if (bedratecard != null)
                {
                    ratetoreturn = bedratecard.Rates.ToString();
                }
            }
            return ratetoreturn;
        }

    }
}
