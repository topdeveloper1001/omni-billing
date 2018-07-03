using System.Linq;
using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using AutoMapper;

using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class CityService : ICityService
    {
        private readonly IRepository<City> _repository;
        private readonly IMapper _mapper;

        public CityService(IRepository<City> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Get the Cities
        /// </summary>
        /// <returns>Return the Countries View Model</returns>
        public List<CityCustomModel> GetCityListByStateId(int stateId)
        {
            var mlst = new List<City>();
            mlst = _repository.GetAll().Where(s => s.StateID == stateId && s.IsActive == true).ToList();
            var vmlst = mlst.Select(x => _mapper.Map<CityCustomModel>(x)).ToList();
            // new List<CityCustomModel>();
            //customCitylist = (from y in list
            //                   select new CityCustomModel
            //                   {
            //                       CityID = y.CityID,
            //                       Name = y.Name
            //                   }).ToList();

            return vmlst;
        }
    }
}
