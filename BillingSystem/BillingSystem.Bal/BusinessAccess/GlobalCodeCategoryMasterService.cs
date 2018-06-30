using System.Linq;
using BillingSystem.Model;
using System.Collections.Generic;
using System;
using BillingSystem.Repository.Interfaces;
using System.Data.SqlClient;
using BillingSystem.Common.Common;
using BillingSystem.Repository.Common;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class GlobalCodeCategoryMasterService : IGlobalCodeCategoryMasterService
    {
        private readonly IRepository<GlobalCodeCategory> _gcRepository;
        private readonly BillingEntities _context;

        public GlobalCodeCategoryMasterService(IRepository<GlobalCodeCategory> gcRepository, BillingEntities context)
        {
            _gcRepository = gcRepository;
            _context = context;
        }

        //Function to get all GlobalCodeCategories
        public List<GlobalCodeCategory> GetAllGlobalCodeCategories()
        {
            var list = new List<GlobalCodeCategory>();
            list = _gcRepository.Where(s => s.IsDeleted == false && s.IsActive).ToList();
            return list;
        }


        //Function to get GlobalCodes by GlobalCodeID
        public GlobalCodeCategory GetGlobalCategoriesByGlobalCodeCategoryId(int globalCodeCategoriesID)
        {
            var m = _gcRepository.Where(s => s.IsDeleted == false && s.GlobalCodeCategoryID == globalCodeCategoriesID).FirstOrDefault();
            return m;

        }

        //Function to add update tab
        public int AddUpdateGlobalCategory(GlobalCodeCategory m)
        {
            try
            {
                var name = m.GlobalCodeCategoryName.ToLower().Trim();
                var isExists = _gcRepository.Where(g => g.FacilityNumber.Equals(m.FacilityNumber)
                && g.GlobalCodeCategoryID != m.GlobalCodeCategoryID
                && (g.GlobalCodeCategoryName.ToLower().Trim().Equals(name)
                || g.GlobalCodeCategoryValue.ToLower().Trim().Equals(m.GlobalCodeCategoryValue))
                && g.IsActive && g.IsDeleted != true
                ).Any();

                if (!isExists)
                {
                    if (m.GlobalCodeCategoryID > 0)
                    {
                        var current = _gcRepository.GetSingle(m.GlobalCodeCategoryID);
                        m.CreatedBy = current.CreatedBy;
                        m.CreatedDate = current.CreatedDate;
                        _gcRepository.UpdateEntity(m, m.GlobalCodeCategoryID);
                    }
                    else
                        _gcRepository.Create(m);
                }
                else
                    return -1;

                return m.GlobalCodeCategoryID;

            }
            catch (Exception x)
            {
                throw x;
            }

        }

        //Function to get all GlobalCodeCategories
        public List<GlobalCodeCategory> GetAllGlobalCodeCategoriesByOrderType(string orderType, string fn = "")
        {
            var list = new List<GlobalCodeCategory>();
            list = _gcRepository.Where(s => s.IsDeleted == false && s.GroupCode == orderType && (string.IsNullOrEmpty(fn) || s.FacilityNumber.Equals(fn))).ToList();
            return list;

        }


        public List<GlobalCodeCategory> GetOrderTypeCategoriesByFacility(long facilityId, long userId, bool status = true)
        {
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("pUserId", userId);
            sqlParameters[1] = new SqlParameter("pFId", facilityId);
            sqlParameters[2] = new SqlParameter("pStatusId", status);

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetOrderTypeCategoriesByFacility.ToString(), isCompiled: false, parameters: sqlParameters))
            {
                var result = ms.GetResultWithJson<GlobalCodeCategory>(JsonResultsArray.GlobalCategory.ToString());
                return result;
            }
        }
    }
}
