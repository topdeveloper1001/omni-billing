using System.Linq;
using System.Transactions;
using System.Web.Configuration;
using BillingSystem.Model;
using System.Collections.Generic;
using BillingSystem.Model.CustomModel;
using BillingSystem.Common.Common;
using System;

namespace BillingSystem.Bal.BusinessAccess
{
    using BillingSystem.Bal.Mapper;
    using BillingSystem.Model.EntityDto;

    public class OpenOrderBal : BaseBal
    {
        private OpenOrderMapper OpenOrderMapper { get; set; }

        public OpenOrderBal()
        {
            OpenOrderMapper = new OpenOrderMapper();
        }

        public OpenOrderBal(string cptTableNumber, string serviceCodeTableNumber, string drgTableNumber, string drugTableNumber, string hcpcsTableNumber, string diagnosisTableNumber)
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

            OpenOrderMapper = new OpenOrderMapper();
        }

        /// <summary>
        /// Get the Physician Orders
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="orderStatus">The order status.</param>
        /// <returns>
        /// Return the Encounter Order list
        /// </returns>
        public List<OpenOrderCustomModel> GetPhysicianOrders(int encounterId, string orderStatus, string fId = "")
        {
            using (var enRepository = UnitOfWork.OpenOrderRepository)
            {
                var status = OrderStatus.Open.ToString();
                var list = new List<OpenOrderCustomModel>();

                //Open And Open Administered Status
                var openOrders = status == orderStatus
                    ? enRepository.Where(
                        a => a.EncounterID == encounterId && (a.OrderStatus == ("1")))
                        .OrderByDescending(x => x.OpenOrderID)
                        .ToList()
                    : enRepository.Where(
                        a => a.EncounterID == encounterId && (a.OrderStatus != ("1")))
                        .OrderBy(x => x.OpenOrderID)
                        .ToList();

                list.AddRange(
                    openOrders.Select(
                        item =>
                        new OpenOrderCustomModel
                        {
                            OpenOrderID = item.OpenOrderID,
                            OpenOrderPrescribedDate = item.OpenOrderPrescribedDate,
                            PhysicianID = item.PhysicianID,
                            PatientID = item.PatientID,
                            EncounterID = item.EncounterID,
                            DiagnosisCode = item.DiagnosisCode,
                            OrderType = item.OrderType,
                            OrderCode = item.OrderCode,
                            Quantity = item.Quantity,
                            FrequencyCode = item.FrequencyCode,
                            PeriodDays = item.PeriodDays,
                            OrderNotes = item.OrderNotes,
                            OrderStatus = item.OrderStatus,
                            IsActivitySchecduled = item.IsActivitySchecduled,
                            ItemName = item.ItemName,
                            ItemStrength = item.ItemStrength,
                            ItemDosage = item.ItemDosage,
                            IsActive = item.IsActive,
                            CreatedBy = item.CreatedBy,
                            CreatedDate = item.CreatedDate,
                            ModifiedBy = item.ModifiedBy,
                            ModifiedDate = item.ModifiedDate,
                            IsDeleted = item.IsDeleted,
                            DeletedBy = item.DeletedBy,
                            DeletedDate = item.DeletedDate,
                            CategoryId = item.CategoryId,
                            SubCategoryId = item.SubCategoryId,
                            StartDate = item.StartDate,
                            EndDate = item.EndDate,
                            DiagnosisDescription = GetDiagnoseNameByCodeId(item.DiagnosisCode),
                            CategoryName = GetGlobalCategoryNameById(Convert.ToString(item.CategoryId), fId),
                            SubCategoryName = GetNameByGlobalCodeValue(Convert.ToString(item.SubCategoryId), Convert.ToString(item.CategoryId), Convert.ToString(item.FacilityID)),
                            OrderDescription = GetCodeDescription(item.OrderCode, item.OrderType),
                            Status =
                                    (((item.CategoryId == Convert.ToInt32(OrderTypeCategory.Pharmacy))
                                      && (!Convert.ToBoolean(item.IsApproved) && (item.OrderStatus
                                          != Convert.ToInt32(OpenOrderActivityStatus.OnBill)
                                                 .ToString())
                                      && (item.OrderStatus
                                          != Convert.ToInt32(OpenOrderActivityStatus.Cancel)
                                                 .ToString())
                                      && (item.OrderStatus
                                          != Convert.ToInt32(OpenOrderActivityStatus.Closed)
                                                 .ToString()))
                                         ? "Waiting For Approval"
                                         : GetNameByGlobalCodeValue(
                                             item.OrderStatus,
                                             Convert.ToInt32(GlobalCodeCategoryValue.OrderStatus).ToString()))),
                            FrequencyText =
                                    GetNameByGlobalCodeValue(
                                        item.FrequencyCode,
                                        Convert.ToInt32(GlobalCodeCategoryValue.OrderFrequencyType).ToString()),
                            SpecimenTypeStr =
                                    CalculateLabResultSpecimanType(
                                        item.OrderCode,
                                        null,
                                        item.PatientID),
                            IsApproved = item.IsApproved
                        }));
                return list;
            }
        }

