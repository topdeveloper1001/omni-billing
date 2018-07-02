using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IProjectTargetsService
    {
        List<ProjectTargetsCustomModel> DeleteProjectTarget(int id, int corporateId, int facilityId);
        ProjectTargets GetProjectTargetsById(int id);
        List<ProjectTargetsCustomModel> GetProjectTargetsList(int corporateid, int facilityid);
        int SaveMonthWiseValuesInProjectDashboard(string ProjectNumber, string Month, string CorporateId, string FacilityId, string TaskNumber);
        List<ProjectTargetsCustomModel> SaveProjectTargets(ProjectTargets model);
    }
}