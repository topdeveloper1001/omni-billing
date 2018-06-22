// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhysicianRepository.cs" company="SPadez">
//   OmniHealth Care
// </copyright>
// <summary>
//   The physician repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Repository.GenericRepository
{
    using System;
    using System.Data.Entity;
    using System.Data.SqlClient;

    using BillingSystem.Common.Common;
    using BillingSystem.Model;
    using System.Collections.Generic;
    using BillingSystem.Repository.Common;
    using System.Threading.Tasks;

    /// <summary>
    /// The physician repository.
    /// </summary>
    public class PhysicianRepository : GenericRepository<Physician>
    {
        #region Fields

        /// <summary>
        /// The _context.
        /// </summary>
        private readonly DbContext _context;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PhysicianRepository"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public PhysicianRepository(BillingEntities context)
            : base(context)
        {
            this.AutoSave = true;
            this._context = context;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The add faculty lunch timmings.
        /// </summary>
        /// <param name="corporateId">
        /// The corporate id.
        /// </param>
        /// <param name="facilityId">
        /// The facility id.
        /// </param>
        /// <param name="userid">
        /// The userid.
        /// </param>
        /// <param name="departmentid">
        /// The departmentid.
        /// </param>
        /// <param name="lunchTimefrom">
        /// The lunch timefrom.
        /// </param>
        /// <param name="lunchTimeTill">
        /// The lunch time till.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool AddFacultyLunchTimmings(
            int corporateId,
            int facilityId,
            int userid,
            int departmentid,
            string lunchTimefrom,
            string lunchTimeTill)
        {
            try
            {
                if (this._context != null)
                {
                    string spName =
                        string.Format(
                            "EXEC {0}  @pCID ,@pFID,@pUserId,@pDeptId,@pLunchTimeFrom,@pLunchTimeTill",
                            StoredProcedures.SPROC_AddUpdateFacultyLunchTimming);
                    var sqlParameters = new SqlParameter[6];
                    sqlParameters[0] = new SqlParameter("pCID", corporateId);
                    sqlParameters[1] = new SqlParameter("pFID", facilityId);
                    sqlParameters[2] = new SqlParameter("pUserId", userid);
                    sqlParameters[3] = new SqlParameter("pDeptId", departmentid);
                    sqlParameters[4] = new SqlParameter("pLunchTimeFrom", lunchTimefrom);
                    sqlParameters[5] = new SqlParameter("pLunchTimeTill", lunchTimeTill);
                    this.ExecuteCommand(spName, sqlParameters);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

        /// <summary>
        /// Adds the facultydefault timmings.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="userid">The userid.</param>
        /// <param name="departmentid">The departmentid.</param>
        /// <param name="userType">Type of the user.</param>
        /// <returns></returns>
        public bool AddFacultydefaultTimmings(
            int corporateId,
            int facilityId,
            int userid,
            int departmentid,
            int userType)
        {
            try
            {
                if (this._context != null)
                {
                    string spName =
                        string.Format(
                            "EXEC {0}  @pFacultyId ,@pUserType,@pDeptId,@pFId,@pCId",
                            StoredProcedures.SPROC_AddFacultyDefaultTiming);
                    var sqlParameters = new SqlParameter[5];
                    sqlParameters[0] = new SqlParameter("pFacultyId", userid);
                    sqlParameters[1] = new SqlParameter("pUserType", userType);
                    sqlParameters[2] = new SqlParameter("pDeptId", departmentid);
                    sqlParameters[3] = new SqlParameter("pFId", facilityId);
                    sqlParameters[4] = new SqlParameter("pCId", corporateId);
                    this.ExecuteCommand(spName, sqlParameters);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }


        public async Task<List<PhysicianViewModel>> GetPhysiciansByFacility(int facilityId, long loggedInUserId)
        {
            try
            {
                var sqlParameters = new SqlParameter[2];
                sqlParameters[0] = new SqlParameter("@FId", facilityId);
                sqlParameters[1] = new SqlParameter("@UserId", loggedInUserId);

                using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SPROC_GetPhysiciansByFacility.ToString(), false, parameters: sqlParameters))
                {
                    var result = (await r.ResultSetForAsync<PhysicianViewModel>()).ToList();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<PhysicianViewModel> GetPhysicians(int facilityId, int corporateId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @CId, @FId", StoredProcedures.SPROC_GetPhysicianRoleWise);
                    var sqlParameters = new object[2];
                    sqlParameters[0] = new SqlParameter("CId", corporateId);
                    sqlParameters[1] = new SqlParameter("FId", facilityId);
                    IEnumerable<PhysicianViewModel> result = _context.Database.SqlQuery<PhysicianViewModel>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }




        public List<PhysicianViewModel> GetPhysiciansWithSchedulingApplied(int corporateId, bool isadmin, int userid, int facilityId)
        {
            try
            {
                if (_context != null)
                {
                    var spName = string.Format("EXEC {0} @CId, @FId, @IsAdmin, @UId", StoredProcedures.SPROC_GetPhysiciansWithSchedulingApplied);
                    var sqlParameters = new object[4];
                    sqlParameters[0] = new SqlParameter("CId", corporateId);
                    sqlParameters[1] = new SqlParameter("FId", facilityId);
                    sqlParameters[2] = new SqlParameter("IsAdmin", facilityId);
                    sqlParameters[3] = new SqlParameter("UId", facilityId);
                    var result = _context.Database.SqlQuery<PhysicianViewModel>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }
        #endregion
    }
}