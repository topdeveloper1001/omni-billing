using BillingSystem.Model;
using BillingSystem.Model.CustomModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillingSystem.Bal.Mapper
{
    public class BillingModifierMapper : Mapper<BillingModifier, BillingModifierCustomModel>
    {
        public override BillingModifierCustomModel MapModelToViewModel(BillingModifier model)
        {
            var vm = base.MapModelToViewModel(model);
            return vm;
        }
    }
}
