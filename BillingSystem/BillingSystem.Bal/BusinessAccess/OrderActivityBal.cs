// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrderActivityBal.cs" company="SPadez">
//   Omnihealthcare
// </copyright>
// <summary>
//   The order activity bal.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Bal.BusinessAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;

    using BillingSystem.Bal.Mapper;
    using BillingSystem.Common.Common;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;
    using BillingSystem.Repository.GenericRepository;

    /// <summary>
    /// The order activity bal.
    /// </summary>
    public class OrderActivityBal : BaseBal
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderActivityBal"/> class.
        /// </summary>
        public OrderActivityBal()
        {
            this.OrderActivityMapper = new OrderActivityMapper();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderActivityBal"/> class.
        /// </summary>
        /// <param name="cptTableNumber">
        /// The cpt table number.
        /// </param>
        /// <param name="serviceCodeTableNumber">
        /// The service code table number.
        /// </param>
        /// <param name="drgTableNumber">
        /// The drg table number.
        /// </param>
        /// <param name="drugTableNumber">
        /// The drug table number.
        /// </param>
        /// <param name="hcpcsTableNumber">
        /// The hcpcs table number.
        /// </param>
        /// <param name="diagnosisTableNumber">
        /// The diagnosis table number.
        /// </param>
        public OrderActivityBal(
            string cptTableNumber,
            string serviceCodeTableNumber,
            string drgTableNumber,
            string drugTableNumber,
            string hcpcsTableNumber,
            string diagnosisTableNumber)
        {
            this.OrderActivityMapper = new OrderActivityMapper();
            if (!string.IsNullOrEmpty(cptTableNumber))
            {
                this.CptTableNumber = cptTableNumber;
            }

            if (!string.IsNullOrEmpty(serviceCodeTableNumber))
            {
                this.ServiceCodeTableNumber = serviceCodeTableNumber;
            }

            if (!string.IsNullOrEmpty(drgTableNumber))
            {
                this.DrgTableNumber = drgTableNumber;
            }

            if (!string.IsNullOrEmpty(drugTableNumber))
            {
                this.DrugTableNumber = drugTableNumber;
            }

            if (!string.IsNullOrEmpty(hcpcsTableNumber))
            {
                this.HcpcsTableNumber = hcpcsTableNumber;
            }

            if (!string.IsNullOrEmpty(diagnosisTableNumber))
            {
                this.DiagnosisTableNumber = diagnosisTableNumber;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the order activity mapper.
        /// </summary>
        private OrderActivityMapper OrderActivityMapper { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Get Entity Name By Id
        /// </summary>
        /// <param name="orderActivity">
        /// The order activity.
        /// </param>
        /// <returns>
        /// Return the Entity Respository
        /// </returns>
        public int AddUptdateOrderActivity(OrderActivity orderActivity)
        {
            using (OrderActivityRepository orderActivityRep = this.UnitOfWork.OrderActivityRepository)
            {
                if (orderActivity.OrderActivityID > 0)
                {
                    orderActivityRep.UpdateEntity(orderActivity, orderActivity.OrderActivityID);
                }
                else
                {
                    orderActivityRep.Create(orderActivity);
                }

                return orderActivity.OrderActivityID;
            }
        }

        /// <summary>
        /// Adds the uptdate order activity.
        /// </summary>
        /// <param name="orderActivity">
        /// The order activity.
        /// </param>
        /// <returns>
        /// The <see cref="int[]"/>.
        /// </returns>
        public int[] AddUptdateOrderActivity(List<OrderActivity> orderActivity)
        {
            try
            {
                var result = new int[orderActivity.Count()];
                using (var transScope = new TransactionScope())
                {
                    using (OrderActivityRepository orderActivityRep = this.UnitOfWork.OrderActivityRepository)
                    {
                        for (int index = 0; index < orderActivity.Count(); index++)
                        {
                            OrderActivity openOrderactivity = orderActivity[index];
                            orderActivityRep.Create(openOrderactivity);
                            result[index] = Convert.ToInt32(openOrderactivity.OrderActivityID);
                        }

                        transScope.Complete();
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Applies the order activity to bill.
        /// </summary>
        /// <param name="corporateId">
        /// The corporate identifier.
        /// </param>
        /// <param name="facilityId">
        /// The facility identifier.
        /// </param>
        /// <param name="encounterId">
        /// The encounter identifier.
        /// </param>
        /// <param name="reclaimFlag">
        /// if set to <c>true</c> [reclaim flag].
        /// </param>
        /// <param name="claimId">
        /// The claim Id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ApplyOrderActivityToBill(
            int corporateId,
            int facilityId,
            int encounterId,
            string reclaimFlag,
            long claimId)
        {
            using (OrderActivityRepository rep = this.UnitOfWork.OrderActivityRepository)
            {
                bool result = rep.ApplyOrderActivityToBill(encounterId, corporateId, facilityId, reclaimFlag, claimId);
                return result;
            }
        }

        /// <summary>
        /// Calculates the type of the lab result.
        /// </summary>
        /// <param name="ordercode">
        /// The ordercode.
        /// </param>
        /// <param name="resultminvalue">
        /// The resultminvalue.
        /// </param>
        /// <param name="patientId">
        /// The patient identifier.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string CalculateLabResultType(string ordercode, decimal? resultminvalue, int? patientId)
        {
            try
            {
                using (OrderActivityRepository orderActivityRep = this.UnitOfWork.OrderActivityRepository)
                {
                    List<LabOrderActivityResultStatus> lstOrderActivity =
                        orderActivityRep.GetLabResultStatusString(
                            Convert.ToInt32(ordercode.Trim()),
                            Convert.ToDecimal(resultminvalue),
                            Convert.ToInt32(patientId));
                    LabOrderActivityResultStatus labresultStatus = lstOrderActivity.FirstOrDefault();
                    return labresultStatus != null
                               ? !string.IsNullOrEmpty(labresultStatus.LabTestStatusFlag)
                                     ? labresultStatus.LabTestStatusFlag
                                     : string.Empty
                               : string.Empty;
                }
            }
            catch (Exception)
            {
            }

            return string.Empty;
        }

        /// <summary>
        /// Calculates the type of the lab result uom.
        /// </summary>
        /// <param name="ordercode">
        /// The ordercode.
        /// </param>
        /// <param name="resultminvalue">
        /// The resultminvalue.
        /// </param>
        /// <param name="patientId">
        /// The patient identifier.
        /// </param>
        /// <returns>
        /// The <see cref="int?"/>.
        /// </returns>
        public int? CalculateLabResultUOMType(string ordercode, decimal? resultminvalue, int? patientId)
        {
            try
            {
                using (OrderActivityRepository orderActivityRep = this.UnitOfWork.OrderActivityRepository)
                {
                    List<LabOrderActivityResultStatus> lstOrderActivity =
                        orderActivityRep.GetLabResultStatusString(
                            Convert.ToInt32(ordercode.Trim()),
                            Convert.ToDecimal(resultminvalue),
                            Convert.ToInt32(patientId));
                    LabOrderActivityResultStatus labresultStatus = lstOrderActivity.FirstOrDefault();
                    return labresultStatus != null
                               ? !string.IsNullOrEmpty(labresultStatus.LabUOM)
                                     ? Convert.ToInt32(labresultStatus.LabUOM)
                                     : 0
                               : 0;
                }
            }
            catch (Exception)
            {
            }

            return 0;
        }

        /// <summary>
        /// Gets the order activities by encounter identifier.
        /// </summary>
        /// <param name="userId">
        /// </param>
        /// <param name="status">
        /// </param>
        /// <param name="orderCategoryId">
        /// </param>
        /// <param name="isActiveEncountersOnly">
        /// </param>
        /// <param name="encounterId">
        /// The encounter Id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<OrderActivityCustomModel> GetLabOrderActivitiesByPhysician(
            int userId,
            int status,
            string orderCategoryId,
            int isActiveEncountersOnly,
            int encounterId)
        {
            using (OrderActivityRepository rep = this.UnitOfWork.OrderActivityRepository)
            {
                List<OrderActivityCustomModel> list = rep.GetLabOrderActivitiesByPhysician(
                    userId,
                    status,
                    orderCategoryId,
                    isActiveEncountersOnly,
                    encounterId);
                return list;
            }
        }

        /// <summary>
        /// Gets the lab order activities by activity identifier.
        /// </summary>
        /// <param name="activityId">
        /// The activity identifier.
        /// </param>
        /// <returns>
        /// The <see cref="OrderActivityCustomModel"/>.
        /// </returns>
        public OrderActivityCustomModel GetLabOrderActivityByActivityId(int activityId)
        {
            try
            {
                var orderactivityObj = new List<OrderActivityCustomModel>();
                using (OrderActivityRepository orderActivityRep = this.UnitOfWork.OrderActivityRepository)
                {
                    List<OrderActivity> lstOrderActivity =
                        orderActivityRep.Where(
                            a => a.IsActive == null || (bool)a.IsActive && a.OrderActivityID == activityId).ToList();
                    orderactivityObj.AddRange(
                        lstOrderActivity.Select(
                            item =>
                            new OrderActivityCustomModel
                            {
                                OrderActivityID = item.OrderActivityID,
                                OrderType = item.OrderType,
                                OrderCode = item.OrderCode,
                                OrderCategoryID = item.OrderCategoryID,
                                OrderSubCategoryID = item.OrderSubCategoryID,
                                OrderActivityStatus = item.OrderActivityStatus,
                                CorporateID = item.CorporateID,
                                FacilityID = item.FacilityID,
                                PatientID = item.PatientID,
                                EncounterID = item.EncounterID,
                                MedicalRecordNumber = item.MedicalRecordNumber,
                                OrderID = item.OrderID,
                                OrderBy = item.OrderBy,
                                OrderActivityQuantity = item.OrderActivityQuantity,
                                OrderScheduleDate = item.OrderScheduleDate,
                                PlannedBy = item.PlannedBy,
                                PlannedDate = item.PlannedDate,
                                PlannedFor = item.PlannedFor,
                                ExecutedBy = item.ExecutedBy,
                                ExecutedDate = item.ExecutedDate,
                                ExecutedQuantity = item.ExecutedQuantity,
                                ResultValueMin = item.ResultValueMin,
                                ResultValueMax =
                                        item.ResultValueMax
                                        ?? this.CalculateLabResultUOMType(
                                            item.OrderCode,
                                            item.ResultValueMin,
                                            item.PatientID),
                                ResultUOM =
                                        item.ResultUOM
                                        ?? this.CalculateLabResultUOMType(
                                            item.OrderCode,
                                            item.ResultValueMin,
                                            item.PatientID),
                                Comments = item.Comments,
                                IsActive = item.IsActive,
                                ModifiedBy = item.ModifiedBy,
                                ModifiedDate = item.ModifiedDate,
                                CreatedBy = item.CreatedBy,
                                CreatedDate = item.CreatedDate,
                                CategoryName =
                                        this.GetGlobalCategoryNameById(
                                            Convert.ToString(item.OrderCategoryID)),
                                SubCategoryName =
                                        this.GetNameByGlobalCodeId(
                                            Convert.ToInt32(item.OrderSubCategoryID)),
                                OrderDescription =
                                        this.GetCodeDescription(
                                            item.OrderCode,
                                            item.OrderType.ToString()),
                                Status =
                                        this.GetNameByGlobalCodeValue(
                                            Convert.ToString(item.OrderActivityStatus),
                                            Convert.ToInt32(
                                                GlobalCodeCategoryValue.ActivityStatus)
                                        .ToString()),
                                OrderTypeName =
                                        this.GetNameByGlobalCodeValue(
                                            item.OrderType.ToString(),
                                            Convert.ToInt32(GlobalCodeCategoryValue.CodeTypes)
                                        .ToString()),
                                ShowEditAction =
                                        DateTime.Compare(
                                            this.GetInvariantCultureDateTime(
                                                Convert.ToInt32(item.FacilityID)),
                                            Convert.ToDateTime(item.OrderScheduleDate)) > 0,
                                ResultUOMStr =
                                        item.ResultUOM != null
                                            ? this.GetNameByGlobalCodeValue(
                                                Convert.ToString(item.ResultUOM),
                                                Convert.ToInt32(
                                                    GlobalCodeCategoryValue.LabMeasurementValue)
                                                  .ToString())
                                            : string.Empty,
                                LabResultTypeStr =
                                        (item.ResultValueMin != null
                                         && Convert.ToInt32(item.OrderCategoryID)
                                         == Convert.ToInt32(
                                             OrderTypeCategory.PathologyandLaboratory))
                                            ? this.CalculateLabResultType(
                                                item.OrderCode,
                                                item.ResultValueMin,
                                                item.PatientID)
                                            : string.Empty,
                                SpecimenTypeStr =
                                        this.CalculateLabResultSpecimanType(
                                            item.OrderCode,
                                            item.ResultValueMin,
                                            item.PatientID),
                                ShowSpecimanEditAction =
                                        Convert.ToString(item.OrderActivityStatus) == "0"
                                        || Convert.ToString(item.OrderActivityStatus) == "20",

                                // LabResultStatusStr = (item.ResultValueMin != null && Convert.ToInt32(item.OrderCategoryID) == Convert.ToInt32(OrderTypeCategory.PathologyandLaboratory))?
                            }));
                    return orderactivityObj.FirstOrDefault() ?? null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the order activities by encounter identifier.
        /// </summary>
        /// <param name="encounterId">
        /// The encounter identifier.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<OrderActivityCustomModel> GetOrderActivitiesByEncounterId(int encounterId)
        {
            try
            {
                var orderactivityObj = new List<OrderActivityCustomModel>();
                using (OrderActivityRepository orderActivityRep = this.UnitOfWork.OrderActivityRepository)
                {
                    List<OrderActivity> lstOrderActivity =
                        orderActivityRep.Where(
                            a => a.IsActive == null || (bool)a.IsActive && a.EncounterID == encounterId).ToList();
                    orderactivityObj.AddRange(
                        lstOrderActivity.Select(
                            item =>
                            new OrderActivityCustomModel
                            {
                                OrderActivityID = item.OrderActivityID,
                                OrderType = item.OrderType,
                                OrderCode = item.OrderCode,
                                OrderCategoryID = item.OrderCategoryID,
                                OrderSubCategoryID = item.OrderSubCategoryID,
                                OrderActivityStatus = item.OrderActivityStatus,
                                CorporateID = item.CorporateID,
                                FacilityID = item.FacilityID,
                                PatientID = item.PatientID,
                                EncounterID = item.EncounterID,
                                MedicalRecordNumber = item.MedicalRecordNumber,
                                OrderID = item.OrderID,
                                OrderBy = item.OrderBy,
                                OrderActivityQuantity = item.OrderActivityQuantity,
                                OrderScheduleDate = item.OrderScheduleDate,
                                PlannedBy = item.PlannedBy,
                                PlannedDate = item.PlannedDate,
                                PlannedFor = item.PlannedFor,
                                ExecutedBy = item.ExecutedBy,
                                ExecutedDate = item.ExecutedDate,
                                ExecutedQuantity = item.ExecutedQuantity,
                                ResultValueMin = item.ResultValueMin,
                                ResultValueMax = item.ResultValueMax,
                                ResultUOM =
                                        item.ResultUOM
                                        ?? this.CalculateLabResultUOMType(
                                            item.OrderCode,
                                            item.ResultValueMin,
                                            item.PatientID),
                                Comments = item.Comments,
                                IsActive = item.IsActive,
                                ModifiedBy = item.ModifiedBy,
                                ModifiedDate = item.ModifiedDate,
                                CreatedBy = item.CreatedBy,
                                CreatedDate = item.CreatedDate,
                                CategoryName =
                                        this.GetGlobalCategoryNameById(
                                            Convert.ToString(item.OrderCategoryID)),
                                SubCategoryName =
                                        this.GetNameByGlobalCodeId(
                                            Convert.ToInt32(item.OrderSubCategoryID)),
                                OrderDescription =
                                        this.GetCodeDescription(
                                            item.OrderCode,
                                            item.OrderType.ToString()),
                                Status =
                                        this.GetNameByGlobalCodeValue(
                                            Convert.ToString(item.OrderActivityStatus),
                                            Convert.ToInt32(
                                                GlobalCodeCategoryValue.ActivityStatus)
                                        .ToString()),
                                OrderTypeName =
                                        this.GetNameByGlobalCodeValue(
                                            item.OrderType.ToString(),
                                            Convert.ToInt32(GlobalCodeCategoryValue.CodeTypes)
                                        .ToString()),
                                ShowEditAction =
                                        DateTime.Compare(
                                           GetInvariantCultureDateTime(Convert.ToInt32(item.FacilityID)),
                                            Convert.ToDateTime(item.OrderScheduleDate)) > 0,
                                ResultUOMStr =
                                        item.ResultUOM != null
                                            ? this.GetNameByGlobalCodeValue(
                                                Convert.ToString(item.ResultUOM),
                                                Convert.ToInt32(
                                                    GlobalCodeCategoryValue.LabMeasurementValue)
                                                  .ToString())
                                            : string.Empty,
                                LabResultTypeStr =
                                        (item.ResultValueMin != null
                                         && Convert.ToInt32(item.OrderCategoryID)
                                         == Convert.ToInt32(
                                             OrderTypeCategory.PathologyandLaboratory))
                                            ? this.CalculateLabResultType(
                                                item.OrderCode,
                                                item.ResultValueMin,
                                                item.PatientID)
                                            : string.Empty,
                                SpecimenTypeStr =
                                        this.CalculateLabResultSpecimanType(
                                            item.OrderCode,
                                            item.ResultValueMin,
                                            item.PatientID),
                                ShowSpecimanEditAction =
                                        Convert.ToString(item.OrderActivityStatus) == "0"
                                        || Convert.ToString(item.OrderActivityStatus) == "1"
                                        || Convert.ToString(item.OrderActivityStatus) == "20",
                                BarCodeValue = item.BarCodeValue
                                // LabResultStatusStr = (item.ResultValueMin != null && Convert.ToInt32(item.OrderCategoryID) == Convert.ToInt32(OrderTypeCategory.PathologyandLaboratory))?
                            }));
                    return orderactivityObj.OrderBy(x => x.OrderScheduleDate).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the order activities by order identifier.
        /// </summary>
        /// <param name="ordersId">
        /// The orders identifier.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<OrderActivity> GetOrderActivitiesByOrderId(int ordersId)
        {
            try
            {
                using (OrderActivityRepository orderActivityRep = this.UnitOfWork.OrderActivityRepository)
                {
                    List<OrderActivity> lstOrderActivity =
                        orderActivityRep.Where(a => a.IsActive == null || (bool)a.IsActive && a.OrderID == ordersId)
                            .ToList();
                    return lstOrderActivity;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the order activities by patient identifier.
        /// </summary>
        /// <param name="patientId">
        /// The patient identifier.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<OrderActivityCustomModel> GetOrderActivitiesByPatientId(int? patientId)
        {
            try
            {
                var orderactivityObj = new List<OrderActivityCustomModel>();
                using (OrderActivityRepository orderActivityRep = this.UnitOfWork.OrderActivityRepository)
                {
                    List<OrderActivity> lstOrderActivity =
                        orderActivityRep.Where(a => a.IsActive == null || (bool)a.IsActive && a.PatientID == patientId)
                            .ToList();
                    orderactivityObj.AddRange(
                        lstOrderActivity.Select(
                            item =>
                            new OrderActivityCustomModel
                            {
                                OrderActivityID = item.OrderActivityID,
                                OrderType = item.OrderType,
                                OrderCode = item.OrderCode,
                                OrderCategoryID = item.OrderCategoryID,
                                OrderSubCategoryID = item.OrderSubCategoryID,
                                OrderActivityStatus = item.OrderActivityStatus,
                                CorporateID = item.CorporateID,
                                FacilityID = item.FacilityID,
                                PatientID = item.PatientID,
                                EncounterID = item.EncounterID,
                                MedicalRecordNumber = item.MedicalRecordNumber,
                                OrderID = item.OrderID,
                                OrderBy = item.OrderBy,
                                OrderActivityQuantity = item.OrderActivityQuantity,
                                OrderScheduleDate = item.OrderScheduleDate,
                                PlannedBy = item.PlannedBy,
                                PlannedDate = item.PlannedDate,
                                PlannedFor = item.PlannedFor,
                                ExecutedBy = item.ExecutedBy,
                                ExecutedDate = item.ExecutedDate,
                                ExecutedQuantity = item.ExecutedQuantity,
                                ResultValueMin = item.ResultValueMin,
                                ResultValueMax = item.ResultValueMax,
                                ResultUOM =
                                        item.ResultUOM
                                        ?? this.CalculateLabResultUOMType(
                                            item.OrderCode,
                                            item.ResultValueMin,
                                            item.PatientID),
                                Comments = item.Comments,
                                IsActive = item.IsActive,
                                ModifiedBy = item.ModifiedBy,
                                ModifiedDate = item.ModifiedDate,
                                CreatedBy = item.CreatedBy,
                                CreatedDate = item.CreatedDate,
                                CategoryName =
                                        this.GetGlobalCategoryNameById(
                                            Convert.ToString(item.OrderCategoryID)),
                                SubCategoryName =
                                        this.GetNameByGlobalCodeId(
                                            Convert.ToInt32(item.OrderSubCategoryID)),
                                OrderDescription =
                                        this.GetCodeDescription(
                                            item.OrderCode,
                                            item.OrderType.ToString()),
                                Status =
                                        this.GetNameByGlobalCodeValue(
                                            Convert.ToString(item.OrderActivityStatus),
                                            Convert.ToInt32(
                                                GlobalCodeCategoryValue.ActivityStatus)
                                        .ToString()),
                                OrderTypeName =
                                        this.GetNameByGlobalCodeValue(
                                            item.OrderType.ToString(),
                                            Convert.ToInt32(GlobalCodeCategoryValue.CodeTypes)
                                        .ToString()),
                                ShowEditAction =
                                        DateTime.Compare(
                                            this.GetInvariantCultureDateTime(
                                                Convert.ToInt32(item.FacilityID)),
                                            Convert.ToDateTime(item.OrderScheduleDate)) > 0,
                                ResultUOMStr =
                                        item.ResultUOM != null
                                            ? this.GetNameByGlobalCodeValue(
                                                Convert.ToString(item.ResultUOM),
                                                Convert.ToInt32(
                                                    GlobalCodeCategoryValue.LabMeasurementValue)
                                                  .ToString())
                                            : string.Empty,
                                LabResultTypeStr =
                                        (item.ResultValueMin != null
                                         && Convert.ToInt32(item.OrderCategoryID)
                                         == Convert.ToInt32(
                                             OrderTypeCategory.PathologyandLaboratory))
                                            ? this.CalculateLabResultType(
                                                item.OrderCode,
                                                item.ResultValueMin,
                                                item.PatientID)
                                            : string.Empty,
                                SpecimenTypeStr =
                                        this.CalculateLabResultSpecimanType(
                                            item.OrderCode,
                                            item.ResultValueMin,
                                            item.PatientID),
                                ShowSpecimanEditAction =
                                        Convert.ToString(item.OrderActivityStatus) == "0"
                                        || Convert.ToString(item.OrderActivityStatus) == "20",

                                // LabResultStatusStr = (item.ResultValueMin != null && Convert.ToInt32(item.OrderCategoryID) == Convert.ToInt32(OrderTypeCategory.PathologyandLaboratory))?
                            }));
                    return orderactivityObj.OrderBy(x => x.OrderScheduleDate).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///     Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<OrderActivity> GetOrderActivity()
        {
            try
            {
                using (OrderActivityRepository OrderActivityRep = this.UnitOfWork.OrderActivityRepository)
                {
                    List<OrderActivity> lstOrderActivity =
                        OrderActivityRep.Where(a => a.IsActive == null || !(bool)a.IsActive).ToList();
                    return lstOrderActivity;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="OrderActivityId">
        /// The order activity identifier.
        /// </param>
        /// <returns>
        /// The <see cref="OrderActivity"/>.
        /// </returns>
        public OrderActivity GetOrderActivityByID(int? OrderActivityId)
        {
            using (OrderActivityRepository orderActivityRep = this.UnitOfWork.OrderActivityRepository)
            {
                OrderActivity orderActivity =
                    orderActivityRep.Where(x => x.OrderActivityID == OrderActivityId).FirstOrDefault();
                return orderActivity;
            }
        }
        public OrderActivityCustomModel GetOrderActivityByIDVM(int orderActivityId)
        {
            using (OrderActivityRepository orderActivityRep = this.UnitOfWork.OrderActivityRepository)
            {
                //    OrderActivity orderActivity =
                //        orderActivityRep.Where(x => x.OrderActivityID == OrderActivityId).FirstOrDefault();
                //    //var list = new OrderActivityCustomModel();
                //   var list = this.OrderActivityMapper.MapModelToViewModel(orderActivity);
                var list = orderActivityRep.GetOrderActivityById(orderActivityId);
                return list;
            }
        }

        /// <summary>
        ///     Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<OrderActivityCustomModel> GetOrderActivityCustom()
        {
            try
            {
                var orderactivityObj = new List<OrderActivityCustomModel>();
                using (OrderActivityRepository orderActivityRep = this.UnitOfWork.OrderActivityRepository)
                {
                    List<OrderActivity> lstOrderActivity =
                        orderActivityRep.Where(a => a.IsActive == null || !(bool)a.IsActive).ToList();
                    orderactivityObj.AddRange(
                        lstOrderActivity.Select(
                            item =>
                                new OrderActivityCustomModel
                                {
                                    OrderActivityID = item.OrderActivityID,
                                    OrderType = item.OrderType,
                                    OrderCode = item.OrderCode,
                                    OrderCategoryID = item.OrderCategoryID,
                                    OrderSubCategoryID = item.OrderSubCategoryID,
                                    OrderActivityStatus = item.OrderActivityStatus,
                                    CorporateID = item.CorporateID,
                                    FacilityID = item.FacilityID,
                                    PatientID = item.PatientID,
                                    EncounterID = item.EncounterID,
                                    MedicalRecordNumber = item.MedicalRecordNumber,
                                    OrderID = item.OrderID,
                                    OrderBy = item.OrderBy,
                                    OrderActivityQuantity = item.OrderActivityQuantity,
                                    OrderScheduleDate = item.OrderScheduleDate,
                                    PlannedBy = item.PlannedBy,
                                    PlannedDate = item.PlannedDate,
                                    PlannedFor = item.PlannedFor,
                                    ExecutedBy = item.ExecutedBy,
                                    ExecutedDate = item.ExecutedDate,
                                    ExecutedQuantity = item.ExecutedQuantity,
                                    ResultValueMin = item.ResultValueMin,
                                    ResultValueMax = item.ResultValueMax,
                                    ResultUOM = item.ResultUOM,
                                    Comments = item.Comments,
                                    IsActive = item.IsActive,
                                    ModifiedBy = item.ModifiedBy,
                                    ModifiedDate = item.ModifiedDate,
                                    CreatedBy = item.CreatedBy,
                                    CreatedDate = item.CreatedDate,
                                    CategoryName =
                                        this.GetGlobalCategoryNameById(
                                            Convert.ToString(item.OrderCategoryID)),
                                    SubCategoryName =
                                        this.GetNameByGlobalCodeId(
                                            Convert.ToInt32(item.OrderSubCategoryID)),
                                    OrderDescription =
                                        this.GetCodeDescription(
                                            item.OrderCode,
                                            item.OrderType.ToString()),
                                    Status =
                                        this.GetNameByGlobalCodeValue(
                                            Convert.ToString(item.OrderActivityStatus),
                                            Convert.ToInt32(
                                                GlobalCodeCategoryValue.ActivityStatus)
                                                .ToString())
                                }));
                    return orderactivityObj;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the activities by encounter identifier.
        /// </summary>
        /// <param name="encounterId">
        /// The encounter identifier.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<OrderActivityCustomModel> GetPCActivitiesByEncounterId(int encounterId)
        {
            try
            {
                var orderactivityObj = new List<OrderActivityCustomModel>();
                using (OrderActivityRepository orderActivityRep = this.UnitOfWork.OrderActivityRepository)
                {
                    List<OrderActivityCustomModel> listFromSp =
                        orderActivityRep.GetOrderActivitiesWithPcareActvities(encounterId, 0, "1,2,3,4,9", 0, 1);
                    orderactivityObj.AddRange(listFromSp.Select(item => this.OrderActivityMapper.MapCustomModelToViewModel(item)));
                    return orderactivityObj.OrderBy(x => x.OrderScheduleDate).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the pc closed activities by encounter identifier.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public List<OrderActivityCustomModel> GetPCClosedActivitiesByEncounterId(int encounterId)
        {
            try
            {
                var orderactivityObj = new List<OrderActivityCustomModel>();
                using (OrderActivityRepository orderActivityRep = this.UnitOfWork.OrderActivityRepository)
                {
                    var listFromSp =
                        orderActivityRep.GetOrderActivitiesWithPcareActvities(encounterId, 0, "3,4,9", 0, 1);
                    orderactivityObj.AddRange(
                        listFromSp.Select(item => this.OrderActivityMapper.MapCustomModelToViewModel(item)));
                    return orderactivityObj.OrderBy(x => x.ExecutedDate).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Creates the partiallyexecuted activity.
        /// </summary>
        /// <param name="activityid">The activityid.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="actvityStatus">The actvity status.</param>
        /// <returns></returns>
        public bool CreatePartiallyexecutedActivity(int activityid, decimal quantity, string actvityStatus)
        {
            try
            {
                using (OrderActivityRepository orderActivityRep = this.UnitOfWork.OrderActivityRepository)
                {
                    return orderActivityRep.CreatePartiallyexecutedActivity(activityid, quantity, actvityStatus);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Closes the order activity.
        /// </summary>
        /// <param name="orderActivityId">The order activity identifier.</param>
        /// <returns></returns>
        public int CloseOrderActivity(int orderActivityId)
        {
            try
            {
                using (var orderActivityRep = this.UnitOfWork.OrderActivityRepository)
                {
                    var orderactvityObj = this.GetOrderActivityByID(orderActivityId);
                    orderactvityObj.OrderActivityStatus = Convert.ToInt32(OpenOrderActivityStatus.Cancel);
                    orderactvityObj.ExecutedQuantity = orderactvityObj.OrderActivityQuantity;
                    orderactvityObj.ExecutedDate = this.GetInvariantCultureDateTime(Convert.ToInt32(orderactvityObj.FacilityID));
                    orderActivityRep.UpdateEntity(orderactvityObj, orderactvityObj.OrderActivityID);

                    // New changes done BY Shashank AS of EOD 14th March 2016 
                    // Changes Start
                    var orderActivities = GetOrderActivitiesByOrderId(Convert.ToInt32(orderactvityObj.OrderID));
                    var openorderactivties =
                        orderActivities.Any(
                            x =>
                            x.OrderActivityStatus == Convert.ToInt32(OpenOrderActivityStatus.Open)
                            || x.OrderActivityStatus == 0 || x.OrderActivityStatus == 40);

                    if (!openorderactivties)
                    {
                        var changesTobeDone = new OpenOrderBal().UpdateOpenOrderStatus(
                            Convert.ToInt32(orderactvityObj.OrderID),
                            Convert.ToString((int)OrderStatus.OnBill),
                            Convert.ToInt32(orderactvityObj.ExecutedBy),
                            this.GetInvariantCultureDateTime(Convert.ToInt32(orderactvityObj.FacilityID)));
                    }
                    // Changes End
                    return orderactvityObj.EncounterID.HasValue ? orderactvityObj.EncounterID.Value : 1;
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Get MAR View detail
        /// </summary>
        /// <param name="oMarViewCustomModel"></param>
        /// <returns></returns>
        public List<MarViewCustomModel> GetMARView(MarViewCustomModel oMarViewCustomModel)
        {
            try
            {
                List<MarViewCustomModel> lstMarViewCustomModel;
                using (var orderActivityRep = this.UnitOfWork.OrderActivityRepository)
                {
                    lstMarViewCustomModel = orderActivityRep.GetMARView(oMarViewCustomModel);
                }
                return lstMarViewCustomModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the order activities by encounter identifier sp.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public List<OrderActivityCustomModel> GetOrderActivitiesByEncounterIdSP(int encounterId)
        {
            try
            {
                List<OrderActivityCustomModel> orderactivityObj = new List<OrderActivityCustomModel>();
                using (var orderActivityRep = this.UnitOfWork.OrderActivityRepository)
                    orderactivityObj = orderActivityRep.GetOrderActivitiesByEncounterIdSP(encounterId);
                return orderactivityObj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public GenerateBarCode GetBarCodeDetails(int orderActivityId)
        {
            using (var rep = this.UnitOfWork.OrderActivityRepository)
            {
                var barCode = rep.GetBarCodeDetailsByOrderActivityId(orderActivityId);
                return barCode != null && !string.IsNullOrEmpty(barCode.BarCodeReadValue)
                    ? barCode
                    : new GenerateBarCode();
            }
        }
        #endregion

        /// <summary>
        /// Pharamacies the order activity administered.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns></returns>
        public bool PharamacyOrderActivityAdministered(int orderId)
        {
            try
            {
                List<OrderActivity> orderactivityObj;
                using (var orderActivityRep = this.UnitOfWork.OrderActivityRepository)
                {
                    orderactivityObj =
                        orderActivityRep.Where(
                            x => x.OrderID == orderId && x.OrderType == 5 && x.OrderActivityStatus == 4).ToList();
                }

                return orderactivityObj.Any();
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}