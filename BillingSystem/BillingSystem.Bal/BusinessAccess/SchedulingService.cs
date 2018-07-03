using System;
using System.Collections.Generic;
using System.Linq;

using System.Data;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using AutoMapper;
using BillingSystem.Common.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;

using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class SchedulingService : ISchedulingService
    {
        private readonly IRepository<Scheduling> _repository;
        private readonly IRepository<FacilityStructure> _fsRepository;
        private readonly IRepository<Facility> _fRepository;
        private readonly IRepository<GlobalCodes> _gRepository;
        private readonly IRepository<Role> _roRepository;
        private readonly IRepository<Physician> _phRepository;
        private readonly IRepository<Corporate> _cRepository;
        private readonly IRepository<PatientLoginDetail> _pldRepository;
        private readonly IRepository<AppointmentTypes> _atRepository;
        private readonly IRepository<PatientInfo> _piRepository;
        private readonly IRepository<DeptTimming> _dptRepository;

        private readonly IMapper _mapper;
        private readonly BillingEntities _context;


        #region Constructors and Destructors

        public SchedulingService(IRepository<Scheduling> repository, IRepository<FacilityStructure> fsRepository, IRepository<Facility> fRepository, IRepository<GlobalCodes> gRepository, IRepository<Role> roRepository, IRepository<Physician> phRepository, IRepository<Corporate> cRepository, IRepository<PatientLoginDetail> pldRepository, IRepository<AppointmentTypes> atRepository, IRepository<PatientInfo> piRepository, IRepository<DeptTimming> dptRepository, IMapper mapper, BillingEntities context)
        {
            _repository = repository;
            _fsRepository = fsRepository;
            _fRepository = fRepository;
            _gRepository = gRepository;
            _roRepository = roRepository;
            _phRepository = phRepository;
            _cRepository = cRepository;
            _pldRepository = pldRepository;
            _atRepository = atRepository;
            _piRepository = piRepository;
            _dptRepository = dptRepository;
            _mapper = mapper;
            _context = context;
        }

        #endregion

        /// <summary>
        /// Saves the patient scheduling.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public SchedulingCustomModelView SavePatientScheduling(IEnumerable<SchedulingCustomModel> model)
        {
            var schedulingObjList = new List<Scheduling>();

            schedulingObjList = MapValuesVM(model);

            if (schedulingObjList.Any())
            {
                foreach (var item in schedulingObjList)
                {
                    if (item.SchedulingId > 0)
                    {
                        DeleteHolidayPlannerData(item.IsRecurring == true ? item.EventParentId : string.Empty,
                            item.SchedulingId, Convert.ToInt32(item.SchedulingType), "");
                    }

                    _repository.Create(item);

                    if (item.IsRecurring == true)
                        CreateRecurringSchedularEvents(item.SchedulingId);
                }
            }

            var list = new SchedulingCustomModelView();

            return list;
        }
        public void CreateRecurringSchedularEvents(
         int pSchedulingId)
        {
            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("pSchedulingId", pSchedulingId);
            _repository.ExecuteCommand(StoredProcedures.SPROC_CreateRecurringEventsSchedular.ToString(), sqlParameters);
        }

        private List<SchedulingCustomModel> MapValues(IEnumerable<Scheduling> m)
        {
            var lst = new List<SchedulingCustomModel>();
            foreach (var model in m)
            {
                var vm = _mapper.Map<SchedulingCustomModel>(model);
                var physicianObj = !string.IsNullOrEmpty(model.PhysicianId) ?
                    GetPhysicianCModelById(Convert.ToInt32(model.PhysicianId)) :
                    null;
                var patientObj = GetPatientCustomModelByPatientId(Convert.ToInt32(model.AssociatedId));
                if (physicianObj != null)
                {
                    vm.PhysicianSPL = physicianObj.UserSpecialityStr;
                    //vm.DepartmentName = physicianObj.UserDepartmentStr;
                    vm.PhysicianName = physicianObj.Physician.PhysicianName;
                    vm.PatientId = patientObj != null && patientObj.PatientInfo != null ? patientObj.PatientInfo.PatientID : 0;
                    vm.PatientName = patientObj != null && patientObj.PatientInfo != null ? patientObj.PatientInfo.PersonFirstName + " " + patientObj.PatientInfo.PersonLastName : string.Empty;
                    vm.PatientEmailId = GetPatientEmail(patientObj.PatientInfo.PatientID);
                    vm.PatientEmirateIdNumber = patientObj != null && patientObj.PatientInfo != null ? patientObj.PatientInfo.PersonEmiratesIDNumber : string.Empty;
                    vm.PatientDOB = patientObj != null && patientObj.PatientInfo != null ? patientObj.PatientInfo.PersonBirthDate : DateTime.Now;
                    vm.MultipleProcedures = Convert.ToBoolean(vm.ExtValue3);
                    var firstOrDefault = patientObj != null && patientObj.PatientInfo != null
                        ? patientObj.PatientInfo.PatientPhone.FirstOrDefault(x => x.IsPrimary == true)
                        : null;

                    if (firstOrDefault != null)
                        vm.PatientPhoneNumber = patientObj.PatientInfo != null ? firstOrDefault.PhoneNo : string.Empty;

                    vm.PhysicianId = model.PhysicianId;
                    vm.DepartmentName = string.IsNullOrEmpty(model.ExtValue1)
                                            ? string.Empty
                                            : GetDepartmentNameById(Convert.ToInt32(model.ExtValue1));
                    vm.AppointmentTypeStr = string.IsNullOrEmpty(model.TypeOfProcedure)
                                                ? string.Empty
                                                : GetAppointmentTypeById(
                                                    Convert.ToInt32(model.TypeOfProcedure));
                    vm.FacilityName = GetFacilityNameByFacilityId(Convert.ToInt32(model.FacilityId));
                    vm.CorporateName = GetCorporateNameFromId(Convert.ToInt32(model.CorporateId));
                }
                lst.Add(vm);
            }
            return lst;
        }
        private SchedulingCustomModel MapValuesObj(Scheduling model)
        {
            var vm = _mapper.Map<SchedulingCustomModel>(model);
            var physicianObj = !string.IsNullOrEmpty(model.PhysicianId) ?
                GetPhysicianCModelById(Convert.ToInt32(model.PhysicianId)) :
                null;
            var patientObj = GetPatientCustomModelByPatientId(Convert.ToInt32(model.AssociatedId));
            if (physicianObj != null)
            {
                vm.PhysicianSPL = physicianObj.UserSpecialityStr;
                //vm.DepartmentName = physicianObj.UserDepartmentStr;
                vm.PhysicianName = physicianObj.Physician.PhysicianName;
                vm.PatientId = patientObj != null && patientObj.PatientInfo != null ? patientObj.PatientInfo.PatientID : 0;
                vm.PatientName = patientObj != null && patientObj.PatientInfo != null ? patientObj.PatientInfo.PersonFirstName + " " + patientObj.PatientInfo.PersonLastName : string.Empty;
                vm.PatientEmailId = GetPatientEmail(patientObj.PatientInfo.PatientID);
                vm.PatientEmirateIdNumber = patientObj != null && patientObj.PatientInfo != null ? patientObj.PatientInfo.PersonEmiratesIDNumber : string.Empty;
                vm.PatientDOB = patientObj != null && patientObj.PatientInfo != null ? patientObj.PatientInfo.PersonBirthDate : DateTime.Now;
                vm.MultipleProcedures = Convert.ToBoolean(vm.ExtValue3);
                var firstOrDefault = patientObj != null && patientObj.PatientInfo != null
                    ? patientObj.PatientInfo.PatientPhone.FirstOrDefault(x => x.IsPrimary == true)
                    : null;

                if (firstOrDefault != null)
                    vm.PatientPhoneNumber = patientObj.PatientInfo != null ? firstOrDefault.PhoneNo : string.Empty;

                vm.PhysicianId = model.PhysicianId;
                vm.DepartmentName = string.IsNullOrEmpty(model.ExtValue1)
                                        ? string.Empty
                                        : GetDepartmentNameById(Convert.ToInt32(model.ExtValue1));
                vm.AppointmentTypeStr = string.IsNullOrEmpty(model.TypeOfProcedure)
                                            ? string.Empty
                                            : GetAppointmentTypeById(
                                                Convert.ToInt32(model.TypeOfProcedure));
                vm.FacilityName = GetFacilityNameByFacilityId(Convert.ToInt32(model.FacilityId));
                vm.CorporateName = GetCorporateNameFromId(Convert.ToInt32(model.CorporateId));
            }
            return vm;
        }
        private List<Scheduling> MapValuesVM(IEnumerable<SchedulingCustomModel> m)
        {
            return m.Select(x => _mapper.Map<Scheduling>(x)).ToList();
            //var lst = new List<Scheduling>();
            //foreach (var model in m)
            //{
            //    var vm = _mapper.Map<Scheduling>(model);
            //    //var physicianObj = !string.IsNullOrEmpty(model.PhysicianId) ?
            //    //    GetPhysicianCModelById(Convert.ToInt32(model.PhysicianId)) :
            //    //    null;
            //    //var patientObj = GetPatientCustomModelByPatientId(Convert.ToInt32(model.AssociatedId));
            //    //if (physicianObj != null)
            //    //{
            //    //    vm.PhysicianSPL = physicianObj.UserSpecialityStr;
            //    //    //vm.DepartmentName = physicianObj.UserDepartmentStr;
            //    //    vm.PhysicianName = physicianObj.Physician.PhysicianName;
            //    //    vm.PatientId = patientObj != null && patientObj.PatientInfo != null ? patientObj.PatientInfo.PatientID : 0;
            //    //    vm.PatientName = patientObj != null && patientObj.PatientInfo != null ? patientObj.PatientInfo.PersonFirstName + " " + patientObj.PatientInfo.PersonLastName : string.Empty;
            //    //    vm.PatientEmailId = GetPatientEmail(patientObj.PatientInfo.PatientID);
            //    //    vm.PatientEmirateIdNumber = patientObj != null && patientObj.PatientInfo != null ? patientObj.PatientInfo.PersonEmiratesIDNumber : string.Empty;
            //    //    vm.PatientDOB = patientObj != null && patientObj.PatientInfo != null ? patientObj.PatientInfo.PersonBirthDate : DateTime.Now;
            //    //    vm.MultipleProcedures = Convert.ToBoolean(vm.ExtValue3);
            //    //    var firstOrDefault = patientObj != null && patientObj.PatientInfo != null
            //    //        ? patientObj.PatientInfo.PatientPhone.FirstOrDefault(x => x.IsPrimary == true)
            //    //        : null;

            //    //    if (firstOrDefault != null)
            //    //        vm.PatientPhoneNumber = patientObj.PatientInfo != null ? firstOrDefault.PhoneNo : string.Empty;

            //    //    vm.PhysicianId = model.PhysicianId;
            //    //    vm.DepartmentName = string.IsNullOrEmpty(model.ExtValue1)
            //    //                            ? string.Empty
            //    //                            : GetDepartmentNameById(Convert.ToInt32(model.ExtValue1));
            //    //    vm.AppointmentTypeStr = string.IsNullOrEmpty(model.TypeOfProcedure)
            //    //                                ? string.Empty
            //    //                                : GetAppointmentTypeById(
            //    //                                    Convert.ToInt32(model.TypeOfProcedure));
            //    //    vm.FacilityName = GetFacilityNameByFacilityId(Convert.ToInt32(model.FacilityId));
            //    //    vm.CorporateName = GetCorporateNameFromId(Convert.ToInt32(model.CorporateId));
            //    //}
            //    lst.Add(vm);
            //}
            //return lst;
        }
        private NotAvialableTimeSlots MapValues(Scheduling model)
        {
            var vm = _mapper.Map<NotAvialableTimeSlots>(model);
            if (model.SchedulingType == "2")
            {
                vm.DateFrom = model.ScheduleFrom;
                vm.DateTo = model.ScheduleTo;
                vm.Reason = !string.IsNullOrEmpty(model.ExtValue2)
                                ? "Some of the Physician under the facility have open appointments in their schedule."
                                : model.SchedulingType == "2"
                                      ? "Physician have open appointments/vacation in the selected Date: " + model.ScheduleFrom.Value.ToShortDateString()
                                      : "Time slot is not available";
            }
            else
            {
                var physicianObj = !string.IsNullOrEmpty(model.PhysicianId)
                                       ? GetPhysicianCModelById(Convert.ToInt32(model.PhysicianId))
                                       : null;
                if (physicianObj != null)
                {
                    vm.PhysicianSpl = physicianObj.UserSpecialityStr;
                    vm.PhysicianDepartment = physicianObj.UserDepartmentStr;
                    vm.PhysicianName = physicianObj.Physician.PhysicianName;
                    vm.DateFrom = model.ScheduleFrom;
                    vm.DateTo = model.ScheduleTo;
                    vm.Reason = !string.IsNullOrEmpty(model.ExtValue2)
                                    ? "Some of the Physician under the facility have open appointments/vacation in their schedule."
                                    : model.SchedulingType == "2"
                                          ? "Physician have open appointments/vacation in the selected Date/Date range."
                                          : "Time slot is not available";
                    vm.DateFromSTR = model.ScheduleFrom.Value.ToShortDateString();
                    vm.DateToSTR = model.ScheduleTo.Value.ToShortDateString();
                    vm.TimeFromStr = model.ScheduleFrom.Value.ToString("HH:mm");
                    vm.TimeTOStr = model.ScheduleTo.Value.ToString("HH:mm");
                }
            }

            return vm;
        }
        private FacultyTimeslotsCustomModel MapValues(FacultyTimeslots model)
        {
            var vm = _mapper.Map<FacultyTimeslotsCustomModel>(model);
            var facultyObj = GetFacultyByUserId(model.UserID);
            vm.FacultyLunchTimeFrom = model.SlotAvailability == "3"
                                          ? model.AvailableDateFrom.Value.ToShortDateString() + " " + model.AvailableDateFrom.Value.ToString("HH:mm")
                                           : string.Empty;
            vm.FacultyLunchTimeTill = model.SlotAvailability == "3"
                                          ? model.AvailableDateTill.Value.ToShortDateString() + " " + model.AvailableDateTill.Value.ToString("HH:mm")
                                          : string.Empty;


            vm.LunchStartTime = Convert.ToDateTime(model.AvailableDateFrom).Minute > 0
                ? Convert.ToString(Convert.ToDateTime(model.AvailableDateFrom).Hour) + ".5"
                : Convert.ToString(Convert.ToDateTime(model.AvailableDateFrom).Hour);
            vm.LunchEndTime = Convert.ToDateTime(model.AvailableDateTill).Minute > 0
                ? Convert.ToString(Convert.ToDateTime(model.AvailableDateTill).Hour) + ".5"
                : Convert.ToString(Convert.ToDateTime(model.AvailableDateTill).Hour);

            return vm;
        }
        private Physician GetFacultyByUserId(int userId)
        {
            var physcianObj = _phRepository.Where(x => x.UserId == userId).FirstOrDefault();
            return physcianObj;
        }
        private string GetAppointmentTypeById(int id)
        {
            var model = _atRepository.Where(a => a.Id == id).FirstOrDefault();
            return model != null ? model.Name : string.Empty;
        }
        private PatientInfoCustomModel GetPatientCustomModelByPatientId(int patientId)
        {
            var pInfoCModel = new PatientInfoCustomModel();

            //Get the patient details from the table PatientInfo
            var pInfo = _piRepository.Where(p => p.PatientID == patientId).Include(p => p.PatientAddressRelation).Include(p => p.PatientPhone).FirstOrDefault();
            pInfoCModel.PatientInfo = pInfo;
            pInfoCModel.PatientName = pInfo != null ? string.Format("{0} {1}", pInfo.PersonFirstName, pInfo.PersonLastName) : string.Empty;
            pInfoCModel.PersonAge = GetAgeByDate(Convert.ToDateTime(pInfo != null ? pInfo.PersonBirthDate : DateTime.Now));
            return pInfoCModel;
        }

        private int GetAgeByDate(DateTime dateValue)
        {
            var age = DateTime.Now - dateValue;
            return (int)(age.Days / 365.25);
        }
        private string GetPatientEmail(int patientId)
        {
            var m = _pldRepository.Where(x => x.PatientId == patientId && (x.IsDeleted == null || x.IsDeleted == false)).FirstOrDefault();
            return m != null ? m.Email : string.Empty;
        }
        private string GetCorporateNameFromId(int corpId)
        {
            var corpName = "";
            var obj = _cRepository.Where(f => f.CorporateID == corpId).FirstOrDefault();
            if (obj != null) corpName = obj.CorporateName;
            return corpName;
        }
        private PhysicianCustomModel GetPhysicianCModelById(int physicianId)
        {
            var m = _phRepository.Get(physicianId);
            var vm = MapValues(m);
            return vm;
        }
        private List<PhysicianCustomModel> MapValues(IEnumerable<Physician> m)
        {
            var lst = new List<PhysicianCustomModel>();
            foreach (var model in m)
            {
                var vm = _mapper.Map<PhysicianCustomModel>(model);
                if (vm != null)
                {
                    vm.Physician = model;
                    vm.PrimaryFacilityName =
                        GetFacilityNameByFacilityId(Convert.ToInt32(model.PhysicianPrimaryFacility));
                    vm.SecondaryFacilityName =
                        GetFacilityNameByFacilityId(Convert.ToInt32(model.PhysicianSecondaryFacility));
                    vm.ThirdFacilityName = GetFacilityNameByFacilityId(
                        Convert.ToInt32(model.PhysicianThirdFacility));
                    vm.PhysicanLicenseTypeName =
                        GetNameByLicenseTypeIdAndUserTypeId(
                            Convert.ToString(model.PhysicianLicenseType),
                            Convert.ToString(model.UserType));

                    vm.UserTypeStr = GetRoleNameById(Convert.ToInt32(model.UserType));

                    int depId = Convert.ToInt32(model.FacultyDepartment);
                    vm.UserDepartmentStr = depId > 0
                                               ? GetDepartmentNameById(Convert.ToInt32(model.FacultyDepartment))
                                               : string.Empty;

                    vm.UserSpecialityStr = GetNameByGlobalCodeValue(
                        Convert.ToString(model.FacultySpeciality),
                        Convert.ToString((int)GlobalCodeCategoryValue.PhysicianSpecialties));
                }

                lst.Add(vm);
            }
            return lst;
        }
        private PhysicianCustomModel MapValues(Physician model)
        {
            var vm = _mapper.Map<PhysicianCustomModel>(model);
            if (vm != null)
            {
                vm.Physician = model;
                vm.PrimaryFacilityName =
                    GetFacilityNameByFacilityId(Convert.ToInt32(model.PhysicianPrimaryFacility));
                vm.SecondaryFacilityName =
                    GetFacilityNameByFacilityId(Convert.ToInt32(model.PhysicianSecondaryFacility));
                vm.ThirdFacilityName = GetFacilityNameByFacilityId(
                    Convert.ToInt32(model.PhysicianThirdFacility));
                vm.PhysicanLicenseTypeName =
                    GetNameByLicenseTypeIdAndUserTypeId(
                        Convert.ToString(model.PhysicianLicenseType),
                        Convert.ToString(model.UserType));

                vm.UserTypeStr = GetRoleNameById(Convert.ToInt32(model.UserType));

                int depId = Convert.ToInt32(model.FacultyDepartment);
                vm.UserDepartmentStr = depId > 0
                                           ? GetDepartmentNameById(Convert.ToInt32(model.FacultyDepartment))
                                           : string.Empty;

                vm.UserSpecialityStr = GetNameByGlobalCodeValue(
                    Convert.ToString(model.FacultySpeciality),
                    Convert.ToString((int)GlobalCodeCategoryValue.PhysicianSpecialties));
            }

            return vm;
        }
        private string GetDepartmentNameById(int facilityDeaprtmentId)
        {
            var facilityDepartment = _fsRepository.Where(x => x.FacilityStructureId == facilityDeaprtmentId).FirstOrDefault();
            return facilityDepartment == null ? string.Empty : facilityDepartment.FacilityStructureName;
        }
        public string GetNameByLicenseTypeIdAndUserTypeId(string licenceTypeId, string userTypeId)
        {
            string licenseName;
            switch (userTypeId)
            {
                case "4":
                    licenseName = GetNameByGlobalCodeValue(
                        licenceTypeId,
                        Convert.ToString((int)GlobalCodeCategoryValue.PhysicianLicenseType));
                    break;
                case "5":
                    licenseName = GetNameByGlobalCodeValue(
                        licenceTypeId,
                        Convert.ToString((int)GlobalCodeCategoryValue.NurseLicenseType));
                    break;
                case "6":
                    licenseName = GetNameByGlobalCodeValue(
                        licenceTypeId,
                        Convert.ToString((int)GlobalCodeCategoryValue.CoderLicenseType));
                    break;
                case "7":
                    licenseName = GetNameByGlobalCodeValue(
                        licenceTypeId,
                        Convert.ToString((int)GlobalCodeCategoryValue.ClerkLicenseType));
                    break;
                default:
                    licenseName = "NA";
                    break;
            }

            return licenseName;
        }
        private string GetRoleNameById(int roleID)
        {
            var role = _roRepository.Where(r => r.RoleID == roleID).FirstOrDefault();
            return role != null ? role.RoleName : string.Empty;
        }
        private string GetNameByGlobalCodeValue(string codeValue, string categoryValue, string fId = "")
        {
            if (!string.IsNullOrEmpty(codeValue))
            {
                var gl = _gRepository.Where(g => g.GlobalCodeValue.Equals(codeValue) && !g.IsDeleted.Value && g.GlobalCodeCategoryValue.Equals(categoryValue) && (string.IsNullOrEmpty(fId) || g.FacilityNumber.Equals(fId))).FirstOrDefault();
                return gl != null ? gl.GlobalCodeName : string.Empty;
            }
            return string.Empty;
        }
        private string GetFacilityNameByFacilityId(int facilityId)
        {
            var m = _fRepository.GetSingle(facilityId);
            return m != null ? m.FacilityName : string.Empty;
        }

        /// <summary>
        /// Saves the holiday scheduling.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public List<SkippedHolidaysData> SaveHolidayScheduling(List<SchedulingCustomModel> model)
        {
            var listtoReturn = new List<SkippedHolidaysData>();

            var schedulingObjList = new List<Scheduling>();
            var isFacilityHoliday = false;
            var schedulingIds = new List<int>();
            schedulingObjList = MapValuesVM(model);
            if (schedulingObjList.Any())
            {
                foreach (var item in schedulingObjList)
                {
                    _repository.Create(item);
                    if (!string.IsNullOrEmpty(item.ExtValue2))
                    {
                        isFacilityHoliday = true;
                        schedulingIds.Add(item.SchedulingId);
                    }
                }
            }

            var associatedId = Convert.ToString(model[0].AssociatedId);

            if (isFacilityHoliday)
            {
                var schIds = (string.Join(",", schedulingIds));
                var spName =
                        "EXEC SPROC_CreateFacilityHolidays_V1 @pSchedulingIds,@pMulitpleProcedure,@pAssociatedId ";
                var sqlParameters = new SqlParameter[3];
                sqlParameters[0] = new SqlParameter("pSchedulingIds", schIds);
                sqlParameters[1] = new SqlParameter("pMulitpleProcedure", "");
                sqlParameters[2] = new SqlParameter("pAssociatedId", Convert.ToInt32(schedulingIds));
                var result = _context.Database.SqlQuery<int>(
                    spName, sqlParameters);
            }
            return listtoReturn;
        }

        /// <summary>
        /// Gets the type of the scheduling data by.
        /// </summary>
        /// <param name="selectedDate">The selected date.</param>
        /// <param name="type">The type.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<SchedulingCustomModel> GetSchedulingDataByType(List<string> phyList, DateTime selectedDate, string type, int facilityId)
        {
            var mlst = new List<Scheduling>();
            var vmlst = new List<SchedulingCustomModel>();
            try
            {
                mlst = phyList.Count > 0
                    ? _repository.Where(
                        x =>
                            (phyList.Contains(x.PhysicianId)
                             || (x.AssociatedType == "2" && phyList.Contains(x.AssociatedId.ToString())))).ToList()
                    : _repository.Where(
                        x => (x.FacilityId == facilityId)).ToList();

                mlst = mlst.Where(
                    x =>
                    x.ScheduleTo != null && (x.ScheduleFrom != null && (x.ScheduleFrom.Value.ToShortDateString() == selectedDate.ToShortDateString()
                                                                        || x.ScheduleTo.Value.ToShortDateString() == selectedDate.ToShortDateString()))).ToList();
                vmlst = MapValues(mlst);
                return vmlst;
            }
            catch (Exception)
            {
                return vmlst;
            }
        }

        /// <summary>
        /// Gets the type of the scheduling dept data by.
        /// </summary>
        /// <param name="deptId">The dept identifier.</param>
        /// <param name="selectedDate">The selected date.</param>
        /// <param name="type">The type.</param>
        /// <param name="facilityIdstr">The facility idstr.</param>
        /// <returns></returns>
        public List<SchedulingCustomModel> GetSchedulingDeptDataByType(int deptId, DateTime selectedDate, string type, string facilityIdstr)
        {
            var vmlist = new List<SchedulingCustomModel>();
            try
            {
                var deptIdStr = deptId.ToString();
                var facilityId = Convert.ToInt32(facilityIdstr);
                var schedulingObj = deptIdStr != "0"
                                              ? _repository.Where(
                                                  x => x.ExtValue1 == deptIdStr
                                                   && x.SchedulingType == type).ToList()
                                              : _repository.Where(
                                                  x => (x.FacilityId == facilityId && x.SchedulingType == type))
                                                    .ToList();

                schedulingObj = schedulingObj.Where(
                    x =>
                    x.ScheduleTo != null && (x.ScheduleFrom != null && (x.ScheduleFrom.Value.ToShortDateString() == selectedDate.ToShortDateString()
                                                                        || x.ScheduleTo.Value.ToShortDateString() == selectedDate.ToShortDateString()))).ToList();
                vmlist = MapValues(schedulingObj);
                return vmlist;
            }
            catch (Exception)
            {
                return vmlist;
            }
        }

        /// <summary>
        /// Gets the other procedures by event parent identifier.
        /// </summary>
        /// <param name="eventparentId">The eventparent identifier.</param>
        /// <param name="scheduleFrom">The schedule from.</param>
        /// <returns></returns>
        public List<TypeOfProcedureCustomModel> GetOtherProceduresByEventParentId(string eventparentId, DateTime scheduleFrom)
        {
            var listtoReturn = new List<TypeOfProcedureCustomModel>();
            try
            {
                var valueToGet = _repository.Where(x => x.EventParentId == eventparentId).ToList();
                valueToGet =
                    valueToGet.Where(
                        x => x.ScheduleFrom.Value.ToShortDateString() == scheduleFrom.ToShortDateString()).ToList();
                listtoReturn.AddRange(
                    valueToGet.Select(item => new TypeOfProcedureCustomModel
                    {
                        DateSelected = item.ScheduleFrom.Value.ToShortDateString(),
                        TimeFrom = Convert.ToBoolean(item.IsRecurring) ? item.RecurringDateFrom.Value.ToString("HH:mm") : item.ScheduleFrom.Value.ToString("HH:mm"),
                        TimeTo = Convert.ToBoolean(item.IsRecurring) ? GetTimeToRecurrance(item) : item.ScheduleTo.Value.ToString("HH:mm"),
                        TypeOfProcedureId = item.TypeOfProcedure,
                        EventParentId = item.EventParentId,
                        MainId = item.SchedulingId,
                        TypeOfProcedureName = GetAppointmentNameById(Convert.ToInt32(item.TypeOfProcedure), Convert.ToInt32(item.CorporateId), Convert.ToInt32(item.FacilityId)),
                        TimeSlotTimeInterval = GetAppointmentTypeTimeIntervalById(Convert.ToInt32(item.TypeOfProcedure), Convert.ToInt32(item.CorporateId), Convert.ToInt32(item.FacilityId)),
                        ProcedureAvailablityStatus = item.Status,
                        IsRecurrance = Convert.ToBoolean(item.IsRecurring),
                        Rec_Pattern = item.RecPattern,
                        Rec_Type = item.RecType,
                        end_By = Convert.ToBoolean(item.IsRecurring) ? item.RecurringDateTill.Value.ToShortDateString() : item.ScheduleTo.Value.ToShortDateString(),//item.ScheduleTo.Value.ToShortDateString(),
                        Rec_Start_date = Convert.ToBoolean(item.IsRecurring) ? item.RecurringDateFrom.Value.ToShortDateString() : item.ScheduleFrom.Value.ToShortDateString(),
                        Rec_end_date = Convert.ToBoolean(item.IsRecurring) ? item.RecurringDateTill.Value.ToShortDateString() : item.ScheduleTo.Value.ToShortDateString(),
                        DeptOpeningDays = GetDeptOpeningDaysForPhysician(Convert.ToInt32(item.PhysicianId))
                    }));


                return listtoReturn;
            }
            catch (Exception)
            {
                return listtoReturn;
            }
        }
        private string GetAppointmentNameById(int id, int corporateId, int facilityId)
        {
            var model = _atRepository.Where(x => x.Id == id && x.CorporateId == corporateId && x.FacilityId == facilityId).FirstOrDefault();
            return model != null ? model.Name : string.Empty;
        }
        private string GetAppointmentTypeTimeIntervalById(int id, int corporateId, int facilityId)
        {
            var model = _atRepository.Where(x => x.Id == id && x.CorporateId == corporateId && x.FacilityId == facilityId).FirstOrDefault();

            return model != null ? model.DefaultTime : string.Empty;
        }
        private string GetTimeToRecurrance(Scheduling itemModel)
        {
            var timefrom = itemModel.RecurringDateFrom;
            var ticksforTimeFrom = Convert.ToDateTime(timefrom);
            var timeto = ticksforTimeFrom.AddTicks(Convert.ToInt64(itemModel.RecEventlength * 10000000));
            return timeto.ToString("HH:mm");
        }

        /// <summary>
        /// Gets the holiday planner data.
        /// </summary>
        /// <param name="physicianId">The physician identifier.</param>
        /// <param name="selectedDate">The selected date.</param>
        /// <param name="type">The type.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<SchedulingCustomModel> GetHolidayPlannerData(int physicianId, DateTime selectedDate, string type, int facilityId)
        {
            var vmlst = new List<SchedulingCustomModel>();
            var pId = physicianId.ToString();

            var lstHoliday = pId != "0"
                ? _repository.Where(x => (x.AssociatedId == physicianId) && (x.AssociatedType == "2") && (x.FacilityId == facilityId && x.SchedulingType == "2")).ToList()
                : new List<Scheduling>();

            switch (type)
            {
                // 1 =daily  , 2= weekly , 3=Monthly
                case "1":
                    lstHoliday = lstHoliday.Where(x => x.ScheduleTo != null && (x.ScheduleFrom != null && (x.ScheduleFrom.Value.Date == selectedDate.Date || x.ScheduleTo.Value.Date == selectedDate.Date))).ToList();

                    break;
                case "2":
                    var weeknumber = GetWeekOfYearISO8601Bal(selectedDate).ToString();
                    lstHoliday =
                        lstHoliday.Where(x => x.WeekDay == weeknumber).ToList();
                    break;
                case "3":
                    var monthStartDate = new DateTime(selectedDate.Year, selectedDate.Month, 1);
                    var monthEndDate = monthStartDate.AddMonths(1).AddDays(-1);
                    lstHoliday =
                        lstHoliday.Where(
                            x =>
                            x.ScheduleTo != null
                            && (x.ScheduleFrom != null
                                && (x.ScheduleFrom.Value.Date >= monthStartDate.Date && x.ScheduleTo.Value.Date <= monthEndDate.Date))).ToList();
                    break;
                default:
                    lstHoliday =
                        lstHoliday.Where(
                            x =>
                            x.ScheduleTo != null
                            && (x.ScheduleFrom != null
                                && (x.ScheduleFrom.Value.ToShortDateString() == selectedDate.ToShortDateString()
                                    || x.ScheduleTo.Value.ToShortDateString()
                                    == selectedDate.ToShortDateString()))).ToList();
                    break;
            }

            vmlst = MapValues(lstHoliday);
            return vmlst;
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
        /// Deletes the holiday planner data.
        /// </summary>
        /// <param name="eventParentid">The event parentid.</param>
        /// <param name="schedulingid">The schedulingid.</param>
        /// <param name="schedulingType">The schedulingType.</param>
        /// <param name="extValue3">The external value3</param>
        /// <returns></returns>
        public bool DeleteHolidayPlannerData(string eventParentid, int schedulingid, int schedulingType, string extValue3)
        {
            try
            {
                var lst = !string.IsNullOrEmpty(eventParentid) ?
                    _repository.Where(x => x.EventParentId == eventParentid).ToList() :
                    _repository.Where(x => x.SchedulingId == schedulingid).ToList();


                if (lst.Count > 0)
                {
                    _repository.Delete(lst);
                    return true;
                }

                return false;

            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks for duplicate record.
        /// </summary>
        /// <param name="schedulingId">The scheduling identifier.</param>
        /// <param name="schedulingType">Type of the scheduling.</param>
        /// <param name="starTime">The star time.</param>
        /// <param name="endTime">The end time.</param>
        /// <param name="userid">The userid.</param>
        /// <param name="physicianid">The physicianid.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <returns></returns>
        public bool CheckForDuplicateRecord(int schedulingId, string schedulingType, DateTime starTime, DateTime endTime, int userid, int physicianid, int facilityid)
        {
            var spName =
                       string.Format(
                           "EXEC {0} @pSchedulingId, @pSchedulingType, @pSelectedDate, @TimeFrom, @TimeTo, @pUserid, @pPhysicianid, @pFacilityid",
                           StoredProcedures.SPROC_GetTimeSlotAvailablity);
            var sqlParameters = new SqlParameter[8];
            sqlParameters[0] = new SqlParameter("pSchedulingId", schedulingId);
            sqlParameters[1] = new SqlParameter("pSchedulingType", schedulingType);
            sqlParameters[2] = new SqlParameter("pSelectedDate", starTime);
            sqlParameters[3] = new SqlParameter("TimeFrom", starTime.ToString("HH:mm"));
            sqlParameters[4] = new SqlParameter("TimeTo", endTime.ToString("HH:mm"));
            sqlParameters[5] = new SqlParameter("pUserid", userid);
            sqlParameters[6] = new SqlParameter("pPhysicianid", physicianid);
            sqlParameters[7] = new SqlParameter("pFacilityid", facilityid);
            IEnumerable<TimeSlotAvailabilityCustomModel> result = _context.Database.SqlQuery<TimeSlotAvailabilityCustomModel>(
                spName, sqlParameters);
            return result.ToList().Count > 0 && result.FirstOrDefault().TimeSlotAvailable > 0;
        }

        /// <summary>
        /// Gets the not avialable time slots custom model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public NotAvialableTimeSlots GetNotAvialableTimeSlotsCustomModel(SchedulingCustomModel model)
        {
            var m = MapValues(model);
            return m;
        }

        /// <summary>
        /// Gets the scheduling custom model by identifier.
        /// </summary>
        /// <param name="schedulingid">The schedulingid.</param>
        /// <returns></returns>
        public SchedulingCustomModel GetSchedulingCustomModelById(int schedulingid)
        {
            var m = _repository.Where(x => x.SchedulingId == schedulingid).FirstOrDefault();
            var vm = MapValuesObj(m);
            return vm;
        }

        public List<SchedulingCustomModel> GetSchedulingListByPatient(int patientId, string physicianId, string vToken)
        {
            var list = new List<SchedulingCustomModel>();
            var schedulingObj = _repository.Where(x => x.AssociatedId == patientId && x.PhysicianId.Trim().Equals(physicianId) && x.ExtValue4.Trim().Equals(vToken)).OrderByDescending(s => s.ScheduleFrom).ToList();
            list = MapValues(schedulingObj);
            return list;
        }


        public bool UpdateSchedulingEvents(List<SchedulingCustomModel> list)
        {
            var schedulingObjList = new List<Scheduling>();
            schedulingObjList = MapValuesVM(list);

            foreach (var item in schedulingObjList)
            {
                if (item.SchedulingId > 0)
                    _repository.UpdateEntity(item, item.SchedulingId);
            }
            return true;
        }

        /// <summary>
        /// Gets the assigned room for procedure.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="appointmentType">Type of the appointment.</param>
        /// <param name="scheduledDate">The scheduled date.</param>
        /// <param name="timeFrom">The time from.</param>
        /// <param name="timeTo">The time to.</param>
        /// <param name="roomId">The room Id.</param>
        /// <returns></returns>
        public RoomEquipmentAvialability GetAssignedRoomForProcedure(int facilityId, int appointmentType, DateTime scheduledDate, string timeFrom, string timeTo, Int32 roomId)
        {
            var spName =
                       string.Format(
                           "EXEC {0} @FacilityId, @AppointmentType, @ScheduledDate, @TimeFrom, @TimeTo, @AvailableRoom",
                           StoredProcedures.SPROC_CheckRoomsAndEquipmentsForScheduling_New);
            var sqlParameters = new SqlParameter[6];
            sqlParameters[0] = new SqlParameter("FacilityId", facilityId);
            sqlParameters[1] = new SqlParameter("AppointmentType", appointmentType);
            sqlParameters[2] = new SqlParameter("ScheduledDate", scheduledDate);
            sqlParameters[3] = new SqlParameter("TimeFrom", timeFrom);
            sqlParameters[4] = new SqlParameter("TimeTo", timeTo);
            sqlParameters[5] = new SqlParameter("AvailableRoom", roomId);
            IEnumerable<RoomEquipmentAvialability> result = _context.Database.SqlQuery<RoomEquipmentAvialability>(
                spName,
                sqlParameters);
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Gets the available time slots.
        /// </summary>
        /// <param name="facilityid">The facilityid.</param>
        /// <param name="physicianId">The physician identifier.</param>
        /// <param name="dateselected">The dateselected.</param>
        /// <param name="typeofproc">The typeofproc.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IEnumerable<AvailabilityTimeSlotForPopupCustomModel> GetAvailableTimeSlots(int facilityid, int physicianId, DateTime dateselected, string typeofproc, out DateTime timeSlotDate, bool firstAvailable = false)
        {
            timeSlotDate = DateTime.Now;
            var sqlParameters = new SqlParameter[6];
            sqlParameters[0] = new SqlParameter("pFromDate", dateselected);
            sqlParameters[1] = new SqlParameter("pToDate", dateselected);
            sqlParameters[2] = new SqlParameter("pAppointMentType", typeofproc);
            sqlParameters[3] = new SqlParameter("pFacilityId", facilityid);
            sqlParameters[4] = new SqlParameter("pPhysicianId", physicianId);
            sqlParameters[5] = new SqlParameter("pFirst", firstAvailable);

            using (var multiResultSet = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetAvailableTimeSlots.ToString(), isCompiled: false, parameters: sqlParameters))
            {
                var mainList = multiResultSet.ResultSetFor<AvailabilityTimeSlotForPopupCustomModel>().ToList();

                if (firstAvailable)
                    timeSlotDate = multiResultSet.ResultSetFor<DateTime>().FirstOrDefault();

                return mainList;
            }
        }

        /// <summary>
        /// Checks for duplicate record recurring.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="timeFrom">The time from.</param>
        /// <param name="timeTo">The time to.</param>
        /// <param name="pFacilityid">The p facilityid.</param>
        /// <param name="pSchedulingId">The p scheduling identifier.</param>
        /// <param name="pPhysicianid">The p physicianid.</param>
        /// <param name="pRecPattern">The p record pattern.</param>
        /// <returns></returns>
        public bool CheckForDuplicateRecordRecurring(DateTime startDate, DateTime endDate, string timeFrom, string timeTo, int pFacilityid, int pSchedulingId, int pPhysicianid, string pRecPattern)
        {
            var spName =
                       string.Format(
                           "EXEC {0} @StartDate, @EndDate, @TimeFrom, @TimeTo, @pFacilityid, @pSchedulingId, @pPhysicianid, @pRec_Pattern",
                           StoredProcedures.SPROC_GetTimeSlotAvialablotyForRecurrance);
            var sqlParameters = new SqlParameter[8];
            sqlParameters[0] = new SqlParameter("StartDate", startDate);
            sqlParameters[1] = new SqlParameter("EndDate", endDate);
            sqlParameters[2] = new SqlParameter("TimeFrom", timeFrom);
            sqlParameters[3] = new SqlParameter("TimeTo", timeTo);
            sqlParameters[4] = new SqlParameter("pFacilityid", pFacilityid);
            sqlParameters[5] = new SqlParameter("pSchedulingId", pSchedulingId);
            sqlParameters[6] = new SqlParameter("pPhysicianid", pPhysicianid);
            sqlParameters[7] = new SqlParameter("pRec_Pattern", pRecPattern);
            IEnumerable<TimeSlotAvailabilityCustomModel> result = _context.Database.SqlQuery<TimeSlotAvailabilityCustomModel>(
                spName, sqlParameters);

            return result != null && result.Any() && result.ToList().Count > 0 && result.FirstOrDefault().TimeSlotAvailable > 0;

        }

        /// <summary>
        /// Gets the patient scheduling.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="selectedDate">The selected date.</param>
        /// <param name="viewtype">The viewtype.</param>
        /// <returns></returns>
        public List<SchedulingCustomModel> GetPatientScheduling(int patientId, DateTime selectedDate, string viewtype)
        {
            var spName = string.Format("EXEC {0} @ViewType, @selectedDate, @PatientId",
                       StoredProcedures.SPROC_GetPatientSchedulingEvents);
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("ViewType", viewtype);
            sqlParameters[1] = new SqlParameter("selectedDate", selectedDate);
            sqlParameters[2] = new SqlParameter("PatientId", patientId);
            IEnumerable<SchedulingCustomModel> result = _context.Database.SqlQuery<SchedulingCustomModel>(spName, sqlParameters);
            return result.ToList();
        }

        /// <summary>
        /// Gets the patient next scheduling.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="selectedDate">The selected date.</param>
        /// <returns></returns>
        public List<SchedulingCustomModel> GetPatientNextScheduling(int patientId, DateTime selectedDate)
        {
            var spName =
                       string.Format(
                           "EXEC {0} @selectedDate, @PatientId",
                           StoredProcedures.SPROC_GetPatientNextSchedulingEvents);
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("selectedDate", selectedDate);
            sqlParameters[1] = new SqlParameter("PatientId", patientId);
            IEnumerable<SchedulingCustomModel> result = _context.Database.SqlQuery<SchedulingCustomModel>(
                spName,
                sqlParameters);
            return result.ToList();
        }

        /// <summary>
        /// Gets the scheduling by physicians data.
        /// </summary>
        /// <param name="physicianIdlist">The physician idlist.</param>
        /// <param name="selectedDate">The selected date.</param>
        /// <param name="viewtype">The viewtype.</param>
        /// <returns></returns>
        public List<SchedulingCustomModel> GetSchedulingByPhysiciansData(string physicianIdlist, DateTime selectedDate, string viewtype, int facilityid)
        {
            var spName = string.Format("EXEC {0} @ViewType, @selectedDate, @PhysicianIdlist,@pfacilityId", StoredProcedures.SPROC_GetSchedulingEvents_Back);
            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter("ViewType", viewtype);
            sqlParameters[1] = new SqlParameter("selectedDate", selectedDate);
            sqlParameters[2] = new SqlParameter("PhysicianIdlist", physicianIdlist);
            sqlParameters[3] = new SqlParameter("pfacilityId", facilityid);
            IEnumerable<SchedulingCustomModel> result = _context.Database.SqlQuery<SchedulingCustomModel>(spName, sqlParameters);
            return result.ToList();
        }

        /// <summary>
        /// Gets the dept opening days for physician.
        /// </summary>
        /// <param name="physicianId">The physician identifier.</param>
        /// <returns></returns>
        public string GetDeptOpeningDaysForPhysician(int physicianId)
        {
            var phyDpeartmentObj = _phRepository.Get(physicianId);
            if (phyDpeartmentObj != null)
            {
                var phyDeptId = phyDpeartmentObj.FacultyDepartment;
                var deptTimmingobj = _dptRepository.FindAll(x => x.DeptTimmingId == Convert.ToInt32(phyDeptId));
                var deptOpeningdays = deptTimmingobj.Select(x => x.OpeningDayId).ToList();
                var objToReturn = string.Join(",", deptOpeningdays);
                return objToReturn;
            }

            return string.Empty;
        }

        /// <summary>
        /// Method to get the data for Over View popup
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public List<SchedularOverViewCustomModel> GetOverView(SchedularOverViewCustomModel m)
        {
            var spName = string.Format("EXEC {0} @StartDate, @EndDate, @StartTime, @EndTime, @AvgTimeSlot, @PassedPhysician, @PassedRoom, @DepartmentID, @FacilityID, @ViewType", StoredProcedures.AvailableSlotsMonthlyView_V1);
            var sqlParameters = new SqlParameter[10];
            sqlParameters[0] = new SqlParameter("StartDate", m.FromDate);
            sqlParameters[1] = new SqlParameter("EndDate", m.ToDate ?? "01/01/01");
            sqlParameters[2] = new SqlParameter("StartTime", m.FromTime);
            sqlParameters[3] = new SqlParameter("EndTime", m.ToTime);
            sqlParameters[4] = new SqlParameter("AvgTimeSlot", m.TimeSlotFrequency);
            sqlParameters[5] = new SqlParameter("PassedPhysician", m.Physician);
            sqlParameters[6] = new SqlParameter("PassedRoom", m.Room);
            sqlParameters[7] = new SqlParameter("DepartmentID", m.DepartmentId);
            sqlParameters[8] = new SqlParameter("FacilityID", m.FacilityId);
            sqlParameters[9] = new SqlParameter("ViewType", m.ViewType);
            IEnumerable<SchedularOverViewCustomModel> result = _context.Database.SqlQuery<SchedularOverViewCustomModel>(spName, sqlParameters);
            return result.ToList();
        }


        public List<SchedulingCustomModel> GetiSchedulingData(string associatedId, DateTime selectedDate, string viewtype, bool isPatient)
        {
            var spName =
                          string.Format(
                              "EXEC {0} @ViewType, @selectedDate, @AssociatedId,@IsPatient",
                              StoredProcedures.SPROC_GetPhyVacationsEvents);
            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter("ViewType", viewtype);
            sqlParameters[1] = new SqlParameter("selectedDate", selectedDate);
            sqlParameters[2] = new SqlParameter("AssociatedId", associatedId);
            sqlParameters[3] = new SqlParameter("IsPatient", isPatient);
            IEnumerable<SchedulingCustomModel> result = _context.Database.SqlQuery<SchedulingCustomModel>(
                spName, sqlParameters);
            return result.ToList();
        }

        /// <summary>
        /// Saves the patient pre scheduling list.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public SchedulingCustomModelView SavePatientPreSchedulingList(List<SchedulingCustomModel> model)
        {
            var schedulingObjList = new List<Scheduling>();
            schedulingObjList = MapValuesVM(model);
            if (schedulingObjList.Any())
            {
                foreach (var item in schedulingObjList)
                {
                    if (item.SchedulingId > 0)
                    {
                        if (item.IsRecurring == true)
                        {
                            DeleteHolidayPlannerData(item.EventParentId, 0, Convert.ToInt32(item.SchedulingType), "");
                        }
                        else
                        {
                            DeleteHolidayPlannerData(string.Empty, item.SchedulingId, Convert.ToInt32(item.SchedulingType), "");
                        }
                    }

                    _repository.Create(item);
                }
            }

            var list = new SchedulingCustomModelView();
            return list;
        }

        /// <summary>
        /// Gets the pre scheduling list.
        /// </summary>
        /// <param name="cId">The c identifier.</param>
        /// <param name="fId">The f identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public List<SchedulingCustomModel> GetPreSchedulingList(int cId, int fId)
        {
            var listroConvert = fId != 0
                                    ? _repository.Where(
                                        x => x.CorporateId == cId && x.FacilityId == fId && x.Status == "90")
                                          .ToList()
                                    : _repository.Where(
                                        x => x.CorporateId == cId && x.Status == "90")
                                          .ToList();
            var lst = new List<SchedulingCustomModel>();
            lst = MapValues(listroConvert);
            return lst;
        }

        /// <summary>
        /// Gets the phy previous vacations.
        /// </summary>
        /// <param name="facilityid">The facilityid.</param>
        /// <param name="physicianId">The physician identifier.</param>
        /// <returns></returns>
        public List<SchedulingCustomModel> GetPhyPreviousVacations(int facilityid, int physicianId)
        {
            var spName =
                           string.Format(
                               "EXEC {0} @facilityid, @physicianId",
                               StoredProcedures.SPROC_GetPhyVacationsEvents);
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("facilityid", facilityid);
            sqlParameters[1] = new SqlParameter("physicianId", physicianId);
            IEnumerable<SchedulingCustomModel> result = _context.Database.SqlQuery<SchedulingCustomModel>(
                spName, sqlParameters);
            return result.ToList();
        }

        /// <summary>
        /// Gets the facility holidays.
        /// </summary>
        /// <param name="facilityid">The facilityid.</param>
        /// <returns></returns>
        public List<SchedulingCustomModel> GetFacilityHolidays(int facilityid)
        {
            var spName =
                         string.Format(
                             "EXEC {0} @facilityid",
                             StoredProcedures.SPROC_GetFacilityHolidays);
            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("facilityid", facilityid);
            IEnumerable<SchedulingCustomModel> result = _context.Database.SqlQuery<SchedulingCustomModel>(
                spName, sqlParameters);
            return result.ToList();
        }

        public bool DeleteHolidaysByEventParentID(string eventParentId)
        {
            try
            {
                var valueToDelete = _repository.Where(x => x.EventParentId.Equals(eventParentId));
                _repository.Delete(valueToDelete);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }







        public RoomEquipmentAvialability GetAssignedRoomForProcedure(int facilityId, int appointmentType, DateTime scheduledDate, string timeFrom, string timeTo, int roomId, int schedulingId, int pId)
        {
            var spName = string.Format(
                          "EXEC {0} @FacilityId, @AppointmentType, @ScheduledDate, @TimeFrom, @TimeTo, @AvailableRoom, @SchId ,@pId",
                          StoredProcedures.SprocCheckRoomsAndOthersAvailibilty);

            var sqlParameters = new SqlParameter[8];
            sqlParameters[0] = new SqlParameter("FacilityId", facilityId);
            sqlParameters[1] = new SqlParameter("AppointmentType", appointmentType);
            sqlParameters[2] = new SqlParameter("ScheduledDate", scheduledDate);
            sqlParameters[3] = new SqlParameter("TimeFrom", timeFrom);
            sqlParameters[4] = new SqlParameter("TimeTo", timeTo);
            sqlParameters[5] = new SqlParameter("AvailableRoom", roomId);
            sqlParameters[6] = new SqlParameter("SchId", schedulingId);
            sqlParameters[7] = new SqlParameter("pId", pId);

            IEnumerable<RoomEquipmentAvialability> result = _context.Database.SqlQuery<RoomEquipmentAvialability>(
                spName, sqlParameters);
            return result.FirstOrDefault();
        }


        /// <summary>
        /// Gets the type of the scheduling dept data by.
        /// </summary>
        /// <param name="deptList">The dept identifier.</param>
        /// <param name="selectedDate">The selected date.</param>
        /// <param name="type">The type.</param>
        /// <param name="facilityId"></param>
        /// <returns></returns>
        public List<SchedulingCustomModel> GetSchedulingDataByDepartments(List<string> deptList, DateTime selectedDate, string type, int facilityId)
        {
            var lst = new List<SchedulingCustomModel>();
            var schedulingObj = _repository.Where(
                    x => (deptList.Count == 0 || deptList.Contains(x.ExtValue1)) && x.SchedulingType == type
                         && x.FacilityId == facilityId
                         && x.ScheduleTo.HasValue &&
                         (x.ScheduleFrom.HasValue &&
                          (x.ScheduleFrom.Value.Date == selectedDate.Date
                           || x.ScheduleTo.Value.Date == selectedDate.Date)))
                    .ToList();

            lst = MapValues(schedulingObj);
            return lst;
        }



        public List<SchedulingCustomModel> GetSchedulingDataByRooms(List<int> roomsList, DateTime selectedDate)
        {
            var lst = new List<SchedulingCustomModel>();
            var mlst = _repository.Where(x => x.RoomAssigned.HasValue && roomsList.Contains(x.RoomAssigned.Value)
                                && x.ScheduleTo != null &&
                                x.SchedulingType == "1"
                                &&
                                (x.ScheduleFrom != null &&
                                 (x.ScheduleFrom.Value.ToShortDateString() ==
                                  selectedDate.ToShortDateString()
                                  ||
                                  x.ScheduleTo.Value.ToShortDateString() ==
                                  selectedDate.ToShortDateString()))).ToList();

            lst = MapValues(mlst);
            return lst;
        }



        public List<SchedulerCustomModelForCalender> GetSchedulerData(int viewType, DateTime selectedDate, string phyIds, int fId, string depIds, string roomIds, string statusIds, string sectionType, int patientId)
        {
            if (phyIds.Equals("0"))
                phyIds = string.Empty;

            if (roomIds.Equals("0"))
                roomIds = string.Empty;

            if (depIds.Equals("0"))
                depIds = string.Empty;

            if (statusIds.Equals("0"))
                statusIds = string.Empty;


            var spName = string.Format(
                "EXEC {0} @pVT, @pDate, @pPhyIds, @pFId, @pDIds, @pRoomIds, @pStatusIds, @pSectionType, @pPatientId",
                StoredProcedures.SPROC_GetSchedulerDataUpdated);

            var sqlParameters = new SqlParameter[9];
            sqlParameters[0] = new SqlParameter("pVT", viewType);
            sqlParameters[1] = new SqlParameter("pDate", selectedDate);
            sqlParameters[2] = new SqlParameter("pPhyIds", phyIds);
            sqlParameters[3] = new SqlParameter("pFId", fId);
            sqlParameters[4] = new SqlParameter("pDIds", depIds);
            sqlParameters[5] = new SqlParameter("pRoomIds", roomIds);
            sqlParameters[6] = new SqlParameter("pStatusIds", statusIds);
            sqlParameters[7] = new SqlParameter("pSectionType", sectionType);
            sqlParameters[8] = new SqlParameter("@pPatientId", patientId);

            //We Assume here: _context is your EF DbContext
            using (var multiResultSet = _context.MultiResultSetSqlQuery(spName, parameters: sqlParameters))
            {
                var mainList = multiResultSet.ResultSetFor<SchedulerCustomModelForCalender>().ToList();

                if (mainList.Any())
                {
                    var innerList = multiResultSet.ResultSetFor<TypeOfProcedureCustomModel>().ToList();

                    var parentId = string.Empty;
                    var list2 = new List<TypeOfProcedureCustomModel>();

                    foreach (var item in mainList)
                    {
                        if (string.IsNullOrEmpty(parentId) || !item.EventParentId.Equals(parentId))
                            list2 = innerList.Where(p => p.EventParentId.Equals(item.EventParentId)).ToList();

                        item.TypeOfProcedureCustomModel = list2;
                    }
                }
                return mainList;
            }
        }


        public bool RemoveJustDeletedSchedulings(string eventParentid, List<int> listSchIds, int schedulingType, string extValue3)
        {
            try
            {
                var mList = !string.IsNullOrEmpty(eventParentid) ? _repository.Where(x => x.EventParentId == eventParentid).ToList() :
                    _repository.Where(x => listSchIds.Contains(x.SchedulingId)).ToList();

                mList = schedulingType == 1
                    ? mList.Where(x => x.ScheduleTo > DateTime.Now).ToList()
                    : mList.Where(x => x.ScheduleFrom.HasValue && x.ScheduleFrom.Value.Date >= DateTime.Now.Date &&
                                       x.ExtValue3 == extValue3).ToList();

                if (mList.Count > 0)
                {
                    _repository.Delete(mList);
                    return true;
                }

                return false;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<SchedulerCustomModelForCalender> GetSchedulerData(int viewType, DateTime selectedDate, string phyIds, int fId, string depIds, string roomIds, string statusIds, string sectionType, int patientId, out List<SchedulingCustomModel> nextList)
        {
            if (phyIds.Equals("0"))
                phyIds = string.Empty;

            if (roomIds.Equals("0"))
                roomIds = string.Empty;

            if (depIds.Equals("0"))
                depIds = string.Empty;

            if (statusIds.Equals("0"))
                statusIds = string.Empty;


            var spName = string.Format(
                "EXEC {0} @pVT, @pDate, @pPhyIds, @pFId, @pDIds, @pRoomIds, @pStatusIds, @pSectionType, @pPatientId",
                StoredProcedures.SPROC_GetPatientSchedulerDataUpdated);

            var sqlParameters = new SqlParameter[9];
            sqlParameters[0] = new SqlParameter("pVT", viewType);
            sqlParameters[1] = new SqlParameter("pDate", selectedDate);
            sqlParameters[2] = new SqlParameter("pPhyIds", phyIds);
            sqlParameters[3] = new SqlParameter("pFId", fId);
            sqlParameters[4] = new SqlParameter("pDIds", depIds);
            sqlParameters[5] = new SqlParameter("pRoomIds", roomIds);
            sqlParameters[6] = new SqlParameter("pStatusIds", statusIds);
            sqlParameters[7] = new SqlParameter("pSectionType", sectionType);
            sqlParameters[8] = new SqlParameter("@pPatientId", patientId);

            //We Assume here: _context is your EF DbContext
            using (var multiResultSet = _context.MultiResultSetSqlQuery(spName, parameters: sqlParameters))
            {
                var mainList = multiResultSet.ResultSetFor<SchedulerCustomModelForCalender>().ToList();

                if (mainList.Any())
                {
                    var innerList = multiResultSet.ResultSetFor<TypeOfProcedureCustomModel>().ToList();

                    var parentId = string.Empty;
                    var list2 = new List<TypeOfProcedureCustomModel>();

                    foreach (var item in mainList)
                    {
                        if (string.IsNullOrEmpty(parentId) || !item.EventParentId.Equals(parentId))
                            list2 = innerList.Where(p => p.EventParentId.Equals(item.EventParentId)).ToList();

                        item.TypeOfProcedureCustomModel = list2;
                    }
                }


                nextList = multiResultSet.ResultSetFor<SchedulingCustomModel>().ToList();

                return mainList;
            }
        }


        public int SavePatientInfoInScheduler(int cId, int fId, DateTime pDate, int pId, string firstName, string lastName, DateTime? dob, string email, string emirateId, int loggedUserId, string phone, int age, string newPwd = "")
        {
            var spName =
                          string.Format("EXEC {0} @pCId,@pFId,@pDate,@pPid,@pFirstName,@pLastName,@pDob,@pEmail,@pEmirates,@pLoggedInUserId,@pPhone,@pAge",
                          StoredProcedures.SavePatientInfoInScheduler);

            var sqlParameters = new SqlParameter[13];
            sqlParameters[0] = new SqlParameter("pCId", cId);
            sqlParameters[1] = new SqlParameter("pFId", fId);
            sqlParameters[2] = new SqlParameter("pDate", pDate);
            sqlParameters[3] = new SqlParameter("pPid", pId);
            sqlParameters[4] = new SqlParameter("pFirstName", firstName);
            sqlParameters[5] = new SqlParameter("pLastName", lastName);
            sqlParameters[6] = new SqlParameter("pDob", dob);
            sqlParameters[7] = new SqlParameter("pEmail", email);
            sqlParameters[8] = new SqlParameter("pEmirates", !string.IsNullOrEmpty(emirateId) ? emirateId : string.Empty);
            sqlParameters[9] = new SqlParameter("pLoggedInUserId", loggedUserId);
            sqlParameters[10] = new SqlParameter("pPhone", phone);
            sqlParameters[11] = new SqlParameter("pAge", age);
            sqlParameters[12] = new SqlParameter("pPwd", newPwd);
            var result = _context.Database.SqlQuery<int>(spName, sqlParameters);
            return result.FirstOrDefault();
        }


        public bool AddUpdatePatientScheduling(List<SchedulingCustomModel> model, int facilityId, List<int> tobedeleted)
        {
            var result = false;
            var mList = new List<Scheduling>();
            var data = new List<SchCreatedData>();

            /* %%%%---- Delete Operation STARTS---%%%% */
            var deletedList = _repository.Where(s => tobedeleted.Contains(s.SchedulingId)).ToList();
            if (deletedList.Count > 0)
                _repository.Delete(deletedList);
            /* %%%%---- Delete Operation ENDS---%%%% */

            mList = MapValuesVM(model);
            if (mList.Any())
            {
                /* %%%%---- Update Operation STARTS---%%%% */
                var updateList = mList.Where(s => s.SchedulingId > 0).ToList();
                if (updateList.Count > 0)
                {
                    var ids = updateList.Select(a => a.SchedulingId).ToList();
                    data = GetCreatedDateByIds(ids, facilityId);

                    updateList.ForEach(u => data.Where(a1 => a1.Id == u.SchedulingId).ToList().ForEach(a =>
                    {
                        a.IsRecurring = (u.IsRecurring == true);
                        a.IsNew = false;
                        u.CreatedBy = a.CreatedBy;
                        u.CreatedDate = a.CreatedDate;
                    }));

                    foreach (var u in updateList)
                        _repository.UpdateEntity(u, u.SchedulingId);

                    //rep.Update(updateList);
                }
                /* %%%%---- Update Operation ENDS---%%%% */


                /* %%%%---- Create Operation starts---%%%% */
                var allNewlist = mList.Where(a => a.SchedulingId == 0).ToList();

                if (allNewlist.Count > 0)
                    _repository.Create(allNewlist);

                data.AddRange(allNewlist.Select(item => new SchCreatedData
                {
                    Id = item.SchedulingId,
                    IsRecurring = (item.IsRecurring == true),
                    IsNew = true
                }));
                /* %%%%---- Create Operation ends---%%%% */



                /* %%%%---- Create Recurring Operation STARTS---%%%% */
                var rList = data.Where(a => a.IsRecurring && model.Any(a1 => a1.SchedulingId == a.Id && a1.UpdateFlag))
                        .Select(f => f.Id)
                        .ToList();
                if (rList.Count > 0)
                {
                    var ids = string.Join(",", rList);
                    var spName =
                        string.Format("EXEC {0} @pSchedulingId", StoredProcedures.SPROC_CreateRecurringEventsSchedularMultiple);
                    var sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("pSchedulingIds", ids);
                    sqlParameters[1] = new SqlParameter("pFacilityId", facilityId);
                    _repository.ExecuteCommand(StoredProcedures.SPROC_CreateRecurringEventsSchedularMultiple.ToString(), sqlParameters);
                    var st = _context.Database.SqlQuery<int>(spName, sqlParameters).FirstOrDefault();
                    result = st > 0;
                }
                /* %%%%---- Create Recurring Operation ENDS---%%%% */
            }
            return result;
        }


        private List<SchCreatedData> GetCreatedDateByIds(List<int> ids, int facilitId)
        {
            var dt = GetInvariantCultureDateTime(facilitId);
            var mList = _repository.Where(s => ids.Contains(s.SchedulingId));
            if (mList.Any())
            {
                var list = mList.Select(a => new SchCreatedData
                {
                    Id = a.SchedulingId,
                    CreatedBy = a.CreatedBy ?? 1,
                    CreatedDate = a.CreatedDate ?? dt
                }).ToList();
                return list;
            }
            return new List<SchCreatedData>();
        }
        private DateTime GetInvariantCultureDateTime(int facilityid)
        {
            var facilityObj = _fRepository.Where(f => f.FacilityId == Convert.ToInt32(facilityid)).FirstOrDefault() != null ? _fRepository.Where(f => f.FacilityId == Convert.ToInt32(facilityid)).FirstOrDefault().FacilityTimeZone : TimeZoneInfo.Utc.ToString();
            var tzi = TimeZoneInfo.FindSystemTimeZoneById(facilityObj);
            var utcTime = DateTime.Now.ToUniversalTime();
            var convertedTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
            return convertedTime;
        }
        public List<SchedulingCustomModel> ValidateScheduling(DataTable dt, int facilityId, int userId, out int status)
        {
            status = 0;
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter
            {
                ParameterName = "pSchedulingList",
                SqlDbType = SqlDbType.Structured,
                Value = dt,
                TypeName = "SchedulerArrayTT"
            };
            sqlParameters[1] = new SqlParameter("pFId", facilityId);
            sqlParameters[2] = new SqlParameter("pUserId", userId);

            //We Assume here: _context is your EF DbContext
            using (var ms = _context.MultiResultSetSqlQuery(StoredProcedures.SprocValidateAppointment.ToString(), isCompiled: false, parameters: sqlParameters))
            {
                var mainList = ms.ResultSetFor<SchedulingCustomModel>().ToList();
                status = ms.ResultSetFor<int>().FirstOrDefault();
                return mainList;
            }
        }

        public bool UpdateAppointmentStatus(long schedulingId, string status, int userId, DateTime currentDatetime)
        {
            var current = _repository.GetSingle(schedulingId);
            if (current != null)
            {
                current.Status = status;
                current.ModifiedBy = userId;
                current.ModifiedDate = currentDatetime;

                var result = _repository.Updatei(current, schedulingId);
                return result >= 0;
            }
            return false;
        }


        public List<Scheduling> MapVMToModel(List<SchedulingCustomModel> vm)
        {
            if (vm != null)
                return MapValuesVM(vm);

            return Enumerable.Empty<Scheduling>().ToList();
        }

    }

    public class SchCreatedData
    {
        public int Id { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsRecurring { get; set; }
        public bool IsNew { get; set; }
    }
}