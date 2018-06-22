namespace BillingSystem.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class PlaceOfServiceAndBillingModifier : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BillingModifier",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    Name = c.String(maxLength: 200),
                    Code = c.String(maxLength: 10),
                    Description = c.String(maxLength: 1000),
                    Type = c.String(maxLength: 50),
                    IsFirst = c.Boolean(nullable: false),
                    EffectiveStartDate = c.DateTime(),
                    EffectiveEndDate = c.DateTime(),
                    ExtValue1 = c.String(maxLength: 50),
                    IsActive = c.Boolean(nullable: false),
                    FacilityId = c.Long(nullable: false),
                    CorporateId = c.Long(nullable: false),
                    CreatedBy = c.Long(nullable: false),
                    CreatedDate = c.DateTime(nullable: false),
                    ModifiedBy = c.Long(),
                    ModifiedDate = c.DateTime(),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.PlaceOfService",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    Name = c.String(maxLength: 200),
                    Code = c.String(maxLength: 10),
                    Description = c.String(maxLength: 1000),
                    EffectiveStartDate = c.DateTime(),
                    EffectiveEndDate = c.DateTime(),
                    ExtValue1 = c.String(maxLength: 50),
                    IsActive = c.Boolean(nullable: false),
                    FacilityId = c.Long(nullable: false),
                    CorporateId = c.Long(nullable: false),
                    CreatedBy = c.Long(nullable: false),
                    CreatedDate = c.DateTime(nullable: false),
                    ModifiedBy = c.Long(),
                    ModifiedDate = c.DateTime(),
                })
                .PrimaryKey(t => t.Id);

            AlterColumn("dbo.InsurancePolices", "PlanName", c => c.String(maxLength: 100));
        }

        public override void Down()
        {
            AlterColumn("dbo.InsurancePolices", "PlanName", c => c.String(nullable: false, maxLength: 100));
            DropTable("dbo.PlaceOfService");
            DropTable("dbo.BillingModifier");
        }
    }
}
