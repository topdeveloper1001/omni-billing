using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class GlobalCodeCategoryService : IGlobalCodeCategoryService
    {
        private readonly IRepository<GlobalCodeCategory> _repository;
        private readonly IRepository<Facility> _fRepository;

        public GlobalCodeCategoryService(IRepository<GlobalCodeCategory> repository, IRepository<Facility> fRepository)
        {
            _repository = repository;
            _fRepository = fRepository;
        }

        /// <summary>
        /// Add Update Global Code Category
        /// </summary>
        /// <param name="model">The global cc.</param>
        /// <returns></returns>
        public int AddUpdateGlobalCodeCategory(GlobalCodeCategory model)
        {
            if (model.GlobalCodeCategoryID > 0)
                _repository.UpdateEntity(model, model.GlobalCodeCategoryID);
            else
                _repository.Create(model);
            return model.GlobalCodeCategoryID;
        }

        //Function to get GlobalCodeCategories
        /// <summary>
        /// Gets the global code categories.
        /// </summary>
        /// <returns></returns>
        public List<GlobalCodeCategory> GetGlobalCodeCategories()
        {
            var lst = _repository.GetAll().OrderBy(_ => _.GlobalCodeCategoryName).ToList();
            return lst;
        }

        //Function to get GlobalCodeCategories
        /// <summary>
        /// Gets the global code categories by null facility.
        /// </summary>
        /// <returns></returns>
        public List<GlobalCodeCategory> GetGlobalCodeCategoriesByNullFacility()
        {
            var lst = _repository.GetAll().Where(f => f.FacilityNumber == "0").OrderBy(_ => _.GlobalCodeCategoryName).ToList();
            return lst;
        }

        //Function to get GlobalCodeCategoriesby facility number
        /// <summary>
        /// Gets the facility global code categories.
        /// </summary>
        /// <param name="facilityNumber">The facility number.</param>
        /// <returns></returns>
        public List<GlobalCodeCategory> GetFacilityGlobalCodeCategories(string facilityNumber)
        {
            var lst = _repository.Where(_ => _.FacilityNumber.Equals(facilityNumber)).OrderBy(_ => _.GlobalCodeCategoryName).ToList();
            return lst;

        }

        //Function to Get GlobalCode Category Range
        /// <summary>
        /// Gets the global code categories range.
        /// </summary>
        /// <param name="startRange">The start range.</param>
        /// <param name="finishRange">The finish range.</param>
        /// <returns></returns>
        public List<GlobalCodeCategory> GetGlobalCodeCategoriesRange(int startRange, int finishRange)
        {
            var lst = _repository.GetAll().ToList().Where(x => x.IsActive && (x.IsDeleted != null || x.IsDeleted == false) && Convert.ToInt32(x.GlobalCodeCategoryValue) >= startRange && Convert.ToInt32(x.GlobalCodeCategoryValue) <= finishRange).OrderBy(y => y.GlobalCodeCategoryName).ToList();
            return lst;
        }

        //Function to get GlobalCodeCategoryCustomModel
        /// <summary>
        /// Gets the name of the global code categories with facility.
        /// </summary>
        /// <returns></returns>
        public List<GlobalCodeCategoryCustomModel> GetGlobalCodeCategoriesWithFacilityName()
        {
            var lst = new List<GlobalCodeCategoryCustomModel>();
            var globalCode = _repository.GetAll().ToList();
            var facility = _fRepository.GetAll().ToList();
            lst = (from global in globalCode
                   join fac in facility on global.FacilityNumber equals fac.FacilityNumber
                   select new GlobalCodeCategoryCustomModel
                   {
                       GlobalCodeCategoryName = global.GlobalCodeCategoryName,
                       GlobalCodeCategoryValue = global.GlobalCodeCategoryValue,
                       FacilityName = fac.FacilityName
                   }).ToList();
            return lst;
        }

        //Function to get GlobalCodeCategories by Id
        /// <summary>
        /// Gets the global code category by value.
        /// </summary>
        /// <param name="globalCodeCategoryvalue">The global code categoryvalue.</param>
        /// <returns></returns>
        public GlobalCodeCategory GetGlobalCodeCategoryByValue(string globalCodeCategoryvalue)
        {
            var m = _repository.Where(f => f.GlobalCodeCategoryValue == globalCodeCategoryvalue).FirstOrDefault();
            return m ?? new GlobalCodeCategory();
        }

        //Function to get GlobalCodeCategories by Category Id
        /// <summary>
        /// Gets the global code categories by category identifier.
        /// </summary>
        /// <param name="GlobalCodeCategoryId">The global code category identifier.</param>
        /// <returns></returns>
        public GlobalCodeCategory GetCurrentCategoryById(int GlobalCodeCategoryId)
        {
            var m = _repository.Where(f => f.GlobalCodeCategoryID == GlobalCodeCategoryId).FirstOrDefault();
            return m;
        }

        public List<GlobalCodeCategory> GetSearchedCategories(string text, string externalValue)
        {
            var searchedResult = _repository.Where(r => r.ExternalValue1.Equals(externalValue) && (r.GlobalCodeCategoryName.Contains(text) || r.GlobalCodeCategoryValue.Contains(text))).ToList();
            return searchedResult;
        }

        /// <summary>
        /// Gets the global code category by value.
        /// </summary>
        /// <param name="categoryValue"></param>
        /// <returns></returns>
        public List<GlobalCodeCategory> GetListByCategoryValue(string categoryValue)
        {
            var list = _repository.Where(f => f.ExternalValue1.Equals(categoryValue) && (f.IsDeleted != null && !(bool)f.IsDeleted)).ToList();
            return list;
        }

        public bool CheckDuplicateCode(GlobalCodeCategory model)
        {
            var result = model.GlobalCodeCategoryID > 0 ?
            _repository.Where(
                g =>
                    g.GlobalCodeCategoryID != model.GlobalCodeCategoryID &&
                    g.GlobalCodeCategoryValue.Equals(model.GlobalCodeCategoryValue)).Any() :
            _repository.Where(
                g => g.GlobalCodeCategoryValue.Equals(model.GlobalCodeCategoryValue)).Any();
            return result;
        }

        public List<GlobalCodeCategory> GetGlobalCodeCategoriesByExternalValue(string facilityNumber = "")
        {
            var listCategory = _repository.Where(x =>
                x.IsActive && x.IsDeleted != true &&
                x.ExternalValue3.Trim().ToLower().Equals("ordercategory")
                &&
                (string.IsNullOrEmpty(facilityNumber) || x.FacilityNumber.Equals(facilityNumber)))
                .OrderBy(x => x.GlobalCodeCategoryName)
                .ToList();
            return listCategory;
        }
    }
}
