using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;


namespace BillingSystem.Bal.BusinessAccess
{
    public class ScrubEditTrackService : IScrubEditTrackService
    {
        private readonly IRepository<ScrubEditTrack> _repository;
        private readonly BillingEntities _context;

        public ScrubEditTrackService(IRepository<ScrubEditTrack> repository, BillingEntities context)
        {
            _repository = repository;
            _context = context;
        }

        public List<ScrubEditTrackCustomModel> GetScrubEditTrack(int corporateId, int facilityId)
        {
            var spName = string.Format("EXEC {0} @CorporateId, @FacilityId", StoredProcedures.SPROC_GetScrubberEditTrack.ToString());
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("CorporateId", corporateId);
            sqlParameters[1] = new SqlParameter("FacilityId", facilityId);
            IEnumerable<ScrubEditTrackCustomModel> result = _context.Database.SqlQuery<ScrubEditTrackCustomModel>(spName, sqlParameters);
            return result.ToList();
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public int SaveScrubEditTrack(ScrubEditTrack model)
        {
            if (model.ScrubEditTrackID > 0)
                _repository.UpdateEntity(model, model.ScrubEditTrackID);
            else
                _repository.Create(model);
            return model.ScrubEditTrackID;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="ScrubEditTrackId">The scrub edit track identifier.</param>
        /// <returns></returns>
        public ScrubEditTrack GetScrubEditTrackByID(int? ScrubEditTrackId)
        {
            var model = _repository.Where(x => x.ScrubEditTrackID == ScrubEditTrackId).FirstOrDefault();
            return model;
        }
    }
}
