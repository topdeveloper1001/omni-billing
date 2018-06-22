namespace BillingSystem.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MCContractDB : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MCContract", "CorporateId", c => c.Int(nullable: false));
            AddColumn("dbo.MCContract", "FacilityId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MCContract", "FacilityId");
            DropColumn("dbo.MCContract", "CorporateId");
        }
    }
}