        /// <summary>
        /// Get the ordr detail by order id
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public OpenOrder GetOpenOrderDetail(int id)
        {
            using (var enRepository = UnitOfWork.OpenOrderRepository)
            {
                var iQueryableOrder = id > 0
                    ? enRepository.GetAll().FirstOrDefault(o => o.OpenOrderID == id)
                    : new OpenOrder();
                return iQueryableOrder;
            }
        }

        /// <summary>
        /// Add the physiccian order to DB
        /// </summary>
        /// <param name="model">The order.</param>
        /// <returns></returns>
        public int AddUpdatePhysicianOpenOrder(OpenOrder model)
        {
            var updateStatus = 0;
            using (var enRepository = UnitOfWork.OpenOrderRepository)
            {
                if (model.OpenOrderID > 0)
                {
                    /*
                    * Purpose: Change Open Order Details in Database
                    * By: Amit Jain 
                    * On: 05 Feb, 2016
                    */
                    //--------------Code Changes starts here-----------------------------
                    //enRepository.UpdateEntity(model, model.OpenOrderID);

                    //--------------Update Procedure Starts here-----------------------------
                    updateStatus = enRepository.ChangeOrderDetail(model);
                    //--------------Update Procedure ends here-----------------------------


                    //--------------Code Changes ends here-----------------------------
                }
                else
                    enRepository.Create(model);

            }

            if (model.OrderStatus != Convert.ToString((int)OrderStatus.Open))
            {
                using (var rep = UnitOfWork.BillHeaderRepository)
                {
                    var result = rep.ApplyOrderBill(Convert.ToInt32(model.EncounterID));
                }
            }
            return model.OpenOrderID;
        }

        /// <summary>
        /// Get the Physician Orders of Current Patient
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns>
        /// Return the Encounter Order list
        /// </returns>
        public List<OpenOrderCustomModel> GetOrdersByPatientId(int patientId)
        {
            var list = new List<OpenOrderCustomModel>();
            using (var enRepository = UnitOfWork.OpenOrderRepository)
            {
                var orderList = enRepository.GetAll().Where(a => a.PatientID == patientId).ToList();
                if (orderList.Count > 0)
                {
                    list.AddRange(orderList.Select(item => new OpenOrderCustomModel
                    {
                        OpenOrderID = item.OpenOrderID,
                        OpenOrderPrescribedDate = item.OpenOrderPrescribedDate,
                        PhysicianID = item.PhysicianID,
                        PatientID = item.PatientID,
                        EncounterID = item.EncounterID,
                        DiagnosisCode = item.DiagnosisCode,
                        OrderType = item.OrderType,
                        OrderCode = item.OrderCode,
                        Quantity = item.Quantity,
                        FrequencyCode = item.FrequencyCode,
                        PeriodDays = item.PeriodDays,
                        OrderNotes = item.OrderNotes,
                        OrderStatus = item.OrderStatus,
                        IsActivitySchecduled = item.IsActivitySchecduled,
                        ItemName = item.ItemName,
                        ItemStrength = item.ItemStrength,
                        ItemDosage = item.ItemDosage,
                        IsActive = item.IsActive,
                        CreatedBy = item.CreatedBy,
                        CreatedDate = item.CreatedDate,
                        ModifiedBy = item.ModifiedBy,
                        ModifiedDate = item.ModifiedDate,
                        IsDeleted = item.IsDeleted,
                        DeletedBy = item.DeletedBy,
                        DeletedDate = item.DeletedDate,
                        StartDate = item.StartDate,
                        EndDate = item.EndDate,
                        DiagnosisDescription = GetDiagnoseNameByCodeId(item.DiagnosisCode),
                        CategoryName = GetGlobalCategoryNameById(Convert.ToString(item.CategoryId)),
                        SubCategoryName = GetNameByGlobalCodeId(Convert.ToInt32(item.SubCategoryId)),
                        OrderDescription = GetCodeDescription(item.OrderCode, item.OrderType),
                        Status = GetNameByGlobalCodeValue(item.OrderStatus, Convert.ToInt32(GlobalCodeCategoryValue.OrderStatus).ToString()),
                        CategoryId = item.CategoryId,
                        SubCategoryId = item.SubCategoryId,
                        FrequencyText = GetNameByGlobalCodeValue(item.FrequencyCode, Convert.ToInt32(GlobalCodeCategoryValue.OrderFrequencyType).ToString()),
                        SpecimenTypeStr = CalculateLabResultSpecimanType(item.OrderCode, null, item.PatientID),
                    }));
                }
                return list;
            }
        }

        /// <summary>
        /// Gets the encounters list by patient identifier.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public IEnumerable<EncounterCustomModel> GetEncountersListByPatientId(int patientId)
        {
            using (var bal = new EncounterBal())
            {
                var list = bal.GetEncounterListByPatientId(patientId);
                return list;
            }
        }

