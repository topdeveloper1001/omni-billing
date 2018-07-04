using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using AutoMapper;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model.Model;


namespace BillingSystem.Bal.BusinessAccess
{
    public class FutureOpenOrderService : IFutureOpenOrderService
    {
        private readonly IRepository<FutureOpenOrder> _repository;
        private readonly IRepository<GlobalCodes> _gRepository;
        private readonly IRepository<GlobalCodeCategory> _ggRepository;
        private readonly IRepository<Diagnosis> _diaRepository;
        private readonly IRepository<OrderActivity> _oaRepository;
        private readonly IRepository<CPTCodes> _cRepository;
        private readonly IRepository<DRGCodes> _dRepository;
        private readonly IRepository<HCPCSCodes> _hcRepository;
        private readonly IRepository<Drug> _drugRepository;
        private readonly IRepository<ServiceCode> _sRepository;
        private readonly IRepository<DiagnosisCode> _dcRepository;
        private readonly BillingEntities _context;
        private readonly IMapper _mapper;

        public FutureOpenOrderService(IRepository<FutureOpenOrder> repository, IRepository<GlobalCodes> gRepository, IRepository<GlobalCodeCategory> ggRepository, IRepository<Diagnosis> diaRepository, IRepository<OrderActivity> oaRepository, IRepository<CPTCodes> cRepository, IRepository<DRGCodes> dRepository, IRepository<HCPCSCodes> hcRepository, IRepository<Drug> drugRepository, IRepository<ServiceCode> sRepository, IRepository<DiagnosisCode> dcRepository, BillingEntities context, IMapper mapper)
        {
            _repository = repository;
            _gRepository = gRepository;
            _ggRepository = ggRepository;
            _diaRepository = diaRepository;
            _oaRepository = oaRepository;
            _cRepository = cRepository;
            _dRepository = dRepository;
            _hcRepository = hcRepository;
            _drugRepository = drugRepository;
            _sRepository = sRepository;
            _dcRepository = dcRepository;
            _context = context;
            _mapper = mapper;
        }
        #region Public Methods and Operators

