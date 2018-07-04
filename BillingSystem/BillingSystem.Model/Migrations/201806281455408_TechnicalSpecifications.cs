namespace BillingSystem.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TechnicalSpecifications : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TechnicalSpecifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemID = c.Long(nullable: false),
                        TechSpec = c.String(maxLength: 120),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.Int(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TechnicalSpecifications");
        }
    }
}