        /// <summary>
        /// Gets the favorite orders.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public List<OpenOrderCustomModel> GetFavoriteOrders(int userId)
        {
            var favOrders = new List<OpenOrderCustomModel>();
            try
            {

                //var orders = GetOrdersByPhysician(userId);
                //if (orders.Count > 0)
                //{
                using (var rep = UnitOfWork.FavoritesRepository)
                {
                    var CPTcategory = Convert.ToInt32(FavoriteCategories.CPT).ToString();
                    var DRGcategory = Convert.ToInt32(FavoriteCategories.DRUG).ToString();
                    var favorites =
                        rep.Where(
                            f =>
                                (f.CategoryId.Equals(CPTcategory) || f.CategoryId.Equals(DRGcategory)) && (f.IsDeleted == null || !(bool)f.IsDeleted) &&
                                f.UserID == userId)
                            .ToList();
                    favOrders.AddRange(from item in favorites
                                       let getOrderCodeparent = GetAllOrderingCodes(item.CodeId)
                                       let generalCodesCustomModel = getOrderCodeparent.FirstOrDefault()
                                       where generalCodesCustomModel != null
                                       select new OpenOrderCustomModel
                                       {
                                           DiagnosisCode = item.CodeId,
                                           OrderType = item.CategoryId,
                                           OrderCode = item.CodeId,
                                           DiagnosisDescription =
                                               item.CategoryId.Equals(Convert.ToInt32(OrderType.Diagnosis).ToString())
                                                   ? GetDiagnoseNameByCodeId(item.CodeId)
                                                   : "",
                                           UserDefinedDescriptionId = item.UserDefinedDescriptionID,
                                           UserDefinedDescription = item.UserDefineDescription,
                                           OrderTypeName =
                                               GetNameByGlobalCodeValue((item.CategoryId),
                                                   Convert.ToInt32(GlobalCodeCategoryValue.CodeTypes).ToString()),
                                           OrderDescription = GetCodeDescription(item.CodeId, item.CategoryId),
                                           CategoryName =
                                               GetNameByGlobalCodeValue((item.CategoryId),
                                                   Convert.ToInt32(GlobalCodeCategoryValue.CodeTypes).ToString()),
                                           SubCategoryName =
                                               GetNameByGlobalCodeValue(
                                                   Convert.ToInt32(generalCodesCustomModel.GlobalCodeId).ToString(), (item.CategoryId)),
                                           //GetNameByGlobalCodeId(Convert.ToInt32(generalCodesCustomModel.GlobalCodeId)),
                                           CategoryId = Convert.ToInt32(generalCodesCustomModel.GlobalCodeCategoryId),
                                           SubCategoryId = generalCodesCustomModel.GlobalCodeId,
                                       });
                    //favOrders = (from i in orders
                    //    join f in favorites
                    //        on Convert.ToString(i.OpenOrderID) equals f.CodeId
                    //    select new OpenOrderCustomModel
                    //    {
                    //        DiagnosisCode = f.CodeId,
                    //        OrderType = f.CategoryId,
                    //        OrderCode = f.CodeId,
                    //        DiagnosisDescription =f.CategoryId.Equals(Convert.ToInt32(OrderType.Diagnosis).ToString()) ? GetDiagnoseNameByCodeId(f.CodeId):"",
                    //        UserDefinedDescriptionId = f.UserDefinedDescriptionID,
                    //        UserDefinedDescription = f.UserDefineDescription,
                    //        OrderTypeName = GetNameByGlobalCodeValue((f.CategoryId), Convert.ToInt32(GlobalCodeCategoryValue.CodeTypes).ToString()),
                    //        OrderDescription = GetCodeDescription(f.CodeId, f.CategoryId),
                    //        CategoryName = GetGlobalCategoryNameById(Convert.ToString(i.CategoryId)),
                    //        SubCategoryName = GetNameByGlobalCodeId(Convert.ToInt32(i.SubCategoryId)),
                    //        Status =
                    //            GetNameByGlobalCodeValue(i.OrderStatus,
                    //                Convert.ToInt32(GlobalCodeCategoryValue.OrderStatus).ToString()),
                    //        CategoryId = i.CategoryId,
                    //        SubCategoryId = i.SubCategoryId,
                    //        FrequencyText =
                    //            GetNameByGlobalCodeValue(i.FrequencyCode,
                    //                Convert.ToInt32(GlobalCodeCategoryValue.OrderFrequencyType).ToString())
                    //    }).ToList();

                }

            }
            catch (Exception)
            {
            }
            return favOrders;
        }

