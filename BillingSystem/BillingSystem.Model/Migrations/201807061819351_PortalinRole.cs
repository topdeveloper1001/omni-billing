namespace BillingSystem.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PortalinRole : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Role", "PortalId", c => c.Int(nullable: false));
            //DropTable("dbo.Catalog");
        }
        
        public override void Down()
        {
            //CreateTable(
            //    "dbo.Catalog",
            //    c => new
            //        {
            //            Id = c.Long(nullable: false, identity: true),
            //            ItemId = c.Long(nullable: false),
            //            ManufacturerId = c.Int(nullable: false),
            //            ManufacturerName = c.String(maxLength: 50),
            //            ProductId = c.Long(nullable: false),
            //            ProductName = c.String(maxLength: 120),
            //            ProductDescription = c.String(maxLength: 150),
            //            ManufacturerItemCode = c.Long(nullable: false),
            //            ItemDescription = c.String(maxLength: 150),
            //            ItemImageUrl = c.String(),
            //            VenderItemCode = c.String(maxLength: 20),
            //            Pkg = c.String(maxLength: 8),
            //            UnitPrice = c.Long(nullable: false),
            //            PriceDescription = c.String(maxLength: 12),
            //            Availability = c.String(maxLength: 18),
            //            CategoryPathId = c.String(maxLength: 60),
            //            CategoryPathName = c.String(maxLength: 200),
            //            PackingListDescritpion = c.String(maxLength: 150),
            //            UnitWeight = c.Long(nullable: false),
            //            UnitVolume = c.Long(nullable: false),
            //            UOMFactor = c.Int(nullable: false),
            //            CountryOfOrigin = c.String(maxLength: 6),
            //            HarmonizedTariffCode = c.String(maxLength: 18),
            //            HazMatClass = c.String(maxLength: 15),
            //            HazMatCode = c.String(maxLength: 15),
            //            PharmacyProductType = c.String(maxLength: 6),
            //            NationalDrugCode = c.String(maxLength: 15),
            //            BrandId = c.String(maxLength: 30),
            //            BrandName = c.String(maxLength: 30),
            //            ProdCatNumber = c.String(maxLength: 12),
            //            Size = c.String(maxLength: 12),
            //            CorporateId = c.Int(),
            //            FacilityId = c.Int(),
            //            CreatedBy = c.Int(nullable: false),
            //            CreatedDate = c.DateTime(nullable: false),
            //            ModifiedBy = c.Int(),
            //            ModifiedDate = c.DateTime(),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            DropColumn("dbo.Role", "PortalId");
        }
    }
}
