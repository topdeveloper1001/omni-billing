using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Query.Dynamic;
using BillingSystem.Common.Common;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Bal.BusinessAccess
{
    public class FavoritesBal : BaseBal
    {
        public FavoritesBal()
        {

        }

        public FavoritesBal(string cptTableNumber, string serviceCodeTableNumber, string drgTableNumber, string drugTableNumber, string hcpcsTableNumber, string diagnosisTableNumber)
        {
            if (!string.IsNullOrEmpty(cptTableNumber))
                CptTableNumber = cptTableNumber;

            if (!string.IsNullOrEmpty(serviceCodeTableNumber))
                ServiceCodeTableNumber = serviceCodeTableNumber;

            if (!string.IsNullOrEmpty(drgTableNumber))
                DrgTableNumber = drgTableNumber;

            if (!string.IsNullOrEmpty(drugTableNumber))
                DrugTableNumber = drugTableNumber;

            if (!string.IsNullOrEmpty(hcpcsTableNumber))
                HcpcsTableNumber = hcpcsTableNumber;

            if (!string.IsNullOrEmpty(diagnosisTableNumber))
                DiagnosisTableNumber = diagnosisTableNumber;
        }

        /// <summary>
        /// Gets the favorite by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public UserDefinedDescriptions GetFavoriteById(int id)
        {
            using (var rep = UnitOfWork.FavoritesRepository)
            {
                var fav = rep.Where(f => f.UserDefinedDescriptionID == id).FirstOrDefault();
                return fav;
            }
        }

        /// <summary>
        /// Adds to favorites.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="isFavorite">if set to <c>true</c> [is favorite].</param>
        /// <returns></returns>
        public int AddToFavorites(UserDefinedDescriptions model, bool isFavorite)
        {
            using (var rep = UnitOfWork.FavoritesRepository)
            {
                if (model.UserDefinedDescriptionID > 0)
                {
                    model.IsDeleted = !isFavorite;
                    model.DeletedBy = !isFavorite ? model.ModifiedBy : null;
                    model.DeletedDate = !isFavorite ? model.Modifieddate : null;
                    rep.UpdateEntity(model, model.UserDefinedDescriptionID);
                }
                else
                    rep.Create(model);

                return model.UserDefinedDescriptionID;
            }
        }

        /// <summary>
        /// Gets the favorite by code identifier.
        /// </summary>
        /// <param name="codeId">The code identifier.</param>
        /// <returns></returns>
        public UserDefinedDescriptions GetFavoriteByCodeId(string codeId)
        {
            using (var rep = UnitOfWork.FavoritesRepository)
            {
                var fav = rep.Where(f => f.CodeId.Equals(codeId)).FirstOrDefault();
                if (fav == null)
                {
                    var userDefinedCodeId = 0;
                    var udefinedCodeId = Int32.TryParse(codeId, out userDefinedCodeId);
                    if (userDefinedCodeId != 0)
                    {
                        fav = rep.Where(f => f.UserDefinedDescriptionID == userDefinedCodeId).FirstOrDefault();
                    }
                }
                return fav;
            }
        }

        /// <summary>
        /// Gets the favorite by phy identifier.
        /// </summary>
        /// <param name="phyid">The phyid.</param>
        /// <returns></returns>
        public List<UserDefinedDescriptions> GetFavoriteByPhyId(int phyid)
        {
            using (var rep = UnitOfWork.FavoritesRepository)
            {
                var fav = rep.Where(f => f.UserID == phyid && (f.IsDeleted == null || !(bool)f.IsDeleted)).ToList();
                return fav;
            }
        }

        /// <summary>
        /// Deletes the fav.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public bool DeleteFav(int id)
        {
            using (var rep = UnitOfWork.FavoritesRepository)
            {
                var isDeleted = rep.Delete(id);
                return isDeleted != null;
            }
        }

        /// <summary>
        /// Gets the favorite orders.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public List<FavoritesCustomModel> GetFavoriteOrders(int userId)
        {
            var favOrders = new List<FavoritesCustomModel>();
            using (var rep = UnitOfWork.FavoritesRepository)
            {
                var favorites =
                    rep.Where(f => f.UserID == userId && (f.IsDeleted == null || !(bool)f.IsDeleted))
                        .ToList();
                favOrders.AddRange(favorites.Select(fav => new FavoritesCustomModel()
                {
                    UserDefinedDescriptionID = fav.UserDefinedDescriptionID,
                    CategoryId = fav.CategoryId,
                    CodeId = fav.CodeId,
                    RoleID = fav.RoleID,
                    UserID = fav.UserID,
                    UserDefineDescription = fav.UserDefineDescription,
                    CreatedBy = fav.CreatedBy,
                    CreatedDate = fav.CreatedDate,
                    ModifiedBy = fav.ModifiedBy,
                    Modifieddate = fav.Modifieddate,
                    IsDeleted = fav.IsDeleted,
                    DeletedDate = fav.DeletedDate,
                    CategoryName = GetNameByGlobalCodeValue((fav.CategoryId), Convert.ToInt32(GlobalCodeCategoryValue.CodeTypes).ToString()),
                    CodeDesc = GetCodeDescription(fav.CodeId, fav.CategoryId)
                }));
            }
            return favOrders;
        }

        /// <summary>
        /// Gets the favorite by code identifier phy identifier.
        /// </summary>
        /// <param name="codeId">The code identifier.</param>
        /// <param name="phyId">The phy identifier.</param>
        /// <returns></returns>
        public UserDefinedDescriptions GetFavoriteByCodeIdPhyId(string codeId, int phyId)
        {
            using (var rep = UnitOfWork.FavoritesRepository)
            {
                var fav = rep.Where(f => f.CodeId.Equals(codeId) && f.UserID == phyId).FirstOrDefault();
                if (fav == null)
                {
                    var userDefinedCodeId = 0;
                    var udefinedCodeId = Int32.TryParse(codeId, out userDefinedCodeId);
                    if (userDefinedCodeId != 0)
                    {
                        fav = rep.Where(f => f.UserDefinedDescriptionID == userDefinedCodeId).FirstOrDefault();
                    }
                }
                return fav;
            }
        }

        /// <summary>
        /// Checks if already fav.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <param name="codeId">The code identifier.</param>
        /// <param name="categoryId">The category identifier.</param>
        /// <returns></returns>
        public bool CheckIfAlreadyFav(int userid, string codeId, string categoryId)
        {
            using (var rep = UnitOfWork.FavoritesRepository)
            {
                var favorites =
                    rep.Where(
                        f =>
                            f.UserID == userid && (f.IsDeleted == null || !(bool)f.IsDeleted) && f.CodeId == codeId &&
                            f.CategoryId == categoryId)
                        .FirstOrDefault();
                return favorites != null;
            }
        }


        public IEnumerable<FavoritesCustomModel> GetFavoriteDiagnosisData(long userId)
        {
            using (var rep = UnitOfWork.DiagnosisRespository)
                return rep.GetFavoriteDiagnosisData(userId, DiagnosisTableNumber, DrgTableNumber);
        }
    }
}

