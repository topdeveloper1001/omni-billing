using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using AutoMapper;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class ServiceCodeService : IServiceCodeService
    {
        private readonly IRepository<ServiceCode> _repository;
        private readonly IRepository<BedRateCard> _bedRepository;
        private readonly IRepository<GlobalCodes> _gRepository;
        private readonly IRepository<GlobalCodeCategory> _ggRepository;
        private readonly IMapper _mapper;
        private readonly BillingEntities _context;

        public ServiceCodeService(IRepository<ServiceCode> repository, IRepository<BedRateCard> bedRepository, IRepository<GlobalCodes> gRepository, IRepository<GlobalCodeCategory> ggRepository, IMapper mapper, BillingEntities context)
        {
            _repository = repository;
            _bedRepository = bedRepository;
            _gRepository = gRepository;
            _ggRepository = ggRepository;
            _mapper = mapper;
            _context = context;
        }



        #region Public Methods and Operators

        /// <summary>
        /// Method to add the ServiceCode in the database.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int AddUpdateServiceCode(ServiceCode model, string ServiceCodeTableNumber)
        {
            int result;
            model.ServiceCodeTableNumber = ServiceCodeTableNumber;
            if (model.ServiceCodeId > 0)
            {
                var current = _repository.GetSingle(model.ServiceCodeId);
                model.CreatedBy = current.CreatedBy;
                model.CreatedDate = current.CreatedDate;
                model.ServiceCodeTableNumber = current.ServiceCodeTableNumber;
                model.ServiceCodeTableDescription = current.ServiceCodeTableDescription;
                _repository.UpdateEntity(model, model.ServiceCodeId);
            }
            else
            {
                _repository.Create(model);
            }

            result = model.ServiceCodeId;

            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("pServiceCodeId", model.ServiceCodeId);
            _repository.ExecuteCommand(StoredProcedures.SPROC_UpdateBedRateCardByPassingServiceCodes.ToString(), sqlParameters);
            return result;
        }

        /// <summary>
        /// The get filtered codes.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        /// The <see cref="List" />.
        /// </returns>
        public List<ServiceCode> GetFilteredCodes(string text, string ServiceCodeTableNumber)
        {
            var lstServiceCode = _repository.Where(s => (s.IsDeleted == null || !(bool)s.IsDeleted) && (s.ServiceCodeValue.Contains(text) || s.ServiceCodeDescription.Contains(text)) && s.ServiceCodeTableNumber.Trim().Equals(ServiceCodeTableNumber) && s.IsActive != false).Take(100).ToList();
            return lstServiceCode;
        }

        /// <summary>
        /// Gets the filtered codes c model.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public List<ServiceCodeCustomModel> GetFilteredCodesCModel(string text, string ServiceCodeTableNumber)
        {
            var list = new List<ServiceCodeCustomModel>();
            var lstServiceCode = _repository.Where(s => (s.IsDeleted == null || !(bool)s.IsDeleted) && (s.ServiceCodeValue.Contains(text) || s.ServiceCodeDescription.Contains(text)) && s.ServiceCodeTableNumber.Trim().Equals(ServiceCodeTableNumber) && s.IsActive != false).Take(100).ToList();
            if (lstServiceCode.Any())
                list = MapValues(lstServiceCode);
            return list;
        }
        private List<ServiceCodeCustomModel> MapValues(List<ServiceCode> m)
        {
            var lst = new List<ServiceCodeCustomModel>();
            foreach (var model in m)
            {
                var vm = _mapper.Map<ServiceCodeCustomModel>(model);
                vm.ServiceCodeServiceCodeMainText = GetExternalValue1ById(model.ServiceCodeServiceCodeMain);
                vm.ServiceServiceCodeSubText = GetNameByGlobalCodeValue(model.ServiceServiceCodeSub, model.ServiceCodeServiceCodeMain);

                lst.Add(vm);
            }
            return lst;
        }
        private string GetExternalValue1ById(string categoryValue)
        {
            if (!string.IsNullOrEmpty(categoryValue))
            {
                    var category = _ggRepository.Where(g => g.GlobalCodeCategoryValue.Equals(categoryValue)).FirstOrDefault();
                    return category != null ? category.ExternalValue1 : string.Empty;
            }
            return string.Empty;
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
        public List<ServiceCodeCustomModel> GetFilteredServiceCodes(string text, string tableNumber)
        {
            var list = new List<ServiceCodeCustomModel>();
            var lstServiceCode = _repository.Where(
                    s =>
                    s.IsActive != false && s.IsDeleted != true
                    && (s.ServiceCodeValue.Contains(text) || s.ServiceCodeDescription.Contains(text))
                    && s.ServiceCodeTableNumber.Trim().Equals(tableNumber))
                    .Take(100)
                    .ToList();
            if (lstServiceCode.Any())
                list = MapValues(lstServiceCode);
            return list;
        }

        /// <summary>
        /// The get service code by code value.
        /// </summary>
        /// <param name="serviceCodeValue">
        /// The service code value.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceCode"/>.
        /// </returns>
        public ServiceCode GetServiceCodeByCodeValue(string serviceCodeValue, string ServiceCodeTableNumber)
        {
            var m = _repository.Where(x => x.ServiceCodeValue.Equals(serviceCodeValue) && x.ServiceCodeTableNumber.Trim().Equals(ServiceCodeTableNumber)).FirstOrDefault();
            return m ?? new ServiceCode();

        }

        /// <summary>
        /// Method to add the Service Code in the database.
        /// </summary>
        /// <param name="serviceCodeId">The service code identifier.</param>
        /// <returns>
        /// The <see cref="ServiceCode" />.
        /// </returns>
        public ServiceCode GetServiceCodeById(int serviceCodeId)
        {
            var serviceCode = _repository.Where(x => x.ServiceCodeId == serviceCodeId).FirstOrDefault();
            return serviceCode;
        }

        /// <summary>
        /// The get service code description.
        /// </summary>
        /// <param name="codeValue">
        /// The code value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetServiceCodeDescription(string codeValue, string ServiceCodeTableNumber)
        {
            var obj = _repository.Where(s => s.ServiceCodeValue.Equals(codeValue) && s.ServiceCodeTableNumber.Trim().Equals(ServiceCodeTableNumber)).FirstOrDefault();
            return obj != null ? obj.ServiceCodeDescription : string.Empty;
        }

        /// <summary>
        /// The get service code price by code value.
        /// </summary>
        /// <param name="codeValue">
        /// The code value.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetServiceCodePriceByCodeValue(string codeValue, DateTime? CodeEffectiveFrom)
        {
            var bedRateCard = _bedRepository.Where(s => s.ServiceCodeValue.Equals(codeValue) &&
                         ((CodeEffectiveFrom.HasValue && s.EffectiveFrom.Value <= CodeEffectiveFrom.Value &&
                           s.EffectiveTill.Value >= CodeEffectiveFrom.Value) || !CodeEffectiveFrom.HasValue)).FirstOrDefault();
            return bedRateCard != null ? Convert.ToInt32(bedRateCard.Rates) : 0;
        }

        /// <summary>
        ///     Get the Service Code
        /// </summary>
        /// <returns>Return the ServiceCode View Model</returns>
        public List<ServiceCode> GetServiceCodes(string ServiceCodeTableNumber)
        {
            var lstServiceCode = _repository.Where(s => (s.IsDeleted == null || !(bool)s.IsDeleted) && s.ServiceCodeTableNumber.Trim().Equals(ServiceCodeTableNumber) && s.IsActive != false).ToList();
            return lstServiceCode;
        }
        /// <summary>
        ///     Get the Service Code
        /// </summary>
        /// <returns>Return the ServiceCode View Model</returns>
        public List<ServiceCodeCustomModel> GetServiceCodesCustomList(string ServiceCodeTableNumber)
        {
            var list = new List<ServiceCodeCustomModel>();
            var lstServiceCode =
                _repository.Where(
                    s =>
                    (s.IsDeleted == null || !(bool)s.IsDeleted)
                    && s.ServiceCodeTableNumber.Trim().Equals(ServiceCodeTableNumber) && s.IsActive != false)
                    .ToList();
            if (lstServiceCode.Any())
            {
                list = MapValues(lstServiceCode);
            }
            return list;
        }
        /// <summary>
        /// Get the Service Codes List by Categories. Here Categories are referred to as ServiceCodeServiceCodeMain Column in
        ///     the table ServiceCode
        /// </summary>
        /// <param name="codeMainValue">
        /// current ServiceCodeServiceCodeMain value
        /// </param>
        /// <param name="rowCount">
        /// Row Count
        /// </param>
        /// <returns>
        /// list of service codes
        /// </returns>
        public List<ServiceCode> GetServiceCodesByCodeMainValue(string codeMainValue, int rowCount, string ServiceCodeTableNumber)
        {
            var list = rowCount > 0
                ? _repository.Where(s => s.IsActive != false && s.IsDeleted != true && s.ServiceCodeServiceCodeMain.Equals(codeMainValue) && s.ServiceCodeTableNumber.Trim().Equals(ServiceCodeTableNumber)).OrderBy(m => m.ServiceCodeValue).Take(rowCount).ToList()
                : _repository.Where(s => s.IsActive != false && s.IsDeleted != true && s.ServiceCodeServiceCodeMain.Equals(codeMainValue) && s.ServiceCodeTableNumber.Trim().Equals(ServiceCodeTableNumber)).OrderBy(m => m.ServiceCodeValue).ToList();
            return list;
        }

        /// <summary>
        /// Gets the service codes custom model.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<ServiceCodeCustomModel> GetServiceCodesCustomModel(string ServiceCodeTableNumber)
        {
            var lstServiceCode =
                _repository.Where(
                    s =>
                    (s.IsDeleted == null || !(bool)s.IsDeleted)
                    && s.ServiceCodeTableNumber.Trim().Equals(ServiceCodeTableNumber)).ToList();
            return
                lstServiceCode.Select(
                    item =>
                    new ServiceCodeCustomModel
                    {
                        ServiceCodeId = item.ServiceCodeId,
                        ServiceCodeTableNumber = item.ServiceCodeTableNumber,
                        ServiceCodeTableDescription = item.ServiceCodeTableDescription,
                        ServiceCodeValue = item.ServiceCodeValue,
                        ServiceCodeDescription = item.ServiceCodeDescription,
                        ServiceCodePrice = item.ServiceCodePrice,
                        ServiceCodeEffectiveDate = item.ServiceCodeEffectiveDate,
                        ServiceExpiryDate = item.ServiceExpiryDate,
                        ServiceCodeBasicApplicationRule =
                                item.ServiceCodeBasicApplicationRule,
                        ServiceCodeOtherApplicationRule =
                                item.ServiceCodeOtherApplicationRule,
                        ServiceCodeServiceCodeMain = item.ServiceCodeServiceCodeMain,
                        ServiceServiceCodeSub = item.ServiceServiceCodeSub,
                        CreatedBy = item.CreatedBy,
                        CreatedDate = item.CreatedDate,
                        ModifiedBy = item.ModifiedBy,
                        ModifiedDate = item.ModifiedDate,
                        IsDeleted = item.IsDeleted,
                        DeletedBy = item.DeletedBy,
                        IsActive = item.IsActive,
                        DeletedDate = item.DeletedDate,
                        CanOverRide = item.CanOverRide,
                        ServiceCodeValueDesc =
                                item.ServiceCodeDescription + " " + item.ServiceCodeValue
                    }).OrderBy(s => s.ServiceCodeDescription)
                    .ToList();
        }

        /// <summary>
        /// Gets the service codes list on demand.
        /// </summary>
        /// <param name="blockNumber">The block number.</param>
        /// <param name="blockSize">Size of the block.</param>
        /// <returns>
        /// The <see cref="List" />.
        /// </returns>
        public List<ServiceCode> GetServiceCodesListOnDemand(int blockNumber, int blockSize, string ServiceCodeTableNumber)
        {
            var startIndex = (blockNumber - 1) * blockSize;
            var lstServiceCode = _repository.Where(s => (s.IsDeleted == null || !(bool)s.IsDeleted) && s.ServiceCodeTableNumber.Trim().Equals(ServiceCodeTableNumber)).OrderByDescending(f => f.ServiceCodeId).Skip(startIndex).Take(blockSize).ToList();
            return lstServiceCode;
        }
        /// <summary>
        /// Gets the service codes list on demand.
        /// </summary>
        /// <param name="blockNumber">The block number.</param>
        /// <param name="blockSize">Size of the block.</param>
        /// <returns>
        /// The <see cref="List" />.
        /// </returns>
        public List<ServiceCodeCustomModel> GetServiceCodesListOnDemandCustom(int blockNumber, int blockSize, string ServiceCodeTableNumber)
        {
            try
            {
                var startIndex = (blockNumber - 1) * blockSize;
                var list = new List<ServiceCodeCustomModel>();
                var lstServiceCode =
                    _repository.Where(
                        s =>
                        (s.IsDeleted == null || !(bool)s.IsDeleted)
                        && s.ServiceCodeTableNumber.Trim().Equals(ServiceCodeTableNumber))
                        .OrderByDescending(f => f.ServiceCodeId)
                        .Skip(startIndex)
                        .Take(blockSize)
                        .ToList();
                if (lstServiceCode.Any())
                {
                    list = MapValues(lstServiceCode);
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Gets the service codes list.
        /// </summary>
        /// <returns></returns>
        public List<ServiceCode> GetServiceCodesList(string ServiceCodeTableNumber)
        {
            var lstServiceCode = _repository.Where(s => (s.IsDeleted == null || !(bool)s.IsDeleted) && s.ServiceCodeTableNumber.Trim().Equals(ServiceCodeTableNumber)).OrderByDescending(f => f.ServiceCodeDescription).ToList();
            return lstServiceCode;
        }

        public List<ServiceCode> GetOveridableBedList(string ServiceCodeTableNumber)
        {
            var spName = string.Format("EXEC {0} @ServiceCodeTableNumber ", StoredProcedures.SPROC_GetOverridableBeds);
            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("ServiceCodeTableNumber ", ServiceCodeTableNumber);
            IEnumerable<ServiceCode> result = _context.Database.SqlQuery<ServiceCode>(spName, sqlParameters);
            return result.ToList();
        }

        #endregion


        public List<ServiceCode> ExportServiceCodes(string text, string tableNumber)
        {
            var lstServiceCode =
                _repository.Where(
                    s =>
                    (s.IsDeleted == null || !(bool)s.IsDeleted)
                    && (s.ServiceCodeValue.Contains(text) || s.ServiceCodeDescription.Contains(text))
                    && s.ServiceCodeTableNumber.Trim().Equals(tableNumber) && s.IsActive != false)
                    .Take(100)
                    .ToList();
            return lstServiceCode;
        }


        public List<ServiceCodeCustomModel> GetServiceCodesActiveInActive(bool showInActive, string ServiceCodeTableNumber)
        {
            var list = new List<ServiceCodeCustomModel>();
            var lstServiceCode =
                _repository.Where(
                    s =>
                    (s.IsDeleted == null || !(bool)s.IsDeleted)
                    && s.ServiceCodeTableNumber.Trim().Equals(ServiceCodeTableNumber) && s.IsActive == showInActive)
                    .ToList();
            if (lstServiceCode.Any())
            {
                list = MapValues(lstServiceCode);
            }
            return list;
        }


    }
}