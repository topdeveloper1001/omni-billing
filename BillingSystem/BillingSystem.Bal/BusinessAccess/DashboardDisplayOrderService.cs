using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Repository.Interfaces;
using AutoMapper;
using System.Data.SqlClient;
using BillingSystem.Common.Common;
using BillingSystem.Repository.Common;

namespace BillingSystem.Bal.BusinessAccess
{
    public class DashboardDisplayOrderService : IDashboardDisplayOrderService
    {
        private readonly IRepository<DashboardDisplayOrder> _dbRepository;
        private readonly BillingEntities _context;
        private readonly IMapper _mapper;

        public DashboardDisplayOrderService(IRepository<DashboardDisplayOrder> repository, BillingEntities context, IMapper mapper)
        {
            _dbRepository = repository;
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<DashboardDisplayOrderCustomModel> GetDashboardDisplayOrderList(int? corporateid, long facilityId = 0)
        {
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter(InputParams.pFID.ToString(), facilityId);
            sqlParameters[1] = new SqlParameter(InputParams.pCID.ToString(), corporateid.GetValueOrDefault());
            using (var ms = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetDashboardDisplayOrders.ToString(), isCompiled: false
                , parameters: sqlParameters))
            {
                var result = ms.GetResultWithJson<DashboardDisplayOrderCustomModel>(JsonResultsArray.DashboardResult.ToString());
                return result;
            }
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="m">The model.</param>
        /// <returns></returns>
        public List<DashboardDisplayOrderCustomModel> SaveDashboardDisplayOrder(DashboardDisplayOrder m, bool isDeleted = false)
        {
            var executedStatus = false;
            if (m.Id > 0)
            {
                var current = _dbRepository.GetSingle(m.Id);
                m.CreatedBy = current.CreatedBy;
                m.CreatedDate = current.CreatedDate;

                if (isDeleted)
                {
                    m.IsDeleted = isDeleted;
                    m = current;
                }
                var r = _dbRepository.UpdateEntity(m, m.Id);
                executedStatus = r != null && r.Id > 0;
            }
            else
            {
                _dbRepository.Create(m);
                executedStatus = m.Id > 0;
            }

            var list = executedStatus
                ? GetDashboardDisplayOrderList(m.CorporateId.GetValueOrDefault(), m.FacilityId.GetValueOrDefault())
                : new List<DashboardDisplayOrderCustomModel>();
            return list;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="DashboardDisplayOrderId">The dashboard display order identifier.</param>
        /// <returns></returns>
        public DashboardDisplayOrder GetDashboardDisplayOrderByID(int id)
        {
            var m = _dbRepository.FindBy(x => x.Id == id).FirstOrDefault();
            return m;
        }
    }
}
