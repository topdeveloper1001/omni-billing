// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PaymentTypeDetailBal.cs" company="Spadez">
//   OmniHealthcare
// </copyright>
// <summary>
//   The holiday planner bal.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using BillingSystem.Model.Model;

namespace BillingSystem.Bal.BusinessAccess
{
    using System.Collections.Generic;
    using System.Linq;

    using BillingSystem.Bal.Mapper;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;
    using BillingSystem.Repository.GenericRepository;

    /// <summary>
    /// The holiday planner bal.
    /// </summary>
    public class PaymentTypeDetailBal : BaseBal
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentTypeDetailBal"/> class.
        /// </summary>
        //public PaymentTypeDetailBal()
        //{
        //    this.PaymentTypeDetailMapper = new PaymentTypeDetailMapper();
        //}

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the holiday planner mapper.
        /// </summary>
        //private PaymentTypeDetailMapper PaymentTypeDetailMapper { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Get the Entity
        /// </summary>
        /// <returns>Return the Entity List</returns>
        public List<PaymentTypeDetailCustomModel> GetPaymentTypeDetail()
        {
            var list = new List<PaymentTypeDetailCustomModel>();
            using (var PaymentTypeDetailRep = this.UnitOfWork.PaymentTypeDetailRepository)
            {
                var lstPaymentTypeDetail = PaymentTypeDetailRep.GetAll().ToList();
                if (lstPaymentTypeDetail.Count > 0)
                {
                    list.AddRange(lstPaymentTypeDetail.Select(item => new PaymentTypeDetailCustomModel()));
                }
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
            using (PaymentTypeDetailRepository rep = this.UnitOfWork.PaymentTypeDetailRepository)
            {
                PaymentTypeDetail model = rep.Where(x => x.Id == paymentTypeDetailId).FirstOrDefault();
                return model;
            }
        }

        public PaymentTypeDetail GetPaymentTypeDetailByPaymentId(int? paymentId)
        {
            using (PaymentTypeDetailRepository rep = this.UnitOfWork.PaymentTypeDetailRepository)
            {
                PaymentTypeDetail model = rep.Where(x => x.PaymentId == paymentId).FirstOrDefault();
                return model;
            }
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
            using (PaymentTypeDetailRepository rep = this.UnitOfWork.PaymentTypeDetailRepository)
            {
                if (model.Id > 0)
                {
                    
                    rep.UpdateEntity(model, model.Id);
                }
                else
                {
                   
                    rep.Create(model);
                }

                return model.Id;
            }
        }

        #endregion
    }
}
