namespace BillingSystem.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangesInRoleTabs : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RoleTabs", "PortalKey", c => c.Int(nullable: false));
            AddColumn("dbo.RoleTabs", "CreatedBy", c => c.Int(nullable: false));
            AddColumn("dbo.RoleTabs", "CreatedDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RoleTabs", "CreatedDate");
            DropColumn("dbo.RoleTabs", "CreatedBy");
            DropColumn("dbo.RoleTabs", "PortalKey");
        }
    }
}
