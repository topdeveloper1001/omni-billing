using AutoMapper;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Model;
using BillingSystem.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;


namespace BillingSystem.Bal.BusinessAccess
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly IRepository<RolePermission> _repository;

        public RolePermissionService(IRepository<RolePermission> repository)
        {
            _repository = repository;
        }


        //function to get all role Permissions
        public List<RolePermission> GetAllRolePermissions()
        {
            var list = _repository.GetAll().ToList();
            return list;
        }
        //Function to get Role Permission by RoleID
        public List<RolePermission> GetRolePermissionByRoleId(int? roleId)
        {
            var list = _repository.Where(rp => rp.RoleID == roleId).ToList();
            return list;
        }
        //Function to add/delete role permissions
        public int AddUpdateRolePermission(List<RolePermission> rolePermissionList)
        {
            var result = -1;
            var newList = new List<RolePermission>();
            var listToBeDeleted = new List<RolePermission>();
            var iqueryableList = _repository.GetAll();
            foreach (var i in iqueryableList)
            {
                if (rolePermissionList.Any(r => r.RoleID == i.RoleID))
                    listToBeDeleted.Add(i);
            }
            _repository.Delete(listToBeDeleted);

            foreach (var item in rolePermissionList)
            {
                newList.Add(item);
            }
            result = Convert.ToInt32(_repository.Create(newList));
            return result;
        }
    }
}
