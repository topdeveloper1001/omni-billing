using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using AutoMapper;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;


namespace BillingSystem.Bal.BusinessAccess
{
    public class OrderActivityService : IOrderActivityService
    {
        private readonly IRepository<OrderActivity> _repository;
        private readonly IRepository<OpenOrder> _oRepository;
        private readonly IRepository<GlobalCodes> _gRepository;
        private readonly BillingEntities _context;
        private readonly IRepository<GlobalCodeCategory> _ggRepository;
        private readonly IRepository<CPTCodes> _cRepository;
        private readonly IRepository<DRGCodes> _dRepository;
        private readonly IRepository<HCPCSCodes> _hcRepository;
        private readonly IRepository<Drug> _drugRepository;
        private readonly IRepository<ServiceCode> _sRepository;
        private readonly IRepository<DiagnosisCode> _dcRepository;
        private readonly IRepository<Facility> _fRepository;
        private readonly IMapper _mapper;

        public OrderActivityService(IRepository<OrderActivity> repository, IRepository<OpenOrder> oRepository, IRepository<GlobalCodes> gRepository, BillingEntities context, IRepository<GlobalCodeCategory> ggRepository, IRepository<CPTCodes> cRepository, IRepository<DRGCodes> dRepository, IRepository<HCPCSCodes> hcRepository, IRepository<Drug> drugRepository, IRepository<ServiceCode> sRepository, IRepository<DiagnosisCode> dcRepository, IRepository<Facility> fRepository, IMapper mapper)
        {
            _repository = repository;
            _oRepository = oRepository;
            _gRepository = gRepository;
            _context = context;
            _ggRepository = ggRepository;
            _cRepository = cRepository;
            _dRepository = dRepository;
            _hcRepository = hcRepository;
            _drugRepository = drugRepository;
            _sRepository = sRepository;
            _dcRepository = dcRepository;
            _fRepository = fRepository;
            _mapper = mapper;
        }

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
            if (orderActivity.OrderActivityID > 0)
                _repository.UpdateEntity(orderActivity, orderActivity.OrderActivityID);
            else
                _repository.Create(orderActivity);

