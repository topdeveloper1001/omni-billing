using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class BedRateCardService : IBedRateCardService
    {
        private readonly IRepository<BedRateCard> _repository;
        private readonly IRepository<Facility> _fRepository;

        private readonly BillingEntities _context;
        private readonly IMapper _mapper;

        public BedRateCardService(IRepository<BedRateCard> repository, IRepository<Facility> fRepository, BillingEntities context, IMapper mapper)
        {
            _repository = repository;
            _fRepository = fRepository;
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Get the GetBedRateCardsList
        /// </summary>
        /// <returns>Return the BedRateCard List</returns>
        public List<BedRateCardCustomModel> GetBedRateCardsList(string serviceCodeTableNumber, int corporateId, int facilityId)
        {
            var list = new List<BedRateCardCustomModel>();
            var lstBedRateCard = _repository.Where(b => (b.IsDeleted != true) && b.IsActive && b.CorporateId == corporateId && (facilityId == 0 || b.FacilityId == facilityId)).ToList();

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
                              FacilityName = _fRepository.Where(f => f.FacilityId == facilityId).FirstOrDefault().FacilityName
                          });

            return list;
        }


        /// <summary>
        /// Get the GetBedRateCardsList
        /// </summary>
        /// <returns>Return the BedRateCard List</returns>
        public List<BedRateCardCustomModel> GetBedRateCardsList(string serviceCodeTableNumber)
        {
            var list = new List<BedRateCardCustomModel>();
            var lstBedRateCard =
                _repository.Where(
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
                              FacilityName = _fRepository.Where(f => f.FacilityId == Convert.ToInt32(item.FacilityId)).FirstOrDefault().FacilityName
                          });

            return list;
        }

        public DateTime GetInvariantCultureDateTime(int facilityid)
        {

            var facilityObj = _fRepository.Where(f => f.FacilityId == Convert.ToInt32(facilityid)).FirstOrDefault() != null ? _fRepository.Where(f => f.FacilityId == Convert.ToInt32(facilityid)).FirstOrDefault().FacilityTimeZone : TimeZoneInfo.Utc.ToString();
            var tzi = TimeZoneInfo.FindSystemTimeZoneById(facilityObj);
            var utcTime = DateTime.Now.ToUniversalTime();
            var convertedTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
            return convertedTime;
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
            bool isExxistData = false;
            if (model.EffectiveTill == null)
            {
                isExxistData = _repository.Where(
                    m => m.BedTypes == model.BedTypes &&
                         (m.BedRateCardID != model.BedRateCardID || model.BedRateCardID == 0) &&
                         m.DayStart == model.DayStart && m.DayEnd == model.DayEnd && m.UnitType == model.UnitType &&
                         m.ServiceCodeValue.Equals(model.ServiceCodeValue) &&
                         m.EffectiveFrom <= effectiveFrom && m.EffectiveTill == null).Any();
            }
            else
            {
                isExxistData = _repository.Where(
                    m => m.BedTypes == model.BedTypes &&
                         (m.BedRateCardID != model.BedRateCardID || model.BedRateCardID == 0) &&
                         m.DayStart == model.DayStart && m.DayEnd == model.DayEnd && m.UnitType == model.UnitType &&
                         m.ServiceCodeValue.Equals(model.ServiceCodeValue) &&
                         m.EffectiveFrom <= effectiveFrom && m.EffectiveTill >= model.EffectiveTill).Any();
            }

            if (isExxistData)
                return result;

            if (model.BedRateCardID > 0)
            {
                var current = _repository.GetSingle(model.BedRateCardID);
                if (current != null)
                {
                    model.CreatedBy = current.CreatedBy;
                    model.CreatedDate = current.CreatedDate;
                }
                _repository.UpdateEntity(model, model.BedRateCardID);
            }
            else
                _repository.Create(model);

            result = model.BedRateCardID;
            return result;
        }

        /// <summary>
        /// Method to add the BedRateCard in the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BedRateCard GetBedRateCardById(int id)
        {
            var m = _repository.Where(x => x.BedRateCardID == id).FirstOrDefault();
            return m;
        }

        public IEnumerable<BedRateCardCustomModel> GetBedRateCardsListByBedType(string bedTypeid, bool nonChargeable)
        {
            var lst = new List<BedRateCardCustomModel>();
            var bedType = Convert.ToInt32(bedTypeid);
            if (!nonChargeable)
            {
                var blst = _repository.Where(x => x.BedTypes == bedType).ToList();
                var globalBal = new GlobalCodeBal();
                var unitTypesList = Convert.ToString((int)GlobalCodeCategoryValue.BedUnitType);
                lst.AddRange(blst.Select(item => new BedRateCardCustomModel
                {
                    BedRateCard = item,
                    UnitTypeName =
                        globalBal.GetGlobalCodeNameByIdAndCategoryId(unitTypesList, Convert.ToInt32(item.UnitType))
                }));
            }
            else
            {
                var m = new BedRateCard { Rates = 0, UnitType = "Non-Chargeable" };
                lst.Add(new BedRateCardCustomModel
                {
                    BedRateCard = m,
                    UnitTypeName = "Non-Chargeable"
                });
            }
            return lst;
        }

        /// <summary>
        /// Method to Get the BedRate.
        /// </summary>
        /// <returns></returns>
        public string GetBedRateByBedTypeId(int id)
        {
            var str = "0.00";
            var m = _repository.Where(x => x.BedTypes == id).FirstOrDefault();
            if (m != null)
            {
                str = m.Rates.ToString();
            }
            return str;
        }

    }
}
