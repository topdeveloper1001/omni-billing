using System.Collections.Generic;
using System.Linq;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class DischargeSummaryDetailService : IDischargeSummaryDetailService
    {
        private readonly IRepository<DischargeSummaryDetail> _repository;
        private readonly IRepository<GlobalCodes> _gRepository;

        public DischargeSummaryDetailService(IRepository<DischargeSummaryDetail> repository, IRepository<GlobalCodes> gRepository)
        {
            _repository = repository;
            _gRepository = gRepository;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public List<DischargeSummaryDetailCustomModel> SaveDischargeSummaryDetail(DischargeSummaryDetail model)
        {
            var list = new List<DischargeSummaryDetailCustomModel>();

            if (model.Id > 0)
                _repository.UpdateEntity(model, model.Id);
            else
                _repository.Create(model);
            if (model.Id > 0)
                list = GetDischargeSummaryDetailListByTypeId(model.AssociatedTypeId);

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
            _repository.Delete(id);
            return GetDischargeSummaryDetailListByTypeId(typeId);
        }

        public List<DischargeSummaryDetailCustomModel> GetDischargeSummaryDetailListByTypeId(string typeId)
        {
            var list = new List<DischargeSummaryDetailCustomModel>();
            var details = _repository.Where(r => r.AssociatedTypeId == typeId).ToList();
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
            return list;
        }
        private GlobalCodes GetGlobalCodeByCategoryAndCodeValue(string gcCategoryValue, string gcvalue)
        {
            var g = _gRepository.Where(s => s.IsDeleted == false && s.GlobalCodeCategoryValue.Equals(gcCategoryValue) && s.GlobalCodeValue.Equals(gcvalue)).FirstOrDefault();
            return g ?? new GlobalCodes();
        }
        public bool CheckIfRecordAlreadyAdded(string id, string typeId)
        {
            var result = _repository.Where(r => r.AssociatedTypeId.Equals(typeId) && r.AssociatedId.Equals(id)).Any();
            return result;
        }
    }
}
