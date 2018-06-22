using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.BusinessAccess
{
    public class DischargeSummaryDetailBal : BaseBal
    {
        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public List<DischargeSummaryDetailCustomModel> SaveDischargeSummaryDetail(DischargeSummaryDetail model)
        {
            var list = new List<DischargeSummaryDetailCustomModel>();
            using (var rep = UnitOfWork.DischargeSummaryDetailRepository)
            {
                if (model.Id > 0)
                    rep.UpdateEntity(model, model.Id);
                else
                    rep.Create(model);
                if (model.Id > 0)
                    list = GetDischargeSummaryDetailListByTypeId(model.AssociatedTypeId);
            }
            return list;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public List<DischargeSummaryDetailCustomModel> DeleteDischargeDetail(int id, string typeId)
        {
            List<DischargeSummaryDetailCustomModel> list;
            using (var rep = UnitOfWork.DischargeSummaryDetailRepository)
            {
                rep.Delete(id);
                list = GetDischargeSummaryDetailListByTypeId(typeId);
            }
            return list;
        }

        public List<DischargeSummaryDetailCustomModel> GetDischargeSummaryDetailListByTypeId(string typeId)
        {
            var list = new List<DischargeSummaryDetailCustomModel>();
            using (var rep = UnitOfWork.DischargeSummaryDetailRepository)
            {
                var details = rep.Where(r => r.AssociatedTypeId == typeId).ToList();
                if (details.Count > 0)
                {
                    list = details.Select(item =>
                    {
                        var gc = GetGlobalCodeByCategoryAndCodeValue(item.AssociatedTypeId, item.AssociatedId);
                        var newItem = new DischargeSummaryDetailCustomModel
                        {
                            Id = item.Id,
                            AssociatedId = item.AssociatedId,
                            AssociatedTypeId = item.AssociatedTypeId,
                            DischargeSummaryId = item.DischargeSummaryId,
                            Name = gc.GlobalCodeName,
                            Description = gc.Description,
                            OtherValue = item.OtherValue
                        };
                        return newItem;
                    }).ToList();
                }
            }
            return list;
        }

        public bool CheckIfRecordAlreadyAdded(string id, string typeId)
        {
            using (var rep = UnitOfWork.DischargeSummaryDetailRepository)
            {
                var result =
                    rep.Where(
                        r =>
                            r.AssociatedTypeId.Equals(typeId) &&
                            r.AssociatedId.Equals(id)).Any();
                return result;
            }
        }
    }
}
