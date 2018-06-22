using System;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Mapper
{
    public class OpenOrderMapper : Mapper<OpenOrder, OpenOrderCustomModel>
    {
        /// <summary>
        /// Maps the model to view model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public override OpenOrderCustomModel MapModelToViewModel(OpenOrder model)
        {
            var vm = base.MapModelToViewModel(model);
            if (vm != null)
            {
                using (var bal = new BaseBal())
                {
                    vm.Status =
                        bal.GetNameByGlobalCodeValue(model.OrderStatus,
                            Convert.ToInt32(GlobalCodeCategoryValue.OrderStatus).ToString());
                    vm.FrequencyText =
                        bal.GetNameByGlobalCodeValue(model.FrequencyCode,
                            Convert.ToInt32(GlobalCodeCategoryValue.OrderFrequencyType).ToString());
                    vm.CategoryName = bal.GetGlobalCategoryNameById(model.CategoryId.ToString());
                    vm.SubCategoryName =
                        model.SubCategoryId == null
                            ? string.Empty
                            : bal.GetNameByGlobalCodeId(Convert.ToInt32(model.SubCategoryId));
                    vm.OrderTypeName = bal.GetNameByGlobalCodeValue(Convert.ToInt32(model.OrderType).ToString(),
                        Convert.ToInt32(GlobalCodeCategoryValue.CodeTypes).ToString());
                    vm.SpecimenTypeStr = bal.CalculateLabResultSpecimanType(model.OrderCode, null, model.PatientID);
                    vm.DiagnosisDescription = bal.GetDiagnoseNameByCodeId(model.DiagnosisCode);
                    vm.OrderDescription = bal.GetCodeDescription(model.OrderCode, model.OrderType);
                }
            }
            return vm;
        }

        /// <summary>
        /// Maps the model to view model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public override OpenOrder MapViewModelToModel(OpenOrderCustomModel model)
        {
            var vm = base.MapViewModelToModel(model);
            return vm;
        }
    }
}
