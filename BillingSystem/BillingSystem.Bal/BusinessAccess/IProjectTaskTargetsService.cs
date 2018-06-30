using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IProjectTaskTargetsService
    {
        List<ProjectTaskTargetsCustomModel> DeleteProjectTaskTargets(int id, int corporateId, int facilityId);
        ProjectTaskTargets GetProjectTaskTargetsById(int? id);
        List<ProjectTaskTargetsCustomModel> GetProjectTaskTargetsList(int corporateid, int facilityid);
        List<ProjectTaskTargetsCustomModel> SaveProjectTaskTargets(ProjectTaskTargets model);
        bool UpdateProjectTaskTargetTaskNumber(string oldTaskNumber, string newTaskNumber, string projectTaskId);
    }
}