using BillingSystem.Model;

namespace BillingSystem.Bal.Interfaces
{
    public interface IPatientCareActivitiesService
    {
        int AddUptdatePatientCareActivity(int id, int facilityid, string status);
        PatientCareActivities GetPatientCarePlanActivity(int id);
    }
}