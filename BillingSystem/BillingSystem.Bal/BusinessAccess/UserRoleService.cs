using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Model;
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IRepository<UserRole> _repository;

        public UserRoleService(IRepository<UserRole> repository)
        {
            _repository = repository;
        }


        //Function Get User Roles by userID
        public List<UserRole> GetUserRolesByUserId(int userId)
        {
            var userRoles = _repository.Where(r => r.UserID == userId && r.IsActive && r.IsDeleted == false).Include(x => x.Role).Include(x => x.Role.FacilityRole).ToList();
            return userRoles;
        }

        /// <summary>
        /// Method to add/Update the role in the database.
        /// </summary>
        /// <returns></returns>
        public int AddUpdateUserRole(IEnumerable<UserRole> userRoles)
        {
            int result;
            var newList = new List<UserRole>();
            var lst = _repository.GetAll().ToList();
            var listToBeDeleted = lst.Where(i => userRoles.Any(r => r.UserID == i.UserID)).ToList();
            _repository.Delete(listToBeDeleted);
            newList.AddRange(userRoles);
            result = Convert.ToInt32(_repository.Create(newList));
            return result;
        }

        /// <summary>
        /// Deletes the role with user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public int DeleteRoleWithUser(int userId)
        {
            var result = -1;
            var lst = _repository.Where(x => x.UserID == userId).ToList();
            if (lst.Count > 0)
                _repository.Delete(lst);
            return result;
        }

        /// <summary>
        /// Checks the role exist.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <returns></returns>
        public bool CheckRoleExist(int roleId)
        {
            var role = _repository.Where(fr => fr.IsActive && fr.IsDeleted != true && fr.RoleID == roleId).FirstOrDefault();
            return role != null;
        }

        /// <summary>
        /// Gets the user roles by corporate facility and user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<UserRole> GetUserRolesByCorporateFacilityAndUserId(int userId, int corporateId, int facilityId)
        {
            var userRoles = _repository.Where(r => r.UserID == userId && r.IsActive && r.IsDeleted == false && (r.Role != null && r.Role.CorporateId != null && (int)r.Role.CorporateId == corporateId && r.Role.FacilityId != null && (int)r.Role.FacilityId == facilityId)).ToList();
            return userRoles;
        }

        /// <summary>
        /// Saves the user role.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public int SaveUserRole(UserRole model)
        {
            var isExists = CheckIfExists(model.UserID, model.RoleID);
            if (model.UserRoleID > 0 || isExists)
                _repository.UpdateEntity(model, model.UserRoleID);
            else
                _repository.Create(model);
            return model.UserRoleID;
        }

        /// <summary>
        /// Checks if exists.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="roleId">The role identifier.</param>
        /// <returns></returns>
        public bool CheckIfExists(int userId, int roleId)
        {
            var role = _repository.Where(fr => fr.IsActive && fr.IsDeleted != true && fr.RoleID == roleId && fr.UserID == userId).FirstOrDefault();
            return role != null;
        }

        /// <summary>
        /// Gets the user identifier by corporate and role type identifier.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="roleIds">The role ids.</param>
        /// <returns></returns>
        public List<UserRole> GetUserIdByCorporateAndRoleTypeId(int corporateId, List<Role> roleIds)
        {
            var list = roleIds.Select(r => r.RoleID).ToList();

            var userIds = _repository.Where(r => list.Contains(r.RoleID)).ToList();
            return userIds;
        }
    }
}
