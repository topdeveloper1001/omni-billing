namespace BillingSystem.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InsurancePlanDB : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.InsurancePlans", "InsuranceCompanyId", c => c.Int(nullable: false));
            AlterColumn("dbo.InsurancePlans", "PlanBeginDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.InsurancePlans", "CreatedBy", c => c.Int(nullable: false));
            AlterColumn("dbo.InsurancePlans", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.InsurancePlans", "IsDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.InsurancePlans", "IsDeleted", c => c.Boolean());
            AlterColumn("dbo.InsurancePlans", "CreatedDate", c => c.DateTime());
            AlterColumn("dbo.InsurancePlans", "CreatedBy", c => c.Int());
            AlterColumn("dbo.InsurancePlans", "PlanBeginDate", c => c.DateTime());
            AlterColumn("dbo.InsurancePlans", "InsuranceCompanyId", c => c.Int());
        }
    }
}
