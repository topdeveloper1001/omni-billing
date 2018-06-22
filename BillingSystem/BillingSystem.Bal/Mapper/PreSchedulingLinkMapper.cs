// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreSchedulingLinkMapper.cs" company="Spadez">
//   OmniHealthcare
// </copyright>
// <owner>
// Shashank (Created on : 1st of Feb 2016)
// </owner>
// <summary>
//   The pre scheduling link.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Bal.Mapper
{
    using System;

    using BillingSystem.Bal.BusinessAccess;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;

    /// <summary>
    /// The pre scheduling link mapper.
    /// </summary>
    public class PreSchedulingLinkMapper : Mapper<PreSchedulingLink, PreSchedulingLinkCustomModel>
    {
        /// <summary>
        /// The map model to view model.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="PreSchedulingLinkCustomModel"/>.
        /// </returns>
        public override PreSchedulingLinkCustomModel MapModelToViewModel(PreSchedulingLink model)
        {
            var vm = base.MapModelToViewModel(model);
            var basebalobj = new BaseBal();
            vm.FacilityName = basebalobj.GetFacilityNameByFacilityId( Convert.ToInt32(model.FacilityId));
            vm.CorporateName = basebalobj.GetCorporateNameFromId( Convert.ToInt32(model.CorporateId));
            return vm;
        }
    }
}