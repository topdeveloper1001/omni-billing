using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using BillingSystem.Model.CustomModel;
using BillingSystem.Models;
using BillingSystem.Common.Common;
using BillingSystem.Common;
using BillingSystem.Model;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    /// <summary>
    /// Security Controller 
    /// </summary>
    public class SecurityController : BaseController
    {
        private readonly IAuditLogService _adService;
        private readonly IUsersService _uService;
        private readonly IUserRoleService _urService;
        private readonly IRoleService _rService;
        private readonly IScreenService _sService;
        private readonly ITabsService _tService;
        private readonly IRolePermissionService _rpService;
        private readonly IFacilityRoleService _frService;
        private readonly IRoleTabsService _rtService;
        private readonly IFacilityService _fService;
        private readonly IModuleAccessService _maService;

        public SecurityController(IAuditLogService adService, IUsersService uService, IUserRoleService urService, IRoleService rService, IScreenService sService, ITabsService tService, IRolePermissionService rpService, IFacilityRoleService frService, IRoleTabsService rtService, IFacilityService fService, IModuleAccessService maService)
        {
            _adService = adService;
            _uService = uService;
            _urService = urService;
            _rService = rService;
            _sService = sService;
            _tService = tService;
            _rpService = rpService;
            _frService = frService;
            _rtService = rtService;
            _fService = fService;
            _maService = maService;
        }



        //
        // GET: /Security/

        #region Users Section

        //Function to get User Screen
        /// <summary>
        /// Users this instance.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult User(int? cId, int? fId)
        {
            var corporateId = Convert.ToInt32(cId) > 0 ? Convert.ToInt32(cId) : Helpers.GetSysAdminCorporateID();
            var facilityId = Convert.ToInt32(fId) > 0 ? Convert.ToInt32(fId) : Helpers.GetDefaultFacilityId();

            if (fId == 0 && !Helpers.GetLoggedInUserIsAdmin())
                facilityId = Helpers.GetDefaultFacilityId();

            var objUsersView = new UsersView
            {
                CurrentUser =
                    new UsersCustomModel
                    {
                        CurrentUser = new Users { IsActive = true, CorporateId = corporateId, FacilityId = facilityId }
                    },
                UsersList = _uService.GetUsersByCorporateIdFacilityId(corporateId, facilityId),
            };
            return View(objUsersView);
        }




        /// <summary>
        /// Gets the name of the role.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <returns></returns>
        private string GetRoleName(int userID)
        {
            string RoleName = string.Empty;
            var userRoles = _urService.GetUserRolesByUserId(userID);
            if (userRoles.Count > 0)
            {
                var firstUserRole = userRoles.FirstOrDefault();
                RoleName = _rService.GetRoleById(firstUserRole.RoleID).RoleName;

            }
            return RoleName;
        }

        /// <summary>
        /// Gets the role identifier.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <returns></returns>
        private int GetRoleId(int userID)
        {
            int RoleId = 0;
            var userRoles = _urService.GetUserRolesByUserId(userID);
            if (userRoles.Count > 0)
            {
                var firstUserRole = userRoles.FirstOrDefault();
                RoleId = _rService.GetRoleById(firstUserRole.RoleID).RoleID;

            }
            return RoleId;
        }

        /// <summary>
        /// Gets the roles users.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetRolesUsers()
        {
            var corporateId = Helpers.GetDefaultCorporateId();
            var facilityId = Helpers.GetDefaultFacilityId();
            var rolesList = _rService.GetAllRolesByCorporateFacility(corporateId, facilityId);
            return Json(rolesList);
        }

        //Function to get  User for editing
        /// <summary>
        /// Edits the user.
        /// </summary>
        /// <param name="UserID">The user identifier.</param>
        /// <returns></returns>
        public ActionResult EditUser(int UserID)
        {
            var user = _uService.GetUserById(UserID);
            var currentUser = new UsersCustomModel
            {
                CurrentUser = user,
                //RoleId = user.UserRole
                //UserID = user.UserID,
                //CountryID = user.CountryID,
                //StateID = user.StateID,
                //CityID = user.CityID,
                //UserGroup = user.UserGroup,
                //UserName = user.UserName,
                //FirstName = user.FirstName,
                //LastName = user.LastName,
                //Answer = user.Answer,
                //Password = user.Password,
                //Address = user.Address,
                //Email = user.Email,
                //Phone = user.Phone,
                //HomePhone = user.HomePhone,
                //AdminUser = user.AdminUser,
                //IsActive = user.IsActive,
                //FailedLoginAttempts = user.FailedLoginAttempts,
                //IsDeleted = user.IsDeleted,
                RoleName = GetRoleName(user.UserID),
                RoleId = GetRoleId(user.UserID)

            };
            return Json(currentUser, JsonRequestBehavior.AllowGet);
            //return PartialView(PartialViews.AddUpdateUser, currentUser);
        }

        /// <summary>
        /// Adds the user.
        /// </summary>
        /// <param name="objUser">The object user.</param>
        /// <param name="roleId"></param>
        /// <param name="cId"></param>
        /// <param name="fId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddUser(Users objUser, int roleId, int cId, int fId)
        {
            if (fId == 0 && !Helpers.GetLoggedInUserIsAdmin())
                fId = Helpers.GetDefaultFacilityId();

            cId = cId == 0 ? Helpers.GetSysAdminCorporateID() : cId;

            var userId = Helpers.GetLoggedInUserId();
            var currentDateTime = Helpers.GetInvariantCultureDateTime();

            if (objUser.UserID > 0)
            {
                objUser.ModifiedBy = userId;
                objUser.ModifiedDate = currentDateTime;
            }
            else
            {
                objUser.CreatedBy = userId;
                objUser.CreatedDate = currentDateTime;
            }

            _uService.AddUpdateUser(objUser, roleId);

            //start

            var auditlogObj = new AuditLog
            {
                AuditLogID = 0,
                UserId = userId,
                CreatedDate = Helpers.GetInvariantCultureDateTime(),
                TableName = "Users",
                FieldName = "Password",
                PrimaryKey = 0,
                OldValue = string.Empty,
                NewValue = string.Empty,
                CorporateId = cId,
                FacilityId = fId
            };
            _adService.AddUptdateAuditLog(auditlogObj);

            var list = _uService.GetUsersByCorporateIdFacilityId(cId, fId);
            return PartialView(PartialViews.UsersList, list);
        }


        //public ActionResult AddRoleWithUser(int userID, int roleID)
        //{
        //    var _urService = new UserRoleBal();
        //    var isExist = _urService.CheckIfExists(userID, roleID);
        //    if (!isExist)
        //    {
        //        var lstUserRoles = new List<UserRole>
        //        {
        //            new UserRole
        //            {
        //                UserID = userID,
        //                RoleID = roleID,
        //                IsActive = true,
        //                IsDeleted = false,
        //                CreatedBy = Helpers.GetLoggedInUserId(),
        //                CreatedDate = Helpers.GetInvariantCultureDateTime()
        //            }
        //        };
        //        return Json(_urService.AddUpdateUserRole(lstUserRoles));
        //    }
        //    return Json(0);
        //}
        /// <summary>
        /// Adds the role with user.
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// Deletes the user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cId"></param>
        /// <param name="fId"></param>
        /// <returns></returns>
        public ActionResult DeleteUser(int userId, int cId, int fId)
        {
            var objUser = _uService.GetUserById(userId);
            objUser.IsDeleted = true;
            objUser.DeletedBy = Helpers.GetLoggedInUserId();
            objUser.DeletedDate = Helpers.GetInvariantCultureDateTime(); //To Do change it to server datetime
            objUser.TokenExpiryDate = null;
            objUser.UserToken = null;
            _uService.AddUpdateUser(objUser, 0);

            cId = cId == 0 ? Helpers.GetSysAdminCorporateID() : cId;

            if (fId == 0 && !Helpers.GetLoggedInUserIsAdmin())
                fId = Helpers.GetDefaultFacilityId();

            var list = _uService.GetUsersByCorporateIdFacilityId(cId, fId);
            return PartialView("UserControls/_UsersList", list);
        }

        //Function To reset the User Form
        /// <summary>
        /// Resets the user form.
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetUserForm()
        {
            var CurrentUser = new UsersCustomModel { CurrentUser = new Users { IsActive = true } };
            return PartialView(PartialViews.AddUpdateUser, CurrentUser);
        }

        // Function to chek duplicate user on the basis of username or Email
        /// <summary>
        /// Checks the duplicate user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="email">The email.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public JsonResult CheckDuplicateUser(string username, string email, int userId)
        {
            var isExist = _uService.CheckForDuplicateEmail(userId, email);
            if (isExist)
            {
                return Json("-1");
            }

            return Json(_uService.CheckDuplicateUser(username, email, userId));
        }

        /// <summary>
        /// Gets the logged in user details.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetLoggedInUserDetails()
        {
            var userid = Helpers.GetLoggedInUserId();
            var user = _uService.GetUserById(userid);
            var viewpath = string.Format("../PatientSearch/{0}", PartialViews.ChangePassword);
            return PartialView(viewpath, user);
        }

        public PartialViewResult RebindUsersList(int cId, int fId)
        {
            if (fId == 0 && !Helpers.GetLoggedInUserIsAdmin())
                fId = Helpers.GetDefaultFacilityId();

            var list = _uService.GetUsersByCorporateIdFacilityId(cId, fId);
            return PartialView("UserControls/_UsersList", list);

        }

        #endregion

        #region "UserRole"

        /// <summary>
        /// Roles this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Role()
        {
            var corporateId = Helpers.GetDefaultCorporateId();
            RoleView objRoleView = new RoleView
            {
                CurrentRole = new Role(),
                RolesList = _rService.GetAllRoles(corporateId)
            };

            //ScreenBal _sService = new ScreenBal();
            //ScreenView screenview = new ScreenView
            //{
            //    AvailableScreens = _sService.GetAllScreensList()
            //};
            //objRoleView.screenView = screenview;
            return View(objRoleView);
        }

        /// <summary>
        /// Gets the roles.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetRoles()
        {
            var corporateId = Helpers.GetDefaultCorporateId();
            List<Role> RolesList = _rService.GetAllRoles(corporateId);
            return PartialView(PartialViews.RoleList, RolesList);
        }

        /// <summary>
        /// Edits the role.
        /// </summary>
        /// <param name="RoleID">The role identifier.</param>
        /// <returns></returns>
        public ActionResult EditRole(int RoleID)
        {
            Role CurrentRole = _rService.GetRoleById(RoleID);
            return PartialView(PartialViews.AddUpdateRole, CurrentRole);
        }

        /// <summary>
        /// Gets the add update role.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAddUpdateRole()
        {
            Role CurrentRole = new Role();
            return PartialView(PartialViews.AddUpdateRole, CurrentRole);
        }

        /// <summary>
        /// Adds the role.
        /// </summary>
        /// <param name="objRole">The object role.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddRole(Role objRole)
        {
            if (objRole.RoleID > 0)
            {
                objRole.ModifiedBy = Helpers.GetLoggedInUserId();
                objRole.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                objRole.CreatedBy = Helpers.GetLoggedInUserId();
                objRole.CreatedDate = Helpers.GetInvariantCultureDateTime();
            }
            else
            {
                objRole.CreatedBy = Helpers.GetLoggedInUserId();
                objRole.CreatedDate = Helpers.GetInvariantCultureDateTime();
            }
            var i = _rService.AddUpdateRole(objRole);
            return Json(i);
            // List<Role> RolesList = _rService.GetAllRoles();
            // return PartialView(PartialViews.RoleList, RolesList);

        }

        /// <summary>
        /// Deletes the role.
        /// </summary>
        /// <param name="RoleId">The role identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteRole(int RoleId)
        {
            Role objRole = _rService.GetRoleById(RoleId);
            objRole.IsDeleted = true;
            objRole.DeletedBy = Helpers.GetLoggedInUserId();
            objRole.DeletedDate = Helpers.GetInvariantCultureDateTime(); //To Do change it to server datetime
            var i = _rService.AddUpdateRole(objRole);
            var corporateId = Helpers.GetDefaultCorporateId();
            List<Role> RolesList = _rService.GetAllRoles(corporateId);
            return PartialView(PartialViews.RoleList, RolesList);

        }

        // Function to chek duplicate Role on the basis of RoleName 
        /// <summary>
        /// Checks the duplicate role.
        /// </summary>
        /// <param name="RoleId">The role identifier.</param>
        /// <param name="RoleName">Name of the role.</param>
        /// <returns></returns>
        public bool CheckDuplicateRole(int RoleId, string RoleName)
        {
            return _rService.CheckDuplicateRole(RoleId, RoleName);
        }

        // Function to chek role exist on the 
        /// <summary>
        /// Checks the role exist.
        /// </summary>
        /// <param name="Id">The identifier.</param>
        /// <returns></returns>
        public ActionResult CheckRoleExist(int Id)
        {
            var result = _urService.CheckRoleExist(Id);
            return Json(result);
        }

        #endregion

        #region "Screen"

        /// <summary>
        /// Screens this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Screen()
        {
            ScreenView objScreenView = new ScreenView
            {
                CurrentScreen = new Screen(),
                TabsList = _tService.GetAllTabs(),
                ScreensList = _sService.GetAllScreensList()
            };
            return View(objScreenView);
        }

        /// <summary>
        /// Gets the screens.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetScreens()
        {
            List<Screen> ScreensList = _sService.GetAllScreensList();
            return PartialView(PartialViews.ScreenList, ScreensList);
        }

        /// <summary>
        /// Edits the screen.
        /// </summary>
        /// <param name="ScreenID">The screen identifier.</param>
        /// <returns></returns>
        public ActionResult EditScreen(int ScreenID)
        {
            ScreenView objScreenView = new ScreenView
            {
                CurrentScreen = _sService.GetScreenDetailById(ScreenID),
                TabsList = _tService.GetAllTabs()
            };
            return PartialView(PartialViews.AddUpdateScreen, objScreenView);
        }

        /// <summary>
        /// Gets the add update screen.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAddUpdateScreen()
        {
            ScreenView objScreenView = new ScreenView { CurrentScreen = new Screen(), TabsList = _tService.GetAllTabs() };
            return PartialView(PartialViews.AddUpdateScreen, objScreenView);
        }

        /// <summary>
        /// Adds the screen.
        /// </summary>
        /// <param name="objScreen">The object screen.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddScreen(Screen objScreen)
        {
            if (objScreen.ScreenId > 0)
            {
                objScreen.ModifiedBy = Helpers.GetLoggedInUserId();
                objScreen.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                objScreen.CreatedBy = Helpers.GetLoggedInUserId();
                objScreen.CreatedDate = Helpers.GetInvariantCultureDateTime();
            }
            else
            {
                objScreen.CreatedBy = Helpers.GetLoggedInUserId();
                objScreen.CreatedDate = Helpers.GetInvariantCultureDateTime();
            }
            var i = _sService.AddUpdateScreen(objScreen);
            List<Screen> ScreensList = _sService.GetAllScreensList();
            return PartialView(PartialViews.ScreenList, ScreensList);

        }

        /// <summary>
        /// Deletes the screen.
        /// </summary>
        /// <param name="screenID">The screen identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteScreen(int screenID)
        {
            Screen objScreen = _sService.GetScreenDetailById(screenID);
            objScreen.IsDeleted = true;
            objScreen.DeletedBy = Helpers.GetLoggedInUserId();
            objScreen.DeletedDate = Helpers.GetInvariantCultureDateTime(); //To Do change it to server datetime
            var i = _sService.AddUpdateScreen(objScreen);
            List<Screen> ScreensList = _sService.GetAllScreensList();
            return PartialView(PartialViews.ScreenList, ScreensList);

        }

        #endregion

        #region "Tabs"

        /// <summary>
        /// Tabs this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Tab()
        {
            TabView objTabView = new TabView
            {
                CurrentTab = new Tabs(),
                TabsList = _tService.GetAllTabs(),
                ScreenList = _sService.GetAllScreensList()
            };
            return View(objTabView);
        }

        //Function to get All tabs
        /// <summary>
        /// Gets the tabs.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetTabs()
        {
            List<Tabs> tabsList = _tService.GetAllTabs();
            return PartialView(PartialViews.TabsList, tabsList);
        }

        /// <summary>
        /// Edits the tab.
        /// </summary>
        /// <param name="TabID">The tab identifier.</param>
        /// <returns></returns>
        public ActionResult EditTab(int TabID)
        {
            TabView objTabView = new TabView
            {
                CurrentTab = _tService.GetTabByTabId(TabID),
                TabsList = _tService.GetAllTabs(),
                ScreenList = _sService.GetAllScreensList()
            };

            return PartialView(PartialViews.AddUpdateTabs, objTabView);
        }

        /// <summary>
        /// Gets the add update tab.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAddUpdateTab()
        {
            TabView objTabView = new TabView
            {
                CurrentTab = new Tabs(),
                TabsList = _tService.GetAllTabs(),
                ScreenList = _sService.GetAllScreensList()
            };
            return PartialView(PartialViews.AddUpdateTabs, objTabView);
        }

        /// <summary>
        /// Adds the tab.
        /// </summary>
        /// <param name="objTab">The object tab.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddTab(Tabs objTab)
        {
            if (objTab.TabId > 0)
            {
                objTab.ModifiedBy = Helpers.GetLoggedInUserId();
                objTab.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                objTab.CreatedBy = Helpers.GetLoggedInUserId();
                objTab.CreatedDate = Helpers.GetInvariantCultureDateTime();
            }
            else
            {
                objTab.CreatedBy = Helpers.GetLoggedInUserId();
                objTab.CreatedDate = Helpers.GetInvariantCultureDateTime();

            }
            var i = _tService.AddUpdateTab(objTab);
            List<Tabs> tabsList = _tService.GetAllTabs();
            return PartialView(PartialViews.TabsList, tabsList);
        }

        /// <summary>
        /// Deletes the tab.
        /// </summary>
        /// <param name="TabID">The tab identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteTab(int TabID)
        {
            Tabs objTab = _tService.GetTabByTabId(TabID);
            objTab.IsDeleted = true;
            objTab.DeletedBy = Helpers.GetLoggedInUserId();
            objTab.DeletedDate = Helpers.GetInvariantCultureDateTime(); //To Do change it to server datetime
            var i = _tService.AddUpdateTab(objTab);
            List<Tabs> tabsList = _tService.GetAllTabs();
            return PartialView(PartialViews.TabsList, tabsList);
        }

        #endregion

        #region Role Permission Section

        /// <summary>
        /// Adds the role permission.
        /// </summary>
        /// <param name="objRolePermissionList">The object role permission list.</param>
        /// <returns></returns>
        public ActionResult AddRolePermission(List<RolePermission> objRolePermissionList)
        {
            try
            {
                var corporateId = Helpers.GetDefaultCorporateId();
                List<RolePermission> objListRolePermission = new List<RolePermission>();
                foreach (var item in objRolePermissionList)
                {
                    RolePermission objRolePermission = new RolePermission();
                    objRolePermission.RoleID = Convert.ToInt32(item.RoleID);
                    objRolePermission.PermissionID = Convert.ToInt32(item.PermissionID);
                    objRolePermission.PermissionTypeID = Convert.ToInt32(PermissionTypes.Screen);
                    objRolePermission.CreatedBy = Helpers.GetLoggedInUserId();
                    objRolePermission.CreatedDate = Helpers.GetInvariantCultureDateTime();
                    //TODo there is must be server date time
                    objRolePermission.IsActive = true;
                    objListRolePermission.Add(objRolePermission);

                }
                _rpService.AddUpdateRolePermission(objListRolePermission);

                RoleView objRoleView = new RoleView();

                ScreenView screenview = new ScreenView
                {
                    AvailableScreens = _sService.GetAllScreensList()
                };
                objRoleView.RolesList = _rService.GetAllRoles(corporateId);
                objRoleView.screenView = screenview;
                return PartialView(PartialViews.PartialScreensPermission, objRoleView);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the permisssions by role identifier.
        /// </summary>
        /// <param name="RoleId">The role identifier.</param>
        /// <returns></returns>
        public ActionResult GetPermisssionsByRoleID(int RoleId)
        {
            RoleView objRoleView = new RoleView();
            List<RolePermissionInfo> objlstRolePermissionInfo = new List<RolePermissionInfo>();
            List<RolePermission> SelectedScreens = _rpService.GetRolePermissionByRoleId(RoleId);
            objlstRolePermissionInfo = (from y in SelectedScreens
                                        select new RolePermissionInfo { RolePermissionID = y.RolePermissionID, PermissionID = y.PermissionID })
                .ToList();
            return Json(objlstRolePermissionInfo);
        }

        #endregion

        #region User Role Section

        /// <summary>
        /// Users the role.
        /// </summary>
        /// <returns></returns>
        public ActionResult UserRole()
        {
            var objUserRoleView = new UserRoleView
            {
                RolesList = new List<Role>(), //_rService.GetAllRolesByCorporateFacility(corporateId, facilityId),
                //UsersList = new List<Users>(), //objUsersBal.GetAllUsersByCorporateIdFacilityId(corporateId, facilityId),
                UserID = 0
            };
            return View(objUserRoleView);
        }

        /// <summary>
        /// Adds the user role.
        /// </summary>
        /// <param name="objUserRoleList">The object user role list.</param>
        /// <returns></returns>
        public ActionResult AddUserRole(List<UserRole> objUserRoleList)
        {
            try
            {
                var objListUserRole = objUserRoleList.Select(item => new UserRole
                {
                    RoleID = Convert.ToInt32(item.RoleID),
                    UserID = Convert.ToInt32(item.UserID),
                    CreatedBy = Helpers.GetLoggedInUserId(),
                    CreatedDate = Helpers.GetInvariantCultureDateTime(),
                    IsActive = true
                }).ToList();
                var updatedId = _urService.AddUpdateUserRole(objListUserRole);
                return Json(updatedId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the user roles by user identifier.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="corporateId"></param>
        /// <param name="facilityId"></param>
        /// <returns></returns>
        public ActionResult GetUserRolesByUserId(int userId, int corporateId, int facilityId)
        {
            var objlstUserRole = new List<UserRole>();
            //objlstUserRole = _urService.GetUserRolesByUserID(UserID);
            objlstUserRole = _urService.GetUserRolesByCorporateFacilityAndUserId(userId, corporateId, facilityId);
            return Json(objlstUserRole);
        }

        /// <summary>
        /// Gets all roles by corporate and facility.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public ActionResult GetAllRolesByCorporateAndFacility(int corporateId, int facilityId)
        {
            var list = _rService.GetAllRolesByCorporateFacility(corporateId, facilityId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Facility Role

        /// <summary>
        /// Facilities the role.
        /// </summary>
        /// <returns></returns>
        //public ActionResult FacilityRole()
        //{
        //    var facilityRoleView = new FacilityRoleView { CurrentFacilityRole = new FacilityRoleCustomModel { IsActive = true }, };
        //    using (var _frService = new FacilityRoleBal())
        //    {
        //        var facilityId = 0;
        //        var corporateId = 0;
        //        var roleId = 0;
        //        if (Session[SessionNames.SessionClass.ToString()] != null)
        //        {
        //            var session = Session[SessionNames.SessionClass.ToString()] as SessionClass;
        //            facilityId = Helpers.GetLoggedInUserIsAdmin() ? 0 : session.FacilityId;
        //            corporateId = session.CorporateId;
        //            roleId = session.RoleId;
        //        }

        //        var list = _frService.GetFacilityRoleListCustom(corporateId, facilityId, roleId);
        //        facilityRoleView.FacilityRolesList = list;
        //    }
        //    return View(facilityRoleView);
        //}



        public ActionResult FacilityRole()
        {
            var facilityRoleView = new FacilityRoleView
            {
                CurrentFacilityRole = new FacilityRoleCustomModel { IsActive = true },
            };

            var facilityId = Helpers.GetDefaultFacilityId();
            var corporateId = Helpers.GetSysAdminCorporateID();
            var roleId = Convert.ToInt32(Helpers.GetDefaultRoleId());
            facilityRoleView.CurrentFacilityRole.CorporateId = corporateId;
            var admin = Helpers.GetLoggedInUserIsAdmin();
            if (admin == true)
            {
                var list = _frService.GetFacilityRoleListByAdminUser(corporateId, facilityId, roleId);
                facilityRoleView.FacilityRolesList = list;

            }
            else
            {
                var list = _frService.GetFacilityRoleListCustom(corporateId, facilityId, roleId);
                facilityRoleView.FacilityRolesList = list;

            }
            return View(facilityRoleView);
        }

        /// <summary>
        /// Adds the update facility role.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public ActionResult AddUpdateFacilityRole(FacilityRole model)
        {
            var newId = 0;
            var isExist = _frService.CheckIfExists(model.RoleId, model.FacilityId, model.CorporateId, model.FacilityRoleId);
            if (!isExist)
            {

                if (model.FacilityRoleId > 0)
                {
                    model.ModifiedBy = Helpers.GetLoggedInUserId();
                    model.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                    model.CreatedBy = Helpers.GetLoggedInUserId();
                    model.CreatedDate = Helpers.GetInvariantCultureDateTime();
                }
                else
                {
                    model.CreatedBy = Helpers.GetLoggedInUserId();
                    model.CreatedDate = Helpers.GetInvariantCultureDateTime();
                }
                newId = _frService.AddUpdateFacilityRole(model);
            }
            return Json(newId);

        }

        /// <summary>
        /// Adds the update facility role custom model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public ActionResult AddUpdateFacilityRoleCustomModel(FacilityRoleCustomModel model)
        {
            var newId = 0;
            var isExist = _frService.CheckIfExists(model.RoleId, model.FacilityId, model.CorporateId, model.FacilityRoleId);
            if (!isExist)
                newId = _frService.SaveFacilityRole(model, Helpers.GetLoggedInUserId(), Helpers.GetInvariantCultureDateTime());
            return Json(newId);
        }


        /// <summary>
        /// Gets the facility role by identifier.
        /// </summary>
        /// <param name="facilityRoleId">The facility role identifier.</param>
        /// <returns></returns>
        public ActionResult GetFacilityRoleById(string facilityRoleId)
        {
            // var current = _frService.GetFacilityRoleById(Convert.ToInt32(facilityRoleId));
            var current = _frService.GetFacilityRoleModelById(Convert.ToInt32(facilityRoleId));
            return PartialView(PartialViews.FacilityRoleAddEdit, current);
        }


        //public ActionResult GetFacilityRole(string facilityRoleId)
        //{
        //    using (var _frService = new FacilityRoleBal())
        //    {
        //        // var current = _frService.GetFacilityRoleById(Convert.ToInt32(facilityRoleId));
        //        var current = _frService.GetFacilityRoleModelById(Convert.ToInt32(facilityRoleId));
        //        var jsonResult = new
        //        {
        //            current.CorporateId,
        //            current.FacilityRoleId,
        //            current.SchedulingApplied,
        //            current.IsActive,
        //            current.FacilityId,
        //            current.CarePlanAccessible
        //        };
        //        return Json(jsonResult, JsonRequestBehavior.AllowGet);
        //    }
        //}

        /// <summary>
        /// Gets the facility roles list.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetFacilityRolesList()
        {
            var facilityId = Helpers.GetLoggedInUserIsAdmin() ? 0 : Helpers.GetDefaultFacilityId();
            var corporateId = Helpers.GetDefaultCorporateId();
            var roleId = Convert.ToInt32(Helpers.GetDefaultRoleId());

            var list = _frService.GetFacilityRoleListCustom(corporateId, facilityId, roleId);
            return PartialView(PartialViews.FacilityRoleList, list);
        }

        /// <summary>
        /// Deletes the facility role.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteFacilityRole(string id)
        {
            var current = _frService.GetFacilityRoleById(Convert.ToInt32(id));
            if (current != null)
            {
                current.IsDeleted = true;
                current.DeletedBy = Helpers.GetLoggedInUserId();
                current.DeletedDate = Helpers.GetInvariantCultureDateTime();
                var deletedId = _frService.AddUpdateFacilityRole(current);
                return PartialView(PartialViews.FacilityRoleList, null);
            }
            return Json(null);
        }

        /// <summary>
        /// Gets the facility list by corporate identifier.
        /// </summary>
        /// <param name="corpId">The corp identifier.</param>
        /// <returns></returns>
        public ActionResult GetFacilityListByCorporateId(string corpId)
        {
            var facilities = _fService.GetFacilitiesByCorporateId(Convert.ToInt32(corpId));
            if (facilities.Count > 0)
            {
                var facilityId = Helpers.GetLoggedInUserIsAdmin() ? 0 : Helpers.GetDefaultFacilityId();
                var list = new List<SelectListItem>();

                if (facilityId > 0 && Convert.ToInt32(corpId) > 0)
                    facilities = facilities.Where(f => f.FacilityId == facilityId).ToList();

                list.AddRange(facilities.Select(item => new SelectListItem
                {
                    Text = item.FacilityName,
                    Value = item.FacilityId.ToString()
                }));
                return Json(list);
            }
            return Json(null);
        }

        /// <summary>
        /// Checks if facility role exists.
        /// </summary>
        /// <param name="corpId">The corp identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="roleId">The role identifier.</param>
        /// <param name="facilityRoleId">The facility role identifier.</param>
        /// <param name="schedulingApplied">if set to <c>true</c> [scheduling applied].</param>
        /// <returns></returns>
        public ActionResult CheckIfFacilityRoleExists(string corpId, string facilityId, string roleId,
            string facilityRoleId, bool schedulingApplied)
        {
            if (!schedulingApplied)
            {
                var isRoleAssined = _frService.CheckRoleIsAssignOrNot(Convert.ToInt32(roleId), Convert.ToInt32(facilityId), Convert.ToInt32(corpId));
                if (isRoleAssined)
                {
                    return Json("-1");
                }
            }


            var isExists = _frService.CheckIfExists(Convert.ToInt32(roleId), Convert.ToInt32(facilityId),
                Convert.ToInt32(corpId), Convert.ToInt32(facilityRoleId));
            return Json(isExists);
        }

        /// <summary>
        /// Gets the facility roles custom list.
        /// </summary>
        /// <param name="corpId">The corp identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="roleId">The role identifier.</param>
        /// <returns></returns>
        public ActionResult GetFacilityRolesCustomList(string corpId, string facilityId, string roleId)
        {
            var selectedFacilitid = facilityId;
            facilityId =
                //Helpers.GetLoggedInUserIsAdmin()? "0":
                ((string.IsNullOrEmpty(facilityId) || facilityId.Equals("0"))
                    ? Convert.ToString(Helpers.GetDefaultFacilityId())
                    : facilityId);

            //var list = _frService.GetFacilityRoleListCustom(Convert.ToInt32(corpId), Convert.ToInt32(facilityId),
            //    Convert.ToInt32(roleId));
            var list = _frService.GetFacilityRoleListByFacility(Convert.ToInt32(corpId), Convert.ToInt32(facilityId),
              Convert.ToInt32(roleId));
            list = list.OrderBy(x => x.RoleName).ToList();
            return PartialView(PartialViews.FacilityRoleList, list);
        }


        public ActionResult GetFacilityRolesCustomList1(string corpId, string facilityId, string roleId)
        {
            var selectedFacilitid = facilityId;
            facilityId =
                //Helpers.GetLoggedInUserIsAdmin()? "0":
                ((string.IsNullOrEmpty(facilityId) || facilityId.Equals("0"))
                    ? Convert.ToString(Helpers.GetDefaultFacilityId())
                    : facilityId);

            var list = _frService.GetFacilityRoleListCustom(Convert.ToInt32(corpId), Convert.ToInt32(facilityId),
                Convert.ToInt32(roleId));

            list = list.OrderBy(x => x.RoleName).ToList();
            return PartialView(PartialViews.FacilityRoleList, list);
        }

        /// <summary>
        /// Reset the FacilityStructure View Model and pass it to FacilityStructureAddEdit Partial View.
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetFacilityRoleForm()
        {
            //Intialize the new object of FacilityStructure ViewModel
            var facilityStructureViewModel = new FacilityRoleCustomModel();

            //Pass the View Model as FacilityStructureViewModel to PartialView FacilityStructureAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.FacilityRoleAddEdit, facilityStructureViewModel);
        }

        /// <summary>
        /// Checks if facility role assigned.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <param name="facilityRoleId">The facility role identifier.</param>
        /// <returns></returns>
        public ActionResult CheckIfFacilityRoleAssigned(string roleId, string facilityRoleId)
        {
            var isExists = _frService.CheckIfRoleAssigned(Convert.ToInt32(roleId), Convert.ToInt32(facilityRoleId));
            return Json(isExists);
        }


        public ActionResult GetActiveInActiveFacilityRoleList(bool showInActive, string facilityId, string corporateId)
        {
            var selectedFacilitid = facilityId;
            facilityId =
                //Helpers.GetLoggedInUserIsAdmin()? "0":
                ((string.IsNullOrEmpty(facilityId) || facilityId.Equals("0"))
                    ? Convert.ToString(Helpers.GetDefaultFacilityId())
                    : facilityId);



            corporateId = ((string.IsNullOrEmpty(corporateId) || corporateId.Equals("0"))
                    ? Convert.ToString(Helpers.GetSysAdminCorporateID())
                    : corporateId);
            //var list = _frService.GetFacilityRoleListCustom(Convert.ToInt32(corpId), Convert.ToInt32(facilityId),
            //    Convert.ToInt32(roleId));
            var list = _frService.GetActiveInActiveRecords(showInActive, Convert.ToInt32(corporateId), Convert.ToInt32(facilityId));
            list = list.OrderBy(x => x.RoleName).ToList();
            return PartialView(PartialViews.FacilityRoleList, list);
        }

        #endregion

        #region Tabs Permission (Role Tabs)

        [HttpPost]
        public ActionResult Checkboxes(string[] checkedFiles)
        {
            checkedFiles = checkedFiles ?? new string[0];
            return null;
        }

        /// <summary>
        /// Tabses the role.
        /// </summary>
        /// <returns></returns>
        public ActionResult TabsRole()
        {
            var objRoleTabsView = new RoleTabsView
            {
                TabList = new List<TabsCustomModel>(),
                RoleList = new List<Role>(),
                CurrentRole = new Role()
            };
            return View(objRoleTabsView);
        }

        /// <summary>
        /// Adds the tab role permissions.
        /// </summary>
        /// <param name="objRoleTabsPermissionList">The object role tabs permission list.</param>
        /// <returns></returns>
        public ActionResult AddTabRolePermissions(List<RoleTabsCustomModel> objRoleTabsPermissionList)
        {
            var dt = Helpers.ToDataTable(objRoleTabsPermissionList);
            _rtService.AddUpdateRolePermissionSP(dt, Helpers.GetLoggedInUserId(), Helpers.GetSysAdminCorporateID(), Helpers.GetDefaultFacilityId());
            return Json(true);
        }

        /// <summary>
        /// Gets the tabs permisssions by role identifier.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <returns></returns>
        public ActionResult GetTabsPermisssionsByRoleId(int roleId)
        {
            var list = _rtService.GetRoleTabsByRoleId(roleId);
            return Json(list);
        }

        /// <summary>
        /// Gets the tabs assigned to facility.
        /// </summary>
        /// <param name="roleid">The roleid.</param>
        /// <returns></returns>
        public ActionResult GetTabsAssignedToFacility(int roleid)
        {
            var facilityrole = _frService.GetFacilityRolesByRoleId(roleid).FirstOrDefault();
            var corporateId = Helpers.GetDefaultCorporateId();
            var facilityid = Helpers.GetDefaultFacilityId();
            var loggedinuserid = Helpers.GetLoggedInUserId();
            if (facilityrole != null)
            {
                corporateId = facilityrole.CorporateId;
                facilityid = facilityrole.FacilityId;
            }
            var tabList = _tService.GetCorporateFacilityTabList(corporateId, facilityid, roleid).Where(t => t.CurrentTab.IsActive && !t.CurrentTab.IsDeleted)
                   .ToList();
            return PartialView(PartialViews.TabsTreeView, tabList);

        }

        /// <summary>
        /// Gets the tabs default to facility.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetTabsDefaultToFacility()
        {
            var corporateId = Helpers.GetDefaultCorporateId();
            var facilityid = Helpers.GetDefaultFacilityId();

            var tabList = _tService.GetTabsListInRoleTabsView(Helpers.GetLoggedInUserId(), facilityId: facilityid, corporateId: corporateId, isDeleted: false, isActive: true);
            return PartialView(PartialViews.TabsTreeView, tabList);
        }

        /// <summary>
        /// Gets the tabs assigned to facility.
        /// </summary>
        /// <param name="corporateId"></param>
        /// <param name="facilityId"></param>
        /// <returns></returns>
        public ActionResult GetTabsByCorporateAndFacility(int corporateId, int facilityId)
        {
            var tabList = _tService.GetTabsByCorporateAndFacilityId(facilityId, corporateId);
            return PartialView(PartialViews.TabsTreeView, tabList);

        }

        /// <summary>
        /// Gets the modules assigned to facility.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public ActionResult GetModulesAssignedToFacility(int corporateId, int facilityId)
        {
            //var moduleAccessList =
            //    _maService.GetModulesAccessList(corporateId, facilityId)
            //        .ToList();

            //var _tService = new TabsBal();
            //var tabList =
            //    _tService.GetCorporateFacilityTabList(corporateId, facilityId, null)
            //        .Where(t => t.CurrentTab.IsActive && !t.CurrentTab.IsDeleted)
            //        .ToList();
            //var newlist = tabList.Where(
            //    t => (moduleAccessList.Any(z => z.TabID == t.CurrentTab.TabId))).ToList();
            //IEnumerable<TabsCustomModel> newlist = new List<TabsCustomModel>();
            var newlist = _tService.GetTabsListInRoleTabsView(Helpers.GetLoggedInUserId(), facilityId: facilityId, corporateId: corporateId, isDeleted: false, isActive: true);
            return PartialView(PartialViews.TabsTreeView, newlist);

        }

        #endregion
    }
}
