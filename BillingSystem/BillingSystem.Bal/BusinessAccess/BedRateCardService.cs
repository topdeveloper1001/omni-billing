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
        private readonly IRepository<GlobalCodes> _gRepository;
        private readonly IRepository<ServiceCode> _scRepository;

        private readonly BillingEntities _context;
        private readonly IMapper _mapper;

        public BedRateCardService(IRepository<BedRateCard> repository, IRepository<Facility> fRepository, IRepository<GlobalCodes> gRepository, BillingEntities context, IMapper mapper)
        {
            _repository = repository;
            _fRepository = fRepository;
            _gRepository = gRepository;
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

            list.AddRange(from item in lstBedRateCard
                          let sc = GetServiceCodeByCodeValue(item.ServiceCodeValue.Trim(), serviceCodeTableNumber)
                          select new BedRateCardCustomModel
                          {
                              BedRateCard = item,
                              UnitTypeName = GetNameByGlobalCodeValueAndCategoryValue(
                                      Convert.ToString((int)GlobalCodeCategoryValue.BedUnitType), item.UnitType.Trim()),
                              BedTypeName = GetNameByGlobalCodeValueAndCategoryValue(
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
        private string GetNameByGlobalCodeValueAndCategoryValue(string categoryValue, string globalCodeValue)
        {
            if (!string.IsNullOrEmpty(categoryValue) && !string.IsNullOrEmpty(globalCodeValue))
            {
                var globalCode = _gRepository.Where(c => c.GlobalCodeCategoryValue.Equals(categoryValue)
                && c.GlobalCodeValue.Equals(globalCodeValue)).FirstOrDefault();

                if (globalCode != null)
                    return globalCode.GlobalCodeName;
            }
            return string.Empty;
        }
        private ServiceCode GetServiceCodeByCodeValue(string serviceCodeValue, string ServiceCodeTableNumber)
        {
            var serviceCode = _scRepository.Where(x => x.ServiceCodeValue.Equals(serviceCodeValue) && x.ServiceCodeTableNumber.Trim().Equals(ServiceCodeTableNumber)).FirstOrDefault();
            return serviceCode ?? new ServiceCode();
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

            list.AddRange(from item in lstBedRateCard
                          let sc = GetServiceCodeByCodeValue(item.ServiceCodeValue.Trim(), serviceCodeTableNumber)
                          select new BedRateCardCustomModel
                          {
                              BedRateCard = item,
                              UnitTypeName = GetNameByGlobalCodeValueAndCategoryValue(
                                      Convert.ToString((int)GlobalCodeCategoryValue.BedUnitType), item.UnitType.Trim()),
                              BedTypeName = GetNameByGlobalCodeValueAndCategoryValue(
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

        private DateTime GetInvariantCultureDateTime(int facilityid)
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

        private string GetGlobalCodeNameByIdAndCategoryId(string categoryId, int globalCodeId)
        {
            var globalCode = _gRepository.Where(c => c.GlobalCodeCategoryValue.Equals(categoryId) && c.GlobalCodeID == globalCodeId).FirstOrDefault();
            if (globalCode != null)
                return globalCode.GlobalCodeName;
            return string.Empty;
        }
        public IEnumerable<BedRateCardCustomModel> GetBedRateCardsListByBedType(string bedTypeid, bool nonChargeable)
        {
            var lst = new List<BedRateCardCustomModel>();
            var bedType = Convert.ToInt32(bedTypeid);
            if (!nonChargeable)
            {
                var blst = _repository.Where(x => x.BedTypes == bedType).ToList();
                var unitTypesList = Convert.ToString((int)GlobalCodeCategoryValue.BedUnitType);
                lst.AddRange(blst.Select(item => new BedRateCardCustomModel
                {
                    BedRateCard = item,
                    UnitTypeName = GetGlobalCodeNameByIdAndCategoryId(unitTypesList, Convert.ToInt32(item.UnitType))
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
