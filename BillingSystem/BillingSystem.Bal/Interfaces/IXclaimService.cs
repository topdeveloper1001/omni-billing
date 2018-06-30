using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IXclaimService
    {
        bool ApplyAdvicePayment(int corporateId, int facilityid);
        bool ApplyAdvicePaymentInRemittanceAdvice(int corporateId, int facilityid, int fileId);
        List<XClaimCustomModel> GetXclaim(string facilityid);
        List<XClaim> GetXclaimByEncounterId(long encounterid);
        List<XClaimCustomModel> GetXclaimByFacilityParameters(string facilityid, string pid, long eid, long claimid);
        XClaim GetXclaimByID(int? XclaimId);
        List<XClaimCustomModel> GetXclaimByParameters(string pid, long eid, long claimid);
        int SaveXclaim(XClaim model);
    }
}