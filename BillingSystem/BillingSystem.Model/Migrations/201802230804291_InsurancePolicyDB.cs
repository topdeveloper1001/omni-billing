namespace BillingSystem.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InsurancePolicyDB : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InsurancePolices", "SocialSecurityNumber", c => c.String(maxLength: 15));
            AddColumn("dbo.InsurancePolices", "FacilityId", c => c.Int(nullable: false));
            AddColumn("dbo.InsurancePolices", "CorporateId", c => c.Int(nullable: false));
            AlterColumn("dbo.InsurancePolices", "PlanName", c => c.String(maxLength: 100));
            AlterColumn("dbo.InsurancePolices", "PlanNumber", c => c.String(maxLength: 20));
            AlterColumn("dbo.InsurancePolices", "PolicyName", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.InsurancePolices", "PolicyNumber", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("dbo.InsurancePolices", "PolicyDescription", c => c.String(maxLength: 200));
            AlterColumn("dbo.InsurancePolices", "PolicyHolderName", c => c.String(maxLength: 100));
            AlterColumn("dbo.InsurancePolices", "InsuranceCompanyId", c => c.Int(nullable: false));
            AlterColumn("dbo.InsurancePolices", "McContractCode", c => c.String(maxLength: 15));
            AlterColumn("dbo.InsurancePolices", "CreatedBy", c => c.Int(nullable: false));
            AlterColumn("dbo.InsurancePolices", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.InsurancePolices", "IsDeleted", c => c.Boolean(nullable: false));
            AlterColumn("dbo.InsurancePolices", "InsurancePlanId", c => c.Int(nullable: false));
            DropColumn("dbo.InsurancePolices", "EmiratesIDNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.InsurancePolices", "EmiratesIDNumber", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.InsurancePolices", "InsurancePlanId", c => c.Int());
            AlterColumn("dbo.InsurancePolices", "IsDeleted", c => c.Boolean());
            AlterColumn("dbo.InsurancePolices", "CreatedDate", c => c.DateTime());
            AlterColumn("dbo.InsurancePolices", "CreatedBy", c => c.Int());
            AlterColumn("dbo.InsurancePolices", "McContractCode", c => c.String());
            AlterColumn("dbo.InsurancePolices", "InsuranceCompanyId", c => c.Int());
            AlterColumn("dbo.InsurancePolices", "PolicyHolderName", c => c.String());
            AlterColumn("dbo.InsurancePolices", "PolicyDescription", c => c.String());
            AlterColumn("dbo.InsurancePolices", "PolicyNumber", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.InsurancePolices", "PolicyName", c => c.String());
            AlterColumn("dbo.InsurancePolices", "PlanNumber", c => c.String());
            AlterColumn("dbo.InsurancePolices", "PlanName", c => c.String());
            DropColumn("dbo.InsurancePolices", "CorporateId");
            DropColumn("dbo.InsurancePolices", "FacilityId");
            DropColumn("dbo.InsurancePolices", "SocialSecurityNumber");
        }
    }
}
