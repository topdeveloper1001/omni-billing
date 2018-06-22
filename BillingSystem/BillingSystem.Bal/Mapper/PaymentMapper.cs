using System;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Mapper
{
    public class PaymentMapper : Mapper<Payment, PaymentCustomModel>
    {
        public override PaymentCustomModel MapModelToViewModel(Payment model)
        {
            var bBal = new BaseBal();
            var vm = base.MapModelToViewModel(model);
            if (vm != null)
            {
                vm.PaymentTypeName = bBal.GetNameByGlobalCodeValue(Convert.ToString(model.PaymentTypeId), "5011");
            }
            return vm;
        }
    }
}
