 
using BillingSystem.Model.CustomModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.SqlServer;
using BillingSystem.Model;
using BillingSystem.Repository.Interfaces;
using AutoMapper;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{ 
    public class RoleService : IRoleService
    {
        private readonly IRepository<Role> _repository;
        private readonly IRepository<FacilityRole> _frRepository;

        private readonly IMapper _mapper;
        private readonly BillingEntities _context;

        public RoleService(IRepository<Role> repository, IRepository<FacilityRole> frRepository, IMapper mapper, BillingEntities context)
        {
            _repository = repository;
            _frRepository = frRepository;
            _mapper = mapper;
            _context = context;
        }

        #region Public Methods and Operators

        /// <summary>
        /// Method to add/Update the role in the database.
        /// </summary>
        /// <param name="role">
        /// The role.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int AddUpdateRole(Role role)
        {
            role.IsGeneric = false;
            if (role.RoleID > 0)
            {
                var currentRoleKey = _repository.Where(r => r.RoleID == role.RoleID).Max(a => a.RoleKey);
                role.RoleKey = currentRoleKey;
                _repository.UpdateEntity(role, role.RoleID);
            }
            else
            {
                if (string.IsNullOrEmpty(role.RoleKey))
                {
                    var newRoleKey = _repository.Where(a => a.FacilityId == role.FacilityId && a.IsActive == true && a.IsDeleted == false).Max(a => a.RoleID);
                    role.RoleKey = Convert.ToString(newRoleKey + 1);
                }
                _repository.Create(role);
            }

            return role.RoleID;
        }

        /// <summary>
        /// Method to To check Duplicate role on the basis of rolename
        /// </summary>
        /// <param name="roleId">
        /// The Role Id.
        /// </param>
        /// <param name="roleName">
        /// The Role Name.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CheckDuplicateRole(int roleId, string roleName)
        {
            var role = _repository.Where(x => x.RoleID != roleId && x.RoleName == roleName && x.IsDeleted == false).FirstOrDefault();
            return role != null;
        }

        /// <summary>
        /// Get the user after login
        /// </summary>
        /// <param name="corporateId">
        /// The corporate Id.
        /// </param>
        /// <returns>
        /// Return the user after login
        /// </returns>
        public List<Role> GetAllRoles(int corporateId)
        {
            var list = corporateId > 0
                                  ? _repository.Where(
                                      x =>
                                      x.IsDeleted == false && x.CorporateId != null
                                      && x.CorporateId == corporateId).ToList()
                                  : _repository.Where(x => x.IsDeleted == false).ToList();
            return list;
        }

        /// <summary>
        /// Get the user after login
        /// </summary>
        /// <param name="corporateId">
        /// The corporate Id.
        /// </param>
        /// <param name="facilityId">
        /// The facility Id.
        /// </param>
        /// <returns>
        /// Return the user after login
        /// </returns>
        public List<Role> GetAllRolesByCorporateFacility(int corporateId, int facilityId)
        {
            var list = corporateId > 0
                ? _repository.Where(
                    x =>
                        x.IsDeleted == false && x.FacilityId == facilityId && (corporateId == 0 || x.CorporateId == corporateId))
                    .OrderBy(r => r.RoleName)
                    .ToList()
                : _repository.Where(x => x.IsDeleted == false).OrderBy(r => r.RoleName).ToList();

            return list.GroupBy(test => test.RoleName).Select(x => x.FirstOrDefault()).OrderBy(r => r.RoleName).ToList();
        }

        /// <summary>
        /// The get facility roles by corporate id facility id.
        /// </summary>
        /// <param name="corporateId">
        /// The corporate id.
        /// </param>
        /// <param name="facilityId">
        /// The facility id.
        /// </param>
        /// <returns>
        /// </returns>
        public List<Role> GetFacilityRolesByCorporateIdFacilityId(int corporateId, int facilityId)
        {
            var list = new List<Role>();
            var roles =
                _frRepository.Where(r => r.CorporateId == corporateId && r.FacilityId == facilityId && !r.IsDeleted).ToList();
            list.AddRange(
                roles.Select(
                    item => new Role { RoleID = item.RoleId, RoleName = GetRoleNameById(item.RoleId) }));
            return list;
        }

        /// <summary>
        /// Gets the physician roles by corporate identifier.
        /// </summary>
        /// <param name="corporateId">
        /// The corporate identifier.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<Role> GetPhysicianRolesByCorporateId(int corporateId)
        {
            var roles = _repository.Where(r => r.CorporateId == corporateId && r.RoleName.Contains("Phy") && !r.IsDeleted)
                    .ToList().OrderBy(r => r.RoleName)
                    .ToList();
            return roles;
        }

        /// <summary>
        /// Gets the role by identifier.
        /// </summary>
        /// <param name="roleID">
        /// The role identifier.
        /// </param>
        /// <returns>
        /// The <see cref="Role"/>.
        /// </returns>
        public Role GetRoleById(int roleID)
        {
            var role = _repository.Where(r => r.RoleID == roleID).FirstOrDefault();
            return role;
        }

        // method to get role by role name
        /// <summary>
        /// Gets the name of the role by role.
        /// </summary>
        /// <param name="roleName">
        /// Name of the role.
        /// </param>
        /// <returns>
        /// The <see cref="Role"/>.
        /// </returns>
        public Role GetRoleByRoleName(string roleName)
        {
            var role = _repository.Where(x => x.RoleName == roleName && x.IsDeleted == false).FirstOrDefault();
            return role;
        }

        /// <summary>
        /// Gets the name of the role identifier by facility and.
        /// </summary>
        /// <param name="roleName">
        /// Name of the role.
        /// </param>
        /// <param name="cId">
        /// The c identifier.
        /// </param>
        /// <param name="fId">
        /// The f identifier.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetRoleIdByFacilityAndName(string roleName, int cId, int fId)
        {
            var role = _repository.Where(p => p.CorporateId == cId && p.FacilityId == fId && p.RoleName.Trim().ToLower().Equals(roleName)).FirstOrDefault();
            return role != null ? role.RoleID : 0;
        }

        /// <summary>
        /// Gets the role name by identifier.
        /// </summary>
        /// <param name="roleID">
        /// The role identifier.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetRoleNameById(int roleID)
        {
            var role = _repository.Where(r => r.RoleID == roleID).FirstOrDefault();
            return role != null ? role.RoleName : string.Empty;
        }

        /// <summary>
        /// Gets the roles by corporate identifier.
        /// </summary>
        /// <param name="corporateId">
        /// The corporate identifier.
        /// </param>
        /// <returns>
        /// </returns>
        public List<Role> GetRolesByCorporateId(int corporateId)
        {
            var roles = _repository.Where(r => r.CorporateId == corporateId && !r.IsDeleted)
                    .ToList()
                    .OrderBy(r => r.RoleName)
                    .ToList();
            return roles;
        }

        /// <summary>
        /// Gets the roles by corporate identifier facility identifier.
        /// </summary>
        /// <param name="corporateId">
        /// The corporate identifier.
        /// </param>
        /// <param name="facilityId">
        /// The facility identifier.
        /// </param>
        /// <returns>
        /// </returns>
        public List<Role> GetRolesByCorporateIdFacilityId(int corporateId, int facilityId)
        {
            var roles = _repository.Where(r => r.CorporateId == corporateId && r.FacilityId == facilityId && !r.IsDeleted).OrderBy(r => r.RoleName).ToList();
            return roles;
        }

        public List<DropdownListData> GetRolesByFacility(int facilityId)
        {
            var list = _repository.Where(
                x => x.IsDeleted == false && x.FacilityId == facilityId).Select(i => new DropdownListData
                {
                    Text = i.RoleName,
                    Value = SqlFunctions.StringConvert((double)i.RoleID).Trim()
                }).Distinct().OrderBy(i => i.Text).ToList();
            return list;
        }

        #endregion
    }
}