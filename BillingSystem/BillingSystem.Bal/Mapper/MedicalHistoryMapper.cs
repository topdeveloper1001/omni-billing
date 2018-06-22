using System;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Mapper
{
    public class MedicalHistoryMapper : Mapper<MedicalHistory, MedicalHistoryCustomModel>
    {
        public string DrugTableNumber { get; set; }

        public MedicalHistoryMapper()
        {

        }

        public MedicalHistoryMapper(string drugTableNumber)
        {
            DrugTableNumber = drugTableNumber;
        }


        public override MedicalHistoryCustomModel MapModelToViewModel(MedicalHistory model)
        {
            var vm = base.MapModelToViewModel(model);
            if (vm != null)
            {
                using (var dBal = new DrugBal(DrugTableNumber))
                {
                    var drug = dBal.GetCurrentDrugByCode(model.DrugName);
                    if (drug != null)
                    {
                        vm.Drug = drug.DrugGenericName;
                        vm.DrugDuration = dBal.GetNameByGlobalCodeValue(model.Duration,
                            Convert.ToString((int)GlobalCodeCategoryValue.CurrentMedicationDuration));
                        vm.DrugVolume = dBal.GetNameByGlobalCodeValue(model.Volume,
                            Convert.ToString((int)GlobalCodeCategoryValue.CurrentMedicationVolume));
                        vm.DrugDosage = dBal.GetNameByGlobalCodeValue(model.Dosage,
                            Convert.ToString((int)GlobalCodeCategoryValue.CurrentMedicationDosage));
                        vm.DrugFrequency = dBal.GetNameByGlobalCodeValue(model.Frequency,
                            Convert.ToString((int)GlobalCodeCategoryValue.OrderFrequencyType));
                        vm.DrugDecription = string.Format("{0}-{1}", drug.DrugGenericName, drug.DrugCode);
                    }
                }
            }
            return vm;
        }
    }
}
