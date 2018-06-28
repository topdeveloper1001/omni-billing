namespace BillingSystem.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Categories : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProdCatNumber = c.String(maxLength: 12),
                        ProdCat = c.String(maxLength: 40),
                        ProdSubcat = c.String(maxLength: 40),
                        ProdSubcat2 = c.String(maxLength: 40),
                        ProdSubcat3 = c.String(maxLength: 40),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.Int(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Categories");
        }
    }
}
