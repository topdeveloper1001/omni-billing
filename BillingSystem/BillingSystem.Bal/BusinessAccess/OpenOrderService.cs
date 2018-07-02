using System.Linq;
using BillingSystem.Model;
using System.Collections.Generic;
using BillingSystem.Model.CustomModel;
using BillingSystem.Common.Common;
using System;

using System.Data.SqlClient;
using AutoMapper;

using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class OpenOrderService : IOpenOrderService
    {
        private readonly IRepository<Encounter> _eRepository;
        private readonly IRepository<GlobalCodes> _gRepository;
        private readonly IRepository<OpenOrder> _repository;
        private readonly IRepository<BillHeader> _bhRepository;
        private readonly IRepository<CPTCodes> _cRepository;
        private readonly IRepository<DRGCodes> _dRepository;
        private readonly IRepository<HCPCSCodes> _hcRepository;
        private readonly IRepository<Drug> _drugRepository;
        private readonly IRepository<ServiceCode> _sRepository;
        private readonly IRepository<DiagnosisCode> _dcRepository;
        private readonly IRepository<GlobalCodeCategory> _ggRepository;
        private readonly IRepository<Diagnosis> _diaRepository;
        private readonly IRepository<UserDefinedDescriptions> _favRepository;
        private readonly IRepository<OrderActivity> _orRepository;

        private readonly BillingEntities _context;
        private readonly IMapper _mapper;

        public OpenOrderService(IRepository<Encounter> eRepository, IRepository<GlobalCodes> gRepository, IRepository<OpenOrder> repository, IRepository<BillHeader> bhRepository, IRepository<CPTCodes> cRepository, IRepository<DRGCodes> dRepository, IRepository<HCPCSCodes> hcRepository, IRepository<Drug> drugRepository, IRepository<ServiceCode> sRepository, IRepository<DiagnosisCode> dcRepository, IRepository<GlobalCodeCategory> ggRepository, IRepository<Diagnosis> diaRepository, IRepository<UserDefinedDescriptions> favRepository, IRepository<OrderActivity> orRepository, BillingEntities context, IMapper mapper)
        {
            _eRepository = eRepository;
            _gRepository = gRepository;
            _repository = repository;
            _bhRepository = bhRepository;
            _cRepository = cRepository;
            _dRepository = dRepository;
            _hcRepository = hcRepository;
            _drugRepository = drugRepository;
            _sRepository = sRepository;
            _dcRepository = dcRepository;
            _ggRepository = ggRepository;
            _diaRepository = diaRepository;
            _favRepository = favRepository;
            _orRepository = orRepository;
            _context = context;
            _mapper = mapper;
        }



        /// <summary>
        /// Get the Physician Orders
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <param name="orderStatus">The order status.</param>
        /// <returns>
        /// Return the Encounter Order list
        /// </returns>
        public List<OpenOrderCustomModel> GetPhysicianOrders(int encounterId, string orderStatus, string CptTableNumber, string DrgTableNumber, string HcpcsTableNumber, string DrugTableNumber,
       string ServiceCodeTableNumber, string DiagnosisTableNumber, string fId = "")
        {
            var status = OrderStatus.Open.ToString();
            var list = new List<OpenOrderCustomModel>();

            //Open And Open Administered Status
            var openOrders = status == orderStatus
                ? _repository.Where(
                    a => a.EncounterID == encounterId && (a.OrderStatus == ("1")))
                    .OrderByDescending(x => x.OpenOrderID)
                    .ToList()
                : _repository.Where(
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
                        OrderDescription = GetCodeDescription(item.OrderCode, item.OrderType, CptTableNumber, DrgTableNumber, HcpcsTableNumber, DrugTableNumber, ServiceCodeTableNumber, DiagnosisTableNumber),
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
        public string CalculateLabResultSpecimanType(string ordercode, decimal? resultminvalue, int? patientId)
        {
            try
            {
                var spName = string.Format("EXEC {0} @pValueRecorded, @pCode, @pPID",
                         StoredProcedures.SPROC_GetLabTestResultStatus);
                var sqlParameters = new SqlParameter[3];
                sqlParameters[0] = new SqlParameter("pValueRecorded", resultminvalue);
                sqlParameters[1] = new SqlParameter("pCode", ordercode.Trim());
                sqlParameters[2] = new SqlParameter("pPID", patientId);
                IEnumerable<LabOrderActivityResultStatus> result = _context.Database.SqlQuery<LabOrderActivityResultStatus>(spName, sqlParameters);


                var lstOrderActivity = result.ToList(); ;
                var labresultStatus = lstOrderActivity.FirstOrDefault();
                return labresultStatus != null
                           ? !string.IsNullOrEmpty(labresultStatus.LabTestSpecimanStr)
                                 ? labresultStatus.LabTestSpecimanStr
                                 : string.Empty
                           : string.Empty;

            }
            catch (Exception)
            {
            }
            return string.Empty;
        }

        private string GetDiagnoseNameByCodeId(string diagnosisId)
        {
            var id = Convert.ToInt32(diagnosisId);
            var diagnosis = _diaRepository.Where(d => d.DiagnosisID == id).FirstOrDefault();
            return diagnosis != null ? diagnosis.DiagnosisCodeDescription : string.Empty;
        }
        private string GetGlobalCategoryNameById(string categoryValue, string facilityId = "")
        {
            if (!string.IsNullOrEmpty(categoryValue))
            {
                var category = _ggRepository.Where(g => g.GlobalCodeCategoryValue.Equals(categoryValue) && g.IsDeleted.HasValue ? !g.IsDeleted.Value : false && (string.IsNullOrEmpty(facilityId) || g.FacilityNumber.Equals(facilityId))).FirstOrDefault();
                return category != null ? category.GlobalCodeCategoryName : string.Empty;
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
        private string GetCodeDescription(string orderCode, string orderType, string CptTableNumber, string DrgTableNumber, string HcpcsTableNumber, string DrugTableNumber,
       string ServiceCodeTableNumber, string DiagnosisTableNumber)
        {
            var codeDescription = string.Empty;

            if (!string.IsNullOrEmpty(orderCode) && !string.IsNullOrEmpty(orderType))
            {
                var codeType = (OrderType)Enum.Parse(typeof(OrderType), orderType);
                switch (codeType)
                {
                    case OrderType.CPT:
                        codeDescription = _cRepository.Where(x => x.CodeNumbering.Contains(orderCode) && x.CodeTableNumber.Trim().Equals(CptTableNumber)).FirstOrDefault().CodeDescription;
                        return codeDescription;
                    case OrderType.DRG:
                        codeDescription = _dRepository.Where(d => d.CodeNumbering == orderCode && d.CodeTableNumber.Trim().Equals(DrgTableNumber)).FirstOrDefault().CodeDescription;
                        return codeDescription;
                    case OrderType.HCPCS:
                        codeDescription = _hcRepository.Where(x => x.CodeNumbering == orderCode && x.CodeTableNumber.Trim().Equals(HcpcsTableNumber)).FirstOrDefault().CodeDescription;
                        return codeDescription;
                    case OrderType.DRUG:
                        codeDescription = _drugRepository.Where(x => x.DrugCode == orderCode && x.DrugTableNumber.Trim().Equals(DrugTableNumber)).FirstOrDefault().DrugDescription;
                        return codeDescription;
                    case OrderType.BedCharges:
                        codeDescription = _sRepository.Where(s => s.ServiceCodeValue.Equals(orderCode) && s.ServiceCodeTableNumber.Trim().Equals(ServiceCodeTableNumber)).FirstOrDefault().ServiceCodeDescription;
                        return codeDescription;
                    case OrderType.Diagnosis:
                        codeDescription = _dcRepository.Where(d => d.DiagnosisCode1 == orderCode && d.DiagnosisTableNumber.Trim().Equals(DiagnosisTableNumber)).FirstOrDefault().DiagnosisFullDescription;
                        return codeDescription;
                    case OrderType.ServiceCode:
                        codeDescription = _sRepository.Where(s => s.ServiceCodeValue.Equals(orderCode) && s.ServiceCodeTableNumber.Trim().Equals(ServiceCodeTableNumber)).FirstOrDefault().ServiceCodeDescription;
                        return codeDescription;
                    default:
                        break;
                }
            }
            return codeDescription;
        }
        /// <summary>
        /// Get the ordr detail by order id
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public OpenOrder GetOpenOrderDetail(int id)
        {
            var iQueryableOrder = id > 0
                ? _repository.GetAll().FirstOrDefault(o => o.OpenOrderID == id)
                : new OpenOrder();
            return iQueryableOrder;
        }

        /// <summary>
        /// Add the physiccian order to DB
        /// </summary>
        /// <param name="model">The order.</param>
        /// <returns></returns>
        public int AddUpdatePhysicianOpenOrder(OpenOrder model)
        {
            var updateStatus = 0;
            if (model.OpenOrderID > 0)
                ChangeOrderDetail(model);
            else
                _repository.Create(model);


            if (model.OrderStatus != Convert.ToString((int)OrderStatus.Open))
            {
                ApplyOrderBill(model.EncounterID);
            }
            return model.OpenOrderID;
        }
        private int ChangeOrderDetail(OpenOrder m)
        {
            var sqlParameters = new SqlParameter[14];
            sqlParameters[0] = new SqlParameter("pPatientId", m.PatientID);
            sqlParameters[1] = new SqlParameter("pOrderCode", m.OrderCode);
            sqlParameters[2] = new SqlParameter("pOrderType", m.OrderType);
            sqlParameters[3] = new SqlParameter("pEncounterId", m.EncounterID);
            sqlParameters[4] = new SqlParameter("pOrderStatus", m.OrderStatus);
            sqlParameters[5] = new SqlParameter("pOrderFrequency", m.FrequencyCode);
            sqlParameters[6] = new SqlParameter("pOrderStartDate", m.StartDate);
            sqlParameters[7] = new SqlParameter("pOrderEndDate", m.EndDate);
            sqlParameters[8] = new SqlParameter("pSchQuantity", m.Quantity);
            sqlParameters[9] = new SqlParameter("pPhysicianId", m.PhysicianID);
            sqlParameters[10] = new SqlParameter("pOpenOrderId", m.OpenOrderID);
            sqlParameters[11] = string.IsNullOrEmpty(m.OrderNotes)
                                    ? new SqlParameter("pOrderNotes", DBNull.Value)
                                    : new SqlParameter("pOrderNotes", m.OrderNotes);
            sqlParameters[12] = new SqlParameter("pOrderCategory", m.CategoryId);
            sqlParameters[13] = new SqlParameter("pOrderSubCategory", m.SubCategoryId);
            _repository.ExecuteCommand(StoredProcedures.SPROC_ChangeCurrentOrderDetail.ToString(), sqlParameters);
            return m.OpenOrderID;
        }
        private bool ApplyOrderBill(int? encounterId)
        {
            try
            {
                var sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter("pEncounuterID", encounterId);
                _bhRepository.ExecuteCommand(StoredProcedures.SPROC_ApplyOrderToBill.ToString(), sqlParameters);
            }
            catch (Exception)
            {
                //throw ex;
            }
            return false;
        }

        /// <summary>
        /// Get the Physician Orders of Current Patient
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns>
        /// Return the Encounter Order list
        /// </returns>
        public List<OpenOrderCustomModel> GetOrdersByPatientId(int patientId, string CptTableNumber, string DrgTableNumber, string HcpcsTableNumber, string DrugTableNumber,
       string ServiceCodeTableNumber, string DiagnosisTableNumber)
        {
            var list = new List<OpenOrderCustomModel>();
            var orderList = _repository.GetAll().Where(a => a.PatientID == patientId).ToList();
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
                    OrderDescription = GetCodeDescription(item.OrderCode, item.OrderType, CptTableNumber, DrgTableNumber, HcpcsTableNumber, DrugTableNumber, ServiceCodeTableNumber, DiagnosisTableNumber),
                    Status = GetNameByGlobalCodeValue(item.OrderStatus, Convert.ToInt32(GlobalCodeCategoryValue.OrderStatus).ToString()),
                    CategoryId = item.CategoryId,
                    SubCategoryId = item.SubCategoryId,
                    FrequencyText = GetNameByGlobalCodeValue(item.FrequencyCode, Convert.ToInt32(GlobalCodeCategoryValue.OrderFrequencyType).ToString()),
                    SpecimenTypeStr = CalculateLabResultSpecimanType(item.OrderCode, null, item.PatientID),
                }));
            }
            return list;
        }
        private string GetNameByGlobalCodeId(int id)
        {
            var gl = _gRepository.Where(g => g.GlobalCodeID == id).FirstOrDefault();
            return gl != null ? gl.GlobalCodeName : string.Empty;
        }

        /// <summary>
        /// Gets the encounters list by patient identifier.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public IEnumerable<EncounterCustomModel> GetEncountersListByPatientId(int patientId)
        {
            var list = new List<EncounterCustomModel>();

            var lstEncounter = _eRepository.Where(_ => _.PatientID == patientId).OrderByDescending(x => x.EncounterStartTime).ToList();
            var encountertypeCat = Convert.ToInt32(GlobalCodeCategoryValue.EncounterType);
            var encounterPatienttypeCat = Convert.ToInt32(GlobalCodeCategoryValue.EncounterPatientType);
            list.AddRange(lstEncounter.Select(item => new EncounterCustomModel
            {
                EncounterID = item.EncounterID,
                EncounterNumber = item.EncounterNumber,
                EncounterStartTime = item.EncounterStartTime,
                EncounterEndTime = item.EncounterEndTime,
                Charges = item.Charges,
                Payment = item.Payment,
                PatientID = item.PatientID,
                EncounterTypeName = GetNameByGlobalCodeValueAndCategoryValue(Convert.ToString(encountertypeCat),
                    item.EncounterType.ToString()),
                EncounterPatientTypeName = GetNameByGlobalCodeValueAndCategoryValue(Convert.ToString(encounterPatienttypeCat),
                    item.EncounterPatientType.ToString()),
            }));

            return list;
        }
        private string GetNameByGlobalCodeValueAndCategoryValue(string categoryValue, string globalCodeValue)
        {
            if (!string.IsNullOrEmpty(categoryValue) && !string.IsNullOrEmpty(globalCodeValue))
            {
                var globalCode = _gRepository.Where(c => c.GlobalCodeCategoryValue.Equals(categoryValue)
                && c.GlobalCodeValue.Equals(globalCodeValue)).FirstOrDefault();

                if (globalCode != null)
                    return globalCode.GlobalCodeName;
            }
            return string.Empty;
        }
        private List<GeneralCodesCustomModel> GetAllOrderingCodes(string text, string CptTableNumber, string HcpcsTableNumber, string DrugTableNumber)
        {
            var finalList = new List<GeneralCodesCustomModel>();
            var cptList = _cRepository.Where(_ => _.IsDeleted == false && _.CodeTableNumber.Trim().Equals(CptTableNumber) && _.IsActive != false).ToList();
            finalList.AddRange(
                cptList.Select(
                    item =>
                    new GeneralCodesCustomModel
                    {
                        Code = item.CodeNumbering,
                        Description = item.CodeDescription,
                        CodeDescription =
                                string.Format(
                                    "{0} - {1}",
                                    item.CodeNumbering,
                                    item.CodeDescription),
                        CodeType = Convert.ToString((int)OrderType.CPT),
                        CodeTypeName = "CPT",
                        ExternalCode = item.CTPCodeRangeValue.ToString(),
                        ID = Convert.ToString(item.CPTCodesId)
                    }));


            var hcpcList = _hcRepository.Where(x => x.IsActive == true && x.CodeTableNumber.Trim().Equals(HcpcsTableNumber)).ToList();
            finalList.AddRange(
                    hcpcList.Select(
                        item =>
                        new GeneralCodesCustomModel
                        {
                            Code = item.CodeNumbering,
                            Description = item.CodeDescription,
                            CodeDescription =
                                    string.Format(
                                        "{0} - {1}",
                                        item.CodeNumbering,
                                        item.CodeDescription),
                            CodeType = Convert.ToString((int)OrderType.HCPCS),
                            CodeTypeName = "HCPCS",
                            ExternalCode = string.Empty,
                            ID = Convert.ToString(item.HCPCSCodesId)
                        }));
            var drugList = _drugRepository.Where(d => !d.DrugStatus.Equals("Deleted") && d.DrugTableNumber.Trim().Equals(DrugTableNumber)).OrderByDescending(x => x.Id).ToList();

            finalList.AddRange(
                drugList.Select(
                    item =>
                        new GeneralCodesCustomModel
                        {
                            Code = item.DrugCode,
                            Description = item.DrugDescription,
                            CodeDescription =
                                string.Format(
                                    "{0} - {1} - {2} - {3}",
                                    item.DrugCode.Trim(),
                                    item.DrugGenericName.Trim(),
                                    !string.IsNullOrEmpty(item.DrugStrength)
                                        ? item.DrugStrength.Trim()
                                        : string.Empty,
                                    !string.IsNullOrEmpty(item.DrugDosage) ? item.DrugDosage.Trim() : string.Empty),
                            CodeType = Convert.ToString((int)OrderType.DRUG),
                            CodeTypeName = "DRUG",
                            ExternalCode =
                                String.IsNullOrEmpty(item.BrandCode)
                                    ? "0"
                                    : Convert.ToString(item.BrandCode),
                            ID = Convert.ToString(item.Id),
                            DrugPackageName =
                                !string.IsNullOrEmpty(item.DrugPackageName)
                                    ? item.DrugPackageName.Trim()
                                    : string.Empty
                        }));

            if (finalList.Count > 0)
            {
                text = text.ToLower().Trim();
                finalList =
                    finalList.Where(
                        f => (f.CodeDescription.ToLower().Contains(text) ||
                             (!string.IsNullOrEmpty(f.DrugPackageName) &&
                              f.DrugPackageName.Trim().ToLower().Contains(text)))).ToList();
            }
            return finalList;
        }

        /// <summary>
        /// Gets the favorite orders.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public List<OpenOrderCustomModel> GetFavoriteOrders(int userId, string CptTableNumber, string DrgTableNumber, string HcpcsTableNumber, string DrugTableNumber,
       string ServiceCodeTableNumber, string DiagnosisTableNumber)
        {
            var favOrders = new List<OpenOrderCustomModel>();
            try
            {

                var CPTcategory = Convert.ToInt32(FavoriteCategories.CPT).ToString();
                var DRGcategory = Convert.ToInt32(FavoriteCategories.DRUG).ToString();
                var favorites = _favRepository.Where(f => (f.CategoryId.Equals(CPTcategory) || f.CategoryId.Equals(DRGcategory)) && (f.IsDeleted == null || !(bool)f.IsDeleted) && f.UserID == userId).ToList();
                favOrders.AddRange(from item in favorites
                                   let getOrderCodeparent = GetAllOrderingCodes(item.CodeId, CptTableNumber, HcpcsTableNumber, DrugTableNumber)
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
                                       OrderDescription = GetCodeDescription(item.CodeId, item.CategoryId, CptTableNumber, DrgTableNumber, HcpcsTableNumber, DrugTableNumber, ServiceCodeTableNumber, DiagnosisTableNumber),
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
        public List<OpenOrderCustomModel> GetSearchedOrders(string text, string CptTableNumber, string DrgTableNumber, string HcpcsTableNumber, string DrugTableNumber,
        string ServiceCodeTableNumber, string DiagnosisTableNumber)
        {
            var list = new List<OpenOrderCustomModel>();

            text = text.ToLower();
            var orders = _repository.Where(o => o.OrderNotes.ToLower().Contains(text) || o.DiagnosisCode.ToLower().Contains(text) || o.OrderCode.ToLower().Contains(text)).ToList();
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
                    OrderDescription = GetCodeDescription(item.OrderCode, item.OrderType, CptTableNumber, DrgTableNumber, HcpcsTableNumber, DrugTableNumber, ServiceCodeTableNumber, DiagnosisTableNumber),
                    Status = GetNameByGlobalCodeValue(item.OrderStatus, Convert.ToInt32(GlobalCodeCategoryValue.OrderStatus).ToString()),
                    FrequencyText = GetNameByGlobalCodeValue(item.FrequencyCode, Convert.ToInt32(GlobalCodeCategoryValue.OrderFrequencyType).ToString()),
                    SpecimenTypeStr = CalculateLabResultSpecimanType(item.OrderCode, null, item.PatientID),
                }));
            }
            return list;
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
            var spName = string.Format("EXEC {0} @pPhysicianID, @pCorporateID,@pFacilityID", StoredProcedures.SPROC_GetPhysicianPreviousOrders);
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("pPhysicianID", userId);
            sqlParameters[1] = new SqlParameter("pCorporateID", corporateId);
            sqlParameters[2] = new SqlParameter("pFacilityID", facilityId);
            IEnumerable<OpenOrderCustomModel> result = _context.Database.SqlQuery<OpenOrderCustomModel>(spName, sqlParameters);
            return result.ToList();
        }

        /// <summary>
        /// Gets the most ordered list.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="numberOfDays">The number of days.</param>
        /// <returns></returns>
        public List<OpenOrderCustomModel> GetMostOrderedList(int userId, int numberOfDays)
        {
            var spName = string.Format(
                         "EXEC {0} @pPhysicianID, @pNumberOfDaysBack",
                         StoredProcedures.SPROC_GetMostOrdered);
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("pPhysicianID", userId);
            sqlParameters[1] = new SqlParameter("pNumberOfDaysBack", numberOfDays);
            IEnumerable<OpenOrderCustomModel> result =
                _context.Database.SqlQuery<OpenOrderCustomModel>(spName, sqlParameters);
            return result.ToList();
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
            var spName = string.Format(
                          "EXEC {0} @pPhysicianID, @pCorporateID,@pFacilityID",
                          StoredProcedures.SPROC_GetPhysicianFavoriteOrders);
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("pPhysicianID", userId);
            sqlParameters[1] = new SqlParameter("pCorporateID", corporateid);
            sqlParameters[2] = new SqlParameter("pFacilityID", facilityid);
            IEnumerable<OpenOrderCustomModel> result =
                _context.Database.SqlQuery<OpenOrderCustomModel>(spName, sqlParameters);
            return result.ToList();
        }
        private List<UserDefinedDescriptions> GetFavoriteByPhyId(int phyid)
        {
            var fav = _favRepository.Where(f => f.UserID == phyid && (f.IsDeleted == null || !(bool)f.IsDeleted)).ToList();
            return fav;
        }
        /// <summary>
        /// Gets the orders by physician identifier.
        /// </summary>
        /// <param name="phyId">The phy identifier.</param>
        /// <returns></returns>
        public List<OpenOrderCustomModel> GetOrdersByPhysicianId(int phyId, string CptTableNumber, string DrgTableNumber, string HcpcsTableNumber, string DrugTableNumber,
        string ServiceCodeTableNumber, string DiagnosisTableNumber)
        {
            var list = new List<OpenOrderCustomModel>();
            var orderList = _repository.GetAll().Where(a => a.PhysicianID == phyId).ToList();
            var phyfavList = GetFavoriteByPhyId(phyId);
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
                    OrderDescription = GetCodeDescription(item.OrderCode, item.OrderType, CptTableNumber, DrgTableNumber, HcpcsTableNumber, DrugTableNumber, ServiceCodeTableNumber, DiagnosisTableNumber),
                    Status = GetNameByGlobalCodeValue(item.OrderStatus, Convert.ToInt32(GlobalCodeCategoryValue.OrderStatus).ToString()),
                    FrequencyText = GetNameByGlobalCodeValue(item.FrequencyCode, Convert.ToInt32(GlobalCodeCategoryValue.OrderFrequencyType).ToString()),
                    SpecimenTypeStr = CalculateLabResultSpecimanType(item.OrderCode, null, item.PatientID),
                    //MulitpleOrderActivites = CheckMulitpleOpenActivites(item.OpenOrderID),
                }));
            }
            return list;
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
            var ordersCount = _repository.Where(a => a.PhysicianID == phyId && a.OrderCode == codeid && a.OrderType == ordertype).Count();
            return ordersCount;
        }

        /// <summary>
        /// Gets the order identifier by order code.
        /// </summary>
        /// <param name="phyId">The phy identifier.</param>
        /// <param name="codeid">The codeid.</param>
        /// <returns></returns>
        public int GetOrderIdByOrderCode(int phyId, string codeid)
        {
            var orderObj = _repository.Where(a => a.PhysicianID == phyId && a.OrderCode == codeid).FirstOrDefault();
            return orderObj != null ? orderObj.OpenOrderID : 0;
        }

        /// <summary>
        /// Gets the open orders by encounter identifier.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public List<OpenOrderCustomModel> GetAllOrdersByEncounterId(int encounterId)
        {
            var spName = string.Format("EXEC {0} @pEncId", StoredProcedures.SPROC_GetOrdersByEncounterId);
            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("pEncId", encounterId);
            IEnumerable<OpenOrderCustomModel> result =
                _context.Database.SqlQuery<OpenOrderCustomModel>(spName, sqlParameters);
            return result.OrderByDescending(x => x.EndDate).ToList();
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
                for (int index = 0; index < model.Count; index++)
                {
                    var openOrder = model[index];
                    _repository.Create(openOrder);

                    /*
                     * Who: Amit Jain
                     * When: 03 March, 2016
                     * Why: It calls the Stored Procedure 'SPROC_ApplyOrderToBill' for billing entries in the BillActivity Table. 
                     */

                    if (model[index].OrderStatus != Convert.ToString((int)OrderStatus.Open))
                        ApplyOrderBill(Convert.ToInt32(model[index].EncounterID));

                    result[index] = Convert.ToInt32(openOrder.OpenOrderID);
                }
                //    transScope.Complete();
                //}
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
        public List<OpenOrderCustomModel> GetOrdersByEncounterid(int encounterId, string CptTableNumber, string DrgTableNumber, string HcpcsTableNumber, string DrugTableNumber,
        string ServiceCodeTableNumber, string DiagnosisTableNumber)
        {
            var list = new List<OpenOrderCustomModel>();
            var orders = _repository.Where(p => p.CreatedBy != null && (int)p.EncounterID == encounterId)
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
                OrderDescription = GetCodeDescription(item.OrderCode, item.OrderType, CptTableNumber, DrgTableNumber, HcpcsTableNumber, DrugTableNumber, ServiceCodeTableNumber, DiagnosisTableNumber),
                Status = GetNameByGlobalCodeValue(item.OrderStatus, Convert.ToInt32(GlobalCodeCategoryValue.OrderStatus).ToString()),
                CategoryId = item.CategoryId,
                SubCategoryId = item.SubCategoryId,
                FrequencyText = GetNameByGlobalCodeValue(item.FrequencyCode, Convert.ToInt32(GlobalCodeCategoryValue.OrderFrequencyType).ToString()),
                SpecimenTypeStr = CalculateLabResultSpecimanType(item.OrderCode, null, item.PatientID),
                // MulitpleOrderActivites = CheckMulitpleOpenActivites(item.OpenOrderID),
            }));
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
            var sqlParams = new SqlParameter[3] { new SqlParameter("pFavoriteId", favorderId), new SqlParameter("pFacilityId", facilityId), new SqlParameter("pUserId", userId) };

            using (var ms = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetFavoriteOrderById.ToString(), isCompiled: false, parameters: sqlParams))
            {
                var result = ms.GetSingleResultWithJson<OpenOrder>(JsonResultsArray.FavoriteResult.ToString());
                return result;
            }
        }

        /// <summary>
        /// Checks the mulitple open activites.
        /// </summary>
        /// <param name="openorderid">The openorderid.</param>
        /// <returns></returns>
        public bool CheckMulitpleOpenActivites(int openorderid)
        {
            var orderactiviteslist = GetOrderActivitiesByOrderId(openorderid);
            return orderactiviteslist.Any() && orderactiviteslist.Count > 1;
        }
        private List<OrderActivity> GetOrderActivitiesByOrderId(int ordersId)
        {
            List<OrderActivity> lstOrderActivity = _orRepository.Where(a => a.IsActive == null || (bool)a.IsActive && a.OrderID == ordersId)
                    .ToList();
            return lstOrderActivity;
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
            var spName = string.Format(
                        "EXEC {0} @pPatientId, @pOrderCode,@pEncounterId",
                        StoredProcedures.Sproc_GetPatientAllergyByPharmacyOrderCode);
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("pOrderCode", ordercode);
            sqlParameters[1] = new SqlParameter("pPatientId", patientId);
            sqlParameters[2] = new SqlParameter("pEncounterId", erncounterid);
            IEnumerable<DrugReactionCustomModel> result =
                _context.Database.SqlQuery<DrugReactionCustomModel>(spName, sqlParameters);
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Gets the active encounter identifier.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public int GetActiveEncounterId(int patientId)
        {
            var encounterId = 0;
            var current = _eRepository.Where(e => e.PatientID == patientId && e.EncounterEndTime == null).FirstOrDefault();
            if (current != null)
                encounterId = current.EncounterID;
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
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("pOrderId", id);
            sqlParameters[1] = new SqlParameter("pOrderStatus", type);
            sqlParameters[2] = new SqlParameter("pComment", comment);
            _eRepository.ExecuteCommand(StoredProcedures.SPROC_ApprovePharmacyOrder.ToString(), sqlParameters);
            return true;
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
            var spName = $"EXEC {StoredProcedures.SPROC_GetPhysicianOrders} @pEncounterId, @pOrderStatus, @pCategoryId";

            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("pEncounterId", encounterId);
            sqlParameters[1] = new SqlParameter("pOrderStatus", orderStatus);
            sqlParameters[2] = new SqlParameter("pCategoryId", categoryId);
            IEnumerable<OpenOrderCustomModel> result = _context.Database.SqlQuery<OpenOrderCustomModel>(spName, sqlParameters);
            return result.ToList();
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
            if (orderId > 0)
            {
                var model = _repository.GetSingle(orderId);
                if (model != null)
                {
                    // This needs to be changed as the Order Status can not be always on Bill,
                    // If the Order have any Activites as Closed/OnBill then order should have on Bill Status
                    // If the order have all activites as Cancel/Revoked then order shouldhave Cancel/Revoked Status.
                    // model.OrderStatus = Convert.ToString((int)OrderStatus.OnBill);
                    // Changes done on the 14th March 2016 BY Shashank Awasthy
                    var getOrderactivites = GetOrderActivitiesByOrderId(orderId);
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
                    _repository.UpdateEntity(model, orderId);
                    updateStatus = orderId;
                }
                else
                    updateStatus = -1;
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
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("pOrderId", orderid);
            sqlParameters[1] = new SqlParameter("pUserId", userid);
            _repository.ExecuteCommand(StoredProcedures.SPROC_CancelOpenOrder.ToString(), sqlParameters);
            return true;
        }

        /// <summary>
        /// Adds the update open order custom model.
        /// </summary>
        /// <param name="vm">The model.</param>
        /// <returns></returns>
        public int AddUpdateOpenOrderCustomModel(OpenOrderCustomModel vm)
        {
            var updateStatus = 0;
            var m = _mapper.Map<OpenOrder>(vm);
            if (m.OpenOrderID > 0)
            {
                updateStatus = ChangeOrderDetail(m);
            }
            else
                _repository.Create(m);


            if (m.OrderStatus != Convert.ToString((int)OrderStatus.Open))
            {
                var result = ApplyOrderBill(Convert.ToInt32(m.EncounterID));
            }
            return m.OpenOrderID;
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
            var spName = string.Format(
                        "EXEC {0} @pPhysicianID, @pNumberOfDaysBack,@fid,@cid",
                        StoredProcedures.SPROC_GetOrdersData);
            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter("pPhysicianID", userId);
            sqlParameters[1] = new SqlParameter("pNumberOfDaysBack", numberOfDays);
            sqlParameters[2] = new SqlParameter("fid", facilityid);
            sqlParameters[3] = new SqlParameter("cid", cid);
            IEnumerable<OpenOrdersData> result =
                _context.Database.SqlQuery<OpenOrdersData>(spName, sqlParameters);
            return result.Select(x => _mapper.Map<OpenOrderCustomModel>(x)).ToList();
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
                    vm.PreviousOrders = vm.PreviousOrders.ToList();

                if (vm.OpenOrders.Any())
                    vm.OpenOrders = vm.OpenOrders.OrderByDescending(a => a.EndDate).ToList();

                return vm;
            }
        }
        public List<OrderActivityCustomModel> OrderActivitiesByEncounterId(int encounterId)
        {
            var spName = string.Format("EXEC {0} @pEncounterId",
                      StoredProcedures.SPROC_GetOrderActivitiesByEncounterId);
            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("pEncounterId", encounterId);
            IEnumerable<OrderActivityCustomModel> result = _context.Database.SqlQuery<OrderActivityCustomModel>(spName, sqlParameters);
            return result.ToList();
        }

        public OrderCustomModel GetPhysicianOrderPlusActivityList(int encounterId, string orderStatus = "", long categoryId = 0, bool withActivities = false)
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
        public OrderCustomModel SavePhysicianOrder(OpenOrderCustomModel vm2, int physicianId, int mostRecentDays, int cId, int fId, int encId, string gcCategoryCodes, int patientId, string opCode, string exParam1 = "")
        {
            var model = _mapper.Map<OpenOrder>(vm2);
            var days = (Convert.ToDateTime(vm2.EndDate) - Convert.ToDateTime(vm2.StartDate)).TotalDays;

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
                model.IsApproved = (vm2.CategoryId != (int)OrderTypeCategory.Pharmacy);
            }

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
                    vm.PreviousOrders = vm.PreviousOrders.ToList();

                if (vm.OpenOrders.Any())
                    vm.OpenOrders = vm.OpenOrders.OrderByDescending(a => a.EndDate).ToList();


                return vm;
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

        /// <summary>
        /// This function returns all data related to Physician Tasks /  Nurse Tasks Tab in EHR.
        /// </summary>
        /// <param name="encId">Current Encounter ID</param>
        /// <param name="patientId">Current Patient ID</param>
        /// <param name="physicianId">Current Physician ID</param>
        /// <returns></returns>
        public PatientSummaryTabData GetPatientSummaryDataOnLoad(long encId, long patientId)
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

                return vm;
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


        public OrderCustomModel GetOrdersAndActivitiesByEncounter(long encounterId)
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
