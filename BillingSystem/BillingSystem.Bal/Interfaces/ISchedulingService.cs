using System;
using System.Collections.Generic;
using System.Data;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface ISchedulingService
    {
        bool AddUpdatePatientScheduling(List<SchedulingCustomModel> model, int facilityId, List<int> tobedeleted);
        bool CheckForDuplicateRecord(int schedulingId, string schedulingType, DateTime starTime, DateTime endTime, int userid, int physicianid, int facilityid);
        bool CheckForDuplicateRecordRecurring(DateTime startDate, DateTime endDate, string timeFrom, string timeTo, int pFacilityid, int pSchedulingId, int pPhysicianid, string pRecPattern);
        void CreateRecurringSchedularEvents(int pSchedulingId);
        bool DeleteHolidayPlannerData(string eventParentid, int schedulingid, int schedulingType, string extValue3);
        bool DeleteHolidaysByEventParentID(string eventParentId);
        RoomEquipmentAvialability GetAssignedRoomForProcedure(int facilityId, int appointmentType, DateTime scheduledDate, string timeFrom, string timeTo, int roomId);
        RoomEquipmentAvialability GetAssignedRoomForProcedure(int facilityId, int appointmentType, DateTime scheduledDate, string timeFrom, string timeTo, int roomId, int schedulingId, int pId);
        IEnumerable<AvailabilityTimeSlotForPopupCustomModel> GetAvailableTimeSlots(int facilityid, int physicianId, DateTime dateselected, string typeofproc, out DateTime timeSlotDate, bool firstAvailable = false);
        string GetDeptOpeningDaysForPhysician(int physicianId);
        List<SchedulingCustomModel> GetFacilityHolidays(int facilityid);
        List<SchedulingCustomModel> GetHolidayPlannerData(int physicianId, DateTime selectedDate, string type, int facilityId);
        List<SchedulingCustomModel> GetiSchedulingData(string associatedId, DateTime selectedDate, string viewtype, bool isPatient);
        string GetNameByLicenseTypeIdAndUserTypeId(string licenceTypeId, string userTypeId);
        NotAvialableTimeSlots GetNotAvialableTimeSlotsCustomModel(SchedulingCustomModel model);
        List<TypeOfProcedureCustomModel> GetOtherProceduresByEventParentId(string eventparentId, DateTime scheduleFrom);
        List<SchedularOverViewCustomModel> GetOverView(SchedularOverViewCustomModel m);
        List<SchedulingCustomModel> GetPatientNextScheduling(int patientId, DateTime selectedDate);
        List<SchedulingCustomModel> GetPatientScheduling(int patientId, DateTime selectedDate, string viewtype);
        List<SchedulingCustomModel> GetPhyPreviousVacations(int facilityid, int physicianId);
        List<SchedulingCustomModel> GetPreSchedulingList(int cId, int fId);
        List<SchedulerCustomModelForCalender> GetSchedulerData(int viewType, DateTime selectedDate, string phyIds, int fId, string depIds, string roomIds, string statusIds, string sectionType, int patientId);
        List<SchedulerCustomModelForCalender> GetSchedulerData(int viewType, DateTime selectedDate, string phyIds, int fId, string depIds, string roomIds, string statusIds, string sectionType, int patientId, out List<SchedulingCustomModel> nextList);
        List<SchedulingCustomModel> GetSchedulingByPhysiciansData(string physicianIdlist, DateTime selectedDate, string viewtype, int facilityid);
        SchedulingCustomModel GetSchedulingCustomModelById(int schedulingid);
        List<SchedulingCustomModel> GetSchedulingDataByDepartments(List<string> deptList, DateTime selectedDate, string type, int facilityId);
        List<SchedulingCustomModel> GetSchedulingDataByRooms(List<int> roomsList, DateTime selectedDate);
        List<SchedulingCustomModel> GetSchedulingDataByType(List<string> phyList, DateTime selectedDate, string type, int facilityId);
        List<SchedulingCustomModel> GetSchedulingDeptDataByType(int deptId, DateTime selectedDate, string type, string facilityIdstr);
        List<SchedulingCustomModel> GetSchedulingListByPatient(int patientId, string physicianId, string vToken, out string patientEmail);
        List<Scheduling> MapVMToModel(List<SchedulingCustomModel> vm);
        bool RemoveJustDeletedSchedulings(string eventParentid, List<int> listSchIds, int schedulingType, string extValue3);
        List<SkippedHolidaysData> SaveHolidayScheduling(List<SchedulingCustomModel> model);
        int SavePatientInfoInScheduler(int cId, int fId, DateTime pDate, int pId, string firstName, string lastName, DateTime? dob, string email, string emirateId, int loggedUserId, string phone, int age, string newPwd = "");
        SchedulingCustomModelView SavePatientPreSchedulingList(List<SchedulingCustomModel> model);
        SchedulingCustomModelView SavePatientScheduling(IEnumerable<SchedulingCustomModel> model);
        bool UpdateAppointmentStatus(long schedulingId, string status, int userId, DateTime currentDatetime);
        bool UpdateSchedulingEvents(List<SchedulingCustomModel> list);
        List<SchedulingCustomModel> ValidateScheduling(DataTable dt, int facilityId, int userId, out int status);
    }
}