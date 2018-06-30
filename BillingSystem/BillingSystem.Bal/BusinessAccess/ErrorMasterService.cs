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
    public class ErrorMasterService : IErrorMasterService
    {
        private readonly IRepository<ErrorMaster> _repository;
        private readonly IRepository<Denial> _dRepository;
        private readonly IRepository<GlobalCodes> _gRepository;
        private readonly IMapper _mapper;

        public ErrorMasterService(IRepository<ErrorMaster> repository, IRepository<Denial> dRepository, IRepository<GlobalCodes> gRepository, IMapper mapper)
        {
            _repository = repository;
            _dRepository = dRepository;
            _gRepository = gRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<ErrorMasterCustomModel> GetErrorListByCorporateAndFacilityId(int corporateId, int facilityId, bool? showInactive)
        {
            var list = new List<ErrorMasterCustomModel>();
            var inactivefrecords = !showInactive;
            var lstErrorMaster = (corporateId > 0 && facilityId > 0)
                ? _repository.Where(
                    a =>
                        a.IsActive != null && (bool)a.IsActive == inactivefrecords &&
                        (a.CorporateID != null && (int)a.CorporateID == corporateId) &&
                        (a.FacilityID != null && (int)a.FacilityID == facilityId)).ToList()
                : _repository.Where(a => a.IsActive != null && (bool)a.IsActive == inactivefrecords).ToList();

            if (lstErrorMaster.Count > 0)
            {
                list.AddRange(lstErrorMaster.Select(item => new ErrorMasterCustomModel
                {
                    ErrorCode = item.ErrorCode,
                    ErrorDescription = item.ErrorDescription,
                    ErrorMasterID = item.ErrorMasterID,
                    CreatedBy = item.CreatedBy,
                    CreatedDate = item.CreatedDate,
                    ErrorResolution = item.ErrorResolution,
                    ErrorType = item.ErrorType,
                    ExtValue1 = item.ExtValue1,
                    ExtValue2 = item.ExtValue2,
                    ExtValue3 = item.ExtValue3,
                    ExtValue4 = item.ExtValue4,
                    FacilityID = item.FacilityID,
                    IsActive = item.IsActive,
                    ModifiedBy = item.ModifiedBy,
                    ModifiedDate = item.ModifiedDate,
                    IsPredefinedDenial = false,
                    ErrorTypeString = GetNameByGlobalCodeValue(Convert.ToString(item.ErrorType), Convert.ToInt32(GlobalCodeCategoryValue.ErrorTypes).ToString())
                }));

            }
            return list;
        }
        private string GetNameByGlobalCodeValue(string codeValue, string categoryValue, string fId = "")
        {
            if (!string.IsNullOrEmpty(codeValue))
            {
                var gl = _gRepository.Where(
                        g => g.GlobalCodeValue.Equals(codeValue) && !g.IsDeleted.Value && g.GlobalCodeCategoryValue.Equals(categoryValue) && (string.IsNullOrEmpty(fId) || g.FacilityNumber.Equals(fId)))
                        .FirstOrDefault();
                return gl != null ? gl.GlobalCodeName : string.Empty;

            }
            return string.Empty;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddUptdateErrorMaster(ErrorMaster model)
        {
            if (model.ErrorMasterID > 0)
                _repository.UpdateEntity(model, model.ErrorMasterID);
            else
                _repository.Create(model);
            return model.ErrorMasterID;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="errorMasterId"></param>
        /// <returns></returns>
        public ErrorMaster GetErrorMasterById(int? errorMasterId)
        {
            var errorMaster = _repository.Where(x => x.ErrorMasterID == errorMasterId).FirstOrDefault();
            return errorMaster;
        }

        public IEnumerable<ErrorMasterCustomModel> GetSearchedDenialsList(string text)
        {
            var list = new List<ErrorMasterCustomModel>();
            text = string.IsNullOrEmpty(text) ? string.Empty : text.ToLower().Trim();
            var lstErrorMaster = _repository.Where(a => a.IsActive != null && (bool)a.IsActive && (a.ErrorDescription.ToLower().Contains(text) || a.ErrorCode.ToLower().Contains(text))).ToList();
            if (lstErrorMaster.Count > 0)
            {
                list.AddRange(lstErrorMaster.Select(item => new ErrorMasterCustomModel
                {
                    ErrorCode = item.ErrorCode,
                    ErrorDescription = item.ErrorDescription,
                    ErrorMasterID = item.ErrorMasterID,
                    CreatedBy = item.CreatedBy,
                    CreatedDate = item.CreatedDate,
                    ErrorResolution = item.ErrorResolution,
                    ErrorType = item.ErrorType,
                    ExtValue1 = item.ExtValue1,
                    ExtValue2 = item.ExtValue2,
                    ExtValue3 = item.ExtValue3,
                    ExtValue4 = item.ExtValue4,
                    FacilityID = item.FacilityID,
                    IsActive = item.IsActive,
                    ModifiedBy = item.ModifiedBy,
                    ModifiedDate = item.ModifiedDate
                }));


            }
            return list;
        }

        private void MergePredefinedDenialsWithErrorlist(List<ErrorMasterCustomModel> errorList)
        {
            var lst = _dRepository.Where(x => x.IsDeleted == false).ToList();

            var denialCodes = lst.Select(x => _mapper.Map<DenialCodeCustomModel>(x)).ToList();
            errorList.AddRange(denialCodes.Select(item => new ErrorMasterCustomModel
            {
                ErrorCode = item.DenialCode,
                ErrorDescription = item.DenialDescription,
                ErrorMasterID = item.DenialSetNumber,
                CreatedBy = item.CreatedBy,
                CreatedDate = item.CreatedDate,
                ErrorResolution = string.Empty,
                ErrorType = 0,
                ExtValue1 = string.Empty,
                ExtValue2 = string.Empty,
                ExtValue3 = string.Empty,
                ExtValue4 = string.Empty,
                FacilityID = 0,
                IsActive = true,
                ModifiedBy = item.ModifiedBy,
                ModifiedDate = item.ModifiedDate,
                IsPredefinedDenial = true
            }));
        }
    }
}