        /// <summary>
        /// Gets the searched orders.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public List<OpenOrderCustomModel> GetSearchedOrders(string text)
        {
            var list = new List<OpenOrderCustomModel>();
            using (var rep = UnitOfWork.OpenOrderRepository)
            {
                text = text.ToLower();
                var orders =
                    rep.Where(
                        o =>
                            o.OrderNotes.ToLower().Contains(text) || o.DiagnosisCode.ToLower().Contains(text) ||
                            o.OrderCode.ToLower().Contains(text)).ToList();
                if (orders.Count > 0)
                {
                    list.AddRange(orders.Select(item => new OpenOrderCustomModel
                    {
                        OpenOrderID = item.OpenOrderID,
                        OpenOrderPrescribedDate = item.OpenOrderPrescribedDate,
                        PhysicianID = item.PhysicianID,
                        PatientID = item.PatientID,
                        EncounterID = item.EncounterID,
                        DiagnosisCode = item.DiagnosisCode,
                        OrderType = item.OrderType,
                        OrderCode = item.OrderCode,
                        Quantity = item.Quantity,
                        FrequencyCode = item.FrequencyCode,
                        PeriodDays = item.PeriodDays,
                        OrderNotes = item.OrderNotes,
                        OrderStatus = item.OrderStatus,
                        IsActivitySchecduled = item.IsActivitySchecduled,
                        ItemName = item.ItemName,
                        ItemStrength = item.ItemStrength,
                        ItemDosage = item.ItemDosage,
                        IsActive = item.IsActive,
                        CreatedBy = item.CreatedBy,
                        CreatedDate = item.CreatedDate,
                        ModifiedBy = item.ModifiedBy,
                        ModifiedDate = item.ModifiedDate,
                        IsDeleted = item.IsDeleted,
                        DeletedBy = item.DeletedBy,
                        DeletedDate = item.DeletedDate,
                        StartDate = item.StartDate,
                        EndDate = item.EndDate,
                        DiagnosisDescription = GetDiagnoseNameByCodeId(item.DiagnosisCode),
                        CategoryName = GetGlobalCategoryNameById(Convert.ToString(item.CategoryId)),
                        SubCategoryName = GetNameByGlobalCodeId(Convert.ToInt32(item.SubCategoryId)),
                        OrderDescription = GetCodeDescription(item.OrderCode, item.OrderType),
                        Status = GetNameByGlobalCodeValue(item.OrderStatus, Convert.ToInt32(GlobalCodeCategoryValue.OrderStatus).ToString()),
                        FrequencyText = GetNameByGlobalCodeValue(item.FrequencyCode, Convert.ToInt32(GlobalCodeCategoryValue.OrderFrequencyType).ToString()),
                        SpecimenTypeStr = CalculateLabResultSpecimanType(item.OrderCode, null, item.PatientID),
                    }));
                }
                return list;
            }
        }

        /// <summary>
        /// Gets the most recent orders.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public List<OpenOrderCustomModel> GetMostRecentOrders(int userId)
        {
            var list = GetMostOrderedList(userId, 0);
            if (list.Count > 10)
                list = list.Take(10).ToList();
            return list;
        }

        /// <summary>
        /// Gets the orders by physician.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<OpenOrderCustomModel> GetOrdersByPhysician(int userId, int corporateId, int facilityId)
        {
            var list = new List<OpenOrderCustomModel>();
            using (var oRep = UnitOfWork.OpenOrderRepository)
            {
                list = oRep.GetPhysicianPreviousOrders(userId, corporateId, facilityId);
            }
            return list;
        }

        /// <summary>
        /// Gets the most ordered list.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="numberOfDays">The number of days.</param>
        /// <returns></returns>
        public List<OpenOrderCustomModel> GetMostOrderedList(int userId, int numberOfDays)
        {
            var list = new List<OpenOrderCustomModel>();
            using (var rep = UnitOfWork.OpenOrderRepository)
            {
                var result = rep.GetMostOrderedList(userId, numberOfDays).ToList();
                return result.Distinct().ToList();
            }
        }

        /// <summary>
        /// Gets the physican favorite ordered list.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <param name="corporateid">The corporateid.</param>
        /// <returns></returns>
        public List<OpenOrderCustomModel> GetPhysicanFavoriteOrderedList(int userId, int facilityid, int corporateid)
        {
            using (var rep = UnitOfWork.OpenOrderRepository)
            {
                var result = rep.GetPhysicianFavoriteOrders(userId, facilityid, corporateid);
                return result.ToList();
            }
        }

