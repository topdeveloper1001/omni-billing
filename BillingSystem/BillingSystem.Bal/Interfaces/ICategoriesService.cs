using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface ICategoriesService
    {
        bool CheckDuplicateCategory(long id, string prodCatNum, string prodCat);
        long DeleteCategoriesData(Categories model);
        Categories GetCategoryById(int id);
        long SaveCategories(Categories model);
        List<CategoriesCustomModel> GetCategoriesData();
    }
}