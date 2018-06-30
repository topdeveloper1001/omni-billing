using BillingSystem.Model.Model;
using System.Collections.Generic;
using System.Linq;
using BillingSystem.Model.CustomModel;
using BillingSystem.Repository.Interfaces;
using BillingSystem.Model;
using AutoMapper;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class PaymentTypeDetailService : IPaymentTypeDetailService
    {
        private readonly IRepository<PaymentTypeDetail> _repository;

        private readonly BillingEntities _context;
        private readonly IMapper _mapper;

        public PaymentTypeDetailService(IRepository<PaymentTypeDetail> repository, BillingEntities context, IMapper mapper)
        {
            _repository = repository;
            _context = context;
            _mapper = mapper;
        }

        #region Public Methods and Operators

        /// <summary>
        ///     Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<PaymentTypeDetailCustomModel> GetPaymentTypeDetail()
        {
            var list = new List<PaymentTypeDetailCustomModel>();
            var lstPaymentTypeDetail = _repository.GetAll().ToList();
            if (lstPaymentTypeDetail.Count > 0)
            {
                list.AddRange(lstPaymentTypeDetail.Select(item => new PaymentTypeDetailCustomModel()));
            }

            return list;
        }

        /// <summary>
        /// Method to add the Entity in the database By Id.
        /// </summary>
        /// <param name="paymentTypeDetailId">The Holiday Planner Id.</param>
        /// <returns>
        /// The <see cref="PaymentTypeDetail" />.
        /// </returns>
        public PaymentTypeDetail GetPaymentTypeDetailById(int? paymentTypeDetailId)
        {
            var model = _repository.Where(x => x.Id == paymentTypeDetailId).FirstOrDefault();
            return model;
        }

        public PaymentTypeDetail GetPaymentTypeDetailByPaymentId(int? paymentId)
        {
            var model = _repository.Where(x => x.PaymentId == paymentId).FirstOrDefault();
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
        public int SavePaymentTypeDetail(PaymentTypeDetail model)
        {
            if (model.Id > 0)
                _repository.UpdateEntity(model, model.Id);
            else
                _repository.Create(model);

            return model.Id;
        }

        #endregion
    }
}
