// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PatientLoginDetailBal.cs" company="SPadez">
//   Omnihealth care
// </copyright>
// <summary>
//   The patient login detail bal.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Bal.BusinessAccess
{
    using System;
    using System.Linq;

    using BillingSystem.Bal.Mapper;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;
    using BillingSystem.Repository.GenericRepository;

    /// <summary>
    /// The patient login detail bal.
    /// </summary>
    public class PatientLoginDetailBal : BaseBal
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PatientLoginDetailBal"/> class.
        /// </summary>
        public PatientLoginDetailBal()
        {
            this.PatientLoginDetailMapper = new PatientLoginDetailMapper();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PatientLoginDetailBal"/> class.
        /// </summary>
        /// <param name="cptTableNumber">
        /// The cpt table number.
        /// </param>
        /// <param name="serviceCodeTableNumber">
        /// The service code table number.
        /// </param>
        /// <param name="drgTableNumber">
        /// The drg table number.
        /// </param>
        /// <param name="drugTableNumber">
        /// The drug table number.
        /// </param>
        /// <param name="hcpcsTableNumber">
        /// The hcpcs table number.
        /// </param>
        /// <param name="diagnosisTableNumber">
        /// The diagnosis table number.
        /// </param>
        public PatientLoginDetailBal(
            string cptTableNumber,
            string serviceCodeTableNumber,
            string drgTableNumber,
            string drugTableNumber,
            string hcpcsTableNumber,
            string diagnosisTableNumber)
        {
            if (!string.IsNullOrEmpty(cptTableNumber))
            {
                this.CptTableNumber = cptTableNumber;
            }

            if (!string.IsNullOrEmpty(serviceCodeTableNumber))
            {
                this.ServiceCodeTableNumber = serviceCodeTableNumber;
            }

            if (!string.IsNullOrEmpty(drgTableNumber))
            {
                this.DrgTableNumber = drgTableNumber;
            }

            if (!string.IsNullOrEmpty(drugTableNumber))
            {
                this.DrugTableNumber = drugTableNumber;
            }

            if (!string.IsNullOrEmpty(hcpcsTableNumber))
            {
                this.HcpcsTableNumber = hcpcsTableNumber;
            }

            if (!string.IsNullOrEmpty(diagnosisTableNumber))
            {
                this.DiagnosisTableNumber = diagnosisTableNumber;
            }

            this.PatientLoginDetailMapper = new PatientLoginDetailMapper();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the patient login detail mapper.
        /// </summary>
        private PatientLoginDetailMapper PatientLoginDetailMapper { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get patient login detail by patient id.
        /// </summary>
        /// <param name="patientId">
        /// The patient id.
        /// </param>
        /// <returns>
        /// The <see cref="PatientLoginDetailCustomModel"/>.
        /// </returns>
        public PatientLoginDetailCustomModel GetPatientLoginDetailByPatientId(int patientId)
        {
            using (PatientLoginDetailRepository rep = this.UnitOfWork.PatientLoginDetailRepository)
            {
                PatientLoginDetail model =
                    rep.Where(m => m.PatientId == patientId && m.IsDeleted != true).FirstOrDefault();
                return model != null
                           ? this.PatientLoginDetailMapper.MapModelToViewModel(model)
                    : new PatientLoginDetailCustomModel();
            }
        }

        /// <summary>
        /// The get patient login details by email.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="PatientLoginDetailCustomModel"/>.
        /// </returns>
        public PatientLoginDetailCustomModel GetPatientLoginDetailsByEmail(string email)
        {
            PatientLoginDetailCustomModel vm = null;
            using (PatientLoginDetailRepository rep = this.UnitOfWork.PatientLoginDetailRepository)
            {
                PatientLoginDetail model =
                    rep.Where(m => m.Email.Equals(email) && (m.IsDeleted == null || m.IsDeleted == false))
                        .FirstOrDefault();
                if (model != null)
                {
                    vm = this.PatientLoginDetailMapper.MapModelToViewModel(model);
                }
            }

            return vm;
        }

        /// <summary>
        /// The save patient login details.
        /// </summary>
        /// <param name="vm">
        /// The vm.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int SavePatientLoginDetails(PatientLoginDetailCustomModel vm)
        {
            using (PatientLoginDetailRepository rep = this.UnitOfWork.PatientLoginDetailRepository)
            {
                PatientLoginDetail model = this.PatientLoginDetailMapper.MapViewModelToModel(vm);
                if (vm.Id > 0)
                {
                    PatientLoginDetail current = rep.GetSingle(model.Id);
                    model.Password = current.Password;
                    model.CreatedBy = current.CreatedBy;
                    model.CreatedDate = current.CreatedDate;
                    rep.UpdateEntity(model, model.Id);
                }
                else
                {
                    model.Password = string.Empty;
                    rep.Create(model);
                }

                return model.Id;
            }
        }

        /// <summary>
        /// The update patient login failed log.
        /// </summary>
        /// <param name="patientId">
        /// The patient id.
        /// </param>
        /// <param name="failedLoginAttempts">
        /// The failed login attempts.
        /// </param>
        /// <param name="lastInvalidLogin">
        /// The last invalid login.
        /// </param>
        public void UpdatePatientLoginFailedLog(int patientId, int failedLoginAttempts, DateTime lastInvalidLogin)
        {
            using (PatientLoginDetailRepository rep = this.UnitOfWork.PatientLoginDetailRepository)
            {
                PatientLoginDetail vm = rep.GetSingle(patientId);
                if (vm != null)
                {
                    vm.LastInvalidLogin = lastInvalidLogin;
                    vm.FailedLoginAttempts = failedLoginAttempts;
                    rep.UpdateEntity(vm, vm.Id);
                }
            }
        }

        /// <summary>
        /// Updates the patient email identifier.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="emailId">The email identifier.</param>
        public void UpdatePatientEmailId(int patientId, string emailId)
        {
            using (var rep = this.UnitOfWork.PatientLoginDetailRepository)
            {
                rep.UpdatePatientEmailAddress(patientId, emailId);
            }
        }

        /// <summary>
        /// Gets the patient email.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public string GetPatientEmail(int patientId)
        {
            using (var rep = this.UnitOfWork.PatientLoginDetailRepository)
            {
                var patientLoginObj = rep.Where(x => x.PatientId == patientId && (x.IsDeleted == null || x.IsDeleted == false)).FirstOrDefault();
                return patientLoginObj != null ? patientLoginObj.Email : string.Empty;
            }
        }
        #endregion
    }
}
