// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreSchedulingLinkBal.cs" company="Spadez">
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
    public class PreSchedulingLinkBal : BaseBal
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PreSchedulingLinkBal"/> class.
        /// </summary>
        public PreSchedulingLinkBal()
        {
            this.PreSchedulingLinkMapper = new PreSchedulingLinkMapper();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the holiday planner mapper.
        /// </summary>
        private PreSchedulingLinkMapper PreSchedulingLinkMapper { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="PreSchedulingLinkId">The Holiday Planner Id.</param>
        /// <returns>
        /// The <see cref="PreSchedulingLink" />.
        /// </returns>
        public PreSchedulingLink GetPreSchedulingLinkById(int? PreSchedulingLinkId)
        {
            using (PreSchedulingLinkRepository rep = this.UnitOfWork.PreSchedulingLinkRepository)
            {
                PreSchedulingLink model = rep.Where(x => x.Id == PreSchedulingLinkId).FirstOrDefault();
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
        public int SavePreSchedulingLink(PreSchedulingLink model)
        {
            using (PreSchedulingLinkRepository rep = this.UnitOfWork.PreSchedulingLinkRepository)
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
        /// Gets the pre scheduling link.
        /// </summary>
        /// <param name="cid">The cid.</param>
        /// <param name="fid">The fid.</param>
        /// <returns></returns>
        public List<PreSchedulingLinkCustomModel> GetPreSchedulingLink(int cid, int fid)
        {
            var list = new List<PreSchedulingLinkCustomModel>();
            using (var preSchedulingLinkRep = this.UnitOfWork.PreSchedulingLinkRepository)
            {
                var lstPreSchedulingLink = cid == 0
                                               ? preSchedulingLinkRep.GetAll().ToList()
                                               : preSchedulingLinkRep.Where(x => x.CorporateId == cid).ToList();
                if (lstPreSchedulingLink.Count > 0)
                {
                    lstPreSchedulingLink = cid == 0
                                               ? lstPreSchedulingLink
                                               : fid != 0
                                                     ? lstPreSchedulingLink.Where(x => x.FacilityId == fid).ToList()
                                                     : lstPreSchedulingLink;
                    list.AddRange(lstPreSchedulingLink.Select(item => PreSchedulingLinkMapper.MapModelToViewModel(item)));
                }
            }

            return list;
        }

        /// <summary>
        /// Deletes the pre scheduling link.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public bool DeletePreSchedulingLink(PreSchedulingLink model)
        {
            using (var rep = UnitOfWork.PreSchedulingLinkRepository)
            {
                rep.Delete(model);
            }

            return true;
        }

        /// <summary>
        /// Checkfors the previous data.
        /// </summary>
        /// <param name="cId">The c identifier.</param>
        /// <param name="fId">The f identifier.</param>
        /// <returns></returns>
        public PreSchedulingLink CheckforPreviousData(int? cId, int? fId)
        {
            using (var rep = UnitOfWork.PreSchedulingLinkRepository)
            {
                var objList = rep.Where(x => x.CorporateId == cId && x.FacilityId == fId).ToList();
                if (objList.Any())
                {
                    return objList.FirstOrDefault();
                }

                return new PreSchedulingLink();
            }
        }
        #endregion
    }
}
