namespace BillingSystem.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MCContractDB1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.MCContract", "BCCreatedBy", c => c.Int(nullable: false));
            AlterColumn("dbo.MCContract", "BCCreatedDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.MCContract", "BCCreatedDate", c => c.DateTime());
            AlterColumn("dbo.MCContract", "BCCreatedBy", c => c.Int());
        }
    }
}
