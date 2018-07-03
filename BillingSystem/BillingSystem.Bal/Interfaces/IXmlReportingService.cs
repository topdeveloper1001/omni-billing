using System;
using System.Collections.Generic;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces  
{
    public interface IXmlReportingService
    {
        List<XmlReportingBatchReport> GetBatchReort(int corporateid, int facilityid);
        List<XmlReportingInitialClaimErrorReport> GetInitialClaimErrorReport(int corporateid, int facilityid, DateTime startdate, DateTime enddate, string encType, string clinicalId);
    }
}