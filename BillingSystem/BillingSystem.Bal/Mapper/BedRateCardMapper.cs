using System;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Mapper
{
    public class BedRateCardMapper : Mapper<BedRateCard, BedRateCardCustomModel>
    {
        public override BedRateCardCustomModel MapModelToViewModel(BedRateCard model)
        {
            var vm = base.MapModelToViewModel(model);
            if (vm != null)
            {

            }
            return vm;
        }
    }
}
