// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MedicalNecessityBal.cs" company="Spadez">
//   OmniHealthcare
// </copyright>
// <summary>
//   The holiday planner bal.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Bal.BusinessAccess
{
    using System.Collections.Generic;
    using System.Linq;

    using BillingSystem.Bal.Mapper;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;
    using BillingSystem.Repository.GenericRepository;

    /// <summary>
    /// The holiday planner bal.
    /// </summary>
    public class MedicalNecessityBal : BaseBal
    {
        #region Constructors and Destructors


       /// <summary>
        /// Initializes a new instance of the <see cref="MedicalNecessityBal"/> class.
        /// </summary>
        public MedicalNecessityBal(string diagnosisTableNumber)
        {
            if (!string.IsNullOrEmpty(diagnosisTableNumber))
                DiagnosisTableNumber = diagnosisTableNumber;
            this.MedicalNecessityMapper = new MedicalNecessityMapper();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the holiday planner mapper.
        /// </summary>
        private MedicalNecessityMapper MedicalNecessityMapper { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<MedicalNecessityCustomModel> GetMedicalNecessity()
        {
            List<MedicalNecessityCustomModel> list;
            using (var medicalNecessityRep = this.UnitOfWork.MedicalNecessityRepository)
            {
                list = medicalNecessityRep.GetMedicalNecessityData(DiagnosisTableNumber);
               //list.AddRange(
               //         lstMedicalNecessity.Select(item => MedicalNecessityMapper.MapModelToViewModel(item)));
            }

            return list;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="medicalNecessityId">The Holiday Planner Id.</param>
        /// <returns>
        /// The <see cref="MedicalNecessity" />.
        /// </returns>
        public MedicalNecessity GetMedicalNecessityById(int? medicalNecessityId)
        {
            using (MedicalNecessityRepository rep = this.UnitOfWork.MedicalNecessityRepository)
            {
                MedicalNecessity model = rep.Where(x => x.Id == medicalNecessityId).FirstOrDefault();
                return model;
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
        public int SaveMedicalNecessity(MedicalNecessity model)
        {
            using (MedicalNecessityRepository rep = this.UnitOfWork.MedicalNecessityRepository)
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

        #endregion
    }
}
