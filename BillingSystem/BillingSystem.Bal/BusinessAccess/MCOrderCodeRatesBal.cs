using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Common.Common;
using BillingSystem.Repository.UOW;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.BusinessAccess
{
    public class MCOrderCodeRatesBal : BaseBal
    {
        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>
        /// Return the Entity List
        /// </returns>
        public List<MCOrderCodeRatesCustomModel> GetMCOrderCodeRatesList()
        {
            var list = new List<MCOrderCodeRatesCustomModel>();
            using (var mcOrderCodeRatesRep = UnitOfWork.MCOrderCodeRatesRepository)
            {
                var lstMcOrderCodeRates = mcOrderCodeRatesRep.GetAll().ToList();
                if (lstMcOrderCodeRates.Count > 0)
                {
                    list.AddRange(lstMcOrderCodeRates.Select(item => new MCOrderCodeRatesCustomModel
                    {
                        MCOrderCodeRatesId = item.MCOrderCodeRatesId,
                        MCCode = item.MCCode,
                        OrderCodeRatTableNumber = item.OrderCodeRatTableNumber,
                        OrderCodeRateTableName = item.OrderCodeRateTableName,
                        OrderType = item.OrderType,
                        OrderCode = item.OrderCode,
                        OrderCodeDescription = item.OrderCodeDescription,
                        OrderCodePerDiemRate = item.OrderCodePerDiemRate,
                        OrderCodeAddOns = item.OrderCodeAddOns,
                        OrderCodeAddOnTableNumber = item.OrderCodeAddOnTableNumber,
                        OrderTypeStr = GetNameByGlobalCodeValue(item.OrderType.ToString(), Convert.ToInt32(GlobalCodeCategoryValue.CodeTypes).ToString()),
                    }));
                }
            }
            return list;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public List<MCOrderCodeRatesCustomModel> SaveMCOrderCodeRates(MCOrderCodeRates model)
        {
            using (var rep = UnitOfWork.MCOrderCodeRatesRepository)
            {
                if (model.MCOrderCodeRatesId > 0)
                {
                    var current = rep.GetSingle(model.MCOrderCodeRatesId);
                    rep.UpdateEntity(model, model.MCOrderCodeRatesId);
                }
                else
                    rep.Create(model);

                var currentId = model.MCOrderCodeRatesId;
                var managecarecode = model.MCCode;
                var list = GetMcOrderCodeRatesListByMcCode(Convert.ToInt32(managecarecode));
                return list;
            }
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="mcOrderCodeRatesId">The mc order code rates identifier.</param>
        /// <returns></returns>
        public MCOrderCodeRates GetMCOrderCodeRatesByID(int? mcOrderCodeRatesId)
        {
            using (var rep = UnitOfWork.MCOrderCodeRatesRepository)
            {
                var model = rep.Where(x => x.MCOrderCodeRatesId == mcOrderCodeRatesId).FirstOrDefault();
                return model;
            }
        }

        /// <summary>
        /// Gets the mc order code rates list by mc code.
        /// </summary>
        /// <param name="mcCode">The mc code.</param>
        /// <returns></returns>
        public List<MCOrderCodeRatesCustomModel> GetMcOrderCodeRatesListByMcCode(int mcCode)
        {
            var list = new List<MCOrderCodeRatesCustomModel>();
            using (var mcOrderCodeRatesRep = UnitOfWork.MCOrderCodeRatesRepository)
            {
                var lstMcOrderCodeRates = mcOrderCodeRatesRep.Where(x => x.MCCode == mcCode).ToList();
                if (lstMcOrderCodeRates.Count > 0)
                {
                    list.AddRange(lstMcOrderCodeRates.Select(item => new MCOrderCodeRatesCustomModel
                    {
                        MCOrderCodeRatesId = item.MCOrderCodeRatesId,
                        MCCode = item.MCCode,
                        OrderCodeRatTableNumber = item.OrderCodeRatTableNumber,
                        OrderCodeRateTableName = item.OrderCodeRateTableName,
                        OrderType = item.OrderType,
                        OrderCode = item.OrderCode,
                        OrderCodeDescription = item.OrderCodeDescription,
                        OrderCodePerDiemRate = item.OrderCodePerDiemRate,
                        OrderCodeAddOns = item.OrderCodeAddOns,
                        OrderCodeAddOnTableNumber = item.OrderCodeAddOnTableNumber,
                        OrderTypeStr = GetNameByGlobalCodeValue(item.OrderType.ToString(), Convert.ToInt32(GlobalCodeCategoryValue.CodeTypes).ToString()),
                    }));
                }
            }
            return list;
        }

        /// <summary>
        /// Deletes the mc order code rates.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public int DeleteMCOrderCodeRates(int id)
        {
            using (var rep = UnitOfWork.MCOrderCodeRatesRepository)
            {
                rep.Delete(id);
                return id;
            }
        }
    }
}
