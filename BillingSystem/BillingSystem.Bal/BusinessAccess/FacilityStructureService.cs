
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using AutoMapper;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;



namespace BillingSystem.Bal.BusinessAccess
{
    /// <summary>
    /// The facility structure bal.
    /// </summary>
    public class FacilityStructureService : IFacilityStructureService
    {

        private readonly IRepository<FacilityStructure> _repository;
        private readonly IRepository<AppointmentTypes> _aRepository;
        private readonly IRepository<Facility> _fRepository;
        private readonly BillingEntities _context;
        private readonly IMapper _mapper;

        public FacilityStructureService(IRepository<FacilityStructure> repository, IRepository<AppointmentTypes> aRepository, IRepository<Facility> fRepository, BillingEntities context, IMapper mapper)
        {
            _repository = repository;
            _aRepository = aRepository;
            _fRepository = fRepository;
            _context = context;
            _mapper = mapper;
        }


        #region Public Methods and Operators

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="facilityStructure">The facility structure.</param>
        /// <returns>
        /// The <see cref="int" />.
        /// </returns>
        public int AddUptdateFacilityStructure(FacilityStructure facilityStructure)
        {
            if (facilityStructure.FacilityStructureId > 0)
            {
                if (facilityStructure.GlobalCodeID == 84)
                {
                    var appointmentId = GetFacilityStructureById(facilityStructure.FacilityStructureId);
                    facilityStructure.ExternalValue4 = appointmentId.ExternalValue4;
                }
                _repository.UpdateEntity(facilityStructure, facilityStructure.FacilityStructureId);
            }
            else
            {
                _repository.Create(facilityStructure);
            }

            return facilityStructure.FacilityStructureId;

        }

        /// <summary>
        /// Checks for childrens.
        /// </summary>
        /// <param name="facilityStructureId">The facility structure identifier.</param>
        /// <param name="globalCodeId">The global code identifier.</param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        public bool CheckForChildrens(int facilityStructureId)
        {
            var iQueryabletransactions = _repository.Where(a => a.ParentId == facilityStructureId && a.IsDeleted == false).FirstOrDefault();
            return iQueryabletransactions != null;
        }

        /// <summary>
        /// Checks the structure exist.
        /// </summary>
        /// <param name="facilityStructure">
        /// The facility structure.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CheckStructureExist(FacilityStructure facilityStructure)
        {
            var iQueryabletransactions = _repository.Where(a => a.FacilityStructureId != facilityStructure.FacilityStructureId && a.FacilityId.Equals(facilityStructure.FacilityId) && a.GlobalCodeID == facilityStructure.GlobalCodeID && a.FacilityStructureName.ToLower().Equals(facilityStructure.FacilityStructureName.ToLower()) && a.IsDeleted == false).FirstOrDefault();
            return iQueryabletransactions != null;
        }

        /// <summary>
        /// Deletes the facility structure by identifier.
        /// </summary>
        /// <param name="facilityStructureId">
        /// The facility structure identifier.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool DeleteFacilityStructureById(int facilityStructureId)
        {
            var bedMaster = _repository.Where(x => x.FacilityStructureId == facilityStructureId).FirstOrDefault();
            return _repository.Delete(bedMaster) > 0;
        }


        /// <summary>
        /// Gets the facility beds.
        /// </summary>
        /// <param name="facilityId">
        /// The facility identifier.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<FacilityStructure> GetFacilityBeds(string facilityId)
        {
            const int FacilityStructureId = 85;
            var facilityStructure = _repository.Where(x => x.GlobalCodeID == FacilityStructureId && x.FacilityId == facilityId && x.IsActive && !(bool)x.IsDeleted).ToList();
            return facilityStructure;
        }

        /// <summary>
        /// Gets the facility departments.
        /// </summary>
        /// <param name="corporateid">
        /// The corporateid.
        /// </param>
        /// <param name="facilityid">
        /// The facilityid.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<FacilityStructure> GetFacilityDepartments(int corporateid, string facilityid)
        {
            var departmentTypes = Convert.ToInt32(BaseFacilityStucture.Department);
            var returnLst = _repository.Where(x => x.FacilityId == facilityid && x.GlobalCodeID == departmentTypes && x.IsDeleted == false).OrderBy(x => x.SortOrder).ToList();
            return returnLst;
        }

        /// <summary>
        /// Gets the facility rooms.
        /// </summary>
        /// <param name="corporateid">The corporateid.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <returns></returns>
        public List<FacilityStructure> GetFacilityRooms(int corporateid, string facilityid)
        {
            var roomTypes = Convert.ToInt32(BaseFacilityStucture.Rooms);
            var returnLst = _repository.Where(x => x.FacilityId == facilityid && x.GlobalCodeID == roomTypes && x.IsDeleted == false).OrderBy(x => x.SortOrder).ToList();
            return returnLst;
        }


