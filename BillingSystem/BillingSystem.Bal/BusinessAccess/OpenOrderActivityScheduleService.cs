using System;
using BillingSystem.Model;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model.CustomModel;
using BillingSystem.Common.Common;

using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class OpenOrderActivityScheduleService : IOpenOrderActivityScheduleService
    {
        private readonly IRepository<OpenOrderActivitySchedule> _repository;
        private readonly IRepository<OrderActivity> _oaRepository;
        private readonly IRepository<GlobalCodes> _gRepository;
        private readonly IRepository<OpenOrder> _oRepository;

        public OpenOrderActivityScheduleService(IRepository<OpenOrderActivitySchedule> repository, IRepository<OrderActivity> oaRepository, IRepository<GlobalCodes> gRepository, IRepository<OpenOrder> oRepository)
        {
            _repository = repository;
            _oaRepository = oaRepository;
            _gRepository = gRepository;
            _oRepository = oRepository;
        }

        /// <summary>
        /// Get the OpenOrderActivitySchedule
        /// </summary>
        /// <returns>Return the OpenOrderActivitySchedule List</returns>
        public List<OrderActivityCustomModel> GetOrderActivitiesByEncounterId(int encounterId)
        {
            try
            {
                var finalList = new List<OrderActivityCustomModel>();

                var orderslist = _oRepository.Where(e => e.EncounterID == encounterId).ToList();
                foreach (var item in orderslist)
                {
                    var result = _oaRepository.Where(o => o.OrderID != null && (int)o.OrderID == item.OpenOrderID).ToList();
                    finalList.AddRange(result.Select(i => new OrderActivityCustomModel
                    {
                        OrderActivityID = i.OrderActivityID,
                        OrderType = i.OrderType,
                        OrderCode = i.OrderCode,
                        OrderCategoryID = i.OrderCategoryID,
                        OrderSubCategoryID = i.OrderSubCategoryID,
                        OrderActivityStatus = i.OrderActivityStatus,
                        CorporateID = i.CorporateID,
                        FacilityID = i.FacilityID,
                        PatientID = i.PatientID,
                        EncounterID = i.EncounterID,
                        MedicalRecordNumber = i.MedicalRecordNumber,
                        OrderID = i.OrderID,
                        OrderBy = i.OrderBy,
                        OrderActivityQuantity = i.OrderActivityQuantity,
                        OrderScheduleDate = i.OrderScheduleDate,
                        PlannedBy = i.PlannedBy,
                        PlannedDate = i.PlannedDate,
                        PlannedFor = i.PlannedFor,
                        ExecutedBy = i.ExecutedBy,
                        ExecutedDate = i.ExecutedDate,
                        ExecutedQuantity = i.ExecutedQuantity,
                        ResultValueMin = i.ResultValueMin,
                        ResultValueMax = i.ResultValueMax,
                        ResultUOM = i.ResultUOM,
                        Comments = i.Comments,
                        IsActive = i.IsActive,
                        ModifiedBy = i.ModifiedBy,
                        ModifiedDate = i.ModifiedDate,
                        CreatedBy = i.CreatedBy,
                        CreatedDate = i.CreatedDate,
                        Status =
                            GetNameByGlobalCodeValue(Convert.ToString(i.OrderActivityStatus),
                                Convert.ToInt32(GlobalCodeCategoryValue.ActivityStatus).ToString())
                    }));
                }


                return finalList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
        /// Method to add/Update the OpenOrderActivitySchedule in the database.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddUpdateOpenOrderActivitySchedule(OpenOrderActivitySchedule model)
        {
            try
            {
                if (model.OpenOrderActivityScheduleID > 0)
                    _repository.UpdateEntity(model, model.OpenOrderActivityScheduleID);
                else
                    _repository.Create(model);
                return model.OpenOrderActivityScheduleID;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Method to add the OpenOrderActivitySchedule in the database.
        /// </summary>
        /// <param Int ID</param>
        /// <returns> OpenOrderActivitySchedule Model</returns>
        public OpenOrderActivitySchedule GetOpenOrderActivityScheduleById(int id)
        {
            var model = _repository.Where(x => x.OpenOrderActivityScheduleID == id).FirstOrDefault();
            return model;

        }
    }
}
