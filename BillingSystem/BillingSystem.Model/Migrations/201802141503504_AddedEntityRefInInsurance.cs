namespace BillingSystem.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedEntityRefInInsurance : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InsuranceCompany", "TPAId", c => c.String(maxLength: 10));
            AddColumn("dbo.InsuranceCompany", "RemittancePayerId", c => c.Int(nullable: false));
            AddColumn("dbo.InsuranceCompany", "ClaimSubmission", c => c.String(maxLength: 50));
            AddColumn("dbo.InsuranceCompany", "BillFileType", c => c.String(maxLength: 50));
            AddColumn("dbo.InsuranceCompany", "ANSIPayer", c => c.String(maxLength: 50));
            AddColumn("dbo.InsuranceCompany", "Capitation", c => c.Boolean(nullable: false));
            AddColumn("dbo.InsuranceCompany", "FacilityId", c => c.Int(nullable: false));
            AddColumn("dbo.InsuranceCompany", "CorporateId", c => c.Int(nullable: false));
            AlterColumn("dbo.InsuranceCompany", "CreatedBy", c => c.Int(nullable: false));
            AlterColumn("dbo.InsuranceCompany", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.InsuranceCompany", "IsDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.InsuranceCompany", "IsDeleted", c => c.Boolean());
            AlterColumn("dbo.InsuranceCompany", "CreatedDate", c => c.DateTime());
            AlterColumn("dbo.InsuranceCompany", "CreatedBy", c => c.Int());
            DropColumn("dbo.InsuranceCompany", "CorporateId");
            DropColumn("dbo.InsuranceCompany", "FacilityId");
            DropColumn("dbo.InsuranceCompany", "Capitation");
            DropColumn("dbo.InsuranceCompany", "ANSIPayer");
            DropColumn("dbo.InsuranceCompany", "BillFileType");
            DropColumn("dbo.InsuranceCompany", "ClaimSubmission");
            DropColumn("dbo.InsuranceCompany", "RemittancePayerId");
            DropColumn("dbo.InsuranceCompany", "TPAId");
        }
    }
}
