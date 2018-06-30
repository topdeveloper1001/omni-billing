using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IProjectTasksService
    {
        bool CheckDuplicateTaskNumber(string projectNumber, string taskNumber, int projectTaskId);
        List<ProjectTasksCustomModel> DeleteProjectTask(int id, int corporateId, int facilityId, string userId);
        List<ProjectsCustomModel> GetProjectsDashboardData(int corporateId, int facilityId, string responsibleUserId);
        List<ProjectsCustomModel> GetProjectsForExecKpiDashboard(int corporateId, int facilityId);
        string GetProjectTaskCommentById(int projectTasksId);
        ProjectTasks GetProjectTasksById(int? projectTasksId);
        List<ProjectTasksCustomModel> GetProjectTasksList(int corporateid, int facilityid, string userId);
        List<string> GetTaskNumbers(int corporateid, int facilityid);
        List<ProjectTasksCustomModel> SaveProjectTasks(ProjectTasks model);
    }
}