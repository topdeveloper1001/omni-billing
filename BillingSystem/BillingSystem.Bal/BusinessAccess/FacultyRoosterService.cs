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
    /// The holiday planner bal.
    /// </summary>
    public class FacultyRoosterService : IFacultyRoosterService
    {
        private readonly IRepository<FacultyRooster> _repository;
        private readonly BillingEntities _context;
        private readonly IMapper _mapper;

        public FacultyRoosterService(IRepository<FacultyRooster> repository, BillingEntities context, IMapper mapper)
        {
            _repository = repository;
            _context = context;
            _mapper = mapper;
        }


        #region Public Methods and Operators

        /// <summary>
        /// Gets the faculty rooster by facultyid.
        /// </summary>
        /// <param name="facultyid">The facultyid.</param>
        /// <returns></returns>
        public List<FacultyRooster> GetFacultyRoosterByFacultyid(int facultyid)
        {
            return _repository.Where(x => x.FacultyId == facultyid && x.ExtValue1 == "2").ToList().OrderBy(x => x.FromDate).ToList();
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="facultyRoosterId">The Holiday Planner Id.</param>
        /// <returns>
        /// The <see cref="FacultyRooster" />.
        /// </returns>
        public FacultyRooster GetFacultyRoosterById(int? facultyRoosterId)
        {
            var model = _repository.Where(x => x.Id == facultyRoosterId).FirstOrDefault();
            return model;
        }

        /// <summary>
        /// Gets the faculty rooster c by identifier.
        /// </summary>
        /// <param name="facultyRoosterId">The faculty rooster identifier.</param>
        /// <returns></returns>
        public FacultyRoosterCustomModel GetFacultyRoosterCById(int? facultyRoosterId)
        {
            var model = _repository.Where(x => x.Id == facultyRoosterId).FirstOrDefault();
            return _mapper.Map<FacultyRoosterCustomModel>(model);
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="m">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int SaveFacultyRooster(FacultyRooster m)
        {
            if (m.Id > 0)
            {
                _repository.UpdateEntity(m, m.Id);
            }
            else
            {
                _repository.Create(m);
            }

            return m.Id;
        }

        /// <summary>
        /// Deletes the faculty rooster.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public int DeleteFacultyRooster(int id)
        {
            try
            {
                var getmodelObj = GetFacultyRoosterById(id);
                if (getmodelObj != null)
                {
                    _repository.Delete(getmodelObj);
                }

                var getFacultyOtherData = GetFacultyRoosterByFacultyid(Convert.ToInt32(getmodelObj.FacultyId));
                if (getFacultyOtherData.Count == 0)
                {
                    var objTodelete = _repository.Where(x => x.FacultyId == getmodelObj.FacultyId).FirstOrDefault();
                    _repository.Delete(objTodelete);
                }
                return 1;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// Saves the faculty rooster list.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public int SaveFacultyRoosterList(List<FacultyRooster> model)
        {
            if (model.Count > 0)
            {
                if (model[0].Id > 0)
                {
                    foreach (var item in model)
                    {
                        _repository.UpdateEntity(item, item.Id);
                    }
                }
                else
                    _repository.Create(model);
                var spName = string.Format("EXEC {0} @pPhyId,@pFId,@pCId ", StoredProcedures.SPROC_CreateRecurringEventsFaculty);
                var sqlParameters = new SqlParameter[3];
                sqlParameters[0] = new SqlParameter("pPhyId", model[0].FacultyId);
                sqlParameters[1] = new SqlParameter("pFId", model[0].FacilityId);
                sqlParameters[2] = new SqlParameter("pCId", model[0].CorporateId);
                _repository.ExecuteCommand(spName, sqlParameters);


                return model[0].Id;
            }

            return -1;
        }

        /// <summary>
        /// Gets the faculty rooster by facility.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<FacultyRoosterCustomModel> GetFacultyRoosterByFacility(int facilityId)
        {
            var list = new List<FacultyRoosterCustomModel>();
            var lstFacultyRooster = _repository.Where(x => x.FacilityId == facilityId && x.ExtValue1 == "1" && x.IsActive == true).ToList();
            if (lstFacultyRooster.Count > 0)
            {
                lstFacultyRooster = lstFacultyRooster.GroupBy(x => x.FacultyId).Select(g => g.First()).ToList();
                list.AddRange(lstFacultyRooster.Select(item => _mapper.Map<FacultyRoosterCustomModel>(item)));
            }

            return list.ToList();
        }

        /// <summary>
        /// Checks for duplicate entry.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public int CheckForDuplicateEntry(FacultyRooster model)
        {
            var spName =
                           string.Format(
                               "EXEC {0} @pFacultyId, @pDeptId, @pDateFrom, @pDateTo,@pId",
                               StoredProcedures.SPROC_CheckForDuplicateRecordFaculty);
            var sqlParameters = new SqlParameter[5];
            sqlParameters[0] = new SqlParameter("pFacultyId", model.FacultyId);
            sqlParameters[1] = new SqlParameter("pDeptId", model.DeptId);
            sqlParameters[2] = new SqlParameter("pDateFrom", model.FromDate.Value.ToString("MM-dd-yyyy HH:mm:ss"));
            sqlParameters[3] = new SqlParameter("pDateTo", model.ToDate.Value.ToString("MM-dd-yyyy HH:mm:ss"));
            sqlParameters[4] = new SqlParameter("pId", model.Id);
            IEnumerable<TimeSlotAvailabilityCustomModel> result = _context.Database.SqlQuery<TimeSlotAvailabilityCustomModel>(
                spName, sqlParameters);

            var r = result.ToList();

            return r.Count > 0 ? r[0].TimeSlotAvailable : 0;
        }


        /// <summary>
        /// Duplicates the entry log.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public FacultyRoosterLogCustomModel DuplicateEntryLog(FacultyRooster model, int type)
        {
            var vm = _mapper.Map<FacultyRoosterLogCustomModel>(model);
            vm.Reason = type == 1
                                   ? "User already have schedule between selected dates for selected department. From Date : "
                                     + model.FromDate.Value.ToString("MM-dd-yyyy HH:mm") + " ,Till Date : "
                                     + model.ToDate.Value.ToString("MM-dd-yyyy HH:mm") + "."
                                   : type == 2
                                         ? "User already have schedule between selected dates. From Date : "
                                           + model.FromDate.Value.ToString("MM-dd-yyyy HH:mm") + " ,Till Date : "
                                           + model.ToDate.Value.ToString("MM-dd-yyyy HH:mm") + "."
                                         : type == 3
                                               ? "User have lunch timming between selected date time range. From Date : "
                                                 + model.FromDate.Value.ToString("MM-dd-yyyy HH:mm") + " ,Till Date : "
                                                 + model.ToDate.Value.ToString("MM-dd-yyyy HH:mm") + "."
                                               : string.Empty;
            return vm;
        }

        #endregion
    }
}
