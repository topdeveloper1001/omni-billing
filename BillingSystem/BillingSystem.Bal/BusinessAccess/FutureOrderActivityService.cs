using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Model.CustomModel;
using BillingSystem.Model.Model;


namespace BillingSystem.Bal.BusinessAccess
{
    public class FutureOrderActivityService : IFutureOrderActivityService
    {
        private readonly IRepository<FutureOrderActivity> _repository;
        private readonly IMapper _mapper;

        public FutureOrderActivityService(IRepository<FutureOrderActivity> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        #region Public Methods and Operators

        /// <summary>
        ///     Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<FutureOrderActivityCustomModel> GetFutureOrderActivity()
        {
            var list = new List<FutureOrderActivityCustomModel>();
            var lstFutureOrderActivity = _repository.GetAll().ToList();
            if (lstFutureOrderActivity.Count > 0)
            {
                list.AddRange(lstFutureOrderActivity.Select(item => _mapper.Map<FutureOrderActivityCustomModel>(item)));
            }

            return list;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="FutureOrderActivityId">The Holiday Planner Id.</param>
        /// <returns>
        /// The <see cref="FutureOrderActivity" />.
        /// </returns>
        public FutureOrderActivity GetFutureOrderActivityById(int? FutureOrderActivityId)
        {
            var m = _repository.Where(x => x.FutureOrderActivityID == FutureOrderActivityId).FirstOrDefault();
            return m;
        }

        /// <summary>
        /// Method to add/Update the Entity in the database.
        /// </summary>
        /// <param name="m">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int SaveFutureOrderActivity(FutureOrderActivity m)
        {
            if (m.FutureOrderActivityID > 0)
                _repository.UpdateEntity(m, m.FutureOrderActivityID);
            else
                _repository.Create(m);

            return m.FutureOrderActivityID;
        }

        #endregion
    }
}
