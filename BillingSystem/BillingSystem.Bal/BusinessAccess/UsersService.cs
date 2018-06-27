﻿using BillingSystem.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Common;
using System.Data.Entity;
using BillingSystem.Model.CustomModel;
using System.Transactions;
using BillingSystem.Repository.Interfaces;
using AutoMapper;
using System.Data.SqlClient;
using BillingSystem.Common.Common;
using BillingSystem.Repository.Common;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class UsersService : IUsersService
    {
        private readonly IRepository<Users> _repository;
        private readonly IRepository<Corporate> _cRepository;
        private readonly IRepository<UserRole> _urRepository;
        private readonly IRepository<Physician> _phRepository;
        private readonly IRepository<Facility> _fRepository;
        private readonly IRepository<Role> _rRepository;
        private readonly IMapper _mapper;
        private readonly BillingEntities _context;

        public UsersService(IRepository<Users> repository, IRepository<Corporate> cRepository, IRepository<UserRole> urRepository, IRepository<Physician> phRepository, IRepository<Facility> fRepository, IRepository<Role> rRepository, IMapper mapper, BillingEntities context)
        {
            _repository = repository;
            _cRepository = cRepository;
            _urRepository = urRepository;
            _phRepository = phRepository;
            _fRepository = fRepository;
            _rRepository = rRepository;
            _mapper = mapper;
            _context = context;
        }
        public Physician GetPhysicianById(int id)
        {
            var phys = _phRepository.Where(p => p.Id == id).FirstOrDefault();
            return phys ?? new Physician();
        }

        /// <summary>
        /// Get the user after login
        /// </summary>
        /// <returns>Return the user after login</returns>
        public Users GetUser(string username, string password, string loginType = "username")
        {
            var m = new Users();
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                var pwd = EncryptDecrypt.GetEncryptedData(password, string.Empty);
                pwd = pwd.Trim();
                var uname = username.ToLower().Trim();
                loginType = loginType.ToLower().Trim();


                //Get the User Object from the table Users against the current Username passed as input parameter.
                m = _repository.Where(u => (loginType.Equals("username") ? u.UserName.ToLower().Equals(uname) : u.Email.ToLower().Equals(uname)) && u.Password.Trim().Equals(pwd)).FirstOrDefault();
            }
            return m;
            //Changes end here

        }

        //function to get user by UserID
        /// <summary>
        /// Gets the user by identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public Users GetUserById(int? userId)
        {
            var m = _repository.Where(x => x.UserID == userId && x.IsDeleted == false).FirstOrDefault();
            if (m != null)
            {
                var encryptPassword = EncryptDecrypt.GetDecryptedData(m.Password, "");
                m.Password = encryptPassword;
            }
            return m;

        }
        public UsersViewModel GetUserByEmail(string email)
        {

            var m = _repository.Where(x => x.Email == email && x.IsDeleted == false).FirstOrDefault();
            if (m != null)
            {
                var encryptPassword = EncryptDecrypt.GetDecryptedData(m.Password, "");
                m.Password = encryptPassword;
            }
            else
            {
                m = new Users();
            }
            return _mapper.Map<UsersViewModel>(m);

        }
        public UsersViewModel GetUserByEmailAndToken(string email, string resetToken)
        {
            var m = _repository.Where(x => x.Email == email && x.IsDeleted == false && x.ResetToken == resetToken).FirstOrDefault();
            if (m != null)
            {
                var encryptPassword = EncryptDecrypt.GetDecryptedData(m.Password, "");
                m.Password = encryptPassword;
            }
            else
            {
                m = new Users();
            }
            return _mapper.Map<UsersViewModel>(m);

        }
        public UsersViewModel GetUserByEmailWithoutDecryption(string email)
        {
            var m = _repository.Where(x => x.Email == email && x.IsDeleted == false).FirstOrDefault() ?? new Users();
            return _mapper.Map<UsersViewModel>(m);
        }

        /// <summary>
        /// Checks the exists password.
        /// </summary> Update By Krishna
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public bool CheckExistsPassword(string newPassword, int userid)
        {
            var pass = EncryptDecrypt.GetEncryptedData(newPassword, "").Trim();

            var isExists = _repository.Where(x => x.UserID == userid && x.Password.Equals(pass)).Any();
            return isExists;
        }

        /// <summary>
        /// Gets the users by corporate identifier facility identifier.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<UsersCustomModel> GetUsersByCorporateIdFacilityId(int corporateId, int facilityId)
        {
            var list = new List<UsersCustomModel>();
            var lstUser = corporateId > 0
                  ? (facilityId == 0 ? _repository.Where(x => x.IsDeleted == false)
                      .Include(u => u.UserRole.Select(ur => ur.Role).Select(r => r.FacilityRole))
                      .Where(
                          ur =>
                              ur.UserRole.Any(
                                  r =>
                                      r.Role.CorporateId != null && r.Role.CorporateId == corporateId))
                                      .ToList() : _repository.Where(x => x.IsDeleted == false)
                      .Include(u => u.UserRole.Select(ur => ur.Role).Select(r => r.FacilityRole))
                      .Where(
                          ur =>
                              ur.UserRole.Any(
                                  r =>
                                      r.Role.CorporateId != null && r.Role.CorporateId == corporateId &&
                                      r.Role.FacilityId == facilityId))
                      .ToList())
                  : _repository.Where(x => x.IsDeleted == false)
                      .Include(u => u.UserRole.Select(r => r.Role).Select(r => r.FacilityRole))
                      .ToList();

            if (lstUser.Count > 0)
            {
                list.AddRange(lstUser.Select(item => new UsersCustomModel
                {
                    CurrentUser = item,
                    PhoneNumber = string.IsNullOrEmpty(item.Phone) || item.Phone.Contains("null") ? "NA" : item.Phone,
                    HomePhoneNumber = string.IsNullOrEmpty(item.HomePhone) || item.HomePhone.Contains("null") ? "NA" : item.HomePhone,
                    RoleName = UserRoles(item.UserRole.ToList()),
                    FacilityNames = UserFacilities(item.UserRole.ToList()),
                    Name = string.Format("{0} {1}", item.FirstName, item.LastName),
                    CorporateName = GetCoprateName(item.CorporateId.ToString())
                }));

                list = list.OrderBy(f => f.Name).ToList();
            }
            return list;

        }


        /// <summary>
        /// Method to add the User in the database.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public int AddUpdateUser(Users m, int roleId)
        {
            int result;
            var userRolebal = new UserRoleBal();
            var facilityRoleBal = new FacilityRoleBal();
            using (var transScope = new TransactionScope())
            {
                var encryptPassword = EncryptDecrypt.GetEncryptedData(m.Password, "");
                m.Password = encryptPassword;
                if (m.UserID > 0)
                {
                    var currentUser = _repository.Where(u => u.UserID == m.UserID).FirstOrDefault();
                    if (currentUser != null)
                    {
                        m.CreatedBy = currentUser.CreatedBy;
                        m.CreatedDate = currentUser.CreatedDate;
                    }

                    _repository.UpdateEntity(m, m.UserID);

                    var name = $"{m.FirstName} - {m.LastName}";
                    var clinician = _phRepository.Where(p => p.UserId == currentUser.UserID).FirstOrDefault();

                    //Add missing updates in roles
                    if (roleId > 0)
                    {
                        var currentRole = _urRepository.Where(r => r.UserID == m.UserID).FirstOrDefault();
                        if (currentRole != null && currentRole.RoleID != roleId)
                        {
                            currentRole.RoleID = roleId;
                            currentRole.ModifiedBy = m.ModifiedBy;
                            currentRole.ModifiedDate = m.ModifiedDate;
                            _urRepository.UpdateEntity(currentRole, currentRole.UserRoleID);

                            //Update the roles in Physician Table too if schedulingApplied is set true to that current role in FacilityRole Table.
                            var isSchedulingApplied = facilityRoleBal.IsSchedulingApplied(roleId);
                            if (isSchedulingApplied && clinician != null)
                            {
                                clinician.UserType = roleId;
                                clinician.ModifiedBy = m.ModifiedBy;
                                clinician.ModifiedDate = m.ModifiedDate;
                            }

                        }
                    }

                    if (clinician != null)
                    {
                        clinician.PhysicianName = name;
                        _phRepository.UpdateEntity(clinician, clinician.Id);
                    }

                    if (Convert.ToBoolean(m.IsDeleted))
                        userRolebal.DeleteRoleWithUser(m.UserID);
                    transScope.Complete();
                }
                else
                {
                    _repository.Create(m);

                    //Check if User Role is added
                    if (m.UserID > 0 && roleId > 0)
                    {
                        var userRoleModel = new UserRole
                        {
                            UserID = m.UserID,
                            RoleID = roleId,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedBy = m.CreatedBy,
                            CreatedDate = m.CreatedDate,
                            UserRoleID = 0
                        };
                        var newUserRoleId = userRolebal.SaveUserRole(userRoleModel);
                        if (newUserRoleId > 0)
                        {
                            var facilityRoleModel = new FacilityRole
                            {
                                RoleId = roleId,
                                FacilityRoleId = 0,
                                FacilityId = Convert.ToInt32(m.FacilityId),
                                IsActive = true,
                                IsDeleted = false,
                                CorporateId = Convert.ToInt32(m.CorporateId),
                                CreatedBy = m.CreatedBy,
                                CreatedDate = m.CreatedDate
                            };
                            var isAdded = facilityRoleBal.SaveFacilityRoleIfNotExists(facilityRoleModel);
                            if (isAdded)
                                transScope.Complete();
                        }
                    }


                }


                result = m.UserID;
            }

            return result;
        }

        /// <summary>
        /// Method to To check Duplicate User on the basis of username or email
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool CheckDuplicateUser(string username, string email, int userId)
        {
            username = username.ToLower();
            var user = userId > 0
                ? _repository.Where(x => x.UserID != userId && x.UserName.ToLower() == username && x.IsDeleted == false).FirstOrDefault()
                : _repository.Where(x => x.UserName.ToLower() == username && x.IsDeleted == false).FirstOrDefault();
            return user != null;

        }

        //Menu Manipulations
        /// <summary>
        /// Gets the name of the tabs by user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        public IEnumerable<Tabs> GetTabsByUserName(string userName)
        {
            var user = _repository.Where(u => u.UserName.Equals(userName) && u.IsDeleted != true)
                .Include(a => a.UserRole.Select(u => u.Role).Select(r => r.RoleTabs.Select(t => t.Tabs))).FirstOrDefault();

            if (user == null) return null;
            var tabs = user.UserRole.Select(a => a.Role).ToList().FirstOrDefault();
            if (tabs == null) return null;
            var menus = tabs.RoleTabs.Select(a => a.Tabs).Where(t => !t.IsDeleted && t.IsActive).OrderBy(a => a.TabOrder);
            return menus.ToList();

        }

        // Changed by Shashank ON : 5th May 2015 : To add the Module access level Security when user log in via Facility and Corporate 
        /// <summary>
        /// Gets the tabs by user identifier role identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="roleId">The role identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateid">The corporateid.</param>
        /// <returns></returns>
        public IEnumerable<Tabs> GetTabsByUserIdRoleId(int userId, int roleId, int facilityId, int corporateid, bool isDeleted = false, bool isActive = true)
        {
            var sqlParameters = new SqlParameter[6];
            sqlParameters[0] = new SqlParameter("RId", roleId);
            sqlParameters[1] = new SqlParameter("UId", userId);
            sqlParameters[2] = new SqlParameter("FId", facilityId);
            sqlParameters[3] = new SqlParameter("CId", corporateid);
            sqlParameters[4] = new SqlParameter("IsDeleted", isDeleted);
            sqlParameters[5] = new SqlParameter("IsActive", isActive);

            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetTabsList.ToString(), parameters: sqlParameters, isCompiled: false))
            {
                var list = r.ResultSetFor<Tabs>().ToList();
                return list;
            }
        }

        /// <summary>
        /// Gets the user details.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public CommonModel GetUserDetails(int roleId, int facilityId, int userId)
        {
            var commonModel = new CommonModel();

            var facility = _fRepository.Where(f => f.FacilityId == facilityId).FirstOrDefault();
            if (facility != null)
            {
                commonModel.DefaultFacility = facility.FacilityName;
                commonModel.FacilityNumber = facility.FacilityNumber;
                commonModel.FacilityId = facility.FacilityId;
            }

            var current = _rRepository.Where(r => r.RoleID == roleId).FirstOrDefault();
            if (current != null)
            {
                commonModel.RoleName = current.RoleName;
                commonModel.RoleKey = current.RoleKey;
            }
            var firstOrDefault = _repository.Where(r => r.UserID == userId).FirstOrDefault();
            if (firstOrDefault != null)
                commonModel.UserName = firstOrDefault.UserName;
            if (firstOrDefault != null)
                commonModel.UserIsAdmin = Convert.ToBoolean(firstOrDefault.AdminUser);



            return commonModel;
        }

        /// <summary>
        /// Users the roles.
        /// </summary>
        /// <param name="roles">The roles.</param>
        /// <returns></returns>
        public string UserRoles(IEnumerable<UserRole> roles)
        {
            var roleNames = string.Empty;
            foreach (var item in roles)
            {
                if (string.IsNullOrEmpty(roleNames))
                    roleNames = item.Role.RoleName;
                else
                    roleNames += string.Format(", {0}", item.Role.RoleName);
            }
            return roleNames;
        }

        /// <summary>
        /// Gets the name of the coprate.
        /// </summary> Updated By Krishna
        /// <param name="corporate">The corporate.</param>
        /// <returns></returns>
        public string GetCoprateName(string corporateId)
        {
            var corporateName = "";
            int cId = Convert.ToInt32(corporateId);
            var lstCorporate = cId > 0 ? _cRepository.Where(a => a.IsDeleted != true && a.CorporateID == cId).FirstOrDefault() : _cRepository.Where(a => a.IsDeleted != true).FirstOrDefault();

            if (lstCorporate != null)
                corporateName = lstCorporate.CorporateName;

            return corporateName;
        }

        /// <summary>
        /// Users the facilities.
        /// </summary>
        /// <param name="userRoles">The user roles.</param>
        /// <returns></returns>
        public string UserFacilities(IEnumerable<UserRole> userRoles)
        {
            var facilityNames = string.Empty;
            var ids = new List<int>();
            foreach (var item in userRoles)
            {
                if (item.Role != null)
                {
                    foreach (var f in item.Role.FacilityRole.ToList())
                    {
                        if (ids.All(fac => fac != f.FacilityId))
                        {
                            ids.Add(f.FacilityId);
                            using (var facBal = new FacilityBal())
                            {
                                if (string.IsNullOrEmpty(facilityNames))
                                    facilityNames = facBal.GetFacilityNameById(f.FacilityId);
                                else
                                    facilityNames += string.Format(", {0}", facBal.GetFacilityNameById(f.FacilityId));
                            }
                        }
                    }
                }
            }
            return facilityNames;
        }

        /// <summary>
        /// Gets all users by facility identifier.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<UsersCustomModel> GetAllUsersByFacilityId(int facilityId)
        {
            var lstUser = new List<UsersCustomModel>();

            var list = _repository.Where(a => a.FacilityId == facilityId && a.IsDeleted != true).ToList();

            if (list.Count > 0)
            {
                lstUser.AddRange(list.Select(item => new UsersCustomModel
                {
                    CurrentUser = item,
                    Name = string.Format("{0} {1}", item.FirstName, item.LastName),
                }));
            }
            return lstUser;

        }

        public List<Users> GetFacilityUsers(int facilityId)
        {
            var list = _repository.Where(u => u.FacilityId == facilityId && u.IsActive && u.IsDeleted == false).ToList();
            return list;

        }

        public List<Users> GetNonAdminUsersByCorporate(int corporateId)
        {
            var list = _repository.Where(u => u.CorporateId == corporateId && u.IsActive && u.IsDeleted == false && u.AdminUser == false).ToList();
            return list;

        }

        public int UpdateUser(Users user)
        {
            _repository.UpdateEntity(user, user.UserID);

            return user.UserID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="corporateId"></param>
        /// <returns></returns>
        public bool CheckUserExistForCorporate(int corporateId)
        {
            var user = _repository.Where(fr => fr.IsActive && fr.IsDeleted != true && fr.CorporateId == corporateId).FirstOrDefault();
            return user != null;
        }

        public bool CheckForDuplicateEmail(int userId, string email)
        {
            email = email.ToLower().Trim();
            var isExist = userId > 0
                ? _repository.Where(x => x.UserID != userId && x.Email.ToLower().Trim().Equals(email)).Any()
                : _repository.Where(x => x.Email.ToLower().Trim().Equals(email)).Any();
            return isExist;

        }


        public string GetUserEmailByUserId(int userid)
        {
            var userEmail = _repository.Where(x => x.UserID == userid).FirstOrDefault();
            return userEmail != null ? userEmail.Email : string.Empty;

        }

        public List<Users> GetUsersByCorporateandFacilityId(int corporateId, int facilityId)
        {
            var list = corporateId > 0 ? _repository.Where(u => u.FacilityId == facilityId && u.IsActive && u.IsDeleted == false && u.CorporateId == corporateId).ToList() :
                _repository.Where(u => u.IsActive && u.IsDeleted == false).ToList();
            return list;

        }

        /// <summary>
        /// Gets the name of the userby user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
        public Users GetUserbyUserName(string username)
        {
            var usersModel = new Users();
            if (!string.IsNullOrEmpty(username))
            {
                var uname = username.ToLower().Trim();
                usersModel = _repository.Where(u => u.UserName.ToLower().Equals(uname)).FirstOrDefault();
            }
            return usersModel;

        }

        public List<UsersViewModel> GetUsersByRole(int facilityId, int cId)
        {
            var spName = string.Format("EXEC {0} @CId, @FId", StoredProcedures.SPROC_GetUsersRoleWise);
            var sqlParameters = new object[2];
            sqlParameters[0] = new SqlParameter("CId", cId);
            sqlParameters[1] = new SqlParameter("FId", facilityId);
            IEnumerable<UsersViewModel> result = _context.Database.SqlQuery<UsersViewModel>(spName, sqlParameters);
            return result.ToList();
        }

        public List<PhysicianViewModel> GetPhysiciansByRole(int facilityId, int cId)
        {
            var spName = string.Format("EXEC {0} @CId, @FId", StoredProcedures.SPROC_GetPhysicianRoleWise);
            var sqlParameters = new object[2];
            sqlParameters[0] = new SqlParameter("CId", cId);
            sqlParameters[1] = new SqlParameter("FId", facilityId);
            IEnumerable<PhysicianViewModel> result = _context.Database.SqlQuery<PhysicianViewModel>(spName, sqlParameters);
            return result.ToList();
        }
        public List<BillEditorUsersCustomModel> GetBillEditorUsers(int corporateId, int facilityId)
        {
            var spName = string.Format("EXEC {0} @pCID, @pFID", StoredProcedures.SPROC_GetBillEditRoleUser);
            var sqlParameters = new object[2];
            sqlParameters[0] = new SqlParameter("pCID", corporateId);
            sqlParameters[1] = new SqlParameter("pFID", facilityId);
            IEnumerable<BillEditorUsersCustomModel> result = _context.Database.SqlQuery<BillEditorUsersCustomModel>(spName, sqlParameters);
            return result.ToList();
        }
    }
}
