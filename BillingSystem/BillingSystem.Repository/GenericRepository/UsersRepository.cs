using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using System.Collections.Generic;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Repository.GenericRepository
{
    public class UsersRepository : GenericRepository<Users>
    {
        private readonly DbContext _context;

        public UsersRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }

        /// <summary>
        /// Gets the registration productivity data.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="displayType">The display type.</param>
        /// <param name="tillDate">The till date.</param>
        /// <returns></returns>
        public List<RegistrationProductivity> GetRegistrationProductivityData(int facilityId, int displayType, DateTime? tillDate)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pFacilityID, @pDisplayTypeID, @pTillDate", StoredProcedures.SPROC_GetDBRegistrationProductivity);
                    var sqlParameters = new object[3];
                    sqlParameters[0] = new SqlParameter("pFacilityID", facilityId);
                    sqlParameters[1] = new SqlParameter("pDisplayTypeID", displayType);
                    sqlParameters[2] = new SqlParameter("pTillDate", tillDate);
                    IEnumerable<RegistrationProductivity> result = _context.Database.SqlQuery<RegistrationProductivity>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        /// <summary>
        /// Gets the high charts registration productivity data.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateid">The corporateid.</param>
        /// <param name="displayType">The display type.</param>
        /// <param name="fiscalyear">The fiscalyear.</param>
        /// <param name="graphtype">The graphtype.</param>
        /// <returns></returns>
        public List<DashboardChargesCustomModel> GetHighChartsRegistrationProductivityData(int facilityId, int corporateid, string displayType, string fiscalyear, string graphtype)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pCorporateID, @pFacilityID, @pType, @pFiscalYear, @pGraphType", StoredProcedures.SPROC_GetCounterRegistrationProductivity);
                    var sqlParameters = new object[5];
                    sqlParameters[0] = new SqlParameter("pCorporateID", corporateid);
                    sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
                    sqlParameters[2] = new SqlParameter("pType", displayType);
                    sqlParameters[3] = new SqlParameter("pFiscalYear", fiscalyear);
                    sqlParameters[4] = new SqlParameter("pGraphType", graphtype);
                    IEnumerable<DashboardChargesCustomModel> result = _context.Database.SqlQuery<DashboardChargesCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        /// <summary>
        /// Gets the high charts billing trend data.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateid">The corporateid.</param>
        /// <param name="displayType">The display type.</param>
        /// <param name="fiscalyear">The fiscalyear.</param>
        /// <returns></returns>
        public List<DashboardChargesCustomModel> GetHighChartsBillingTrendData(int facilityId, int corporateid, string displayType, string fiscalyear)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @CID, @FID, @BudgetFor, @pFiscalYear", StoredProcedures.SPROC_GetBillingTrendData);
                    var sqlParameters = new object[4];
                    sqlParameters[0] = new SqlParameter("CID", corporateid);
                    sqlParameters[1] = new SqlParameter("FID", facilityId);
                    sqlParameters[2] = new SqlParameter("BudgetFor", displayType);
                    sqlParameters[3] = new SqlParameter("pFiscalYear", fiscalyear);
                    IEnumerable<DashboardChargesCustomModel> result = _context.Database.SqlQuery<DashboardChargesCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        /// <summary>
        /// Gets the bill edit role user.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<BillEditorUsersCustomModel> GetBillEditRoleUser(int corporateId,int facilityId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pCID, @pFID", StoredProcedures.SPROC_GetBillEditRoleUser);
                    var sqlParameters = new object[2];
                    sqlParameters[0] = new SqlParameter("pCID", corporateId);
                    sqlParameters[1] = new SqlParameter("pFID", facilityId);
                    IEnumerable<BillEditorUsersCustomModel> result = _context.Database.SqlQuery<BillEditorUsersCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }


        public List<UsersViewModel> GetUsersRoleWise(int facilityId, int cId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @CId, @FId", StoredProcedures.SPROC_GetUsersRoleWise);
                    var sqlParameters = new object[2];
                    sqlParameters[0] = new SqlParameter("CId", cId);
                    sqlParameters[1] = new SqlParameter("FId", facilityId);
                    IEnumerable<UsersViewModel> result = _context.Database.SqlQuery<UsersViewModel>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }
    }
}
