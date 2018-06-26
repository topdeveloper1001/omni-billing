using System;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Mapper
{
    public class DiagnosisMapper : Mapper<Diagnosis, DiagnosisCustomModel>
    {
        public string DiagnosisTableNumber { get; set; }
        public string DrgTableNumber { get; set; }

        public DiagnosisMapper(string drgTableNumber, string diagnosisTableNumer)
        {
            DrgTableNumber = drgTableNumber;
            DiagnosisTableNumber = diagnosisTableNumer;
        }

        public override DiagnosisCustomModel MapModelToViewModel(Diagnosis model)
        {
            var vm = base.MapModelToViewModel(model);

            if (vm != null)
            {
                using (var bal = new DRGCodesBal(DrgTableNumber))
                {
                    var drgModel = bal.GetDRGCodeById(Convert.ToInt32(model.DRGCodeID));
                    vm.DrgCodeDescription = drgModel.CodeDescription;
                    vm.DrgCodeValue = drgModel.CodeNumbering;

                    if (model.DiagnosisType == (int)DiagnosisType.DRG)
                    {
                        vm.DiagnosisCode = drgModel.CodeNumbering;
                        vm.DiagnosisCodeDescription = drgModel.CodeDescription;
                    }
                }

                //using (var enBal = new EncounterBal())
                //{
                //    vm.EncounterNumber = enBal.GetEncounterNumberByEncounterId(Convert.ToInt32(model.EncounterID));
                //    vm.EnteredBy = enBal.GetNameByUserId(Convert.ToInt32(model.CreatedBy));
                //}

                using (var dBal = new DiagnosisBal(DiagnosisTableNumber, DrgTableNumber))
                    vm.DiagnosisTypeName = dBal.GetDiagnosisTypeById(model.DiagnosisType);
            }

            return vm;
        }
    }
}
