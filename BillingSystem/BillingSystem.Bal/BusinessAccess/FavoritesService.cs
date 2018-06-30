using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Query.Dynamic;
using BillingSystem.Common.Common;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;
using BillingSystem.Repository.Interfaces;
using BillingSystem.Repository.Common;
using System.Data.SqlClient;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class FavoritesService : IFavoritesService
    {

        private readonly IRepository<UserDefinedDescriptions> _repository;
        private readonly IRepository<GlobalCodes> _gRepository;

        private readonly IRepository<CPTCodes> _cRepository;
        private readonly IRepository<DRGCodes> _dRepository;
        private readonly IRepository<HCPCSCodes> _hcRepository;
        private readonly IRepository<Drug> _drugRepository;
        private readonly IRepository<ServiceCode> _sRepository;
        private readonly IRepository<DiagnosisCode> _dcRepository;

        private readonly BillingEntities _context;

        public FavoritesService(IRepository<UserDefinedDescriptions> repository, IRepository<GlobalCodes> gRepository, IRepository<CPTCodes> cRepository, IRepository<DRGCodes> dRepository, IRepository<HCPCSCodes> hcRepository, IRepository<Drug> drugRepository, IRepository<ServiceCode> sRepository, IRepository<DiagnosisCode> dcRepository, BillingEntities context)
        {
            _repository = repository;
            _gRepository = gRepository;
            _cRepository = cRepository;
            _dRepository = dRepository;
            _hcRepository = hcRepository;
            _drugRepository = drugRepository;
            _sRepository = sRepository;
            _dcRepository = dcRepository;
            _context = context;
        }

        /// <summary>
        /// Gets the favorite by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public UserDefinedDescriptions GetFavoriteById(int id)
        {
                var fav = _repository.Where(f => f.UserDefinedDescriptionID == id).FirstOrDefault();
                return fav;
        }

        /// <summary>
        /// Adds to favorites.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="isFavorite">if set to <c>true</c> [is favorite].</param>
        /// <returns></returns>
        public int AddToFavorites(UserDefinedDescriptions model, bool isFavorite)
        {
            if (model.UserDefinedDescriptionID > 0)
            {
                model.IsDeleted = !isFavorite;
                model.DeletedBy = !isFavorite ? model.ModifiedBy : null;
                model.DeletedDate = !isFavorite ? model.Modifieddate : null;
                _repository.UpdateEntity(model, model.UserDefinedDescriptionID);
            }
            else
                _repository.Create(model);

            return model.UserDefinedDescriptionID;
        }

        /// <summary>
        /// Gets the favorite by code identifier.
        /// </summary>
        /// <param name="codeId">The code identifier.</param>
        /// <returns></returns>
        public UserDefinedDescriptions GetFavoriteByCodeId(string codeId)
        {
            var fav = _repository.Where(f => f.CodeId.Equals(codeId)).FirstOrDefault();
            if (fav == null)
            {
                var userDefinedCodeId = 0;
                var udefinedCodeId = Int32.TryParse(codeId, out userDefinedCodeId);
                if (userDefinedCodeId != 0)
                {
                    fav = _repository.Where(f => f.UserDefinedDescriptionID == userDefinedCodeId).FirstOrDefault();
                }
            }
            return fav;
        }

        /// <summary>
        /// Gets the favorite by phy identifier.
        /// </summary>
        /// <param name="phyid">The phyid.</param>
        /// <returns></returns>
        public List<UserDefinedDescriptions> GetFavoriteByPhyId(int phyid)
        {
            var fav = _repository.Where(f => f.UserID == phyid && (f.IsDeleted == null || !(bool)f.IsDeleted)).ToList();
            return fav;
        }

        /// <summary>
        /// Deletes the fav.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public bool DeleteFav(int id)
        {
            var isDeleted = _repository.Delete(id);
            return isDeleted > 0;
        }

        /// <summary>
        /// Gets the favorite orders.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public List<FavoritesCustomModel> GetFavoriteOrders(int userId, string CptTableNumber, string DrgTableNumber, string HcpcsTableNumber, string DrugTableNumber,
           string ServiceCodeTableNumber, string DiagnosisTableNumber)
        {
            var favOrders = new List<FavoritesCustomModel>();

            var favorites =
                _repository.Where(f => f.UserID == userId && (f.IsDeleted == null || !(bool)f.IsDeleted))
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
                CodeDesc = GetCodeDescription(fav.CodeId, fav.CategoryId, CptTableNumber, DrgTableNumber, HcpcsTableNumber, DrugTableNumber, ServiceCodeTableNumber, DiagnosisTableNumber)
            }));

            return favOrders;
        }
        private string GetCodeDescription(string orderCode, string orderType, string CptTableNumber, string DrgTableNumber, string HcpcsTableNumber, string DrugTableNumber,
           string ServiceCodeTableNumber, string DiagnosisTableNumber)
        {
            var codeDescription = string.Empty;

            if (!string.IsNullOrEmpty(orderCode) && !string.IsNullOrEmpty(orderType))
            {
                var codeType = (OrderType)Enum.Parse(typeof(OrderType), orderType);
                switch (codeType)
                {
                    case OrderType.CPT:
                        codeDescription = _cRepository.Where(x => x.CodeNumbering.Contains(orderCode) && x.CodeTableNumber.Trim().Equals(CptTableNumber)).FirstOrDefault().CodeDescription;
                        return codeDescription;
                    case OrderType.DRG:
                        codeDescription = _dRepository.Where(d => d.CodeNumbering == orderCode && d.CodeTableNumber.Trim().Equals(DrgTableNumber)).FirstOrDefault().CodeDescription;
                        return codeDescription;
                    case OrderType.HCPCS:
                        codeDescription = _hcRepository.Where(x => x.CodeNumbering == orderCode && x.CodeTableNumber.Trim().Equals(HcpcsTableNumber)).FirstOrDefault().CodeDescription;
                        return codeDescription;
                    case OrderType.DRUG:
                        codeDescription = _drugRepository.Where(x => x.DrugCode == orderCode && x.DrugTableNumber.Trim().Equals(DrugTableNumber)).FirstOrDefault().DrugDescription;
                        return codeDescription;
                    case OrderType.BedCharges:
                        codeDescription = _sRepository.Where(s => s.ServiceCodeValue.Equals(orderCode) && s.ServiceCodeTableNumber.Trim().Equals(ServiceCodeTableNumber)).FirstOrDefault().ServiceCodeDescription;
                        return codeDescription;
                    case OrderType.Diagnosis:
                        codeDescription = _dcRepository.Where(d => d.DiagnosisCode1 == orderCode && d.DiagnosisTableNumber.Trim().Equals(DiagnosisTableNumber)).FirstOrDefault().DiagnosisFullDescription;
                        return codeDescription;
                    case OrderType.ServiceCode:
                        codeDescription = _sRepository.Where(s => s.ServiceCodeValue.Equals(orderCode) && s.ServiceCodeTableNumber.Trim().Equals(ServiceCodeTableNumber)).FirstOrDefault().ServiceCodeDescription;
                        return codeDescription;
                    default:
                        break;
                }
            }
            return codeDescription;
        }
        private string GetNameByGlobalCodeValue(string codeValue, string categoryValue, string fId = "")
        {
            if (!string.IsNullOrEmpty(codeValue))
            {
                var gl = _gRepository.Where(g => g.GlobalCodeValue.Equals(codeValue) && !g.IsDeleted.Value && g.GlobalCodeCategoryValue.Equals(categoryValue) && (string.IsNullOrEmpty(fId) || g.FacilityNumber.Equals(fId))).FirstOrDefault();
                return gl != null ? gl.GlobalCodeName : string.Empty;
            }
            return string.Empty;
        }
        /// <summary>
        /// Gets the favorite by code identifier phy identifier.
        /// </summary>
        /// <param name="codeId">The code identifier.</param>
        /// <param name="phyId">The phy identifier.</param>
        /// <returns></returns>
        public UserDefinedDescriptions GetFavoriteByCodeIdPhyId(string codeId, int phyId)
        {
            var fav = _repository.Where(f => f.CodeId.Equals(codeId) && f.UserID == phyId).FirstOrDefault();
            if (fav == null)
            {
                var userDefinedCodeId = 0;
                var udefinedCodeId = Int32.TryParse(codeId, out userDefinedCodeId);
                if (userDefinedCodeId != 0)
                {
                    fav = _repository.Where(f => f.UserDefinedDescriptionID == userDefinedCodeId).FirstOrDefault();
                }
            }
            return fav;
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
            var favorites = _repository.Where(f => f.UserID == userid && (f.IsDeleted == null || !(bool)f.IsDeleted) && f.CodeId == codeId && f.CategoryId == categoryId).FirstOrDefault();
            return favorites != null;
        }


        public IEnumerable<FavoritesCustomModel> GetFavoriteDiagnosisData(long userId, string DiagnosisTableNumber, string DrgTableNumber)
        {
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("pUserId", userId);
            sqlParameters[1] = new SqlParameter("pDRGTN", DrgTableNumber);
            sqlParameters[2] = new SqlParameter("pDiagnosisTN", DiagnosisTableNumber);

            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetFavoriteDiagnosisData.ToString(), parameters: sqlParameters, isCompiled: false))
                return r.GetResultWithJson<FavoritesCustomModel>(JsonResultsArray.FavoriteDiagnosis.ToString());

        }
    }
}

