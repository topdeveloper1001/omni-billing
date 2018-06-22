// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FacultyRoosterRepository.cs" company="Spadez">
//   Omnihealthcare
// </copyright>
// <summary>
//   The faculty rooster repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Repository.GenericRepository
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.SqlClient;
    using System.Linq;

    using BillingSystem.Common.Common;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;

    /// <summary>
    /// The faculty rooster repository.
    /// </summary>
    public class FacultyRoosterRepository : GenericRepository<FacultyRooster>
    {
        #region Fields

        /// <summary>
        /// The _context.
        /// </summary>
        private readonly DbContext _context;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FacultyRoosterRepository"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public FacultyRoosterRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Creates the faculty recurring events.
        /// </summary>
        /// <param name="phyId">The phy identifier.</param>
        /// <param name="fId">The f identifier.</param>
        /// <param name="cId">The c identifier.</param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        public bool CreateFacultyRecurringEvents(int phyId,int fId, int cId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @pPhyId,@pFId,@pCId ", StoredProcedures.SPROC_CreateRecurringEventsFaculty);
                    var sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("pPhyId", phyId);
                    sqlParameters[1] = new SqlParameter("pFId", fId);
                    sqlParameters[2] = new SqlParameter("pCId", cId);
                    ExecuteCommand(spName, sqlParameters);
                    return true;
                }
            }
            catch (Exception )
            {
                // throw ex;
            }

            return false;
        }


        /// <summary>
        /// Checks for duplicate record.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="pFacultyid">The p facultyid.</param>
        /// <param name="pDeptId">The p dept identifier.</param>
        /// <param name="pid">The pid.</param>
        /// <returns></returns>
        public List<TimeSlotAvailabilityCustomModel> CheckForDuplicateRecord(
           string startDate, string endDate, int pFacultyid, int pDeptId, int pid)
        {
            try
            {
                var spName =
                        string.Format(
                            "EXEC {0} @pFacultyId, @pDeptId, @pDateFrom, @pDateTo,@pId",
                            StoredProcedures.SPROC_CheckForDuplicateRecordFaculty);
                var sqlParameters = new SqlParameter[5];
                sqlParameters[0] = new SqlParameter("pFacultyId", pFacultyid);
                sqlParameters[1] = new SqlParameter("pDeptId", pDeptId);
                sqlParameters[2] = new SqlParameter("pDateFrom", startDate);
                sqlParameters[3] = new SqlParameter("pDateTo", endDate);
                sqlParameters[4] = new SqlParameter("pId", pid);
                IEnumerable<TimeSlotAvailabilityCustomModel> result = _context.Database.SqlQuery<TimeSlotAvailabilityCustomModel>(
                    spName, sqlParameters);
                return result.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #endregion
    }
}