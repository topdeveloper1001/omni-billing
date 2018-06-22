using System;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Mapper
{
    public class AuthorizationMapper : Mapper<Authorization, AuthorizationCustomModel>
    {
        public override AuthorizationCustomModel MapModelToViewModel(Authorization model)
        {
            var vm = base.MapModelToViewModel(model);
            if (vm != null)
            {
                using (var bal = new BaseBal())
                {
                    vm.AuthrizationTypeStr = bal.GetNameByGlobalCodeValue(Convert.ToString(model.AuthorizationType),
                        Convert.ToString((int)GlobalCodeCategoryValue.AuthorizationType));
                    vm.isActive = (model.IsDeleted == null || !Convert.ToBoolean(model.IsDeleted)) &&
                                  (model.AuthorizationEnd.HasValue && model.AuthorizationEnd.Value.Date >= bal.GetInvariantCultureDateTime(Convert.ToInt32(model.FacilityID)));
                    int idPayer;
                    if (int.TryParse(model.AuthorizationIDPayer, out idPayer))
                        vm.IdPayer = bal.GetInsuranceIdPayerById(idPayer);
                }
            }
            return vm;
        }
    }
}
