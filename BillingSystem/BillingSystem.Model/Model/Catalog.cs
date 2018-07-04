using System;
using System.ComponentModel.DataAnnotations;

namespace BillingSystem.Model
{
    public class Catalog
    {
        [Key]
        public int Id { get; set; }
        public long ItemID { get; set; }
        public int ManufacturerID { get; set; }
        [MaxLength(50)]
        public string ManufacturerName { get; set; }
        public long ProductID { get; set; }
        [MaxLength(120)]
        public string ProductName { get; set; }
        [MaxLength(150)]
        public string ProductDescription { get; set; }
        public long ManufacturerItemCode { get; set; }
        [MaxLength(150)]
        public string ItemDescription { get; set; }
        public string ItemImageURL { get; set; }
        [MaxLength(20)]
        public string VenderItemCode { get; set; }
        [MaxLength(8)]
        public string Pkg { get; set; }
        public long UnitPrice { get; set; }
        [MaxLength(12)]
        public string PriceDescription { get; set; }
        [MaxLength(18)]
        public string Availability { get; set; }
        [MaxLength(60)]
        public string CategoryPathID { get; set; }
        [MaxLength(200)]
        public string CategoryPathName { get; set; }
        [MaxLength(150)]
        public string PackingListDescritpion { get; set; }
        public long UnitWeight { get; set; }
        public long UnitVolume { get; set; }
        public int UOMFactor { get; set; }
        [MaxLength(6)]
        public string CountryOfOrigin { get; set; }
        [MaxLength(18)]
        public string HarmonizedTariffCode { get; set; }
        [MaxLength(15)]
        public string HazMatClass { get; set; }
        [MaxLength(15)]
        public string HazMatCode { get; set; }
        [MaxLength(6)]
        public string PharmacyProductType { get; set; }
        [MaxLength(15)]
        public string NationalDrugCode { get; set; }
        [MaxLength(30)]
        public string BrandID { get; set; }
        [MaxLength(30)]
        public string BrandName { get; set; }
        [MaxLength(12)]
        public string ProdCatNumber { get; set; }
        [MaxLength(12)]
        public string Size { get; set; }
        public int? CorporateId { get; set; }

        public int? FacilityId { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

    }
}
