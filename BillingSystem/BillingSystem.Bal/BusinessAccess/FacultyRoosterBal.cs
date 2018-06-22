// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FacultyRoosterBal.cs" company="Spadez">
//   OmniHealthcare
// </copyright>
// <summary>
//   The holiday planner bal.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Bal.BusinessAccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using BillingSystem.Bal.Mapper;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;
    using BillingSystem.Repository.GenericRepository;

    /// <summary>
    /// The holiday planner bal.
    /// </summary>
    public class FacultyRoosterBal : BaseBal
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FacultyRoosterBal"/> class.
        /// </summary>
        public FacultyRoosterBal()
        {
            this.FacultyRoosterMapper = new FacultyRoosterMapper();
            FacultyRoosterDuplicateLogMapper = new FacultyRoosterDuplicateLogMapper();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the holiday planner mapper.
        /// </summary>
        private FacultyRoosterMapper FacultyRoosterMapper { get; set; }

        /// <summary>
        /// Gets or sets the faculty rooster duplicate log mapper.
        /// </summary>
        /// <value>
        /// The faculty rooster duplicate log mapper.
        /// </value>
        private FacultyRoosterDuplicateLogMapper FacultyRoosterDuplicateLogMapper { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Gets the faculty rooster by facultyid.
        /// </summary>
        /// <param name="facultyid">The facultyid.</param>
        /// <returns></returns>
        public List<FacultyRooster> GetFacultyRoosterByFacultyid(int facultyid)
        {
            using (var facultyRoosterRep = this.UnitOfWork.FacultyRoosterRepository)
            {
                return facultyRoosterRep.Where(x => x.FacultyId == facultyid && x.ExtValue1 == "2").ToList().OrderBy(x => x.FromDate).ToList();
            }
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
            using (var rep = this.UnitOfWork.FacultyRoosterRepository)
            {
                var model = rep.Where(x => x.Id == facultyRoosterId).FirstOrDefault();
                return model;
            }
        }

        /// <summary>
        /// Gets the faculty rooster c by identifier.
        /// </summary>
        /// <param name="facultyRoosterId">The faculty rooster identifier.</param>
        /// <returns></returns>
        public FacultyRoosterCustomModel GetFacultyRoosterCById(int? facultyRoosterId)
        {
            using (var rep = this.UnitOfWork.FacultyRoosterRepository)
            {
                var model = rep.Where(x => x.Id == facultyRoosterId).FirstOrDefault();
                return FacultyRoosterMapper.MapModelToViewModel(model);
            }
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int SaveFacultyRooster(FacultyRooster model)
        {
            using (FacultyRoosterRepository rep = this.UnitOfWork.FacultyRoosterRepository)
            {
                if (model.Id > 0)
                {
                    rep.UpdateEntity(model, model.Id);
                }
                else
                {
                    rep.Create(model);
                }

                return model.Id;
            }
        }

        /// <summary>
        /// Deletes the faculty rooster.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public int DeleteFacultyRooster(int id)
        {
            using (FacultyRoosterRepository rep = this.UnitOfWork.FacultyRoosterRepository)
            {
                try
                {
                    var getmodelObj = GetFacultyRoosterById(id);
                    if (getmodelObj != null)
                    {
                        rep.Delete(getmodelObj);
                    }

                    var getFacultyOtherData = GetFacultyRoosterByFacultyid(Convert.ToInt32(getmodelObj.FacultyId));
                    if (getFacultyOtherData.Count == 0)
                    {
                        var objTodelete = rep.Where(x => x.FacultyId == getmodelObj.FacultyId).FirstOrDefault();
                        rep.Delete(objTodelete);
                    }
                    return 1;
                }
                catch (Exception)
                {
                    return -1;
                }
            }
        }

        /// <summary>
        /// Saves the faculty rooster list.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public int SaveFacultyRoosterList(List<FacultyRooster> model)
        {
            using (var rep = this.UnitOfWork.FacultyRoosterRepository)
            {
                if (model.Count > 0)
                {
                    if (model[0].Id > 0)
                    {
                        foreach (var item in model)
                        {
                            rep.UpdateEntity(item, item.Id);
                        }
                    }
                    else
                    {
                        rep.Create(model);
                    }
                    rep.CreateFacultyRecurringEvents(
                        Convert.ToInt32(model[0].FacultyId),
                        Convert.ToInt32(model[0].FacilityId),
                        Convert.ToInt32(model[0].CorporateId));

                    return model[0].Id;
                }

                return -1;
            }
        }

        /// <summary>
        /// Gets the faculty rooster by facility.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public List<FacultyRoosterCustomModel> GetFacultyRoosterByFacility(int facilityId)
        {
            var list = new List<FacultyRoosterCustomModel>();
            using (var facultyRoosterRep = this.UnitOfWork.FacultyRoosterRepository)
            {
                var lstFacultyRooster = facultyRoosterRep.Where(x => x.FacilityId == facilityId && x.ExtValue1 == "1" && x.IsActive == true).ToList();
                if (lstFacultyRooster.Count > 0)
                {
                    lstFacultyRooster = lstFacultyRooster.GroupBy(x => x.FacultyId).Select(g => g.First()).ToList();
                    list.AddRange(lstFacultyRooster.Select(item => FacultyRoosterMapper.MapModelToViewModel(item)));
                }
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
            using (var facultyRoosterRep = this.UnitOfWork.FacultyRoosterRepository)
            {
              var result = facultyRoosterRep.CheckForDuplicateRecord(
                    (model.FromDate.Value.ToString("MM-dd-yyyy HH:mm:ss")),
                    (model.ToDate.Value.ToString("MM-dd-yyyy HH:mm:ss")),
                    Convert.ToInt32(model.FacultyId),
                    Convert.ToInt32(model.DeptId),
                    Convert.ToInt32(model.Id));

                return result.Count > 0 ? result[0].TimeSlotAvailable : 0;
            }
        }


        /// <summary>
        /// Duplicates the entry log.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public FacultyRoosterLogCustomModel DuplicateEntryLog(FacultyRooster model, int type)
        {
            var notavialabletimeslot = FacultyRoosterDuplicateLogMapper.MapModelToViewModel(model, type);
            return notavialabletimeslot;
        }

        #endregion
    }
}