        /// <summary>
        /// Gets the department rooms.
        /// </summary>
        /// <param name="deptIds">The dept ids.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<FacilityStructure> GetDepartmentRooms(List<SchedularFiltersCustomModel> deptIds, string facilityId)
        {
            var roomTypes = Convert.ToInt32(BaseFacilityStucture.Rooms);
            var returnLst = _repository.Where(x => x.FacilityId == facilityId && x.GlobalCodeID == roomTypes && x.IsDeleted == false).OrderBy(x => x.SortOrder).ToList();
            if (deptIds.All(x => x.Id != 0))
            {
                returnLst = returnLst.Where(x => deptIds.Any(dp => dp.Id == x.ParentId)).ToList();
            }
            return returnLst;
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <param name="facilityId">
        /// The facility Id.
        /// </param>
        /// <returns>
        /// Return the Entity List
        /// </returns>
        public List<FacilityStructureCustomModel> GetFacilityStructure(string facilityId)
        {
            try
            {
                var vmlist = new List<FacilityStructureCustomModel>();
                var list = _repository.Where(a => a.FacilityId == facilityId && (a.IsDeleted == null || !(bool)a.IsDeleted) && a.IsActive).OrderByDescending(_ => _.FacilityStructureId).ToList();

                vmlist = list.Select(x => _mapper.Map<FacilityStructureCustomModel>(x)).ToList();
                //lstFacilityStructureCustomModel.AddRange(
                //    lstFacilityStructure.Select(item => FacilityStructureMapper.MapModelToViewModel(item)));
                return vmlist;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <param name="facilityId">
        /// The facility Id.
        /// </param>
        /// <returns>
        /// Return the Entity List
        /// </returns>
        public List<FacilityStructure> GetFacilityStructureForDDL(string facilityId)
        {
            try
            {
                var lst = _repository.Where(a => a.FacilityId == facilityId && (a.IsDeleted == null || !(bool)a.IsDeleted) && a.IsActive).OrderByDescending(_ => _.FacilityStructureId).ToList();
                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the facility structure bread crumbs.
        /// </summary>
        /// <param name="facilityStructureId">
        /// The facility structure identifier.
        /// </param>
        /// <param name="facilityid">
        /// The facilityid.
        /// </param>
        /// <param name="ParentId">
        /// The parent identifier.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetFacilityStructureBreadCrumbs(int facilityStructureId, string facilityid, string ParentId)
        {
            var _facilityStructureId = string.Empty;

            _facilityStructureId = _fRepository.Get(Convert.ToInt32(facilityid)).FacilityName;


            switch (facilityStructureId)
            {
                case 82:
                    _facilityStructureId += " : " + "Floor";
                    break;
                case 83:
                    _facilityStructureId += " : " + "Department";
                    break;
                case 84:
                    _facilityStructureId += " : " + "Room";
                    break;
                case 85:
                    _facilityStructureId += " : " + "Bed";
                    break;
            }

            return _facilityStructureId;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="facilityStructureId">
        /// </param>
        /// <returns>
        /// The <see cref="FacilityStructure"/>.
        /// </returns>
        public FacilityStructure GetFacilityStructureById(int? facilityStructureId)
        {
            var lst = _repository.Where(x => x.FacilityStructureId == facilityStructureId).FirstOrDefault();
            return lst;
        }

        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <param name="facilityId">
        /// The facility Id.
        /// </param>
        /// <param name="structureId">
        /// The structure Id.
        /// </param>
        /// <returns>
        /// Return the Entity List
        /// </returns>
        public List<FacilityStructureCustomModel> GetFacilityStructureCustom(string facilityId, int structureId)
        {
            try
            {
                var vmlst = new List<FacilityStructureCustomModel>();
                var lst = _repository.Where(a => (facilityId == "0" || a.FacilityId == facilityId) && (structureId == 0 || a.GlobalCodeID == structureId) && (a.IsDeleted == null || !(bool)a.IsDeleted) && a.IsActive).OrderByDescending(_ => _.GlobalCodeID).ThenBy(_ => _.SortOrder).ToList();

                //vmlst.AddRange(lstFacilityStructure.Select(
                //    item => FacilityStructureMapper.MapModelToViewModel(item)));
                vmlst = lst.Select(x => _mapper.Map<FacilityStructureCustomModel>(x)).ToList();
                vmlst = vmlst.OrderBy(x => x.GlobalCodeID).ThenBy(x => x.SortOrder).ToList();


                return vmlst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get Entity Name By Id
        /// </summary>
        /// <param name="facilityStructureId">
        /// The facility Structure Id.
        /// </param>
        /// <returns>
        /// Return the Entity Respository
        /// </returns>
        public string GetFacilityStructureNameById(int? facilityStructureId)
        {
            var m = _repository.Where(a => a.FacilityStructureId == facilityStructureId).FirstOrDefault();
            return (m != null) ? m.FacilityStructureName : string.Empty;
        }

        /// <summary>
        /// Gets the facility structure parent.
        /// </summary>
        /// <param name="facilityStructureId">
        /// The facility structure identifier.
        /// </param>
        /// <param name="facilityid">
        /// The facilityid.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<FacilityStructureCustomModel> GetFacilityStructureParent(int facilityStructureId, string facilityid)
        {
            var _facilityStructureId = facilityStructureId;
            switch (facilityStructureId)
            {
                case 82:
                    _facilityStructureId = 0;
                    break;
                case 83:
                    _facilityStructureId = 82;
                    break;
                case 84:
                    _facilityStructureId = 83;
                    break;
                case 85:
                    _facilityStructureId = 84;
                    break;
            }

            var customlist = GetfacilityStructureData(Convert.ToInt32(facilityid),
                _facilityStructureId, true);
            return customlist;
        }

        /// <summary>
        /// Gets the in active facility structure custom list.
        /// </summary>
        /// <param name="facilityId">
        /// The facility identifier.
        /// </param>
        /// <param name="structureId">
        /// The structure identifier.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<FacilityStructureCustomModel> GetInActiveFacilityStructureCustomList(
            string facilityId,
            int structureId)
        {
            try
            {
                var vmlst = new List<FacilityStructureCustomModel>();
                var lst = _repository.Where(a => (facilityId == "0" || a.FacilityId == facilityId) && (structureId == 0 || a.GlobalCodeID == structureId) && (a.IsDeleted == null || !(bool)a.IsDeleted) && a.IsActive == false).OrderByDescending(_ => _.GlobalCodeID).ThenBy(_ => _.SortOrder).ToList();

                vmlst = lst.Select(x => _mapper.Map<FacilityStructureCustomModel>(x)).ToList();
                //vmlst.AddRange(
                //    lst.Select(item => FacilityStructureMapper.MapModelToViewModel(item)));

                vmlst =
                    vmlst.OrderBy(x => x.GlobalCodeID).ThenBy(x => x.SortOrder).ToList();


                return vmlst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the maximum sort order.
        /// </summary>
        /// <param name="facilityId">
        /// The facility identifier.
        /// </param>
        /// <param name="structureType">
        /// Type of the structure.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetMaxSortOrder(string facilityId, string structureType)
        {
            var sType = Convert.ToInt32(structureType);
            var result = _repository.Where(
                    f => f.FacilityId.Equals(facilityId) && f.GlobalCodeID != null && (int)f.GlobalCodeID == sType)
                    .Max(m => m.SortOrder);
            return result != null ? Convert.ToInt32(result) + 1 : 1;
        }

        /// <summary>
        /// Gets the department rooms.
        /// </summary>
        /// <param name="deptId">The dept identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<FacilityStructure> GetDepartmentRooms(int deptId, string facilityId)
        {
            var roomTypes = Convert.ToInt32(BaseFacilityStucture.Rooms);
            var returnLst = _repository.Where(x => x.FacilityId == facilityId && x.GlobalCodeID == roomTypes && x.ParentId == deptId && x.IsDeleted == false)
                .OrderBy(x => x.SortOrder).ToList();
            return returnLst;
        }

        /// <summary>
        /// Gets the department appointment types.
        /// </summary>
        /// <param name="deptId">The dept identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<AppointmentTypes> GetDepartmentAppointmentTypes(int deptId, string facilityId)
        {
            var roomTypes = Convert.ToInt32(BaseFacilityStucture.Rooms);
            var returnLst =
                _repository.Where(
                    x => !string.IsNullOrEmpty(x.ExternalValue4) &&
                         x.FacilityId == facilityId && x.GlobalCodeID == roomTypes && x.ParentId == deptId &&
                         x.IsDeleted == false).Select(a => a.ExternalValue4).ToList();

            if (returnLst.Any())
            {
                var ids = string.Join(",", returnLst).Split(',').Select(int.Parse).ToList();
                if (ids.Count > 0)
                {
                    var list = _aRepository.Where(x => x.FacilityId == Convert.ToInt32(facilityId) && x.IsActive && ids.Contains(x.Id)).ToList();// _aRepository.GetAppointmentTypesByFacilityId(Convert.ToInt32(facilityId), appTypesArray);
                    return list;
                }
            }
            return new List<AppointmentTypes>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the parent name by identifier.
        /// </summary>
        /// <param name="parentId">
        /// The parent identifier.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetParentNameById(int parentId)
        {
            var model = _repository.Where(a => a.FacilityStructureId == parentId).FirstOrDefault();
            return (model != null) ? model.FacilityStructureName : string.Empty;
        }


        public string GetParentNameByFacilityStructureId(int facilityStructureId)
        {
            var result = string.Empty;
            var model = _repository.Where(x => x.FacilityStructureId == facilityStructureId).FirstOrDefault();
            if (model != null)
            {
                var parent = _repository.Where(m => m.FacilityStructureId == model.ParentId).FirstOrDefault();
                if (parent != null)
                    result = parent.FacilityStructureName;
            }
            return result;
        }

        public List<FacilityStructure> GetRoomsByFacilityId(string facilityid)
        {
            var departmentTypes = Convert.ToInt32(BaseFacilityStucture.Rooms);
            var returnLst = _repository.Where(x => x.FacilityId == facilityid && x.GlobalCodeID == departmentTypes && x.IsDeleted == false).OrderByDescending(x => x.SortOrder).ToList();

            return returnLst;
        }

        #endregion

        public List<FacilityStructureCustomModel> GetAppointRoomAssignmentsList(string facilityId)
        {
            var fList = new List<FacilityStructureCustomModel>();
            var list = _repository.Where(f => f.FacilityId.Trim().Equals(facilityId) && f.IsActive && f.IsDeleted != true && f.GlobalCodeID == 84).OrderBy(f1 => f1.FacilityStructureName).ToList();
            var fName = _fRepository.Get(Convert.ToInt32(facilityId)).FacilityName;

            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    var appList = new List<AppointmentTypes>();
                    if (!string.IsNullOrEmpty(item.ExternalValue4))
                    {
                        var appointmentTypeIds = item.ExternalValue4.Trim();
                        if (!string.IsNullOrEmpty(appointmentTypeIds))
                        {
                            var intList = appointmentTypeIds.Split(',').Select(int.Parse).ToList();
                            appList = _aRepository.Where(f => intList.Contains(f.Id) && f.IsActive).ToList();

                        }
                    }

                    var newItem = new FacilityStructureCustomModel
                    {
                        AppointmentList = appList,
                        FacilityStructureId = item.FacilityStructureId,
                        FacilityStructureName = item.FacilityStructureName,
                        FacilityId = item.FacilityId,
                        FacilityName = fName,//GetFacilityNameByFacilityId(Convert.ToInt32(item.FacilityId)),
                        RoomDepartment = GetParentNameByFacilityStructureId(item.FacilityStructureId)
                    };

                    fList.Add(newItem);
                }
            }
            return fList;
        }



        public List<FacilityStructureCustomModel> GetfacilityStructureData(int facilityId, int structureId, bool showIsActive)
        {
            var spName = string.Format("EXEC {0} @pFacilityId, @pStructureId,@pShowIsActive", StoredProcedures.SPROC_GetFacilityStructureData);
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("pFacilityId", facilityId);
            sqlParameters[1] = new SqlParameter("pStructureId", structureId);
            sqlParameters[2] = new SqlParameter("pShowIsActive", showIsActive);
            IEnumerable<FacilityStructureCustomModel> result = _context.Database.SqlQuery<FacilityStructureCustomModel>(spName, sqlParameters);
            return result.ToList();
        }



        public List<DropdownListData> GetFacilityStructureListByParentId(int parentId)
        {
            var list = new List<DropdownListData>();

            var fsList = _repository.Where(f => f.GlobalCodeID == parentId && f.IsActive && f.IsDeleted != true).ToList();

            if (fsList.Any())
            {
                list.AddRange(fsList.Select(item => new DropdownListData
                {
                    Text = item.FacilityStructureName,
                    Value = Convert.ToString(item.FacilityStructureId)
                }));
            }

            return list;
        }



        /// <summary>
        /// Gets the facility rooms custom model.
        /// </summary>
        /// <param name="corporateid">The corporateid.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <returns></returns>
        public List<FacilityStructureRoomsCustomModel> GetFacilityRoomsCustomModel(int corporateid, string facilityid)
        {
            var roomTypes = Convert.ToInt32(BaseFacilityStucture.Rooms);
            var spName = string.Format("EXEC {0} @pFacilityId, @pStructureId", StoredProcedures.SPROC_GetFacilityStructueChildParent);
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("pFacilityId", facilityid);
            sqlParameters[1] = new SqlParameter("pStructureId", roomTypes);
            IEnumerable<FacilityStructureRoomsCustomModel> result = _context.Database.SqlQuery<FacilityStructureRoomsCustomModel>(spName, sqlParameters);
            return result.ToList();
        }


        public List<DropdownListData> GetRevenueDepartments(int corporateid, string facilityid)
        {
            var list = new List<DropdownListData>();
            var departmentTypes = (int)BaseFacilityStucture.Department;
            var returnLst = _repository.Where(x => x.FacilityId == facilityid && !string.IsNullOrEmpty(x.ExternalValue1) && x.GlobalCodeID == departmentTypes && x.IsDeleted == false)
                .OrderBy(x => x.SortOrder);

            if (returnLst.Any())
            {
                list.AddRange(returnLst.Select(item => new DropdownListData
                {
                    Value = item.ExternalValue1,
                    Text = item.ExternalValue1
                                                + @" (Department Name :" + item.FacilityStructureName
                                                + @" )",
                    ExternalValue1 = item.FacilityStructureName
                }));
            }
            return list;
        }


        public List<FacilityStructureRoomsCustomModel> GetFacilityRoomsByDepartments(int corporateid, string facilityid, string depIds, string roomIds)
        {
            var structureId = Convert.ToInt32(BaseFacilityStucture.Rooms);
            var spName = string.Format("EXEC {0} @pFacilityId, @pStructureId, @pDeptIds, @pRoomIds", StoredProcedures.SPROC_GetRoomsByDepartments);
            var sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter("pFacilityId", facilityid);
            sqlParameters[1] = new SqlParameter("pStructureId", structureId);
            sqlParameters[2] = new SqlParameter("pDeptIds", depIds);
            sqlParameters[3] = new SqlParameter("pRoomIds", roomIds);
            IEnumerable<FacilityStructureRoomsCustomModel> result = _context.Database.SqlQuery<FacilityStructureRoomsCustomModel>(spName, sqlParameters);
            return result.ToList();
        }

        public List<FacilityStructureCustomModel> GetAppointRoomAssignmentsList(int facilityId, string txtSearch)
        {
            var searchText = string.IsNullOrEmpty(txtSearch) ? string.Empty : txtSearch;
            var spName = string.Format("EXEC {0} @pFId, @pSearch", StoredProcedures.SPROC_GetAppointmentRoomsAssignements);
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("pFId", facilityId);
            sqlParameters[1] = new SqlParameter("pSearch", searchText);

            using (var multiResultSet = _context.MultiResultSetSqlQuery(spName, parameters: sqlParameters))
            {
                var fList = multiResultSet.ResultSetFor<FacilityStructureCustomModel>().ToList();
                var appList = multiResultSet.ResultSetFor<AppointmentTypesCustomModel>().ToList();

                fList = fList.Select(fs => { var aList = appList.Where(a => int.Parse(a.ExtValue2) == fs.FacilityStructureId).ToList(); fs.AppointmentList = new List<AppointmentTypes>(aList); return fs; }).ToList();

                return fList;
            }
        }


        public List<AppointmentTypesCustomModel> GetDepartmentAppointmentTypes(int deptId, int facilityId, int cId, bool active)
        {
            var list = new List<AppointmentTypesCustomModel>();
            var spName = string.Format("EXEC {0} @FacilityId,@CorporateId, @ShowInActive", StoredProcedures.SPORC_GetAppointmentTypes);
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("FacilityId", facilityId);
            sqlParameters[1] = new SqlParameter("CorporateId", cId);
            sqlParameters[2] = new SqlParameter("ShowInActive", active);
            IEnumerable<AppointmentTypesCustomModel> result = _context.Database.SqlQuery<AppointmentTypesCustomModel>(spName, sqlParameters);
            list = result.ToList();


            var fIdStr = Convert.ToString(facilityId);

            var roomTypes = Convert.ToInt32(BaseFacilityStucture.Rooms);
            var returnLst = _repository.Where(x => !string.IsNullOrEmpty(x.ExternalValue4) && x.FacilityId.Equals(fIdStr) && x.GlobalCodeID == roomTypes && x.ParentId == deptId && x.IsDeleted == false).Select(a => a.ExternalValue4).ToList();

            if (returnLst.Any())
            {
                var appTypesArray = string.Join(",", returnLst).Split(',').Select(int.Parse).ToList();
                if (appTypesArray.Count > 0)
                    list = list.Where(a => appTypesArray.Contains(a.Id)).ToList();

            }
            return list;
        }
    }
}