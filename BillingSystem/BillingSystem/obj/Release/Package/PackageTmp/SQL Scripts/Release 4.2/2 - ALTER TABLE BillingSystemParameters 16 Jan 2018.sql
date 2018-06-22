IF NOT EXISTS(SELECT 1 FROM sys.columns
          WHERE Name = N'DefaultCountry'
          AND Object_ID = Object_ID(N'dbo.BillingSystemParameters'))
Begin
	ALTER TABLE [dbo].[BillingSystemParameters]
	ADD DefaultCountry bigint NULL

	Update [BillingSystemParameters] SET DefaultCountry=0

	ALTER TABLE [dbo].[BillingSystemParameters]
	ALTER COLUMN DefaultCountry bigint NOT NULL
End

--ALTER TABLE [dbo].[BillingSystemParameters]
--	DROP COLUMN DefaultCountry 