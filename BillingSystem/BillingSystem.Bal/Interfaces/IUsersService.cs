﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model.EntityDto;

namespace BillingSystem.Bal.Interfaces
{
    public interface IUsersService
    {
        UsersViewModel AuthenticateUser(string userName, string password, DateTime currentDatetime, string ipAddress, string loginTypeId, out int statusId);

        Physician GetPhysicianById(int id);
        List<BillEditorUsersCustomModel> GetBillEditorUsers(int corporateId, int facilityId);
        int AddUpdateUser(Users m, int roleId);
        bool CheckDuplicateUser(string username, string email, int userId);
        bool CheckExistsPassword(string newPassword, int userid);
        bool CheckForDuplicateEmail(int userId, string email);
        bool CheckUserExistForCorporate(int corporateId);
        List<UsersCustomModel> GetAllUsersByFacilityId(int facilityId);
        string GetCoprateName(string corporateId);
        List<Users> GetFacilityUsers(int facilityId);
        List<Users> GetNonAdminUsersByCorporate(int corporateId);
        List<PhysicianViewModel> GetPhysiciansByRole(int facilityId, int cId);
        IEnumerable<Tabs> GetTabsByUserIdRoleId(int userId, int roleId, int facilityId, int corporateid, bool isDeleted = false, bool isActive = true);
        IEnumerable<Tabs> GetTabsByUserName(string userName);
        Users GetUser(string username, string password, string loginType = "username");
        UsersViewModel GetUserByEmail(string email);
        UsersViewModel GetUserByEmailAndToken(string email, string resetToken);
        UsersViewModel GetUserByEmailWithoutDecryption(string email);
        Users GetUserById(int? userId);
        Users GetUserbyUserName(string username);
        CommonModel GetUserDetails(int roleId, int facilityId, int userId);
        string GetUserEmailByUserId(int userid);
        List<Users> GetUsersByCorporateandFacilityId(int corporateId, int facilityId);
        List<UsersCustomModel> GetUsersByCorporateIdFacilityId(int corporateId, int facilityId);
        List<UsersViewModel> GetUsersByRole(int facilityId, int cId);
        int UpdateUser(Users user);
        string UserFacilities(IEnumerable<UserRole> userRoles);
        string UserRoles(IEnumerable<UserRole> roles);
        string GetNameByUserId(int id);

        Task<UserDto> AuthenticateAsync(string username, string password, string deviceToken, string platform, bool isPatient = false);
        Task<UserDto> GetUserAsync(long userId, bool isPatient = true, string deviceToken = "", string platform = "");
        Task<bool> SaveUserLocationAsync(string lat, string lng, long userId, bool isPatient = true);
        Task<List<PatientDto>> GetPatientsByUserIdAsync(long userId = 0);
    }
}