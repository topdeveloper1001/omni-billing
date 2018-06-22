using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using BillingSystem.Common.Common;
using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class ServiceCodeRepository : GenericRepository<ServiceCode>
    {
        private readonly DbContext _context;
        public ServiceCodeRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }

        /// <summary>
        /// Saves the data save billing table data for table number.
        /// </summary>
        /// <param name="tableNumber">The table number.</param>
        /// <param name="selectedCodeid">The selected codeid.</param>
        /// <param name="typeid">The typeid.</param>
        /// <returns></returns>
        public bool SaveDataSaveBillingTableDataForTableNumber(string tableNumber, string selectedCodeid, string typeid)
        {
            try
            {
                if (_context != null)
                {
                    var spName =
                        string.Format(
                            "EXEC {0} @ServiceCodeIds, @tableNumber, @type",
                            StoredProcedures.SPROC_SaveBillingTableDataForTableNumber);
                    var sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("ServiceCodeIds", selectedCodeid);
                    sqlParameters[1] = new SqlParameter("tableNumber", tableNumber);
                    sqlParameters[2] = new SqlParameter("type", typeid);
                    ExecuteCommand(spName, sqlParameters);
                    return true;
                }
            }
            catch (Exception )
            {
                //throw ex;
            }
            return false;
        }


        /// <summary>
        /// Updates the Bed Rate Card table values whenever user changes the Service code values
        /// </summary>
        /// <param name="serviceCodeId"></param>
        /// <returns></returns>
        public bool UpdateBedRateCardByPassingServiceCodes(Int32 serviceCodeId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pServiceCodeId",
                        StoredProcedures.SPROC_UpdateBedRateCardByPassingServiceCodes);
                    var sqlParameters = new SqlParameter[1];
                    sqlParameters[0] = new SqlParameter("pServiceCodeId", serviceCodeId);
                    ExecuteCommand(spName, sqlParameters);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }


        /// <summary>
        /// Gets the encounter bill detail view.
        /// </summary>
        /// <param name="tableNumber">The Billing Code Table Number.</param>
        /// <returns></returns>
        public List<ServiceCode> GetOverrideableBeds(string tableNumber)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @ServiceCodeTableNumber ", StoredProcedures.SPROC_GetOverridableBeds);
                    var sqlParameters = new SqlParameter[1];
                    sqlParameters[0] = new SqlParameter("ServiceCodeTableNumber ", tableNumber);
                    IEnumerable<ServiceCode> result = _context.Database.SqlQuery<ServiceCode>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception )
            {
                //throw ex;
            }
            return new List<ServiceCode>();
        }
    }
}