            return orderActivity.OrderActivityID;
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
                for (int index = 0; index < orderActivity.Count(); index++)
                {
                    OrderActivity openOrderactivity = orderActivity[index];
                    _repository.Create(openOrderactivity);
                    result[index] = Convert.ToInt32(openOrderactivity.OrderActivityID);
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
            bool result = ApplyOrderActivityToBillRep(encounterId, corporateId, facilityId, reclaimFlag, claimId);
            return result;
        }
        private bool ApplyOrderActivityToBillRep(int encounterId, int corporateId, int facilityId, string reclaimFlag, Int64 claimid)
        {
            try
            {
                var sqlParameters = new SqlParameter[5];
                sqlParameters[0] = new SqlParameter("pCorporateID", corporateId);
                sqlParameters[1] = new SqlParameter("pFacilityID", facilityId);
                sqlParameters[2] = new SqlParameter("pEncounterID", encounterId);
                sqlParameters[3] = new SqlParameter("pReClaimFlag", reclaimFlag);
                sqlParameters[4] = new SqlParameter("pClaimId", claimid);
                _repository.ExecuteCommand(StoredProcedures.SPROC_ApplyOrderActivityToBill.ToString(), sqlParameters);
                return true;

            }
            catch (Exception ex)
            {
                return false;
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
            List<LabOrderActivityResultStatus> lstOrderActivity = GetLabResultStatusString(Convert.ToInt32(ordercode.Trim()), Convert.ToDecimal(resultminvalue), Convert.ToInt32(patientId));
            LabOrderActivityResultStatus labresultStatus = lstOrderActivity.FirstOrDefault();
            return labresultStatus != null
                       ? !string.IsNullOrEmpty(labresultStatus.LabTestStatusFlag)
                             ? labresultStatus.LabTestStatusFlag
                             : string.Empty
                       : string.Empty;

        }
        public List<LabOrderActivityResultStatus> GetLabResultStatusString(int ordercode, decimal resultminvalue, int patientId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pValueRecorded, @pCode, @pPID",
                        StoredProcedures.SPROC_GetLabTestResultStatus);
                    var sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("pValueRecorded", resultminvalue);
                    sqlParameters[1] = new SqlParameter("pCode", ordercode);
                    sqlParameters[2] = new SqlParameter("pPID", patientId);
                    IEnumerable<LabOrderActivityResultStatus> result = _context.Database.SqlQuery<LabOrderActivityResultStatus>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
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
                List<LabOrderActivityResultStatus> lstOrderActivity = GetLabResultStatusString(Convert.ToInt32(ordercode.Trim()), Convert.ToDecimal(resultminvalue), Convert.ToInt32(patientId));
                LabOrderActivityResultStatus labresultStatus = lstOrderActivity.FirstOrDefault();
                return labresultStatus != null
                           ? !string.IsNullOrEmpty(labresultStatus.LabUOM)
                                 ? Convert.ToInt32(labresultStatus.LabUOM)
                                 : 0
                           : 0;
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
        public List<OrderActivityCustomModel> GetLabOrderActivitiesByPhysician(int userId, int status, string orderCategoryId, int isActiveEncountersOnly, int encounterId)
        {
            var spName = string.Format("EXEC {0} @UserId, @OrderActivityStatus, @OrderCategory, @IsActiveEncountersOnly, @EncounterId",
                        StoredProcedures.SPROC_GetActiveLabOrdersByPhysicianId);
            var sqlParameters = new SqlParameter[5];
            sqlParameters[0] = new SqlParameter("UserId", userId);
            sqlParameters[1] = new SqlParameter("OrderActivityStatus", status);
            sqlParameters[2] = new SqlParameter("OrderCategory", orderCategoryId);
            sqlParameters[3] = new SqlParameter("IsActiveEncountersOnly", isActiveEncountersOnly);
            sqlParameters[4] = new SqlParameter("EncounterId", encounterId);
            var result = _context.Database.SqlQuery<OrderActivityCustomModel>(spName, sqlParameters);
            return result.ToList();
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
        public OrderActivityCustomModel GetLabOrderActivityByActivityId(int activityId, string CptTableNumber, string DrgTableNumber, string HcpcsTableNumber, string DrugTableNumber,
       string ServiceCodeTableNumber, string DiagnosisTableNumber)
        {
            try
            {
                var orderactivityObj = new List<OrderActivityCustomModel>();

                List<OrderActivity> lstOrderActivity = _repository.Where(a => a.IsActive == null || (bool)a.IsActive && a.OrderActivityID == activityId).ToList();
                orderactivityObj.AddRange(lstOrderActivity.Select(item => new OrderActivityCustomModel
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
                                  ?? CalculateLabResultUOMType(
                                      item.OrderCode,
                                      item.ResultValueMin,
                                      item.PatientID),
                    ResultUOM =
                                  item.ResultUOM
                                  ?? CalculateLabResultUOMType(
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
                                  GetGlobalCategoryNameById(
                                      Convert.ToString(item.OrderCategoryID)),
                    SubCategoryName =
                                  GetNameByGlobalCodeId(
                                      Convert.ToInt32(item.OrderSubCategoryID)),
                    OrderDescription =
                                  GetCodeDescription(
                                      item.OrderCode,
                                      item.OrderType.ToString(), CptTableNumber, DrgTableNumber, HcpcsTableNumber, DrugTableNumber, ServiceCodeTableNumber, DiagnosisTableNumber),
                    Status =
                                  GetNameByGlobalCodeValue(
                                      Convert.ToString(item.OrderActivityStatus),
                                      Convert.ToInt32(
                                          GlobalCodeCategoryValue.ActivityStatus)
                                  .ToString()),
                    OrderTypeName =
                                  GetNameByGlobalCodeValue(
                                      item.OrderType.ToString(),
                                      Convert.ToInt32(GlobalCodeCategoryValue.CodeTypes)
                                  .ToString()),
                    ShowEditAction =
                                  DateTime.Compare(
                                      GetInvariantCultureDateTime(
                                          Convert.ToInt32(item.FacilityID)),
                                      Convert.ToDateTime(item.OrderScheduleDate)) > 0,
                    ResultUOMStr =
                                  item.ResultUOM != null
                                      ? GetNameByGlobalCodeValue(
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
                                      ? CalculateLabResultType(
                                          item.OrderCode,
                                          item.ResultValueMin,
                                          item.PatientID)
                                      : string.Empty,
                    SpecimenTypeStr =
                                  CalculateLabResultSpecimanType(
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
            catch (Exception ex)
            {
                throw ex;
            }
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
        private string GetNameByGlobalCodeId(int id)
        {
            var gl = _gRepository.Where(g => g.GlobalCodeID == id).FirstOrDefault();
            return gl != null ? gl.GlobalCodeName : string.Empty;
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
        public List<OrderActivityCustomModel> GetOrderActivitiesByEncounterId(int encounterId, string CptTableNumber, string DrgTableNumber, string HcpcsTableNumber, string DrugTableNumber,
       string ServiceCodeTableNumber, string DiagnosisTableNumber)
        {
            try
            {
                var orderactivityObj = new List<OrderActivityCustomModel>();
                List<OrderActivity> lstOrderActivity =
                    _repository.Where(
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
                                    ?? CalculateLabResultUOMType(
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
                                    GetGlobalCategoryNameById(
                                        Convert.ToString(item.OrderCategoryID)),
                            SubCategoryName =
                                    GetNameByGlobalCodeId(
                                        Convert.ToInt32(item.OrderSubCategoryID)),
                            OrderDescription =
                                    GetCodeDescription(
                                        item.OrderCode,
                                        item.OrderType.ToString(), CptTableNumber, DrgTableNumber, HcpcsTableNumber, DrugTableNumber, ServiceCodeTableNumber, DiagnosisTableNumber),
                            Status =
                                    GetNameByGlobalCodeValue(
                                        Convert.ToString(item.OrderActivityStatus),
                                        Convert.ToInt32(
                                            GlobalCodeCategoryValue.ActivityStatus)
                                    .ToString()),
                            OrderTypeName =
                                    GetNameByGlobalCodeValue(
                                        item.OrderType.ToString(),
                                        Convert.ToInt32(GlobalCodeCategoryValue.CodeTypes)
                                    .ToString()),
                            ShowEditAction =
                                    DateTime.Compare(
                                       GetInvariantCultureDateTime(Convert.ToInt32(item.FacilityID)),
                                        Convert.ToDateTime(item.OrderScheduleDate)) > 0,
                            ResultUOMStr =
                                    item.ResultUOM != null
                                        ? GetNameByGlobalCodeValue(
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
                                        ? CalculateLabResultType(
                                            item.OrderCode,
                                            item.ResultValueMin,
                                            item.PatientID)
                                        : string.Empty,
                            SpecimenTypeStr =
                                    CalculateLabResultSpecimanType(
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
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private string CalculateLabResultSpecimanType(string ordercode, decimal? resultminvalue, int? patientId)
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
            List<OrderActivity> lstOrderActivity =
                _repository.Where(a => a.IsActive == null || (bool)a.IsActive && a.OrderID == ordersId)
                    .ToList();
            return lstOrderActivity;
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
        public List<OrderActivityCustomModel> GetOrderActivitiesByPatientId(int? patientId, string CptTableNumber, string DrgTableNumber, string HcpcsTableNumber, string DrugTableNumber,
       string ServiceCodeTableNumber, string DiagnosisTableNumber)
        {
            try
            {
                var orderactivityObj = new List<OrderActivityCustomModel>();
                List<OrderActivity> lstOrderActivity =
                    _repository.Where(a => a.IsActive == null || (bool)a.IsActive && a.PatientID == patientId)
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
                                    ?? CalculateLabResultUOMType(
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
                                    GetGlobalCategoryNameById(
                                        Convert.ToString(item.OrderCategoryID)),
                            SubCategoryName =
                                    GetNameByGlobalCodeId(
                                        Convert.ToInt32(item.OrderSubCategoryID)),
                            OrderDescription =
                                    GetCodeDescription(
                                        item.OrderCode,
                                        item.OrderType.ToString(), CptTableNumber, DrgTableNumber, HcpcsTableNumber, DrugTableNumber, ServiceCodeTableNumber, DiagnosisTableNumber),
                            Status =
                                    GetNameByGlobalCodeValue(
                                        Convert.ToString(item.OrderActivityStatus),
                                        Convert.ToInt32(
                                            GlobalCodeCategoryValue.ActivityStatus)
                                    .ToString()),
                            OrderTypeName =
                                    GetNameByGlobalCodeValue(
                                        item.OrderType.ToString(),
                                        Convert.ToInt32(GlobalCodeCategoryValue.CodeTypes)
                                    .ToString()),
                            ShowEditAction =
                                    DateTime.Compare(
                                        GetInvariantCultureDateTime(
                                            Convert.ToInt32(item.FacilityID)),
                                        Convert.ToDateTime(item.OrderScheduleDate)) > 0,
                            ResultUOMStr =
                                    item.ResultUOM != null
                                        ? GetNameByGlobalCodeValue(
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
                                        ? CalculateLabResultType(
                                            item.OrderCode,
                                            item.ResultValueMin,
                                            item.PatientID)
                                        : string.Empty,
                            SpecimenTypeStr =
                                    CalculateLabResultSpecimanType(
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
            List<OrderActivity> lstOrderActivity =
                _repository.Where(a => a.IsActive == null || !(bool)a.IsActive).ToList();
            return lstOrderActivity;
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
            var orderActivity = _repository.Where(x => x.OrderActivityID == OrderActivityId).FirstOrDefault();
            return orderActivity;
        }
        public OrderActivityCustomModel GetOrderActivityByIDVM(int orderActivityId)
        {
            var spName = string.Format("EXEC {0} @pOrderActivityId",
                         StoredProcedures.SPROC_GetOrderActivityDetailsByOrderActivityID);
            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("pOrderActivityId", orderActivityId);
            var result = _context.Database.SqlQuery<OrderActivityCustomModel>(spName, sqlParameters);
            return result.FirstOrDefault();
        }

        /// <summary>
        ///     Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<OrderActivityCustomModel> GetOrderActivityCustom(string CptTableNumber, string DrgTableNumber, string HcpcsTableNumber, string DrugTableNumber,
       string ServiceCodeTableNumber, string DiagnosisTableNumber)
        {
            try
            {
                var orderactivityObj = new List<OrderActivityCustomModel>();
                List<OrderActivity> lstOrderActivity =
                    _repository.Where(a => a.IsActive == null || !(bool)a.IsActive).ToList();
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
                                    GetGlobalCategoryNameById(
                                        Convert.ToString(item.OrderCategoryID)),
                                SubCategoryName =
                                    GetNameByGlobalCodeId(
                                        Convert.ToInt32(item.OrderSubCategoryID)),
                                OrderDescription =
                                    GetCodeDescription(
                                        item.OrderCode,
                                        item.OrderType.ToString(), CptTableNumber, DrgTableNumber, HcpcsTableNumber, DrugTableNumber, ServiceCodeTableNumber, DiagnosisTableNumber),
                                Status =
                                    GetNameByGlobalCodeValue(
                                        Convert.ToString(item.OrderActivityStatus),
                                        Convert.ToInt32(
                                            GlobalCodeCategoryValue.ActivityStatus)
                                            .ToString())
                            }));
                return orderactivityObj;
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
                var listFromSp = GetOrderActivitiesWithPcareActvities(encounterId, 0, "1,2,3,4,9", 0, 1);
                orderactivityObj.AddRange(listFromSp.Select(item => _mapper.Map<OrderActivityCustomModel>(item)));
                return orderactivityObj.OrderBy(x => x.OrderScheduleDate).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private List<OrderActivityCustomModel> GetOrderActivitiesWithPcareActvities(int pEncounterId, int pCategoryId, string pStatus, int pPatientId, int pFlag)
        {
            var spName = string.Format(
                "EXEC {0} @pEncounterId, @pCategoryId, @pStatus, @pPatientId, @pFlag",
                StoredProcedures.SPROC_GetOrderTypeActivity);
            var sqlParameters = new SqlParameter[5];
            sqlParameters[0] = new SqlParameter("pEncounterId", pEncounterId);
            sqlParameters[1] = new SqlParameter("pCategoryId", pCategoryId);
            sqlParameters[2] = new SqlParameter("pStatus", pStatus);
            sqlParameters[3] = new SqlParameter("pPatientId", pPatientId);
            sqlParameters[4] = new SqlParameter("pFlag", pFlag);
            var result = _context.Database.SqlQuery<OrderActivityCustomModel>(spName, sqlParameters);
            return result.ToList();
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
                var listFromSp = GetOrderActivitiesWithPcareActvities(encounterId, 0, "3,4,9", 0, 1);
                orderactivityObj.AddRange(
                    listFromSp.Select(item => _mapper.Map<OrderActivityCustomModel>(item)));
                return orderactivityObj.OrderBy(x => x.ExecutedDate).ToList();
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
                var sqlParameters = new SqlParameter[3];
                sqlParameters[0] = new SqlParameter("pActivityId", activityid);
                sqlParameters[1] = new SqlParameter("pOrderActivityQuantity", quantity);
                sqlParameters[2] = new SqlParameter("pActvityStatus", actvityStatus);
                _repository.ExecuteCommand(StoredProcedures.SPROC_CreatePartialOrderActvities.ToString(), sqlParameters);
                return true;
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
                var orderactvityObj = GetOrderActivityByID(orderActivityId);
                orderactvityObj.OrderActivityStatus = Convert.ToInt32(OpenOrderActivityStatus.Cancel);
                orderactvityObj.ExecutedQuantity = orderactvityObj.OrderActivityQuantity;
                orderactvityObj.ExecutedDate = GetInvariantCultureDateTime(Convert.ToInt32(orderactvityObj.FacilityID));
                _repository.UpdateEntity(orderactvityObj, orderactvityObj.OrderActivityID);

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
                    var changesTobeDone = UpdateOpenOrderStatus(
                        Convert.ToInt32(orderactvityObj.OrderID),
                        Convert.ToString((int)OrderStatus.OnBill),
                        Convert.ToInt32(orderactvityObj.ExecutedBy),
                        GetInvariantCultureDateTime(Convert.ToInt32(orderactvityObj.FacilityID)));
                }
                // Changes End
                return orderactvityObj.EncounterID.HasValue ? orderactvityObj.EncounterID.Value : 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        private int UpdateOpenOrderStatus(int orderId, string status, int userId, DateTime modifiedDate)
        {
            var updateStatus = 0;
            if (orderId > 0)
            {
                var model = _oRepository.GetSingle(orderId);
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
                    _oRepository.UpdateEntity(model, orderId);
                    updateStatus = orderId;
                }
                else
                    updateStatus = -1;
            }

            return updateStatus;
        }


        private DateTime GetInvariantCultureDateTime(int facilityid)
        {
            var facilityObj = _fRepository.Where(f => f.FacilityId == Convert.ToInt32(facilityid)).FirstOrDefault() != null ? _fRepository.Where(f => f.FacilityId == Convert.ToInt32(facilityid)).FirstOrDefault().FacilityTimeZone : TimeZoneInfo.Utc.ToString();
            var tzi = TimeZoneInfo.FindSystemTimeZoneById(facilityObj);
            var utcTime = DateTime.Now.ToUniversalTime();
            var convertedTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
            return convertedTime;
        }

        /// <summary>
        /// Get MAR View detail
        /// </summary>
        /// <param name="oMarViewCustomModel"></param>
        /// <returns></returns>
        public List<MarViewCustomModel> GetMARView(MarViewCustomModel oMarViewCustomModel)
        {
            string spName =
                       string.Format(
                           "EXEC {0} @PID, @EID, @FromDate, @TillDate, @DisplayFlag",
                           StoredProcedures.SPROC_MARView_V1);
            var sqlParameters = new SqlParameter[5];
            sqlParameters[0] = new SqlParameter("PID", oMarViewCustomModel.PatientId);
            sqlParameters[1] = new SqlParameter("EID", oMarViewCustomModel.EncounterId);
            sqlParameters[2] = new SqlParameter("FromDate", oMarViewCustomModel.FromDate);
            sqlParameters[3] = new SqlParameter("TillDate", oMarViewCustomModel.TillDate);
            sqlParameters[4] = new SqlParameter("DisplayFlag", oMarViewCustomModel.DisplayFlag);
            IEnumerable<MarViewCustomModel> result = _context.Database.SqlQuery<MarViewCustomModel>(
                spName,
                sqlParameters);
            return result.ToList();
        }

        /// <summary>
        /// Gets the order activities by encounter identifier sp.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public List<OrderActivityCustomModel> GetOrderActivitiesByEncounterIdSP(int encounterId)
        {
            var spName = string.Format("EXEC {0} @pEncounterId",
                       StoredProcedures.SPROC_GetOrderActivitiesByEncounterId);
            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("pEncounterId", encounterId);
            IEnumerable<OrderActivityCustomModel> result = _context.Database.SqlQuery<OrderActivityCustomModel>(spName, sqlParameters);
            return result.ToList();
        }

        public GenerateBarCode GetBarCodeDetails(int orderActivityId)
        {
            var spName = string.Format("EXEC {0} @pOrderActivityId",
                       StoredProcedures.SPROC_GetBarCodeDetailsByOrderActivityID);
            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("pOrderActivityId", orderActivityId);
            IEnumerable<GenerateBarCode> result = _context.Database.SqlQuery<GenerateBarCode>(spName, sqlParameters);

            var barCode = result.FirstOrDefault();
            return barCode != null && !string.IsNullOrEmpty(barCode.BarCodeReadValue)
                    ? barCode
                    : new GenerateBarCode();
        }
        #endregion

        /// <summary>
        /// Pharamacies the order activity administered.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns></returns>
        public bool PharamacyOrderActivityAdministered(int orderId)
        {
            var orderactivityObj = _repository.Where(x => x.OrderID == orderId && x.OrderType == 5 && x.OrderActivityStatus == 4).ToList();

            return orderactivityObj.Any();

        }
    }
}