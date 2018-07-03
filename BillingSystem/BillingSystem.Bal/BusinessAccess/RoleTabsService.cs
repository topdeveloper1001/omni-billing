using System.Data;
using BillingSystem.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model.CustomModel;

using AutoMapper;
using BillingSystem.Common.Common;
using System.Data.SqlClient;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class RoleTabsService : IRoleTabsService
    {
        private readonly IRepository<RoleTabs> _repository;
        private readonly IRepository<Tabs> _tRepository;

        public RoleTabsService(IRepository<RoleTabs> repository, IRepository<Tabs> tRepository)
        {
            _repository = repository;
            _tRepository = tRepository;
        }


        //function to get all role Permissions
        /// <summary>
        /// Gets all role tabs permissions.
        /// </summary>
        /// <returns></returns>
        public List<RoleTabs> GetAllRoleTabsPermissions()
        {
            var list = _repository.GetAll().ToList();
            return list;
        }

        //Function to get Role Permission by RoleID
        /// <summary>
        /// Gets the role tabs by role identifier.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <returns></returns>
        public List<RoleTabs> GetRoleTabsByRoleId(int? roleId)
        {
            var list = _repository.Where(rp => rp.RoleID == roleId).ToList();
            return list;
        }

        /// <summary>
        /// Checks if tab name accessible to given role.
        /// </summary>
        /// <param name="tabName">Name of the tab.</param>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="roleId">The role identifier.</param>
        /// <returns></returns>
        public bool CheckIfTabNameAccessibleToGivenRole(string tabName, string controllerName, string actionName,
            int roleId)
        {
            controllerName = controllerName.ToLower().Trim();
            tabName = tabName.ToLower().Trim();
            actionName = actionName.ToLower().Trim();
            var tabId =
                Convert.ToInt32(_tRepository.Where(
                        t =>
                            (t.Controller.ToLower().Trim().Equals(controllerName) ||
                             string.IsNullOrEmpty(controllerName)) &&
                            t.TabName.ToLower().Trim().Equals(tabName) &&
                            (t.Action.ToLower().Trim().Equals(actionName) || string.IsNullOrEmpty(controllerName)) &&
                            !t.IsDeleted)
                        .Select(i => i.TabId)
                        .FirstOrDefault());

            if (tabId > 0)
            {
                var isExists = _repository.Where(rp => rp.RoleID == roleId && rp.TabID == tabId).Any();
                return isExists;
            }
            return false;
        }

        //Function to add/delete role permissions
        /// <summary>
        /// Adds the update role permission.
        /// </summary>
        /// <param name="roleTabsList">The role tabs list.</param>
        /// <returns></returns>
        public int AddUpdateRolePermission(List<RoleTabs> roleTabsList)
        {
            var result = -1;
            var roleId = roleTabsList.Where(a => a.RoleID != null).Select(r => (int)r.RoleID).FirstOrDefault();
            var deleted = _repository.Where(r => r.RoleID != null && (int)r.RoleID == roleId).ToList();
            _repository.Delete(deleted);

            //Add newly added role tabs in the Database table RoleTabs, with Parent Tabs in the same table
            foreach (var item in roleTabsList)
            {
                var current = _tRepository.Where(t => t.TabId == item.TabID).FirstOrDefault();
                SaveNodesData(current, Convert.ToInt32(item.RoleID));
            }
            result = 1;
            return result;
        }


        private void SaveNodesData(Tabs tab, int roleId)
        {
            var isExists = _repository.Where(r => r.RoleID == roleId && r.TabID == tab.TabId).Any();
            if (!isExists)
            {
                var roleTab = new RoleTabs
                {
                    RoleID = roleId,
                    TabID = tab.TabId,
                    ID = 0
                };
                _repository.Create(roleTab);
            }

            //Check for the Current tab's Parent Tab
            if (tab.ParentTabId > 0)
            {
                var parentTab = _tRepository.Where(rp => rp.TabId == tab.ParentTabId).FirstOrDefault();
                if (parentTab != null)
                    SaveNodesData(parentTab, roleId);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public int AddUpdateRolePermissionSP(DataTable dt, long userId, long corporaIdId, long facilityId)
        {
            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter
            {
                ParameterName = "@pRoleTabsList",
                SqlDbType = SqlDbType.Structured,
                Value = dt,
                TypeName = "RoleTabsTT"
            };

            sqlParameters[1] = new SqlParameter("@pUId", userId);
            sqlParameters[2] = new SqlParameter("@pCId", corporaIdId);
            sqlParameters[3] = new SqlParameter("@pFId", facilityId);

            _repository.ExecuteCommand(StoredProcedures.SPROC_AddUpdateRolePermission.ToString(), sqlParameters);
            return 1;
        }


        public List<TabsAccessible> CheckIfTabsAccessibleToGivenRole(int roleId, IEnumerable<Tabs> tabs)
        {
            var result = new List<TabsAccessible>();
            result.AddRange(from item in tabs
                            let tabId =
                                Convert.ToInt32(
                                    _tRepository.Where(
                                        t =>
                                            (t.Controller.ToLower().Trim().Equals(item.Controller) ||
                                             string.IsNullOrEmpty(item.Controller)) &&
                                            t.TabName.ToLower().Trim().Equals(item.TabName) &&
                                            (t.Action.ToLower().Trim().Equals(item.Action) ||
                                             string.IsNullOrEmpty(item.Controller)) && !t.IsDeleted)
                                        .Select(i => i.TabId)
                                        .FirstOrDefault())
                            where tabId > 0
                            let isExists = _repository.Where(rp => rp.RoleID == roleId && rp.TabID == tabId).Any()
                            select new TabsAccessible
                            {
                                IsAccessible = isExists,
                                TabId = item.TabId
                            });
            return result;
        }
    }
}
