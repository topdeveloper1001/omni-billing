// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PatientCarePlanRepository.cs" company="Spadez">
//   Pmnihealthcare
// </copyright>
// </Screen Owner>
// Shashank Modified on : Feb 09 2016
// </Screen Owner>
// <summary>
//   The patient care plan repository.
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
    /// The patient care plan repository.
    /// </summary>
    public class PatientCarePlanRepository : GenericRepository<PatientCarePlan>
    {
        #region Fields

        /// <summary>
        /// The _context.
        /// </summary>
        private readonly DbContext _context;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PatientCarePlanRepository"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public PatientCarePlanRepository(BillingEntities context)
            : base(context)
        {
            this.AutoSave = true;
            this._context = context;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Adds the update patient care plan.
        /// </summary>
        /// <param name="corporateId">
        /// The corporate identifier.
        /// </param>
        /// <param name="facilityId">
        /// The facility identifier.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<PatientCarePlanCustomModel> AddUpdatePatientCarePlan(int corporateId, int facilityId)
        {
            try
            {
                if (this._context != null)
                {
                    string spName = string.Format(
                        "EXEC {0} @pFId, @pCId", 
                        StoredProcedures.SPROC_CreatePatientCarePlanActivites);
                    var sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("pFId", facilityId);
                    sqlParameters[1] = new SqlParameter("pCId", corporateId);
                    IEnumerable<PatientCarePlanCustomModel> result =
                        this._context.Database.SqlQuery<PatientCarePlanCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception )
            {
                // throw ex;
            }

            return new List<PatientCarePlanCustomModel>();
        }

        /// <summary>
        /// Adds the update patient care plan_1.
        /// </summary>
        /// <param name="patientId">
        /// The patient identifier.
        /// </param>
        /// <param name="encounterId">
        /// The encounter identifier.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<PatientCarePlanCustomModel> AddUpdatePatientCarePlan_1(int patientId, int encounterId)
        {
            try
            {
                if (this._context != null)
                {
                    string spName = string.Format(
                        "EXEC {0} @pPId, @pEId", 
                        StoredProcedures.SPROC_CreatePatientCarePlanActivites_V1);
                    var sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("pPId", patientId);
                    sqlParameters[1] = new SqlParameter("pEId", encounterId);
                    IEnumerable<PatientCarePlanCustomModel> result =
                        this._context.Database.SqlQuery<PatientCarePlanCustomModel>(spName, sqlParameters);
                    return result.ToList();
                }
            }
            catch (Exception )
            {
                // throw ex;
            }

            return new List<PatientCarePlanCustomModel>();
        }

        /// <summary>
        /// Deletes the patient care plan.
        /// </summary>
        /// <param name="patientId">
        /// The patient identifier.
        /// </param>
        /// <param name="encounterId">
        /// The encounter identifier.
        /// </param>
        /// <param name="id">
        /// The identifier.
        /// </param>
        /// <param name="tasknumber">
        /// The tasknumber.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool DeletePatientCarePlan(int patientId, int encounterId, int id, string tasknumber)
        {
            try
            {
                if (this._context != null)
                {
                    string spName = string.Format(
                        "EXEC {0} @pPId, @pEId,@pId,@pTaskNumber", 
                        StoredProcedures.SPROC_DeletePatientCarePlanActivites);
                    var sqlParameters = new SqlParameter[4];
                    sqlParameters[0] = new SqlParameter("pPId", patientId);
                    sqlParameters[1] = new SqlParameter("pEId", encounterId);
                    sqlParameters[2] = new SqlParameter("pId", id);
                    sqlParameters[3] = new SqlParameter("pTaskNumber", tasknumber);
                    this.ExecuteCommand(spName, sqlParameters);
                    return true;
                }
            }
            catch (Exception )
            {
                return false;
            }

            return false;
        }

        #endregion
    }
}