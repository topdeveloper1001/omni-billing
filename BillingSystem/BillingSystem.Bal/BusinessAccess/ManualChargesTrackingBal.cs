using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.BusinessAccess
{
    public class ManualChargesTrackingBal : BaseBal
    {
        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <returns>
        /// Return the Entity List
        /// </returns>
        public List<ManualChargesTrackingCustomModel> GetManualChargesTrackingList(int facilityid, int corporateid)
        {
            var list = new List<ManualChargesTrackingCustomModel>();
            using (var manualChargesTrackingRep = UnitOfWork.ManualChargesTrackingRepository)
            {
                var lstManualChargesTracking =
                    manualChargesTrackingRep.Where(
                        a =>
                            (a.IsVisible == null || (bool) a.IsVisible) && a.CorporateID == corporateid &&
                            a.FacilityID == facilityid).ToList();
                if (lstManualChargesTracking.Count > 0)
                {
                    list.AddRange(lstManualChargesTracking.Select(item => new ManualChargesTrackingCustomModel
                    {
                        ManualChargesTrackingID = item.ManualChargesTrackingID,
                        TrackingType = item.TrackingType,
                        TrackingTypeNameVal = item.TrackingTypeNameVal,
                        TrackingColumnName = item.TrackingColumnName,
                        TrackingTableName = item.TrackingTableName,
                        TrackingValue = item.TrackingValue,
                        TrackingBillStatus = item.TrackingBillStatus,
                        BillHeaderID = item.BillHeaderID,
                        EncounterID = item.EncounterID,
                        PatientID = item.PatientID,
                        FacilityID = item.FacilityID,
                        CorporateID = item.CorporateID,
                        CreatedBy = item.CreatedBy,
                        CreatedDate = item.CreatedDate,
                        ModifiedBy = item.ModifiedBy,
                        ModifiedDate = item.ModifiedDate,
                        DeletedBy = item.DeletedBy,
                        DeletedDate = item.DeletedDate,
                        ExternalValue1 = item.ExternalValue1,
                        ExternalValue2 = item.ExternalValue2,
                        ExternalValue3 = item.ExternalValue3,
                        ExternalValue4 = item.ExternalValue4,
                        ExternalValue5 = item.ExternalValue5,
                        ExternalValue6 = item.ExternalValue6,
                        IsVisible = item.IsVisible,
                        TrackingTypeStr = GetNameByGlobalCodeValue(item.TrackingType,"1019"),
                        TrackingTypeNameValStr = GetNameByGlobalCodeValue(item.TrackingTypeNameVal, "1201"),
                        FacilityName = GetFacilityNameByFacilityId(item.FacilityID),
                        CorporateName = GetNameByCorporateId(item.CorporateID),
                        EncounterNumber = GetEncounterNumberById(item.EncounterID),
                        PatientName = GetPatientNameById(item.PatientID),
                        BillNumber = GetBillNumberByBillHeaderId(item.BillHeaderID),
                        TrackingBillStatusStr = GetBillHeaderStatusByStatus(item.TrackingBillStatus),
                        UpdatedBy = GetNameByUserId(item.CreatedBy)
                    }));
                }
            }
            return list.OrderByDescending(i => i.ManualChargesTrackingID).ToList();
        }


        /// <summary>
        /// Gets the manual charges tracking list date range.
        /// </summary>
        /// <param name="facilityid">The facilityid.</param>
        /// <param name="corporateid">The corporateid.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="tilldate">The tilldate.</param>
        /// <returns></returns>
        public List<ManualChargesTrackingCustomModel> GetManualChargesTrackingListDateRange(int facilityid, int corporateid, DateTime fromDate, DateTime tilldate)
        {
            var list = new List<ManualChargesTrackingCustomModel>();
            using (var manualChargesTrackingRep = UnitOfWork.ManualChargesTrackingRepository)
            {
                tilldate = tilldate.AddHours(23);
                var lstManualChargesTracking =
                    manualChargesTrackingRep.Where(
                        a =>
                            (a.IsVisible == null || (bool)a.IsVisible) && a.CorporateID == corporateid &&
                            a.FacilityID == facilityid).ToList();
                lstManualChargesTracking = lstManualChargesTracking .Where(a => a.CreatedDate >= fromDate && a.CreatedDate <= tilldate).ToList();
                if (lstManualChargesTracking.Count > 0)
                {
                    list.AddRange(lstManualChargesTracking.Select(item => new ManualChargesTrackingCustomModel
                    {
                        ManualChargesTrackingID = item.ManualChargesTrackingID,
                        TrackingType = item.TrackingType,
                        TrackingTypeNameVal = item.TrackingTypeNameVal,
                        TrackingColumnName = item.TrackingColumnName,
                        TrackingTableName = item.TrackingTableName,
                        TrackingValue = item.TrackingValue,
                        TrackingBillStatus = item.TrackingBillStatus,
                        BillHeaderID = item.BillHeaderID,
                        EncounterID = item.EncounterID,
                        PatientID = item.PatientID,
                        FacilityID = item.FacilityID,
                        CorporateID = item.CorporateID,
                        CreatedBy = item.CreatedBy,
                        CreatedDate = item.CreatedDate,
                        ModifiedBy = item.ModifiedBy,
                        ModifiedDate = item.ModifiedDate,
                        DeletedBy = item.DeletedBy,
                        DeletedDate = item.DeletedDate,
                        ExternalValue1 = item.ExternalValue1,
                        ExternalValue2 = item.ExternalValue2,
                        ExternalValue3 = item.ExternalValue3,
                        ExternalValue4 = item.ExternalValue4,
                        ExternalValue5 = item.ExternalValue5,
                        ExternalValue6 = item.ExternalValue6,
                        IsVisible = item.IsVisible,
                        TrackingTypeStr = GetNameByGlobalCodeValue(item.TrackingType, "1019"),
                        TrackingTypeNameValStr = GetNameByGlobalCodeValue(item.TrackingTypeNameVal, "1201"),
                        FacilityName = GetFacilityNameByFacilityId(item.FacilityID),
                        CorporateName = GetNameByCorporateId(item.CorporateID),
                        EncounterNumber = GetEncounterNumberById(item.EncounterID),
                        PatientName = GetPatientNameById(item.PatientID),
                        BillNumber = GetBillNumberByBillHeaderId(item.BillHeaderID),
                        TrackingBillStatusStr = GetBillHeaderStatusByStatus(item.TrackingBillStatus),
                        UpdatedBy = GetNameByUserId(item.CreatedBy)
                    }));
                }
            }
            return list.OrderByDescending(i => i.ManualChargesTrackingID).ToList();
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="ManualChargesTrackingId">The manual charges tracking identifier.</param>
        /// <returns></returns>
        public ManualChargesTracking GetManualChargesTrackingByID(int? ManualChargesTrackingId)
        {
            using (var rep = UnitOfWork.ManualChargesTrackingRepository)
            {
                var model = rep.Where(x => x.ManualChargesTrackingID == ManualChargesTrackingId).FirstOrDefault();
                return model;
            }
        }

        /// <summary>
        /// Adds the uptdate corporate.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public int AddUptdateManualChargesTracking(ManualChargesTracking model)
        {
            using (var corporateRep = UnitOfWork.ManualChargesTrackingRepository)
            {
                if (model.ManualChargesTrackingID > 0)
                {
                    corporateRep.UpdateEntity(model, model.ManualChargesTrackingID);
                }
                else
                    corporateRep.Create(model);
                return model.CorporateID;
            }
        }
    }
}
