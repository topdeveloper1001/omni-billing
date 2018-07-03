using System;
using BillingSystem.Common.Common;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;
using System.Collections.Generic;
using System.Linq;
using System.Data;

using System.Data.SqlClient;

using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class CPTCodesService : ICPTCodesService
    {
        private readonly IRepository<CPTCodes> _repository;
        private readonly IRepository<Users> _uRepository;
        private readonly IRepository<GlobalCodes> _gRepository;
        private readonly IRepository<GlobalCodeCategory> _gcRepository;

        private readonly BillingEntities _context;

        public CPTCodesService(IRepository<CPTCodes> repository, IRepository<Users> uRepository, IRepository<GlobalCodes> gRepository, IRepository<GlobalCodeCategory> gcRepository, BillingEntities context)
        {
            _repository = repository;
            _uRepository = uRepository;
            _gRepository = gRepository;
            _gcRepository = gcRepository;
            _context = context;
        }



        /// <summary>
        /// Get the Service Code
        /// </summary>
        /// <returns>Return the ServiceCode View Model</returns>
        public List<CPTCodes> GetCptCodesListOnDemand(int blockNumber, int blockSize, string CptTableNumber)
        {
            try
            {
                int startIndex = (blockNumber - 1) * blockSize;

                var cptCodes = _repository.Where(c => c.CodeTableNumber.Trim().Equals(CptTableNumber) && c.IsActive != false && (c.IsDeleted == null || !(bool)c.IsDeleted)).OrderByDescending(f => f.CPTCodesId).Skip(startIndex).Take(blockSize).ToList();
                return cptCodes;

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
        public List<CPTCodes> GetCPTCodes(string CptTableNumber)
        {
            var cptCodes = _repository.Where(c => c.CodeTableNumber.Trim().Equals(CptTableNumber) && c.IsActive != false).ToList();
            return cptCodes;

        }

        /// <summary>
        /// Gets all CPT codes.
        /// </summary> Update By Krishna on 07082015
        /// <returns></returns>
        public IEnumerable<CPTCodes> GetAllCptCodes(string CptTableNumber)
        {
            var cptCodes = _repository.Where(_ => _.IsDeleted == false && _.CodeTableNumber.Trim().Equals(CptTableNumber) && _.IsActive != false).ToList();
            return cptCodes;

        }

        /// <summary>
        /// Method to add update the CPT Code in the database.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddUpdateCPTCodes(CPTCodes model, string CptTableNumber)
        {
            model.CodeTableNumber = CptTableNumber;
            if (model.CPTCodesId > 0)
            {
                var current = _repository.GetSingle(model.CPTCodesId);
                if (current != null)
                {
                    model.CodeTableNumber = current.CodeTableNumber;
                    model.CodeTableDescription = current.CodeTableDescription;
                    model.CreatedBy = current.CreatedBy;
                    model.CreatedDate = current.CreatedDate;
                    _repository.UpdateEntity(model, model.CPTCodesId);
                }
            }
            else
                _repository.Create(model);
            return model.CPTCodesId;

        }

        /// <summary>
        /// Method to add the Service Code in the database.
        /// </summary>
        /// <param name="shared"></param>
        /// <returns></returns>
        public CPTCodes GetCPTCodesById(int id)
        {
            var cptCodes = _repository.Where(x => x.CPTCodesId == id).FirstOrDefault();
            return cptCodes;

        }

        /// <summary>
        /// Get the Service Code
        /// </summary>
        /// <returns>Return the ServiceCode View Model</returns>
        public List<CPTCodes> GetFilteredCodes(string text, string CptTableNumber)
        {
            text = text.ToLower();
            var lstCptCodes = _repository.Where(c => (c.IsDeleted == null || !(bool)c.IsDeleted) && (c.CodeNumbering.ToLower().Contains(text) || c.CodeDescription.ToLower().Contains(text)) && c.CodeTableNumber.Trim().Equals(CptTableNumber) && c.IsActive != false).Take(100).ToList();
            return lstCptCodes;

        }

        public List<CPTCodes> GetFilteredCodeExportToExcel(string text, string tableNumber)
        {
            text = text.ToLower();

            var lstCptCodes = _repository.Where(c => (c.IsDeleted == null || !(bool)c.IsDeleted) && (string.IsNullOrEmpty(text) || c.CodeNumbering.ToLower().Contains(text) ||
                c.CodeDescription.ToLower().Contains(text)) && c.CodeTableNumber.Trim().Equals(tableNumber) && c.IsActive != false).ToList();
            return lstCptCodes;

        }

        public List<CPTCodes> GetFilteredCptCodes(string text, string tableNumber)
        {
            text = text.ToLower();

            var lstCptCodes = _repository.Where(c => c.IsDeleted != true &&
            (c.CodeNumbering.ToLower().Contains(text) || c.CodeDescription.ToLower().Contains(text)) &&
            c.CodeTableNumber.Trim().Equals(tableNumber) && c.IsActive != false).Take(100).ToList();
            return lstCptCodes;

        }

        /// <summary>
        /// Gets the order code descby code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        public string GetOrderCodeDescbyCode(string code, string CptTableNumber)
        {
            code = code.ToLower();

            var cptCodesObj = _repository.Where(x => x.CodeTableNumber.Trim().Equals(CptTableNumber) && (x.IsDeleted == null || !(bool)x.IsDeleted) && (x.CodeNumbering.ToLower().Equals(code))).FirstOrDefault();
            return cptCodesObj != null ? cptCodesObj.CodeDescription : "";

        }

        /// <summary>
        /// Gets the codes by range.
        /// </summary>
        /// <param name="startRange">The start range.</param>
        /// <param name="endRange">The end range.</param>
        /// <returns></returns>
        public List<CPTCodes> GetCodesByRange(int startRange, int endRange, string CptTableNumber)
        {
            var list = _repository.Where(x => !x.CodeNumbering.Contains("T") && !x.CodeNumbering.Contains("F") && x.CodeTableNumber.Trim().Equals(CptTableNumber))
                    .ToList()
                    .Where(c =>
                            Convert.ToInt32(c.CodeNumbering) >= startRange &&
                            Convert.ToInt32(c.CodeNumbering) <= endRange)
                    .OrderBy(v => v.CodeNumbering)
                    .ToList();
            return list;

        }

        /// <summary>
        /// Gets the CPT code description.
        /// </summary>
        /// <param name="codeid">The codeid.</param>
        /// <returns></returns>
        public string GetCPTCodeDescription(string codeid, string CptTableNumber)
        {
            var cptCodes = _repository.Where(x => x.CodeNumbering.Contains(codeid) && x.CodeTableNumber.Trim().Equals(CptTableNumber)).FirstOrDefault();
            return cptCodes != null ? cptCodes.CodeDescription : string.Empty;

        }

        /// <summary>
        /// Method to add the Service Code in the database.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public CPTCodes GetCPTCodesByCode(string code, string CptTableNumber)
        {
            var cptCodes = _repository.Where(x => x.CodeNumbering.Equals(code) && x.CodeTableNumber.Trim().Equals(CptTableNumber))
                    .FirstOrDefault();
            return cptCodes;

        }

        /// <summary>
        /// Gets the CPT custom codes by range.
        /// </summary>
        /// <param name="startRange">The start range.</param>
        /// <param name="endRange">The end range.</param>
        /// <returns></returns>
        public List<CPTCodesCustomModel> GetCPTCustomCodesByRange(int startRange, int endRange, string CptTableNumber)
        {
            var cptcodeCustomobj = new List<CPTCodesCustomModel>();

            var list = _repository.Where(x => !x.CodeNumbering.Contains("T") && !x.CodeNumbering.Contains("F") && x.CodeTableNumber.Trim().Equals(CptTableNumber)).ToList().Where(c => Convert.ToInt32(c.CodeNumbering) >= startRange && Convert.ToInt32(c.CodeNumbering) <= endRange).OrderBy(v => v.CodeNumbering).ToList();
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

                var customModel = GetGeneralGlobalCodeByRangeVal(Convert.ToInt64(item.CTPCodeRangeValue),
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
        private GeneralCodesCustomModel GetGeneralGlobalCodeByRangeVal(long coderange, string type, string categoryId)
        {
            var generalCodesCustomModel = new GeneralCodesCustomModel();
            var gModellist =
                    _gRepository.Where(
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
        private GlobalCodeCategory GetGlobalCodeCategoryByValue(string globalCodeCategoryvalue)
        {
            var m = _gcRepository.Where(f => f.GlobalCodeCategoryValue == globalCodeCategoryvalue).FirstOrDefault();
            return m ?? new GlobalCodeCategory();
        }
        private string GetNameByUserId(int? UserID)
        {
            var usersModel = _uRepository.Where(x => x.UserID == UserID && x.IsDeleted == false).FirstOrDefault();
            return usersModel != null ? usersModel.FirstName + " " + usersModel.LastName : string.Empty;
        }
        /// <summary>
        /// Gets the CPT codes custom by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public CPTCodesCustomModel GetCPTCodesCustomById(int id, string CptTableNumber)
        {
                var cptCodes = _repository.Where(x => x.CPTCodesId == id && x.CodeTableNumber.Trim().Equals(CptTableNumber)).FirstOrDefault();
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

                var customModel = GetGeneralGlobalCodeByRangeVal(Convert.ToInt64(cptCodes.CTPCodeRangeValue),
                        Convert.ToInt32(OrderType.CPT).ToString(),
                        Convert.ToInt32(GlobalCodeCategoryValue.LabTest).ToString());
                cptCodecustom.CategoryId = customModel.GlobalCodeId.ToString();
                cptCodecustom.CategoryName = customModel.GlobalCodeName;
                cptCodecustom.GlobalCodeCategoryId = Convert.ToInt32(customModel.GlobalCodeCategoryId);
                cptCodecustom.CategoryName = customModel.GlobalCodeCategoryName;
                cptCodecustom.CreatedByName = GetNameByUserId(cptCodes.CreatedBy);
                return cptCodecustom;
        }

        /// <summary>
        /// Checks if CPT code exists in range.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="startRange">The start range.</param>
        /// <param name="endRange">The end range.</param>
        /// <returns></returns>
        public bool CheckIfCptCodeExistsInRange(string value, int startRange, int endRange, string CptTableNumber)
        {
            var isExists = false;

            if (!string.IsNullOrEmpty(value))
            {
                isExists = _repository.Where(x => !x.CodeNumbering.Contains("T") && !x.CodeNumbering.Contains("F") && x.CodeTableNumber.Trim().Equals(CptTableNumber))
                                .ToList().Any(c => Convert.ToInt32(c.CodeNumbering) >= startRange &&
                                                   Convert.ToInt32(c.CodeNumbering) <= endRange && c.CodeNumbering.Contains(value));
            }

            return isExists;
        }

        public List<CPTCodes> GetCptCodesListByMueValue(string mueValue, string CptTableNumber)
        {
            mueValue = mueValue.ToLower().Trim();
            var list = _repository.Where(c => c.CodeCPTMUEValues.ToLower().Trim().Equals(mueValue) && c.IsDeleted == false && c.CodeTableNumber.Trim().Equals(CptTableNumber)).OrderBy(m => m.CodeNumbering).ToList();
            return list;

        }

        public List<CPTCodes> GetCPTCodesData(bool showInActive, string CptTableNumber)
        {
            var cptCodes = _repository.Where(c => c.CodeTableNumber.Trim().Equals(CptTableNumber) && c.IsActive == showInActive && (c.IsDeleted == null || !(bool)c.IsDeleted)).ToList();
            return cptCodes;

        }

        public List<ExportCodesData> GetCodesDataToExport(long cId, long fId, long userId, string tableNumber, string codeType, string searchText, out string columns, string tableName = "")
        {
            var sqlParams = new SqlParameter[7];
            sqlParams[0] = new SqlParameter("@pSearchText", !string.IsNullOrEmpty(searchText) ? searchText : string.Empty);
            sqlParams[1] = new SqlParameter("@pCodeType", codeType);
            sqlParams[2] = new SqlParameter("@pCId", cId);
            sqlParams[3] = new SqlParameter("@pFId", fId);
            sqlParams[4] = new SqlParameter("@pTableNumber", !string.IsNullOrEmpty(tableNumber) ? tableNumber : string.Empty);
            sqlParams[5] = new SqlParameter("@pTableName", !string.IsNullOrEmpty(tableName) ? tableName : string.Empty);
            sqlParams[6] = new SqlParameter("@pUserId", userId);

            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetOrderCodesToExport.ToString(), false, parameters: sqlParams))
            {
                var result = r.ResultSetFor<ExportCodesData>().ToList();
                columns = r.ResultSetFor<string>().FirstOrDefault();
                return result;
            }
        }


        public bool ImportAndSaveCodesToDatabase(string codeType, long cId, long fId, string tno, string tname, long loggedinUserId, DataTable dt)
        {
            var status = false;

            var sqlParams = new SqlParameter[7];

            sqlParams[0] = new SqlParameter("@pCodeType", codeType);
            sqlParams[1] = new SqlParameter("@pCId", cId);
            sqlParams[2] = new SqlParameter("@pFId", fId);
            sqlParams[3] = new SqlParameter("@pTableNumber", tno);
            sqlParams[4] = new SqlParameter("@pTableName", tname);
            sqlParams[5] = new SqlParameter("@pUserId", loggedinUserId);

            sqlParams[6] = new SqlParameter
            {
                ParameterName = "@TCodes",
                SqlDbType = SqlDbType.Structured,
                Value = dt,
                TypeName = "SqlBillingCodeType"
            };

            try
            {
                _repository.ExecuteCommand(StoredProcedures.SprocImportBillingCodes.ToString(), sqlParams);
                status = true;
            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }
    }
}
