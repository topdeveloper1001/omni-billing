using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IProjectsService
    {
        bool CheckDuplicateProjectNumber(string projectNumber, int projectId);
        List<ProjectsCustomModel> DeleteProjects(int id, int corporateid, int facilityid);
        Projects GetProjectDetailsByNumber(string projectNumber);
        List<Projects> GetProjectNumbers(int corporateId, int facilityId);
        Projects GetProjectsById(int? id);
        List<ProjectsCustomModel> GetProjectsList(int corporateid, int facilityid);
        List<ProjectsCustomModel> SaveProjects(Projects model);
    }
}