using BillingSystem.Model;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model.CustomModel;
using System;
using BillingSystem.Repository.Interfaces;
using AutoMapper;
using System.Data.SqlClient;
using BillingSystem.Common.Common;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class FacilityRoleService : IFacilityRoleService
    {
        private readonly IRepository<Corporate> _cRepository;
        private readonly IRepository<FacilityRole> _repository;
        private readonly IRepository<Physician> _phRepository;
        private readonly IRepository<Facility> _fRepository;
        private readonly IRepository<Role> _rRepository;
        private readonly IMapper _mapper;

        public FacilityRoleService(IRepository<Corporate> cRepository, IRepository<FacilityRole> repository, IRepository<Physician> phRepository, IRepository<Facility> fRepository, IRepository<Role> rRepository, IMapper mapper)
        {
            _cRepository = cRepository;
            _repository = repository;
            _phRepository = phRepository;
            _fRepository = fRepository;
            _rRepository = rRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets the facility role by identifier.
        /// </summary>
        /// <param name="facilityRoleId">The facility role identifier.</param>
        /// <returns></returns>
        public FacilityRole GetFacilityRoleById(int facilityRoleId)
        {
            var facilityRole = _repository.Where(fr => fr.IsActive && !fr.IsDeleted && fr.FacilityRoleId == facilityRoleId).FirstOrDefault();
            return facilityRole;
        }

        /// <summary>
        /// Adds the update facility role.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public int AddUpdateFacilityRole(FacilityRole model)
        {
            if (model.FacilityRoleId > 0)
                _repository.UpdateEntity(model, model.FacilityRoleId);
            else
                _repository.Create(model);
            return model.FacilityRoleId;
        }

        /// <summary>
        /// Checks if exists.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityRoleId">The facility role identifier.</param>
        /// <returns></returns>
        public bool CheckIfExists(int roleId, int facilityId, int corporateId, int facilityRoleId)
        {
            var current = new FacilityRole();
            if (facilityRoleId > 0)
                current = _repository.Where(f => f.FacilityRoleId != facilityRoleId && f.RoleId == roleId && f.FacilityId == facilityId && f.CorporateId == corporateId
                  && f.IsActive).FirstOrDefault();
            else
                current = _repository.Where(f => f.RoleId == roleId && f.FacilityId == facilityId && f.CorporateId == corporateId && f.IsActive).FirstOrDefault();
            return current != null;

        }


        public bool CheckRoleIsAssignOrNot(int roleId, int facilityId, int corporateId)
        {
            var currentStatus = _phRepository.Where(x => x.UserType == roleId && x.FacilityId == facilityId && x.CorporateId == corporateId && x.IsActive)
                    .FirstOrDefault();
            return currentStatus != null;
        }

        /// <summary>
        /// Gets the facility roles by role identifier.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <returns></returns>
        public IEnumerable<FacilityRole> GetFacilityRolesByRoleId(int roleId)
        {
            var facilityRole = _repository.Where(fr => fr.IsActive && !fr.IsDeleted && fr.RoleId == roleId).ToList();
            return facilityRole;
        }

        /// <summary>
        /// Adds the update facility role custom model.
        /// </summary>
        /// <param name="vm">The model.</param>
        /// <returns></returns>
        public int AddUpdateFacilityRoleCustomModel(FacilityRoleCustomModel vm)
        {
            var newRoleId = 0;
            var frModel = _mapper.Map<FacilityRole>(vm);
            if (vm.RoleId == 0 && !string.IsNullOrEmpty(vm.RoleName))
            {
                var rModel = new Role
                {
                    RoleID = vm.RoleId,
                    CorporateId = vm.CorporateId,
                    FacilityId = vm.FacilityId, // new column added on Nov 07 2014 by Shashank
                    IsActive = vm.IsActive,
                    RoleName = vm.RoleName,
                    CreatedBy = vm.CreatedBy,
                    CreatedDate = vm.CreatedDate,
                    ModifiedBy = vm.ModifiedBy,
                    ModifiedDate = vm.ModifiedDate,
                    IsDeleted = vm.IsDeleted,
                    DeletedBy = vm.DeletedBy,
                    DeletedDate = vm.DeletedDate,
                };

                if (!CheckDuplicateRole(vm.RoleId, vm.RoleName))
                    newRoleId = AddUpdateRole(rModel);
                else
                    return 0;

                //Add newly added role to other facilities of this corporate
                if (vm.AddToAll)
                {
                    var facIds = GetFacilityIdsByCorporateId(vm.CorporateId);
                    if (facIds != null && facIds.Any())
                    {
                        facIds = facIds.Where(f => f != vm.FacilityId).ToList();
                        foreach (var facId in facIds)
                        {
                            //Add entries in the table Role for other facilities too.
                            var rModel1 = new Role
                            {
                                RoleID = 0,
                                CorporateId = vm.CorporateId,
                                FacilityId = facId, // new column added on Nov 07 2014 by Shashank
                                IsActive = vm.IsActive,
                                RoleName = vm.RoleName,
                                CreatedBy = vm.CreatedBy,
                                CreatedDate = vm.CreatedDate,
                                ModifiedBy = vm.ModifiedBy,
                                ModifiedDate = vm.ModifiedDate,
                                IsDeleted = vm.IsDeleted,
                                DeletedBy = vm.DeletedBy,
                                DeletedDate = vm.DeletedDate,
                                IsGeneric = false
                            };

                            var nRoleId = AddUpdateRole(rModel1);

                            //Add Entries in the table FacilityRole table for other facilities.
                            frModel.FacilityId = facId;
                            frModel.RoleId = nRoleId;
                            _repository.Create(frModel);
                        }
                    }
                }
            }

            frModel.FacilityId = vm.FacilityId;
            frModel.RoleId = newRoleId > 0 ? newRoleId : vm.RoleId;

            if (vm.FacilityRoleId > 0)
                _repository.UpdateEntity(frModel, frModel.FacilityRoleId);
            else
                _repository.Create(frModel);

            return 1;
        }
        private bool CheckDuplicateRole(int roleId, string roleName)
        {
            var role = _rRepository.Where(x => x.RoleID != roleId && x.RoleName == roleName && x.IsDeleted == false)
                    .FirstOrDefault();
            return role != null;
        }
        private int AddUpdateRole(Role role)
        {
            role.IsGeneric = false;
            if (role.RoleID > 0)
            {
                var currentRoleKey = _rRepository.Where(r => r.RoleID == role.RoleID).Max(a => a.RoleKey);
                role.RoleKey = currentRoleKey;
                _rRepository.UpdateEntity(role, role.RoleID);
            }
            else
            {
                if (string.IsNullOrEmpty(role.RoleKey))
                {
                    var newRoleKey = _rRepository.Where(a => a.FacilityId == role.FacilityId && a.IsActive == true && a.IsDeleted == false).Max(a => a.RoleID);
                    role.RoleKey = Convert.ToString(newRoleKey + 1);
                }
                _rRepository.Create(role);
            }

            return role.RoleID;
        }

        private List<int> GetFacilityIdsByCorporateId(int cId)
        {
            List<int> list;

            list = _fRepository.Where(c => c.CorporateID == cId).Select(f => f.FacilityId).ToList();
            return list;
        }

        /// <summary>
        /// Gets the facility role model by identifier.
        /// </summary>
        /// <param name="facilityRoleId">The facility role identifier.</param>
        /// <returns></returns>
        public FacilityRole GetFacilityRoleModelById(int facilityRoleId)
        {
            var model = _repository.Where(fr => !fr.IsDeleted && fr.FacilityRoleId == facilityRoleId).FirstOrDefault();
            if (model != null)
            {
                var facilityroleModel = new FacilityRoleCustomModel
                {
                    FacilityRoleId = model.FacilityRoleId,
                    FacilityId = model.FacilityId,
                    RoleId = model.RoleId,
                    CorporateId = model.CorporateId,
                    CreatedBy = model.CreatedBy,
                    CreatedDate = model.CreatedDate,
                    ModifiedBy = model.ModifiedBy,
                    ModifiedDate = model.ModifiedDate,
                    IsDeleted = model.IsDeleted,
                    DeletedBy = model.DeletedBy,
                    DeletedDate = model.DeletedDate,
                    IsActive = model.IsActive,
                    SchedulingApplied = model.SchedulingApplied,
                    CarePlanAccessible = model.CarePlanAccessible
                };
                return facilityroleModel;
            }
            return new FacilityRole();
        }

        //function to check if Active Corporate Exist
        /// <summary>
        /// Checks the corporate exist.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <returns></returns>
        public bool CheckCorporateExist(int corporateId)
        {
            var facilityRole = _repository.Where(fr => fr.IsActive && fr.IsDeleted != true && fr.CorporateId == corporateId).FirstOrDefault();
            return facilityRole != null;
        }

        /// <summary>
        /// Gets the facility role list custom.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="roleId">The role identifier.</param>
        /// <returns></returns>
        public List<FacilityRoleCustomModel> GetFacilityRoleListCustom(int corporateId, int facilityId, int roleId)
        {
            var list = new List<FacilityRoleCustomModel>();
            var facilityRoleList = corporateId > 0 ?
                (facilityId == 0
                ? _repository.Where(fr => !fr.IsDeleted && fr.IsActive && fr.CorporateId == corporateId).ToList()
                : _repository.Where(fr => !fr.IsDeleted && fr.IsActive && fr.CorporateId == corporateId
                                  && fr.FacilityId == facilityId).ToList())
                                  :
                                  _repository.Where(fr => !fr.IsDeleted && fr.IsActive).ToList();

            if (roleId > 0)
                facilityRoleList = facilityRoleList.Where(r => r.RoleId == roleId).ToList();


            list.AddRange(facilityRoleList.Select(item => new FacilityRoleCustomModel
            {
                FacilityRoleId = item.FacilityRoleId,
                FacilityId = item.FacilityId,
                RoleId = item.RoleId,
                CorporateId = item.CorporateId,
                CreatedBy = item.CreatedBy,
                CreatedDate = item.CreatedDate,
                ModifiedBy = item.ModifiedBy,
                ModifiedDate = item.ModifiedDate,
                IsDeleted = item.IsDeleted,
                DeletedBy = item.DeletedBy,
                DeletedDate = item.DeletedDate,
                IsActive = item.IsActive,
                FacilityName = _fRepository.Get(item.FacilityId).FacilityName,
                CorporateName = GetCorporateNameById(item.CorporateId),
                RoleName = GetRoleNameById(item.RoleId)
            }));
            list = list.GroupBy(x => new { x.RoleName, x.FacilityId }).Select(x => x.FirstOrDefault()).ToList();
            return list.OrderBy(x => x.RoleName).ToList();
        }
        private string GetCorporateNameById(int? corporateId)
        {
            var q = _cRepository.Where(a => a.CorporateID == corporateId).FirstOrDefault();
            return (q != null) ? q.CorporateName : string.Empty;
        }

        public List<FacilityRoleCustomModel> GetFacilityRoleListByFacility(int corporateId, int facilityId, int roleId)
        {
            var list = new List<FacilityRoleCustomModel>();
            var facilityRoleList = corporateId > 0 ?
                (facilityId == 0
                ? _repository.Where(fr => !fr.IsDeleted && fr.IsActive && fr.CorporateId == corporateId).ToList()
                : _repository.Where(fr => !fr.IsDeleted && fr.IsActive && fr.CorporateId == corporateId
                                  && fr.FacilityId == facilityId).ToList())
                                  :
                                  _repository.Where(fr => !fr.IsDeleted && fr.IsActive).ToList();

            //if (roleId > 0)
            //    facilityRoleList = facilityRoleList.Where(r => r.RoleId == roleId).ToList();


            list.AddRange(facilityRoleList.Select(item => new FacilityRoleCustomModel
            {
                FacilityRoleId = item.FacilityRoleId,
                FacilityId = item.FacilityId,
                RoleId = item.RoleId,
                CorporateId = item.CorporateId,
                CreatedBy = item.CreatedBy,
                CreatedDate = item.CreatedDate,
                ModifiedBy = item.ModifiedBy,
                ModifiedDate = item.ModifiedDate,
                IsDeleted = item.IsDeleted,
                DeletedBy = item.DeletedBy,
                DeletedDate = item.DeletedDate,
                IsActive = item.IsActive,
                FacilityName = _fRepository.Get(item.FacilityId).FacilityName,
                CorporateName = GetCorporateNameById(item.CorporateId),
                RoleName = GetRoleNameById(item.RoleId)
            }));
            list = list.GroupBy(x => new { x.RoleName, x.FacilityId }).Select(x => x.FirstOrDefault()).ToList();
            return list.OrderBy(x => x.RoleName).ToList();
        }

        /// <summary>
        /// Checks if role assigned.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <param name="facilityRoleId">The facility role identifier.</param>
        /// <returns></returns>
        public bool CheckIfRoleAssigned(int roleId, int facilityRoleId)
        {
            if (facilityRoleId > 0)
            {
                var current = _repository.Where(f => f.FacilityRoleId != facilityRoleId && f.RoleId == roleId && f.IsActive).FirstOrDefault();
                return current != null;
            }
            else
            {
                var current = _repository.Where(f => f.RoleId == roleId && f.IsActive).FirstOrDefault();
                return current != null;
            }
        }

        /// <summary>
        /// Saves the facility role if not exists.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public bool SaveFacilityRoleIfNotExists(FacilityRole model)
        {
            var currentModel = _repository.Where(f => f.FacilityId == model.FacilityId && f.RoleId == model.RoleId).FirstOrDefault();
            if (currentModel != null)
            {
                currentModel.IsActive = true;
                currentModel.IsDeleted = false;
            }
            else
                _repository.Create(model);
            return true;
        }

        #region Bind Facility Grid According to User is Admin or Not


        public List<FacilityRoleCustomModel> GetFacilityRoleListByAdminUser(int corporateId, int facilityId, int roleId)
        {
            var list = new List<FacilityRoleCustomModel>();
            var facilityRoleList = corporateId > 0 ?
                (facilityId == 0
                ? _repository.Where(fr => !fr.IsDeleted && fr.IsActive && fr.CorporateId == corporateId).ToList()
                : _repository.Where(fr => !fr.IsDeleted && fr.IsActive && fr.CorporateId == corporateId
                                  && fr.FacilityId == facilityId).ToList())
                                  :
                                  _repository.Where(fr => !fr.IsDeleted && fr.IsActive).ToList();

            list.AddRange(facilityRoleList.Select(item => new FacilityRoleCustomModel
            {
                FacilityRoleId = item.FacilityRoleId,
                FacilityId = item.FacilityId,
                RoleId = item.RoleId,
                CorporateId = item.CorporateId,
                CreatedBy = item.CreatedBy,
                CreatedDate = item.CreatedDate,
                ModifiedBy = item.ModifiedBy,
                ModifiedDate = item.ModifiedDate,
                IsDeleted = item.IsDeleted,
                DeletedBy = item.DeletedBy,
                DeletedDate = item.DeletedDate,
                IsActive = item.IsActive,
                FacilityName = _fRepository.Get(item.FacilityId).FacilityName,
                CorporateName = GetCorporateNameById(item.CorporateId),
                RoleName = GetRoleNameById(item.RoleId)
            }));
            list = list.GroupBy(x => new { x.RoleName, x.FacilityId }).Select(x => x.FirstOrDefault()).ToList();
            return list.OrderBy(x => x.RoleName).ToList();
        }


        #endregion

        public List<FacilityRoleCustomModel> GetUserTypeRoleDropDown(int corporateId, int facilityId, bool scheduledApplied)
        {
            var list = new List<FacilityRoleCustomModel>();
            var facilityRoleList = _repository.Where(x => x.IsDeleted != true && x.CorporateId == corporateId && x.FacilityId == facilityId && x.SchedulingApplied == scheduledApplied).ToList();
            list.AddRange(facilityRoleList.Select(item => new FacilityRoleCustomModel
            {
                RoleId = item.RoleId,
                RoleName = GetRoleNameById(item.RoleId)
            }));
            return list;
        }

        public List<FacilityRoleCustomModel> GetUserTypeRoleDropDownInTaskPlan(int corporateId, int facilityId, bool carePlanAccessible)
        {
            var list = new List<FacilityRoleCustomModel>();
            var facilityRoleList = _repository.Where(x => x.IsDeleted != true && x.CorporateId == corporateId && x.FacilityId == facilityId && x.CarePlanAccessible == carePlanAccessible).ToList();
            list.AddRange(facilityRoleList.Select(item => new FacilityRoleCustomModel
            {
                RoleId = item.RoleId,
                RoleName = GetRoleNameById(item.RoleId)
            }));
            return list;
        }



        public List<FacilityRoleCustomModel> GetActiveInActiveRecords(bool showInActive, int corporateId, int facilityId)
        {
            var list = new List<FacilityRoleCustomModel>();
            var facilityRoleList = corporateId > 0 ?
                (facilityId == 0
                ? _repository.Where(fr => !fr.IsDeleted && fr.IsActive == showInActive && fr.CorporateId == corporateId).ToList()
                : _repository.Where(fr => !fr.IsDeleted && fr.IsActive == showInActive && fr.CorporateId == corporateId
                                  && fr.FacilityId == facilityId).ToList())
                                  :
                                  _repository.Where(fr => !fr.IsDeleted && fr.IsActive == showInActive).ToList();

            //if (roleId > 0)
            //    facilityRoleList = facilityRoleList.Where(r => r.RoleId == roleId).ToList();


            list.AddRange(facilityRoleList.Select(item => new FacilityRoleCustomModel
            {
                FacilityRoleId = item.FacilityRoleId,
                FacilityId = item.FacilityId,
                RoleId = item.RoleId,
                CorporateId = item.CorporateId,
                CreatedBy = item.CreatedBy,
                CreatedDate = item.CreatedDate,
                ModifiedBy = item.ModifiedBy,
                ModifiedDate = item.ModifiedDate,
                IsDeleted = item.IsDeleted,
                DeletedBy = item.DeletedBy,
                DeletedDate = item.DeletedDate,
                IsActive = item.IsActive,
                FacilityName = _fRepository.Get(item.FacilityId).FacilityName,
                CorporateName = GetCorporateNameById(item.CorporateId),
                RoleName = GetRoleNameById(item.RoleId)
            }));

            list = list.GroupBy(x => new
            {
                x.RoleName,
                x.FacilityId
            }).Select(x => x.FirstOrDefault()).ToList();
            return list.OrderBy(x => x.RoleName).ToList();
        }

        public List<FacilityRoleCustomModel> GetFacilityRoleListData(int corporateId, int facilityId, int roleId, bool showInActive)
        {
            var list = new List<FacilityRoleCustomModel>();
            var facilityRoleList = corporateId > 0 ?
                (facilityId == 0
                ? _repository.Where(fr => !fr.IsDeleted && fr.IsActive && fr.CorporateId == corporateId).ToList()
                : _repository.Where(fr => !fr.IsDeleted && fr.IsActive && fr.CorporateId == corporateId
                                  && fr.FacilityId == facilityId).ToList())
                                  :
                                  _repository.Where(fr => !fr.IsDeleted && fr.IsActive).ToList();

            list.AddRange(facilityRoleList.Select(item => new FacilityRoleCustomModel
            {
                FacilityRoleId = item.FacilityRoleId,
                FacilityId = item.FacilityId,
                RoleId = item.RoleId,
                CorporateId = item.CorporateId,
                CreatedBy = item.CreatedBy,
                CreatedDate = item.CreatedDate,
                ModifiedBy = item.ModifiedBy,
                ModifiedDate = item.ModifiedDate,
                IsDeleted = item.IsDeleted,
                DeletedBy = item.DeletedBy,
                DeletedDate = item.DeletedDate,
                IsActive = item.IsActive,
                FacilityName = _fRepository.Get(item.FacilityId).FacilityName,
                CorporateName = GetCorporateNameById(item.CorporateId),
                RoleName = GetRoleNameById(item.RoleId)
            }));
            list = list.GroupBy(x => new { x.RoleName, x.FacilityId }).Select(x => x.FirstOrDefault()).ToList();
            return list.OrderBy(x => x.RoleName).ToList();
        }
        private string GetRoleNameById(int roleID)
        {
            var role = _rRepository.Where(r => r.RoleID == roleID).FirstOrDefault();
            return role != null ? role.RoleName : string.Empty;
        }

        public bool IsSchedulingApplied(int roleId)
        {
            var fr = _repository.Where(f => f.IsActive && !f.IsDeleted && f.RoleId == roleId).FirstOrDefault();
            return fr != null && fr.SchedulingApplied;
        }


        public int SaveFacilityRole(FacilityRoleCustomModel vm, long userId, DateTime currentDate)
        {
            var m = _mapper.Map<FacilityRole>(vm);
            var roleName = string.IsNullOrEmpty(vm.RoleName) ? string.Empty : vm.RoleName;
            var sqlParameters = new SqlParameter[11];
            sqlParameters[0] = new SqlParameter("@FRoleId", m.FacilityRoleId);
            sqlParameters[1] = new SqlParameter("@FId", m.FacilityId);
            sqlParameters[2] = new SqlParameter("@RId", m.RoleId);
            sqlParameters[3] = new SqlParameter("@CId", m.CorporateId);
            sqlParameters[4] = new SqlParameter("@UId", userId);
            sqlParameters[5] = new SqlParameter("@AddToAll", vm.AddToAll);
            sqlParameters[6] = new SqlParameter("@IsDeleted", m.IsDeleted);
            sqlParameters[7] = new SqlParameter("@SchedulingApplied", m.SchedulingApplied);
            sqlParameters[8] = new SqlParameter("@CarePlanApplied", m.CarePlanAccessible);
            sqlParameters[9] = new SqlParameter("@RName", roleName);
            sqlParameters[10] = new SqlParameter("@CurrentDate", currentDate);

            _repository.ExecuteCommand(StoredProcedures.SprocSaveFacilityRole.ToString(), sqlParameters);
            return 2;
        }
    }
}
