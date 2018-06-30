using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{

    /// <summary>
    /// The holiday planner bal.
    /// </summary>
    public class PreSchedulingLinkService : IPreSchedulingLinkService
    {
        private readonly IRepository<PreSchedulingLink> _repository;
        private readonly IMapper _mapper;

        public PreSchedulingLinkService(IRepository<PreSchedulingLink> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

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
            var model = _repository.Where(x => x.Id == PreSchedulingLinkId).FirstOrDefault();
            return model;
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
            if (model.Id > 0)
                _repository.UpdateEntity(model, model.Id);
            else
                _repository.Create(model);

            return model.Id;
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
            var lstPreSchedulingLink = cid == 0
                                           ? _repository.GetAll().ToList()
                                           : _repository.Where(x => x.CorporateId == cid).ToList();
            if (lstPreSchedulingLink.Count > 0)
            {
                lstPreSchedulingLink = cid == 0
                                           ? lstPreSchedulingLink
                                           : fid != 0
                                                 ? lstPreSchedulingLink.Where(x => x.FacilityId == fid).ToList()
                                                 : lstPreSchedulingLink;
                list = MapValues(lstPreSchedulingLink);
            }

            return list;
        }
        private List<PreSchedulingLinkCustomModel> MapValues(List<PreSchedulingLink> m)
        {
            var lst = new List<PreSchedulingLinkCustomModel>();
            foreach (var model in m)
            {
                var vm = _mapper.Map<PreSchedulingLinkCustomModel>(model);
                var basebalobj = new BaseBal();
                vm.FacilityName = basebalobj.GetFacilityNameByFacilityId(Convert.ToInt32(model.FacilityId));
                vm.CorporateName = basebalobj.GetCorporateNameFromId(Convert.ToInt32(model.CorporateId));
                lst.Add(vm);
            }

            return lst;
        }
        private PreSchedulingLinkCustomModel MapValues(PreSchedulingLink model)
        {
            var vm = _mapper.Map<PreSchedulingLinkCustomModel>(model);
            var basebalobj = new BaseBal();
            vm.FacilityName = basebalobj.GetFacilityNameByFacilityId(Convert.ToInt32(model.FacilityId));
            vm.CorporateName = basebalobj.GetCorporateNameFromId(Convert.ToInt32(model.CorporateId));

            return vm;
        }
        /// <summary>
        /// Deletes the pre scheduling link.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public bool DeletePreSchedulingLink(PreSchedulingLink model)
        {
            _repository.Delete(model);
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
            var objList = _repository.Where(x => x.CorporateId == cId && x.FacilityId == fId).ToList();
            if (objList.Any())
            {
                return objList.FirstOrDefault();
            }

            return new PreSchedulingLink();
        }
        #endregion
    }
}
