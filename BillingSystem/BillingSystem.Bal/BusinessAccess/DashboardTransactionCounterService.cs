using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;


namespace BillingSystem.Bal.BusinessAccess
{
    public class DashboardTransactionCounterService : IDashboardTransactionCounterService
    {

        private readonly IRepository<DashboardTransactionCounter> _repository;
        private readonly IRepository<GlobalCodes> _gRepository;

        private readonly BillingEntities _context;

        public DashboardTransactionCounterService(IRepository<DashboardTransactionCounter> repository, IRepository<GlobalCodes> gRepository, BillingEntities context)
        {
            _repository = repository;
            _gRepository = gRepository;
            _context = context;
        }

        /// <summary>
        /// Gets the dashboard transaction counter.
        /// </summary>
        /// <returns></returns>
        public List<DashboardTransactionCounterCustomModel> GetDashboardTransactionCounter(int corporateid, int facilityid)
        {
            var list = new List<DashboardTransactionCounterCustomModel>();
            var lstDashboardTransactionCounter = _repository.Where(a => a.CorporateId == corporateid && a.FacilityId == facilityid && (a.IsActive == null || (bool)a.IsActive)).ToList();
            if (lstDashboardTransactionCounter.Count > 0)
            {
                list.AddRange(
                    lstDashboardTransactionCounter.Select(item => new DashboardTransactionCounterCustomModel
                    {
                        CounterId = item.CounterId,
                        StatisticDescription = item.StatisticDescription,
                        ActivityDay = item.ActivityDay,
                        ActivityTotal = item.ActivityTotal,
                        DepartmentNumber = item.DepartmentNumber,
                        CorporateId = item.CorporateId,
                        FacilityId = item.FacilityId,
                        CreatedBy = item.CreatedBy,
                        CreatedDate = item.CreatedDate,
                        ModifiedBy = item.ModifiedBy,
                        ModifiedDate = item.ModifiedDate,
                        IsActive = item.IsActive,
                        DashboardStatDescstring = GetNameByGlobalCodeValue(item.StatisticDescription.ToString(), Convert.ToInt32(GlobalCodeCategoryValue.DashboardStatDesc).ToString()),
                    }));
            }
            return list.OrderByDescending(x => x.ActivityDay).ThenBy(x => x.StatisticDescription).ToList();
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
        /// Saves the dashboard transaction counter.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public int SaveDashboardTransactionCounter(DashboardTransactionCounter model)
        {
            if (model.CounterId > 0)
                _repository.UpdateEntity(model, model.CounterId);
            else
                _repository.Create(model);
            return model.CounterId;
        }


        /// <summary>
        /// Gets the dashboard transaction counter by identifier.
        /// </summary>
        /// <param name="DashboardTransactionCounterId">The dashboard transaction counter identifier.</param>
        /// <returns></returns>
        public DashboardTransactionCounter GetDashboardTransactionCounterById(int? DashboardTransactionCounterId)
        {
            var model = _repository.Where(x => x.CounterId == DashboardTransactionCounterId).FirstOrDefault();
            return model;
        }

        /// <summary>
        /// Deletes the dashboard transaction counter.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public int DeleteDashboardTransactionCounter(DashboardTransactionCounter model)
        {
            return _repository.Delete(model);
        }

        public List<DashboardTransactionCounterCustomModel> GetDashboardTrancationData(int corporateId, int facilityId)
        {
            var spName = string.Format("EXEC {0} @Corporateid,@FacilityId", StoredProcedures.SPORC_GetDashboardTransactionCounterData);
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("Corporateid", corporateId);
            sqlParameters[1] = new SqlParameter("FacilityId", facilityId);
            var result = _context.Database.SqlQuery<DashboardTransactionCounterCustomModel>(spName, sqlParameters);
            return result.ToList();
        }
    }
}
