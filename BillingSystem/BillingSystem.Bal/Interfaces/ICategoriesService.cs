using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface ICategoriesService
    {
        bool CheckDuplicateCategory(int id, string prodCatNum, string prodCat);
        int DeleteCategoriesData(Categories model);
        Categories GetCategoryById(int id);
        int SaveCategories(Categories model);
        List<CategoriesCustomModel> GetCategoriesData();
    }
}