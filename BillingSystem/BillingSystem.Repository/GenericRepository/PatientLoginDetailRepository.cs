// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PatientLoginDetailRepository.cs" company="Spadez">
//   OmniHealthcare 
// </copyright>
// <summary>
//   The patient login detail repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Repository.GenericRepository
{
    using System;
    using System.Data.Entity;
    using System.Data.SqlClient;

    using BillingSystem.Common.Common;
    using BillingSystem.Model;

    /// <summary>
    /// The patient login detail repository.
    /// </summary>
    public class PatientLoginDetailRepository : GenericRepository<PatientLoginDetail>
    {
        #region Fields

        /// <summary>
        /// The _context.
        /// </summary>
        private readonly DbContext _context;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PatientLoginDetailRepository"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public PatientLoginDetailRepository(BillingEntities context)
            : base(context)
        {
            this.AutoSave = true;
            this._context = context;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The update patient email address.
        /// </summary>
        /// <param name="patientId">
        /// The patient id.
        /// </param>
        /// <param name="emailAddress">
        /// The email address.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool UpdatePatientEmailAddress(int patientId, string emailAddress)
        {
            try
            {
                if (this._context != null)
                {
                    string spName = string.Format("EXEC {0} @pId,@EmailId", StoredProcedures.SProc_UpdatePatientEmailId);
                    var sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("@pId", patientId);
                    sqlParameters[1] = new SqlParameter("@EmailId", emailAddress);
                    this.ExecuteCommand(spName, sqlParameters);
                    return true;
                }
            }
            catch (Exception )
            {
                // throw ex;
            }

            return false;
        }

        #endregion
    }
}