// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CarePlanMapper.cs" company="SPadez">
//   OmniHealtchare
// </copyright>
// <summary>
//   The care plan mapper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using BillingSystem.Bal.BusinessAccess;

namespace BillingSystem.Bal.Mapper
{
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;

    /// <summary>
    /// The care plan mapper.
    /// </summary>
    public class PatientCarePlanMapper : Mapper<PatientCarePlan, PatientCarePlanCustomModel>
    {
        #region Public Methods and Operators


        /// <summary>
        /// Maps the model to view model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public override PatientCarePlanCustomModel MapModelToViewModel(PatientCarePlan model)
        {
            //var pBal = new PatientCarePlanBal();

            PatientCarePlanCustomModel vm = base.MapModelToViewModel(model);
            //var cBal = new CarePlanTaskService();
            //vm.CarePlanName = !string.IsNullOrEmpty((model.CarePlanId))
            //                      ? Convert.ToInt32(model.CarePlanId) != 9999
            //                            ? cBal.GetCarePlanNameById(Convert.ToInt32(model.CarePlanId))
            //                            : "Single Task"
            //                      : "NA";
            //vm.CarePlanNumber = !string.IsNullOrEmpty((model.CarePlanId))
            //                      ? Convert.ToInt32(model.CarePlanId) != 9999
            //                            ? cBal.GetCarePlanNumberById(Convert.ToInt32(model.CarePlanId))
            //                            : "-"
            //                      : "NA"; 
            //vm.CarePlanTaskName = cBal.GetCarePlanTaskNameById(Convert.ToInt32(model.TaskId));
            //vm.CarePlanTaskNumber = cBal.GetCarePlanTaskNumberById(Convert.ToInt32(model.TaskId));
            //var patientCareList = pBal.GetPatientCarePlanPlanId(
            //    Convert.ToString(model.CarePlanId),
            //    Convert.ToString(model.PatientId),
            //    Convert.ToInt32(model.EncounterId));
            //vm.PatientCarePlanList = new List<PatientCarePlanTaskCustomModel>();
            //foreach (var item in patientCareList)
            //{
            //    vm.PatientCarePlanList.Add(new PatientCarePlanTaskMapper().MapCustomModelModelToViewModel(item));
            //}

            return vm;
        }

        #endregion
    }


    public class PatientCarePlanTaskMapper : Mapper<PatientCarePlan, PatientCarePlanTaskCustomModel>
    {
        public  PatientCarePlanTaskCustomModel MapCustomModelModelToViewModel(PatientCarePlan model)
        {
            PatientCarePlanTaskCustomModel vm = base.MapModelToViewModel(model);
            //var cBal = new CarePlanTaskService();
            //vm.CareTaskName = cBal.GetCarePlanTaskNameById(Convert.ToInt32(model.TaskId));
            //vm.CareTaskNumber = cBal.GetCarePlanTaskNumberById(Convert.ToInt32(model.TaskId));
            //vm.StartDate = model.FromDate.ToString();
            //vm.EndDate = model.TillDate.ToString();
            return vm;
        }
    }

}