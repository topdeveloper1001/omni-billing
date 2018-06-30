using System.Collections.Generic;
using BillingSystem.Model;

namespace BillingSystem.Bal.Interfaces
{
    public interface IGlobalCodeCategoryMasterService
    {
        int AddUpdateGlobalCategory(GlobalCodeCategory m);
        List<GlobalCodeCategory> GetAllGlobalCodeCategories();
        List<GlobalCodeCategory> GetAllGlobalCodeCategoriesByOrderType(string orderType, string fn = "");
        GlobalCodeCategory GetGlobalCategoriesByGlobalCodeCategoryId(int globalCodeCategoriesID);
        List<GlobalCodeCategory> GetOrderTypeCategoriesByFacility(long facilityId, long userId, bool status = true);
    }
}