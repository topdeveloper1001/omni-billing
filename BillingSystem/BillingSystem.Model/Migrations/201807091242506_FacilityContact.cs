namespace BillingSystem.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FacilityContact : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FacilityContact",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ContactName = c.String(maxLength: 100),
                        Email = c.String(maxLength: 100),
                        FacilityId = c.Int(nullable: false),
                        IsMain = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FacilityContact");
        }
    }
}
