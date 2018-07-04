namespace BillingSystem.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFieldsTechnicalSpecifications : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TechnicalSpecifications", "CorporateId", c => c.Int());
            AddColumn("dbo.TechnicalSpecifications", "FacilityId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TechnicalSpecifications", "FacilityId");
            DropColumn("dbo.TechnicalSpecifications", "CorporateId");
        }
    }
}
