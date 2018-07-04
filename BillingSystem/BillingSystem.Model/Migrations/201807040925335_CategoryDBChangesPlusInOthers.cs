namespace BillingSystem.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CategoryDBChangesPlusInOthers : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Categories");
            AlterColumn("dbo.Categories", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.Categories", "CreatedBy", c => c.Int(nullable: false));
            AlterColumn("dbo.Categories", "CreatedDate", c => c.DateTime(nullable: false));
            AddPrimaryKey("dbo.Categories", "Id");
            DropTable("dbo.DashboardIndicatorData_BKUP");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.DashboardIndicatorData_BKUP",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        IndicatorId = c.Int(),
                        IndicatorNumber = c.String(),
                        SubCategory1 = c.String(),
                        SubCategory2 = c.String(),
                        StatisticData = c.String(),
                        Month = c.Int(),
                        Year = c.String(),
                        FacilityId = c.Int(),
                        CorporateId = c.Int(),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        ExternalValue1 = c.String(),
                        ExternalValue2 = c.String(),
                        ExternalValue3 = c.String(),
                        IsActive = c.Boolean(),
                        DepartmentNumber = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            DropPrimaryKey("dbo.Categories");
            AlterColumn("dbo.Categories", "CreatedDate", c => c.DateTime());
            AlterColumn("dbo.Categories", "CreatedBy", c => c.Int());
            AlterColumn("dbo.Categories", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Categories", "Id");
        }
    }
}
