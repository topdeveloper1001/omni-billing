﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Threading.Tasks;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Model.CustomModel;
using BillingSystem.Common;
using BillingSystem.Model;
using BillingSystem.Filters;
using BillingSystem.Common.Common;

namespace BillingSystem.Controllers
{
    [CheckRolesAuthorize("1", "2", "3", "4", "5", "6", "7", "8", "9", "0")]
    public class SchedulerController : BaseController
    {
        private readonly ISchedulingService _service;
        private readonly ICountryService _cService;
        private readonly IAppointmentTypesService _atService;
        private readonly IPatientInfoService _piService;
        private readonly IFacilityStructureService _fsService;
        private readonly IPhysicianService _phService;
        private readonly IUsersService _uService;
        private readonly IFacilityService _fService;
        private readonly IGlobalCodeService _gService;
        private readonly ISchedulingParametersService _spService;


        private const string partialViewPath = "../Scheduler/{0}";

        public SchedulerController(ICountryService cService, IAppointmentTypesService atService, IPatientInfoService piService, IFacilityStructureService fsService, IPhysicianService phService, ISchedulingService service, IUsersService uService, IFacilityService fService, IGlobalCodeService gService, ISchedulingParametersService spService)
        {
            _cService = cService;
            _atService = atService;
            _piService = piService;
            _fsService = fsService;
            _phService = phService;
            _service = service;
            _uService = uService;
            _fService = fService;
            _gService = gService;
            _spService = spService;
        }


        #region Public Methods and Operators

        /// <summary>
        ///     The index.
        /// </summary>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult Index(int? v)
        {
            if (!v.HasValue)
                return RedirectToAction(CommonConfig.LoginAction, CommonConfig.LoginController);

            var roleKey = Helpers.CurrentRoleKey;
            ViewBag.ViewId = v.HasValue ? v.Value : 0;

            return View(roleKey.Equals("8") ? "sview1" : string.Empty);
        }

