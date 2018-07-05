using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AutoMapper;
using System.Threading.Tasks;

using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Common.Common;
using System.Data.SqlClient;

using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class PhysicianService : IPhysicianService
    {
        private readonly IRepository<Physician> _repository;
        private readonly IRepository<Encounter> _eRepository;
        private readonly IRepository<FacilityRole> _frRepository;
        private readonly IRepository<UserRole> _urRepository;
        private readonly IRepository<Users> _uRepository;
        private readonly IRepository<FacilityStructure> _fsRepository;
        private readonly IRepository<Facility> _fRepository;
        private readonly IRepository<GlobalCodes> _gRepository;
        private readonly IRepository<Role> _roRepository;
        private readonly IMapper _mapper;
        private readonly BillingEntities _context;

        public PhysicianService(IRepository<Physician> repository, IRepository<Encounter> eRepository, IRepository<FacilityRole> frRepository
            , IRepository<UserRole> urRepository, IRepository<Users> uRepository, IRepository<FacilityStructure> fsRepository, IRepository<Facility> fRepository
            , IRepository<GlobalCodes> gRepository, IRepository<Role> roRepository, IMapper mapper, BillingEntities context)
        {
            _repository = repository;
            _eRepository = eRepository;
            _frRepository = frRepository;
            _urRepository = urRepository;
            _uRepository = uRepository;
            _fsRepository = fsRepository;
            _fRepository = fRepository;
            _gRepository = gRepository;
            _roRepository = roRepository;
            _mapper = mapper;
            _context = context;
        }

        #region Public Methods and Operators

        /// <summary>
        /// Method to add/Update the Physician in the database.
        /// </summary>
        /// <param name="vm">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int AddUpdatePhysician(PhysicianViewModel vm)
        {
            var model = _mapper.Map<Physician>(vm);
            if (model.Id > 0)
            {
                var current = _repository.GetSingle(vm.Id);
                model.CreatedDate = current.CreatedDate;
                model.CreatedBy = current.CreatedBy;
                model.UserId = current.UserId;


                _repository.UpdateEntity(model, model.Id);
            }
            else
                _repository.Create(model);


            // Create the Custom Lunch timming for the scheduling in the DB table Faculty Timeslots 
            if ((!string.IsNullOrEmpty(vm.FacultyLunchTimeFrom) && vm.FacultyLunchTimeFrom != "__:__")
                && (!string.IsNullOrEmpty(vm.FacultyLunchTimeTill) && vm.FacultyLunchTimeTill != "__:__"))
            {
                AddFacultyLunchTimmings(vm.CorporateId, vm.FacilityId, Convert.ToInt32(vm.Id),
                    Convert.ToInt32(vm.FacultyDepartment), vm.FacultyLunchTimeFrom, vm.FacultyLunchTimeTill);
            }

            // Add Default timming in Faculty rooster screen
            AddFacultydefaultTimmings(vm.CorporateId, vm.FacilityId, Convert.ToInt32(vm.Id),
                   Convert.ToInt32(vm.FacultyDepartment), Convert.ToInt32(vm.UserType));

            return vm.Id;
        }
        private bool AddFacultydefaultTimmings(int corporateId, int facilityId, int userid, int departmentid, int userType)
        {
            try
            {
                var sqlParameters = new SqlParameter[5];
                sqlParameters[0] = new SqlParameter("pFacultyId", userid);
                sqlParameters[1] = new SqlParameter("pUserType", userType);
                sqlParameters[2] = new SqlParameter("pDeptId", departmentid);
                sqlParameters[3] = new SqlParameter("pFId", facilityId);
                sqlParameters[4] = new SqlParameter("pCId", corporateId);
                _repository.ExecuteCommand(StoredProcedures.SPROC_AddFacultyDefaultTiming.ToString(), sqlParameters);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private bool AddFacultyLunchTimmings(int corporateId, int facilityId, int userid, int departmentid, string lunchTimefrom, string lunchTimeTill)
        {
            try
            {
                var sqlParameters = new SqlParameter[6];
                sqlParameters[0] = new SqlParameter("pCID", corporateId);
                sqlParameters[1] = new SqlParameter("pFID", facilityId);
                sqlParameters[2] = new SqlParameter("pUserId", userid);
                sqlParameters[3] = new SqlParameter("pDeptId", departmentid);
                sqlParameters[4] = new SqlParameter("pLunchTimeFrom", lunchTimefrom);
                sqlParameters[5] = new SqlParameter("pLunchTimeTill", lunchTimeTill);
                _repository.ExecuteCommand(StoredProcedures.SPROC_AddUpdateFacultyLunchTimming.ToString(), sqlParameters);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// Checks the duplicate clinical identifier.
        /// </summary>
        /// Updated By Krishna on 10072015
        /// <param name="clinicalId">
        /// The clinical identifier.
        /// </param>
        /// <param name="id">
        /// The identifier.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CheckDuplicateClinicalId(string clinicalId, int id)
        {
            var isExist = id > 0
                ? _repository.Where(x => x.Id != id && x.PhysicianLicenseNumber.ToLower().Trim().Equals(clinicalId) && x.IsDeleted == null).Any()
                : _repository.Where(x => x.PhysicianLicenseNumber.ToLower().Equals(clinicalId) && x.IsDeleted == null).Any();
            return isExist;
        }

        /// <summary>
        /// Checks the duplicate emp no and clinical identifier.
        /// </summary>
        /// update by Krishna on 10072015
        /// <param name="empNo">
        /// The emp no.
        /// </param>
        /// <param name="physicianId">
        /// The clinical identifier.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CheckDuplicateEmpNo(int empNo, int physicianId)
        {
            var isExists = _repository.Where(p => (p.Id != physicianId || physicianId == 0) && p.PhysicianEmployeeNumber == empNo && p.IsDeleted != true).Any();
            return isExists;
        }

        /// <summary>
        /// Checks if user type and user identifier already exists.
        /// </summary>
        /// <param name="userType">
        /// Type of the user.
        /// </param>
        /// <param name="userId">
        /// The user identifier.
        /// </param>
        /// <param name="physicianId">
        /// The physician identifier.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CheckIfUserTypeAndUserIdAlreadyExists(int userType, int userId, int physicianId)
        {
            bool isExists = _repository.Where(p => (p.Id != physicianId || physicianId == 0) && p.IsActive && p.IsDeleted != true && p.UserType == userType && p.UserId == userId).Any();

            return isExists;
        }

        /// <summary>
        /// The get distinct users by user type id.
        /// </summary>
        /// <param name="roleId">
        /// The user type id.
        /// </param>
        /// <param name="corporateId">
        /// The corporate id.
        /// </param>
        /// <param name="facilityId">
        /// The facility id.
        /// </param>
        /// <returns>
        /// </returns>
        public List<Users> GetDistinctUsersByUserTypeId(int roleId, int corporateId, int facilityId)
        {
            var list = _uRepository.Where(u => u.CorporateId == corporateId && u.FacilityId == facilityId)
                    .Include(f => f.UserRole)
                    .Where(u => u.UserRole.Any(u1 => u1.RoleID == roleId))
                    .ToList();

            if (list.Count > 0)
            {
                list = list.GroupBy(x => x.UserID).Select(grp => grp.First()).ToList();
            }

            return list;
        }

        /// <summary>
        /// The get facility physicians.
        /// </summary>
        /// <param name="facilityId">
        /// The facility id.
        /// </param>
        /// <returns>
        /// </returns>
        public List<Physician> GetFacilityPhysicians(int facilityId)
        {
            var lstPhysician = _repository.Where(x => x.IsActive && (x.IsDeleted == null || !(bool)x.IsDeleted)
                   && (x.PhysicianPrimaryFacility == facilityId || x.PhysicianSecondaryFacility == facilityId
                       || x.PhysicianThirdFacility == facilityId)).ToList();
            return lstPhysician;
        }

        /// <summary>
        /// The get name by license type id and user type id.
        /// </summary>
        /// <param name="licenceTypeId">
        /// The licence type id.
        /// </param>
        /// <param name="userTypeId">
        /// The user type id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
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

        /// <summary>
        /// Gets the physician cmodel by identifier.
        /// </summary>
        /// <param name="physicianId">The physician identifier.</param>
        /// <returns></returns>
        public PhysicianCustomModel GetPhysicianCModelById(int physicianId)
        {
            var m = _repository.Get(physicianId);
            var vm = MapValues(m);
            return vm;
        }

        /// <summary>
        /// Gets the physician by user identifier.
        /// </summary>
        /// <param name="physicianId">
        /// The physician identifier.
        /// </param>
        /// <returns>
        /// The <see cref="Physician"/>.
        /// </returns>
        public Physician GetPhysicianByUserId(int physicianId)
        {
            var m = _repository.Where(x => x.UserId == physicianId).FirstOrDefault();
            return m ?? new Physician();
        }

        /// <summary>
        /// The get physicians by physician type id.
        /// </summary>
        /// <param name="physicianTypeId">
        /// The physician type id.
        /// </param>
        /// <param name="facilityid">
        /// The facilityid.
        /// </param>
        /// <returns>
        /// </returns>
        public List<Physician> GetPhysiciansByPhysicianTypeId(int physicianTypeId, int facilityid)
        {
            var list = _repository.GetAll()
                    .Where(
                        _ =>
                        (_.PhysicianPrimaryFacility == facilityid || _.PhysicianSecondaryFacility == facilityid
                         || _.PhysicianThirdFacility == facilityid) && _.PhysicianLicenseType == physicianTypeId
                         && _.IsActive && _.IsDeleted == false)
                    .ToList();
            return list;
        }

        /// <summary>
        /// The get physicians list by facility id.
        /// </summary>
        /// <param name="facilityId">
        /// The facility id.
        /// </param>
        /// <returns>
        /// </returns>
        public List<PhysicianCustomModel> GetPhysiciansListByFacilityId(int facilityId)
        {
            var list = new List<PhysicianCustomModel>();
            /*Updated by Krishna on 15072015*/
            var physiciansList = _repository.Where(
                    x => x.IsActive && (x.IsDeleted == null || x.IsDeleted == false)
                    && (x.PhysicianPrimaryFacility == facilityId || x.PhysicianSecondaryFacility == facilityId
                        || x.PhysicianThirdFacility == facilityId)).ToList();

            if (physiciansList.Count > 0)
            {
                list.AddRange(
                    physiciansList.Select(
                        item =>
                        new PhysicianCustomModel
                        {
                            Physician = item,
                            PrimaryFacilityName =
                                    GetFacilityNameByFacilityId(
                                        Convert.ToInt32(item.PhysicianPrimaryFacility)),
                            SecondaryFacilityName =
                                    GetFacilityNameByFacilityId(
                                        Convert.ToInt32(item.PhysicianSecondaryFacility)),
                            ThirdFacilityName =
                                    GetFacilityNameByFacilityId(
                                        Convert.ToInt32(item.PhysicianThirdFacility)),
                            PhysicanLicenseTypeName =
                                    GetNameByLicenseTypeIdAndUserTypeId(
                                        Convert.ToString(item.PhysicianLicenseType),
                                        Convert.ToString(item.UserType)),
                            UserTypeStr = GetRoleName(Convert.ToInt32(item.UserType)),
                            UserDepartmentStr =
                                    Convert.ToInt32(item.FacultyDepartment) > 0
                                        ? GetDepartmentNameById(
                                            Convert.ToInt32(item.FacultyDepartment))
                                        : string.Empty,
                            UserSpecialityStr =
                                    GetNameByGlobalCodeValue(
                                        Convert.ToString(item.FacultySpeciality),
                                        Convert.ToString(
                                            (int)GlobalCodeCategoryValue.PhysicianSpecialties))
                        }));
            }

            return list;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get role name.
        /// </summary>
        /// <param name="roleId">
        /// The role id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetRoleName(int roleId)
        {
            return _roRepository.Where(r => r.RoleID == roleId).FirstOrDefault().RoleName;
        }

        #endregion

        /// <summary>
        /// Method is used to get the physician list by patient id
        /// </summary>
        /// <param name="patientId"></param>
        /// <returns></returns>
        public List<PhysicianCustomModel> GetPhysicianListByPatientId(int patientId)
        {
            var encounterList = _eRepository.Where(i => i.PatientID == patientId).ToList();
            var phyobj = _repository.GetAll();
            return (from item in encounterList
                    let item1 = item
                    let physicianList = phyobj.Where(x => x.Id == item1.EncounterAttendingPhysician).FirstOrDefault()
                    select new PhysicianCustomModel
                    {
                        Physician = physicianList,
                        EncounterNumber = item.EncounterNumber
                    }).ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="facilityId"></param>
        /// <param name="specialityId"></param>
        /// <returns></returns>
        public List<PhysicianCustomModel> BindPhysicianBySpeciality(int facilityId, string specialityId)
        {
            var list = new List<PhysicianCustomModel>();
            if (specialityId == "0")
                list = GetCorporatePhysiciansList(0, true, 0, facilityId);
            else
            {
                var physicianList = _repository.Where(x => x.FacilityId == facilityId && x.FacultySpeciality == specialityId && x.IsDeleted != true).ToList();
                list = MapValues(physicianList);
            }
            return list;
        }
        private List<PhysicianCustomModel> MapValues(List<Physician> m)
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
        public List<PhysicianCustomModel> GetCorporatePhysiciansList(int corporateId, bool isadmin, int userid, int facilityId)
        {
            try
            {
                var physiciansList = new List<PhysicianCustomModel>();
                var roles = facilityId > 0
                                ? _roRepository.Where(
                                    r =>
                                    r.FacilityId == facilityId
                                    && (r.RoleName.Contains("Phy") || r.RoleName.Contains("Nur"))
                                    && !r.IsDeleted && r.IsActive).Select(r => r.RoleID).ToList()
                                : _roRepository.Where(
                                    r =>
                                    r.CorporateId == corporateId
                                    && (r.RoleName.Contains("Phy") || r.RoleName.Contains("Nur"))
                                    && !r.IsDeleted && r.IsActive).Select(r => r.RoleID).ToList();

                if (roles.Count > 0)
                {
                    List<Physician> list;
                    var userIds = _urRepository.Where(r => roles.Contains(r.RoleID)).Select(u => u.UserID).ToList();
                    list = facilityId > 0
                               ? _repository.Where(
                                   p =>
                                   p.FacilityId == facilityId && userIds.Any(u => p.UserId == u)
                                   && p.IsActive && p.IsDeleted != true).ToList()
                               : _repository.Where(
                                   p =>
                                   p.CorporateId == corporateId && userIds.Any(u => p.UserId == u)
                                   && p.IsActive && p.IsDeleted != true).ToList();

                    if (list.Count > 0)
                    {
                        physiciansList = MapValues(list);
                    }
                }
                return physiciansList;
            }
            catch
            {
                return new List<PhysicianCustomModel>();
            }
        }


        public List<Physician> GetPhysicianByCorporateIdandfacilityId(int corporateId, int facilityId)
        {
            var list = _repository.Where(x => x.CorporateId == corporateId && x.FacilityId == facilityId).ToList();

            return list;
        }

        /// <summary>
        /// Gets the corporate physicians.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<PhysicianCustomModel> GetCorporatePhysiciansPreScheduling(int corporateId, int facilityId)
        {
            try
            {
                var physiciansList = new List<PhysicianCustomModel>();
                var roles = facilityId > 0
                                ? _roRepository.Where(
                                    r =>
                                    r.FacilityId == facilityId
                                    && (r.RoleName.Contains("Phy") || r.RoleName.Contains("Nur"))
                                    && !r.IsDeleted && r.IsActive).Select(r => r.RoleID).ToList()
                                : _roRepository.Where(
                                    r =>
                                    r.CorporateId == corporateId
                                    && (r.RoleName.Contains("Phy") || r.RoleName.Contains("Nur"))
                                    && !r.IsDeleted && r.IsActive).Select(r => r.RoleID).ToList();

                if (roles.Count > 0)
                {
                    List<Physician> list;
                    var userIds = _urRepository.Where(r => roles.Contains(r.RoleID)).Select(u => u.UserID).ToList();
                    list = facilityId > 0
                               ? _repository.Where(
                                   p =>
                                   p.FacilityId == facilityId && userIds.Any(u => p.UserId == u)
                                   && p.IsActive && p.IsDeleted != true).ToList()
                               : _repository.Where(
                                   p =>
                                   p.CorporateId == corporateId && userIds.Any(u => p.UserId == u)
                                   && p.IsActive && p.IsDeleted != true).ToList();

                    if (list.Count > 0)
                    {
                        physiciansList = MapValues(list);
                    }
                }
                return physiciansList;
            }
            catch
            {
                return new List<PhysicianCustomModel>();
            }
        }



        public List<PhysicianCustomModel> GetPhysicians(int corporateId, bool isadmin, int userid, int facilityId)
        {
            var physiciansList = new List<PhysicianCustomModel>();
            var uList = new List<int>();

            //Check If Logged-On User is a Admin
            if (isadmin)
            {
                var roles = _frRepository.Where(r =>
                         ((facilityId == 0 && r.CorporateId == corporateId) || r.FacilityId == facilityId) &&
                         !r.IsDeleted && r.IsActive && r.SchedulingApplied).Select(r => r.RoleId).ToList();

                //Get Users based on Roles Names Physicians and Nurses.
                if (roles.Count > 0)
                {
                    uList = _urRepository.Where(r => roles.Contains(r.RoleID)).Select(u => u.UserID).ToList();
                }
            }

            //Finally, Get Physicians Data based on the above selected parameters.
            var list =
                _repository.Where(r => ((facilityId == 0 && r.CorporateId == corporateId) || r.FacilityId == facilityId) &&
                               ((isadmin && uList.Any(u => r.UserId == u)) || (!isadmin && r.UserId == userid))
                               && r.IsActive && r.IsDeleted != true).ToList();

            //Map Model to ViewModel
            if (list.Any())
                physiciansList = MapValues(list);

            if (physiciansList.Any())
                physiciansList = physiciansList.OrderBy(p => p.UserTypeStr).ThenBy(p1 => p1.Physician.PhysicianName).ToList();

            return physiciansList;

        }


        public async Task<List<PhysicianViewModel>> GetFacultyList(int facilityId, long userId = 0)
        {
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("@FId", facilityId);
            sqlParameters[1] = new SqlParameter("@UserId", userId);

            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SPROC_GetPhysiciansByFacility.ToString(), false, parameters: sqlParameters))
            {
                var result = (await r.ResultSetForAsync<PhysicianViewModel>()).ToList();
                return result;
            }
        }
    }
}