        /// <summary>
        ///     Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<FutureOpenOrderCustomModel> GetFutureOpenOrder(string CptTableNumber, string ServiceCodeTableNumber, string DrgTableNumber, string DrugTableNumber, string HcpcsTableNumber, string DiagnosisTableNumber)
        {
            var list = new List<FutureOpenOrderCustomModel>();
            var lstFutureOpenOrder = _repository.GetAll().ToList();
            if (lstFutureOpenOrder.Count > 0)
            {
                list.AddRange(MapValues(lstFutureOpenOrder, CptTableNumber, ServiceCodeTableNumber, DrgTableNumber, DrugTableNumber, HcpcsTableNumber, DiagnosisTableNumber));
            }

            return list;
        }
        private string GetDiagnoseNameByCodeId(string diagnosisId)
        {
            var id = Convert.ToInt32(diagnosisId);
            var diagnosis = _diaRepository.Where(d => d.DiagnosisID == id).FirstOrDefault();
            return diagnosis != null ? diagnosis.DiagnosisCodeDescription : string.Empty;
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
        private string GetNameByGlobalCodeValue(string codeValue, string categoryValue, string fId = "")
        {
            if (!string.IsNullOrEmpty(codeValue))
            {
                var gl = _gRepository.Where(g => g.GlobalCodeValue.Equals(codeValue) && !g.IsDeleted.Value && g.GlobalCodeCategoryValue.Equals(categoryValue) && (string.IsNullOrEmpty(fId) || g.FacilityNumber.Equals(fId))).FirstOrDefault();
                return gl != null ? gl.GlobalCodeName : string.Empty;
            }
            return string.Empty;
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
        private string GetNameByGlobalCodeId(int id)
        {
            var gl = _gRepository.Where(g => g.GlobalCodeID == id).FirstOrDefault();
            return gl != null ? gl.GlobalCodeName : string.Empty;
        }
        private List<FutureOpenOrderCustomModel> MapValues(List<FutureOpenOrder> m, string CptTableNumber, string ServiceCodeTableNumber, string DrgTableNumber, string DrugTableNumber, string HcpcsTableNumber, string DiagnosisTableNumber)
        {
            var list = new List<FutureOpenOrderCustomModel>();
            if (m != null && m.Any())
            {
                foreach (var model in m)
                {
                    var vm = _mapper.Map<FutureOpenOrderCustomModel>(model);

                    vm.DiagnosisDescription = GetDiagnoseNameByCodeId(vm.DiagnosisCode);
                    vm.OrderDescription = GetCodeDescription(vm.OrderCode, vm.OrderType, CptTableNumber, DrgTableNumber, HcpcsTableNumber, DrugTableNumber, ServiceCodeTableNumber, DiagnosisTableNumber);
                    vm.Status = GetNameByGlobalCodeValue(
                        vm.OrderStatus,
                        Convert.ToInt32(GlobalCodeCategoryValue.OrderStatus).ToString());
                    vm.FrequencyText = GetNameByGlobalCodeValue(
                        vm.FrequencyCode,
                        Convert.ToInt32(GlobalCodeCategoryValue.OrderFrequencyType).ToString());
                    vm.CategoryName = GetGlobalCategoryNameById(vm.CategoryId.ToString());
                    vm.SubCategoryName = vm.SubCategoryId == null
                                             ? string.Empty
                                             : GetNameByGlobalCodeId(Convert.ToInt32(vm.SubCategoryId));
                    vm.OrderTypeName = GetNameByGlobalCodeValue(
                        Convert.ToInt32(vm.OrderType).ToString(),
                        Convert.ToInt32(GlobalCodeCategoryValue.CodeTypes).ToString());
                    vm.SpecimenTypeStr = CalculateLabResultSpecimanType(vm.OrderCode, null, vm.PatientID);
                    list.Add(vm);
                }
            }
            return list;
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

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="FutureOpenOrderId">The Holiday Planner Id.</param>
        /// <returns>
        /// The <see cref="FutureOpenOrder" />.
        /// </returns>
        public FutureOpenOrder GetFutureOpenOrderById(int? FutureOpenOrderId)
        {
            FutureOpenOrder model = _repository.Where(x => x.FutureOpenOrderID == FutureOpenOrderId).FirstOrDefault();
            return model;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int SaveFutureOpenOrder(FutureOpenOrder model)
        {
            if (model.FutureOpenOrderID > 0)
                _repository.UpdateEntity(model, model.FutureOpenOrderID);
            else
                _repository.Create(model);
            return model.FutureOpenOrderID;
        }

        /// <summary>
        /// Gets the future open order by enc identifier.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public List<FutureOpenOrderCustomModel> GetFutureOpenOrderByEncId(int? encounterId, string CptTableNumber, string ServiceCodeTableNumber, string DrgTableNumber, string DrugTableNumber, string HcpcsTableNumber, string DiagnosisTableNumber)
        {
            var list = new List<FutureOpenOrderCustomModel>();
            var lstFutureOpenOrder = _repository.Where(x => x.EncounterID == encounterId).ToList();
            list.AddRange(MapValues(lstFutureOpenOrder, CptTableNumber, ServiceCodeTableNumber, DrgTableNumber, DrugTableNumber, HcpcsTableNumber, DiagnosisTableNumber));

            return list;
        }

        /// <summary>
        /// Gets the future open order by patient identifier.
        /// </summary>
        /// <param name="pid">The pid.</param>
        /// <returns></returns>
        public List<FutureOpenOrderCustomModel> GetFutureOpenOrderByPatientId(int? pid)
        {
            var spName = string.Format("EXEC {0} @pPId", StoredProcedures.SPROC_GetPaitentFutureOrder);
            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("pPId", pid);
            IEnumerable<FutureOpenOrderCustomModel> result = _context.Database.SqlQuery<FutureOpenOrderCustomModel>(spName, sqlParameters);
            return result.ToList();
        }


        /// <summary>
        /// Adds the current enc to future orders.
        /// </summary>
        /// <param name="ordersId">The orders identifier.</param>
        /// <param name="encId">The enc identifier.</param>
        /// <param name="cid">The cid.</param>
        /// <param name="fid">The fid.</param>
        /// <returns></returns>
        public bool AddCurrentEncToFutureOrders(string[] ordersId, int encId, int cid, int fid)
        {
            var orderIds = string.Join(",", ordersId);

            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter("pEncId", encId);
            sqlParameters[1] = new SqlParameter("pFutureOpenOrderId", orderIds);
            sqlParameters[2] = new SqlParameter("pCId", cid);
            sqlParameters[3] = new SqlParameter("pFId", fid);
            _repository.ExecuteCommand(StoredProcedures.SPROC_AddFutureOrdersToCurrentEncounter.ToString(), sqlParameters);
            return true;
        }

        #endregion
    }
}
