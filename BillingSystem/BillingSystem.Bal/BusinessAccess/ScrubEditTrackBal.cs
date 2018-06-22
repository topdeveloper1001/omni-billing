using System;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.BusinessAccess
{
    public class ScrubEditTrackBal : BaseBal
    {
        /// <summary>
        /// Get the Entity
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns>
        /// Return the Entity List
        /// </returns>
        //public List<ScrubEditTrackCustomModel> GetScrubEditTrack(int corporateid, int facilityid)
        //{
        //    var list = new List<ScrubEditTrackCustomModel>();
        //    using (var scrubEditTrackRep = UnitOfWork.ScrubEditTrackRepository)
        //    {
        //        var globalCodeBal = new GlobalCodeBal();
        //        var lstScrubEditTrack = scrubEditTrackRep.Where(x => x.CorporateId == corporateid && x.FacilityId == facilityid).ToList();
        //        if (lstScrubEditTrack.Count > 0)
        //        {
        //            list.AddRange(lstScrubEditTrack.Select(item => new ScrubEditTrackCustomModel
        //            {
        //                ScrubEditTrackID = item.ScrubEditTrackID,
        //                TrackRuleMasterID = item.TrackRuleMasterID,
        //                TrackRuleStepID = item.TrackRuleStepID,
        //                TrackType = item.TrackType,
        //                TrackTable = item.TrackTable,
        //                TrackColumn = item.TrackColumn,
        //                TrackKeyColumn = item.TrackKeyColumn,
        //                TrackValueBefore = item.TrackValueBefore,
        //                TrackValueAfter = item.TrackValueAfter,
        //                TrackKeyIDValue = item.TrackKeyIDValue,
        //                TrackSide = item.TrackSide,
        //                IsActive = item.IsActive,
        //                CreatedBy = item.CreatedBy,
        //                CreatedDate = item.CreatedDate,
        //                CorporateId = item.CorporateId,
        //                FacilityId = item.FacilityId,
        //                TrackSideName = item.TrackSide == "LHS" ? "Left Hand Side" : "Right Hand Side",
        //                CreatedByName = GetNameByUserId(item.CreatedBy),
        //                BillNumber =
        //                    item.BillHeaderId == null
        //                        ? ""
        //                        : GetBillNumberByBillHeaderId(Convert.ToInt32(item.BillHeaderId))
        //            }));
        //        }
        //    }
        //    return list.OrderByDescending(X => X.CreatedDate).ToList();
        //}


        public List<ScrubEditTrackCustomModel> GetScrubEditTrack(int corporateId, int facilityId)
        {
            using (var scrubEditTrackRep = UnitOfWork.ScrubEditTrackRepository)
            {
                var scrubTrackList = scrubEditTrackRep.GetScruberTrackData(corporateId, facilityId);
                return scrubTrackList;
            }
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public int SaveScrubEditTrack(ScrubEditTrack model)
        {
            using (var rep = UnitOfWork.ScrubEditTrackRepository)
            {
                if (model.ScrubEditTrackID > 0)
                    rep.UpdateEntity(model, model.ScrubEditTrackID);
                else
                    rep.Create(model);
                return model.ScrubEditTrackID;
            }
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="ScrubEditTrackId">The scrub edit track identifier.</param>
        /// <returns></returns>
        public ScrubEditTrack GetScrubEditTrackByID(int? ScrubEditTrackId)
        {
            using (var rep = UnitOfWork.ScrubEditTrackRepository)
            {
                var model = rep.Where(x => x.ScrubEditTrackID == ScrubEditTrackId).FirstOrDefault();
                return model;
            }
        }
    }
}
