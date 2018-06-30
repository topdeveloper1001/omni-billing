using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IFacultyRoosterService
    {
        int CheckForDuplicateEntry(FacultyRooster model);
        int DeleteFacultyRooster(int id);
        FacultyRoosterLogCustomModel DuplicateEntryLog(FacultyRooster model, int type);
        List<FacultyRoosterCustomModel> GetFacultyRoosterByFacility(int facilityId);
        List<FacultyRooster> GetFacultyRoosterByFacultyid(int facultyid);
        FacultyRooster GetFacultyRoosterById(int? facultyRoosterId);
        FacultyRoosterCustomModel GetFacultyRoosterCById(int? facultyRoosterId);
        int SaveFacultyRooster(FacultyRooster m);
        int SaveFacultyRoosterList(List<FacultyRooster> model);
    }
}