using System.Collections.Generic;
using BillingSystem.Model.CustomModel;
using System.Linq;
using System;
using BillingSystem.Common.Common;

using BillingSystem.Model;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class PreliminaryBillService : IPreliminaryBillService
    {
        private readonly IRepository<UBedMaster> _bedRepository;
        private readonly IRepository<GlobalCodes> _gRepository;
        private readonly IRepository<MappingPatientBed> _mpRepository;
        private readonly IRepository<FacilityStructure> _fsRepository;
        private readonly IRepository<BedTransaction> _bedTRepository;
        private readonly IRepository<PatientInfo> _piRepository;
        private readonly IRepository<BedTransaction> _btRepository;
        private readonly IRepository<Facility> _fRepository;

        public PreliminaryBillService(IRepository<UBedMaster> bedRepository, IRepository<GlobalCodes> gRepository, IRepository<MappingPatientBed> mpRepository, IRepository<FacilityStructure> fsRepository, IRepository<BedTransaction> bedTRepository, IRepository<PatientInfo> piRepository, IRepository<BedTransaction> btRepository, IRepository<Facility> fRepository)
        {
            _bedRepository = bedRepository;
            _gRepository = gRepository;
            _mpRepository = mpRepository;
            _fsRepository = fsRepository;
            _bedTRepository = bedTRepository;
            _piRepository = piRepository;
            _btRepository = btRepository;
            _fRepository = fRepository;
        }

        private string GetBedNameByInPatientEncounterId(string encounterId)
        {
            var bedName = string.Empty;
            var bedTypeAssigned = _mpRepository.Where(m => m.EncounterID.Equals(encounterId) && m.EndDate == null).OrderByDescending(x => x.PatientID).FirstOrDefault();
            if (bedTypeAssigned != null)
            {
                var bedId = Convert.ToInt32(bedTypeAssigned.BedNumber);

                var bedMaster = _bedRepository.Where(b => b.BedId == bedId).FirstOrDefault();
                if (bedMaster != null)
                {
                    var fsId = Convert.ToInt32(bedMaster.FacilityStructureId);

                    var fsStructure = _fsRepository.Where(f => f.FacilityStructureId == fsId).FirstOrDefault();
                    if (fsStructure != null)
                        bedName = fsStructure.FacilityStructureName;

                }


            }
            return bedName;
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
        public List<BedTransactionCustomModel> GetBedTransactionList(int encounterId)
        {
            var finalList = new List<BedTransactionCustomModel>();
            var btList = _bedTRepository.Where(t => t.EncounterID == encounterId).ToList();
            if (btList.Count > 0)
            {
                //finalList.AddRange(btList.Select(item => new BedTransactionCustomModel
                //{
                //    BedTransaction = item
                //}));
                foreach (var item in btList)
                {
                    var bedTransaction = new BedTransactionCustomModel
                    {
                        BedTransactionID = item.BedTransactionID,
                        MappingPatientBedID = item.MappingPatientBedID,
                        FacilityID = item.FacilityID,
                        EncounterID = item.EncounterID,
                        PatientID = item.PatientID,
                        RoomNumber = item.RoomNumber,
                        BedID = item.BedID,
                        BedType = item.BedType,
                        EffectiveDays = item.EffectiveDays,
                        BedTransactionComputedOn = item.BedTransactionComputedOn,
                        ChargeUnit = item.ChargeUnit,
                        Transactionbracket = item.Transactionbracket,
                        BedCharges = item.BedCharges,
                        ProcessStatus = item.ProcessStatus,
                        BillNumber = item.BillNumber,
                        BillStatus = item.BillStatus,
                        CreatedBy = item.CreatedBy,
                        CreatedDate = item.CreatedDate,
                        ModifiedBy = item.ModifiedBy,
                        ModifiedDate = item.ModifiedDate,
                        IsDeleted = item.IsDeleted,
                        DeletedBy = item.DeletedBy,
                        DeletedDate = item.DeletedDate,
                        CorporateID = item.CorporateID,
                        BedTypeName =
                            item.BedType != null && Convert.ToInt32(item.BedType) > 0
                                ? GetNameByGlobalCodeValueAndCategoryValue(
                                    Convert.ToInt32(GlobalCodeCategoryValue.Bedtypes).ToString(),
                                    Convert.ToInt32(item.BedType).ToString())
                                : string.Empty,
                        BedName = GetBedNameByInPatientEncounterId(item.EncounterID.ToString()),
                    };
                    GetBedTransactionDetail(bedTransaction);
                    finalList.Add(bedTransaction);
                }
            }
            return finalList;

        }
        private string GetPatientNameById1(int PatientID)
        {
            var m = _piRepository.GetSingle(Convert.ToInt32(PatientID));
            return m != null ? m.PersonFirstName + " " + m.PersonLastName : string.Empty;
        }
        public BedTransactionCustomModel GetBedTransactionDetail(BedTransactionCustomModel obj)
        {
            obj.FacilityName = GetFacilityNameById(Convert.ToInt32(obj.FacilityID));
            obj.PatientName = GetPatientNameById1(Convert.ToInt32(obj.PatientID));
            return obj;
        }

        private string GetFacilityNameById(int facilityId)
        {
            var m = _fRepository.GetSingle(facilityId);
            return m != null ? m.FacilityName : string.Empty;
        }
        /// <summary>
        /// Method to add the Entity in the database By Encounter Id.
        /// </summary>
        /// <param name="encounterId"></param>
        /// <returns></returns>
        public List<BedTransactionCustomModel> GetBedTransactionByEncounterID(int? encounterId)
        {
            var bedTransactions = new List<BedTransactionCustomModel>();

            var bedTransaction = _btRepository.Where(x => x.EncounterID == encounterId && (x.IsDeleted == null || x.IsDeleted == false)).OrderByDescending(y => y.BedTransactionID).ToList();
            var bedtypeGlobalCodeCategory = Convert.ToInt32(GlobalCodeCategoryValue.Bedtypes).ToString();
            var billStatusGlobalCodeCategory = Convert.ToInt32(GlobalCodeCategoryValue.BillStatus).ToString();
            bedTransactions.AddRange(bedTransaction.Select(item => new BedTransactionCustomModel()
            {
                BedTransactionID = item.BedTransactionID,
                MappingPatientBedID = item.MappingPatientBedID,
                FacilityID = item.FacilityID,
                EncounterID = item.EncounterID,
                PatientID = item.PatientID,
                RoomNumber = item.RoomNumber,
                BedID = item.BedID,
                BedType = item.BedType,
                EffectiveDays = item.EffectiveDays,
                BedTransactionComputedOn = item.BedTransactionComputedOn,
                ChargeUnit = item.ChargeUnit,
                Transactionbracket = item.Transactionbracket,
                BedCharges = item.BedCharges,
                ProcessStatus = item.ProcessStatus,
                BillNumber = item.BillNumber,
                BillStatus = item.BillStatus,
                CreatedBy = item.CreatedBy,
                CreatedDate = item.CreatedDate,
                ModifiedBy = item.ModifiedBy,
                ModifiedDate = item.ModifiedDate,
                IsDeleted = item.IsDeleted,
                DeletedBy = item.DeletedBy,
                DeletedDate = item.DeletedDate,
                CorporateID = item.CorporateID,
                BedName = GetBedNameFromBedId(Convert.ToInt32(item.BedID)),
                BedTypeName =
                    item.BedType != null
                        ? GetGlobalCodeNameByIdAndCategoryId(bedtypeGlobalCodeCategory,
                            Convert.ToInt32(item.BedType))
                        : "NA",
                BillProgressStatus = GetBillProgressStatus(Convert.ToInt32(item.ProcessStatus)),
                BillStatusStr = item.BillStatus != null
                        ? GetGlobalCodeNameByIdAndCategoryId(billStatusGlobalCodeCategory,
                            Convert.ToInt32(item.BillStatus))
                        : "NA",
            }));
            return bedTransactions;
        }
        private string GetGlobalCodeNameByIdAndCategoryId(string categoryId, int globalCodeId)
        {
            var globalCode = _gRepository.Where(c => c.GlobalCodeCategoryValue.Equals(categoryId) && c.GlobalCodeID == globalCodeId).FirstOrDefault();
            if (globalCode != null)
                return globalCode.GlobalCodeName;
            return string.Empty;
        }

        private string GetBedNameFromBedId(int bedId)
        {
            var m = _bedRepository.Where(x => x.BedId == bedId).FirstOrDefault();
            if (m != null)
            {
                var list = _fsRepository.Where(f => f.FacilityStructureId == m.FacilityStructureId).FirstOrDefault();
                return list != null ? list.FacilityStructureName : string.Empty;
            }
            return string.Empty;

        }
        private string GetBillProgressStatus(int progressStatus)
        {
            if (progressStatus == Convert.ToInt32(EncounterProcessStatus.NotStarted))
            {
                return (EncounterProcessStatus.NotStarted).ToString();
            }
            else if (progressStatus == Convert.ToInt32(EncounterProcessStatus.Intiailized))
            {
                return (EncounterProcessStatus.Intiailized).ToString();
            }
            else if (progressStatus == Convert.ToInt32(EncounterProcessStatus.Billed))
            {
                return (EncounterProcessStatus.Billed).ToString();
            }
            return string.Empty;
        }


    }
}
