using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IGlobalCodeCategoryService
    {
        int AddUpdateGlobalCodeCategory(GlobalCodeCategory model);
        bool CheckDuplicateCode(GlobalCodeCategory model);
        GlobalCodeCategory GetCurrentCategoryById(int GlobalCodeCategoryId);
        List<GlobalCodeCategory> GetFacilityGlobalCodeCategories(string facilityNumber);
        List<GlobalCodeCategory> GetGlobalCodeCategories();
        List<GlobalCodeCategory> GetGlobalCodeCategoriesByExternalValue(string facilityNumber = "");
        List<GlobalCodeCategory> GetGlobalCodeCategoriesByNullFacility();
        List<GlobalCodeCategory> GetGlobalCodeCategoriesRange(int startRange, int finishRange);
        List<GlobalCodeCategoryCustomModel> GetGlobalCodeCategoriesWithFacilityName();
        GlobalCodeCategory GetGlobalCodeCategoryByValue(string globalCodeCategoryvalue);
        List<GlobalCodeCategory> GetListByCategoryValue(string categoryValue);
        List<GlobalCodeCategory> GetSearchedCategories(string text, string externalValue);
    }
}