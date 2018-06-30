using System.Linq;
using System.Collections.Generic;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Interfaces;
using BillingSystem.Model;
using AutoMapper;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class StateService : IStateService
    {
        private readonly IRepository<State> _repository;
        private readonly IMapper _mapper;

        public StateService(IRepository<State> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Get the States
        /// </summary>
        /// <returns>Return the states by country ID</returns>
        public List<StateCustomModel> GetStatesByCountryId(int countryId)
        {
            var list = _repository.Where(s => s.CountryID == countryId && s.IsDeleted == false).ToList();
            var lst = list.Select(x => _mapper.Map<StateCustomModel>(x)).ToList();
            return lst;
        }
    }
}
