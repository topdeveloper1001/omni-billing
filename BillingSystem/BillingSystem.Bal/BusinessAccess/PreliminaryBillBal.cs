using System.Collections.Generic;
using BillingSystem.Model.CustomModel;
using System.Linq;
using System;
using BillingSystem.Common.Common;

namespace BillingSystem.Bal.BusinessAccess
{
    public class PreliminaryBillBal : BaseBal
    {
        public List<BedTransactionCustomModel> GetBedTransactionList(int encounterId)
        {
            using (var rep = UnitOfWork.BedTransactionRepository)
            {
                var finalList = new List<BedTransactionCustomModel>();
                var btList = rep.Where(t => t.EncounterID == encounterId).ToList();
                if (btList.Count > 0)
                {
                    //finalList.AddRange(btList.Select(item => new BedTransactionCustomModel
                    //{
                    //    BedTransaction = item
                    //}));
                    using (var gBal = new GlobalCodeBal())
                    {
                        var bedMasterBal = new BedMasterBal();
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
                                        ? gBal.GetNameByGlobalCodeValueAndCategoryValue(
                                            Convert.ToInt32(GlobalCodeCategoryValue.Bedtypes).ToString(),
                                            Convert.ToInt32(item.BedType).ToString())
                                        : string.Empty,
                                BedName = bedMasterBal.GetBedNameByInPatientEncounterId(item.EncounterID.ToString()),
                            };
                            GetBedTransactionDetail(bedTransaction);
                            finalList.Add(bedTransaction);
                        }
                    }
                }
                return finalList;
            }

        }

        public BedTransactionCustomModel GetBedTransactionDetail(BedTransactionCustomModel obj)
        {
            using (var facBal = new FacilityBal())
            {
                using (var patientBal = new PatientInfoBal())
                {
                    obj.FacilityName = facBal.GetFacilityNameById(Convert.ToInt32(obj.FacilityID));
                    obj.PatientName = patientBal.GetPatientNameById(Convert.ToInt32(obj.PatientID));
                }
            }
            return obj;
        }

        /// <summary>
        /// Method to add the Entity in the database By Encounter Id.
        /// </summary>
        /// <param name="encounterId"></param>
        /// <returns></returns>
        public List<BedTransactionCustomModel> GetBedTransactionByEncounterID(int? encounterId)
        {
            var bedTransactions = new List<BedTransactionCustomModel>();
            using (var bedTransactionRep = UnitOfWork.BedTransactionRepository)
            {
                using (var ubedmaster = new BedMasterBal())
                {
                    using (var globalCodebal = new GlobalCodeBal())
                    {
                        var bedTransaction =
                            bedTransactionRep.Where(
                                x => x.EncounterID == encounterId && (x.IsDeleted == null || x.IsDeleted == false))
                                .OrderByDescending(_ => _.BedTransactionID)
                                .ToList();
                        var bedtypeGlobalCodeCategory =
                            Convert.ToInt32(Common.Common.GlobalCodeCategoryValue.Bedtypes).ToString();
                        var billStatusGlobalCodeCategory =
                            Convert.ToInt32(Common.Common.GlobalCodeCategoryValue.BillStatus).ToString();
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
                            BedName = ubedmaster.GetBedNameFromBedId(Convert.ToInt32(item.BedID)),
                            BedTypeName =
                                item.BedType != null
                                    ? globalCodebal.GetGlobalCodeNameByIdAndCategoryId(bedtypeGlobalCodeCategory,
                                        Convert.ToInt32(item.BedType))
                                    : "NA",
                            BillProgressStatus = GetBillProgressStatus(Convert.ToInt32(item.ProcessStatus)),
                            BillStatusStr = item.BillStatus != null
                                    ? globalCodebal.GetGlobalCodeNameByIdAndCategoryId(billStatusGlobalCodeCategory,
                                        Convert.ToInt32(item.BillStatus))
                                    : "NA",
                        }));
                        return bedTransactions;
                    }
                }
            }
        }

        private string GetBillProgressStatus(int progressStatus)
        {
            if (progressStatus == Convert.ToInt32(Common.Common.EncounterProcessStatus.NotStarted))
            {
                return (Common.Common.EncounterProcessStatus.NotStarted).ToString();
            }
            else if (progressStatus == Convert.ToInt32(Common.Common.EncounterProcessStatus.Intiailized))
            {
                return (Common.Common.EncounterProcessStatus.Intiailized).ToString();
            }
            else if (progressStatus == Convert.ToInt32(Common.Common.EncounterProcessStatus.Billed))
            {
                return (Common.Common.EncounterProcessStatus.Billed).ToString();
            }
            return string.Empty;
        }
    

    }
}
