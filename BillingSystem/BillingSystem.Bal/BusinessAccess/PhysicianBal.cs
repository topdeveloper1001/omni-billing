// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhysicianBal.cs" company="Spadez">
//   OmniHealthcare
// </copyright>
// <summary>
//   Defines the PhysicianBal type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BillingSystem.Bal.BusinessAccess
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;

    using Mapper;
    using Common.Common;
    using Model;
    using Model.CustomModel;
    using Repository.GenericRepository;
    using AutoMapper;
    using System.Threading.Tasks;

    /// <summary>
    ///     The physician bal.
    /// </summary>
    public class PhysicianBal : BaseBal
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PhysicianBal"/> class.
        /// </summary>
        public PhysicianBal()
        {
            PhysicianMapper = new PhysicianMapper();
            
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the physician mapper.
        /// </summary>
        private PhysicianMapper PhysicianMapper { get; set; }

        #endregion

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
            using (var rep = UnitOfWork.PhysicianRepository)
            {
                var model = Mapper.Map<Physician>(vm);
                if (model.Id > 0)
                {
                    var current = rep.GetSingle(vm.Id);
                    model.CreatedDate = current.CreatedDate;
                    model.CreatedBy = current.CreatedBy;
                    model.UserId = current.UserId;


                    rep.UpdateEntity(model, model.Id);
                }
                else
                    rep.Create(model);


                // Create the Custom Lunch timming for the scheduling in the DB table Faculty Timeslots 
                if ((!string.IsNullOrEmpty(vm.FacultyLunchTimeFrom) && vm.FacultyLunchTimeFrom != "__:__")
                    && (!string.IsNullOrEmpty(vm.FacultyLunchTimeTill) && vm.FacultyLunchTimeTill != "__:__"))
                {
                    rep.AddFacultyLunchTimmings(vm.CorporateId, vm.FacilityId, Convert.ToInt32(vm.Id),
                        Convert.ToInt32(vm.FacultyDepartment), vm.FacultyLunchTimeFrom, vm.FacultyLunchTimeTill);
                }

                // Add Default timming in Faculty rooster screen
                rep.AddFacultydefaultTimmings(vm.CorporateId, vm.FacilityId, Convert.ToInt32(vm.Id),
                       Convert.ToInt32(vm.FacultyDepartment), Convert.ToInt32(vm.UserType));

                return vm.Id;
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
            using (var rep = UnitOfWork.PhysicianRepository)
            {
                var isExist = id > 0
                    ? rep.Where(
                        x =>
                            x.Id != id && x.PhysicianLicenseNumber.ToLower().Trim().Equals(clinicalId)
                            && x.IsDeleted == null).Any()
                    : rep.Where(
                        x =>
                            x.PhysicianLicenseNumber.ToLower().Equals(clinicalId) && x.IsDeleted == null)
                        .Any();
                return isExist;
            }
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
            using (var rep = UnitOfWork.PhysicianRepository)
            {
                var isExists =
                    rep.Where(p => (p.Id != physicianId || physicianId == 0) && p.PhysicianEmployeeNumber == empNo && p.IsDeleted != true)
                        .Any();
                return isExists;
            }
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
            bool isExists;
            using (var rep = UnitOfWork.PhysicianRepository)
            {
                isExists =
                    rep.Where(
                        p =>
                        (p.Id != physicianId || physicianId == 0) && p.IsActive && p.IsDeleted != true
                        && p.UserType == userType && p.UserId == userId).Any();
            }

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
            using (var rep = UnitOfWork.UsersRepository)
            {
                var list =
                    rep.Where(u => u.CorporateId == corporateId && u.FacilityId == facilityId)
                        .Include(f => f.UserRole)
                        .Where(u => u.UserRole.Any(u1 => u1.RoleID == roleId))
                        .ToList();

                if (list.Count > 0)
                {
                    list = list.GroupBy(x => x.UserID).Select(grp => grp.First()).ToList();
                }

                return list;
            }
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
            try
            {
                using (var physicianRep = UnitOfWork.PhysicianRepository)
                {
                    var lstPhysician =
                        physicianRep.Where(
                            x =>
                            x.IsActive && (x.IsDeleted == null || !(bool)x.IsDeleted)
                            && (x.PhysicianPrimaryFacility == facilityId || x.PhysicianSecondaryFacility == facilityId
                                || x.PhysicianThirdFacility == facilityId)).ToList();
                    return lstPhysician;
                }
            }
            catch (Exception)
            {
                return new List<Physician>();
            }
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
            using (var physicianRep = UnitOfWork.PhysicianRepository)
            {
                var lstPhysicianCustomModel = new List<PhysicianCustomModel>();
                var physicianObj = physicianRep.Where(x => x.Id == physicianId).ToList();
                lstPhysicianCustomModel.AddRange(physicianObj.Select(item => PhysicianMapper.MapModelToViewModel(item)));
                return lstPhysicianCustomModel.FirstOrDefault();
            }
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
            using (var physicianRep = UnitOfWork.PhysicianRepository)
            {
                var physician = physicianRep.Where(x => x.UserId == physicianId).FirstOrDefault();
                return physician ?? new Physician();
            }
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
            using (var physicianRep = UnitOfWork.PhysicianRepository)
            {
                var list =
                    physicianRep.GetAll()
                        .Where(
                            _ =>
                            (_.PhysicianPrimaryFacility == facilityid || _.PhysicianSecondaryFacility == facilityid
                             || _.PhysicianThirdFacility == facilityid) && _.PhysicianLicenseType == physicianTypeId
                             && _.IsActive && _.IsDeleted == false)
                        .ToList();
                return list;
            }
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
            using (var physicianRep = UnitOfWork.PhysicianRepository)
            {
                /*Updated by Krishna on 15072015*/
                var physiciansList =
                    physicianRep.Where(
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
            using (var roleBal = new RoleBal())
            {
                return roleBal.GetRoleNameById(roleId);
            }
        }

        #endregion

        /// <summary>
        /// Method is used to get the physician list by patient id
        /// </summary>
        /// <param name="patientId"></param>
        /// <returns></returns>
        public List<PhysicianCustomModel> GetPhysicianListByPatientId(int patientId)
        {
            var encounterList = UnitOfWork.EncounterRepository.Where(i => i.PatientID == patientId).ToList();
            return (from item in encounterList
                    let item1 = item
                    let physicianList =
                        UnitOfWork.PhysicianRepository.Where(x => x.Id == item1.EncounterAttendingPhysician)
                            .FirstOrDefault()
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
                var physicianList = UnitOfWork.PhysicianRepository.Where(
                x => x.FacilityId == facilityId && x.FacultySpeciality == specialityId && x.IsDeleted != true)
                .ToList();
                list.AddRange(physicianList.Select(item => PhysicianMapper.MapModelToViewModel(item)));
            }
            return list;
        }

        public List<PhysicianCustomModel> GetCorporatePhysiciansList(int corporateId, bool isadmin, int userid, int facilityId)
        {
            try
            {
                //var physiciansList = new List<PhysicianCustomModel>();
                //using (var rep = UnitOfWork.PhysicianRepository)
                //{
                //    using (var rRep = UnitOfWork.RoleRepository)
                //    {
                //        var roleNames = new[] { "Physicians", "Nurses" };
                //        //var roles =
                //        //    rRep.Where(r =>
                //        //        ((facilityId == 0 && r.CorporateId == corporateId) || r.FacilityId == facilityId) &&
                //        //        (r.RoleName.Contains("Phy") || r.RoleName.Contains("Nur")) && !r.IsDeleted &&
                //        //        r.IsActive).Select(r => r.RoleID).ToList();

                //        var roles =
                //            rRep.Where(r =>
                //                ((facilityId == 0 && r.CorporateId == corporateId) || r.FacilityId == facilityId) &&
                //                roleNames.Contains(r.RoleName) &&
                //                !r.IsDeleted &&
                //                r.IsActive).Select(r => r.RoleID).ToList();

                //        if (roles.Count > 0)
                //        {
                //            List<Physician> list;
                //            if (isadmin)
                //            {
                //                using (var urRep = UnitOfWork.UserRoleRepository)
                //                {
                //                    var userIds = urRep.Where(r => roles.Contains(r.RoleID)).Select(u => u.UserID).ToList();
                //                    list = facilityId > 0 ? rep.Where(p =>
                //                        p.FacilityId == facilityId && userIds.Any(u => p.UserId == u) && p.IsActive &&
                //                        p.IsDeleted != true).ToList() : rep.Where(p =>
                //                        p.CorporateId == corporateId && userIds.Any(u => p.UserId == u) && p.IsActive &&
                //                        p.IsDeleted != true).ToList();
                //                }
                //            }
                //            else
                //            {
                //                list = facilityId > 0 ? rep.Where(p =>
                //                    p.FacilityId == facilityId && p.UserId == userid && p.IsActive &&
                //                    p.IsDeleted != true).ToList() : rep.Where(p =>
                //                    p.CorporateId == corporateId && p.UserId == userid && p.IsActive &&
                //                    p.IsDeleted != true).ToList();
                //            }

                //            if (list.Count > 0)
                //                physiciansList.AddRange(list.Select(item => PhysicianMapper.MapModelToViewModel(item)));
                //        }
                //    }

                //    return physiciansList;
                //}

                var physiciansList = new List<PhysicianCustomModel>();
                using (var rep = UnitOfWork.PhysicianRepository)
                {
                    using (var rRep = UnitOfWork.RoleRepository)
                    {
                        var roles = facilityId > 0
                                        ? rRep.Where(
                                            r =>
                                            r.FacilityId == facilityId
                                            && (r.RoleName.Contains("Phy") || r.RoleName.Contains("Nur"))
                                            && !r.IsDeleted && r.IsActive).Select(r => r.RoleID).ToList()
                                        : rRep.Where(
                                            r =>
                                            r.CorporateId == corporateId
                                            && (r.RoleName.Contains("Phy") || r.RoleName.Contains("Nur"))
                                            && !r.IsDeleted && r.IsActive).Select(r => r.RoleID).ToList();

                        if (roles.Count > 0)
                        {
                            List<Physician> list;
                            using (var urRep = UnitOfWork.UserRoleRepository)
                            {
                                var userIds = urRep.Where(r => roles.Contains(r.RoleID)).Select(u => u.UserID).ToList();
                                list = facilityId > 0
                                           ? rep.Where(
                                               p =>
                                               p.FacilityId == facilityId && userIds.Any(u => p.UserId == u)
                                               && p.IsActive && p.IsDeleted != true).ToList()
                                           : rep.Where(
                                               p =>
                                               p.CorporateId == corporateId && userIds.Any(u => p.UserId == u)
                                               && p.IsActive && p.IsDeleted != true).ToList();
                            }

                            if (list.Count > 0)
                            {
                                physiciansList.AddRange(list.Select(item => PhysicianMapper.MapModelToViewModel(item)));
                            }
                        }
                    }
                    return physiciansList;
                }

            }
            catch
            {
                return new List<PhysicianCustomModel>();
            }
        }


        public List<Physician> GetPhysicianByCorporateIdandfacilityId(int corporateId, int facilityId)
        {
            using (var physicianRep = UnitOfWork.PhysicianRepository)
            {
                var list = physicianRep.Where(x => x.CorporateId == corporateId && x.FacilityId == facilityId).ToList();

                return list;
            }
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
                using (var rep = UnitOfWork.PhysicianRepository)
                {
                    using (var rRep = UnitOfWork.RoleRepository)
                    {
                        var roles = facilityId > 0
                                        ? rRep.Where(
                                            r =>
                                            r.FacilityId == facilityId
                                            && (r.RoleName.Contains("Phy") || r.RoleName.Contains("Nur"))
                                            && !r.IsDeleted && r.IsActive).Select(r => r.RoleID).ToList()
                                        : rRep.Where(
                                            r =>
                                            r.CorporateId == corporateId
                                            && (r.RoleName.Contains("Phy") || r.RoleName.Contains("Nur"))
                                            && !r.IsDeleted && r.IsActive).Select(r => r.RoleID).ToList();

                        if (roles.Count > 0)
                        {
                            List<Physician> list;
                            using (var urRep = UnitOfWork.UserRoleRepository)
                            {
                                var userIds = urRep.Where(r => roles.Contains(r.RoleID)).Select(u => u.UserID).ToList();
                                list = facilityId > 0
                                           ? rep.Where(
                                               p =>
                                               p.FacilityId == facilityId && userIds.Any(u => p.UserId == u)
                                               && p.IsActive && p.IsDeleted != true).ToList()
                                           : rep.Where(
                                               p =>
                                               p.CorporateId == corporateId && userIds.Any(u => p.UserId == u)
                                               && p.IsActive && p.IsDeleted != true).ToList();
                            }

                            if (list.Count > 0)
                            {
                                physiciansList.AddRange(list.Select(item => PhysicianMapper.MapModelToViewModel(item)));
                            }
                        }
                    }
                    return physiciansList;
                }
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
                using (var rRep = UnitOfWork.FacilityRoleRepository)
                {
                    /*
                     * Following line should be uncommmented if all resources should be shown in the Scheduler 
                     * having access to scheduling applied true in the facility role screen.
                     * Also, change the repository from role to Facility Role, in the above used.
                     */
                    var roles =
                        rRep.Where(r =>
                            ((facilityId == 0 && r.CorporateId == corporateId) || r.FacilityId == facilityId) &&
                            !r.IsDeleted && r.IsActive && r.SchedulingApplied).Select(r => r.RoleId).ToList();

                    //Get Users based on Roles Names Physicians and Nurses.
                    if (roles.Count > 0)
                    {
                        using (var urRep = UnitOfWork.UserRoleRepository)
                            uList = urRep.Where(r => roles.Contains(r.RoleID)).Select(u => u.UserID).ToList();
                    }
                }
            }

            //Finally, Get Physicians Data based on the above selected parameters.
            using (var rep = UnitOfWork.PhysicianRepository)
            {
                var list =
                    rep.Where(r => ((facilityId == 0 && r.CorporateId == corporateId) || r.FacilityId == facilityId) &&
                                   ((isadmin && uList.Any(u => r.UserId == u)) || (!isadmin && r.UserId == userid))
                                   && r.IsActive && r.IsDeleted != true).ToList();

                //Map Model to ViewModel
                if (list.Any())
                    physiciansList.AddRange(list.Select(item => PhysicianMapper.MapModelToViewModel(item)));

                if (physiciansList.Any())
                    physiciansList = physiciansList.OrderBy(p => p.UserTypeStr).ThenBy(p1 => p1.Physician.PhysicianName).ToList();

                return physiciansList;
            }

        }


        public async Task<List<PhysicianViewModel>> GetFacultyList(int facilityId, long userId = 0)
        {
            using (var rep = UnitOfWork.PhysicianRepository)
            {
                var list = await rep.GetPhysiciansByFacility(facilityId, userId);
                return list;
            }
        }
    }
}