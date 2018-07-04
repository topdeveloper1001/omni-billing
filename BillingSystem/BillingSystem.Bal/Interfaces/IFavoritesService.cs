using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IFavoritesService
    {
        int AddToFavorites(UserDefinedDescriptions model, bool isFavorite);
        bool CheckIfAlreadyFav(int userid, string codeId, string categoryId);
        bool DeleteFav(int id);
        UserDefinedDescriptions GetFavoriteByCodeId(string codeId);
        UserDefinedDescriptions GetFavoriteByCodeIdPhyId(string codeId, int phyId);
        UserDefinedDescriptions GetFavoriteById(int id);
        List<UserDefinedDescriptions> GetFavoriteByPhyId(int phyid);
        IEnumerable<FavoritesCustomModel> GetFavoriteDiagnosisData(long userId, string DiagnosisTableNumber, string DrgTableNumber);
        List<FavoritesCustomModel> GetFavoriteOrders(int userId, string CptTableNumber, string DrgTableNumber, string HcpcsTableNumber, string DrugTableNumber, string ServiceCodeTableNumber, string DiagnosisTableNumber);
    }
}