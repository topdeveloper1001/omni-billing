using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IParametersService
    {
        int AddUptdateParameters(Parameters Parameters);
        List<Parameters> GetParameters();
        Parameters GetParametersByID(int? ParametersId);
        List<ParametersCustomModel> GetParametersCustomModel(int corporateid, int facilityid);
        ParametersCustomModel GetParametersCustomModelByID(int? ParametersId);
        Parameters GetSingleParameterBySystemCode(string systemCode, int corporateId, int facilityId);
    }
}