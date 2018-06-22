using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class BedRateCardCustomModel
    {
        public BedRateCard BedRateCard { get; set; }
        public string UnitTypeName { get; set; }

        //public List<GlobalCodes> LstBedTypesList { get; set; }
        //public List<ServiceCodeCustomModel> LstServiceCodeList { get; set; }
        //public List<GlobalCodes> LstUnitTypeList { get; set; }
        public string ServiceCodeName { get; set; }
        public string BedTypeName { get; set; }
        public string ServiceCodeTableNumber { get; set; }
        public string ServiceCodeEffectiveFrom { get; set; }
        public string ServiceCodeEffectiveTill { get; set; }
        public string FacilityName { get; set; }
    }
}
