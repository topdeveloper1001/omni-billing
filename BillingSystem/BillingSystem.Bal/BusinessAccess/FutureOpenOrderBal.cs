// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FutureOpenOrderBal.cs" company="Spadez">
//   OmniHealthcare
// </copyright>
// <summary>
//   The holiday planner bal.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Bal.BusinessAccess
{
    using System.Collections.Generic;
    using System.Linq;

    using BillingSystem.Bal.Mapper;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;
    using BillingSystem.Model.Model;
    using BillingSystem.Repository.GenericRepository;

    /// <summary>
    /// The holiday planner bal.
    /// </summary>
    public class FutureOpenOrderBal : BaseBal
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FutureOpenOrderBal"/> class.
        /// </summary>
        public FutureOpenOrderBal()
        {
            this.FutureOpenOrderMapper = new FutureOpenOrderMapper();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FutureOpenOrderBal"/> class.
        /// </summary>
        /// <param name="cptTableNumber">The CPT table number.</param>
        /// <param name="serviceCodeTableNumber">The service code table number.</param>
        /// <param name="drgTableNumber">The DRG table number.</param>
        /// <param name="drugTableNumber">The drug table number.</param>
        /// <param name="hcpcsTableNumber">The HCPCS table number.</param>
        /// <param name="diagnosisTableNumber">The diagnosis table number.</param>
        public FutureOpenOrderBal(string cptTableNumber, string serviceCodeTableNumber, string drgTableNumber, string drugTableNumber, string hcpcsTableNumber, string diagnosisTableNumber)
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

            this.FutureOpenOrderMapper = new FutureOpenOrderMapper(CptTableNumber, ServiceCodeTableNumber, DrgTableNumber, DrugTableNumber, HcpcsTableNumber, DiagnosisTableNumber);
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the holiday planner mapper.
        /// </summary>
        private FutureOpenOrderMapper FutureOpenOrderMapper { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<FutureOpenOrderCustomModel> GetFutureOpenOrder()
        {
            var list = new List<FutureOpenOrderCustomModel>();
            using (var futureOpenOrderRep = this.UnitOfWork.FutureOpenOrderRepository)
            {
                var lstFutureOpenOrder = futureOpenOrderRep.GetAll().ToList();
                if (lstFutureOpenOrder.Count > 0)
                {
                    list.AddRange(lstFutureOpenOrder.Select(item => FutureOpenOrderMapper.MapModelToViewModel(item)));
                }
            }

            return list;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="FutureOpenOrderId">The Holiday Planner Id.</param>
        /// <returns>
        /// The <see cref="FutureOpenOrder" />.
        /// </returns>
        public FutureOpenOrder GetFutureOpenOrderById(int? FutureOpenOrderId)
        {
            using (FutureOpenOrderRepository rep = this.UnitOfWork.FutureOpenOrderRepository)
            {
                FutureOpenOrder model = rep.Where(x => x.FutureOpenOrderID == FutureOpenOrderId).FirstOrDefault();
                return model;
            }
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int SaveFutureOpenOrder(FutureOpenOrder model)
        {
            using (var rep = this.UnitOfWork.FutureOpenOrderRepository)
            {
                if (model.FutureOpenOrderID > 0)
                {
                    rep.UpdateEntity(model, model.FutureOpenOrderID);
                }
                else
                {
                    rep.Create(model);
                }
                return model.FutureOpenOrderID;
            }
        }

        /// <summary>
        /// Gets the future open order by enc identifier.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public List<FutureOpenOrderCustomModel> GetFutureOpenOrderByEncId(int? encounterId)
        {
            var list = new List<FutureOpenOrderCustomModel>();
            using (var futureOpenOrderRep = this.UnitOfWork.FutureOpenOrderRepository)
            {
                var lstFutureOpenOrder = futureOpenOrderRep.Where(x => x.EncounterID == encounterId).ToList();
                if (lstFutureOpenOrder.Count > 0)
                {
                    list.AddRange(lstFutureOpenOrder.Select(item => FutureOpenOrderMapper.MapModelToViewModel(item)));
                }
            }

            return list;
        }

        /// <summary>
        /// Gets the future open order by patient identifier.
        /// </summary>
        /// <param name="pid">The pid.</param>
        /// <returns></returns>
        public List<FutureOpenOrderCustomModel> GetFutureOpenOrderByPatientId(int? pid)
        {
            var list = new List<FutureOpenOrderCustomModel>();
            using (var futureOpenOrderRep = this.UnitOfWork.FutureOpenOrderRepository)
            {
                list = futureOpenOrderRep.GetFutureOpenOrderByPatientId(pid);
            }
            return list;
        }


        /// <summary>
        /// Adds the current enc to future orders.
        /// </summary>
        /// <param name="ordersId">The orders identifier.</param>
        /// <param name="encId">The enc identifier.</param>
        /// <param name="cid">The cid.</param>
        /// <param name="fid">The fid.</param>
        /// <returns></returns>
        public bool AddCurrentEncToFutureOrders(string[] ordersId, int encId, int cid,int fid)
        {
            var orderIds = string.Join(",", ordersId);
            using (var futureOpenOrderRep = this.UnitOfWork.FutureOpenOrderRepository)
            {
                return futureOpenOrderRep.AddCurrentEncToFutureOrders(orderIds, encId, cid, fid);
            }
        }

        #endregion
    }
}
