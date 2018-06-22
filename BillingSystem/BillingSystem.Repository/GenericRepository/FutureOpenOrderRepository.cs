// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FutureOpenOrderRepository.cs" company="Spadez">
//   OmniHelathcare
// </copyright>
// <summary>
//   The future open order repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Repository.GenericRepository
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.SqlClient;
    using System.Linq;

    using BillingSystem.Common.Common;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;
    using BillingSystem.Model.Model;

    /// <summary>
    /// The future open order repository.
    /// </summary>
    public class FutureOpenOrderRepository : GenericRepository<FutureOpenOrder>
    {
        #region Fields

        /// <summary>
        /// The _context.
        /// </summary>
        private readonly DbContext _context;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FutureOpenOrderRepository"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public FutureOpenOrderRepository(BillingEntities context)
            : base(context)
        {
            this.AutoSave = true;
            this._context = context;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The add current enc to future orders.
        /// </summary>
        /// <param name="orderIds">The order ids.</param>
        /// <param name="encId">The enc id.</param>
        /// <param name="cid">The cid.</param>
        /// <param name="fid">The fid.</param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        public bool AddCurrentEncToFutureOrders(string orderIds, int encId, int cid, int fid)
        {
            try
            {
                if (this._context != null)
                {
                    string spName = string.Format(
                        "EXEC {0} @pEncId, @pFutureOpenOrderId,@pCId,@pFId",
                        StoredProcedures.SPROC_AddFutureOrdersToCurrentEncounter);
                    var sqlParameters = new SqlParameter[4];
                    sqlParameters[0] = new SqlParameter("pEncId", encId);
                    sqlParameters[1] = new SqlParameter("pFutureOpenOrderId", orderIds);
                    sqlParameters[2] = new SqlParameter("pCId", cid);
                    sqlParameters[3] = new SqlParameter("pFId", fid);
                    this.ExecuteCommand(spName, sqlParameters);
                    return true;
                }
            }
            catch (Exception )
            {
                // throw ex;
            }

            return false;
        }

        /// <summary>
        /// Gets the future open order by patient identifier.
        /// </summary>
        /// <param name="pid">The pid.</param>
        /// <returns></returns>
        public List<FutureOpenOrderCustomModel> GetFutureOpenOrderByPatientId(int? pid)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pPId", StoredProcedures.SPROC_GetPaitentFutureOrder);
                    var sqlParameters = new SqlParameter[1];
                    sqlParameters[0] = new SqlParameter("pPId", pid);
                    IEnumerable<FutureOpenOrderCustomModel> result = _context.Database.SqlQuery<FutureOpenOrderCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        #endregion

        
    }
}