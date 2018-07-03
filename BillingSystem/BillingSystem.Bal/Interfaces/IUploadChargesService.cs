using System.Collections.Generic;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IUploadChargesService
    {
        List<BillDetailCustomModel> DeleteBillActivity(int billActivityId, int userid, int billHeaderId);
        List<BillDetailCustomModel> GetBillDetailsByBillHeaderId(int billHeaderId);
        List<PatientInfoXReturnPaymentCustomModel> GetXPaymentReturnDenialClaims(CommonModel common);
        List<PatientInfoXReturnPaymentCustomModel> GetXPaymentReturnDenialClaimsByPatientId(int patientId, int encounterId, int billHeaderId);
    }
}