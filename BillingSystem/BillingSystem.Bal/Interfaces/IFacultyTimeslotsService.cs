using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface IFacultyTimeslotsService
    {
        bool DeleteTimeSlot(int timeslotId);
        FacultyTimeslotsCustomModelView GetFacultyTimeslotsList(int userid, string weeknumber);
        FacultyTimeslotsCustomModelView SaveFacultyTimeslots(List<FacultyTimeslots> model);
    }
}