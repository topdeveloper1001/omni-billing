using System;
using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IBillHeaderService
    {
        bool AddBillonEncounterStart(int encounterId, int billheaderid, int patientId, int corporateId, int facilityId);
        bool ApplyBedChargesAndOrderBill(int encounterId);
        int ApplyBedChargesOnly(int encounterId);
        List<BillHeaderCustomModel> FindClaim(string serachstring, string claimstatus, DateTime? datefrom, DateTime? datetill, int facilityId, int corporateId, int fileid = 0);
        List<BillHeaderCustomModel> FindClaimByFileId(string serachstring, string claimstatus, DateTime? datefrom, DateTime? datetill, int facilityId, int corporateId, int? fileid);
        List<BillHeaderCustomModel> GetAllBillHeaderList(int corporateId, int facilityId);
        List<BillHeaderCustomModel> GetAllBillHeaderListByEncounterId(int encounterId);
        List<BillHeaderCustomModel> GetAllBillHeaderListByPatientId(int patientId);
        List<int> GetAllEncounterIdsInBillHeader();
        BillHeaderCustomModel GetBillHeaderById(int billHeaderId);
        BillHeaderCustomModel GetBillHeaderDetailByCurrentEncounter(int mostRecentEncounterId);
        List<BillHeaderCustomModel> GetBillHeaderListByEncounterId(int encounterId, int corporateId, int facilityId);
        List<BillHeaderCustomModel> GetBillHeaderListByPatientId(int patientId, int corporateId, int facilityId);
        List<BillHeader> GetBillHeaderModelListByEncounterId(int encounterId);
        List<BillHeader> GetBillHeadersByBillId(int billid);
        BillHeader GetBillHeaderToUpdateById(int billheaderid);
        List<BillHeaderCustomModel> GetFinalBillByPayerHeadersList(int corporateId, int facilityId, string payerIds);
        List<BillHeaderCustomModel> GetFinalBillHeadersList(int encounterId, int patientId, bool encounterSelected, int corporateId, int facilityId);
        List<BillHeaderCustomModel> GetFinalBillPayerHeadersList(int corporateId, int facilityId);
        List<BillHeaderPreXMLModel> GetPreXMLFile(int billHeaderId, int facilityId);
        bool RecalculateBill(int corporateId, int facilityId, int encounterId, string reclaimFlag, long calimId, int userId);
        int SaveManualPayment(BillHeader model);
        IEnumerable<BillHeaderXMLModel> SendEClaimsByPayer(int facilityId, string payerId, string billHeaderIds);
        bool SetBillHeaderStatus(List<int> billHeaderIds, string status, string oldStatus);
        bool SetPreliminaryBillStatusByEncounterId(List<int> encounterids, int userId);
        List<BillHeaderCustomModel> UpdateBillHeaderByBillHeaderEncounterId(int encounterId, int billheaderid, bool isEncounterSelected, int patientId, int corporateId, int facilityId);
        List<BillHeaderCustomModel> UpdateBillHeadersByEncounterId(int encounterId, int patientId, bool isEncounterSelected, int corporateId, int facilityId);
        bool XMLScrubBill(int claimId, int userid);
    }
}