// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FutureOrderActivityMapper.cs" company="Spadez">
//   Omnihealthcare
// </copyright>
// <summary>
//   The future order activity mapper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



namespace BillingSystem.Bal.Mapper
{
    using BillingSystem.Model.CustomModel;
    using BillingSystem.Model.Model;

    /// <summary>
    /// The future order activity mapper.
    /// </summary>
    public class FutureOrderActivityMapper : Mapper<FutureOrderActivity, FutureOrderActivityCustomModel>
    {
        /// <summary>
        /// Maps the view model to model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public override FutureOrderActivity MapViewModelToModel(FutureOrderActivityCustomModel model)
        {
            var vm = base.MapModelToViewModel(model);
            return vm;
        }
    }
}