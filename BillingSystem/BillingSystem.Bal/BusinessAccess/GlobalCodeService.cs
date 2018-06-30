using System.Linq;
using BillingSystem.Common.Common;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;
using System.Collections.Generic;
using System;
using BillingSystem.Repository.Interfaces;
using System.Data.SqlClient;
using BillingSystem.Repository.Common;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class GlobalCodeService : IGlobalCodeService
    {
        private readonly IRepository<GlobalCodes> _repository;
        private readonly IRepository<FacilityStructure> _fsRepository;
        private readonly IRepository<Facility> _fRepository;
        private readonly IRepository<IndicatorDataCheckList> _idlRepository;
        private readonly IRepository<GlobalCodeCategory> _gcRepository;
        private readonly BillingEntities _context;

        public GlobalCodeService(IRepository<GlobalCodes> repository, IRepository<FacilityStructure> fsRepository, IRepository<Facility> fRepository, IRepository<IndicatorDataCheckList> idlRepository, IRepository<GlobalCodeCategory> gcRepository, BillingEntities context)
        {
            _repository = repository;
            _fsRepository = fsRepository;
            _fRepository = fRepository;
            _idlRepository = idlRepository;
            _gcRepository = gcRepository;
            _context = context;
        }

        private DateTime GetInvariantCultureDateTime(int facilityid)
        {
            var facilityObj = _fRepository.Where(f => f.FacilityId == Convert.ToInt32(facilityid)).FirstOrDefault() != null ? _fRepository.Where(f => f.FacilityId == Convert.ToInt32(facilityid)).FirstOrDefault().FacilityTimeZone : TimeZoneInfo.Utc.ToString();
            var tzi = TimeZoneInfo.FindSystemTimeZoneById(facilityObj);
            var utcTime = DateTime.Now.ToUniversalTime();
            var convertedTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
            return convertedTime;
        }

        //Function to get all GlobalCodes
        /// <summary>
        /// Gets all global codes.
        /// </summary>
        /// <param name="categoryValue">The category value.</param>
        /// <returns></returns>
        public List<GlobalCodeCustomModel> GetAllGlobalCodes(string categoryValue)
        {
            var list = new List<GlobalCodeCustomModel>();
            var globalCodes = !string.IsNullOrEmpty(categoryValue) ? _repository.Where(s => s.IsDeleted == false && s.IsActive && s.GlobalCodeCategoryValue.Equals(categoryValue)).ToList() : _repository.Where(s => s.IsDeleted == false && s.IsActive).ToList();
            if (globalCodes.Count > 0)
            {
                var unitOfMeasure = Convert.ToString(Convert.ToInt32(GlobalCodeCategoryValue.UnitOfMeasure));
                list.AddRange(globalCodes.Select(item => new GlobalCodeCustomModel
                {
                    GlobalCodes = item,
                    GlobalCodeCustomValue = GetGlobalCategoryNameById(item.GlobalCodeCategoryValue),
                    UnitOfMeasure =
                        !string.IsNullOrEmpty(item.ExternalValue3)
                            ? GetNameByGlobalCodeValueAndCategoryValue(unitOfMeasure, item.ExternalValue3)
                            : string.Empty
                }));
            }
            return list;
        }
        private GlobalCodeCategory GetGlobalCodeCategoryByValue(string globalCodeCategoryvalue)
        {
            var m = _gcRepository.Where(f => f.GlobalCodeCategoryValue == globalCodeCategoryvalue).FirstOrDefault();
            return m ?? new GlobalCodeCategory();
        }
        public string GetGlobalCategoryNameById(string categoryValue, string facilityId = "")
        {
            if (!string.IsNullOrEmpty(categoryValue))
            {
                var category = _gcRepository.Where(g => g.GlobalCodeCategoryValue.Equals(categoryValue) && g.IsDeleted.HasValue ? !g.IsDeleted.Value : false && (string.IsNullOrEmpty(facilityId) || g.FacilityNumber.Equals(facilityId))).FirstOrDefault();
                return category != null ? category.GlobalCodeCategoryName : string.Empty;
            }
            return string.Empty;
        }
        //Function to get GlobalCodes by GlobalCodeID
        /// <summary>
        /// Gets the global code by global code identifier.
        /// </summary>
        /// <param name="globalCodeId">The global code identifier.</param>
        /// <returns></returns>
        public GlobalCodes GetGlobalCodeByGlobalCodeId(int globalCodeId)
        {
            var globalCode = _repository.Where(s => s.IsDeleted == false && s.GlobalCodeID == globalCodeId).FirstOrDefault();
            return globalCode ?? new GlobalCodes();
        }

        //Function to get GlobalCodes by GlobalCodeCategoryID
        /// <summary>
        /// Gets the global code by global code category identifier.
        /// </summary>
        /// <param name="categoryValue">The global code category identifier.</param>
        /// <returns></returns>
        public List<GlobalCodes> GetGCodesListByCategoryValue(string categoryValue)
        {
            var list = _repository.Where(s => s.IsDeleted == false
            && s.GlobalCodeCategoryValue.Equals(categoryValue))
            .OrderBy(g => g.SortOrder).ToList();
            return list;
        }

        //Function to add update tab
        /// <summary>
        /// Adds the update global codes.
        /// </summary>
        /// <param name="globalCodes">The global codes.</param>
        /// <returns></returns>
        public int AddUpdateGlobalCodes(GlobalCodes globalCodes)
        {
            if (string.IsNullOrEmpty(globalCodes.GlobalCodeValue) || int.Parse(globalCodes.GlobalCodeValue) == 0)
            {
                var maxId = GetMaxGlobalCodeValueByCategory(globalCodes.GlobalCodeCategoryValue) + 1;
                globalCodes.GlobalCodeValue = Convert.ToString(maxId);
            }


            if (globalCodes.GlobalCodeID > 0)
            {
                var current = _repository.GetSingle(globalCodes.GlobalCodeID);
                if (current != null)
                {
                    globalCodes.CreatedBy = current.CreatedBy;
                    globalCodes.CreatedDate = current.CreatedDate;
                }
                _repository.UpdateEntity(globalCodes, globalCodes.GlobalCodeID);
            }
            else
                _repository.Create(globalCodes);
            return globalCodes.GlobalCodeID;
        }

        /// <summary>
        /// Adds the update global codes list.
        /// </summary>
        /// <param name="globalCodes">The global codes.</param>
        /// <returns></returns>
        public bool AddUpdateGlobalCodesList(List<GlobalCodes> globalCodes)
        {
            try
            {
                var objToDelete = GetPreviousRecords(globalCodes[0].FacilityNumber, globalCodes[0].ExternalValue1);
                if (objToDelete.Any()) _repository.Delete(objToDelete);
                _repository.Create(globalCodes);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<GlobalCodes> GetPreviousRecords(string facilitynumber, string roomId)
        {
            var l = _repository.Where(x => x.GlobalCodeCategoryValue == "4910" && x.FacilityNumber == facilitynumber && x.ExternalValue1 == roomId).ToList();
            return l;
        }

        /// <summary>
        /// Gets the list by category identifier.
        /// </summary>
        /// <param name="categoryValue">The identifier.</param>
        /// <returns></returns>
        public List<GlobalCodes> GetGlobalCodesByCategoryValue(string categoryValue, string fn = "")
        {
            var list = _repository.Where(g => g.GlobalCodeCategoryValue.Equals(categoryValue) && g.IsActive && g.IsDeleted.HasValue && !g.IsDeleted.Value && (string.IsNullOrEmpty(fn) || g.FacilityNumber.Equals(fn))).ToList();
            return list.OrderBy(x => x.GlobalCodeName).ToList();
        }

        /// <summary>
        /// Gets the type of the encounter types by patient.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="patientTypeId">The patient type identifier.</param>
        /// <returns></returns>
        public List<GlobalCodes> GetEncounterTypesByPatientType(string id, string patientTypeId)
        {
            var list = _repository.Where(g => g.GlobalCodeCategoryValue.Equals(id) && !(bool)g.IsDeleted && g.ExternalValue1.Equals(patientTypeId)).OrderBy(g => g.GlobalCodeID).ToList();
            return list;
        }

        /// <summary>
        /// Method to To check Duplicate GlobalCodeName
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="categoryValue">The category value.</param>
        /// <returns></returns>
        public bool CheckDuplicateGlobalCodeName(string name, int id, string categoryValue, string facilityNumber)
        {
            name = name.Trim().ToLower();

            var isExists = _repository.Where(g => g.IsDeleted == false && g.GlobalCodeName.Trim().ToLower().Equals(name) && g.GlobalCodeCategoryValue.Trim().Equals(categoryValue) && (string.IsNullOrEmpty(facilityNumber) || g.FacilityNumber.Equals(facilityNumber)) && ((id > 0 && g.GlobalCodeID != id) || id == 0)).Any();
            return isExists;
        }

        /// <summary>
        /// Gets the global code by identifier and category identifier.
        /// </summary>
        /// <param name="categoryValue">The category identifier.</param>
        /// <param name="globalCodeValue">The global code identifier.</param>
        /// <returns></returns>
        public string GetNameByGlobalCodeValueAndCategoryValue(string categoryValue, string globalCodeValue)
        {
            if (!string.IsNullOrEmpty(categoryValue) && !string.IsNullOrEmpty(globalCodeValue))
            {
                var globalCode = _repository.Where(c => c.GlobalCodeCategoryValue.Equals(categoryValue)
                && c.GlobalCodeValue.Equals(globalCodeValue)).FirstOrDefault();

                if (globalCode != null)
                    return globalCode.GlobalCodeName;
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the global code name by identifier and category identifier.
        /// </summary>
        /// <param name="categoryId">The category identifier.</param>
        /// <param name="globalCodeId">The global code identifier.</param>
        /// <returns></returns>
        public string GetGlobalCodeNameByIdAndCategoryId(string categoryId, int globalCodeId)
        {
            var globalCode = _repository.Where(c => c.GlobalCodeCategoryValue.Equals(categoryId) && c.GlobalCodeID == globalCodeId).FirstOrDefault();
            if (globalCode != null)
                return globalCode.GlobalCodeName;
            return string.Empty;
        }

        /// <summary>
        /// Gets the global codes by categories range.
        /// </summary>
        /// <param name="startRange">The start range.</param>
        /// <param name="endRange">The end range.</param>
        /// <returns></returns>
        public List<GlobalCodeCustomModel> GetGlobalCodesByCategoriesRange(int startRange, int endRange)
        {
            var list = new List<GlobalCodeCustomModel>();
            var gModellist =
                _repository.Where(f => f.IsDeleted != null && !(bool)f.IsDeleted)
                    .ToList()
                    .Where(
                        g =>
                            Convert.ToInt32(g.GlobalCodeCategoryValue) >= startRange &&
                            Convert.ToInt32(g.GlobalCodeCategoryValue) <= endRange)
                    .OrderByDescending(v => v.CreatedDate)
                    .ToList();

            if (gModellist.Count > 0)
            {
                list.AddRange(gModellist.Select(items => new GlobalCodeCustomModel
                {
                    GlobalCodes = items,
                    GlobalCodeCustomValue = GetGlobalCategoryNameById(items.GlobalCodeCategoryValue)
                }));
            }
            return list;
        }

        /// <summary>
        /// Gets the maximum global code value by category.
        /// </summary>
        /// <param name="categoryValue">The category value.</param>
        /// <returns></returns>
        public int GetMaxGlobalCodeValueByCategory(string categoryValue)
        {
            var maxValue = 0;
            var list = _repository.Where(g => g.GlobalCodeCategoryValue.Equals(categoryValue) && !string.IsNullOrEmpty(g.GlobalCodeValue)).Select(m => m.GlobalCodeValue).ToList();
            if (list.Count > 0)
                maxValue = list.Select(int.Parse).Max();
            return maxValue;
        }


        /// <summary>
        /// Gets the external val1 val2 by identifier and category identifier.
        /// </summary>
        /// <param name="categoryId">The category identifier.</param>
        /// <param name="globalCodeId">The global code identifier.</param>
        /// <returns></returns>
        public string GetExternalVal1Val2ByIdAndCategoryId(string categoryId, int globalCodeId)
        {
            var globalCode = _repository.Where(c => c.GlobalCodeCategoryValue.Equals(categoryId) && c.GlobalCodeID == globalCodeId).FirstOrDefault();
            if (globalCode != null)
                return string.Format("{0} - {1}", globalCode.ExternalValue1, globalCode.ExternalValue2);
            return string.Empty;
        }

        /// <summary>
        /// Gets the general global code by range value.
        /// </summary>
        /// <param name="coderange">The coderange.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        internal GeneralCodesCustomModel GetGeneralGlobalCodeByRangeVal(long coderange, string type)
        {
            var generalCodesCustomModel = new GeneralCodesCustomModel();
            var gModellist =
                    _repository.Where(f =>
                            f.IsDeleted != null && !(bool)f.IsDeleted &&
                            f.ExternalValue1 == type && !string.IsNullOrEmpty(f.ExternalValue2) && !string.IsNullOrEmpty(f.ExternalValue3)).ToList();
            if (type == Convert.ToInt32(OrderType.CPT).ToString())
            {

                var globalcodeval =
                    gModellist.FirstOrDefault(f => Convert.ToInt32(f.ExternalValue2) <= coderange && Convert.ToInt32(f.ExternalValue3) >= coderange);
                if (globalcodeval != null)
                {

                    generalCodesCustomModel.GlobalCodeName = globalcodeval.GlobalCodeName;
                    generalCodesCustomModel.GlobalCodeId = globalcodeval.GlobalCodeID;
                    var globalcodecategoryVal = GetGlobalCodeCategoryByValue(globalcodeval.GlobalCodeCategoryValue);
                    if (globalcodecategoryVal != null)
                    {
                        generalCodesCustomModel.GlobalCodeCategoryName =
                            globalcodecategoryVal.GlobalCodeCategoryName;
                        generalCodesCustomModel.GlobalCodeCategoryId =
                            globalcodecategoryVal.GlobalCodeCategoryValue;
                    }
                }
            }
            else if (type == Convert.ToInt32(OrderType.DRUG).ToString())
            {
                var coderangestr = coderange.ToString();
                var gModelDruglist =
                    _repository.Where(
                        f =>
                            f.IsDeleted != null && !(bool)f.IsDeleted &&
                            f.GlobalCodeValue == coderangestr).SingleOrDefault();
                if (gModelDruglist != null)
                {
                    generalCodesCustomModel.GlobalCodeName = gModelDruglist.GlobalCodeName;
                    generalCodesCustomModel.GlobalCodeId = gModelDruglist.GlobalCodeID;
                    var globalcodecategoryVal = GetGlobalCodeCategoryByValue(
                            gModelDruglist.GlobalCodeCategoryValue);
                    if (globalcodecategoryVal != null)
                    {
                        generalCodesCustomModel.GlobalCodeCategoryName =
                            globalcodecategoryVal.GlobalCodeCategoryName;
                        generalCodesCustomModel.GlobalCodeCategoryId =
                            globalcodecategoryVal.GlobalCodeCategoryValue;
                    }
                }
            }
            else if (type == Convert.ToInt32(OrderType.HCPCS).ToString())
            {
                var coderangestr = coderange.ToString();
                var gModelDruglist =
                    _repository.Where(
                        f =>
                            f.IsDeleted != null && !(bool)f.IsDeleted &&
                            f.GlobalCodeValue == coderangestr).SingleOrDefault();
                if (gModelDruglist != null)
                {
                    generalCodesCustomModel.GlobalCodeName = gModelDruglist.GlobalCodeName;
                    generalCodesCustomModel.GlobalCodeId = gModelDruglist.GlobalCodeID;
                    var globalcodecategoryVal = GetGlobalCodeCategoryByValue(
                            gModelDruglist.GlobalCodeCategoryValue);
                    if (globalcodecategoryVal != null)
                    {
                        generalCodesCustomModel.GlobalCodeCategoryName =
                            globalcodecategoryVal.GlobalCodeCategoryName;
                        generalCodesCustomModel.GlobalCodeCategoryId =
                            globalcodecategoryVal.GlobalCodeCategoryValue;
                    }
                }
            }
            return generalCodesCustomModel;
        }

        /// <summary>
        /// Gets the general global code by range value.
        /// </summary>
        /// <param name="coderange">The coderange.</param>
        /// <param name="type">The type.</param>
        /// <param name="categoryId">The category identifier.</param>
        /// <returns></returns>
        internal GeneralCodesCustomModel GetGeneralGlobalCodeByRangeVal(long coderange, string type, string categoryId)
        {
            var generalCodesCustomModel = new GeneralCodesCustomModel();
            var gModellist =
                    _repository.Where(
                        f =>
                            f.IsDeleted != null && !(bool)f.IsDeleted &&
                            f.ExternalValue1 == type && f.GlobalCodeCategoryValue == categoryId).ToList();
            if (type == Convert.ToInt32(OrderType.CPT).ToString())
            {

                var globalcodeval =
                    gModellist.SingleOrDefault(f => Convert.ToInt32(f.ExternalValue2) <= coderange &&
                        Convert.ToInt32(f.ExternalValue3) >= coderange);
                if (globalcodeval != null)
                {

                    generalCodesCustomModel.GlobalCodeName = globalcodeval.GlobalCodeName;
                    generalCodesCustomModel.GlobalCodeId = globalcodeval.GlobalCodeID;
                    var globalcodecategoryVal = GetGlobalCodeCategoryByValue(globalcodeval.GlobalCodeCategoryValue);
                    if (globalcodecategoryVal != null)
                    {
                        generalCodesCustomModel.GlobalCodeCategoryName =
                            globalcodecategoryVal.GlobalCodeCategoryName;
                        generalCodesCustomModel.GlobalCodeCategoryId =
                            globalcodecategoryVal.GlobalCodeCategoryValue;
                    }
                }
            }
            return generalCodesCustomModel;
        }

        /// <summary>
        /// Gets the type of the range by category.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        public string GetRangeByCategoryType(string p)
        {
            var globalCode = _repository.Where(c => c.GlobalCodeCategoryValue.Equals(p)).ToList();
            if (globalCode.Any())
            {
                var first = globalCode.FirstOrDefault();
                var lastOrDefault = globalCode.LastOrDefault();
                if (first != null && lastOrDefault != null)
                {
                    return string.Format("{0} - {1}", first.ExternalValue2, lastOrDefault.ExternalValue3);
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the maximum global code by category value.
        /// </summary>
        /// <param name="categoryValue">The category value.</param>
        /// <returns></returns>
        public GlobalCodes GetMaxGlobalCodeByCategoryValue(string categoryValue)
        {
            var model = _repository.Where(g => g.GlobalCodeCategoryValue.Equals(categoryValue)).OrderByDescending(a => a.GlobalCodeValue).FirstOrDefault();
            return model;
        }

        /// <summary>
        /// Gets all global codes.
        /// </summary>
        /// <param name="categoryValue">The category value.</param>
        /// <param name="showDeleted"></param>
        /// <returns></returns>
        public List<GlobalCodeCustomModel> ShowDeletedRecordsByCategoryValue(string categoryValue, bool showDeleted)
        {
            var list = new List<GlobalCodeCustomModel>();

            var globalCodes = showDeleted
                ? _repository.Where(s => s.IsDeleted == true && s.GlobalCodeCategoryValue.Equals(categoryValue)).ToList()
                : _repository.Where(
                    s => s.IsDeleted == false && s.IsActive && s.GlobalCodeCategoryValue.Equals(categoryValue))
                    .ToList();
            if (globalCodes.Count > 0)
            {
                list.AddRange(globalCodes.Select(item => new GlobalCodeCustomModel
                {
                    GlobalCodes = item,
                    GlobalCodeCustomValue = GetGlobalCategoryNameById(item.GlobalCodeCategoryValue)
                }));
            }

            return list;
        }


        public List<GlobalCodeCustomModel> DeleteGlobalCodeById(int globalCodeId, string category, bool withList = true)
        {
            var list = new List<GlobalCodeCustomModel>();

            _repository.Delete(globalCodeId);
            if (withList)
                list = GetAllGlobalCodes(category);

            return list;
        }


        /// <summary>
        /// Gets the global code by global code identifier.
        /// </summary>
        /// <param name="globalCodeId">The global code identifier.</param>
        /// <returns></returns>
        public GlobalCodeCustomModel GetGlobalCodeCustomById(int globalCodeId)
        {
            var cModel = new GlobalCodeCustomModel { GlobalCodes = new GlobalCodes() };

            var globalCode = _repository.Where(s => s.IsDeleted == false && s.GlobalCodeID == globalCodeId).FirstOrDefault();
            if (globalCode != null)
            {
                cModel.GlobalCodes = globalCode;
                cModel.GlobalCodeCustomValue = GetGlobalCategoryNameById(globalCode.GlobalCodeCategoryValue);
            }

            return cModel;
        }

        public List<GlobalCodes> DeleteRecordAndGetGlobalCodesList(int globalCodeId, string category)
        {
            List<GlobalCodes> list;

            _repository.Delete(globalCodeId);
            list = GetGCodesListByCategoryValue(category);

            return list;
        }

        public List<GlobalCodeCustomModel> ShowInActiveRecordsByCategoryValue(string categoryValue, bool showInActive)
        {
            var list = new List<GlobalCodeCustomModel>();

            var globalCodes = showInActive
                ? _repository.Where(s => s.GlobalCodeCategoryValue.Equals(categoryValue) && !s.IsActive && s.IsDeleted == false).ToList()
                : _repository.Where(
                    s => s.IsActive && s.GlobalCodeCategoryValue.Equals(categoryValue) && s.IsDeleted == false)
                    .ToList();
            if (globalCodes.Count > 0)
            {
                list.AddRange(globalCodes.Select(item => new GlobalCodeCustomModel
                {
                    GlobalCodes = item,
                    GlobalCodeCustomValue = GetGlobalCategoryNameById(item.GlobalCodeCategoryValue)
                }));
            }

            return list;
        }


        /// <summary>
        /// Gets the global code by global code category identifier.
        /// </summary>
        /// <param name="categoryValue">The global code category identifier.</param>
        /// <returns></returns>
        public List<string> GetGlobalCodesLabelsListByCategoryValue(string categoryValue)
        {

            var list = _repository.Where(s => s.IsDeleted == false && s.GlobalCodeCategoryValue.Equals(categoryValue)).OrderBy(g => g.SortOrder).Select(g => g.GlobalCodeName).ToList();
            return list;

        }

        public bool CheckDuplicateVital(int id, string categoryValue, string value, string unitOfMeasure)
        {
            value = value.ToLower();
            var gc = id > 0
                ? _repository.Where(
                    x =>
                        x.IsDeleted == false && x.GlobalCodeValue.ToLower().Equals(value) && x.GlobalCodeID != id &&
                        x.GlobalCodeCategoryValue.Equals(categoryValue) && x.ExternalValue1.Equals(unitOfMeasure)).FirstOrDefault()
                : _repository.Where(x =>
                        x.IsDeleted == false && x.GlobalCodeValue.ToLower().Equals(value) &&
                        x.GlobalCodeCategoryValue.Equals(categoryValue) && x.ExternalValue1.Equals(unitOfMeasure)).FirstOrDefault();
            return gc != null;

        }

        public GlobalCodes GetGlobalCodeByFacilityAndCategory(string category, string facilityNumber)
        {
            var model =
                _repository.Where(g => g.GlobalCodeCategoryValue.Equals(category) && g.FacilityNumber.Equals(facilityNumber) && g.IsDeleted == false && g.IsActive)
                    .FirstOrDefault();
            return model ?? new GlobalCodes();

        }

        public GlobalCodes GetGlobalCodeByFacilityAndCategoryForSecurityparameter(string category, string facilityNumber)
        {
            var model =
                _repository.Where(g => g.GlobalCodeCategoryValue.Equals(category) && g.FacilityNumber.Equals(facilityNumber) && g.IsDeleted == false && g.IsActive)
                    .FirstOrDefault();
            return model;

        }

        public int GetMaxGlobalCodeValueByCategory(string categoryValue, string facilityNumber)
        {
            var maxValue = 0;
            var list =
                _repository.Where(g => g.GlobalCodeCategoryValue.Equals(categoryValue) && !string.IsNullOrEmpty(g.GlobalCodeValue) && g.FacilityNumber.Equals(facilityNumber))
                    .Select(m => m.GlobalCodeValue).ToList();
            if (list.Count > 0)
                maxValue = list.Select(int.Parse).Max();
            return maxValue;

        }

        public List<int> GetDefaultMonthAndYearByFacilityId(int facilityId, int corporateId)
        {
            var currentDateTime = GetInvariantCultureDateTime(facilityId);
            var result = new List<int>();

            var current = _idlRepository.Where(f => f.FacilityId == facilityId && f.CorporateId == corporateId && f.Year == currentDateTime.Year).FirstOrDefault();

            if (current != null)
            {
                result.Add(Convert.ToInt32(current.ExternalValue1));        //Default Year
                result.Add(Convert.ToInt32(current.ExternalValue2));        //Default Month
            }

            return result;
        }

        /// <summary>
        /// Gets all global codes.
        /// </summary>
        /// <param name="categoryValue">The category value.</param>
        /// <returns></returns>
        public List<GlobalCodeCustomModel> GetSubCategoriesList(string categoryValue)
        {
            var list = new List<GlobalCodeCustomModel>();
            var globalCodes = _repository.Where(
                s => s.IsDeleted == false && s.IsActive && s.GlobalCodeCategoryValue.Equals(categoryValue))
                .ToList();

            if (globalCodes.Count > 0)
            {
                if (categoryValue.Trim().Equals("4347"))
                {
                    list.AddRange(globalCodes.Select(item => new GlobalCodeCustomModel
                    {
                        SubCategory1 = item.GlobalCodeName,
                        GcValue = item.GlobalCodeValue,
                        SubCategory2 = string.Empty,
                        GcId = item.GlobalCodeID
                    }));
                }
                else
                {
                    list.AddRange(globalCodes.Select(item => new GlobalCodeCustomModel
                    {
                        SubCategory1 = !string.IsNullOrEmpty(item.ExternalValue1) ? GetSubCategory1Name(item.ExternalValue1) : string.Empty,
                        GcValue = item.GlobalCodeValue,
                        SubCategory2 = item.GlobalCodeName,
                        GcId = item.GlobalCodeID,
                    }));
                }
                list = list.OrderBy(g => int.Parse(g.GcValue)).ToList();
            }
            return list;
        }
        /// <summary>
        /// Gets all global codes.
        /// </summary>
        /// <param name="categoryValue">The category value.</param>
        /// <returns></returns>
        public List<GlobalCodeCustomModel> GetSubCategoriesListBySubCategory1Value(string categoryValue, string selectedValue)
        {
            var list = new List<GlobalCodeCustomModel>();
            var globalCodes = _repository.Where(
                s => s.IsDeleted == false && s.IsActive && s.GlobalCodeCategoryValue.Equals(categoryValue) && s.ExternalValue1.Equals(selectedValue))
                .ToList();

            if (globalCodes.Count > 0)
            {
                if (categoryValue.Trim().Equals("4347"))
                {
                    list.AddRange(globalCodes.Select(item => new GlobalCodeCustomModel
                    {
                        SubCategory1 = item.GlobalCodeName,
                        GcValue = item.GlobalCodeValue,
                        SubCategory2 = string.Empty,
                        GcId = item.GlobalCodeID
                    }));
                }
                else
                {
                    list.AddRange(globalCodes.Select(item => new GlobalCodeCustomModel
                    {
                        SubCategory1 = !string.IsNullOrEmpty(item.ExternalValue1) ? GetSubCategory1Name(item.ExternalValue1) : string.Empty,
                        GcValue = item.GlobalCodeValue,
                        SubCategory2 = item.GlobalCodeName,
                        GcId = item.GlobalCodeID,
                    }));
                }
                list = list.OrderBy(g => int.Parse(g.GcValue)).ToList();
            }
            return list;
        }

        private string GetSubCategory1Name(string gcValue1)
        {
            var sc2 = string.Empty;
            var current =
                _repository.Where(
                    g => g.GlobalCodeCategoryValue.Trim().Equals("4347") && g.GlobalCodeValue.Trim().Equals(gcValue1))
                    .FirstOrDefault();
            if (current != null)
                sc2 = current.GlobalCodeName;
            return sc2;
        }

        public GlobalCodes GetIndicatorSettingsByCorporateId(string corporateId)
        {
            var categoryValue = Convert.ToString((int)GlobalCodeCategoryValue.DashboardIndicatorSettings);
            var maxValue = GetMaxGlobalCodeValueByCategory(categoryValue) + 1;
            var result =
                _repository.Where(
                    g =>
                        g.ExternalValue1.ToLower().Trim().Equals(corporateId) &&
                        g.GlobalCodeCategoryValue.ToLower().Trim().Equals(categoryValue)).FirstOrDefault();
            return result ?? new GlobalCodes
            {
                GlobalCodeID = 0,
                GlobalCodeCategoryValue = categoryValue,
                GlobalCodeValue = Convert.ToString(maxValue),
                ExternalValue1 = corporateId,
                ExternalValue2 = string.Empty
            };
        }



        public List<GlobalCodeCustomModel> GetActiveInActiveRecord(string categoryValue, bool showInActive)
        {
            var list = new List<GlobalCodeCustomModel>();
            var globalCodes = !string.IsNullOrEmpty(categoryValue) ? _repository.Where(s => s.IsDeleted == false && s.IsActive == showInActive && s.GlobalCodeCategoryValue.Equals(categoryValue)).ToList() : _repository.Where(s => s.IsDeleted == false && s.IsActive).ToList();
            if (globalCodes.Count > 0)
            {
                var unitOfMeasure = Convert.ToString(Convert.ToInt32(GlobalCodeCategoryValue.UnitOfMeasure));
                list.AddRange(globalCodes.Select(item => new GlobalCodeCustomModel
                {
                    GlobalCodes = item,
                    GlobalCodeCustomValue = GetGlobalCategoryNameById(item.GlobalCodeCategoryValue),
                    UnitOfMeasure =
                        !string.IsNullOrEmpty(item.ExternalValue3)
                            ? GetNameByGlobalCodeValueAndCategoryValue(unitOfMeasure, item.ExternalValue3)
                            : string.Empty
                }));
            }
            return list;
        }

        /// <summary>
        /// Gets the global codes by categories range.
        /// </summary>
        /// <param name="startRange">The start range.</param>
        /// <param name="endRange">The end range.</param>
        /// <param name="blockNumber"></param>
        /// <param name="blockSize"></param>
        /// <param name="skip"></param>
        /// <param name="showInActive"></param>
        /// <returns></returns>
        public List<GlobalCodeCustomModel> GetGlobalCodesByCategoriesRangeOnDemand(int startRange, int endRange, int blockNumber, int blockSize, bool skip, bool showInActive, long facilityId = 0, long corporateId = 0)
        {
            var startIndex = (blockNumber - 1) * blockSize;

            var list = new List<GlobalCodeCustomModel>();
            var fnumber = facilityId > 0 ? Convert.ToString(facilityId) : string.Empty;
            var gModellist =
                _repository.Where(f => f.IsDeleted != null && !(bool)f.IsDeleted && f.IsActive == showInActive && (string.IsNullOrEmpty(fnumber) || f.FacilityNumber.Equals(fnumber)))
                    .ToList()
                    .Where(
                        g =>
                            Convert.ToInt32(g.GlobalCodeCategoryValue) >= startRange &&
                            Convert.ToInt32(g.GlobalCodeCategoryValue) <= endRange)
                    .OrderByDescending(v => v.CreatedDate)
                    .Skip(skip ? 0 : startIndex)
                    .Take(blockSize)
                    .ToList();

            if (gModellist.Count > 0)
            {
                list.AddRange(gModellist.Select(items => new GlobalCodeCustomModel
                {
                    GlobalCodes = items,
                    GlobalCodeCustomValue = GetGlobalCategoryNameById(items.GlobalCodeCategoryValue, items.FacilityNumber),
                    CodeType = GetNameByGlobalCodeValue(items.ExternalValue1, "5101")

                }));
            }
            return list;
        }
        private string GetNameByGlobalCodeValue(string codeValue, string categoryValue, string fId = "")
        {
            if (!string.IsNullOrEmpty(codeValue))
            {
                var gl = _repository.Where(g => g.GlobalCodeValue.Equals(codeValue) && !g.IsDeleted.Value && g.GlobalCodeCategoryValue.Equals(categoryValue) && (string.IsNullOrEmpty(fId) || g.FacilityNumber.Equals(fId))).FirstOrDefault();
                return gl != null ? gl.GlobalCodeName : string.Empty;
            }
            return string.Empty;
        }

        public List<GlobalCodeCustomModel> GetGlobalCodesByCategoriesRangeOnDemand(string gcc, int blockNumber, int blockSize, bool skip, bool showInActive, long facilityId = 0, long corporateId = 0)
        {
            var startIndex = (blockNumber - 1) * blockSize;
            var fnumber = facilityId > 0 ? Convert.ToString(facilityId) : string.Empty;

            var list = new List<GlobalCodeCustomModel>();

            gcc = gcc.Trim();
            var gModellist =
                _repository.Where(
                    f => f.IsDeleted != null && !(bool)f.IsDeleted && f.GlobalCodeCategoryValue.Trim().Equals(gcc) && f.IsActive == showInActive
                    && (string.IsNullOrEmpty(fnumber) || f.FacilityNumber.Equals(fnumber)))
                    .OrderByDescending(v => v.CreatedDate)
                    .Skip(skip ? 0 : startIndex)
                    .Take(blockSize)
                    .ToList();

            if (gModellist.Count > 0)
            {
                list.AddRange(gModellist.Select(items => new GlobalCodeCustomModel
                {
                    GlobalCodes = items,
                    GlobalCodeCustomValue = GetGlobalCategoryNameById(items.GlobalCodeCategoryValue, items.FacilityNumber),
                    CodeType = GetNameByGlobalCodeValue(items.ExternalValue1, "5101")
                }));
            }
            return list;
        }


        /// <summary>
        /// Gets the room equipment list.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="roomid">The roomid.</param>
        /// <returns></returns>
        public List<GlobalCodes> GetRoomEquipmentList(string facilityId, string roomid)
        {
            var roomEquipmentsList = _repository.Where(
                  x =>
                  x.FacilityNumber == facilityId && x.ExternalValue1 == roomid && x.GlobalCodeCategoryValue == "4910")
                  .ToList();
            return roomEquipmentsList;

        }
        private string GetFacilityStructureNameById(int id)
        {
            var roomObj = _fsRepository.Where(x => x.FacilityStructureId == id).FirstOrDefault();
            return roomObj != null ? roomObj.FacilityStructureName : string.Empty;
        }

        /// <summary>
        /// Gets the room equipment all list.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<GlobalCodeCustomDModel> GetRoomEquipmentALLList(string facilityId)
        {
            var list = new List<GlobalCodeCustomDModel>();
            var roomEquipmentsList = _repository.Where(
                  x =>
                  x.FacilityNumber == facilityId && x.GlobalCodeCategoryValue == "4910")
                  .ToList();
            list.AddRange(roomEquipmentsList.Select(items => new GlobalCodeCustomDModel
            {
                CorporateId = GetCorporateIdFromFacilityId(Convert.ToInt32(items.FacilityNumber)),
                RoomId = (items.ExternalValue1),
                GlobalCodeID = items.GlobalCodeID,
                GlobalCodeName = items.GlobalCodeName,
                GlobalCodeCategoryValue = items.GlobalCodeCategoryValue,
                GlobalCodeValue = items.GlobalCodeValue,
                RoomSTR = GetFacilityStructureNameById(Convert.ToInt32(items.ExternalValue1)),
                FacilityNumber = items.FacilityNumber
            }));
            return list;

        }
        public int GetCorporateIdFromFacilityId(int facilityid)
        {
            var id = 0;
            var obj = _fRepository.Where(f => f.FacilityId == facilityid).FirstOrDefault();
            if (obj != null)
                id = Convert.ToInt32(obj.CorporateID);
            return id;
        }


        /// <summary>
        /// Deletes the global code.
        /// </summary>
        /// <param name="globalCodeId">The global code identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<GlobalCodeCustomDModel> DeleteGlobalCode(int globalCodeId, string facilityId)
        {
            List<GlobalCodeCustomDModel> list;
            _repository.Delete(globalCodeId);
            list = GetRoomEquipmentALLList(facilityId);
            return list;
        }


        /// <summary>
        /// Gets the global code name by value and category identifier.
        /// </summary>
        /// <param name="categoryId">The category identifier.</param>
        /// <param name="globalCodeVal">The global code value.</param>
        /// <returns></returns>
        public string GetGlobalCodeNameByValueAndCategoryId(string categoryId, string globalCodeVal)
        {
            var globalCode = _repository.Where(c => c.GlobalCodeCategoryValue.Equals(categoryId) && c.GlobalCodeValue == globalCodeVal).FirstOrDefault();
            if (globalCode != null)
                return globalCode.GlobalCodeName;
            return string.Empty;
        }

        /// <summary>
        /// Gets the global code sotring by value and category identifier.
        /// </summary>
        /// <param name="categoryId">The category identifier.</param>
        /// <param name="globalCodeVal">The global code value.</param>
        /// <returns></returns>
        public int GetGlobalCodeSotringByValueAndCategoryId(string categoryId, string globalCodeVal)
        {
            var globalCode = _repository.Where(c => c.GlobalCodeCategoryValue.Equals(categoryId) && c.GlobalCodeValue == globalCodeVal).FirstOrDefault();
            if (globalCode != null)
                return Convert.ToInt32(globalCode.SortOrder);

            return 0;
        }


        /// <summary>
        /// Gets the list by categories range.
        /// </summary>
        /// <param name="categories">The categories.</param>
        /// <returns></returns>
        public List<DropdownListData> GetListByCategoriesRange(IEnumerable<string> categories, string facilityId = "0")
        {
            var list = new List<DropdownListData>();
            var gc =
                _repository.Where(g => g.IsActive && g.IsDeleted == false && categories.Contains(g.GlobalCodeCategoryValue) && (g.FacilityNumber.Equals(facilityId)))
                    .ToList();

            if (gc.Any())
            {
                gc = gc.OrderBy(g => g.GlobalCodeCategoryValue).ThenBy(g1 => g1.GlobalCodeName).ToList();
                list.AddRange(gc.Select(item => new DropdownListData
                {
                    Text = item.GlobalCodeName,
                    Value = item.GlobalCodeValue,
                    ExternalValue1 = item.GlobalCodeCategoryValue,
                    ExternalValue2 = item.ExternalValue1 ?? "0",
                    //SortOrder = Convert.ToInt32(item.GlobalCodeValue)
                }));
            }
            return list;
        }

        public List<DropdownListData> GetListByCategoriesRange(IEnumerable<string> categories, IEnumerable<string> extValue1)
        {
            var list = new List<DropdownListData>();
            var gc =
                _repository.Where(g => g.IsActive && g.IsDeleted == false && categories.Contains(g.GlobalCodeCategoryValue) && (!extValue1.Any() || extValue1.Contains(g.ExternalValue1))).ToList();
            if (gc.Any())
            {
                gc = gc.OrderBy(g => g.GlobalCodeCategoryValue).ThenBy(g1 => g1.GlobalCodeName).ToList();
                list.AddRange(gc.Select(item => new DropdownListData
                {
                    Text = item.GlobalCodeName,
                    Value = item.GlobalCodeValue,
                    ExternalValue1 = item.ExternalValue1,
                    ExternalValue2 = item.GlobalCodeCategoryValue
                    //SortOrder = Convert.ToInt32(item.GlobalCodeValue)
                }));
            }
            return list;
        }

        /// <summary>
        /// Creates the order activity scheduler timming.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public bool CreateOrderActivitySchedulerTimming(int id)
        {
            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("GlobalCodeId", id);
            _repository.ExecuteCommand(StoredProcedures.SPROC_CreateOpenOrderActivityTimeForFrequency.ToString(), sqlParameters);
            return true;
        }



        public List<DropdownListData> GetGCodesListByCategoryValue(string categoryValue, IEnumerable<string> extValues1, string withoutValue)
        {
            var list = new List<DropdownListData>();
            var gclist = _repository.Where(s => s.IsDeleted == false && s.GlobalCodeCategoryValue.Equals(categoryValue) && (extValues1.Count() == 0 || extValues1.Contains(s.ExternalValue1)) && (string.IsNullOrEmpty(withoutValue) || !s.ExternalValue1.Equals(withoutValue))).OrderBy(g => g.SortOrder).ToList();

            if (gclist.Count > 0)
            {
                list.AddRange(gclist.Select(item => new DropdownListData
                {
                    Text = item.GlobalCodeName,
                    Value = item.GlobalCodeValue,
                    ExternalValue1 = item.ExternalValue1,
                    SortOrder = Convert.ToInt32(item.GlobalCodeValue)
                }));
            }

            return list;
        }


        public List<GlobalCodes> GetGCodesListByCategoryValue(string categoryValue, string externalValue1, string externalValue3)
        {
            var list =
                _repository.Where(
                    s =>
                        s.IsDeleted == false && s.GlobalCodeCategoryValue.Equals(categoryValue) &&
                        !s.ExternalValue1.Equals(externalValue1) && !s.ExternalValue3.Equals(externalValue3))
                    .OrderBy(g => g.SortOrder)
                    .ToList();
            return list;

        }



        public List<GlobalCodeCustomModel> GetGlobalCodesByCategory(string categoryValue, long corporateId, long facilityId, long userId, long id, out long newId, bool listStatus, bool isFacilityPassed = false)
        {
            newId = 0;
            var list = new List<GlobalCodeCustomModel>();
            var sqlParameters = new SqlParameter[7];
            sqlParameters[0] = new SqlParameter("pCategoryValue", categoryValue);
            sqlParameters[1] = new SqlParameter("pFId", facilityId);
            sqlParameters[2] = new SqlParameter("pCId", corporateId);
            sqlParameters[3] = new SqlParameter("pUId", userId);
            sqlParameters[4] = new SqlParameter("pId", id);
            sqlParameters[5] = new SqlParameter("pFacilitySpecific", isFacilityPassed);
            sqlParameters[6] = new SqlParameter("pStatus", listStatus);
            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetGlobalCodesByCategory.ToString(), isCompiled: false, parameters: sqlParameters))
            {
                var codes = r.ResultSetFor<GlobalCodes>().ToList();
                var globalCodeName = r.ResultSetFor<string>().FirstOrDefault();
                var unitOfMeasures = new List<GlobalCodeCustomModel>();

                //Get the New Global Code Value for the new record to be added in the table.
                var value = r.ResultSetFor<long?>().FirstOrDefault();

                newId = value.HasValue ? value.Value : 1;

                //Check if there is a list of UnitOfMeasures
                if (codes.Any(a => !string.IsNullOrEmpty(a.ExternalValue3)))
                    unitOfMeasures = r.ResultSetFor<GlobalCodeCustomModel>().ToList();

                foreach (var c in codes)
                {
                    var um = string.Empty;
                    if (unitOfMeasures.Any())
                        um = unitOfMeasures.Where(m => m.GcId == c.GlobalCodeID).FirstOrDefault().UnitOfMeasure;

                    list.Add(new GlobalCodeCustomModel
                    {
                        GlobalCodes = c,
                        GlobalCodeCustomValue = globalCodeName,
                        UnitOfMeasure = um
                    });
                }
            }
            return list;
        }


        public OrderCodes GetOrderCodesByRange(string tableNo, string categoryValue, string subCategoryValue, string orderCode, long startRange, long endRange, long fId)
        {
            var vm = new OrderCodes();

            var sqlParams = new SqlParameter[7];
            sqlParams[0] = new SqlParameter("@pTableNumber", tableNo);
            sqlParams[1] = new SqlParameter("@pCategoryValue", categoryValue);
            sqlParams[2] = new SqlParameter("@pSubCategoryValue", subCategoryValue);
            sqlParams[3] = new SqlParameter("@pOrderCodeType", orderCode);
            sqlParams[4] = new SqlParameter("@pStartRange", startRange);
            sqlParams[5] = new SqlParameter("@pEndRange", endRange);
            sqlParams[6] = new SqlParameter("@pFId", fId);

            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetOrderCodesBySubCategoryValue.ToString(), isCompiled: false, parameters: sqlParams))
            {
                try
                {
                    vm.GlobalCode = r.ResultSetFor<GlobalCodes>().FirstOrDefault();
                    vm.OrderCodeList = r.ResultSetFor<DropdownListData>().ToList();
                }
                catch (Exception ex)
                {
                    //throw;
                }
            }
            return vm;
        }
    }
}
