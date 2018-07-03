using System.Collections.Generic;
using System.Threading.Tasks;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IGlobalCodeService
    {
        List<GlobalCodes> GetGlobalCodesByCategoriesSp(string gccValues);
        List<GlobalCodes> GetSubCategories2(string gcValue1);
        GlobalCodes GetGlobalCodeByCategoryAndCodeValue(string gcCategoryValue, string gcvalue);
        string GetKeyColmnNameByTableName(string tableName);
        List<GlobalCodes> GetTableStruturebyTableId(string id);
        GeneralCodesCustomModel GetSelectedCodeParent1(string orderCode, string codeType, long facilityId, string tableNumber);

        string GetNameByGlobalCodeValue(string codeValue, string categoryValue, string fId = "");
        Task<string> GetGlobalCodeNameAsync(string codeValue, string categoryValue);
        int AddUpdateGlobalCodes(GlobalCodes globalCodes);
        bool AddUpdateGlobalCodesList(List<GlobalCodes> globalCodes);
        bool CheckDuplicateGlobalCodeName(string name, int id, string categoryValue, string facilityNumber);
        bool CheckDuplicateVital(int id, string categoryValue, string value, string unitOfMeasure);
        bool CreateOrderActivitySchedulerTimming(int id);
        List<GlobalCodeCustomDModel> DeleteGlobalCode(int globalCodeId, string facilityId);
        List<GlobalCodeCustomModel> DeleteGlobalCodeById(int globalCodeId, string category, bool withList = true);
        List<GlobalCodes> DeleteRecordAndGetGlobalCodesList(int globalCodeId, string category);
        List<GlobalCodeCustomModel> GetActiveInActiveRecord(string categoryValue, bool showInActive);
        List<GlobalCodeCustomModel> GetAllGlobalCodes(string categoryValue);
        int GetCorporateIdFromFacilityId(int facilityid);
        List<int> GetDefaultMonthAndYearByFacilityId(int facilityId, int corporateId);
        List<GlobalCodes> GetEncounterTypesByPatientType(string id, string patientTypeId);
        string GetExternalVal1Val2ByIdAndCategoryId(string categoryId, int globalCodeId);
        List<GlobalCodes> GetGCodesListByCategoryValue(string categoryValue);
        List<DropdownListData> GetGCodesListByCategoryValue(string categoryValue, IEnumerable<string> extValues1, string withoutValue);
        List<GlobalCodes> GetGCodesListByCategoryValue(string categoryValue, string externalValue1, string externalValue3);
        string GetGlobalCategoryNameById(string categoryValue, string facilityId = "");
        GlobalCodes GetGlobalCodeByFacilityAndCategory(string category, string facilityNumber);
        GlobalCodes GetGlobalCodeByFacilityAndCategoryForSecurityparameter(string category, string facilityNumber);
        GlobalCodes GetGlobalCodeByGlobalCodeId(int globalCodeId);
        GlobalCodeCustomModel GetGlobalCodeCustomById(int globalCodeId);
        string GetGlobalCodeNameByIdAndCategoryId(string categoryId, int globalCodeId);
        string GetGlobalCodeNameByValueAndCategoryId(string categoryId, string globalCodeVal);
        List<GlobalCodeCustomModel> GetGlobalCodesByCategoriesRange(int startRange, int endRange);
        List<GlobalCodeCustomModel> GetGlobalCodesByCategoriesRangeOnDemand(int startRange, int endRange, int blockNumber, int blockSize, bool skip, bool showInActive, long facilityId = 0, long corporateId = 0);
        List<GlobalCodeCustomModel> GetGlobalCodesByCategoriesRangeOnDemand(string gcc, int blockNumber, int blockSize, bool skip, bool showInActive, long facilityId = 0, long corporateId = 0);
        List<GlobalCodeCustomModel> GetGlobalCodesByCategory(string categoryValue, long corporateId, long facilityId, long userId, long id, out long newId, bool listStatus, bool isFacilityPassed = false);
        List<GlobalCodes> GetGlobalCodesByCategoryValue(string categoryValue, string fn = "");
        List<string> GetGlobalCodesLabelsListByCategoryValue(string categoryValue);
        int GetGlobalCodeSotringByValueAndCategoryId(string categoryId, string globalCodeVal);
        GlobalCodes GetIndicatorSettingsByCorporateId(string corporateId);
        List<DropdownListData> GetListByCategoriesRange(IEnumerable<string> categories, string facilityId = "0");
        List<DropdownListData> GetListByCategoriesRange(IEnumerable<string> categories, IEnumerable<string> extValue1);
        GlobalCodes GetMaxGlobalCodeByCategoryValue(string categoryValue);
        int GetMaxGlobalCodeValueByCategory(string categoryValue);
        int GetMaxGlobalCodeValueByCategory(string categoryValue, string facilityNumber);
        string GetNameByGlobalCodeValueAndCategoryValue(string categoryValue, string globalCodeValue);
        OrderCodes GetOrderCodesByRange(string tableNo, string categoryValue, string subCategoryValue, string orderCode, long startRange, long endRange, long fId);
        List<GlobalCodes> GetPreviousRecords(string facilitynumber, string roomId);
        string GetRangeByCategoryType(string p);
        List<GlobalCodeCustomDModel> GetRoomEquipmentALLList(string facilityId);
        List<GlobalCodes> GetRoomEquipmentList(string facilityId, string roomid);
        List<GlobalCodeCustomModel> GetSubCategoriesList(string categoryValue);
        List<GlobalCodeCustomModel> GetSubCategoriesListBySubCategory1Value(string categoryValue, string selectedValue);
        List<GlobalCodeCustomModel> ShowDeletedRecordsByCategoryValue(string categoryValue, bool showDeleted);
        List<GlobalCodeCustomModel> ShowInActiveRecordsByCategoryValue(string categoryValue, bool showInActive);
    }
}