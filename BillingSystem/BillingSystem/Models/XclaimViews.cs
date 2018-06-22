using System.Collections.Generic;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model;

namespace BillingSystem.Models
{
    public class XclaimView
    {
        public XClaim CurrentXclaim { get; set; }
        public List<XClaimCustomModel> XclaimList { get; set; }
        public int? ClaimId { get; set; }
        public int? EncounterId { get; set; }
        public int? PatientId { get; set; }
    }
}
