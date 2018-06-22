using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.BusinessAccess
{
    public class ErrorMasterBal : BaseBal
    {
        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<ErrorMasterCustomModel> GetErrorListByCorporateAndFacilityId(int corporateId, int facilityId, bool? showInactive)
        {
            var list = new List<ErrorMasterCustomModel>();
            var inactivefrecords = !showInactive;
            using (var errorMasterRep = UnitOfWork.ErrorMasterRepository)
            {
                var lstErrorMaster = (corporateId > 0 && facilityId > 0)
                    ? errorMasterRep.Where(
                        a =>
                            a.IsActive != null && (bool) a.IsActive == inactivefrecords &&
                            (a.CorporateID != null && (int) a.CorporateID == corporateId) &&
                            (a.FacilityID != null && (int) a.FacilityID == facilityId)).ToList()
                    : errorMasterRep.Where(a => a.IsActive != null && (bool)a.IsActive == inactivefrecords).ToList();

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

                    //MergePredefinedDenialsWithErrorlist(list);
                }
            }
            return list;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddUptdateErrorMaster(ErrorMaster model)
        {
            using (var errorMasterRep = UnitOfWork.ErrorMasterRepository)
            {
                if (model.ErrorMasterID > 0)
                    errorMasterRep.UpdateEntity(model, model.ErrorMasterID);
                else
                    errorMasterRep.Create(model);
                return model.ErrorMasterID;
            }
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="errorMasterId"></param>
        /// <returns></returns>
        public ErrorMaster GetErrorMasterById(int? errorMasterId)
        {
            using (var errorMasterRep = UnitOfWork.ErrorMasterRepository)
            {
                var errorMaster = errorMasterRep.Where(x => x.ErrorMasterID == errorMasterId).FirstOrDefault();
                return errorMaster;
            }
        }

        public IEnumerable<ErrorMasterCustomModel> GetSearchedDenialsList(string text)
        {
            var list = new List<ErrorMasterCustomModel>();
            using (var errorMasterRep = UnitOfWork.ErrorMasterRepository)
            {
                text = string.IsNullOrEmpty(text) ? string.Empty : text.ToLower().Trim();
                var lstErrorMaster = errorMasterRep.Where(a => a.IsActive != null && (bool)a.IsActive && (a.ErrorDescription.ToLower().Contains(text) || a.ErrorCode.ToLower().Contains(text))).ToList();
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

                    //MergePredefinedDenialsWithErrorlist(list);

                }
            }
            return list;
        }

        private void MergePredefinedDenialsWithErrorlist(List<ErrorMasterCustomModel> errorList)
        {
            using (var bal = new DenialBal())
            {
                var denialCodes = bal.GetDenial();
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
}

