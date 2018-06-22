// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCodeBal.cs" company="Spadez">
//   OmniHelathcare
// </copyright>
// <owner>
// Krishan
// </owner>
// <summary>
//   Defines the ServiceCodeBal type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using BillingSystem.Bal.Mapper;

namespace BillingSystem.Bal.BusinessAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;
    using BillingSystem.Repository.GenericRepository;

    /// <summary>
    /// The service code bal.
    /// </summary>
    public class ServiceCodeBal : BaseBal
    {
        private ServiceCodeMapper ServiceCodeMapper { get; set; }
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCodeBal"/> class.
        /// </summary>
        /// <param name="tableNumber">
        /// The table number.
        /// </param>
        public ServiceCodeBal(string tableNumber)
        {
            if (!string.IsNullOrEmpty(tableNumber))
            {
                this.ServiceCodeTableNumber = tableNumber;
                ServiceCodeMapper = new ServiceCodeMapper();
            }
        }


        public ServiceCodeBal(string tableNumber, DateTime? effectiveFrom)
        {
            if (!string.IsNullOrEmpty(tableNumber))
            {
                this.ServiceCodeTableNumber = tableNumber;
                CodeEffectiveFrom = effectiveFrom;
                ServiceCodeMapper = new ServiceCodeMapper();
            }
        }

        public ServiceCodeBal()
        {
            ServiceCodeMapper = new ServiceCodeMapper();
        }
        #endregion

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
        public int AddUpdateServiceCode(ServiceCode model)
        {
            int result;
            using (var rep = this.UnitOfWork.ServiceCodeRepository)
            {
                model.ServiceCodeTableNumber = this.ServiceCodeTableNumber;
                if (model.ServiceCodeId > 0)
                {
                    var current = rep.GetSingle(model.ServiceCodeId);
                    model.CreatedBy = current.CreatedBy;
                    model.CreatedDate = current.CreatedDate;
                    model.ServiceCodeTableNumber = current.ServiceCodeTableNumber;
                    model.ServiceCodeTableDescription = current.ServiceCodeTableDescription;
                    rep.UpdateEntity(model, model.ServiceCodeId);
                }
                else
                {
                    rep.Create(model);
                }

                result = model.ServiceCodeId;

                rep.UpdateBedRateCardByPassingServiceCodes(model.ServiceCodeId);
            }

            return result;
        }

        /// <summary>
        /// The get filtered codes.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        /// The <see cref="List" />.
        /// </returns>
        public List<ServiceCode> GetFilteredCodes(string text)
        {
            using (var rep = this.UnitOfWork.ServiceCodeRepository)
            {
                var lstServiceCode =
                    rep.Where(
                        s =>
                        (s.IsDeleted == null || !(bool)s.IsDeleted)
                        && (s.ServiceCodeValue.Contains(text) || s.ServiceCodeDescription.Contains(text))
                        && s.ServiceCodeTableNumber.Trim().Equals(this.ServiceCodeTableNumber) && s.IsActive != false)
                        .Take(100)
                        .ToList();
                return lstServiceCode;
            }
        }

        /// <summary>
        /// Gets the filtered codes c model.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public List<ServiceCodeCustomModel> GetFilteredCodesCModel(string text)
        {
            using (var serviceCodeRep = this.UnitOfWork.ServiceCodeRepository)
            {
                var list = new List<ServiceCodeCustomModel>();
                var lstServiceCode =
                    serviceCodeRep.Where(
                        s =>
                        (s.IsDeleted == null || !(bool)s.IsDeleted)
                        && (s.ServiceCodeValue.Contains(text) || s.ServiceCodeDescription.Contains(text))
                        && s.ServiceCodeTableNumber.Trim().Equals(this.ServiceCodeTableNumber) && s.IsActive != false)
                        .Take(100)
                        .ToList();
                if (lstServiceCode.Any())
                {
                    list.AddRange(lstServiceCode.Select(item => ServiceCodeMapper.MapModelToViewModel(item)));
                }
                return list;
            }
        }

        public List<ServiceCodeCustomModel> GetFilteredServiceCodes(string text, string tableNumber)
        {
            using (var rep = this.UnitOfWork.ServiceCodeRepository)
            {
                var list = new List<ServiceCodeCustomModel>();
                var lstServiceCode = rep.Where(
                        s =>
                        s.IsActive != false && s.IsDeleted != true
                        && (s.ServiceCodeValue.Contains(text) || s.ServiceCodeDescription.Contains(text))
                        && s.ServiceCodeTableNumber.Trim().Equals(tableNumber))
                        .Take(100)
                        .ToList();
                if (lstServiceCode.Any())
                    list.AddRange(lstServiceCode.Select(item => ServiceCodeMapper.MapModelToViewModel(item)));
                return list;
            }
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
        public ServiceCode GetServiceCodeByCodeValue(string serviceCodeValue)
        {
            using (var serviceCodeRep = this.UnitOfWork.ServiceCodeRepository)
            {
                var serviceCode =
                    serviceCodeRep.Where(
                        x =>
                        x.ServiceCodeValue.Equals(serviceCodeValue)
                        && x.ServiceCodeTableNumber.Trim().Equals(this.ServiceCodeTableNumber)).FirstOrDefault();
                return serviceCode ?? new ServiceCode();
            }
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
            using (var serviceCodeRep = this.UnitOfWork.ServiceCodeRepository)
            {
                var serviceCode = serviceCodeRep.Where(x => x.ServiceCodeId == serviceCodeId).FirstOrDefault();
                return serviceCode;
            }
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
        public string GetServiceCodeDescription(string codeValue)
        {
            using (var rep = this.UnitOfWork.ServiceCodeRepository)
            {
                var obj =
                    rep.Where(
                        s =>
                        s.ServiceCodeValue.Equals(codeValue)
                        && s.ServiceCodeTableNumber.Trim().Equals(this.ServiceCodeTableNumber)).FirstOrDefault();
                return obj != null ? obj.ServiceCodeDescription : string.Empty;
            }
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
        public int GetServiceCodePriceByCodeValue(string codeValue)
        {
            //using (var rep = UnitOfWork.ServiceCodeRepository)
            //{
            //    var obj =
            //        rep.Where(
            //            s =>
            //                s.ServiceCodeValue.Equals(codeValue)
            //                && s.ServiceCodeTableNumber.Trim().Equals(this.ServiceCodeTableNumber) &&
            //                ((CodeEffectiveFrom.HasValue && s.ServiceCodeEffectiveDate <= CodeEffectiveFrom.Value &&
            //                  s.ServiceExpiryDate >= CodeEffectiveFrom.Value) || !CodeEffectiveFrom.HasValue))
            //            .FirstOrDefault();


            //    return obj != null ? Convert.ToInt32(obj.ServiceCodePrice) : 0;
            //}

            using (var brrep = UnitOfWork.BedRateCardRepository)
            {
                var bedRateCard = brrep.Where(s => s.ServiceCodeValue.Equals(codeValue) &&
                             ((CodeEffectiveFrom.HasValue && s.EffectiveFrom.Value <= CodeEffectiveFrom.Value &&
                               s.EffectiveTill.Value >= CodeEffectiveFrom.Value) || !CodeEffectiveFrom.HasValue)).FirstOrDefault();
                return bedRateCard != null ? Convert.ToInt32(bedRateCard.Rates) : 0;
            }
        }

        /// <summary>
        ///     Get the Service Code
        /// </summary>
        /// <returns>Return the ServiceCode View Model</returns>
        public List<ServiceCode> GetServiceCodes()
        {
            using (var serviceCodeRep = this.UnitOfWork.ServiceCodeRepository)
            {
                var lstServiceCode =
                    serviceCodeRep.Where(
                        s =>
                        (s.IsDeleted == null || !(bool)s.IsDeleted)
                        && s.ServiceCodeTableNumber.Trim().Equals(this.ServiceCodeTableNumber) && s.IsActive != false)
                        .ToList();
                return lstServiceCode;
            }
        }
        /// <summary>
        ///     Get the Service Code
        /// </summary>
        /// <returns>Return the ServiceCode View Model</returns>
        public List<ServiceCodeCustomModel> GetServiceCodesCustomList()
        {
            using (var serviceCodeRep = this.UnitOfWork.ServiceCodeRepository)
            {
                var list = new List<ServiceCodeCustomModel>();
                var lstServiceCode =
                    serviceCodeRep.Where(
                        s =>
                        (s.IsDeleted == null || !(bool)s.IsDeleted)
                        && s.ServiceCodeTableNumber.Trim().Equals(this.ServiceCodeTableNumber) && s.IsActive != false)
                        .ToList();
                if (lstServiceCode.Any())
                {
                    list.AddRange(
                        lstServiceCode.Select(item => ServiceCodeMapper.MapModelToViewModel(item)));
                }
                return list;
            }
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
        public List<ServiceCode> GetServiceCodesByCodeMainValue(string codeMainValue, int rowCount)
        {
            using (var rep = this.UnitOfWork.ServiceCodeRepository)
            {
                var list = rowCount > 0
                                             ? rep.Where(
                                                 s =>
                                                 s.IsActive != false && s.IsDeleted != true
                                                 && s.ServiceCodeServiceCodeMain.Equals(codeMainValue)
                                                 && s.ServiceCodeTableNumber.Trim().Equals(this.ServiceCodeTableNumber))
                                                   .OrderBy(m => m.ServiceCodeValue)
                                                   .Take(rowCount)
                                                   .ToList()
                                             : rep.Where(
                                                 s =>
                                                 s.IsActive != false && s.IsDeleted != true
                                                 && s.ServiceCodeServiceCodeMain.Equals(codeMainValue)
                                                 && s.ServiceCodeTableNumber.Trim().Equals(this.ServiceCodeTableNumber))
                                                   .OrderBy(m => m.ServiceCodeValue)
                                                   .ToList();
                return list;
            }
        }

        /// <summary>
        /// Gets the service codes custom model.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<ServiceCodeCustomModel> GetServiceCodesCustomModel()
        {
            using (var serviceCodeRep = this.UnitOfWork.ServiceCodeRepository)
            {
                var lstServiceCode =
                    serviceCodeRep.Where(
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
        }

        /// <summary>
        /// Gets the service codes list on demand.
        /// </summary>
        /// <param name="blockNumber">The block number.</param>
        /// <param name="blockSize">Size of the block.</param>
        /// <returns>
        /// The <see cref="List" />.
        /// </returns>
        public List<ServiceCode> GetServiceCodesListOnDemand(int blockNumber, int blockSize)
        {
            try
            {
                var startIndex = (blockNumber - 1) * blockSize;
                using (var serviceCodeRep = this.UnitOfWork.ServiceCodeRepository)
                {
                    var lstServiceCode =
                        serviceCodeRep.Where(
                            s =>
                            (s.IsDeleted == null || !(bool)s.IsDeleted)
                            && s.ServiceCodeTableNumber.Trim().Equals(this.ServiceCodeTableNumber))
                            .OrderByDescending(f => f.ServiceCodeId)
                            .Skip(startIndex)
                            .Take(blockSize)
                            .ToList();
                    return lstServiceCode;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Gets the service codes list on demand.
        /// </summary>
        /// <param name="blockNumber">The block number.</param>
        /// <param name="blockSize">Size of the block.</param>
        /// <returns>
        /// The <see cref="List" />.
        /// </returns>
        public List<ServiceCodeCustomModel> GetServiceCodesListOnDemandCustom(int blockNumber, int blockSize)
        {
            try
            {
                var startIndex = (blockNumber - 1) * blockSize;
                using (var serviceCodeRep = this.UnitOfWork.ServiceCodeRepository)
                {
                    var list = new List<ServiceCodeCustomModel>();
                    var lstServiceCode =
                        serviceCodeRep.Where(
                            s =>
                            (s.IsDeleted == null || !(bool)s.IsDeleted)
                            && s.ServiceCodeTableNumber.Trim().Equals(this.ServiceCodeTableNumber))
                            .OrderByDescending(f => f.ServiceCodeId)
                            .Skip(startIndex)
                            .Take(blockSize)
                            .ToList();
                    if (lstServiceCode.Any())
                    {
                        list.AddRange(
                            lstServiceCode.Select(item => ServiceCodeMapper.MapModelToViewModel(item)));
                    }
                    return list;
                }
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
        public List<ServiceCode> GetServiceCodesList()
        {
            try
            {
                using (var serviceCodeRep = this.UnitOfWork.ServiceCodeRepository)
                {
                    var lstServiceCode =
                        serviceCodeRep.Where(
                            s =>
                            (s.IsDeleted == null || !(bool)s.IsDeleted)
                            && s.ServiceCodeTableNumber.Trim().Equals(this.ServiceCodeTableNumber))
                            .OrderByDescending(f => f.ServiceCodeDescription)
                            .ToList();
                    return lstServiceCode;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ServiceCode> GetOveridableBedList()
        {
            List<ServiceCode> list;
            using (var sRep = UnitOfWork.ServiceCodeRepository)
            {
                list = sRep.GetOverrideableBeds(ServiceCodeTableNumber);
            }
            return list;
        }

        #endregion


        public List<ServiceCode> ExportServiceCodes(string text, string tableNumber)
        {
            using (var rep = this.UnitOfWork.ServiceCodeRepository)
            {
                var lstServiceCode =
                    rep.Where(
                        s =>
                        (s.IsDeleted == null || !(bool)s.IsDeleted)
                        && (s.ServiceCodeValue.Contains(text) || s.ServiceCodeDescription.Contains(text))
                        && s.ServiceCodeTableNumber.Trim().Equals(tableNumber) && s.IsActive != false)
                        .Take(100)
                        .ToList();
                return lstServiceCode;
            }
        }


        public List<ServiceCodeCustomModel> GetServiceCodesActiveInActive(bool showInActive)
        {
            using (var serviceCodeRep = this.UnitOfWork.ServiceCodeRepository)
            {
                var list = new List<ServiceCodeCustomModel>();
                var lstServiceCode =
                    serviceCodeRep.Where(
                        s =>
                        (s.IsDeleted == null || !(bool)s.IsDeleted)
                        && s.ServiceCodeTableNumber.Trim().Equals(this.ServiceCodeTableNumber) && s.IsActive == showInActive)
                        .ToList();
                if (lstServiceCode.Any())
                {
                    list.AddRange(
                        lstServiceCode.Select(item => ServiceCodeMapper.MapModelToViewModel(item)));
                }
                return list;
            }
        }


    }
}