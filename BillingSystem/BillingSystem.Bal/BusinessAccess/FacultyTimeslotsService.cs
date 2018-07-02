using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using AutoMapper;

using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class FacultyTimeslotsService : IFacultyTimeslotsService
    {
        private readonly IRepository<FacultyTimeslots> _repository;
        private readonly IRepository<Physician> _phRepository;
        private readonly IRepository<DeptTimming> _deptRepository;
        private readonly IRepository<FacilityStructure> _fsRepository;
        private readonly BillingEntities _context;
        private readonly IMapper _mapper;

        public FacultyTimeslotsService(IRepository<FacultyTimeslots> repository, IRepository<Physician> phRepository, IRepository<DeptTimming> deptRepository, IRepository<FacilityStructure> fsRepository, BillingEntities context, IMapper mapper)
        {
            _repository = repository;
            _phRepository = phRepository;
            _deptRepository = deptRepository;
            _fsRepository = fsRepository;
            _context = context;
            _mapper = mapper;
        }

        #region Public Methods and Operators

        private DateTime FirstDateOfWeekBal(int year, int weekNum, CalendarWeekRule rule)
        {
            var jan1 = new DateTime(year, 1, 1);

            var daysOffset = DayOfWeek.Monday - jan1.DayOfWeek;
            var firstMonday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            var firstWeek = cal.GetWeekOfYear(firstMonday, rule, DayOfWeek.Monday);

            if (firstWeek > 0) weekNum -= 1;

            var result = firstMonday.AddDays(weekNum * 7);

            return result;
        }

        /// <summary>
        /// Gets the faculty timeslots list.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <param name="weeknumber">The weeknumber.</param>
        /// <returns>
        /// The <see cref="List" />.
        /// </returns>
        public FacultyTimeslotsCustomModelView GetFacultyTimeslotsList(int userid, string weeknumber)
        {
            var savedRecordslist = new List<FacultyTimeslotsCustomModel>();
            var departmentOpeningSlotslist = new List<FacultyTimeslotsCustomModel>();
            #region Saved Records in the DB for the Week
            var lstFacultyTimeslots = _repository.Where(a => a.UserID == userid && a.WeekDay == weeknumber).ToList();
            if (lstFacultyTimeslots.Count > 0)
            {
                var vm2 = MapValues(lstFacultyTimeslots);
                savedRecordslist.AddRange(vm2);
            }
            #endregion


            #region Department Opening slots in week.
            var firstdate = FirstDateOfWeekBal(DateTime.Now.Year, Convert.ToInt32(weeknumber), CalendarWeekRule.FirstFourDayWeek);
            var timeslotListlist = new List<FacultyTimeslots>();
            for (var i = 0; i < 7; i++)
            {
                var datetocheck = firstdate.AddDays(i);
                var weekdday = new FacultyTimeslots { AvailableDateFrom = datetocheck, AvailableDateTill = datetocheck, UserID = userid };
                timeslotListlist.Add(weekdday);
            }
            //Mapper
            var vm = MapValues(timeslotListlist);
            departmentOpeningSlotslist.AddRange(vm);

            //Mapper end


            #endregion

            var facultyTimeslotsCustomModelView = new FacultyTimeslotsCustomModelView()
            {
                DepartmentOpeningSlotsList = departmentOpeningSlotslist,
                FacultySavedSlotsList = savedRecordslist
            };
            return facultyTimeslotsCustomModelView;
        }
        private List<FacultyTimeslotsCustomModel> MapValues(List<FacultyTimeslots> m)
        {
            var list = new List<FacultyTimeslotsCustomModel>();
            if (m != null && m.Any())
            {
                foreach (var model in m)
                {
                    var vm = _mapper.Map<FacultyTimeslotsCustomModel>(model);
                    var facultyObj = GetPhysicianById(model.UserID);
                    var facultyDepartment = facultyObj != null && !string.IsNullOrEmpty(facultyObj.FacultyDepartment)
                                                   ? facultyObj.FacultyDepartment
                                                   : string.Empty;

                    var facilityObj = string.IsNullOrEmpty(facultyDepartment)
                                                        ? null
                                          : GetFacilityStructureById(Convert.ToInt32(facultyDepartment));
                    vm.DepartmentName = facilityObj != null ? facilityObj.FacilityStructureName : string.Empty;
                    var departmentid = facilityObj != null ? facilityObj.FacilityStructureId : 0;

                    var departmentTimmings = GetDeptTimmingByDepartmentId(departmentid);
                    if (departmentTimmings != null)
                    {
                        if (facilityObj != null)
                        {
                            var openingday = ((int)(Convert.ToDateTime(model.AvailableDateFrom).DayOfWeek)).ToString();
                            var departmentFacilityobj = departmentTimmings.FirstOrDefault(x => x.OpeningDayId == openingday);
                            if (departmentFacilityobj != null)
                            {
                                vm.DeptOpeningDays =
                                    ((int)(Convert.ToDateTime(model.AvailableDateFrom).DayOfWeek)).ToString();
                                vm.DeptOpeningTime = departmentFacilityobj.OpeningTime;
                                vm.DeptClosingTime = departmentFacilityobj.ClosingTime;
                            }
                        }
                    }
                    list.Add(vm);
                }
            }
            return list;
        }
        private Physician GetPhysicianById(int id)
        {
            var phys = _phRepository.Where(p => p.Id == id).FirstOrDefault();
            return phys ?? new Physician();
        }
        private List<DeptTimmingCustomModel> GetDeptTimmingByDepartmentId(int departmenId)
        {
            var lst = _deptRepository.Where(x => x.FacilityStructureID == departmenId).ToList();
            return lst.Select(x => _mapper.Map<DeptTimmingCustomModel>(x)).ToList();
        }
        private FacilityStructure GetFacilityStructureById(int? facilityStructureId)
        {
            var lst = _fsRepository.Where(x => x.FacilityStructureId == facilityStructureId).FirstOrDefault();
            return lst;
        }

        /// <summary>
        /// Saves the faculty timeslots.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public FacultyTimeslotsCustomModelView SaveFacultyTimeslots(List<FacultyTimeslots> model)
        {
            foreach (var item in model)
            {
                if (item.ID > 0)
                {
                    var current = _repository.GetSingle(item.ID);
                    item.CreatedBy = current.CreatedBy;
                    item.CreatedDate = current.CreatedDate;
                }

                var item1 = item;
                var eventPid =
                    _repository.Where(
                        x => x.ID == item1.ID &&
                        x.EventId == item1.EventId && x.AvailableDateTill == item1.AvailableDateTill
                        && x.AvailableDateFrom == item1.AvailableDateFrom).Any();

                if (eventPid)
                {
                    _repository.Delete(item);
                }
            }

            if (model[0].ID > 0)
            {
                _repository.Update(model);
            }
            else
            {
                _repository.Create(model);
            }
            var weeknumber = GetWeekOfYearISO8601Bal(Convert.ToDateTime(model[0].AvailableDateFrom)).ToString();
            var list = GetFacultyTimeslotsList(model[0].UserID, weeknumber);
            return list;
        }
        private int GetWeekOfYearISO8601Bal(DateTime date)
        {
            var day = (int)CultureInfo.CurrentCulture.Calendar.GetDayOfWeek(date);
            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                date.AddDays(4 - (day == 0 ? 7 : day)),
                CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek.Monday);
        }

        /// <summary>
        /// Deletes the time slot.
        /// </summary>
        /// <param name="timeslotId">The timeslot identifier.</param>
        /// <returns></returns>
        public bool DeleteTimeSlot(int timeslotId)
        {
            try
            {
                var valueToDelete = _repository.Where(x => x.ID == timeslotId);
                var deletedrecord = _repository.Delete(valueToDelete);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

    }
}