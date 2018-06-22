using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class BedRateCardView
    {
        public IEnumerable<BedRateCardCustomModel> BedRateCardsList { get; set; }
        public BedRateCardCustomModel CurrentBedRateCard { get; set; }
        //public BedRateCardViewModel CurrentBedRateCard { get; set; }
    }

    //public class BedRateCardViewModel
    //{
    //    public List<GlobalCodes> LstBedTypesList { get; set; }
    //    public List<ServiceCodeCustomModel> LstServiceCodeList { get; set; }
    //    public List<GlobalCodes> LstUnitTypeList { get; set; }
    //    public BedRateCard CurrentBedRateCard { get; set; }

    //    public string BedTypeName { get; set; }
    //    public string UnitTypeName { get; set; }
    //    public string ServiceCodeName { get; set; }
    //}
}