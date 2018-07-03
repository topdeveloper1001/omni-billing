using System.Linq;
using AutoMapper;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;


namespace BillingSystem.Bal.BusinessAccess
{
    public class SchedulingParametersService : ISchedulingParametersService
    {
        private readonly IRepository<SchedulingParameters> _repository;
        private readonly IMapper _mapper;

        public SchedulingParametersService(IRepository<SchedulingParameters> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        #region Public Methods and Operators

        public SchedulingParametersCustomModel FindById(int? id)
        {
            var model = _repository.Where(x => x.Id == id).FirstOrDefault();
            var vm = _mapper.Map<SchedulingParametersCustomModel>(model);
            return vm;
        }

        public long SaveRecord(SchedulingParametersCustomModel vm)
        {
            var model = _mapper.Map<SchedulingParameters>(vm);

            var current = _repository.Where(a => a.FacilityId == model.FacilityId).FirstOrDefault();
            if (model.Id > 0 || current != null)
            {
                current.StartHour = model.StartHour;
                current.EndHour = model.EndHour;
                current.ModifiedBy = model.ModifiedBy;
                current.ModifiedDate = model.ModifiedDate;
                _repository.UpdateEntity(current, model.Id);
            }
            else
            {
                _repository.Create(model);
            }

            return model.Id;
        }

        public SchedulingParametersCustomModel GetDataByFacilityId(long facilityId)
        {
            var m = _repository.Where(x => x.FacilityId == facilityId && x.IsActive).FirstOrDefault();
            if (m != null)
            {
                var vm = _mapper.Map<SchedulingParametersCustomModel>(m);
                return vm;
            }
            return new SchedulingParametersCustomModel();
        }
        #endregion
    }
}
