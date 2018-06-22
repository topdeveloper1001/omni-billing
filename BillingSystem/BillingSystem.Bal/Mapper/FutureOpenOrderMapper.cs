// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FutureOpenOrderMapper.cs" company="Spadez">
//   Omnihealthcare
// </copyright>
// <summary>
//   The future open order mapper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------




namespace BillingSystem.Bal.Mapper
{
    using System;

    using BillingSystem.Bal.BusinessAccess;
    using BillingSystem.Common.Common;
    using BillingSystem.Model.CustomModel;
    using BillingSystem.Model.Model;

    /// <summary>
    /// The future open order mapper.
    /// </summary>
    public class FutureOpenOrderMapper : Mapper<FutureOpenOrder, FutureOpenOrderCustomModel>
    {

        public string TableNumber { get; set; }

        public string TableDescription { get; set; }

        public string CptTableNumber { get; set; }

        public string ServiceCodeTableNumber { get; set; }

        public string DrgTableNumber { get; set; }

        public string DrugTableNumber { get; set; }

        public string HcpcsTableNumber { get; set; }

        public string DiagnosisTableNumber { get; set; }

        public string BillEditRuleTableNumber { get; set; }

        public FutureOpenOrderMapper()
        {
            
        }

        public FutureOpenOrderMapper(
            string cptTableNumber,
            string serviceCodeTableNumber,
            string drgTableNumber,
            string drugTableNumber,
            string hcpcsTableNumber,
            string diagnosisTableNumber)
        {
            if (!string.IsNullOrEmpty(cptTableNumber)) CptTableNumber = cptTableNumber;

            if (!string.IsNullOrEmpty(serviceCodeTableNumber)) ServiceCodeTableNumber = serviceCodeTableNumber;

            if (!string.IsNullOrEmpty(drgTableNumber)) DrgTableNumber = drgTableNumber;

            if (!string.IsNullOrEmpty(drugTableNumber)) DrugTableNumber = drugTableNumber;

            if (!string.IsNullOrEmpty(hcpcsTableNumber)) HcpcsTableNumber = hcpcsTableNumber;

            if (!string.IsNullOrEmpty(diagnosisTableNumber)) DiagnosisTableNumber = diagnosisTableNumber;
        }

        /// <summary>
        /// Maps the view model to model.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="FutureOpenOrder"/>.
        /// </returns>
        public override FutureOpenOrder MapViewModelToModel(FutureOpenOrderCustomModel model)
        {
            var vm = base.MapViewModelToModel(model);
            return vm;
        }

        /// <summary>
        /// Maps the model to view model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public override FutureOpenOrderCustomModel MapModelToViewModel(FutureOpenOrder model)
        {
            var vm = base.MapModelToViewModel(model);
            var basebal = new BaseBal(CptTableNumber, ServiceCodeTableNumber, DrgTableNumber, DrugTableNumber, HcpcsTableNumber, DiagnosisTableNumber);
            vm.DiagnosisDescription = basebal.GetDiagnoseNameByCodeId(vm.DiagnosisCode);
            vm.OrderDescription = basebal.GetCodeDescription(vm.OrderCode, vm.OrderType);
            vm.Status = basebal.GetNameByGlobalCodeValue(
                vm.OrderStatus,
                Convert.ToInt32(GlobalCodeCategoryValue.OrderStatus).ToString());
            vm.FrequencyText = basebal.GetNameByGlobalCodeValue(
                vm.FrequencyCode,
                Convert.ToInt32(GlobalCodeCategoryValue.OrderFrequencyType).ToString());
            vm.CategoryName = basebal.GetGlobalCategoryNameById(vm.CategoryId.ToString());
            vm.SubCategoryName = vm.SubCategoryId == null
                                     ? string.Empty
                                     : basebal.GetNameByGlobalCodeId(Convert.ToInt32(vm.SubCategoryId));
            vm.OrderTypeName = basebal.GetNameByGlobalCodeValue(
                Convert.ToInt32(vm.OrderType).ToString(),
                Convert.ToInt32(GlobalCodeCategoryValue.CodeTypes).ToString());
            vm.SpecimenTypeStr = basebal.CalculateLabResultSpecimanType(vm.OrderCode, null, vm.PatientID);
            return vm;
        }
    }
}