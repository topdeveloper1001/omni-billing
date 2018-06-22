using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Common.Common;
using BillingSystem.Repository.UOW;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.BusinessAccess
{
    public class LabTestResultBal : BaseBal
    {
        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<LabTestResultCustomModel> GetLabTestResult()
        {
            var list = new List<LabTestResultCustomModel>();
            using (var LabTestResultRep = UnitOfWork.LabTestResultRepository)
            {
                var lstLabTestResult = LabTestResultRep.Where(a => a.IsDeleted == null || !(bool)a.IsDeleted).ToList();
                if (lstLabTestResult.Count > 0)
                {
                    list.AddRange(lstLabTestResult.Select(item => new LabTestResultCustomModel
                    {
                        LabTestResultID = item.LabTestResultID,
                        LabTestResultTableNumber = item.LabTestResultTableNumber,
                        LabTestResultTableName = item.LabTestResultTableName,
                        LabTestResultCPTCode = item.LabTestResultCPTCode,
                        LabTestResultTestName = item.LabTestResultTestName,
                        LabTestResultSpecimen = item.LabTestResultSpecimen,
                        LabTestResultGender = item.LabTestResultGender,
                        LabTestResultAgeFrom = item.LabTestResultAgeFrom,
                        LabTestResultAgeTo = item.LabTestResultAgeTo,
                        LabTestResultMeasurementValue = item.LabTestResultMeasurementValue,
                        LabTestResultLowRangeResult = item.LabTestResultLowRangeResult,
                        LabTestResultHighRangeResult = item.LabTestResultHighRangeResult,
                        LabTestResultGoodFrom = item.LabTestResultGoodFrom,
                        LabTestResultGoodTo = item.LabTestResultGoodTo,
                        LabTestResultCautionFrom = item.LabTestResultCautionFrom,
                        LabTestResultCautionTo = item.LabTestResultCautionTo,
                        LabTestResultBadFrom = item.LabTestResultBadFrom,
                        LabTestResultBadTo = item.LabTestResultBadTo,
                        ModifiedBy = item.ModifiedBy,
                        Modifieddate = item.Modifieddate,
                        IsDeleted = item.IsDeleted,
                        DeletedBy = item.DeletedBy,
                        DeletedDate = item.DeletedDate,
                        CreatedBy = item.CreatedBy,
                        CreatedDate = item.CreatedDate,
                        AgeFromString = GetNameByGlobalCodeValue(item.LabTestResultAgeFrom.ToString(),Convert.ToInt32(GlobalCodeCategoryValue.LabTestResultAgeFrom).ToString()),
                        AgeToString = GetNameByGlobalCodeValue(item.LabTestResultAgeTo.ToString(), Convert.ToInt32(GlobalCodeCategoryValue.LabTestResultAgeFrom).ToString()),
                        GenderString = GetNameByGlobalCodeValue(item.LabTestResultGender.ToString(), Convert.ToInt32(GlobalCodeCategoryValue.LabTestResultGender).ToString()),
                        MeasurementValueString = GetNameByGlobalCodeValue(item.LabTestResultMeasurementValue.ToString(), Convert.ToInt32(GlobalCodeCategoryValue.LabMeasurementValue).ToString()),
                        SpecifimenString = GetNameByGlobalCodeValue(item.LabTestResultSpecimen.ToString(), Convert.ToInt32(GlobalCodeCategoryValue.LabTestResultSpecimen).ToString()),
                    }));
                }
            }
            return list;
        }

        /// <summary>
        /// Gets the lab test result by corporate facility.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<LabTestResultCustomModel> GetLabTestResultByCorporateFacility(int corporateId, int facilityId)
        {
            var list = new List<LabTestResultCustomModel>();
            using (var LabTestResultRep = UnitOfWork.LabTestResultRepository)
            {
                var lstLabTestResult = LabTestResultRep.Where(a => (a.IsDeleted == null || !(bool)a.IsDeleted) && a.CorporateId == corporateId && a.FacilityId == facilityId)
                    .ToList();
                if (lstLabTestResult.Count > 0)
                {
                    list.AddRange(lstLabTestResult.Select(item => new LabTestResultCustomModel
                    {
                        LabTestResultID = item.LabTestResultID,
                        LabTestResultTableNumber = item.LabTestResultTableNumber,
                        LabTestResultTableName = item.LabTestResultTableName,
                        LabTestResultCPTCode = item.LabTestResultCPTCode,
                        LabTestResultTestName = item.LabTestResultTestName,
                        LabTestResultSpecimen = item.LabTestResultSpecimen,
                        LabTestResultGender = item.LabTestResultGender,
                        LabTestResultAgeFrom = item.LabTestResultAgeFrom,
                        LabTestResultAgeTo = item.LabTestResultAgeTo,
                        LabTestResultMeasurementValue = item.LabTestResultMeasurementValue,
                        LabTestResultLowRangeResult = item.LabTestResultLowRangeResult,
                        LabTestResultHighRangeResult = item.LabTestResultHighRangeResult,
                        LabTestResultGoodFrom = item.LabTestResultGoodFrom,
                        LabTestResultGoodTo = item.LabTestResultGoodTo,
                        LabTestResultCautionFrom = item.LabTestResultCautionFrom,
                        LabTestResultCautionTo = item.LabTestResultCautionTo,
                        LabTestResultBadFrom = item.LabTestResultBadFrom,
                        LabTestResultBadTo = item.LabTestResultBadTo,
                        ModifiedBy = item.ModifiedBy,
                        Modifieddate = item.Modifieddate,
                        IsDeleted = item.IsDeleted,
                        DeletedBy = item.DeletedBy,
                        DeletedDate = item.DeletedDate,
                        CreatedBy = item.CreatedBy,
                        CreatedDate = item.CreatedDate,
                        AgeFromString = GetNameByGlobalCodeValue(item.LabTestResultAgeFrom.ToString(), Convert.ToInt32(GlobalCodeCategoryValue.LabTestResultAgeFrom).ToString()),
                        AgeToString = GetNameByGlobalCodeValue(item.LabTestResultAgeTo.ToString(), Convert.ToInt32(GlobalCodeCategoryValue.LabTestResultAgeFrom).ToString()),
                        GenderString = GetNameByGlobalCodeValue(item.LabTestResultGender.ToString(), Convert.ToInt32(GlobalCodeCategoryValue.LabTestResultGender).ToString()),
                        MeasurementValueString = GetNameByGlobalCodeValue(item.LabTestResultMeasurementValue.ToString(), Convert.ToInt32(GlobalCodeCategoryValue.LabMeasurementValue).ToString()),
                        SpecifimenString = GetNameByGlobalCodeValue(item.LabTestResultSpecimen.ToString(), Convert.ToInt32(GlobalCodeCategoryValue.LabTestResultSpecimen).ToString()),
                    }));
                }
            }
            return list;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public int SaveLabTestResult(LabTestResult model)
        {
            using (var rep = UnitOfWork.LabTestResultRepository)
            {
                if (model.LabTestResultID > 0)
                    rep.UpdateEntity(model, model.LabTestResultID);
                else
                    rep.Create(model);
                return model.LabTestResultID;
            }
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="LabTestResultId">The lab test result identifier.</param>
        /// <returns></returns>
        public LabTestResult GetLabTestResultByID(int? LabTestResultId)
        {
            using (var rep = UnitOfWork.LabTestResultRepository)
            {
                var model = rep.Where(x => x.LabTestResultID == LabTestResultId).FirstOrDefault();
                return model;
            }
        }

        /// <summary>
        /// Deletes the lab test result.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public int DeleteLabTestResult(LabTestResult model)
        {
            using (var rep = UnitOfWork.LabTestResultRepository)
            {
                return Convert.ToInt32(rep.Delete(model));
            }
        }
    }
}
