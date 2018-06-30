
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
    public class BillActivityService : IBillActivityService
    {
        private readonly IRepository<BillActivity> _repository;
        private readonly IRepository<BillHeader> _bRepository;
        private readonly IRepository<GlobalCodes> _gRepository;
        private readonly IRepository<CPTCodes> _cRepository;
        private readonly IRepository<DRGCodes> _dRepository;
        private readonly IRepository<HCPCSCodes> _hcRepository;
        private readonly IRepository<Drug> _drugRepository;
        private readonly IRepository<ServiceCode> _sRepository;
        private readonly IRepository<DiagnosisCode> _dcRepository;
        private readonly BillingEntities _context;

        public BillActivityService(IRepository<BillActivity> repository, IRepository<BillHeader> bRepository, IRepository<GlobalCodes> gRepository, IRepository<CPTCodes> cRepository, IRepository<DRGCodes> dRepository, IRepository<HCPCSCodes> hcRepository, IRepository<Drug> drugRepository, IRepository<ServiceCode> sRepository, IRepository<DiagnosisCode> dcRepository, BillingEntities context)
        {
            _repository = repository;
            _bRepository = bRepository;
            _gRepository = gRepository;
            _cRepository = cRepository;
            _dRepository = dRepository;
            _hcRepository = hcRepository;
            _drugRepository = drugRepository;
            _sRepository = sRepository;
            _dcRepository = dcRepository;
            _context = context;
        }
        /// <summary>
        /// Gets the bill detail view.
        /// </summary>
        /// <param name="billHeaderId">The bill header identifier.</param>
        /// <returns></returns>
        public List<BillDetailCustomModel> GetBillActivitiesByBillHeaderId(int billHeaderId)
        {
            var spName = string.Format("EXEC {0} @pBillHeaderID ", StoredProcedures.SPROC_BillDetailView);
            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("pBillHeaderID ", billHeaderId);
            IEnumerable<BillDetailCustomModel> result = _context.Database.SqlQuery<BillDetailCustomModel>(spName, sqlParameters);
            var list = result.ToList();
            var totalrow = new BillDetailCustomModel()
            {
                ActivityType = string.Empty,
                ActivityTypeName = string.Empty,
                ActivityCode = string.Empty,
                BillNumber = string.Empty,
                BillHeaderID = 0,
                BillActivityID = 0,
                CorporateID = 0,
                EncounterID = 0,
                FacilityID = 0,
                OrderedOn = null,
                ExecutedOn = null,
                QuantityOrdered = null,
                ActivityCodeDescription = "Total",
                GrossCharges = list.Sum(x => x.GrossCharges),
                PayerShareNet = list.Sum(x => x.PayerShareNet),
                PatientShare = list.Sum(x => x.PatientShare),
                ActivityCost = list.Sum(x => x.ActivityCost),
                GrossChargesSum = list.Sum(x => x.PayerShareNet) + list.Sum(x => x.PatientShare)
            };
            list.Add(totalrow);
            return list;

        }

        public string GetNameByGlobalCodeValue(string codeValue, string categoryValue, string fId = "")
        {
            if (!string.IsNullOrEmpty(codeValue))
            {
                var gl = _gRepository.Where(g => g.GlobalCodeValue.Equals(codeValue) && !g.IsDeleted.Value && g.GlobalCodeCategoryValue.Equals(categoryValue) && (string.IsNullOrEmpty(fId) || g.FacilityNumber.Equals(fId))).FirstOrDefault();
                return gl != null ? gl.GlobalCodeName : string.Empty;
            }
            return string.Empty;
        }

        public string GetCodeDescription(string orderCode, string orderType, string CptTableNumber, string DrgTableNumber, string HcpcsTableNumber, string DrugTableNumber,
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
        /// Gets the bill activities by encounter identifier.
        /// </summary>
        /// <param name="encounterId">The encounter identifier.</param>
        /// <returns></returns>
        public List<BillDetailCustomModel> GetBillActivitiesByEncounterId(int encounterId, string CptTableNumber, string DrgTableNumber, string HcpcsTableNumber, string DrugTableNumber,
             string ServiceCodeTableNumber, string DiagnosisTableNumber)
        {
            var spName = string.Format("EXEC {0} @pEncounterID ", StoredProcedures.SPROC_EncounterTransactionView);
            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("pEncounterID ", encounterId);
            IEnumerable<BillDetailCustomModel> result = _context.Database.SqlQuery<BillDetailCustomModel>(spName, sqlParameters);
            var list = result.ToList();
            //_bRepository.GetEncounterBillDetailView(encounterId);
            list = list.Select(item =>
            {
                item.ActivityTypeName = GetNameByGlobalCodeValue(item.ActivityType,
                    Convert.ToString((int)GlobalCodeCategoryValue.CodeTypes));
                var billHeader = _bRepository.Where(a => a.BillHeaderID == item.BillHeaderID).FirstOrDefault();
                if (billHeader != null)
                {
                    item.BillNumber = billHeader.BillNumber;
                    item.ActivityCodeDescription = GetCodeDescription(item.ActivityCode, item.ActivityType, CptTableNumber, DrgTableNumber, HcpcsTableNumber, DrugTableNumber,
              ServiceCodeTableNumber, DiagnosisTableNumber);
                }
                return item;
            }).OrderBy(b => b.ExecutedOn).ToList();
            var totalrow = new BillDetailCustomModel()
            {
                ActivityType = string.Empty,
                ActivityTypeName = string.Empty,
                ActivityCode = string.Empty,
                BillNumber = string.Empty,
                BillHeaderID = 0,
                BillActivityID = 0,
                CorporateID = 0,
                EncounterID = 0,
                FacilityID = 0,
                OrderedOn = null,
                ExecutedOn = null,
                QuantityOrdered = null,
                ActivityCodeDescription = "Total",
                GrossCharges = list.Sum(x => x.GrossCharges),
                PayerShareNet = list.Sum(x => x.PayerShareNet),
                PatientShare = list.Sum(x => x.PatientShare),
                GrossChargesSum = list.Sum(x => x.PayerShareNet) + list.Sum(x => x.PatientShare)
            };
            list.Add(totalrow);
            return list;

        }

        /// <summary>
        /// Gets the bill header identifier by bill activity identifier.
        /// </summary>
        /// <param name="billactivityId">The billactivity identifier.</param>
        /// <returns></returns>
        public int GetPatientIdByBillActivityId(int billactivityId)
        {
            var result = _repository.Where(x => x.BillActivityID == billactivityId).FirstOrDefault();
            return result != null ? Convert.ToInt32(result.ActivityID) : 0;
        }

        /// <summary>
        /// Gets the encounter identifier by bill activity identifier.
        /// </summary>
        /// <param name="billactivityId">The billactivity identifier.</param>
        /// <returns></returns>
        public int GetEncounterIdByBillActivityId(int billactivityId)
        {
            var result = _repository.Where(x => x.BillActivityID == billactivityId).FirstOrDefault();
            return result != null ? Convert.ToInt32(result.EncounterID) : 0;
        }

        /// <summary>
        /// Deletes the bill activity from bill.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public bool DeleteBillActivityFromBill(int id)
        {
            return _repository.Delete(id) > 0;
        }

        //public object GetPatientBabyMotherLink(int patientId)
        //{
        //    using (var rep = UnitOfWork.PatientInfoRepository)
        //    {
        //        var PinfoObj
        //        return true;
        //    }
        //}

        public bool DeleteDiagnosisTypeBillActivity(int encounterId, int patientId, string actCode, int userId)
        {
            var current = _repository.Where(
                    b => b.ActivityCode.Equals(actCode) && b.EncounterID == encounterId && b.PatientID == patientId && b.IsDeleted == false)
                    .FirstOrDefault();

            if (current != null)
            {
                var sqlParameters = new SqlParameter[2];
                sqlParameters[0] = new SqlParameter("pBillActivityID ", current.BillActivityID);
                sqlParameters[1] = new SqlParameter("pCreatedBy ", userId);
                _bRepository.ExecuteCommand(StoredProcedures.SPROC_DeleteBillActivites.ToString(), sqlParameters);
                return true;

            }
            return false;
        }

        /// <summary>
        /// Gets the bill PDF format.
        /// </summary>
        /// <param name="billHeaderId">The bill header identifier.</param>
        /// <returns></returns>
        public List<BillPdfFormatCustomModel> GetBillPdfFormat(int billHeaderId)
        {
            var spName = string.Format("EXEC {0} @pBillHeaderID ", StoredProcedures.SPROC_GetBillFormatData);
            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("pBillHeaderID ", billHeaderId);
            IEnumerable<BillPdfFormatCustomModel> result = _context.Database.SqlQuery<BillPdfFormatCustomModel>(spName, sqlParameters);
            return result.ToList();
        }
    }
}