        /// <summary>
        /// Gets the orders by physician identifier.
        /// </summary>
        /// <param name="phyId">The phy identifier.</param>
        /// <returns></returns>
        public List<OpenOrderCustomModel> GetOrdersByPhysicianId(int phyId)
        {
            var list = new List<OpenOrderCustomModel>();
            using (var enRepository = UnitOfWork.OpenOrderRepository)
            {
                var orderList = enRepository.GetAll().Where(a => a.PhysicianID == phyId).ToList();
                var favBal = new FavoritesBal();
                var phyfavList = favBal.GetFavoriteByPhyId(phyId);
                orderList =
                    orderList.Where(
                        x => x.PhysicianID == phyId && !(phyfavList.Any(y => y.CodeId == x.OpenOrderID.ToString())))
                        .ToList();

                if (orderList.Count > 0)
                {
                    list.AddRange(orderList.Select(item => new OpenOrderCustomModel
                    {
                        OpenOrderID = item.OpenOrderID,
                        OpenOrderPrescribedDate = item.OpenOrderPrescribedDate,
                        PhysicianID = item.PhysicianID,
                        PatientID = item.PatientID,
                        EncounterID = item.EncounterID,
                        DiagnosisCode = item.DiagnosisCode,
                        OrderType = item.OrderType,
                        OrderCode = item.OrderCode,
                        Quantity = item.Quantity,
                        FrequencyCode = item.FrequencyCode,
                        PeriodDays = item.PeriodDays,
                        OrderNotes = item.OrderNotes,
                        OrderStatus = item.OrderStatus,
                        IsActivitySchecduled = item.IsActivitySchecduled,
                        ItemName = item.ItemName,
                        ItemStrength = item.ItemStrength,
                        ItemDosage = item.ItemDosage,
                        IsActive = item.IsActive,
                        CreatedBy = item.CreatedBy,
                        CreatedDate = item.CreatedDate,
                        ModifiedBy = item.ModifiedBy,
                        ModifiedDate = item.ModifiedDate,
                        IsDeleted = item.IsDeleted,
                        DeletedBy = item.DeletedBy,
                        DeletedDate = item.DeletedDate,
                        StartDate = item.StartDate,
                        EndDate = item.EndDate,
                        DiagnosisDescription = GetDiagnoseNameByCodeId(item.DiagnosisCode),
                        OrderTypeName = GetNameByGlobalCodeValue(Convert.ToInt32(item.OrderType).ToString(), Convert.ToInt32(GlobalCodeCategoryValue.CodeTypes).ToString()),
                        OrderCount = GetOrdersCount(phyId, item.OrderCode, item.OrderType),
                        OrderDescription = GetCodeDescription(item.OrderCode, item.OrderType),
                        Status = GetNameByGlobalCodeValue(item.OrderStatus, Convert.ToInt32(GlobalCodeCategoryValue.OrderStatus).ToString()),
                        FrequencyText = GetNameByGlobalCodeValue(item.FrequencyCode, Convert.ToInt32(GlobalCodeCategoryValue.OrderFrequencyType).ToString()),
                        SpecimenTypeStr = CalculateLabResultSpecimanType(item.OrderCode, null, item.PatientID),
                        //MulitpleOrderActivites = CheckMulitpleOpenActivites(item.OpenOrderID),
                    }));
                }
                return list;
            }
        }

        /// <summary>
        /// Gets the orders count.
        /// </summary>
        /// <param name="phyId">The phy identifier.</param>
        /// <param name="codeid">The codeid.</param>
        /// <param name="ordertype">The ordertype.</param>
        /// <returns></returns>
        private int GetOrdersCount(int phyId, string codeid, string ordertype)
        {
            using (var enRepository = UnitOfWork.OpenOrderRepository)
            {
                var ordersCount = enRepository.Where(a => a.PhysicianID == phyId && a.OrderCode == codeid && a.OrderType == ordertype).Count();
                return ordersCount;
            }
        }

        /// <summary>
        /// Gets the order identifier by order code.
        /// </summary>
        /// <param name="phyId">The phy identifier.</param>
        /// <param name="codeid">The codeid.</param>
        /// <returns></returns>
        public int GetOrderIdByOrderCode(int phyId, string codeid)
        {
            using (var enRepository = UnitOfWork.OpenOrderRepository)
            {
                var orderObj = enRepository.Where(a => a.PhysicianID == phyId && a.OrderCode == codeid).FirstOrDefault();
                return orderObj != null ? orderObj.OpenOrderID : 0;
            }
        }

        /// <summary>
        /// Gets the open orders by encounter identifier.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public List<OpenOrderCustomModel> GetAllOrdersByEncounterId(int encounterId)
        {
            var list = new List<OpenOrderCustomModel>();
            using (var enRepository = UnitOfWork.OpenOrderRepository)
            {
                list = enRepository.GetAllOrdersByEncounterId(encounterId);
                return list.OrderByDescending(x => x.EndDate).ToList();
            }
        }

