using System;
using BillingSystem.Model;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model.CustomModel;
using BillingSystem.Common.Common;

namespace BillingSystem.Bal.BusinessAccess
{
    public class OpenOrderActivityScheduleBal : BaseBal
    {
        /// <summary>
        /// Get the OpenOrderActivitySchedule
        /// </summary>
        /// <returns>Return the OpenOrderActivitySchedule List</returns>
        public List<OrderActivityCustomModel> GetOrderActivitiesByEncounterId(int encounterId)
        {
            try
            {
                var finalList = new List<OrderActivityCustomModel>();
                using (var rep = UnitOfWork.OpenOrderRepository)
                {
                    var orderslist = rep.Where(e => e.EncounterID == encounterId).ToList();
                    foreach (var item in orderslist)
                    {
                        using (var rep1 = UnitOfWork.OrderActivityRepository)
                        {
                            var result = rep1.Where(o => o.OrderID != null && (int)o.OrderID == item.OpenOrderID).ToList();
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
                    }
                }
                return finalList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
                using (var rep = UnitOfWork.OpenOrderActivityScheduleRepository)
                {
                    if (model.OpenOrderActivityScheduleID > 0)
                        rep.UpdateEntity(model, model.OpenOrderActivityScheduleID);
                    else
                        rep.Create(model);
                    return model.OpenOrderActivityScheduleID;
                }
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
            using (var rep = UnitOfWork.OpenOrderActivityScheduleRepository)
            {
                var model = rep.Where(x => x.OpenOrderActivityScheduleID == id).FirstOrDefault();
                return model;
            }
        }
    }
}
