using System;
using BillingSystem.Common.Common;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace BillingSystem.Bal.BusinessAccess
{
    public class CPTCodesBal : BaseBal
    {
        public CPTCodesBal(string cptTableNumber)
        {
            if (!string.IsNullOrEmpty(cptTableNumber))
                CptTableNumber = cptTableNumber;
        }

        /// <summary>
        /// Get the Service Code
        /// </summary>
        /// <returns>Return the ServiceCode View Model</returns>
        public List<CPTCodes> GetCptCodesListOnDemand(int blockNumber, int blockSize)
        {
            try
            {
                int startIndex = (blockNumber - 1) * blockSize;

                using (var rep = UnitOfWork.CPTCodesRepository)
                {
                    var cptCodes =
                        rep.Where(c => c.CodeTableNumber.Trim().Equals(CptTableNumber) && c.IsActive != false && (c.IsDeleted == null || !(bool)c.IsDeleted))
                            .OrderByDescending(f => f.CPTCodesId)
                            .Skip(startIndex)
                            .Take(blockSize)
                            .ToList();
                    return cptCodes;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get the Service Code
        /// </summary>
        /// <returns>Return the ServiceCode View Model</returns>
        public List<CPTCodes> GetCPTCodes()
        {
            try
            {
                using (var rep = UnitOfWork.CPTCodesRepository)
                {
                    var cptCodes = rep.Where(c => c.CodeTableNumber.Trim().Equals(CptTableNumber) && c.IsActive != false).ToList();
                    return cptCodes;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets all CPT codes.
        /// </summary> Update By Krishna on 07082015
        /// <returns></returns>
        public IEnumerable<CPTCodes> GetAllCptCodes()
        {
            try
            {
                using (var rep = UnitOfWork.CPTCodesRepository)
                {
                    var cptCodes = rep.Where(_ => _.IsDeleted == false && _.CodeTableNumber.Trim().Equals(CptTableNumber) && _.IsActive != false).ToList();
                    return cptCodes;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Method to add update the CPT Code in the database.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddUpdateCPTCodes(CPTCodes model)
        {
            using (var rep = UnitOfWork.CPTCodesRepository)
            {
                model.CodeTableNumber = CptTableNumber;
                if (model.CPTCodesId > 0)
                {
                    var current = rep.GetSingle(model.CPTCodesId);
                    if (current != null)
                    {
                        model.CodeTableNumber = current.CodeTableNumber;
                        model.CodeTableDescription = current.CodeTableDescription;
                        model.CreatedBy = current.CreatedBy;
                        model.CreatedDate = current.CreatedDate;
                        rep.UpdateEntity(model, model.CPTCodesId);
                    }
                }
                else
                    rep.Create(model);
                return model.CPTCodesId;
            }
        }

        /// <summary>
        /// Method to add the Service Code in the database.
        /// </summary>
        /// <param name="shared"></param>
        /// <returns></returns>
        public CPTCodes GetCPTCodesById(int id)
        {
            using (var rep = UnitOfWork.CPTCodesRepository)
            {
                var cptCodes = rep.Where(x => x.CPTCodesId == id).FirstOrDefault();
                return cptCodes;
            }
        }

        /// <summary>
        /// Get the Service Code
        /// </summary>
        /// <returns>Return the ServiceCode View Model</returns>
        public List<CPTCodes> GetFilteredCodes(string text)
        {
            try
            {
                text = text.ToLower();
                using (var cptCodesRep = UnitOfWork.CPTCodesRepository)
                {
                    var lstCptCodes = cptCodesRep.Where(c => (c.IsDeleted == null || !(bool)c.IsDeleted) && (c.CodeNumbering.ToLower().Contains(text) || c.CodeDescription.ToLower().Contains(text)) && c.CodeTableNumber.Trim().Equals(CptTableNumber) && c.IsActive != false).Take(100).ToList();
                    return lstCptCodes;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CPTCodes> GetFilteredCodeExportToExcel(string text, string tableNumber)
        {
            try
            {
                text = text.ToLower();
                using (var cptCodesRep = UnitOfWork.CPTCodesRepository)
                {
                    var lstCptCodes = cptCodesRep.Where(c => (c.IsDeleted == null || !(bool)c.IsDeleted) && (string.IsNullOrEmpty(text) || c.CodeNumbering.ToLower().Contains(text) ||
                        c.CodeDescription.ToLower().Contains(text)) && c.CodeTableNumber.Trim().Equals(tableNumber) && c.IsActive != false).ToList();
                    return lstCptCodes;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CPTCodes> GetFilteredCptCodes(string text, string tableNumber)
        {
            try
            {
                text = text.ToLower();
                using (var cptCodesRep = UnitOfWork.CPTCodesRepository)
                {
                    var lstCptCodes = cptCodesRep.Where(c => c.IsDeleted != true && 
                    (c.CodeNumbering.ToLower().Contains(text) || c.CodeDescription.ToLower().Contains(text)) && 
                    c.CodeTableNumber.Trim().Equals(tableNumber) && c.IsActive != false).Take(100).ToList();
                    return lstCptCodes;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the order code descby code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        public string GetOrderCodeDescbyCode(string code)
        {
            try
            {
                code = code.ToLower();
                using (var cptCodesRep = UnitOfWork.CPTCodesRepository)
                {
                    var cptCodesObj =
                        cptCodesRep.Where(
                            x =>
                                x.CodeTableNumber.Trim().Equals(CptTableNumber) &&
                                (x.IsDeleted == null || !(bool)x.IsDeleted) && (x.CodeNumbering.ToLower().Equals(code)))
                            .FirstOrDefault();
                    return cptCodesObj != null ? cptCodesObj.CodeDescription : "";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the codes by range.
        /// </summary>
        /// <param name="startRange">The start range.</param>
        /// <param name="endRange">The end range.</param>
        /// <returns></returns>
        public List<CPTCodes> GetCodesByRange(int startRange, int endRange)
        {
            using (var rep = UnitOfWork.CPTCodesRepository)
            {
                var list =
                    rep.Where(x => !x.CodeNumbering.Contains("T") && !x.CodeNumbering.Contains("F") && x.CodeTableNumber.Trim().Equals(CptTableNumber))
                        .ToList()
                        .Where(c =>
                                Convert.ToInt32(c.CodeNumbering) >= startRange &&
                                Convert.ToInt32(c.CodeNumbering) <= endRange)
                        .OrderBy(v => v.CodeNumbering)
                        .ToList();
                return list;
            }
        }

        /// <summary>
        /// Gets the CPT code description.
        /// </summary>
        /// <param name="codeid">The codeid.</param>
        /// <returns></returns>
        public string GetCPTCodeDescription(string codeid)
        {
            using (var rep = UnitOfWork.CPTCodesRepository)
            {
                var cptCodes = rep.Where(x => x.CodeNumbering.Contains(codeid) && x.CodeTableNumber.Trim().Equals(CptTableNumber)).FirstOrDefault();
                return cptCodes != null ? cptCodes.CodeDescription : string.Empty;
            }
        }

        /// <summary>
        /// Method to add the Service Code in the database.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public CPTCodes GetCPTCodesByCode(string code)
        {
            using (var cptCodesRep = UnitOfWork.CPTCodesRepository)
            {
                var cptCodes =
                    cptCodesRep.Where(x => x.CodeNumbering.Equals(code) && x.CodeTableNumber.Trim().Equals(CptTableNumber))
                        .FirstOrDefault();
                return cptCodes;
            }
        }

        /// <summary>
        /// Gets the CPT custom codes by range.
        /// </summary>
        /// <param name="startRange">The start range.</param>
        /// <param name="endRange">The end range.</param>
        /// <returns></returns>
        public List<CPTCodesCustomModel> GetCPTCustomCodesByRange(int startRange, int endRange)
        {
            var cptcodeCustomobj = new List<CPTCodesCustomModel>();
            using (var rep = UnitOfWork.CPTCodesRepository)
            {
                using (var globalcodebal = new GlobalCodeBal())
                {
                    var list =
                        rep.Where(x => !x.CodeNumbering.Contains("T") && !x.CodeNumbering.Contains("F") && x.CodeTableNumber.Trim().Equals(CptTableNumber))
                            .ToList()
                            .Where(
                                c =>
                                    Convert.ToInt32(c.CodeNumbering) >= startRange &&
                                    Convert.ToInt32(c.CodeNumbering) <= endRange)
                            .OrderBy(v => v.CodeNumbering)
                            .ToList();
                    foreach (var item in list)
                    {
                        var cptCodecustom = new CPTCodesCustomModel
                        {
                            CPTCodesId = item.CPTCodesId,
                            CodeTableNumber = item.CodeTableNumber,
                            CodeTableDescription = item.CodeTableDescription,
                            CodeNumbering = item.CodeNumbering,
                            CodeDescription = item.CodeDescription,
                            CodePrice = item.CodePrice,
                            CodeAnesthesiaBaseUnit = item.CodeAnesthesiaBaseUnit,
                            CodeEffectiveDate = item.CodeEffectiveDate,
                            CodeExpiryDate = item.CodeExpiryDate,
                            CodeBasicProductApplicationRule = item.CodeBasicProductApplicationRule,
                            CodeOtherProductsApplicationRule = item.CodeOtherProductsApplicationRule,
                            CodeServiceMainCategory = item.CodeServiceMainCategory,
                            CodeServiceCodeSubCategory = item.CodeServiceCodeSubCategory,
                            CodeUSCLSChapter = item.CodeUSCLSChapter,
                            CodeCPTMUEValues = item.CodeCPTMUEValues,
                            CodeGroup = item.CodeGroup,
                            CreatedBy = item.CreatedBy,
                            CreatedDate = item.CreatedDate,
                            ModifiedBy = item.ModifiedBy,
                            ModifiedDate = item.ModifiedDate,
                            IsDeleted = item.IsDeleted,
                            DeletedBy = item.DeletedBy,
                            IsActive = item.IsActive,
                            DeletedDate = item.DeletedDate,
                            CTPCodeRangeValue = item.CTPCodeRangeValue,
                            ExternalValue1 = item.ExternalValue1,
                            ExternalValue2 = item.ExternalValue2,
                            ExternalValue3 = item.ExternalValue3,
                        };

                        var customModel =
                            globalcodebal.GetGeneralGlobalCodeByRangeVal(Convert.ToInt64(item.CTPCodeRangeValue),
                                Convert.ToInt32(OrderType.CPT).ToString(), Convert.ToInt32(GlobalCodeCategoryValue.LabTest).ToString());
                        cptCodecustom.CategoryId = customModel.GlobalCodeId.ToString();
                        cptCodecustom.CategoryName = customModel.GlobalCodeName;
                        cptCodecustom.GlobalCodeCategoryId = Convert.ToInt32(customModel.GlobalCodeCategoryId);
                        cptCodecustom.CategoryName = customModel.GlobalCodeCategoryName;
                        cptCodecustom.CreatedByName = GetNameByUserId(item.CreatedBy);
                        cptcodeCustomobj.Add(cptCodecustom);
                    }
                    return cptcodeCustomobj;
                }
            }
        }

        /// <summary>
        /// Gets the CPT codes custom by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public CPTCodesCustomModel GetCPTCodesCustomById(int id)
        {
            using (var CPTCodesRep = UnitOfWork.CPTCodesRepository)
            {
                using (var globalcodebal = new GlobalCodeBal())
                {
                    var cptCodes = CPTCodesRep.Where(x => x.CPTCodesId == id && x.CodeTableNumber.Trim().Equals(CptTableNumber)).FirstOrDefault();
                    var cptCodecustom = new CPTCodesCustomModel
                    {
                        CPTCodesId = cptCodes.CPTCodesId,
                        CodeTableNumber = cptCodes.CodeTableNumber,
                        CodeTableDescription = cptCodes.CodeTableDescription,
                        CodeNumbering = cptCodes.CodeNumbering,
                        CodeDescription = cptCodes.CodeDescription,
                        CodePrice = cptCodes.CodePrice,
                        CodeAnesthesiaBaseUnit = cptCodes.CodeAnesthesiaBaseUnit,
                        CodeEffectiveDate = cptCodes.CodeEffectiveDate,
                        CodeExpiryDate = cptCodes.CodeExpiryDate,
                        CodeBasicProductApplicationRule = cptCodes.CodeBasicProductApplicationRule,
                        CodeOtherProductsApplicationRule = cptCodes.CodeOtherProductsApplicationRule,
                        CodeServiceMainCategory = cptCodes.CodeServiceMainCategory,
                        CodeServiceCodeSubCategory = cptCodes.CodeServiceCodeSubCategory,
                        CodeUSCLSChapter = cptCodes.CodeUSCLSChapter,
                        CodeCPTMUEValues = cptCodes.CodeCPTMUEValues,
                        CodeGroup = cptCodes.CodeGroup,
                        CreatedBy = cptCodes.CreatedBy,
                        CreatedDate = cptCodes.CreatedDate,
                        ModifiedBy = cptCodes.ModifiedBy,
                        ModifiedDate = cptCodes.ModifiedDate,
                        IsDeleted = cptCodes.IsDeleted,
                        DeletedBy = cptCodes.DeletedBy,
                        IsActive = cptCodes.IsActive,
                        DeletedDate = cptCodes.DeletedDate,
                        CTPCodeRangeValue = cptCodes.CTPCodeRangeValue,
                        ExternalValue1 = cptCodes.ExternalValue1,
                        ExternalValue2 = cptCodes.ExternalValue2,
                        ExternalValue3 = cptCodes.ExternalValue3,
                    };

                    var customModel =
                        globalcodebal.GetGeneralGlobalCodeByRangeVal(Convert.ToInt64(cptCodes.CTPCodeRangeValue),
                            Convert.ToInt32(OrderType.CPT).ToString(),
                            Convert.ToInt32(GlobalCodeCategoryValue.LabTest).ToString());
                    cptCodecustom.CategoryId = customModel.GlobalCodeId.ToString();
                    cptCodecustom.CategoryName = customModel.GlobalCodeName;
                    cptCodecustom.GlobalCodeCategoryId = Convert.ToInt32(customModel.GlobalCodeCategoryId);
                    cptCodecustom.CategoryName = customModel.GlobalCodeCategoryName;
                    cptCodecustom.CreatedByName = GetNameByUserId(cptCodes.CreatedBy);
                    return cptCodecustom;
                }
            }
        }

        /// <summary>
        /// Checks if CPT code exists in range.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="startRange">The start range.</param>
        /// <param name="endRange">The end range.</param>
        /// <returns></returns>
        public bool CheckIfCptCodeExistsInRange(string value, int startRange, int endRange)
        {
            var isExists = false;
            using (var rep = UnitOfWork.CPTCodesRepository)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    isExists =
                                rep.Where(x => !x.CodeNumbering.Contains("T") && !x.CodeNumbering.Contains("F") && x.CodeTableNumber.Trim().Equals(CptTableNumber))
                                    .ToList().Any(c => Convert.ToInt32(c.CodeNumbering) >= startRange &&
                                                       Convert.ToInt32(c.CodeNumbering) <= endRange && c.CodeNumbering.Contains(value));
                }
            }
            return isExists;
        }

        public List<CPTCodes> GetCptCodesListByMueValue(string mueValue)
        {
            mueValue = mueValue.ToLower().Trim();
            using (var rep = UnitOfWork.CPTCodesRepository)
            {
                var list =
                    rep.Where(
                        c =>
                            c.CodeCPTMUEValues.ToLower().Trim().Equals(mueValue) && c.IsDeleted == false &&
                            c.CodeTableNumber.Trim().Equals(CptTableNumber)).OrderBy(m => m.CodeNumbering).ToList();
                return list;
            }
        }

        public List<CPTCodes> GetCPTCodesData(bool showInActive)
        {
            try
            {
                using (var rep = UnitOfWork.CPTCodesRepository)
                {
                    var cptCodes = rep.Where(c => c.CodeTableNumber.Trim().Equals(CptTableNumber) && c.IsActive == showInActive && (c.IsDeleted == null || !(bool)c.IsDeleted)).ToList();
                    return cptCodes;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ExportCodesData> GetCodesDataToExport(long cId, long fId, long userId, string tableNumber, string codeType, string searchText, out string columns, string tableName = "")
        {
            using (var rep = UnitOfWork.CPTCodesRepository)
            {
                return rep.GetCodesDataToExport(cId, fId, userId, tableNumber, codeType, searchText, out columns, tableName);
            }
        }


        public bool ImportAndSaveCodesToDatabase(string codeType, long cId, long fId, string tno, string tname, long loggedinUserId, DataTable dt)
        {
            using (var rep = UnitOfWork.CPTCodesRepository)
            {
                var result = rep.ImportBillingCodes(codeType, cId, fId, tno, tname, loggedinUserId, dt);
                return result;
            }
        }
    }
}