        /// <summary>
        /// Adds the update physician multiple open order.
        /// </summary>
        /// <param name="model">The order.</param>
        /// <returns></returns>
        public int[] AddUpdatePhysicianMultipleOpenOrder(List<OpenOrder> model)
        {
            try
            {
                var result = new int[model.Count()];
                //using (var transScope = new TransactionScope())
                //{
                using (var openOrderRep = UnitOfWork.OpenOrderRepository)
                {
                    for (int index = 0; index < model.Count; index++)
                    {
                        var openOrder = model[index];
                        openOrderRep.Create(openOrder);

                        /*
                         * Who: Amit Jain
                         * When: 03 March, 2016
                         * Why: It calls the Stored Procedure 'SPROC_ApplyOrderToBill' for billing entries in the BillActivity Table. 
                         */

                        if (model[index].OrderStatus != Convert.ToString((int)OrderStatus.Open))
                        {
                            using (var rep = UnitOfWork.BillHeaderRepository)
                                rep.ApplyOrderBill(Convert.ToInt32(model[index].EncounterID));
                        }
                        result[index] = Convert.ToInt32(openOrder.OpenOrderID);
                    }
                    //    transScope.Complete();
                    //}
                }
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// Gets the orders by encounterid.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public List<OpenOrderCustomModel> GetOrdersByEncounterid(int encounterId)
        {
            var list = new List<OpenOrderCustomModel>();
            using (var oRep = UnitOfWork.OpenOrderRepository)
            {
                var orders =
                    oRep.Where(p => p.CreatedBy != null && (int)p.EncounterID == encounterId)
                        .OrderByDescending(f => f.CreatedDate)
                        .ToList();
                list.AddRange(orders.Select(item => new OpenOrderCustomModel
                {
                    OpenOrderID = item.OpenOrderID,
                    OpenOrderPrescribedDate = item.OpenOrderPrescribedDate,
                    PhysicianID = item.PhysicianID,
                    PatientID = item.PatientID,
                    EncounterID = item.EncounterID,
                    DiagnosisCode = item.DiagnosisCode,
                    OrderType = item.OrderType,
                    OrderCode = item.OrderCode,
                    Quantity = item.Quantity,
                    FrequencyCode = item.FrequencyCode,
                    PeriodDays = item.PeriodDays,
                    OrderNotes = item.OrderNotes,
                    OrderStatus = item.OrderStatus,
                    IsActivitySchecduled = item.IsActivitySchecduled,
                    ItemName = item.ItemName,
                    ItemStrength = item.ItemStrength,
                    ItemDosage = item.ItemDosage,
                    IsActive = item.IsActive,
                    CreatedBy = item.CreatedBy,
                    CreatedDate = item.CreatedDate,
                    ModifiedBy = item.ModifiedBy,
                    ModifiedDate = item.ModifiedDate,
                    IsDeleted = item.IsDeleted,
                    DeletedBy = item.DeletedBy,
                    DeletedDate = item.DeletedDate,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    DiagnosisDescription = GetDiagnoseNameByCodeId(item.DiagnosisCode),
                    CategoryName = GetGlobalCategoryNameById(Convert.ToString(item.CategoryId)),
                    SubCategoryName = GetNameByGlobalCodeId(Convert.ToInt32(item.SubCategoryId)),
                    OrderTypeName = GetNameByGlobalCodeValue((item.OrderType), Convert.ToInt32(GlobalCodeCategoryValue.CodeTypes).ToString()),
                    OrderDescription = GetCodeDescription(item.OrderCode, item.OrderType),
                    Status = GetNameByGlobalCodeValue(item.OrderStatus, Convert.ToInt32(GlobalCodeCategoryValue.OrderStatus).ToString()),
                    CategoryId = item.CategoryId,
                    SubCategoryId = item.SubCategoryId,
                    FrequencyText = GetNameByGlobalCodeValue(item.FrequencyCode, Convert.ToInt32(GlobalCodeCategoryValue.OrderFrequencyType).ToString()),
                    SpecimenTypeStr = CalculateLabResultSpecimanType(item.OrderCode, null, item.PatientID),
                    // MulitpleOrderActivites = CheckMulitpleOpenActivites(item.OpenOrderID),
                }));
            }
            return list;
        }

        /// <summary>
        /// Gets the fav open order detail.
        /// </summary>
        /// <param name="favorderId">The favorder identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public OpenOrder GetFavOpenOrderDetail(long favorderId, long facilityId, long userId)
        {
            using (var rep = UnitOfWork.FavoritesRepository)
            {
                var results = rep.GetFavoriteOrderById(favorderId, facilityId, userId);
                return results;
            }

            //var currentDateTime = GetInvariantCultureDateTime(facilityId);
            //var physicianFavbal = new FavoritesBal();
            //var phyicianFavObj = physicianFavbal.GetFavoriteById(favorderId);
            ////var getOrderCodeparent = GetAllOrderingCodes(phyicianFavObj.CodeId);
            //var getOrderCodeparent = GetSelectedCodeParent(phyicianFavObj.CodeId);

            //var generalCodesCustomModel = getOrderCodeparent;

            //var openorderObj = new OpenOrder()
            //{
            //    OpenOrderID = 0,
            //    OpenOrderPrescribedDate = currentDateTime,
            //    PhysicianID = 0,
            //    PatientID = 0,
            //    EncounterID = 0,
            //    DiagnosisCode = phyicianFavObj.CodeId,
            //    StartDate = currentDateTime,
            //    EndDate = currentDateTime,
            //    CategoryId = generalCodesCustomModel != null ? Convert.ToInt32(generalCodesCustomModel.GlobalCodeCategoryId) : 0,
            //    SubCategoryId = generalCodesCustomModel != null ? generalCodesCustomModel.GlobalCodeId : 0, //null,
            //    OrderType = generalCodesCustomModel != null ? generalCodesCustomModel.CodeType : "0",
            //    OrderCode = phyicianFavObj.CodeId,
            //    Quantity = 0,
            //    FrequencyCode = "",
            //    PeriodDays = "",
            //    OrderNotes = "",
            //    OrderStatus = "1",
            //    IsActivitySchecduled = null,
            //    ActivitySchecduledOn = null,
            //    ItemName = null,
            //    ItemStrength = null,
            //    ItemDosage = null,
            //    IsActive = true,
            //    CreatedBy = 1,
            //    CreatedDate = currentDateTime,
            //    ModifiedBy = null,
            //    ModifiedDate = null,
            //    IsDeleted = null,
            //    DeletedBy = null,
            //    DeletedDate = null,
            //    CorporateID = null,
            //    FacilityID = null,
            //};
            //return openorderObj;
        }

        /// <summary>
        /// Checks the mulitple open activites.
        /// </summary>
        /// <param name="openorderid">The openorderid.</param>
        /// <returns></returns>
        public bool CheckMulitpleOpenActivites(int openorderid)
        {
            using (var orderActivityBal = new OrderActivityBal())
            {
                var orderactiviteslist = orderActivityBal.GetOrderActivitiesByOrderId(openorderid);
                return orderactiviteslist.Any() && orderactiviteslist.Count > 1;
            }
        }

        /// <summary>
        /// Checks the mulitple open activites.
        /// </summary>
        /// <param name="ordercode">The ordercode.</param>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="erncounterid">The erncounterid.</param>
        /// <returns></returns>
        public DrugReactionCustomModel CheckDurgAllergy(string ordercode, int patientId, int erncounterid)
        {
            using (var rep = UnitOfWork.OpenOrderRepository)
            {
                var result = rep.CheckDurgAllergy(ordercode, patientId, erncounterid);
                return result != null ? result.FirstOrDefault() : null;
            }
        }

        /// <summary>
        /// Gets the active encounter identifier.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public int GetActiveEncounterId(int patientId)
        {
            var encounterId = 0;
            using (var rep = UnitOfWork.EncounterRepository)
            {
                var current = rep.Where(e => e.PatientID == patientId && e.EncounterEndTime == null).FirstOrDefault();
                if (current != null)
                    encounterId = current.EncounterID;
            }
            return encounterId;
        }

        /// <summary>
        /// Approves the pharmacy order.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="type">The type.</param>
        /// <param name="comment">The comment.</param>
        /// <returns></returns>
        public bool ApprovePharmacyOrder(int id, string type, string comment)
        {
            using (var rep = UnitOfWork.EncounterRepository)
            {
                bool isExecuted = rep.ApprovePharmacyOrder(id, type, comment);
                return isExecuted;
            }
        }

        /// <summary>
        /// Get the Physician Orders
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="orderStatus">The order status.</param>
        /// <returns>
        /// Return the Encounter Order list
        /// </returns>
        public List<OpenOrderCustomModel> GetPhysicianOrdersList(int encounterId, string orderStatus, long categoryId = 0)
        {
            using (var openOrderRepository = UnitOfWork.OpenOrderRepository)
            {
                var list = openOrderRepository.GetPhysicianOrdersList(encounterId, orderStatus, categoryId);
                return list;
            }
        }


        /// <summary>
        /// Updates the open order status.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="status">The status.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="modifiedDate">The modified date.</param>
        /// <returns></returns>
        public int UpdateOpenOrderStatus(int orderId, string status, int userId, DateTime modifiedDate)
        {
            var updateStatus = 0;
            using (var enRepository = UnitOfWork.OpenOrderRepository)
            {
                if (orderId > 0)
                {
                    var model = enRepository.GetSingle(orderId);
                    if (model != null)
                    {
                        // This needs to be changed as the Order Status can not be always on Bill,
                        // If the Order have any Activites as Closed/OnBill then order should have on Bill Status
                        // If the order have all activites as Cancel/Revoked then order shouldhave Cancel/Revoked Status.
                        // model.OrderStatus = Convert.ToString((int)OrderStatus.OnBill);
                        // Changes done on the 14th March 2016 BY Shashank Awasthy
                        var getOrderactivites = new OrderActivityBal().GetOrderActivitiesByOrderId(orderId);
                        if (getOrderactivites.All(x => x.OrderActivityStatus == 4))
                        {
                            model.OrderStatus = Convert.ToString((int)OrderStatus.OnBill);
                        }
                        else if (getOrderactivites.All(x => x.OrderActivityStatus == 3 || x.OrderActivityStatus == 9))
                        {
                            model.OrderStatus = Convert.ToString((int)OrderStatus.Cancel);
                        }
                        else
                        {
                            model.OrderStatus = Convert.ToString((int)OrderStatus.Closed);
                        }
                        model.ModifiedBy = userId;
                        model.ModifiedDate = modifiedDate;
                        enRepository.UpdateEntity(model, orderId);
                        updateStatus = orderId;
                    }
                    else
                        updateStatus = -1;
                }
            }

            return updateStatus;
        }

        /// <summary>
        /// Cancels the open order.
        /// </summary>
        /// <param name="orderid">The orderid.</param>
        /// <param name="userid">The userid.</param>
        /// <returns></returns>
        public bool CancelOpenOrder(int orderid, int userid)
        {
            using (var orderRep = UnitOfWork.OpenOrderRepository)
            {
                return orderRep.CancelOpenOrder(orderid, userid);
            }
        }

        /// <summary>
        /// Adds the update open order custom model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public int AddUpdateOpenOrderCustomModel(OpenOrderCustomModel model)
        {
            var updateStatus = 0;
            var openOrderOj = OpenOrderMapper.MapViewModelToModel(model);
            using (var enRepository = UnitOfWork.OpenOrderRepository)
            {
                if (openOrderOj.OpenOrderID > 0)
                {
                    /*
                    * Purpose: Change Open Order Details in Database
                    * By: Amit Jain 
                    * On: 05 Feb, 2016
                    */
                    //--------------Code Changes starts here-----------------------------
                    //enRepository.UpdateEntity(model, model.OpenOrderID);

                    //--------------Update Procedure Starts here-----------------------------
                    updateStatus = enRepository.ChangeOrderDetail(openOrderOj);
                    //--------------Update Procedure ends here-----------------------------


                    //--------------Code Changes ends here-----------------------------
                }
                else
                    enRepository.Create(openOrderOj);

            }

            if (openOrderOj.OrderStatus != Convert.ToString((int)OrderStatus.Open))
            {
                using (var rep = UnitOfWork.BillHeaderRepository)
                {
                    var result = rep.ApplyOrderBill(Convert.ToInt32(openOrderOj.EncounterID));
                }
            }
            return openOrderOj.OpenOrderID;
        }

        /// <summary>
        /// Customs the orders list list.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="numberOfDays">The number of days.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <param name="cid">The cid.</param>
        /// <returns></returns>
        public List<OpenOrderCustomModel> CustomOrdersListList(int userId, int numberOfDays, int facilityid, int cid)
        {
            var list = new List<OpenOrderCustomModel>();
            using (var rep = UnitOfWork.OpenOrderRepository)
            {
                var result = rep.CustomOrdersListList(userId, numberOfDays, facilityid, cid).ToList();

                return list;
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
            using (var rep = UnitOfWork.OpenOrderRepository)
            {
                var result = rep.OrdersViewData(physicianId, mostRecentDays, cId, fId, encId, gcCategoryCodes, patientId, opCode, exParam1);
                return result;
            }
        }
        public List<OrderActivityCustomModel> OrderActivitiesByEncounterId(int encounterId)
        {
            using (var rep = UnitOfWork.OrderActivityRepository)
            {
                var result = rep.GetOrderActivitiesByEncounterIdSP(encounterId);
                return result;
            }
        }

        public OrderCustomModel GetPhysicianOrderPlusActivityList(int encounterId, string orderStatus = "", long categoryId = 0, bool withActivities = false)
        {
            using (var rep = UnitOfWork.OpenOrderRepository)
            {
                var result = rep.GetPhysicianOrderPlusActivityList(encounterId, orderStatus, categoryId, withActivities);
                return result;
            }
        }



        /// <summary>
        /// This function does the following: 
        ///   1. Add / updates order details
        ///   2. Schedules the order activity in case of New Order
        ///   3. Applies Order Activities to Bill in case of Administering the order activity.
        ///   4. Gets the result sets required to show after Current Encounters' Orders are updated.
        /// </summary>
        /// <param name="vm">object of the OpenOrderCustomModel</param>
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
        public OrderCustomModel SavePhysicianOrder(OpenOrderCustomModel vm, int physicianId, int mostRecentDays, int cId, int fId, int encId, string gcCategoryCodes, int patientId, string opCode, string exParam1 = "")
        {
            var model = OpenOrderMapper.MapViewModelToModel(vm);
            var days = (Convert.ToDateTime(vm.EndDate) - Convert.ToDateTime(vm.StartDate)).TotalDays;

            var periodDays = days + 1;
            model.PeriodDays = Convert.ToString(periodDays);

            model.IsActivitySchecduled = (model.OrderStatus == Convert.ToString((int)OrderStatus.Closed));
            if (model.OpenOrderID > 0)
            {
                model.ModifiedBy = physicianId;
                model.PhysicianID = physicianId;
            }
            else
            {
                model.CreatedBy = physicianId;
                model.PhysicianID = physicianId;
                model.ActivitySchecduledOn = null;
                model.ModifiedBy = 0;
                model.IsApproved = (vm.CategoryId != (int)OrderTypeCategory.Pharmacy);
            }
            using (var rep = UnitOfWork.OpenOrderRepository)
            {
                var result = rep.SavePhysicianOrder(model, physicianId, mostRecentDays, cId, fId, encId, gcCategoryCodes, patientId, opCode, exParam1);
                return result;
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
            using (var rep = UnitOfWork.OpenOrderRepository)
            {
                var result = rep.GetPhysicianOrNurseTabData(encId, patientId, physicianId, notesUserType);
                return result;
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
            using (var rep = UnitOfWork.OpenOrderRepository)
            {
                var result = rep.GetPatientSummaryDataOnLoad(encId, patientId);
                return result;
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
            using (var rep = UnitOfWork.OpenOrderRepository)
            {
                var result = rep.ApprovePharmacyOrder(id, type, comment, encounterId, withActivities, categoryId, physicianId, corporateId, facilityId);
                return result;
            }
        }


        public OrderCustomModel GetOrdersAndActivitiesByEncounter(long encounterId)
        {
            using (var rep = UnitOfWork.OpenOrderRepository)
            {
                var result = rep.GetOrdersAndActivities(encounterId);
                return result;
            }
        }
        public List<OrderActivityCustomModel> GetOrderActivitiesByOpenOrder(long openOrderId)
        {
            using (var rep = UnitOfWork.OpenOrderRepository)
            {
                var result = rep.GetOrderActivitiesByOpenOrder(openOrderId);
                return result;
            }
        }
    }
}