        /// <summary>
        /// Gets the schedular with physicians.
        /// </summary>
        /// <param name="filters">The filters.</param>
        /// <returns></returns>
        public ActionResult GetSchedularWithFilters(List<SchedularTypeCustomModel> filters)
        {
            var listtoReturn = new List<SchedulingCustomModel>();

            var selectedPhysicianList = filters[0].PhysicianId;
            var selectedStatusList = filters[0].StatusType;
            var selectedDate = filters[0].SelectedDate;
            var selectedDepartmentList = filters[0].DeptData;
            var facilityid = filters[0].Facility;
            var viewtype = filters[0].ViewType;
            var patientId = filters[0].PatientId;
            var selectedRoomsList = filters[0].RoomIds;
            var phyObj = ConvertArrayToCommaSeperatorString(selectedPhysicianList);
            //var dep = ConvertArrayToCommaSeperatorString(selectedDepartmentList);


            // Get the Facility Rooms list and bind to div #divFacilityRoomList
            if (patientId > 0)
                listtoReturn.AddRange(_service.GetPatientScheduling(patientId, selectedDate, viewtype));
            else
            {
                if (selectedPhysicianList.All(item => item.Id != 0))
                {
                    listtoReturn.AddRange(_service.GetSchedulingByPhysiciansData(phyObj, selectedDate, viewtype,
                        Convert.ToInt32(facilityid)));
                }
            }


            if (selectedDepartmentList != null && selectedDepartmentList.Count > 0 && selectedDepartmentList[0].Id != 0)
            {
                var depList = selectedDepartmentList.Select(i => i.Id.ToString()).ToList();


                //listtoReturn = listtoReturn.Count > 0
                //                       ? listtoReturn.Where(
                //                           x => selectedDepartmentList.Any(x1 => x1.Id.ToString() == x.ExtValue1))
                //                             .ToList() : new List<SchedulingCustomModel>();


                listtoReturn = listtoReturn.Any()
                    ? listtoReturn.Where(a => !string.IsNullOrEmpty(a.ExtValue1) && depList.Contains(a.ExtValue1))
                        .ToList()
                    : new List<SchedulingCustomModel>();

                if (listtoReturn.Count == 0)
                {
                    var schList = _service.GetSchedulingDataByDepartments(depList, selectedDate, viewtype,
                        Convert.ToInt32(facilityid));
                    listtoReturn.AddRange(schList);

                    //foreach (var item in selectedDepartmentList)
                    //{
                    //    listtoReturn.AddRange(
                    //        _service.GetSchedulingDeptDataByType(
                    //            item.Id,
                    //            selectedDate,
                    //            viewtype,
                    //            facilityid));
                    //}
                }
            }


            if (selectedRoomsList != null)
            {
                // Consider External value 1 as the Department
                if (selectedRoomsList[0].Id != 0)
                {
                    var roomsList = selectedRoomsList.Select(s => s.Id).ToList();

                    //listtoReturn = listtoReturn.Count > 0
                    //                   ? listtoReturn.Where(
                    //                       x => selectedRoomsList.Any(x1 => x1.Id == x.RoomAssigned))
                    //                         .ToList()
                    //                         : new List<SchedulingCustomModel>();

                    listtoReturn = listtoReturn.Count > 0
                        ? listtoReturn.Where(
                            x => x.RoomAssigned.HasValue && roomsList.Contains(x.RoomAssigned.Value)).ToList()
                        : new List<SchedulingCustomModel>();

                    if (listtoReturn.Count == 0)
                    {
                        var schList = _service.GetSchedulingDataByRooms(roomsList, selectedDate);
                        listtoReturn.AddRange(schList);
                    }
                }
            }

            listtoReturn = listtoReturn.Where(x => selectedStatusList.Any(x1 => x1.Id.ToString() == x.Status)).ToList();

            var textlist = GetListSectionWise(listtoReturn, "others");
            return Json(textlist, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the custom schedular.
        /// </summary>
        /// <param name="filters">The filters.</param>
        /// <returns></returns>
        public ActionResult GetCustomSchedular(List<SchedularTypeCustomModel> filters)
        {
            var listtoReturn = new List<SchedulingCustomModel>();
            var selectedPhysicianList = filters[0].PhysicianId;
            var selectedDate = filters[0].SelectedDate;
            var selectedfacility = filters[0].Facility;
            var selectedViewType = filters[0].ViewType;
            if (selectedPhysicianList.Count > 0)
            {
                var phyList = selectedPhysicianList.Select(p => p.Id.ToString()).ToList();
                var list = _service.GetSchedulingDataByType(phyList, selectedDate, selectedViewType,
                    Convert.ToInt32(selectedfacility));
                listtoReturn.AddRange(list);
            }

            var hList = _service.GetHolidayPlannerData(0, Convert.ToDateTime(selectedDate), "2",
                Convert.ToInt32(selectedfacility));
            listtoReturn.AddRange(hList);

            var textlist = GetListSectionWise(listtoReturn, "others");
            return Json(textlist, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Loads the schedulng data.
        /// </summary>
        /// <param name="selectedDate">The selected date.</param>
        /// <param name="facility">The facility.</param>
        /// <param name="viewType">Type of the view.</param>
        /// <returns></returns>
        public ActionResult LoadSchedulngData(string selectedDate, int facility, string viewType)
        {
            var listtoconvert = new List<SchedulingCustomModel>();
            var selectedViewType = viewType;
            var cId = Convert.ToString(Helpers.GetSysAdminCorporateID());
            cId = (facility) == 0
                ? cId
                : Convert.ToString(Helpers.GetCorporateIdByFacilityId(Convert.ToInt32(facility)));
            var isAdmin = Helpers.GetLoggedInUserIsAdmin();
            var userid = Helpers.GetLoggedInUserId();
            var corporateUsers = _phService.GetCorporatePhysiciansList(
                Convert.ToInt32(cId), isAdmin, userid, facility);

            var selectedPhysicians = new List<SchedularFiltersCustomModel>();
            if (corporateUsers.Count > 0)
            {
                selectedPhysicians.AddRange(corporateUsers.Select(item => new SchedularFiltersCustomModel { Id = item.Physician.Id, Name = item.Physician.PhysicianName }));

                DateTime dateValue;
                if (DateTime.TryParse(selectedDate, out dateValue))
                {
                    dateValue = Helpers.GetInvariantCultureDateTime();
                }

                var phyObj = ConvertArrayToCommaSeperatorString(selectedPhysicians);
                listtoconvert.AddRange(
                    _service.GetSchedulingByPhysiciansData(
                        phyObj,
                        Convert.ToDateTime(dateValue),
                        selectedViewType,
                        Convert.ToInt32(facility)));
            }


            var listtoreturn = new
            {
                savedSlots = GetListSectionWise(listtoconvert, "others"),
                selectedPhysicians,

                // departmentTimmingsList = GetDepartmentTimmingCustomModel(departmentOpeningSlotslist)
            };
            return Json(listtoreturn, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Saves the holiday planner scheduling.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public JsonResult SaveHolidayPlannerScheduling(List<SchedulingCustomModel> model)
        {
            var list = new List<SkippedHolidaysData>();
            var notAvialableTimeslotslist = new List<NotAvialableTimeSlots>();
            if (model != null)
            {
                var corporateId = Helpers.GetSysAdminCorporateID();
                var randomnumber = Helpers.GenerateCustomRandomNumber();
                // check to add the physician department
                var physicianBal = _uService.GetPhysicianById(Convert.ToInt32(model[0].AssociatedId));
                var physicianDeptid = physicianBal != null ? physicianBal.FacultyDepartment : string.Empty;
                //var eventId = model[0].SchedulingId != 0
                //                  ? _service.GetEventIdbySchedulingId(model[0].SchedulingId)
                //                  : string.Empty;

                // ..... Remove the Old Data for the user in case of multiple Holidays/Leaves
                if (model[0].SchedulingId != 0)
                {
                    if (model[0].MultipleProcedures)
                    {
                        _service.DeleteHolidayPlannerData(model[0].EventParentId, 0, Convert.ToInt32(model[0].SchedulingType), model[0].ExternalValue3);
                    }
                    else
                    {
                        _service.DeleteHolidayPlannerData(string.Empty, model[0].SchedulingId, Convert.ToInt32(model[0].SchedulingType), model[0].ExternalValue3);
                    }
                }

                #region Commented Code

                #endregion
                foreach (var item in model)
                {
                    var newEventId = Helpers.GenerateCustomRandomNumber();


                    item.ScheduleFrom = new DateTime(
                        item.ScheduleTo.Value.Year,
                        item.ScheduleTo.Value.Month,
                        item.ScheduleTo.Value.Day,
                        0,
                        0,
                        0);
                    item.ScheduleTo = new DateTime(
                        item.ScheduleTo.Value.Year,
                        item.ScheduleTo.Value.Month,
                        item.ScheduleTo.Value.Day,
                        23,
                        59,
                        0);
                    item.AssociatedId = item.AssociatedId != 0 ? item.AssociatedId : 0;
                    item.CorporateId = corporateId;
                    item.ExtValue1 = physicianDeptid;
                    item.CreatedBy = Helpers.GetLoggedInUserId();
                    item.CreatedDate = Helpers.GetInvariantCultureDateTime();
                    item.EventId = newEventId; //string.IsNullOrEmpty(eventId) ? item.EventId : eventId;
                    item.WeekDay = Helpers.GetWeekOfYearISO8601(Convert.ToDateTime(item.ScheduleFrom)).ToString();
                    item.IsActive = true;
                    item.EventParentId = string.IsNullOrEmpty(item.EventParentId)
                                             ? randomnumber
                                             : item.EventParentId;

                    //Added the Type Of Visit
                    item.TypeOfVisit = item.Comments;

                    if (string.IsNullOrEmpty(item.ExtValue2) && item.SchedulingType == "2")
                    {
                        var isSlotAvailable = _service.CheckForDuplicateRecord(
                            item.SchedulingId,
                            item.SchedulingType,
                            Convert.ToDateTime(item.ScheduleFrom),
                            Convert.ToDateTime(item.ScheduleTo),
                            Convert.ToInt32(item.AssociatedId),
                            Convert.ToInt32(item.AssociatedId),
                            Convert.ToInt32(item.FacilityId));

                        if (isSlotAvailable)
                        {
                            notAvialableTimeslotslist.Add(
                                _service.GetNotAvialableTimeSlotsCustomModel(item));

                            // model.RemoveAt(index);
                            var listobjtoReturn =
                                new
                                {
                                    IsError = true,
                                    notAvialableTimeslotslist,
                                };
                            return Json(listobjtoReturn, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                list = _service.SaveHolidayScheduling(model);
            }
            return Json(new { IsError = false, SkippedHolidaysData1 = list }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method is used to delete the scheduling
        /// </summary>
        /// <param name="eventParentId">The event parent identifier.</param>
        /// <param name="schedulingId">The scheduling identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteSchduling(string eventParentId, string schedulingId, int schedulingType, string externalValue3)
        {
            var isDeleted = false;
            if (!string.IsNullOrEmpty(eventParentId))
                isDeleted = _service.DeleteHolidayPlannerData(eventParentId, 0, schedulingType, externalValue3);
            else
                isDeleted = _service.DeleteHolidayPlannerData(eventParentId, Convert.ToInt32(schedulingId), schedulingType, externalValue3);
            return Json(isDeleted, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the available time slots.
        /// </summary>
        /// <param name="facilityid">The facilityid.</param>
        /// <param name="physicianId">The physician identifier.</param>
        /// <param name="dateselected">The dateselected.</param>
        /// <param name="typeofproc">The typeofproc.</param>
        /// <returns></returns>
        public ActionResult GetAvailableTimeSlots(int facilityid, int physicianId, DateTime dateselected, string typeofproc)
        {
            DateTime timeSlotDate = DateTime.Now;
            var avialableTimeslotListing = _service.GetAvailableTimeSlots(facilityid, physicianId, dateselected, typeofproc, out timeSlotDate);
            return Json(avialableTimeslotListing, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Gets the available time slots.
        /// </summary>
        /// <param name="facilityid">The facilityid.</param>
        /// <param name="physicianId">The physician identifier.</param>
        /// <param name="dateselected">The dateselected.</param>
        /// <param name="typeofproc">The typeofproc.</param>
        /// <returns></returns>
        public ActionResult GetClosestAvailableTimeSlots(int facilityid, int physicianId, DateTime dateselected, string typeofproc)
        {
            DateTime timeSlotDate = DateTime.Now;
            var list = _service.GetAvailableTimeSlots(facilityid, physicianId, dateselected, typeofproc, out timeSlotDate, firstAvailable: true);
            var jsonData = new { list, dt = timeSlotDate.ToShortDateString() };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAvailableTimeSlotsUpdated(int facilityid, int physicianId, DateTime dateselected, string typeofproc)
        {
            var timeSlotDate = DateTime.Now;
            var avialableTimeslotListing = _service.GetAvailableTimeSlots(facilityid, physicianId, dateselected, typeofproc, out timeSlotDate);
            var deptOpeningDays = _service.GetDeptOpeningDaysForPhysician(physicianId);
            var objToReturn =
                new { avialableTimeslotListing, deptOpeningDays };
            return Json(objToReturn, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the patient information scheduling search result.
        /// </summary>
        /// <param name="common">The common.</param>
        /// <returns></returns>
        public ActionResult GetPatientInfoSchedulingSearchResult(CommonModel common)
        {
            var facilityid = Helpers.GetDefaultFacilityId();
            var corporateid = Helpers.GetSysAdminCorporateID();
            common.FacilityId = facilityid;
            common.CorporateId = corporateid;

            var objPatientInfoData = _piService.GetPatientSearchResult(common);
            ViewBag.Message = null;
            return PartialView(PartialViews.PatientSearchResultPView, objPatientInfoData);
        }

        /// <summary>
        /// Gets the patient scheduling.
        /// </summary>
        /// <param name="filters">The filters.</param>
        /// <returns></returns>
        public ActionResult GetPatientScheduling(List<SchedularTypeCustomModel> filters)
        {
            var listtoReturn = new List<SchedulingCustomModel>();

            var selectedStatusList = filters[0].StatusType;
            var selectedDate = filters[0].SelectedDate;
            var viewtype = filters[0].ViewType;
            var patientId = filters[0].PatientId;

            var list1 = _service.GetPatientScheduling(patientId, selectedDate, viewtype);
            list1 = list1.Where(a => selectedStatusList.Any(a1 => a1.Id.ToString() == a.Status)).ToList();
            listtoReturn.AddRange(list1);
            var list2 = _service.GetPatientNextScheduling(patientId, selectedDate);
            var patietnNextAppointmentCus = GetListSectionWise(list2, "others");

            var textlist = GetListSectionWise(listtoReturn, "others");
            var jsonObjectToReturn = new
            {
                mainList = textlist,
                patientNextAppointments = patietnNextAppointmentCus
            };
            return Json(jsonObjectToReturn, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the scheduling by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult GetSchedulingById(int id)
        {
            var schedulingobj = _service.GetSchedulingCustomModelById(id);
            var objToreturn = ConvertSchedulingObjToCustomModelForCalender(schedulingobj);
            return Json(objToreturn, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method to get the data for Over View popup
        /// </summary>
        /// <param name="oSchedularOverViewCustomModel"></param>
        /// <returns></returns>
        public ActionResult GetOverView(SchedularOverViewCustomModel oSchedularOverViewCustomModel)
        {
            var objToreturn = _service.GetOverView(oSchedularOverViewCustomModel);
            var t1 = Json(new
            {
                aaData = objToreturn
            }, JsonRequestBehavior.AllowGet);
            t1.MaxJsonLength = int.MaxValue;
            return t1;
        }

        /// <summary>
        /// Gets the phy previous vacations.
        /// </summary>
        /// <param name="facilityid">The facilityid.</param>
        /// <param name="physicianId">The physician identifier.</param>
        /// <returns></returns>
        public ActionResult GetPhyPreviousVacations(int facilityid, int physicianId)
        {
            var objtoReturn = _service.GetPhyPreviousVacations(facilityid, physicianId);
            var textlist = GetListSectionWise(objtoReturn, "others");
            var t1 = Json(new
            {
                objtoReturn = textlist
            }, JsonRequestBehavior.AllowGet);
            t1.MaxJsonLength = int.MaxValue;
            return t1;
        }
        #endregion

        #region New Code

        /*
         * Who: Amit Jain
         * When: 13 March, 2016
         * What: Get All Ajax Calls executed on Page-Load in one Ajax Request.
         * Why: Optimize the Screen
         */
        public JsonResult GetAllSchedulingData(string category, int facilityId, string corporateId, string selectedDate, string viewType)
        {
            List<DropdownListData> finalGcList = null;
            List<DropdownListData> hTypes = null;
            List<DropdownListData> hStatus = null;
            var depList = new List<DropdownListData>();
            List<PhysicianCustomModel> pList = null;
            List<SelectListItem> fList = null;
            List<DropdownListData> specialities = null;
            List<DropdownListData> mWeekDays = null;
            List<CountryCustomModel> countries = null;
            var appTypes = new List<AppointmentTypesCustomModel>();

            var selectedPhysicians = new List<SchedularFiltersCustomModel>();
            List<SchedulerCustomModelForCalender> savedSlots = null;
            string facilityView;
            string fDepView;
            string fRoomsView;
            string viewpath;
            var phyView = string.Empty;
            string bStatusView;

            var cId = Helpers.GetSysAdminCorporateID();
            var userisAdmin = Helpers.GetLoggedInUserIsAdmin();
            var loggedInUserId = Helpers.GetLoggedInUserId();

            //#################--Get Physicians' Availibility Data  Code starts here--#####################
            var extValues1 = new[] { "1", "3", "4" };
            var gcList = _gService.GetGCodesListByCategoryValue(category, extValues1, string.Empty);
            if (gcList.Count > 0)
            {
                finalGcList = gcList.Where(g => g.ExternalValue1.Equals("1")).ToList();
                hTypes = gcList.Where(g => g.ExternalValue1.Equals("3")).ToList();
                hStatus = gcList.Where(g => g.ExternalValue1.Equals("4")).ToList();

                pList = _phService.GetPhysicians(cId, userisAdmin, loggedInUserId, facilityId);

                //Bind Physicians to Dropdown

                if (pList.Any())
                {
                    selectedPhysicians.AddRange(pList.Select(item => new SchedularFiltersCustomModel
                    {
                        Id = item.Physician.Id,
                        Name = item.Physician.PhysicianName
                    }));
                }

                //Bind the physicians list to div #divPhysicianList
                viewpath = string.Format(partialViewPath, PartialViews.PhysicianCheckBoxList);
                phyView = RenderPartialViewToStringBase(viewpath, pList);
            }
            //#################--Get Physicians' Availibility Data Code ends here--##########################


            //######################--"Get Specialities and Monthly Week Days" Code Starts Here--#####################
            var categories = new List<string> { "1121", "901" };
            var gc = _gService.GetListByCategoriesRange(categories);
            if (gc.Count > 0)
            {
                specialities = gc.Where(f => f.ExternalValue1.Equals("1121")).ToList();
                mWeekDays = gc.Where(f => f.ExternalValue1.Equals("901")).OrderBy(f => f.SortOrder).ToList();
            }
            //######################--"Get Specialities and Monthly Week Days" Code ends Here--#####################



            //######################--"Get Holiday Status View" Code Starts Here--#####################

            //Get Holiday List and bind to div #divStatusList
            var globalCodelist = _gService.GetGCodesListByCategoryValue(category, "3", "1").OrderBy(x => Convert.ToInt32(x.GlobalCodeValue)).ToList();
            var skippedStatuses = new List<string> { "10", "11", "12", "7", "9" };
            globalCodelist = globalCodelist.Where(x => !skippedStatuses.Any(s => s.Equals(x.GlobalCodeValue))).ToList();

            viewpath = string.Format(partialViewPath, PartialViews.StatusCheckBoxList);
            bStatusView = RenderPartialViewToStringBase(viewpath, globalCodelist);

            //######################--"Get Holiday Status View" Code Starts Here--#####################


            //######################--"Get Saved Slots" Code Starts Here--#####################
            if (selectedPhysicians.Any())
            {
                var dateValue = Helpers.GetInvariantCultureDateTime();
                var phyObj = ConvertArrayToCommaSeperatorString(selectedPhysicians);

                //var listtoconvert = new List<SchedulingCustomModel>();
                //listtoconvert.AddRange(_service.GetSchedulingByPhysiciansData(phyObj, dateValue, viewType,
                //    facilityId));

                //if (listtoconvert.Any())
                //    savedSlots = GetListSectionWise(listtoconvert, "others");

                savedSlots = _service.GetSchedulerData(Convert.ToInt32(viewType), dateValue, phyObj,
                    Convert.ToInt32(facilityId), string.Empty, string.Empty, string.Empty, "others", 0);
            }
            //######################--"Get Saved Slots" Code Ends Here--#####################


            //#################--Get Facilities Data starts here--###############################
            var ff = userisAdmin ? _fService.GetFacilities(cId) : _fService.GetFacilities(cId, Helpers.GetDefaultFacilityId());
            //var isSysAdminCorporate = Helpers.GetSysAdminCorporateID() == 6;
            //var ff = isSysAdminCorporate ? facBal.GetFacilities(cId) : facBal.GetFacilities(cId, Helpers.GetDefaultFacilityId());

            //Bind Facilities for dropdown
            if (ff.Any())
            {
                ff = ff.Where(a => a.CreatedBy != 1).ToList();
                fList = new List<SelectListItem>();
                fList.AddRange(ff.Select(item => new SelectListItem
                {
                    Text = item.FacilityName,
                    Value = Convert.ToString(item.FacilityId),
                }));

                fList = fList.OrderBy(f => f.Text).ToList();
            }

            // Get the facilities list View and bind to div #divLocationList
            viewpath = string.Format(partialViewPath, PartialViews.LocationListView);
            facilityView = RenderPartialViewToStringBase(viewpath, ff);
            //#################--Get Facilities Data ends here--#################################


            //######################--"Get Facility Departments and rooms" Code starts Here--#####################

            // Get the facility Department list and bind to div #divFacilityDepartmentList
            var facilityDepartmentList = _fsService.GetFacilityDepartments(cId, Convert.ToString(facilityId));

            if (facilityDepartmentList.Count > 0)
            {
                depList.AddRange(facilityDepartmentList.Select(f => new DropdownListData
                {
                    Text = f.FacilityStructureName,
                    Value = Convert.ToString(f.FacilityStructureId)
                }));
            }


            viewpath = string.Format(partialViewPath, PartialViews.FacilityDepartmentListView);
            fDepView = RenderPartialViewToStringBase(viewpath, facilityDepartmentList);


            // Get the Facility Rooms list and bind to div #divFacilityRoomList
            var fRooms = _fsService.GetFacilityRoomsCustomModel(cId, Convert.ToString(facilityId));
            viewpath = string.Format(partialViewPath, PartialViews.FacilityRoomsListView);
            fRoomsView = RenderPartialViewToStringBase(viewpath, fRooms);

            //######################--"Get Facility Departments and rooms" Code Ends Here--#####################




            //######################--"Get Appointment Types" Code starts Here--#####################

            appTypes = _atService.GetAppointmentTypesData(cId, facilityId, true);


            //######################--"Get Appointment Types" Code Ends Here--#####################


            countries = _cService.GetCountryWithCode();

            SchedulingParametersCustomModel parm = null;
            parm = _spService.GetDataByFacilityId(facilityId);



            var jsonData = new
            {
                gClist = finalGcList,
                hTypes,
                hStatus,
                pList,
                fList,
                specialities,
                mWeekDays,
                savedSlots,
                selectedPhysicians,
                facilityView,
                fDepView,
                fRoomsView,
                phyView,
                bStatusView,
                countries,
                appTypes,
                depList,
                StartHour = parm != null ? parm.StartHour : 0,
                EndHour = parm != null ? parm.EndHour : 0
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult OnPhysicianSelectionFromPreviousEncountersList(string category, int facilityId, int facilityStructureId)
        {
            List<DropdownListData> finalGcList = null;
            List<PhysicianCustomModel> pList;
            //List<SchedularFiltersCustomModel> selectedPhysicians = new List<SchedularFiltersCustomModel>();
            var appTypes = new List<AppointmentTypes>();

            var cId = Helpers.GetSysAdminCorporateID();
            var isAdmin = Helpers.GetLoggedInUserIsAdmin();
            var userid = Helpers.GetLoggedInUserId();

            //#################--Get Availibility Data Code starts here--#####################
            var extValues1 = new[] { "1" };
            var gcList = _gService.GetGCodesListByCategoryValue(category, extValues1, string.Empty);
            if (gcList.Count > 0)
                finalGcList = gcList.Where(g => g.ExternalValue1.Equals("1")).ToList();

            pList = _phService.GetPhysicians(cId, isAdmin, userid, facilityId);
            //#################--Get Availibility Data Code ends here--##########################

            //Appointment Types List

            appTypes = _fsService.GetDepartmentAppointmentTypes(facilityStructureId, Convert.ToString(facilityId));

            var jsonData = new
            {
                finalGcList,
                pList,
                appTypes
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BindLocationDataInScheduler(int facilityId)
        {
            string fDepView;
            string viewpath;
            string phyView;
            var cId = Helpers.GetSysAdminCorporateID();
            var userisAdmin = Helpers.GetLoggedInUserIsAdmin();
            var loggedInUserId = Helpers.GetLoggedInUserId();

            //######################--"Get Physicians" Code starts Here--#####################
            var pList = _phService.GetPhysicians(cId, userisAdmin, loggedInUserId, facilityId);

            //Bind the physicians list to div #divPhysicianList
            viewpath = string.Format(partialViewPath, PartialViews.PhysicianCheckBoxList);
            phyView = RenderPartialViewToStringBase(viewpath, pList);
            //######################--"Get Physicians" Code ends Here--#####################


            //######################--"Get Facility Departments and rooms" Code starts Here--#####################
            // Get the facility Department list and bind to div #divFacilityDepartmentList
            var facilityDepartmentList = _fsService.GetFacilityDepartments(cId, Convert.ToString(facilityId));
            viewpath = string.Format(partialViewPath, PartialViews.FacilityDepartmentListView);
            fDepView = RenderPartialViewToStringBase(viewpath, facilityDepartmentList);
            //######################--"Get Facility Departments and rooms" Code Ends Here--#####################

            var jsonData = new { phyView, fDepView };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region Overview Data

        public JsonResult OnChangeFacilityDropdownInAppointmentsOverview(int fId)
        {
            List<PatientInfo> pInfo;
            List<AppointmentTypesCustomModel> aList;
            List<PhysicianCustomModel> physicians;

            var deps = new List<DropdownListData>();
            var cId = Helpers.GetSysAdminCorporateID();
            var isAdmin = Helpers.GetLoggedInUserIsAdmin();
            var userid = Helpers.GetLoggedInUserId();

            //Patients List
            pInfo = _piService.GetPatientNames(fId, cId);

            //Appointment Types List
            aList = _atService.GetAppointmentTypesData(cId, fId, true);

            //Facility Departments 
            var facilityDepartments = _fsService.GetFacilityDepartments(cId, Convert.ToString(fId));
            if (facilityDepartments.Any())
            {
                deps.AddRange(facilityDepartments.Select(item => new DropdownListData
                {
                    Text = string.Format(" {0} ", item.FacilityStructureName),
                    Value = Convert.ToString(item.FacilityStructureId),
                    //ExternalValue1 = _service.Get
                }));
            }

            //Physicians by Facility
            physicians = _phService.GetPhysicians(cId, isAdmin, userid, fId);


            var jsonData = new
            {
                pInfo,
                aList,
                deps,
                physicians
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public ActionResult GetFacilityHolidays(int facilityid)
        {
            var objtoReturn = _service.GetFacilityHolidays(facilityid);
            var textlist = GetListSectionWise(objtoReturn, "others");
            var t1 = Json(new
            {
                objtoReturn = textlist
            }, JsonRequestBehavior.AllowGet);
            t1.MaxJsonLength = int.MaxValue;
            return t1;
        }

        /*---Here, id is being used as EventParentID of Scheduler---*/
        public ActionResult DeleteHoliday(string id, int facilityid)
        {
            var objtoReturn = _service.DeleteHolidaysByEventParentID(id);
            return Json(objtoReturn, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateAppointmentStatus(string status, int id, List<SchedularTypeCustomModel> filters)
        {
            var result = false;
            result = _service.UpdateAppointmentStatus(id, status, Helpers.GetLoggedInUserId(), Helpers.GetInvariantCultureDateTime());

            if (result)
            {
                var jsonResult = GetSchedulerDataUpdated(filters);
                jsonResult.MaxJsonLength = int.MaxValue;
                jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return jsonResult;
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> SaveAppointment(List<SchedulingCustomModel> model)
        {
            var isAllReadyAppointed = false;
            var notAvialableTimeslotslist = new List<NotAvialableTimeSlots>();
            var roomEquipmentnotaviable = new List<NotAvialableRoomEquipment>();

            if (model != null)
            {
                var facilityId = Helpers.GetDefaultFacilityId();
                var corporateId = Helpers.GetSysAdminCorporateID();
                var loggedInUserId = Helpers.GetLoggedInUserId();
                var currentDateTime = Helpers.GetInvariantCultureDateTime();
                var token = CommonConfig.GenerateLoginCode(8, false);

                var pId = model[0].AssociatedId != 0 ? Convert.ToInt32(model[0].AssociatedId) : 0;
                var pAge = Helpers.GetAgeByDate(model[0].PatientDOB.Value);
                var pName = model[0].PatientName.Split(' ');
                if (pName.Length > 0)
                {
                    var pFirstName = pName[0].Trim();
                    var pLastName = model[0].PatientName.Replace(pFirstName, string.Empty).Trim();

                    pId = _service.SavePatientInfoInScheduler(Convert.ToInt32(corporateId),
                           Convert.ToInt32(model[0].FacilityId), currentDateTime, pId, pFirstName, pLastName,
                           model[0].PatientDOB,
                           model[0].PatientEmailId, model[0].PatientEmirateIdNumber, loggedInUserId,
                           model[0].PatientPhoneNumber, pAge);

                    if (pId == -2)
                    {
                        var listobjtoReturn = new
                        {
                            IsError = true,
                            patientid = model[0].PatientId,
                            errorMessage = "",
                            ErrorStatus = -2
                        };
                        return Json(listobjtoReturn, JsonRequestBehavior.AllowGet);
                    }

                    model[0].PatientId = pId;
                    model[0].AssociatedId = pId;
                }

                //-------------------########### SAVING PATIENT DETAILS SECTION ends here ###########-----------------------------
                var dtScheduling = Helpers.ToDataTable(_service.MapVMToModel(model));

                var isValid = 0;
                var existingList = _service.ValidateScheduling(dtScheduling, facilityId, loggedInUserId, out isValid);


                //Here IsValid must having ZERO value as Valid Scheduling
                if (existingList.Any() || isValid > 0)
                {
                    var listobjtoReturn = new
                    {
                        IsError = true,
                        patientid = model[0].PatientId,
                        booked = isValid == 2 ? existingList : new List<SchedulingCustomModel>(),
                        errorMessage = ResourceKeyValues.GetKeyValue("errsametimeslot"),
                        ErrorStatus = isValid
                    };
                    return Json(listobjtoReturn, JsonRequestBehavior.AllowGet);
                }

                if (Convert.ToInt32(model[0].PatientId) == 0)
                    model[0].PatientId = pId;

                var randomnumber = Helpers.GenerateCustomRandomNumber();
                var counter = 1;


                var ss = model.Where(s => s.IsRecurring != true).GroupBy(g => g.ScheduleFrom).Where(f => f.Count() > 1).Select(y => y.Key).ToList();
                if (ss.Count > 0)
                {
                    var dtFrom = ss[0].HasValue ? ss[0].Value : DateTime.Now;
                    var item = model.FirstOrDefault(a => a.ScheduleFrom == dtFrom);

                    var listobjtoReturn = new
                    {
                        IsError = true,
                        SameTimeApp = true,
                        isAllReadyAppointed,
                        patientid = model[0].PatientId,
                        notAvialableTimeslotslist,
                        roomEquipmentnotaviable,
                        appId = item.TypeOfProcedure,
                    };
                    return Json(listobjtoReturn, JsonRequestBehavior.AllowGet);
                }

                foreach (var item in model)
                {
                    var newId = Helpers.GenerateCustomRandomNumber();
                    var eventId = string.Format("{0}{1}", newId, counter);
                    counter++;
                    item.AssociatedId = item.AssociatedId != 0 ? item.AssociatedId : pId;
                    item.CorporateId = corporateId;

                    ///TODO: Need to Confirm the below line.
                    /*
                       SPECIAL NOTE: Need to confirm the Department
                     * Issue: Currently, the Department ID being saved into Scheduling, is the one that belongs to Physician 
                     * but that should be the one that belongs to the ROOM Assigned.
                     */
                    //item.ExtValue1 = physicianDeptid;
                    item.SchedulingId = item.SchedulingId != 0 ? item.SchedulingId : 0;
                    item.CreatedBy = loggedInUserId;
                    item.CreatedDate = currentDateTime;
                    item.EventId = eventId;
                    item.ExtValue4 = token;
                    var app = _atService.GetAppointmentTypesById(Convert.ToInt32(item.TypeOfProcedure));
                    var appointmentType = app != null ? app.Name : string.Empty;

                    //Added the type of Appointment Type Name, on 23 June, 2017
                    item.TypeOfVisit = !string.IsNullOrEmpty(appointmentType) ? appointmentType : item.Comments;


                    /*
                     * Updated BY: Amit Jain
                     * On: 30 Jan, 2016
                     * Here, In case Equipment Required is False, that means System doesn't need an appointment 
                     * for that specific Appointment Type or Procedure Type. 
                     * Otherwise, Check If Equipment ID has some value. Now, After checking, it still shows 0, 
                     * that means it doesn't have any Equipment available. And in that case, it shouldn't allow 
                     * the User to save the Appointment and so, show proper message to the User.
                     */
                    //Check should be bypassed if Appointment types are with the tag 'No Equipment Required'
                    var equipmentRequired = app != null && (!string.IsNullOrEmpty(app.ExtValue1) &&
                                                            int.Parse(app.ExtValue1) == 1);

                    if (!equipmentRequired)
                        item.EquipmentAssigned = 0;

                    if (item.EquipmentAssigned == 0 && equipmentRequired)
                    {
                        roomEquipmentnotaviable.Add(new NotAvialableRoomEquipment
                        {
                            EquipmentId = Convert.ToInt32(item.EquipmentAssigned),
                            EquipmentNameId = string.Empty,
                            TypeOfProcedureStr = appointmentType,
                            RoomName = string.Empty,
                            RoomId = 0,
                            Reason = "Equipment is not available in the selected timeslot for procedure " + appointmentType
                        });
                        var listobjtoReturn = new
                        {
                            IsError = true,
                            SameTimeApp = true,
                            patientid = model[0].PatientId,
                            notAvialableTimeslotslist,
                            roomEquipmentnotaviable,
                        };
                        return Json(listobjtoReturn, JsonRequestBehavior.AllowGet);
                    }

                    item.AppointmentType = appointmentType;
                    item.WeekDay = Convert.ToString(Helpers.GetWeekOfYearISO8601(Convert.ToDateTime(item.ScheduleFrom)));
                    item.IsActive = true;

                    item.EventParentId = item.IsRecurring == false || string.IsNullOrEmpty(item.EventParentId)
                        ? randomnumber
                        : item.EventParentId;

                    item.RecEventPId = item.IsRecurring == false || string.IsNullOrEmpty(item.EventParentId)
                        ? Convert.ToInt32(randomnumber)
                        : Convert.ToInt32(item.EventParentId);

                    // This method call will tell that the room and equipment is avialable for the type of procedure and for the selected date
                    var roomObj = _service.GetAssignedRoomForProcedure(Convert.ToInt32(item.FacilityId),
                        Convert.ToInt32(item.TypeOfProcedure),
                        Convert.ToDateTime(item.ScheduleFrom),
                        Convert.ToDateTime(item.ScheduleFrom).ToString("HH:mm"),
                        Convert.ToDateTime(item.ScheduleTo).ToString("HH:mm"), Convert.ToInt32(item.RoomAssigned)
                        , item.SchedulingId, Convert.ToInt32(item.AssociatedId));

                    //Check Patient All ready booked appointment with the same time.
                    isAllReadyAppointed = roomObj != null && roomObj.IsAppointed;
                    if (isAllReadyAppointed)
                    {
                        var listobjtoReturn = new
                        {
                            IsError = true,
                            SameTimeApp = true,
                            isAllReadyAppointed,
                            patientid = model[0].PatientId,
                            notAvialableTimeslotslist,
                            roomEquipmentnotaviable,
                            appId = item.TypeOfProcedure
                        };
                        return Json(listobjtoReturn, JsonRequestBehavior.AllowGet);
                    }


                    if (roomObj != null)
                    {
                        item.RoomAssigned = roomObj.RoomId;
                        item.EquipmentAssigned = roomObj.EquipmentId;
                        item.ExtValue1 = Convert.ToString(roomObj.DepartmentId);
                        equipmentRequired = roomObj.IsEquipmentRequired;
                    }


                    if (item.EquipmentAssigned == 0 && equipmentRequired)
                    {
                        roomEquipmentnotaviable.Add(new NotAvialableRoomEquipment
                        {
                            EquipmentId = Convert.ToInt32(item.EquipmentAssigned),
                            EquipmentNameId = string.Empty,
                            TypeOfProcedureStr = appointmentType,
                            RoomName = string.Empty,
                            RoomId = 0,
                            Reason = "Equipment is not available in the selected timeslot for procedure " + appointmentType
                        });
                        var listobjtoReturn = new
                        {
                            IsError = true,
                            SameTimeApp = true,
                            patientid = model[0].PatientId,
                            notAvialableTimeslotslist,
                            roomEquipmentnotaviable,
                        };
                        return Json(listobjtoReturn, JsonRequestBehavior.AllowGet);
                    }
                    if (Convert.ToInt32(item.RoomAssigned) == 0)
                    {
                        roomEquipmentnotaviable.Add(new NotAvialableRoomEquipment
                        {
                            EquipmentId = 0,
                            EquipmentNameId = string.Empty,
                            TypeOfProcedureStr = appointmentType,
                            RoomName = string.Empty,
                            RoomId = Convert.ToInt32(item.RoomAssigned),
                            Reason =
                                "Room is not available in the selected timeslot for Procedure: " + appointmentType
                        });

                        var listobjtoReturn = new
                        {
                            IsError = true,
                            SameTimeApp = true,
                            patientid = model[0].PatientId,
                            notAvialableTimeslotslist,
                            roomEquipmentnotaviable
                        };
                        return Json(listobjtoReturn, JsonRequestBehavior.AllowGet);
                    }
                }

                var toBeDeletedSchedulings = model.Where(item => !string.IsNullOrEmpty(item.RemovedAppointmentTypes));
                foreach (var item in toBeDeletedSchedulings)
                {
                    if (!string.IsNullOrEmpty(item.RemovedAppointmentTypes))
                    {
                        var schIdsToBeRemoved = item.RemovedAppointmentTypes.Split(',').Select(int.Parse).ToList();
                        _service.RemoveJustDeletedSchedulings(string.Empty, schIdsToBeRemoved, Convert.ToInt32(item.SchedulingType), "");
                    }
                }

                _service.SavePatientScheduling(model);

                var statusList = new[] { "1", "3", "4" };
                if (model.Count > 0 && statusList.Contains(model[0].Status))
                {
                    var status = Convert.ToInt32(model[0].Status) + 1;
                    await Helpers.SendAppointmentNotification(model, model[0].PatientEmailId, Convert.ToString(model[0].EmailTemplateId),
                           Convert.ToInt32(model[0].PatientId), Convert.ToInt32(model[0].PhysicianId), status);
                }
            }

            var listobj = new
            {
                IsError = false,
                patientid = model[0].PatientId,
                notAvialableTimeslotslist,
                roomEquipmentnotaviable
            };

            return Json(listobj, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetFilteredSchedulerData(List<SchedularTypeCustomModel> filters)
        {
            var selectedDepartmentList = filters[0].DeptData;
            var facilityid = filters[0].Facility;
            var selectedRoomsList = filters[0].RoomIds;
            var dep = selectedDepartmentList != null && selectedDepartmentList.Count > 0
                ? ConvertArrayToCommaSeperatorString(selectedDepartmentList)
                : string.Empty;
            var roomsList = selectedRoomsList != null && selectedRoomsList.Count > 0
                ? ConvertArrayToCommaSeperatorString(selectedRoomsList)
                : string.Empty;

            var list = GetSchedulingData(filters, "dep");

            var rList = new List<FacilityStructureRoomsCustomModel>();
            if (!string.IsNullOrEmpty(dep))
            {
                rList = _fsService.GetFacilityRoomsByDepartments(Helpers.GetSysAdminCorporateID(), facilityid, dep, roomsList);
            }


            var jsonResult = new
            {
                SchData = list,
                rList
            };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetSchedulerDataUpdated(List<SchedularTypeCustomModel> filters)
        {
            var list = GetSchedulingData(filters, "others");
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetSchedulingDataByIdNew(int id)
        //{
        //    using (var _service = new _service())
        //    {
        //        var schedulingobj = _service.GetSchedulingCustomModelById(id);
        //        var objToreturn = ConvertSchedulingObjToCustomModelForCalender(schedulingobj);
        //        return Json(objToreturn, JsonRequestBehavior.AllowGet);
        //    }
        //}


        public JsonResult GetAllDataOnPhysicianSelection(string category, int facilityId, int deptId)
        {
            List<DropdownListData> finalGcList = null;
            List<DropdownListData> hTypes = null;
            List<DropdownListData> hStatus = null;
            List<PhysicianCustomModel> pList = null;
            var appTypes = new List<AppointmentTypesCustomModel>();

            var depList = new List<DropdownListData>();
            //List<DropdownListData> specialities = null;
            //List<DropdownListData> mWeekDays = null;
            //List<CountryCustomModel> countries = null;
            var facilityDepartmentList = new List<FacilityStructure>();
            var patients = new List<PatientInfo>();

            //var selectedPhysicians = new List<SchedularFiltersCustomModel>();
            //List<SchedulerCustomModelForCalender> savedSlots = null;
            //string facilityView;
            //string fDepView;
            //string fRoomsView;
            //string viewpath;
            //string bStatusView;

            var cId = Helpers.GetSysAdminCorporateID();
            var userisAdmin = Helpers.GetLoggedInUserIsAdmin();
            var loggedInUserId = Helpers.GetLoggedInUserId();

            //#################--Get Physicians' Availibility Data  Code starts here--#####################
            var extValues1 = new[] { "1", "3", "4" };
            var gcList = _gService.GetGCodesListByCategoryValue(category, extValues1, string.Empty);
            if (gcList.Count > 0)
            {
                finalGcList = gcList.Where(g => g.ExternalValue1.Equals("1")).ToList();
                hTypes = gcList.Where(g => g.ExternalValue1.Equals("3")).ToList();
                hStatus = gcList.Where(g => g.ExternalValue1.Equals("4")).ToList();

                pList = _phService.GetPhysicians(cId, userisAdmin, loggedInUserId, facilityId);
            }
            //#################--Get Physicians' Availibility Data Code ends here--##########################



            //######################--"Get Facility Departments and rooms" Code starts Here--#####################
            // Get the facility Department list and bind to div #divFacilityDepartmentList
            facilityDepartmentList = _fsService.GetFacilityDepartments(cId, Convert.ToString(facilityId));

            if (facilityDepartmentList.Count > 0)
            {
                depList.AddRange(facilityDepartmentList.Select(f => new DropdownListData
                {
                    Text = f.FacilityStructureName,
                    Value = Convert.ToString(f.FacilityStructureId),
                }));
            }

            appTypes = _fsService.GetDepartmentAppointmentTypes(deptId, facilityId, cId, true);
            //######################--"Get Facility Departments and rooms" Code Ends Here--#####################


            //######################--"Get Appointment Types" Code starts Here--#####################




            //######################--"Get Appointment Types" Code Ends Here--#####################

            //######################--"Get Patients Data" Code starts Here--#####################
            patients = _piService.GetPatientsByFacilityId(facilityId);
            //######################--"Get Patients Data" Code ends Here--#####################


            //using (var cBal = new CountryBal())
            //    countries = cBal.GetCountryWithCode();


            var jsonData = new
            {
                gClist = finalGcList,
                hTypes,
                hStatus,
                pList,
                appTypes,
                patients,
                depList
                //fList,
                //specialities,
                //mWeekDays,
                //savedSlots,
                //selectedPhysicians,
                //facilityView,
                //fDepView,
                //fRoomsView,
                //phyView,
                //bStatusView,
                //countries
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetPatientSchedulingUpdated(List<SchedularTypeCustomModel> filters)
        {

            //List<SchedulingCustomModel> nextList;
            var mainList = GetSchedulingData(filters, "others");
            var list2 = _service.GetPatientNextScheduling(filters[0].PatientId, filters[0].SelectedDate);
            var patietnNextAppointmentCus = GetListSectionWise(list2, "others");

            var jsonObjectToReturn = new
            {
                mainList,
                patientNextAppointments = patietnNextAppointmentCus
            };
            return Json(jsonObjectToReturn, JsonRequestBehavior.AllowGet);
        }


        public JsonResult AddUpdateMainScheduling(List<SchedulingCustomModel> model)
        {
            var notAvialableTimeslotslist = new List<NotAvialableTimeSlots>();
            var roomEquipmentnotaviable = new List<NotAvialableRoomEquipment>();

            if (model != null)
            {
                var corporateId = Helpers.GetSysAdminCorporateID();
                var loggedInUserId = Helpers.GetLoggedInUserId();
                var currentDateTime = Helpers.GetInvariantCultureDateTime();

                var token = CommonConfig.GenerateLoginCode(8, false);


                //-------------------########### SAVING PATIENT DETAILS SECTION starts here  ###########-----------------------------

                var pId = model[0].AssociatedId != 0 ? Convert.ToInt32(model[0].AssociatedId) : 0;
                var pAge =
                    Helpers.GetAgeByDate(model[0].PatientDOB.HasValue
                        ? model[0].PatientDOB.Value
                        : DateTime.Now.AddYears(-10));
                var pName = model[0].PatientName.Split(' ');
                if (pName.Length > 0)
                {
                    var pFirstName = pName[0].Trim();
                    var pLastName = model[0].PatientName.Replace(pFirstName, string.Empty).Trim();

                    pId = _service.SavePatientInfoInScheduler(Convert.ToInt32(corporateId),
                           Convert.ToInt32(model[0].FacilityId), currentDateTime, pId, pFirstName, pLastName,
                           model[0].PatientDOB,
                           model[0].PatientEmailId, model[0].PatientEmirateIdNumber, loggedInUserId,
                           model[0].PatientPhoneNumber, pAge);



                }

                //-------------------########### SAVING PATIENT DETAILS SECTION ends here ###########-----------------------------


                if (Convert.ToInt32(model[0].PatientId) == 0)
                    model[0].PatientId = pId;

                var ss = model.GroupBy(g => g.ScheduleFrom).Where(f => f.Count() > 1).Select(y => y.Key).ToList();
                if (ss.Count > 0)
                {
                    var dtFrom = ss[0].HasValue ? ss[0].Value : DateTime.Now;
                    var item = model.FirstOrDefault(a => a.ScheduleFrom == dtFrom);

                    var listobjtoReturn = new
                    {
                        IsError = true,
                        SameTimeApp = true,
                        isAllReadyAppointed = false,
                        patientid = model[0].PatientId,
                        notAvialableTimeslotslist,
                        roomEquipmentnotaviable,
                        appId = item.TypeOfProcedure
                    };
                    return Json(listobjtoReturn, JsonRequestBehavior.AllowGet);
                }

                var counter = 1;

                var facilityId = model[0].FacilityId;
                string eventParentId;
                if (model.Count > 1 && model.Any(s => s.SchedulingId > 0))
                    eventParentId = model[0].EventParentId;
                else
                    eventParentId = Helpers.GenerateCustomRandomNumber();

                foreach (var item in model)
                {

                    var app = _atService.GetAppointmentTypesById(Convert.ToInt32(item.TypeOfProcedure));
                    var appointmentType = app != null ? app.Name : string.Empty;

                    /*
                     * Updated BY: Amit Jain
                     * On: 30 Jan, 2016
                     * Here, In case Equipment Required is False, that means System doesn't need an appointment 
                     * for that specific Appointment Type or Procedure Type. 
                     * Otherwise, Check If Equipment ID has some value. Now, After checking, it still shows 0, 
                     * that means it doesn't have any Equipment available. And in that case, it shouldn't allow 
                     * the User to save the Appointment and so, show proper message to the User.
                     */
                    //Check should be bypassed if Appointment types are with the tag 'No Equipment Required'
                    var equipmentRequired = app != null && (!string.IsNullOrEmpty(app.ExtValue1) && int.Parse(app.ExtValue1) == 1);


                    if (!equipmentRequired)
                        item.EquipmentAssigned = 0;

                    if (item.EquipmentAssigned == 0 && equipmentRequired)
                    {
                        roomEquipmentnotaviable.Add(new NotAvialableRoomEquipment
                        {
                            EquipmentId = Convert.ToInt32(item.EquipmentAssigned),
                            EquipmentNameId = string.Empty,
                            TypeOfProcedureStr = appointmentType,
                            RoomName = string.Empty,
                            RoomId = 0,
                            Reason =
                                "Equipment is not available in the selected timeslot for procedure " +
                                appointmentType
                        });
                        var listobjtoReturn = new
                        {
                            IsError = true,
                            SameTimeApp = true,
                            patientid = model[0].PatientId,
                            notAvialableTimeslotslist,
                            roomEquipmentnotaviable,
                        };
                        return Json(listobjtoReturn, JsonRequestBehavior.AllowGet);
                    }

                    if (item.SchedulingId > 0)
                    {
                        item.ModifiedBy = loggedInUserId;
                        item.ModifiedDate = currentDateTime;
                    }
                    else
                    {
                        item.CreatedBy = loggedInUserId;
                        item.CreatedDate = currentDateTime;
                    }

                    item.EventParentId = eventParentId;
                    item.EventId = string.Format("{0}{1}", Helpers.GenerateCustomRandomNumber(), counter);
                    counter++;

                    item.SchedulingId = item.SchedulingId != 0 ? item.SchedulingId : 0;
                    item.AssociatedId = item.AssociatedId != 0 ? item.AssociatedId : pId;
                    item.CorporateId = corporateId;
                    item.ExtValue4 = token;
                    //item.EventParentId = (item.IsRecurring == false && item.SchedulingId == 0) ||
                    //                     string.IsNullOrEmpty(item.EventParentId)
                    //    ? eventParentId
                    //    : item.EventParentId;

                    item.RecEventPId = Convert.ToInt32(item.EventParentId);
                    item.AppointmentType = appointmentType;
                    item.WeekDay = Convert.ToString(Helpers.GetWeekOfYearISO8601(Convert.ToDateTime(item.ScheduleFrom)));
                    item.IsActive = true;



                    // This method call will tell that the room and equipment is avialable for the type of procedure and for the selected date
                    var isRoomalloted = _service.GetAssignedRoomForProcedure(Convert.ToInt32(item.FacilityId),
                        Convert.ToInt32(item.TypeOfProcedure), item.ScheduleFrom.Value,
                        Convert.ToDateTime(item.ScheduleFrom).ToString("HH:mm"),
                        Convert.ToDateTime(item.ScheduleTo).ToString("HH:mm"), Convert.ToInt32(item.RoomAssigned)
                        , item.SchedulingId, Convert.ToInt32(item.AssociatedId));

                    //Check Patient All ready booked appointment with the same time.
                    var isAllReadyAppointed = isRoomalloted != null && isRoomalloted.IsAppointed;
                    if (isAllReadyAppointed)
                    {
                        var listobjtoReturn = new
                        {
                            IsError = true,
                            SameTimeApp = true,
                            isAllReadyAppointed = true,
                            patientid = model[0].PatientId,
                            notAvialableTimeslotslist,
                            roomEquipmentnotaviable,
                            appId = item.TypeOfProcedure
                        };
                        return Json(listobjtoReturn, JsonRequestBehavior.AllowGet);
                    }

                    if (isRoomalloted != null)
                    {
                        item.RoomAssigned = isRoomalloted.RoomId;
                        item.EquipmentAssigned = isRoomalloted.EquipmentId;
                        item.ExtValue1 = Convert.ToString(isRoomalloted.DepartmentId);
                    }


                    if (Convert.ToInt32(item.RoomAssigned) == 0)
                    {
                        roomEquipmentnotaviable.Add(new NotAvialableRoomEquipment
                        {
                            EquipmentId = 0,
                            EquipmentNameId = string.Empty,
                            TypeOfProcedureStr = appointmentType,
                            RoomName = string.Empty,
                            RoomId = Convert.ToInt32(item.RoomAssigned),
                            Reason =
                                "Room is not available in the selected timeslot for Procedure: " + appointmentType
                        });

                        var listobjtoReturn = new
                        {
                            IsError = true,
                            SameTimeApp = true,
                            patientid = model[0].PatientId,
                            notAvialableTimeslotslist,
                            roomEquipmentnotaviable
                        };
                        return Json(listobjtoReturn, JsonRequestBehavior.AllowGet);
                    }
                }

                var toBeDeletedSchedulings = model.Where(item => !string.IsNullOrEmpty(item.RemovedAppointmentTypes));
                var deletedList = new List<int>();
                foreach (var i in toBeDeletedSchedulings)
                    deletedList.AddRange(i.RemovedAppointmentTypes.Split(',').Select(int.Parse).ToList());



                var result = _service.AddUpdatePatientScheduling(model, Convert.ToInt32(facilityId), deletedList);
                if (model.Count > 0 && result)
                {
                    var status = Convert.ToInt32(model[0].Status) + 1;
                    Helpers.SendAppointmentNotification(model, model[0].PatientEmailId,
                        Convert.ToString(model[0].EmailTemplateId), Convert.ToInt32(model[0].PatientId), Convert.ToInt32(model[0].PhysicianId), status);
                }
            }

            var listobj = new
            {
                IsError = false,
                patientid = model[0].PatientId,
                notAvialableTimeslotslist,
                roomEquipmentnotaviable
            };

            return Json(listobj, JsonRequestBehavior.AllowGet);
        }



        #region Methods
        private string GetSectionId(string sectionType, string roomId, string physicianId, string associatedId)
        {
            string id;
            switch (sectionType)
            {
                case "dep":
                case "room":
                    id = roomId;
                    break;
                default:
                    id = !string.IsNullOrEmpty(physicianId) ? physicianId : associatedId;
                    break;
            }
            return id;
        }

        private List<SchedulerCustomModelForCalender> GetSchedulingData(List<SchedularTypeCustomModel> filters, string sectionType)
        {
            var selectedPhysicianList = filters[0].PhysicianId;
            var selectedStatusList = filters[0].StatusType;
            var selectedDate = filters[0].SelectedDate;
            var selectedDepartmentList = filters[0].DeptData;
            var facilityid = Convert.ToInt32(filters[0].Facility);
            var viewtype = Convert.ToInt32(filters[0].ViewType);
            var patientId = filters[0].PatientId;
            var selectedRoomsList = filters[0].RoomIds;
            var phyObj = selectedPhysicianList != null && selectedPhysicianList.Count > 0
                ? ConvertArrayToCommaSeperatorString(selectedPhysicianList)
                : string.Empty;
            var dep = selectedDepartmentList != null && selectedDepartmentList.Count > 0
                ? ConvertArrayToCommaSeperatorString(selectedDepartmentList)
                : string.Empty;
            var roomsList = selectedRoomsList != null && selectedRoomsList.Count > 0
                ? ConvertArrayToCommaSeperatorString(selectedRoomsList)
                : string.Empty;
            var statuses = selectedStatusList != null && selectedStatusList.Count > 0
                ? ConvertArrayToCommaSeperatorString(selectedStatusList)
                : string.Empty;

            var list = new List<SchedulerCustomModelForCalender>();

            list = _service.GetSchedulerData(viewtype, selectedDate, phyObj, facilityid, dep, roomsList, statuses,
                sectionType, patientId);

            return list;
        }


        private List<SchedulerCustomModelForCalender> GetSchedulingData(List<SchedularTypeCustomModel> filters, string sectionType, out List<SchedulingCustomModel> nextList)
        {
            var selectedPhysicianList = filters[0].PhysicianId;
            var selectedStatusList = filters[0].StatusType;
            var selectedDate = filters[0].SelectedDate;
            var selectedDepartmentList = filters[0].DeptData;
            var facilityid = Convert.ToInt32(filters[0].Facility);
            var viewtype = Convert.ToInt32(filters[0].ViewType);
            var patientId = filters[0].PatientId;
            var selectedRoomsList = filters[0].RoomIds;
            var phyObj = selectedPhysicianList != null && selectedPhysicianList.Count > 0
                ? ConvertArrayToCommaSeperatorString(selectedPhysicianList)
                : string.Empty;
            var dep = selectedDepartmentList != null && selectedDepartmentList.Count > 0
                ? ConvertArrayToCommaSeperatorString(selectedDepartmentList)
                : string.Empty;
            var roomsList = selectedRoomsList != null && selectedRoomsList.Count > 0
                ? ConvertArrayToCommaSeperatorString(selectedRoomsList)
                : string.Empty;
            var statuses = selectedStatusList != null && selectedStatusList.Count > 0
                ? ConvertArrayToCommaSeperatorString(selectedStatusList)
                : string.Empty;

            var list = new List<SchedulerCustomModelForCalender>();
            list = _service.GetSchedulerData(viewtype, selectedDate, phyObj, facilityid, dep, roomsList, statuses,
                sectionType, patientId, out nextList);
            return list;
        }

        /// <summary>
        /// Gets the other procedures by event parent identifier.
        /// </summary>
        /// <param name="eventparentId">The eventparent identifier.</param>
        /// <returns></returns>
        private List<TypeOfProcedureCustomModel> GetOtherProceduresByEventParentId(string eventparentId, DateTime scheduleFrom)
        {
            var schdulingList = _service.GetOtherProceduresByEventParentId(eventparentId, scheduleFrom);
            return schdulingList;
        }




        /// <summary>
        /// Converts the array to comma seperator string.
        /// </summary>
        /// <param name="listToParse">The list to parse.</param>
        /// <returns></returns>
        private string ConvertArrayToCommaSeperatorString(IEnumerable<SchedularFiltersCustomModel> listToParse)
        {
            var variablestring = string.Join(",", listToParse.Select(x => x.Id));
            return variablestring;
        }

        private SchedulerCustomModelForCalender ConvertSchedulingObjToCustomModelForCalender(
            SchedulingCustomModel schedularObj)
        {
            var departmentOpeningObj =
                    new SchedulerCustomModelForCalender
                    {
                        id =
                            !string.IsNullOrEmpty(schedularObj.EventId)
                                ? Convert.ToInt64(schedularObj.EventId)
                                : 0,
                        text = schedularObj.Comments,
                        start_date = schedularObj.ScheduleFrom.Value.ToString("MM/dd/yyyy HH:mm"),
                        end_date = schedularObj.ScheduleTo.Value.ToString("MM/dd/yyyy HH:mm"),
                        Availability = schedularObj.Status,
                        _timed = Convert.ToBoolean(schedularObj.IsRecurring),
                        IsRecurrance = Convert.ToBoolean(schedularObj.IsRecurring),
                        rec_pattern = schedularObj.RecPattern,
                        rec_type = schedularObj.RecType,
                        rec_pattern_type = schedularObj.RecPattern,
                        rec_type_type = schedularObj.RecType,
                        TimeSlotId = schedularObj.SchedulingId,
                        event_length = Convert.ToInt64(schedularObj.RecEventlength),
                        event_pid =
                            Convert.ToBoolean(schedularObj.IsRecurring)
                                ? Convert.ToInt32(schedularObj.EventId)
                                : 0,
                        Department = schedularObj.ExtValue1,
                        PatientName = schedularObj.PatientName,
                        PatientEmailId = schedularObj.PatientEmailId,
                        EmirateIdnumber = schedularObj.PatientEmirateIdNumber,
                        PatientPhoneNumber = schedularObj.PatientPhoneNumber,
                        PatientDOB =
                            schedularObj.PatientDOB.HasValue
                                ? schedularObj.PatientDOB.Value.ToShortDateString()
                                : string.Empty,
                        PhysicianComments = schedularObj.Comments,
                        PhysicianSpeciality = schedularObj.PhysicianSpeciality,
                        physicianid = Convert.ToInt32(schedularObj.PhysicianId),
                        patientid = Convert.ToInt32(schedularObj.PatientId),
                        section_id =
                            string.IsNullOrEmpty(schedularObj.PhysicianId)
                                ? schedularObj.AssociatedId.ToString()
                                : schedularObj.PhysicianId,
                        SchedulingType = schedularObj.SchedulingType,
                        TypeOfProcedureCustomModel =
                            GetOtherProceduresByEventParentId(schedularObj.EventParentId, Convert.ToDateTime(schedularObj.ScheduleFrom)),
                        MultipleProcedure = Convert.ToBoolean(schedularObj.ExtValue3),
                        Rec_Start_date =
                         schedularObj.RecurringDateFrom != null
                             ? schedularObj.RecurringDateFrom.Value.ToShortDateString()
                             : string.Empty,
                        Rec_end_date = schedularObj.RecurringDateTill != null
                             ? schedularObj.RecurringDateTill.Value.ToShortDateString()
                             : string.Empty,
                        EventParentId = schedularObj.EventParentId,
                        location = schedularObj.FacilityId.ToString(),
                        TimeSlotTimeInterval = string.Empty,
                        VacationType = schedularObj.TypeOfProcedure,
                        PhysicianName = schedularObj.PhysicianName,
                        Rec_Start_date_type =
                          schedularObj.RecurringDateFrom != null
                              ? schedularObj.RecurringDateFrom.Value.ToShortDateString()
                              : string.Empty,
                        Rec_end_date_type =
                            schedularObj.RecurringDateTill != null
                                ? schedularObj.RecurringDateTill.Value.ToShortDateString()
                                : string.Empty,
                        AppointmentTypeStr = schedularObj.AppointmentTypeStr,
                        DepartmentName = schedularObj.DepartmentName,
                        PhysicianSpecialityStr = schedularObj.PhysicianSPL
                    };

            return departmentOpeningObj;
        }

        private List<SchedulerCustomModelForCalender> GetListSectionWise(IEnumerable<SchedulingCustomModel> facultyTimeslotslist, string sectionType)
        {
            var departmentOpeninglist = new List<SchedulerCustomModelForCalender>();

            departmentOpeninglist.AddRange(
                facultyTimeslotslist.Select(
                    item =>
                    new SchedulerCustomModelForCalender
                    {
                        id =
                            !string.IsNullOrEmpty(item.EventId)
                                ? Convert.ToInt64(item.EventId)
                                : 0,
                        text = item.Comments,
                        start_date = item.ScheduleFrom.Value.ToString("MM/dd/yyyy HH:mm"),
                        // Convert.ToBoolean(item.IsRecurring)
                        // ? (item.RecurringDateFrom.HasValue
                        // ? string.Format(
                        // "{0} {1}",
                        // item.ScheduleFrom.Value.ToString("MM/dd/yyyy"),
                        // item.RecurringDateFrom.Value.ToString("HH:mm"))
                        // : string.Empty) :
                        end_date = item.ScheduleTo.Value.ToString("MM/dd/yyyy HH:mm"),
                        // Convert.ToBoolean(item.IsRecurring)
                        // ? item.RecurringDateTill.Value.ToString(
                        // "MM/dd/yyyy HH:mm"):
                        Availability = item.Status,
                        _timed = Convert.ToBoolean(item.IsRecurring),
                        IsRecurrance = Convert.ToBoolean(item.IsRecurring),
                        rec_pattern = item.RecPattern,
                        rec_type = item.RecType,
                        rec_pattern_type = item.RecPattern,
                        rec_type_type = item.RecType,
                        TimeSlotId = item.SchedulingId,
                        event_length = Convert.ToInt64(item.RecEventlength),
                        event_pid =
                            Convert.ToBoolean(item.IsRecurring)
                                ? Convert.ToInt64(item.EventId)
                                : 0,
                        Department = item.ExtValue1,
                        PatientName = item.PatientName,
                        PatientEmailId = item.PatientEmailId,
                        EmirateIdnumber = item.PatientEmirateIdNumber,
                        PatientPhoneNumber = item.PatientPhoneNumber,
                        PatientDOB =
                            item.PatientDOB.HasValue
                                ? item.PatientDOB.Value.ToShortDateString()
                                : string.Empty,
                        PhysicianComments = item.ExtValue5,
                        PhysicianSpeciality = item.PhysicianSpeciality,
                        physicianid = Convert.ToInt32(item.PhysicianId),
                        patientid = Convert.ToInt32(item.PatientId),
                        section_id = GetSectionId(sectionType, Convert.ToString(item.RoomAssigned), item.PhysicianId, Convert.ToString(item.AssociatedId)),
                        //string.IsNullOrEmpty(item.PhysicianId)
                        //    ? item.AssociatedId.ToString()
                        //    : item.PhysicianId,
                        SchedulingType = item.SchedulingType,
                        TypeOfProcedureCustomModel = GetOtherProceduresByEventParentId(Convert.ToString(item.EventParentId), Convert.ToDateTime(item.ScheduleFrom)),
                        MultipleProcedure = Convert.ToBoolean(item.ExtValue3),
                        Rec_Start_date =
                         item.RecurringDateFrom != null
                             ? item.RecurringDateFrom.Value.ToShortDateString()
                             : string.Empty,
                        Rec_end_date = item.RecurringDateTill != null
                             ? item.RecurringDateTill.Value.ToShortDateString()
                             : string.Empty,
                        EventParentId = item.EventParentId,
                        location = item.FacilityId.ToString(),
                        TimeSlotTimeInterval = string.Empty,
                        VacationType = item.TypeOfProcedure,
                        PhysicianName = item.PhysicianName,
                        Rec_Start_date_type =
                          item.RecurringDateFrom != null
                              ? item.RecurringDateFrom.Value.ToShortDateString()
                              : string.Empty,
                        Rec_end_date_type =
                            item.RecurringDateTill != null
                                ? item.RecurringDateTill.Value.ToShortDateString()
                                : string.Empty,
                        AppointmentTypeStr = item.AppointmentTypeStr,
                        DepartmentName = item.DepartmentName,
                        PhysicianSpecialityStr = item.PhysicianSPL
                    }));

            return departmentOpeninglist;
        }

        #endregion
    }
}