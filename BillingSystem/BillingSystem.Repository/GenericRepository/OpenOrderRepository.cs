// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OpenOrderRepository.cs" company="">
//   
// </copyright>
// <summary>
//   The open order repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Common;

namespace BillingSystem.Repository.GenericRepository
{
    using Microsoft.Ajax.Utilities;
    using System;

    /// <summary>
    /// The open order repository.
    /// </summary>
    public class OpenOrderRepository : GenericRepository<OpenOrder>
    {
        /// <summary>
        /// The _context.
        /// </summary>
        private readonly DbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenOrderRepository"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public OpenOrderRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }

        // SPROC_GetPhysicianPreviousOrders
        /// <summary>
        /// Gets the most ordered list.
        /// </summary>
        /// <param name="physicianId">
        /// The physician identifier.
        /// </param>
        /// <param name="numberOfDays">
        /// The number of days.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<OpenOrderCustomModel> GetMostOrderedList(int physicianId, int numberOfDays)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format(
                        "EXEC {0} @pPhysicianID, @pNumberOfDaysBack",
                        StoredProcedures.SPROC_GetMostOrdered);
                    var sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("pPhysicianID", physicianId);
                    sqlParameters[1] = new SqlParameter("pNumberOfDaysBack", numberOfDays);
                    IEnumerable<OpenOrderCustomModel> result =
                        _context.Database.SqlQuery<OpenOrderCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the physician previous orders.
        /// </summary>
        /// <param name="physicianId">
        /// The physician identifier.
        /// </param>
        /// <param name="corporateId">
        /// The corporate identifier.
        /// </param>
        /// <param name="facilityId">
        /// The facility identifier.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<OpenOrderCustomModel> GetPhysicianPreviousOrders(int physicianId, int corporateId, int facilityId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format(
                        "EXEC {0} @pPhysicianID, @pCorporateID,@pFacilityID",
                        StoredProcedures.SPROC_GetPhysicianPreviousOrders);
                    var sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("pPhysicianID", physicianId);
                    sqlParameters[1] = new SqlParameter("pCorporateID", corporateId);
                    sqlParameters[2] = new SqlParameter("pFacilityID", facilityId);
                    IEnumerable<OpenOrderCustomModel> result =
                        _context.Database.SqlQuery<OpenOrderCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the physician favorite orders.
        /// </summary>
        /// <param name="physicianId">
        /// The physician identifier.
        /// </param>
        /// <param name="corporateId">
        /// The corporate identifier.
        /// </param>
        /// <param name="facilityId">
        /// The facility identifier.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<OpenOrderCustomModel> GetPhysicianFavoriteOrders(int physicianId, int corporateId, int facilityId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format(
                        "EXEC {0} @pPhysicianID, @pCorporateID,@pFacilityID",
                        StoredProcedures.SPROC_GetPhysicianFavoriteOrders);
                    var sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("pPhysicianID", physicianId);
                    sqlParameters[1] = new SqlParameter("pCorporateID", corporateId);
                    sqlParameters[2] = new SqlParameter("pFacilityID", facilityId);
                    IEnumerable<OpenOrderCustomModel> result =
                        _context.Database.SqlQuery<OpenOrderCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Checks the durg allergy.
        /// </summary>
        /// <param name="ordercode">
        /// The ordercode.
        /// </param>
        /// <param name="patientId">
        /// The patient identifier.
        /// </param>
        /// <param name="erncounterid">
        /// The erncounterid.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<DrugReactionCustomModel> CheckDurgAllergy(string ordercode, int patientId, int erncounterid)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format(
                        "EXEC {0} @pPatientId, @pOrderCode,@pEncounterId",
                        StoredProcedures.Sproc_GetPatientAllergyByPharmacyOrderCode);
                    var sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("pOrderCode", ordercode);
                    sqlParameters[1] = new SqlParameter("pPatientId", patientId);
                    sqlParameters[2] = new SqlParameter("pEncounterId", erncounterid);
                    IEnumerable<DrugReactionCustomModel> result =
                        _context.Database.SqlQuery<DrugReactionCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// The change order detail.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        public int ChangeOrderDetail(OpenOrder model)
        {
            try
            {
                if (_context != null)
                {
                    var spName =
                        string.Format(
                            "EXEC {0} @pPatientId,@pOrderCode,@pOrderType,@pEncounterId,@pOrderStatus,@pOrderFrequency,@pOrderStartDate, @pOrderEndDate,@pSchQuantity,@pPhysicianId,@pOpenOrderId,@pOrderNotes,@pOrderCategory,@pOrderSubCategory",
                            StoredProcedures.SPROC_ChangeCurrentOrderDetail);
                    var sqlParameters = new SqlParameter[14];
                    sqlParameters[0] = new SqlParameter("pPatientId", model.PatientID);
                    sqlParameters[1] = new SqlParameter("pOrderCode", model.OrderCode);
                    sqlParameters[2] = new SqlParameter("pOrderType", model.OrderType);
                    sqlParameters[3] = new SqlParameter("pEncounterId", model.EncounterID);
                    sqlParameters[4] = new SqlParameter("pOrderStatus", model.OrderStatus);
                    sqlParameters[5] = new SqlParameter("pOrderFrequency", model.FrequencyCode);
                    sqlParameters[6] = new SqlParameter("pOrderStartDate", model.StartDate);
                    sqlParameters[7] = new SqlParameter("pOrderEndDate", model.EndDate);
                    sqlParameters[8] = new SqlParameter("pSchQuantity", model.Quantity);
                    sqlParameters[9] = new SqlParameter("pPhysicianId", model.PhysicianID);
                    sqlParameters[10] = new SqlParameter("pOpenOrderId", model.OpenOrderID);
                    sqlParameters[11] = string.IsNullOrEmpty(model.OrderNotes)
                                            ? new SqlParameter("pOrderNotes", DBNull.Value)
                                            : new SqlParameter("pOrderNotes", model.OrderNotes);
                    sqlParameters[12] = new SqlParameter("pOrderCategory", model.CategoryId);
                    sqlParameters[13] = new SqlParameter("pOrderSubCategory", model.SubCategoryId);
                    ExecuteCommand(spName, sqlParameters);
                }

                return model.OpenOrderID;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get the physician order list
        /// </summary>
        /// <param name="encounterId">
        /// </param>
        /// <param name="orderStatus">
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<OpenOrderCustomModel> GetPhysicianOrdersList(int encounterId, string orderStatus, long categoryId = 0)
        {
            try
            {
                if (_context != null)
                {
                    var spName = $"EXEC {StoredProcedures.SPROC_GetPhysicianOrders} @pEncounterId, @pOrderStatus, @pCategoryId";

                    var sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("pEncounterId", encounterId);
                    sqlParameters[1] = new SqlParameter("pOrderStatus", orderStatus);
                    sqlParameters[2] = new SqlParameter("pCategoryId", categoryId);
                    IEnumerable<OpenOrderCustomModel> result = _context.Database.SqlQuery<OpenOrderCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Cancels the open order.
        /// </summary>
        /// <param name="orderid">
        /// The orderid.
        /// </param>
        /// <param name="userid">
        /// The userid.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CancelOpenOrder(int orderid, int userid)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pOrderId, @pUserId", StoredProcedures.SPROC_CancelOpenOrder);
                    var sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("pOrderId", orderid);
                    sqlParameters[1] = new SqlParameter("pUserId", userid);
                    ExecuteCommand(spName, sqlParameters);
                    return true;
                }

                return true;
            }
            catch (Exception)
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the open orders by encounter identifier.
        /// </summary>
        /// <param name="encounterId">
        /// The encounter identifier.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<OpenOrderCustomModel> GetAllOrdersByEncounterId(int encounterId)
        {
            var spName = string.Format("EXEC {0} @pEncId", StoredProcedures.SPROC_GetOrdersByEncounterId);
            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("pEncId", encounterId);
            IEnumerable<OpenOrderCustomModel> result =
                _context.Database.SqlQuery<OpenOrderCustomModel>(spName, sqlParameters);
            return result.ToList();
        }

        public List<OpenOrdersData> CustomOrdersListList(int physicianId, int numberOfDays, int fid, int cid)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format(
                        "EXEC {0} @pPhysicianID, @pNumberOfDaysBack,@fid,@cid",
                        StoredProcedures.SPROC_GetOrdersData);
                    var sqlParameters = new SqlParameter[4];
                    sqlParameters[0] = new SqlParameter("pPhysicianID", physicianId);
                    sqlParameters[1] = new SqlParameter("pNumberOfDaysBack", numberOfDays);
                    sqlParameters[2] = new SqlParameter("fid", fid);
                    sqlParameters[3] = new SqlParameter("cid", cid);
                    IEnumerable<OpenOrdersData> result =
                        _context.Database.SqlQuery<OpenOrdersData>(spName, sqlParameters);
                    return result.ToList();
                }

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// This returns all the data in go, required at Orders Tab in the EHR View.
        /// </summary>
        /// <param name="physicianId">Current Physician ID</param>
        /// <param name="mostRecentDays">Last Days from the Current Date</param>
        /// <param name="cId">Corporate ID</param>
        /// <param name="fId">Facility ID</param>
        /// <param name="encId">Encounter ID</param>
        /// <param name="gcCategoryCodes">collective value of various GC Category Codes</param>
        /// <param name="patientId">Current Patient ID</param>
        /// <param name="opCode">Conditional Value based on which the Main Stored Procedure will execute the inner queries (Optional)</param>
        /// <param name="exParam1">Extra Parameter to be passed in case the Current Proc requires any other parameter</param>
        /// <returns></returns>
        public OrderCustomModel OrdersViewData(int physicianId, int mostRecentDays, int cId, int fId, int encId, string gcCategoryCodes, int patientId, string opCode, string exParam1)
        {
            var spName = $"EXEC {StoredProcedures.SprocOrdersViewData} @PhysicianId, @MostRecentDays, @CId, @FId, @EncId, @GCCategoryCodes, @PatientId, @OpCode, @ExParam1";
            var sqlParameters = new SqlParameter[9];
            sqlParameters[0] = new SqlParameter("PhysicianId", physicianId);
            sqlParameters[1] = new SqlParameter("MostRecentDays", mostRecentDays);
            sqlParameters[2] = new SqlParameter("CId", cId);
            sqlParameters[3] = new SqlParameter("FId", fId);
            sqlParameters[4] = new SqlParameter("EncId", encId);
            sqlParameters[5] = new SqlParameter("GCCategoryCodes", gcCategoryCodes);
            sqlParameters[6] = new SqlParameter("PatientId", patientId);
            sqlParameters[7] = new SqlParameter("OpCode", opCode);
            sqlParameters[8] = new SqlParameter("ExParam1", exParam1);

            using (var r = _context.MultiResultSetSqlQuery(spName, parameters: sqlParameters))
            {
                var vm = new OrderCustomModel();
                vm.MostRecentOrders = r.ResultSetFor<OpenOrderCustomModel>().ToList();
                vm.PreviousOrders = r.ResultSetFor<OpenOrderCustomModel>().ToList();
                vm.FavoriteOrders = r.ResultSetFor<OpenOrderCustomModel>().ToList();
                vm.OpenOrders = r.ResultSetFor<OpenOrderCustomModel>().ToList();
                vm.OrderActivities = r.ResultSetFor<OrderActivityCustomModel>().ToList();
                vm.FutureOpenOrders = r.ResultSetFor<FutureOpenOrderCustomModel>().ToList();
                vm.GlobalCodes = r.ResultSetFor<GlobalCodes>().ToList();

                if (vm.PreviousOrders.Any())
                    vm.PreviousOrders = vm.PreviousOrders.DistinctBy(a => a.OrderCode).ToList();

                if (vm.OpenOrders.Any())
                    vm.OpenOrders = vm.OpenOrders.OrderByDescending(a => a.EndDate).ToList();

                return vm;
            }
        }


        /// <summary>
        /// Get the physician order list
        /// </summary>
        /// <param name="encounterId">
        /// </param>
        /// <param name="orderStatus">
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public OrderCustomModel GetPhysicianOrderPlusActivityList(int encounterId, string orderStatus = "", long categoryId = 0, bool withActivities = false)
        {
            try
            {
                if (_context != null)
                {
                    var spName = $"EXEC {StoredProcedures.SprocGetPhysicianOrdersAndActivities} @pEncounterId, @pOrderStatus, @pCategoryId, @pWithActivities";

                    var sqlParameters = new SqlParameter[4];
                    sqlParameters[0] = new SqlParameter("pEncounterId", encounterId);
                    sqlParameters[1] = new SqlParameter("pOrderStatus", orderStatus);
                    sqlParameters[2] = new SqlParameter("pCategoryId", categoryId);
                    sqlParameters[3] = new SqlParameter("pWithActivities", withActivities);

                    using (var r = _context.MultiResultSetSqlQuery(spName, parameters: sqlParameters))
                    {
                        var vm = new OrderCustomModel();
                        vm.OpenOrders = r.ResultSetFor<OpenOrderCustomModel>().ToList();

                        if (withActivities)
                            vm.OrderActivities = r.ResultSetFor<OrderActivityCustomModel>().ToList();

                        return vm;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// This function does the following: 
        ///   1. Add / updates order details
        ///   2. Schedules the order activity in case of New Order
        ///   3. Applies Order Activities to Bill in case of Administering the order activity.
        ///   4. Gets the result sets required to show after Current Encounters' Orders are updated.
        /// </summary>
        /// <param name="model">object of the Open Order</param>
        /// <param name="physicianId">Current Physician ID</param>
        /// <param name="mostRecentDays">Number of last Days in which Orders might get applied</param>
        /// <param name="cId">Current Corporate ID</param>
        /// <param name="fId">Current Facility ID</param>
        /// <param name="encId">Current Encounter ID</param>
        /// <param name="gcCategoryCodes">list of Category Codes</param>
        /// <param name="patientId">Current Patient ID</param>
        /// <param name="opCode">Critiria Value to list out the result Set that are executed after Save Orders operations done.</param>
        /// <param name="exParam1">Extra Parameter to be passed to the Database Query</param>
        /// <returns></returns>
        public OrderCustomModel SavePhysicianOrder(OpenOrder model, int physicianId, int mostRecentDays, int cId, int fId, int encId, string gcCategoryCodes, int patientId, string opCode, string exParam1 = "")
        {
            try
            {
                var spName = $"EXEC {StoredProcedures.SprocSaveOrderDetail} @PhysicianId, @MostRecentDays, @CId, @FId, @EncId, @GCCategoryCodes, @PatientId, @OpCode," +
                            " @ExParam1, @ODiagnosisCode, @OPeriodDays, @OOrderStatus, @OIsApproved, @OCategoryId, @OOpenOrderId, @OModifiedBy," +
                            " @OStartDate, @OEndDate, @OSubCategoryId, @OOrderType, @OOrderCode, @OQuantity, @OFrequencyCode, @OIsActivitySchecduled," +
                            " @OActivitySchecduledOn, @OItemName, @OItemStrength, @OItemDosage, @OIsActive, @OOrderNotes, @OEV1, @OEV2, @OEV3, @OEV4";

                var sqlParameters = new SqlParameter[34];
                sqlParameters[0] = new SqlParameter("PhysicianId", physicianId);
                sqlParameters[1] = new SqlParameter("MostRecentDays", mostRecentDays);
                sqlParameters[2] = new SqlParameter("CId", cId);
                sqlParameters[3] = new SqlParameter("FId", fId);
                sqlParameters[4] = new SqlParameter("EncId", encId);
                sqlParameters[5] = new SqlParameter("GCCategoryCodes", gcCategoryCodes);
                sqlParameters[6] = new SqlParameter("PatientId", patientId);
                sqlParameters[7] = new SqlParameter("OpCode", opCode);
                sqlParameters[8] = new SqlParameter("ExParam1", exParam1);

                sqlParameters[9] = new SqlParameter("ODiagnosisCode", model.DiagnosisCode);
                sqlParameters[10] = new SqlParameter("OPeriodDays", model.PeriodDays);
                sqlParameters[11] = new SqlParameter("OOrderStatus", model.OrderStatus);
                sqlParameters[12] = new SqlParameter("OIsApproved", !model.IsApproved.HasValue || model.IsApproved.Value);
                sqlParameters[13] = new SqlParameter("OCategoryId", model.CategoryId);
                sqlParameters[14] = new SqlParameter("OModifiedBy", model.ModifiedBy);
                sqlParameters[15] = new SqlParameter("OStartDate", model.StartDate);
                sqlParameters[16] = new SqlParameter("OEndDate", model.EndDate);
                sqlParameters[17] = new SqlParameter("OSubCategoryId", model.SubCategoryId);
                sqlParameters[18] = new SqlParameter("OOrderType", model.OrderType);
                sqlParameters[19] = new SqlParameter("OOrderCode", model.OrderCode);
                sqlParameters[20] = new SqlParameter("OQuantity", model.Quantity);
                sqlParameters[21] = new SqlParameter("OFrequencyCode", model.FrequencyCode);
                sqlParameters[22] = new SqlParameter("OIsActivitySchecduled", model.IsActivitySchecduled);
                sqlParameters[23] = model.ActivitySchecduledOn.HasValue ? new SqlParameter("OActivitySchecduledOn", model.ActivitySchecduledOn.Value) : new SqlParameter("OActivitySchecduledOn", DBNull.Value);
                sqlParameters[24] = new SqlParameter("OOpenOrderId", model.OpenOrderID);
                sqlParameters[25] = new SqlParameter("OItemName", string.IsNullOrEmpty(model.ItemName) ? string.Empty : model.ItemName);
                sqlParameters[26] = new SqlParameter("OItemStrength", string.IsNullOrEmpty(model.ItemStrength) ? string.Empty : model.ItemStrength);
                sqlParameters[27] = new SqlParameter("OItemDosage", string.IsNullOrEmpty(model.ItemDosage) ? string.Empty : model.ItemDosage);
                sqlParameters[28] = new SqlParameter("OIsActive", !model.IsActive.HasValue || model.IsActive.Value);
                sqlParameters[29] = new SqlParameter("OOrderNotes", !string.IsNullOrEmpty(model.OrderNotes) ? model.OrderNotes : string.Empty);
                sqlParameters[30] = new SqlParameter("OEV1", !string.IsNullOrEmpty(model.EV1) ? model.EV1 : string.Empty);
                sqlParameters[31] = new SqlParameter("OEV2", !string.IsNullOrEmpty(model.EV2) ? model.EV2 : string.Empty);
                sqlParameters[32] = new SqlParameter("OEV3", !string.IsNullOrEmpty(model.EV3) ? model.EV3 : string.Empty);
                sqlParameters[33] = new SqlParameter("OEV4", !string.IsNullOrEmpty(model.EV4) ? model.EV4 : string.Empty);


                using (var r = _context.MultiResultSetSqlQuery(spName, parameters: sqlParameters))
                {
                    var vm = new OrderCustomModel();
                    vm.MostRecentOrders = r.ResultSetFor<OpenOrderCustomModel>().ToList();
                    vm.PreviousOrders = r.ResultSetFor<OpenOrderCustomModel>().ToList();
                    vm.FavoriteOrders = r.ResultSetFor<OpenOrderCustomModel>().ToList();
                    vm.OpenOrders = r.ResultSetFor<OpenOrderCustomModel>().ToList();
                    vm.OrderActivities = r.ResultSetFor<OrderActivityCustomModel>().ToList();
                    vm.FutureOpenOrders = r.ResultSetFor<FutureOpenOrderCustomModel>().ToList();
                    vm.GlobalCodes = r.ResultSetFor<GlobalCodes>().ToList();
                    vm.OrderId = r.ResultSetFor<long>().FirstOrDefault();

                    if (vm.PreviousOrders.Any())
                        vm.PreviousOrders = vm.PreviousOrders.DistinctBy(a => a.OrderCode).ToList();

                    if (vm.OpenOrders.Any())
                        vm.OpenOrders = vm.OpenOrders.OrderByDescending(a => a.EndDate).ToList();


                    return vm;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// This function returns all data related to Physician Tasks /  Nurse Tasks Tab in EHR.
        /// </summary>
        /// <param name="encId">Current Encounter ID</param>
        /// <param name="patientId">Current Patient ID</param>
        /// <param name="physicianId">Current Physician ID</param>
        /// <returns></returns>
        public PhysicianTabData GetPhysicianOrNurseTabData(long encId, long patientId, long physicianId, int notesUserType)
        {
            try
            {
                var spName = $"EXEC {StoredProcedures.SprocGetPhysicianTabData} @EId, @PId, @PhyId, @NotesUserType";

                var sqlParameters = new SqlParameter[4];
                sqlParameters[0] = new SqlParameter("EId", encId);
                sqlParameters[1] = new SqlParameter("PId", patientId);
                sqlParameters[2] = new SqlParameter("PhyId", physicianId);
                sqlParameters[3] = new SqlParameter("NotesUserType", notesUserType);

                using (var r = _context.MultiResultSetSqlQuery(spName, parameters: sqlParameters))
                {
                    var vm = new PhysicianTabData();
                    vm.EncounterListData = r.ResultSetFor<PatientEvaluationSetCustomModel>().ToList();
                    vm.MedicalNotes = r.ResultSetFor<MedicalNotes>().ToList();
                    vm.MedicalNotes2 = r.ResultSetFor<MedicalNotesCustomModel>().ToList();
                    vm.OpenOrders = r.ResultSetFor<OpenOrderCustomModel>().ToList();
                    vm.NurseDocuments = r.ResultSetFor<DocumentsTemplates>().ToList();
                    vm.PatientCareActivities = r.ResultSetFor<OrderActivityCustomModel>().ToList();
                    vm.Vitals = r.ResultSetFor<MedicalVital>().ToList();
                    vm.Vitals2 = r.ResultSetFor<MedicalVitalCustomModel>().ToList();
                    vm.DropdownListData = r.ResultSetFor<GlobalCodes>().ToList();
                    vm.OrderTypes = r.ResultSetFor<GlobalCodeCategory>().ToList();

                    if (notesUserType == 1)
                        vm.LabOrders = r.ResultSetFor<OpenOrderCustomModel>().ToList();

                    if (vm.MedicalNotes.Any() && vm.MedicalNotes2.Any())
                    {
                        var medicalNotes = vm.MedicalNotes.Join(vm.MedicalNotes2, arg => arg.MedicalNotesID, arg => arg.MedicalNotesID2, (first, second) => new MedicalNotesCustomModel
                        {
                            MedicalNotes = first,
                            MedicalNotesID2 = first.MedicalNotesID,
                            NotesAddedBy = second.NotesAddedBy,
                            NotesTypeName = second.NotesTypeName,
                            NotesUserTypeName = second.NotesUserTypeName
                        });
                        vm.MedicalNotes2 = medicalNotes.ToList();
                    }

                    if (vm.Vitals.Any() && vm.Vitals2.Any())
                    {
                        var vitals = vm.Vitals.Join(vm.Vitals2, arg => arg.MedicalVitalID, arg => arg.MedicalVitalID2,
                            (first, second) => new MedicalVitalCustomModel
                            {
                                MedicalVital = first,
                                PressureCustom = second.PressureCustom,
                                VitalAddedBy = second.VitalAddedBy,
                                VitalAddedOn = second.VitalAddedOn,
                                MedicalVitalName = second.MedicalVitalName,
                                UnitOfMeasureName = second.UnitOfMeasureName,
                                MedicalVitalID2 = first.MedicalVitalID
                            });
                        vm.Vitals2 = vitals.ToList();
                    }

                    return vm;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// This function returns all data related to Physician Tasks /  Nurse Tasks Tab in EHR.
        /// </summary>
        /// <param name="encId">Current Encounter ID</param>
        /// <param name="patientId">Current Patient ID</param>
        /// <param name="physicianId">Current Physician ID</param>
        /// <returns></returns>
        public PatientSummaryTabData GetPatientSummaryDataOnLoad(long encId, long patientId)
        {
            try
            {
                var sqlParameters = new SqlParameter[2];
                sqlParameters[0] = new SqlParameter("PId", patientId);
                sqlParameters[1] = new SqlParameter("EId", encId);

                using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetPatientSummaryTabData.ToString()
                    , parameters: sqlParameters, isCompiled: false))
                {
                    var vm = new PatientSummaryTabData
                    {
                        Encounters = r.ResultSetFor<EncounterCustomModel>().ToList(),        //Result Set 4 i.e. Encounters List

                        MedicalRecords = r.GetResultWithJson<MedicalRecord>(JsonResultsArray.MedicalRecord.ToString()),           //Result Set 5 i.e. Medical Records
                        Vitals = r.GetResultWithJson<MedicalVitalCustomModel>(JsonResultsArray.Vitals.ToString()),        //Result Set 6 i.e. Medical Vitals 2
                        MedicalNotes = r.GetResultWithJson<MedicalNotesCustomModel>(JsonResultsArray.MedicalNotes.ToString()),  //Result Set 7 i.e. Medical Notes 2
                        AllergyRecords = r.GetResultWithJson<AlergyCustomModel>(JsonResultsArray.Allergy.ToString()),      //Result Set 8 i.e. Allergy Records 2 (type AlergyCustomModel)

                        OpenOrders = r.ResultSetFor<OpenOrderCustomModel>().ToList(),            //Result Set 9 i.e. Open Orders
                        DiagnosisList = r.ResultSetFor<DiagnosisCustomModel>().ToList(),         //Result Set 10 i.e. Diagnosis List.
                        RiskFactor = r.ResultSetFor<RiskFactorViewModel>().FirstOrDefault(),     //Result Set 11 i.e. Risk Factors
                        CurrentEncounterId = r.ResultSetFor<long>().FirstOrDefault(),            //Result Set 12 i.e. Current Encounter ID

                        //Result Set 13 i.e. Current Medications (MedicalHistory)
                        MedicalHistory = r.GetResultWithJson<MedicalHistoryCustomModel>(JsonResultsArray.MedicalHistory.ToString())
                    };

                    ////Result Set 1
                    //var patientInfo = r.ResultSetFor<PatientInfo>().FirstOrDefault();               //Result Set 1 i.e. Main Patient Info
                    //vm.PatientInfo = r.ResultSetFor<PatientInfoCustomModel>().FirstOrDefault();     //Result Set 2  i.e. Patient Info

                    //if (vm.PatientInfo != null)
                    //{
                    //    vm.PatientInfo.PatientInfo = patientInfo;
                    //    var encounter = r.ResultSetFor<Encounter>().FirstOrDefault();       //Result Set 3 i.e. Current Encounter
                    //    vm.PatientInfo.CurrentEncounter = encounter;
                    //}

                    return vm;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Approves the pharmacy order.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="type">The type.</param>
        /// <param name="comment">The comment.</param>
        /// <returns></returns>
        public OrderCustomModel ApprovePharmacyOrder(int id, string type, string comment, long encounterId, bool withActivities, long categoryId, long physicianId, int corporateId, int facilityId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = $"EXEC {StoredProcedures.SprocApprovePharmacyOrderAndGetOrdersViewData} @pOrderId,@pOrderStatus,@pComment,@pEId,@pWithActivities,@pOrderCategoryId,@pPhysicianId,@pCId,@pFId";
                    var sqlParameters = new SqlParameter[9];
                    sqlParameters[0] = new SqlParameter("pOrderId", id);
                    sqlParameters[1] = new SqlParameter("pOrderStatus", type);
                    sqlParameters[2] = new SqlParameter("pComment", comment);
                    sqlParameters[3] = new SqlParameter("pEId", encounterId);
                    sqlParameters[4] = new SqlParameter("pWithActivities", withActivities);
                    sqlParameters[5] = new SqlParameter("pOrderCategoryId", categoryId);
                    sqlParameters[6] = new SqlParameter("pPhysicianId", physicianId);
                    sqlParameters[7] = new SqlParameter("pCId", corporateId);
                    sqlParameters[8] = new SqlParameter("pFId", facilityId);

                    using (var r = _context.MultiResultSetSqlQuery(spName, parameters: sqlParameters))
                    {
                        var vm = new OrderCustomModel();
                        vm.OpenOrders = r.ResultSetFor<OpenOrderCustomModel>().ToList();        //Result Set 1 (Order Activities)

                        if (withActivities)
                            vm.OrderActivities = r.ResultSetFor<OrderActivityCustomModel>().ToList();   //Result Set 2 (Orders)


                        vm.PreviousOrders = r.ResultSetFor<OpenOrderCustomModel>().ToList();        //Result Set 3 (Most Recent Orders)

                        return vm;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }


        public OrderCustomModel GetOrdersAndActivities(long encounterId)
        {
            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("pEncounterId", encounterId);

            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetOrdersAndActivitiesByEncounter.ToString(), parameters: sqlParameters, isCompiled: false))
            {
                var vm = new OrderCustomModel();
                vm.OpenOrders = r.GetResultWithJson<OpenOrderCustomModel>(JsonResultsArray.OpenOrders.ToString());
                vm.OrderActivities = r.GetResultWithJson<OrderActivityCustomModel>(JsonResultsArray.OrderActivities.ToString());
                return vm;
            }
        }
        public List<OrderActivityCustomModel> GetOrderActivitiesByOpenOrder(long openOrderId)
        {
            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("pOpenOrderId", openOrderId);

            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetOrderActivitiesByOpenOrder.ToString(), parameters: sqlParameters, isCompiled: false))
            {
                var vm = r.GetResultWithJson<OrderActivityCustomModel>(JsonResultsArray.OrderActivities.ToString());
                return vm;
            }
        }
    }
}
