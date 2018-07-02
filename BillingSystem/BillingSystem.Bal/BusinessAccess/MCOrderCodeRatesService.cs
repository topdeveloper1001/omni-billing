using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;


namespace BillingSystem.Bal.BusinessAccess
{
    public class MCOrderCodeRatesService : IMCOrderCodeRatesService
    {
        private readonly IRepository<MCOrderCodeRates> _repository;
        private readonly IRepository<GlobalCodes> _gRepository;

        public MCOrderCodeRatesService(IRepository<MCOrderCodeRates> repository, IRepository<GlobalCodes> gRepository)
        {
            _repository = repository;
            _gRepository = gRepository;
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
        /// Get the Entity
        /// </summary>
        /// <returns>
        /// Return the Entity List
        /// </returns>
        public List<MCOrderCodeRatesCustomModel> GetMCOrderCodeRatesList()
        {
            var list = new List<MCOrderCodeRatesCustomModel>();
            var lstMcOrderCodeRates = _repository.GetAll().ToList();
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
            return list;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public List<MCOrderCodeRatesCustomModel> SaveMCOrderCodeRates(MCOrderCodeRates model)
        {
            if (model.MCOrderCodeRatesId > 0)
            {
                var current = _repository.GetSingle(model.MCOrderCodeRatesId);
                _repository.UpdateEntity(model, model.MCOrderCodeRatesId);
            }
            else
                _repository.Create(model);

            var currentId = model.MCOrderCodeRatesId;
            var managecarecode = model.MCCode;
            var list = GetMcOrderCodeRatesListByMcCode(Convert.ToInt32(managecarecode));
            return list;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="mcOrderCodeRatesId">The mc order code rates identifier.</param>
        /// <returns></returns>
        public MCOrderCodeRates GetMCOrderCodeRatesByID(int? mcOrderCodeRatesId)
        {
            var model = _repository.Where(x => x.MCOrderCodeRatesId == mcOrderCodeRatesId).FirstOrDefault();
            return model;
        }

        /// <summary>
        /// Gets the mc order code rates list by mc code.
        /// </summary>
        /// <param name="mcCode">The mc code.</param>
        /// <returns></returns>
        public List<MCOrderCodeRatesCustomModel> GetMcOrderCodeRatesListByMcCode(int mcCode)
        {
            var list = new List<MCOrderCodeRatesCustomModel>();
            var lstMcOrderCodeRates = _repository.Where(x => x.MCCode == mcCode).ToList();
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
            return list;
        }

        /// <summary>
        /// Deletes the mc order code rates.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public int DeleteMCOrderCodeRates(int id)
        {
            _repository.Delete(id);
            return id;
        }
    }
}
